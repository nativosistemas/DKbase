using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DKbase.Tests.TestInfra;
using DKbase.web;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.cSubirpedido
{
    /// <summary>Subida de archivo de pedido de punta a punta: guarda el archivo, lo parsea, lo manda al SP y registra el historial.</summary>
    public class LeerArchivoPedidoTests : TestBase
    {
        private const string SpCargar = "LogRegistro.spCargarCarritoProductoSucursalDesdeArchivoPedidosV5";
        private const string SpHistorial = "LogRegistro.spAgregarHistorialSubirArchivo";
        private string CarpetaPedidos => Path.Combine(Carpeta, "archivos", "ArchivosPedidos");

        private Task<cSubirPedido_return> Subir(string nombre, string contenido, string sucursal = "CC")
            => cSubirpedido_base.LeerArchivoPedido(Crear.Usuario(), Crear.Cliente(), new FakeFormFile(nombre, contenido), sucursal);

        private static DataTable TablaEnviada() => (DataTable)FakeDb.Unica(SpCargar).Param("@Tabla_Detalle");

        [Fact]
        public async Task deberia_guardar_archivo_parsear_TXT_y_registrar_historial()
        {
            FakeClock.Ahora = new DateTime(2024, 5, 31, 23, 59, 0);

            var r = await Subir("pedido.txt", "Encabezado\n1\tx\t2\tx\tx\t779001\n\n2\tx\t5\tx\tx\t779002\n");

            r.isCorrect.Should().BeTrue();
            r.SucursalEleginda.Should().Be("CC");
            r.nombreArchivoCompleto.Should().Be("pedido.txt");
            r.nombreArchivoCompletoOriginal.Should().Be("pedido.txt");
            r.ListaProductos.Should().BeNull(); // el SP (fake) no devolvió las 4 tablas esperadas
            File.Exists(Path.Combine(CarpetaPedidos, "pedido.txt")).Should().BeTrue();

            var tabla = TablaEnviada();
            tabla.Rows.Cast<DataRow>().Select(x => ((string)x["codigobarra"], (int)x["cantidad"], (int)x["nroordenamiento"])).Should().Equal(("779001", 2, 1), ("779002", 5, 2));
            var cargar = FakeDb.Unica(SpCargar);
            cargar.Param("@TipoDeArchivo").Should().Be("S");
            cargar.Param("@lrc_codSucursal").Should().Be("CC");
            cargar.Param("@Sucursal").Should().Be("CC");
            cargar.Param("@lrc_codCliente").Should().Be(1234);
            cargar.Param("@lcp_codUsuario").Should().Be(77);
            cargar.Param("@cli_codprov").Should().Be("Santa Fe");
            cargar.Param("@cli_isGLN").Should().Be(false);

            var hist = FakeDb.Unica(SpHistorial);
            hist.Param("@has_NombreArchivo").Should().Be("pedido.txt");
            hist.Param("@has_codCliente").Should().Be(1234);
            hist.Param("@has_fecha").Should().Be(new DateTime(2024, 5, 31, 23, 59, 0));
        }

        [Fact]
        public async Task deberia_renombrar_con_sufijo_cuando_el_archivo_ya_existe()
        {
            Directory.CreateDirectory(CarpetaPedidos);
            File.WriteAllText(Path.Combine(CarpetaPedidos, "pedido.txt"), "");
            File.WriteAllText(Path.Combine(CarpetaPedidos, "pedido_1.txt"), "");

            var r = await Subir("pedido.txt", "");

            r.nombreArchivoCompleto.Should().Be("pedido_2.txt");
            r.nombreArchivoCompletoOriginal.Should().Be("pedido.txt");
        }

        [Fact]
        public async Task deberia_quitar_puntos_intermedios_del_nombre_cuando_tiene_varios()
        {
            // CARACTERIZACIÓN: posible bug - se eliminan los puntos intermedios del nombre del archivo subido.
            var r = await Subir("mi.pedido.del.dia.ASC", "a,b,1,d,779\n");

            r.nombreArchivoCompleto.Should().Be("mipedidodeldia.ASC");
            FakeDb.Unica(SpCargar).Param("@TipoDeArchivo").Should().Be("S");
            TablaEnviada().Rows.Should().ContainSingle();
        }

        [Fact]
        public async Task deberia_usar_el_nombre_como_extension_cuando_no_tiene_punto()
        {
            // CARACTERIZACIÓN: posible bug - un archivo sin extensión se guarda como ".<nombre>" y se rechaza como formato desconocido.
            var r = await Subir("pedido", "algo");

            File.Exists(Path.Combine(CarpetaPedidos, ".pedido")).Should().BeTrue();
            r.Should().BeNull();
        }

        [Fact]
        public async Task deberia_parsear_formato_S_por_la_primera_letra_del_nombre()
        {
            var linea = "00003" + "ALFA000001" + "TROQ000002" + "7791234567890";
            var r = await Subir("S_pedido.dat", linea + "\n" + "corta\n");

            r.isCorrect.Should().BeTrue();
            FakeDb.Unica(SpCargar).Param("@TipoDeArchivo").Should().Be("S");
            TablaEnviada().Rows.Cast<DataRow>().Single()["codigoalfabeta"].Should().Be("ALFA000001");
        }

        [Theory]
        [InlineData(21, 0)] // por debajo del mínimo: la línea se ignora sin intentar parsearla
        [InlineData(22, 1)] // en el mínimo (22) se intenta parsear y, como el parseo necesita 38, se loguea un error
        public async Task deberia_intentar_parsear_lineas_S_desde_22_caracteres(int largo, int erroresEsperados)
        {
            var r = await Subir("S_pedido.dat", new string('1', largo) + "\n");

            r.isCorrect.Should().BeTrue();
            FakeDb.ErroresLogueados.Should().HaveCount(erroresEsperados);
        }

        [Fact]
        public async Task deberia_parsear_formato_F_por_la_primera_letra_del_nombre()
        {
            var r = await Subir("Fpedido.dat", "1234" + "00007" + "PRODUCTO00001" + "XX\n" + "1234000\n");

            FakeDb.Unica(SpCargar).Param("@TipoDeArchivo").Should().Be("F");
            TablaEnviada().Rows.Cast<DataRow>().Select(x => (string)x["codProducto"]).Should().Equal("PRODUCTO00001");
        }

        [Fact]
        public async Task deberia_ignorar_lineas_F_de_22_o_23_caracteres_aunque_sean_parseables()
        {
            // CARACTERIZACIÓN: posible bug - las líneas F exigen 24 caracteres aunque el formato usa 22.
            // El lector exige 24 caracteres aunque el formato F solo usa 22.
            await Subir("Fpedido.dat", "1234" + "00007" + "PRODUCTO00001" + "\n");

            TablaEnviada().Rows.Should().BeEmpty();
        }

        [Fact]
        public async Task deberia_devolver_null_sin_llamar_a_la_base_cuando_formato_es_desconocido()
        {
            var r = await Subir("Xpedido.dat", "contenido");

            r.Should().BeNull();
            FakeDb.SinLogs.Should().BeEmpty();
        }

        [Fact]
        public async Task deberia_devolver_resultado_correcto_cuando_formato_desconocido_esta_vacio()
        {
            // CARACTERIZACIÓN: posible bug - un archivo vacío de formato desconocido se acepta como correcto.
            // Archivo vacío: no hay líneas, nunca se detecta formato erróneo.
            var r = await Subir("Xpedido.dat", "");

            r.isCorrect.Should().BeTrue();
            FakeDb.Unica(SpCargar).Param("@TipoDeArchivo").Should().Be("X");
        }

        [Fact]
        public async Task deberia_leer_solo_celdas_numericas_y_saltear_encabezado_cuando_es_excel()
        {
            // CARACTERIZACIÓN: posible bug - los códigos de barra guardados como texto compartido (shared string) en Excel se ignoran.
            var bytes = Excel.Crear(
                new[] { "s:codigo", "s:cantidad" },
                new[] { "n:7791234567890", "n:4" },
                new[] { "s:7790000000001", "n:2" },  // código como texto compartido → se ignora
                new[] { "7790000000002", "9" });

            var r = await cSubirpedido_base.LeerArchivoPedido(Crear.Usuario(), Crear.Cliente(), new FakeFormFile("pedido.xlsx", bytes), "CC");

            r.isCorrect.Should().BeTrue();
            FakeDb.Unica(SpCargar).Param("@TipoDeArchivo").Should().Be("E");
            TablaEnviada().Rows.Cast<DataRow>().Select(x => ((string)x["codigobarra"], (int)x["cantidad"])).Should().Equal(("7791234567890", 4), ("7790000000002", 9));
        }

        [Fact]
        public async Task deberia_enviar_tabla_nula_cuando_excel_esta_corrupto()
        {
            // CARACTERIZACIÓN: posible bug - un Excel corrupto envía una tabla null a la base y el resultado queda como correcto.
            var r = await Subir("pedido.xlsx", "no es un excel");

            r.isCorrect.Should().BeTrue();
            FakeDb.Unica(SpCargar).Param("@Tabla_Detalle").Should().BeNull();
        }

        [Fact]
        public async Task deberia_devolver_null_sin_guardar_cuando_no_hay_usuario_o_cliente()
        {
            var f = new FakeFormFile("pedido.txt", "x");

            (await cSubirpedido_base.LeerArchivoPedido(null, Crear.Cliente(), f, "CC")).Should().BeNull();
            (await cSubirpedido_base.LeerArchivoPedido(Crear.Usuario(), null, f, "CC")).Should().BeNull();
            Directory.Exists(CarpetaPedidos).Should().BeFalse();
        }

        [Fact]
        public async Task deberia_devolver_null_cuando_nombre_de_archivo_es_vacio()
        {
            (await Subir("", "x")).Should().BeNull();
        }

        [Fact]
        public async Task deberia_marcar_correcto_aunque_falle_la_base()
        {
            // CARACTERIZACIÓN: posible bug - el resultado queda como correcto aunque falle la base.
            FakeDb.Cuando(SpCargar).Fallar();
            FakeDb.Cuando(SpHistorial).Fallar();

            var r = await Subir("pedido.txt", "1\tx\t2\tx\tx\t779001\n");

            r.isCorrect.Should().BeTrue();
            r.ListaProductos.Should().BeNull();
            FakeDb.ErroresLogueados.Should().HaveCount(2);
        }
    }

    public class CargarArchivoPedidoDeNuevoTests : TestBase
    {
        [Fact]
        public void deberia_releer_archivo_del_historial_con_su_sucursal()
        {
            var carpeta = Path.Combine(Carpeta, "archivos", "ArchivosPedidos");
            Directory.CreateDirectory(carpeta);
            File.WriteAllText(Path.Combine(carpeta, "viejo.txt"), "1\tx\t3\tx\tx\t779\n");
            FakeDb.Cuando("LogRegistro.spRecuperarHistorialSubirArchivoPorId").Devolver(Tabla.De(new
            {
                has_id = 5, has_NombreArchivo = "viejo.txt", has_NombreArchivoOriginal = "original.txt", has_sucursal = "SF",
                has_codCliente = 1234, has_fecha = new DateTime(2024, 1, 1)
            }));

            var r = cSubirpedido_base.CargarArchivoPedidoDeNuevo(Crear.Usuario(), Crear.Cliente(), 5);

            r.SucursalEleginda.Should().Be("SF");
            r.nombreArchivoCompletoOriginal.Should().Be("original.txt");
            FakeDb.Unica("LogRegistro.spRecuperarHistorialSubirArchivoPorId").Param("@has_id").Should().Be(5);
        }

        [Fact]
        public void deberia_devolver_null_cuando_no_existe_el_historial()
        {
            cSubirpedido_base.CargarArchivoPedidoDeNuevo(Crear.Usuario(), Crear.Cliente(), 5).Should().BeNull();
        }

        [Fact]
        public void deberia_devolver_null_cuando_el_archivo_ya_no_esta_en_disco()
        {
            FakeDb.Cuando("LogRegistro.spRecuperarHistorialSubirArchivoPorId").Devolver(Tabla.De(new
            {
                has_id = 5, has_NombreArchivo = "borrado.txt", has_NombreArchivoOriginal = "o.txt", has_sucursal = "SF",
                has_codCliente = 1234, has_fecha = new DateTime(2024, 1, 1)
            }));

            cSubirpedido_base.CargarArchivoPedidoDeNuevo(Crear.Usuario(), Crear.Cliente(), 5).Should().BeNull();
        }

        [Fact]
        public void deberia_devolver_null_sin_consultar_cuando_falta_usuario()
        {
            cSubirpedido_base.CargarArchivoPedidoDeNuevo(null, Crear.Cliente(), 5).Should().BeNull();
            FakeDb.Llamadas.Should().BeEmpty();
        }
    }
}
