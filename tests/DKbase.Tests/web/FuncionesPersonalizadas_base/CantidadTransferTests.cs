using System;
using DKbase.Tests.TestInfra;
using DKbase.web;
using DKbase.web.capaDatos;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.FuncionesPersonalizadas
{
    /// <summary>
    /// Reparto de la cantidad pedida entre carrito común y carrito transfer.
    /// El resultado es [cantidadComun, cantidadTransfer].
    /// </summary>
    public class CargarProductoCantidadDependiendoTransferTests : TestBase
    {
        private static cProductosGenerico ProductoTransfer(Action<cProductosGenerico> reglas)
            => Crear.ProductoGenerico(p =>
            {
                p.isProductoFacturacionDirecta = true;
                p.PrecioFinal = 1000m;          // precio común más caro...
                p.PrecioFinalTransfer = 800m;   // ...que el transfer → conviene transfer
                reglas(p);
            });

        private static int[] Repartir(cProductosGenerico p, int cantidad, cClientes cliente = null)
            => FuncionesPersonalizadas_base.CargarProductoCantidadDependiendoTransfer(cliente ?? Crear.Cliente(), p, cantidad).ToArray();

        [Fact]
        public void deberia_mandar_todo_al_carrito_comun_cuando_producto_no_es_facturacion_directa()
        {
            var p = Crear.ProductoGenerico(x => { x.isProductoFacturacionDirecta = false; x.tde_fijuni = 1; });

            Repartir(p, 7).Should().Equal(7, 0);
        }

        [Theory]
        [InlineData(800)]   // igual al transfer: no conviene
        [InlineData(900)]   // transfer más caro
        public void deberia_mandar_todo_al_carrito_comun_cuando_transfer_no_es_mas_barato(int precioTransfer)
        {
            var p = ProductoTransfer(x => { x.PrecioFinal = 800m; x.PrecioFinalTransfer = precioTransfer; x.tde_fijuni = 1; });

            Repartir(p, 7).Should().Equal(7, 0);
        }

        [Fact]
        public void deberia_comparar_contra_precio_con_oferta_cuando_la_cantidad_alcanza_la_oferta()
        {
            // Con 10 unidades la oferta (25 %) deja el precio en 750, menor que el transfer (800).
            var p = ProductoTransfer(x => { x.tde_fijuni = 1; x.pro_ofeunidades = 10; x.pro_ofeporcentaje = 25; });

            Repartir(p, 9).Should().Equal(8, 1);   // 9 no alcanza la oferta → conviene transfer
            Repartir(p, 10).Should().Equal(10, 0); // 10 alcanza la oferta → todo común
        }

        // ----- Unidad múltiplo + bonificadas (M=10, B=2 → bloque de 12) -----
        [Theory]
        [InlineData(9, 9, 0)]       // menos que el múltiplo → todo común
        [InlineData(0, 0, 0)]
        [InlineData(-3, -3, 0)]
        [InlineData(10, 0, 12)]     // dentro de [M, M+B] → un bloque completo de transfer
        [InlineData(11, 0, 12)]
        [InlineData(12, 0, 12)]
        [InlineData(13, 1, 12)]     // pasa el bloque: 1 bloque + 1 común
        [InlineData(19, 7, 12)]
        [InlineData(20, 0, 24)]     // 2 bloques (24) > pedido → común 0, se mandan más unidades que las pedidas
        [InlineData(24, 0, 24)]
        [InlineData(25, 1, 24)]
        public void deberia_repartir_en_bloques_cuando_hay_unidad_multiplo_y_bonificadas(int cantidad, int esperadoComun, int esperadoTransfer)
        {
            var p = ProductoTransfer(x => { x.tde_muluni = 10; x.tde_unidadesbonificadas = 2; });

            Repartir(p, cantidad).Should().Equal(esperadoComun, esperadoTransfer);
        }

        [Fact]
        public void deberia_usar_el_primer_bloque_que_cubre_el_pedido_cuando_bonificadas_son_grandes()
        {
            // M=10, B=10 (bloque 20). Pedido 35 → truncado 3 bloques (60), pero el bucle encuentra 2 bloques = 40 >= 35.
            var p = ProductoTransfer(x => { x.tde_muluni = 10; x.tde_unidadesbonificadas = 10; });

            Repartir(p, 35).Should().Equal(0, 40);
        }

        [Fact]
        public void deberia_repartir_resto_al_comun_cuando_bonificadas_es_cero()
        {
            var p = ProductoTransfer(x => { x.tde_muluni = 10; x.tde_unidadesbonificadas = 0; });

            Repartir(p, 25).Should().Equal(5, 20);
        }

        [Fact]
        public void deberia_lanzar_OverflowException_cuando_unidad_multiplo_es_cero()
        {
            // CARACTERIZACIÓN: posible bug - tde_muluni = 0 divide por cero (Infinity) y Convert.ToInt32 lanza OverflowException sin manejar.
            var p = ProductoTransfer(x => { x.tde_muluni = 0; x.tde_unidadesbonificadas = 0; });

            var act = () => Repartir(p, 5);

            act.Should().Throw<OverflowException>();
        }

        // ----- Unidad fija (F=6) -----
        [Theory]
        [InlineData(5, 5, 0)]
        [InlineData(6, 0, 6)]
        [InlineData(7, 1, 6)]
        [InlineData(100, 94, 6)]
        [InlineData(0, 0, 0)]
        public void deberia_mandar_unidad_fija_al_transfer_y_resto_al_comun_cuando_hay_unidad_fija(int cantidad, int comun, int transfer)
        {
            var p = ProductoTransfer(x => x.tde_fijuni = 6);

            Repartir(p, cantidad).Should().Equal(comun, transfer);
        }

        // ----- Mínimo y máximo (3..8) -----
        [Theory]
        [InlineData(2, 2, 0)]
        [InlineData(3, 0, 3)]
        [InlineData(5, 0, 5)]
        [InlineData(8, 0, 8)]
        [InlineData(9, 1, 8)]
        [InlineData(20, 12, 8)]
        public void deberia_respetar_rango_cuando_hay_unidad_minima_y_maxima(int cantidad, int comun, int transfer)
        {
            var p = ProductoTransfer(x => { x.tde_minuni = 3; x.tde_maxuni = 8; });

            Repartir(p, cantidad).Should().Equal(comun, transfer);
        }

        // ----- Solo mínimo (4) -----
        [Theory]
        [InlineData(3, 3, 0)]
        [InlineData(4, 0, 4)]
        [InlineData(1000, 0, 1000)]
        public void deberia_mandar_todo_al_transfer_cuando_supera_unidad_minima(int cantidad, int comun, int transfer)
        {
            var p = ProductoTransfer(x => x.tde_minuni = 4);

            Repartir(p, cantidad).Should().Equal(comun, transfer);
        }

        [Fact]
        public void deberia_priorizar_multiplo_sobre_unidad_fija_cuando_hay_varias_reglas()
        {
            var p = ProductoTransfer(x => { x.tde_muluni = 10; x.tde_unidadesbonificadas = 2; x.tde_fijuni = 1; x.tde_minuni = 1; x.tde_maxuni = 2; });

            Repartir(p, 10).Should().Equal(0, 12);
        }

        [Fact]
        public void deberia_ignorar_multiplo_sin_bonificadas_y_usar_unidad_fija()
        {
            var p = ProductoTransfer(x => { x.tde_muluni = 10; x.tde_fijuni = 3; });

            Repartir(p, 10).Should().Equal(7, 3);
        }

        [Fact]
        public void deberia_ignorar_maximo_sin_minimo()
        {
            // Solo tde_maxuni no es una regla reconocida.
            var p = ProductoTransfer(x => x.tde_maxuni = 5);

            Repartir(p, 3).Should().Equal(0, 0);
        }

        [Fact]
        public void deberia_devolver_cero_y_cero_cuando_conviene_transfer_pero_no_hay_reglas()
        {
            // CARACTERIZACIÓN: posible bug - facturación directa, transfer más barato y sin reglas de unidades:
            // no se marca "pasar directamente" y la cantidad pedida se pierde ([0, 0]).
            var p = ProductoTransfer(_ => { });

            Repartir(p, 5).Should().Equal(0, 0);
        }

        [Fact]
        public void deberia_lanzar_NullReferenceException_cuando_cliente_es_null_y_es_facturacion_directa()
        {
            var p = ProductoTransfer(x => x.tde_fijuni = 1);

            var act = () => FuncionesPersonalizadas_base.CargarProductoCantidadDependiendoTransfer(null, p, 1);

            act.Should().Throw<NullReferenceException>();
        }
    }
}
