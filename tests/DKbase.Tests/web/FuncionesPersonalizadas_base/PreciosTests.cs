using System.Globalization;
using DKbase.Tests.TestInfra;
using DKbase.web;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.FuncionesPersonalizadas
{
    /// <summary>Cálculo de precio final de un producto según tipo y descuentos del cliente.</summary>
    public class ObtenerPrecioFinalTests : TestBase
    {
        private static decimal D(string s) => decimal.Parse(s, CultureInfo.InvariantCulture);

        // Cliente por defecto: betmed=10, netos=5, directo=20. Precio base 1000.
        [Theory]
        [InlineData(true, "M", "0", "900")]       // neto medicamento → cli_pordesbetmed
        [InlineData(true, "P", "0", "950")]       // neto perfumería → cli_pordesnetos
        [InlineData(true, "A", "0", "950")]
        [InlineData(true, "V", "0", "950")]
        [InlineData(false, "M", "0", "800")]      // no neto → descuento directo
        [InlineData(false, "P", "0", "800")]      // no neto ignora el tipo
        [InlineData(false, null, "0", "800")]
        [InlineData(true, "M", "9", "990")]       // porcentaje a restar reduce el descuento (10-9=1%)
        [InlineData(true, "M", "10", "1000")]     // justo en el umbral: descuento 0
        [InlineData(true, "M", "11", "1000")]     // descuento negativo se lleva a 0
        [InlineData(false, "M", "20", "1000")]
        [InlineData(false, "M", "19.5", "995")]
        public void deberia_aplicar_descuento_segun_tipo_cuando_se_calcula_precio_final(bool neto, string tipo, string porcARestar, string esperado)
        {
            var cliente = Crear.Cliente();
            var producto = Crear.Producto(p => { p.pro_neto = neto; p.pro_codtpopro = tipo; p.pro_PorcARestarDelDtoDeCliente = D(porcARestar); });

            var precio = FuncionesPersonalizadas_base.ObtenerPrecioFinal(cliente, producto);

            precio.Should().Be(D(esperado));
        }

        [Theory]
        [InlineData("X")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("m")]
        public void deberia_devolver_cero_cuando_producto_neto_tiene_tipo_desconocido(string tipo)
        {
            // CARACTERIZACIÓN: posible bug - un producto neto con tipo distinto de M/P/A/V queda con precio final 0.
            var producto = Crear.Producto(p => { p.pro_neto = true; p.pro_codtpopro = tipo; });

            var precio = FuncionesPersonalizadas_base.ObtenerPrecioFinal(Crear.Cliente(), producto);

            precio.Should().Be(0m);
        }

        [Fact]
        public void deberia_devolver_precio_negativo_cuando_descuento_supera_100()
        {
            // CARACTERIZACIÓN: posible bug - no se acota el descuento a 100 %, el precio puede quedar negativo.
            var cliente = Crear.Cliente(c => c.cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto = 150m);

            var precio = FuncionesPersonalizadas_base.ObtenerPrecioFinal(cliente, Crear.Producto());

            precio.Should().Be(-500m);
        }

        [Theory]
        [InlineData("0", "0")]
        [InlineData("0.01", "0.008")]
        [InlineData("-1000", "-800")]
        [InlineData("79228162514264337593543950.335", "63382530011411470074835160.268")]
        public void deberia_respetar_decimales_y_limites_cuando_precio_base_es_extremo(string precioBase, string esperado)
        {
            var producto = Crear.Producto(p => p.pro_PrecioBase = D(precioBase));

            var precio = FuncionesPersonalizadas_base.ObtenerPrecioFinal(Crear.Cliente(), producto);

            precio.Should().Be(D(esperado));
        }

        [Fact]
        public void deberia_lanzar_NullReferenceException_cuando_cliente_es_null()
        {
            var act = () => FuncionesPersonalizadas_base.ObtenerPrecioFinal(null, Crear.Producto());

            act.Should().Throw<System.NullReferenceException>();
        }
    }

    public class ObtenerPrecioFinalTransferBaseTests : TestBase
    {
        private static decimal D(string s) => decimal.Parse(s, CultureInfo.InvariantCulture);

        // Cliente: betmed=10, netos=5, directo=20. Pre-descuento 500, precio con descuento directo 1000.
        [Theory]
        [InlineData(false, true, "M", "0", "500")]     // transfer no deshabilitado → tde_predescuento
        [InlineData(false, false, "M", "0", "500")]
        [InlineData(true, true, "M", "0", "900")]      // deshab + neto M → betmed sobre precio directo
        [InlineData(true, true, "P", "0", "950")]
        [InlineData(true, true, "A", "0", "950")]
        [InlineData(true, true, "V", "0", "950")]
        [InlineData(true, true, "X", "0", "500")]      // tipo desconocido → se mantiene predescuento
        [InlineData(true, true, null, "0", "500")]
        [InlineData(true, false, "M", "0", "800")]     // deshab + no neto → descuento directo
        [InlineData(true, false, "M", "5", "850")]
        [InlineData(true, false, "M", "25", "1000")]   // descuento negativo → 0
        public void deberia_calcular_precio_transfer_segun_rama_cuando_no_hay_descuento_adicional(bool deshab, bool neto, string tipo, string porcARestar, string esperado)
        {
            var precio = FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(Crear.Cliente(), deshab, null, neto, tipo, 99m, 500m, 1000m, D(porcARestar));

            precio.Should().Be(D(esperado));
        }

        [Theory]
        [InlineData(false, "10", "450")]
        [InlineData(false, "0", "500")]
        [InlineData(false, "100", "0")]
        [InlineData(false, "-10", "550")]
        [InlineData(true, "50", "400")]     // se aplica después del descuento del cliente (800 * 0.5)
        public void deberia_aplicar_descuento_adicional_cuando_tfr_pordesadi_tiene_valor(bool deshab, string pordesadi, string esperado)
        {
            var precio = FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(Crear.Cliente(), deshab, D(pordesadi), false, "M", 0m, 500m, 1000m, 0m);

            precio.Should().Be(D(esperado));
        }

        [Fact]
        public void deberia_ignorar_descuento_web_cuando_calcula_precio_transfer()
        {
            var sinDescuentoWeb = FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(Crear.Cliente(), true, null, false, "M", 0m, 500m, 1000m, 0m);
            var conDescuentoWeb = FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(Crear.Cliente(), true, null, false, "M", 75m, 500m, 1000m, 0m);

            conDescuentoWeb.Should().Be(sinDescuentoWeb);
        }

        [Fact]
        public void deberia_no_requerir_cliente_cuando_transfer_no_esta_deshabilitado()
        {
            var precio = FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(null, false, null, true, "M", 0m, 123.45m, 1000m, 0m);

            precio.Should().Be(123.45m);
        }
    }

    public class ObtenerPrecioUnitarioConDescuentoOfertaTests : TestBase
    {
        [Theory]
        [InlineData(0, 0, 1000)]
        [InlineData(5, 0, 1000)]     // sin porcentaje → sin oferta
        [InlineData(0, 10, 1000)]    // sin unidades → sin oferta
        [InlineData(5, 10, 900)]
        [InlineData(1, 100, 0)]
        [InlineData(1, 1, 990)]
        [InlineData(-1, 10, 900)]    // unidades negativas cuentan como oferta
        public void deberia_aplicar_porcentaje_oferta_solo_cuando_unidades_y_porcentaje_son_distintos_de_cero(int unidades, int porcentaje, int esperado)
        {
            var producto = Crear.Producto(p => { p.pro_ofeunidades = unidades; p.pro_ofeporcentaje = porcentaje; });

            var precio = FuncionesPersonalizadas_base.ObtenerPrecioUnitarioConDescuentoOferta(1000m, producto);

            precio.Should().Be(esperado);
        }
    }

    public class ObtenerPrecioUnitarioConDescuentoOfertaSiLlegaConLaCantidadTests : TestBase
    {
        [Theory]
        [InlineData(true, 5, 10, 4, 1000)]   // justo antes del umbral
        [InlineData(true, 5, 10, 5, 900)]    // en el umbral
        [InlineData(true, 5, 10, 6, 900)]    // después del umbral
        [InlineData(true, 5, 10, 0, 1000)]
        [InlineData(true, 5, 10, -1, 1000)]
        [InlineData(true, 0, 10, 100, 1000)] // sin unidades de oferta
        [InlineData(true, 5, 0, 100, 1000)]  // sin porcentaje
        [InlineData(false, 5, 10, 100, 1000)] // cliente que no toma ofertas
        public void deberia_aplicar_oferta_solo_cuando_cliente_toma_ofertas_y_alcanza_cantidad(bool tomaOfertas, int unidades, int porcentaje, int cantidad, int esperado)
        {
            var cliente = Crear.Cliente(c => c.cli_tomaOfertas = tomaOfertas);
            var producto = Crear.ProductoGenerico(p => { p.PrecioFinal = 1000m; p.pro_ofeunidades = unidades; p.pro_ofeporcentaje = porcentaje; });

            var precio = FuncionesPersonalizadas_base.ObtenerPrecioUnitarioConDescuentoOfertaSiLlegaConLaCantidad(cliente, producto, cantidad);

            precio.Should().Be(esperado);
        }
    }
}
