using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DKbase.generales;
using DKbase.Tests.TestInfra;
using DKbase.web.capaDatos;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.capaDatos
{
    /// <summary>
    /// Armado de carritos y del resultado del buscador/subida de archivos en capaCAR_WebService_base
    /// (mapeo DataSet → objetos, productos no encontrados, facturación directa y reglas de perfumería).
    /// </summary>
    public class BuscadorYCarritosTests : TestBase
    {
        private const string SpCarritos = "CAR.spRecuperarCarritosPorSucursalYProductos";
        private const string SpArchivo = "LogRegistro.spCargarCarritoProductoSucursalDesdeArchivoPedidosV5";

        /// <summary>Tabla con todas las columnas conocidas y una fila por producto (valores universales salvo lo indicado).</summary>
        private static DataTable Filas(int cantidad, Action<DataRow, int> completar, params string[] columnasExtra)
        {
            var t = FakeDb.TablaUniversal();
            foreach (var c in columnasExtra) if (!t.Columns.Contains(c)) t.Columns.Add(c, typeof(object));
            var plantilla = t.Rows[0].ItemArray;
            t.Rows.Clear();
            for (int i = 0; i < cantidad; i++)
            {
                var f = t.NewRow();
                f.ItemArray = plantilla;
                completar(f, i);
                t.Rows.Add(f);
            }
            return t;
        }

        // ---------- RecuperarCarritosPorSucursalYProductos_generica ----------
        private void ConCarritos(bool conTransfers)
        {
            var productos = Filas(3, (f, i) =>
            {
                f["cad_codCarrito"] = i < 2 ? 5 : 6;
                f["pro_nombre"] = new[] { "A", "B", "C" }[i];
                f["cad_codProducto"] = "COD" + i;
                f["cad_cantidad"] = i + 1;
                f["stk_stock"] = i == 1 ? (object)DBNull.Value : "S";
                f["pro_neto"] = false;
            }, "cad_codCarrito");
            var carritos = Tabla.De(new { car_id = 5, car_codSucursal = "CC" }, new { car_id = 6, car_codSucursal = "RO" }, new { car_id = 7, car_codSucursal = "SF" });
            var r = FakeDb.Cuando(SpCarritos).Devolver(productos, carritos);
            if (conTransfers)
                r.Devolver(Filas(1, (f, i) => { f["tde_codpro"] = "A"; f["tde_predescuento"] = DBNull.Value; }));
        }

        [Fact]
        public void deberia_agrupar_productos_por_carrito_y_descartar_carritos_vacios()
        {
            ConCarritos(conTransfers: false);

            var r = capaCAR_WebService_base.RecuperarCarritosPorSucursalYProductos_generica(Crear.Cliente(), "C");

            var l = FakeDb.Unica(SpCarritos);
            l.Param("@car_codCliente").Should().Be(1234);
            l.Param("@car_tipo").Should().Be("C");
            r.Select(c => (c.car_id, c.lrc_id, c.codSucursal)).Should().Equal((5, 5, "CC"), (6, 6, "RO"));
            r[0].listaProductos.Select(p => (p.pro_nombre, p.codProducto, p.cantidad, p.stk_stock)).Should().Equal(("A", "COD0", 1, "S"), ("B", "COD1", 2, null));
            r[1].listaProductos.Select(p => p.pro_nombre).Should().Equal("C");
            r.SelectMany(c => c.listaProductos).Should().OnlyContain(p => !p.isProductoFacturacionDirecta && p.PrecioFinal != 0);
        }

        [Fact]
        public void deberia_marcar_facturacion_directa_y_calcular_precio_transfer_cuando_el_producto_tiene_transfer()
        {
            ConCarritos(conTransfers: true);

            var r = capaCAR_WebService_base.RecuperarCarritosPorSucursalYProductos_generica(Crear.Cliente(), "C");

            var todos = r.SelectMany(c => c.listaProductos).ToList();
            todos.Where(p => p.isProductoFacturacionDirecta).Select(p => p.pro_nombre).Should().Equal("A");
            var a = todos.Single(p => p.pro_nombre == "A");
            a.tde_codpro.Should().Be("A");
            a.tde_predescuento.Should().BeNull();
            a.PrecioFinalTransfer.Should().Be(DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(
                Crear.Cliente(), a.tfr_deshab, a.tfr_pordesadi, a.pro_neto, a.pro_codtpopro, a.pro_descuentoweb, 0, a.tde_PrecioConDescuentoDirecto, a.tde_PorcARestarDelDtoDeCliente));
            a.PrecioFinalTransfer.Should().NotBe(0);
            todos.Single(p => p.pro_nombre == "B").PrecioFinalTransfer.Should().Be(0);
        }

        [Fact]
        public void deberia_lanzar_excepcion_cuando_la_base_no_devuelve_la_tabla_de_carritos()
        {
            // CARACTERIZACIÓN: posible bug - con la base vacía o caída se accede a Tables[1] sin validar y se lanza excepción.
            FluentActions.Invoking(() => capaCAR_WebService_base.RecuperarCarritosPorSucursalYProductos_generica(Crear.Cliente(), "C"))
                .Should().Throw<Exception>();
        }

        // ---------- ConvertToProductoBuscadorV5 / AgregarProductoAlCarritoDesdeArchivoPedidosV5 ----------
        private static DataTable NoEncontrados(params object[] filas) => Tabla.De(filas);

        private static DataSet DataSetBuscador(DataTable noEncontrados) => new DataSet
        {
            Tables = { new DataTable("productos"), new DataTable("stocks"), noEncontrados, new DataTable("transfers") },
        };

        public static IEnumerable<object[]> CasosNoEncontrados() => new[]
        {
            new object[] { "IBU", "779", "T1", "AB", "<b> Código Barra: </b>779<b> Troquel: </b>T1<b> Código Alfabeta: </b>AB" },
            new object[] { "IBU", "", "", "", "<b> Código producto: </b>IBU" },
            new object[] { "IBU", null, null, null, "<b> Código producto: </b>IBU" },
            new object[] { null, "779", null, null, "<b> Código Barra: </b>779" },
            new object[] { null, "", "T1", null, "<b> Troquel: </b>T1" },
            new object[] { null, "", null, "AB", "<b> Código Alfabeta: </b>AB" },
            new object[] { null, "", null, null, null },
        };

        [Theory]
        [MemberData(nameof(CasosNoEncontrados))]
        public void deberia_describir_el_producto_no_encontrado_con_los_codigos_informados(string nombre, string barra, string troquel, string alfabeta, string esperado)
        {
            var ds = DataSetBuscador(NoEncontrados(new { nombreNoEncontrado = (object)nombre, codigobarra = (object)barra, troquel = (object)troquel, codigoalfabeta = (object)alfabeta, cantidad = (object)3, nroordenamiento = (object)9 }));

            var r = capaCAR_WebService_base.ConvertToProductoBuscadorV5(Crear.Cliente(), ds, "CC");

            var p = r.Should().ContainSingle().Subject;
            p.isProductoNoEncontrado.Should().BeTrue();
            p.pro_codigo.Should().Be("-1");
            p.pro_nombre.Should().Be(esperado);
            p.cantidad.Should().Be(3);
            p.nroordenamiento.Should().Be(9);
        }

        [Fact]
        public void deberia_ignorar_filas_sin_nombre_ni_codigo_de_barras()
        {
            var ds = DataSetBuscador(NoEncontrados(new { nombreNoEncontrado = (object)null, codigobarra = (object)null, troquel = (object)"T1", codigoalfabeta = (object)"AB", cantidad = (object)1, nroordenamiento = (object)1 }));

            capaCAR_WebService_base.ConvertToProductoBuscadorV5(Crear.Cliente(), ds, "CC").Should().BeEmpty();
        }

        [Fact]
        public void deberia_tolerar_columnas_opcionales_ausentes_y_nulos_en_cantidad_y_orden()
        {
            var ds = DataSetBuscador(NoEncontrados(new { nombreNoEncontrado = "IBU", codigobarra = (object)null, cantidad = (object)null, nroordenamiento = (object)null }));

            var p = capaCAR_WebService_base.ConvertToProductoBuscadorV5(Crear.Cliente(), ds, "CC").Single();

            p.pro_nombre.Should().Be("<b> Código producto: </b>IBU");
            p.cantidad.Should().Be(0);
            p.nroordenamiento.Should().Be(0);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void deberia_devolver_null_cuando_el_dataset_no_tiene_al_menos_dos_tablas(int tablas)
        {
            var ds = new DataSet();
            for (int i = 0; i < tablas; i++) ds.Tables.Add(new DataTable("t" + i));

            capaCAR_WebService_base.ConvertToProductoBuscadorV5(Crear.Cliente(), ds, "CC").Should().BeNull();
            capaCAR_WebService_base.ConvertToProductoBuscadorV5(Crear.Cliente(), null, "CC").Should().BeNull();
        }

        private void ConArchivo()
        {
            FakeDb.Cuando(SpArchivo).Devolver(new DataTable(), new DataTable(), NoEncontrados(
                new { nombreNoEncontrado = "B", codigobarra = (object)null, nroordenamiento = 2 },
                new { nombreNoEncontrado = "A", codigobarra = (object)null, nroordenamiento = 1 },
                new { nombreNoEncontrado = "C", codigobarra = (object)null, nroordenamiento = 3 }), new DataTable());
        }

        private static List<cProductosGenerico> SubirArchivo(cClientes cliente) =>
            capaCAR_WebService_base.AgregarProductoAlCarritoDesdeArchivoPedidosV5(cliente, "CC", "RO", Tabla.De(new { codProducto = "x" }), "S", 1234, "Santa Fe", true, 77);

        [Fact]
        public void deberia_enviar_el_archivo_y_ordenar_el_resultado_por_nro_de_orden()
        {
            ConArchivo();

            var r = SubirArchivo(Crear.Cliente());

            var l = FakeDb.Unica(SpArchivo);
            (l.Param("@lrc_codSucursal"), l.Param("@Sucursal"), l.Param("@lrc_codCliente"), l.Param("@lcp_codUsuario"), l.Param("@TipoDeArchivo"), l.Param("@cli_codprov"), l.Param("@cli_isGLN"))
                .Should().Be(("CC", "RO", 1234, 77, "S", "Santa Fe", true));
            r.Select(p => p.pro_nombre).Should().Equal("<b> Código producto: </b>A", "<b> Código producto: </b>B", "<b> Código producto: </b>C");
        }

        [Theory]
        [InlineData("PERFUMERIA", true, false)]  // perfumería: solo puede pedir perfumería (los no encontrados no tienen tipo)
        [InlineData("TODOS", true, true)]        // todos y toma perfumería: puede pedir todo
        [InlineData("TODOS", false, true)]       // todos sin perfumería: se bloquea solo perfumería (los no encontrados no tienen tipo)
        [InlineData("OTRO", false, true)]        // otro tipo: sin reglas
        public void deberia_aplicar_reglas_de_perfumeria_segun_el_tipo_de_cliente(string tipo, bool tomaPerfumeria, bool permitido)
        {
            ConArchivo();
            var codigo = tipo switch { "PERFUMERIA" => Constantes.cTipoCliente_Perfumeria, "TODOS" => Constantes.cTipoCliente_Todos, _ => "?" };
            var cliente = Crear.Cliente(c => { c.cli_tipo = codigo; c.cli_tomaPerfumeria = tomaPerfumeria; });

            SubirArchivo(cliente).Should().OnlyContain(p => p.isPermitirPedirProducto == permitido);
        }

        [Fact]
        public void deberia_devolver_null_cuando_la_subida_falla_en_la_base()
        {
            FakeDb.ErrorAlConectar = new InvalidOperationException("caída");

            SubirArchivo(Crear.Cliente()).Should().BeNull();
        }
    }
}
