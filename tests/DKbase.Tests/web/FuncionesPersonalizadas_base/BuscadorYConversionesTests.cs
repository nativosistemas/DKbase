using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DKbase.Tests.TestInfra;
using DKbase.web;
using DKbase.web.capaDatos;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.FuncionesPersonalizadas
{
    public class GenerarWhereLikeTests : TestBase
    {
        private const string C = " collate SQL_Latin1_General_Cp1_CI_AI like ";

        [Theory]
        [InlineData("", "")]
        [InlineData("   ", "")]
        [InlineData("ibu", " pro_nombre" + C + "'%ibu%' ")]
        [InlineData("ibu 400", " pro_nombre" + C + "'%ibu%'  AND  pro_nombre" + C + "'%400%' ")]
        [InlineData("  ibu   400 ", " pro_nombre" + C + "'%ibu%'  AND  pro_nombre" + C + "'%400%' ")]
        [InlineData("ñandú", " pro_nombre" + C + "'%ñandú%' ")]
        public void deberia_generar_un_like_por_palabra_unidos_con_AND(string texto, string esperado)
        {
            FuncionesPersonalizadas_base.GenerarWhereLikeConColumna(texto, "pro_nombre").Should().Be(esperado);
        }

        [Fact]
        public void deberia_concatenar_texto_sin_escapar_cuando_tiene_comillas()
        {
            // CARACTERIZACIÓN: posible bug - inyección SQL: el texto del buscador se concatena sin escapar.
            var where = FuncionesPersonalizadas_base.GenerarWhereLikeConColumna("x' OR 1=1--", "pro_nombre");

            where.Should().Contain("'%x'%'").And.Contain("'%1=1--%'");
        }

        [Fact]
        public void deberia_lanzar_NullReferenceException_cuando_texto_es_null()
        {
            var act = () => FuncionesPersonalizadas_base.GenerarWhereLikeConColumna(null, "c");

            act.Should().Throw<NullReferenceException>();
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("ibu", " pro_nombre" + C + "'ibu%' ")]
        [InlineData("  ibu 400", " pro_nombre" + C + "'ibu%' ")] // solo la primera palabra no vacía
        public void deberia_usar_solo_la_primera_palabra_cuando_empieza_con(string texto, string esperado)
        {
            FuncionesPersonalizadas_base.GenerarWhereLikeConColumna_EmpiezaCon(texto, "pro_nombre").Should().Be(esperado);
        }

        [Fact]
        public void deberia_combinar_columnas_con_OR_y_palabras_con_AND()
        {
            var where = FuncionesPersonalizadas_base.GenerarWhereLikeConVariasColumnas("ibu 400", new List<string> { "a", "b" });

            where.Should().Be(" (  a" + C + "'%ibu%'  OR  b" + C + "'%ibu%'  )  AND (  a" + C + "'%400%'  OR  b" + C + "'%400%'  ) ");
        }

        [Fact]
        public void deberia_generar_parentesis_vacios_cuando_no_hay_columnas()
        {
            FuncionesPersonalizadas_base.GenerarWhereLikeConVariasColumnas("ibu", new List<string>()).Should().Be(" (  ) ");
        }

        [Fact]
        public void deberia_devolver_vacio_cuando_texto_es_vacio_con_varias_columnas()
        {
            FuncionesPersonalizadas_base.GenerarWhereLikeConVariasColumnas(" ", new List<string> { "a" }).Should().BeEmpty();
        }
    }

    public class ConversionesTablaTests : TestBase
    {
        [Fact]
        public void deberia_crear_una_fila_por_producto_con_codigo_y_cantidad()
        {
            var lista = new List<cProductosAndCantidad>
            {
                new cProductosAndCantidad { codProductoNombre = "A", cantidad = 1 },
                new cProductosAndCantidad { codProductoNombre = "B", cantidad = 0 },
            };

            var t = FuncionesPersonalizadas_base.ConvertProductosAndCantidadToDataTable(lista);

            t.Columns.Cast<DataColumn>().Select(c => (c.ColumnName, c.DataType)).Should().Equal(("codProducto", typeof(string)), ("cantidad", typeof(int)));
            t.Rows.Cast<DataRow>().Select(r => ((string)r["codProducto"], (int)r["cantidad"])).Should().Equal(("A", 1), ("B", 0));
        }

        [Fact]
        public void deberia_devolver_tabla_vacia_cuando_lista_de_productos_es_vacia()
        {
            FuncionesPersonalizadas_base.ConvertProductosAndCantidadToDataTable(new List<cProductosAndCantidad>()).Rows.Should().BeEmpty();
        }

        [Fact]
        public void deberia_lanzar_NullReferenceException_cuando_lista_de_productos_es_null()
        {
            var act = () => FuncionesPersonalizadas_base.ConvertProductosAndCantidadToDataTable(null);
            act.Should().Throw<NullReferenceException>();
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData(new string[0], 0)]
        [InlineData(new[] { "CC" }, 1)]
        [InlineData(new[] { "CC", "SF", "CC" }, 3)]
        public void deberia_crear_una_fila_por_nombre_de_seccion(string[] nombres, int filas)
        {
            var t = FuncionesPersonalizadas_base.ConvertNombresSeccionToDataTable(nombres?.ToList());

            t.Columns.Cast<DataColumn>().Single().ColumnName.Should().Be("NombreSeccion");
            t.Rows.Count.Should().Be(filas);
            if (filas > 0) t.Rows.Cast<DataRow>().Select(r => (string)r[0]).Should().Equal(nombres);
        }

        [Fact]
        public void deberia_crear_tabla_de_archivos_de_pedido_con_seis_columnas()
        {
            var t = FuncionesPersonalizadas_base.ObtenerDataTableProductosCarritoArchivosPedidos();

            t.Columns.Cast<DataColumn>().Select(c => $"{c.ColumnName}:{c.DataType.Name}").Should().Equal(
                "codProducto:String", "codigobarra:String", "codigoalfabeta:String", "troquel:String", "cantidad:Int32", "nroordenamiento:Int32");
            t.Rows.Should().BeEmpty();
        }

        [Theory]
        [InlineData("F", "COD", null, null, null)]
        [InlineData("S", null, "BARRA", "ALFA", "TROQ")]
        [InlineData("E", null, "BARRA", null, null)]
        [InlineData("X", null, null, null, null)]
        [InlineData(null, null, null, null, null)]
        public void deberia_completar_columnas_segun_tipo_de_archivo(string tipo, string cod, string barra, string alfa, string troquel)
        {
            var t = FuncionesPersonalizadas_base.ObtenerDataTableProductosCarritoArchivosPedidos();

            var fila = FuncionesPersonalizadas_base.ConvertProductosCarritoArchivosPedidosToDataRow(t, "COD", 7, "BARRA", "ALFA", "TROQ", tipo);

            Valor(fila["codProducto"]).Should().Be(cod);
            Valor(fila["codigobarra"]).Should().Be(barra);
            Valor(fila["codigoalfabeta"]).Should().Be(alfa);
            Valor(fila["troquel"]).Should().Be(troquel);
            fila["cantidad"].Should().Be(tipo is "F" or "S" or "E" ? 7 : DBNull.Value);
            fila["nroordenamiento"].Should().Be(1);
        }

        [Fact]
        public void deberia_numerar_segun_filas_existentes_y_no_agregar_la_fila_a_la_tabla()
        {
            var t = FuncionesPersonalizadas_base.ObtenerDataTableProductosCarritoArchivosPedidos();
            t.Rows.Add(FuncionesPersonalizadas_base.ConvertProductosCarritoArchivosPedidosToDataRow(t, "A", 1, null, null, null, "F"));
            t.Rows.Add(FuncionesPersonalizadas_base.ConvertProductosCarritoArchivosPedidosToDataRow(t, "B", 1, null, null, null, "F"));

            var tercera = FuncionesPersonalizadas_base.ConvertProductosCarritoArchivosPedidosToDataRow(t, "C", 1, null, null, null, "F");

            tercera["nroordenamiento"].Should().Be(3);
            t.Rows.Count.Should().Be(2);
        }

        private static string Valor(object o) => o == DBNull.Value ? null : (string)o;
    }

    public class ConversionesSimplesTests : TestBase
    {
        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(1, 0, true)]
        [InlineData(0, 1, true)]
        [InlineData(3, 10, true)]
        public void deberia_marcar_oferta_cuando_unidades_o_porcentaje_son_distintos_de_cero(int unidades, int porcentaje, bool esOferta)
        {
            var p = Crear.ProductoGenerico(x => { x.cantidad = 4; x.pro_nombre = "NOMBRE"; x.pro_codigo = "COD"; x.pro_ofeunidades = unidades; x.pro_ofeporcentaje = porcentaje; });

            var r = FuncionesPersonalizadas_base.ProductosEnCarrito_ToConvert_DllProductosAndCantidad(p);

            r.cantidad.Should().Be(4);
            r.codProductoNombre.Should().Be("NOMBRE"); // usa el nombre, no el código
            r.isOferta.Should().Be(esOferta);
            r.IdTransfer.Should().Be(0);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("Error 1234-5678: sin stock", "Error : sin stock")]
        [InlineData("0123456789-", "")]
        [InlineData("abc", "abc")]
        [InlineData("–", "–")] // guion largo unicode no se quita
        public void deberia_quitar_digitos_y_guiones_cuando_limpia_error_de_pedido(string entrada, string esperado)
        {
            FuncionesPersonalizadas_base.LimpiarStringErrorPedido(entrada).Should().Be(esperado);
        }

        [Theory]
        [InlineData(1, "Enero")] [InlineData(2, "Febrero")] [InlineData(3, "Marzo")] [InlineData(4, "Abril")]
        [InlineData(5, "Mayo")] [InlineData(6, "Junio")] [InlineData(7, "Julio")] [InlineData(8, "Agosto")]
        [InlineData(9, "Septiembre")] [InlineData(10, "Octubre")] [InlineData(11, "Noviembre")] [InlineData(12, "Diciembre")]
        [InlineData(0, "")] [InlineData(13, "")] [InlineData(-1, "")]
        public void deberia_devolver_nombre_de_mes_cuando_numero_es_valido(int mes, string nombre)
        {
            FuncionesPersonalizadas_base.obtenerNombreMes(mes).Should().Be(nombre);
        }
    }

    public class SucursalesYStockTests : TestBase
    {
        private static void SucursalesDependientes(params (string suc, string dep)[] filas)
            => FakeDb.Cuando("Clientes.spGestionSucursal").Devolver(Tabla.De(filas.Select((f, i) => (object)new { sde_codigo = (int?)(i + 1), sde_sucursal = f.suc, sde_sucursalDependiente = f.dep }).ToArray()));

        [Fact]
        public void deberia_devolver_sucursal_SF_y_CC_sin_consultar_base_cuando_reparto_es_S7()
        {
            var r = FuncionesPersonalizadas_base.RecuperarSucursalesParaBuscadorDeCliente(Crear.Cliente(c => { c.cli_codsuc = "SF"; c.cli_codrep = "S7"; }));

            r.Should().Equal("SF", "CC");
            FakeDb.Llamadas.Should().BeEmpty();
        }

        [Fact]
        public void deberia_agregar_CC_y_SF_cuando_reparto_S7_tiene_otra_sucursal()
        {
            FuncionesPersonalizadas_base.RecuperarSucursalesParaBuscadorDeCliente(Crear.Cliente(c => { c.cli_codsuc = "RO"; c.cli_codrep = "S7"; }))
                .Should().Equal("RO", "SF", "CC");
        }

        [Fact]
        public void deberia_agregar_dependientes_y_alternativa_sin_repetir_cuando_no_es_S7()
        {
            SucursalesDependientes(("CC", "CC"), ("CC", "SF"), ("CC", "RO"), ("SF", "XX"));

            var r = FuncionesPersonalizadas_base.RecuperarSucursalesParaBuscadorDeCliente(Crear.Cliente(c => { c.cli_codsuc = "CC"; c.cli_IdSucursalAlternativa = "SF"; }));

            r.Should().Equal("CC", "SF", "RO");
        }

        [Fact]
        public void deberia_agregar_sucursal_alternativa_cuando_no_esta_en_dependientes()
        {
            SucursalesDependientes(("CC", "SF"));

            FuncionesPersonalizadas_base.RecuperarSucursalesParaBuscadorDeCliente(Crear.Cliente(c => c.cli_IdSucursalAlternativa = "VT"))
                .Should().Equal("CC", "SF", "VT");
        }

        [Fact]
        public void deberia_lanzar_ArgumentNullException_cuando_falla_consulta_de_dependientes()
        {
            FakeDb.Cuando("Clientes.spGestionSucursal").Fallar();

            var act = () => FuncionesPersonalizadas_base.RecuperarSucursalesParaBuscadorDeCliente(Crear.Cliente());

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void deberia_mapear_sucursales_dependientes_con_nombre_compuesto()
        {
            FakeDb.Cuando("Clientes.spGestionSucursal").Devolver(Tabla.De(
                new { sde_codigo = (int?)7, sde_sucursal = "CC", sde_sucursalDependiente = "SF" },
                new { sde_codigo = (int?)null, sde_sucursal = (string)null, sde_sucursalDependiente = "RO" }));

            var r = FuncionesPersonalizadas_base.RecuperarTodasSucursalesDependientes();

            r.Should().HaveCount(2);
            r[0].Should().BeEquivalentTo(new { sde_codigo = 7, sde_sucursal = "CC", sde_sucursalDependiente = "SF", sucursal_sucursalDependiente = "CC - SF" });
            r[1].sde_codigo.Should().Be(0);
            r[1].sde_sucursal.Should().BeNull();
            r[1].sucursal_sucursalDependiente.Should().BeNull();
            var llamada = FakeDb.Unica("Clientes.spGestionSucursal");
            llamada.Param("@accion").Should().Be("SELECT");
        }

        [Fact]
        public void deberia_devolver_null_cuando_falla_consulta_de_sucursales_dependientes()
        {
            FakeDb.Cuando("Clientes.spGestionSucursal").Fallar();
            FuncionesPersonalizadas_base.RecuperarTodasSucursalesDependientes().Should().BeNull();
        }

        private static void Stock(params (string pro, string suc, string stock)[] filas)
            => FakeDb.Cuando("Productos.spRecuperarStockPorProductosAndSucursal").Devolver(Tabla.De(filas.Select(f => (object)new { stk_codpro = f.pro, stk_codsuc = f.suc, stk_stock = f.stock }).ToArray()));

        [Fact]
        public void deberia_no_tocar_stock_ni_consultar_base_cuando_cliente_no_es_S7_ni_tiene_alternativa()
        {
            var productos = new List<cProductosGenerico> { Crear.ProductoGenerico() };

            var r = FuncionesPersonalizadas_base.ActualizarStockListaProductos(Crear.Cliente(), new List<string> { "CC" }, productos);

            r.Should().BeSameAs(productos);
            r[0].listaSucursalStocks.Should().BeEmpty();
            FakeDb.Llamadas.Should().BeEmpty();
        }

        [Theory]
        [InlineData("S7", null)]
        [InlineData("R1", "SF")]
        public void deberia_cargar_stock_por_sucursal_cuando_cliente_es_S7_o_tiene_alternativa(string reparto, string alternativa)
        {
            Stock(("P001", "CC", "S"), ("P001", "SF", "N"), ("P002", "CC", "B"));
            var productos = new List<cProductosGenerico> { Crear.ProductoGenerico(), Crear.ProductoGenerico(p => p.pro_codigo = "P003") };

            var r = FuncionesPersonalizadas_base.ActualizarStockListaProductos(Crear.Cliente(c => { c.cli_codrep = reparto; c.cli_IdSucursalAlternativa = alternativa; }), new List<string> { "CC", "SF" }, productos);

            r[0].listaSucursalStocks.Select(s => (s.stk_codpro, s.stk_codsuc, s.stk_stock)).Should().Equal(("P001", "CC", "S"), ("P001", "SF", "N"));
            r[1].listaSucursalStocks.Should().BeEmpty();
            var llamada = FakeDb.Unica("Productos.spRecuperarStockPorProductosAndSucursal");
            ((DataTable)llamada.Param("@Tabla_Sucursales")).Rows.Cast<DataRow>().Select(x => (string)x[0]).Should().Equal("CC", "SF");
            ((DataTable)llamada.Param("@Tabla_Productos")).Rows.Cast<DataRow>().Select(x => (string)x["codProducto"]).Should().Equal("P001", "P003");
        }

        [Fact]
        public void deberia_dejar_stock_original_cuando_falla_la_consulta_de_stock()
        {
            FakeDb.Cuando("Productos.spRecuperarStockPorProductosAndSucursal").Fallar();
            var productos = new List<cProductosGenerico> { Crear.ProductoGenerico() };

            var r = FuncionesPersonalizadas_base.ActualizarStockListaProductos(Crear.Cliente(c => c.cli_codrep = "S7"), new List<string>(), productos);

            r[0].listaSucursalStocks.Should().BeEmpty();
            FakeDb.ErroresLogueados.Should().ContainSingle();
        }

        [Fact]
        public void deberia_lanzar_excepcion_cuando_codigo_de_producto_tiene_comilla()
        {
            // CARACTERIZACIÓN: posible bug - el código de producto se concatena en DataTable.Select sin escapar comillas.
            Stock(("P'1", "CC", "S"));
            var productos = new List<cProductosGenerico> { Crear.ProductoGenerico(p => p.pro_codigo = "P'1") };

            var act = () => FuncionesPersonalizadas_base.ActualizarStockListaProductos(Crear.Cliente(c => c.cli_codrep = "S7"), new List<string>(), productos);

            act.Should().Throw<SyntaxErrorException>();
        }

        private static void Sucursales(params (string cod, bool perfumeria)[] filas)
            => FakeDb.Cuando("Clientes.spRecuperarTodasSucursal").Devolver(Tabla.De(filas.Select(f => (object)new { suc_codigo = f.cod, suc_trabajaPerfumeria = f.perfumeria }).ToArray()));

        [Fact]
        public void deberia_asignar_cantidad_a_la_sucursal_elegida_cuando_es_S7()
        {
            Stock(("P001", "CC", "S"), ("P001", "SF", "S"));
            Sucursales(("CC", true), ("SF", true));
            var productos = new List<cProductosGenerico> { Crear.ProductoGenerico(p => p.cantidad = 5) };

            var r = FuncionesPersonalizadas_base.ActualizarStockListaProductos_SubirArchico(Crear.Cliente(c => c.cli_codrep = "S7"), new List<string> { "CC", "SF" }, productos, "SF");

            r[0].listaSucursalStocks.Single(s => s.stk_codsuc == "SF").cantidadSucursal.Should().Be(5);
            r[0].listaSucursalStocks.Single(s => s.stk_codsuc == "CC").cantidadSucursal.Should().Be(0);
        }

        [Fact]
        public void deberia_mandar_perfumeria_a_CC_cuando_sucursal_elegida_no_trabaja_perfumeria()
        {
            Sucursales(("CC", true), ("RO", false));
            var perfume = Crear.ProductoGenerico(p => { p.pro_codtpopro = "P"; p.cantidad = 2; });
            perfume.listaSucursalStocks.Add(new cSucursalStocks { stk_codsuc = "CC" });
            perfume.listaSucursalStocks.Add(new cSucursalStocks { stk_codsuc = "RO" });
            var remedio = Crear.ProductoGenerico(p => { p.pro_codtpopro = "M"; p.cantidad = 3; });
            remedio.listaSucursalStocks.Add(new cSucursalStocks { stk_codsuc = "CC" });
            remedio.listaSucursalStocks.Add(new cSucursalStocks { stk_codsuc = "RO" });

            var r = FuncionesPersonalizadas_base.ActualizarStockListaProductos_SubirArchico(Crear.Cliente(), new List<string>(), new List<cProductosGenerico> { perfume, remedio }, "RO");

            r[0].listaSucursalStocks.Select(s => s.cantidadSucursal).Should().Equal(2, 0);
            r[1].listaSucursalStocks.Select(s => s.cantidadSucursal).Should().Equal(0, 3);
        }

        [Fact]
        public void deberia_no_asignar_cantidades_cuando_no_es_S7_ni_alternativa_y_trabaja_perfumeria()
        {
            Sucursales(("CC", true));
            var p = Crear.ProductoGenerico(x => x.cantidad = 2);
            p.listaSucursalStocks.Add(new cSucursalStocks { stk_codsuc = "CC" });

            var r = FuncionesPersonalizadas_base.ActualizarStockListaProductos_SubirArchico(Crear.Cliente(), new List<string>(), new List<cProductosGenerico> { p }, "CC");

            r[0].listaSucursalStocks[0].cantidadSucursal.Should().Be(0);
        }

        [Fact]
        public void deberia_lanzar_NullReferenceException_cuando_falla_consulta_de_sucursales()
        {
            FakeDb.Cuando("Clientes.spRecuperarTodasSucursal").Fallar();

            var act = () => FuncionesPersonalizadas_base.ActualizarStockListaProductos_SubirArchico(Crear.Cliente(), new List<string>(), new List<cProductosGenerico>(), "CC");

            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void deberia_mapear_imagenes_de_productos()
        {
            FakeDb.Cuando("dbo.spObtenerProductosImagenes").Devolver(Tabla.De(
                new { pro_codigo = "P1", pro_nombre = "UNO", pri_nombreArchivo = "uno.jpg", pri_ancho_ampliar = (int?)300, pri_alto_ampliar = (int?)200 },
                new { pro_codigo = "P2", pro_nombre = (string)null, pri_nombreArchivo = (string)null, pri_ancho_ampliar = (int?)null, pri_alto_ampliar = (int?)null }));

            var r = FuncionesPersonalizadas_base.ObtenerProductosImagenes();

            r.Should().HaveCount(2);
            r[0].Should().BeEquivalentTo(new { pro_codigo = "P1", pro_nombre = "UNO", pri_nombreArchivo = "uno.jpg", pri_ancho_ampliar = 300, pri_alto_ampliar = 200, pri_ancho_ampliar_original = 300, pri_alto_ampliar_original = 200 });
            r[1].pri_ancho_ampliar.Should().Be(DKbase.generales.Constantes.cImg_ancho_ampliar_dafault);
            r[1].pri_ancho_ampliar_original.Should().BeNull();
        }

        [Fact]
        public void deberia_devolver_null_cuando_falla_consulta_de_imagenes()
        {
            FakeDb.Cuando("dbo.spObtenerProductosImagenes").Fallar();
            FuncionesPersonalizadas_base.ObtenerProductosImagenes().Should().BeNull();
        }
    }
}
