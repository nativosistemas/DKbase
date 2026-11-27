using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DKbase.dll;
using DKbase.generales;
using DKbase.Tests.TestInfra;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.generales
{
    public class LogTests : TestBase
    {
        // Método "de producción" de mentira cuya firma se usa para nombrar parámetros en el log.
        public static void MetodoDeEjemplo(int pId, string pNombre, List<cDllProductosAndCantidad> pLista) { }
        private static readonly MethodBase Metodo = typeof(LogTests).GetMethod(nameof(MetodoDeEjemplo));

        public LogTests() { FakeClock.Ahora = new DateTime(2024, 2, 29, 13, 45, 10); Helper.getTipoApp = "web"; }

        private string ArchivoLog => Path.Combine(Carpeta, "log", "20240229web.txt");

        [Fact]
        public void deberia_crear_archivo_diario_con_tipo_de_app_cuando_loguea_en_archivo()
        {
            Log.LogErrorFile("Pantalla1", "mensaje 1");

            File.ReadAllLines(ArchivoLog).Should().Equal("fecha: 02/29/2024 13:45:10 - pantalla: Pantalla1 - mensaje :mensaje 1");
        }

        [Fact]
        public void deberia_agregar_lineas_cuando_el_archivo_ya_existe()
        {
            Log.LogErrorFile("A", "1");
            Log.LogErrorFile("B", "2");

            File.ReadAllLines(ArchivoLog).Should().HaveCount(2);
        }

        [Fact]
        public void deberia_usar_formato_de_fecha_de_la_cultura_actual()
        {
            UsarCultura("es-AR");

            Log.LogErrorFile("A", "1");

            // El formato exacto depende de ICU / Windows; se fija solo que es día/mes y no mes/día.
            File.ReadAllText(ArchivoLog).Should().StartWith("fecha: 29/");
        }

        [Fact]
        public void deberia_no_lanzar_cuando_no_hay_carpeta_configurada()
        {
            Helper.getFolder = null;

            var act = () => Log.saveInFile("x", "y.txt");

            act.Should().NotThrow();
        }

        [Fact]
        public void deberia_registrar_error_con_mensaje_y_parametros_en_la_base()
        {
            Log.LogError(Metodo, "algo falló", DateTime.MinValue, 5, "abc", null);

            var l = FakeDb.ErroresLogueados.Should().ContainSingle().Subject;
            l.Param("err_Nombre").Should().Be("MetodoDeEjemplo");
            l.Param("err_Parameters").Should().Be("<pId>5</pId><pNombre>abc</pNombre><pLista></pLista>");
            l.Param("err_Message").Should().Be("algo falló");
            l.Param("err_Data").Should().BeNull();
            l.Param("err_StackTrace").Should().BeNull();
            l.Param("err_fecha").Should().Be(FakeClock.Ahora); // ignora la fecha recibida, usa la actual
            l.Param("err_tipo").Should().Be("web");
            l.CommandType.Should().Be(System.Data.CommandType.StoredProcedure);
        }

        [Fact]
        public void deberia_detallar_cada_producto_cuando_el_parametro_es_lista_de_productos()
        {
            var lista = new List<cDllProductosAndCantidad>
            {
                new cDllProductosAndCantidad { codProductoNombre = "IBU", cantidad = 2, IdTransfer = 9, isOferta = true },
                new cDllProductosAndCantidad { codProductoNombre = "AAS", cantidad = 1 },
            };

            Log.LogError(Metodo, "m", DateTime.Now, 1, "x", lista);

            FakeDb.ErroresLogueados.Single().Param("err_Parameters").Should().Be(
                "<pId>1</pId><pNombre>x</pNombre><pLista>" +
                "codProductoNombre = IBU || cantidad = 2 || IdTransfer = 9 || isOferta = True" +
                "codProductoNombre = AAS || cantidad = 1 || IdTransfer = 0 || isOferta = False</pLista>");
        }

        [Fact]
        public void deberia_dejar_parametros_vacios_cuando_no_se_pasan_valores()
        {
            Log.LogError(Metodo, "m", DateTime.Now);

            FakeDb.ErroresLogueados.Single().Param("err_Parameters").Should().Be("");
        }

        [Fact]
        public void deberia_loguear_error_extra_y_parametros_parciales_cuando_faltan_valores()
        {
            // Menos valores que parámetros: getParameters falla con IndexOutOfRange y lo registra aparte.
            Log.LogError(Metodo, "m", DateTime.Now, 1);

            var errores = FakeDb.ErroresLogueados.ToList();
            errores.Should().HaveCount(2);
            errores[0].Param("err_Nombre").Should().Be("getParameters");
            errores[1].Param("err_Parameters").Should().Be("<pId>1</pId><pNombre>");
        }

        [Fact]
        public void deberia_registrar_datos_de_la_excepcion()
        {
            Exception ex;
            try { throw new InvalidOperationException("boom", new ArgumentException("interna")) { HelpLink = "http://help" }; }
            catch (Exception e) { ex = e; }

            Log.LogError(Metodo, ex, new DateTime(2000, 1, 1), 1, "x", null);

            var l = FakeDb.ErroresLogueados.Single();
            l.Param("err_Message").Should().Be("boom");
            l.Param("err_HelpLink").Should().Be("http://help");
            ((string)l.Param("err_InnerException")).Should().StartWith("System.ArgumentException: interna");
            l.Param("err_Source").Should().Be("DKbase.Tests");
            ((string)l.Param("err_StackTrace")).Should().Contain(nameof(deberia_registrar_datos_de_la_excepcion));
            l.Param("err_Data").Should().Be("System.Collections.ListDictionaryInternal");
            l.Param("err_fecha").Should().Be(FakeClock.Ahora);
        }

        [Fact]
        public void deberia_escribir_en_archivo_cuando_falla_el_log_en_base()
        {
            FakeDb.Cuando("LogRegistro.spError").Fallar(new InvalidOperationException("db caída"));

            Log.LogError(Metodo, "mensaje original", DateTime.Now, 1, "x", null);

            var lineas = File.ReadAllLines(ArchivoLog).Where(x => x.StartsWith("fecha: ")).ToArray(); // el stack trace ocupa varias líneas
            lineas.Should().HaveCount(2);
            lineas[0].Should().Contain("pantalla: MetodoDeEjemplo").And.Contain("err_Parameters:<pId>1</pId>").And.Contain("err_Message: mensaje original");
            lineas[1].Should().Contain("spError").And.Contain("db caída");
        }

        [Fact]
        public void deberia_registrar_info_con_nombre_de_tipo_y_contenido_vacio_por_defecto()
        {
            Log.LogInfo(Metodo, "info", "adicional", "tipo", "pdf", null, 1, "x", null);

            var l = FakeDb.Unica("LogRegistro.spLogInfo");
            l.Param("log_Nombre").Should().Be(typeof(LogTests).AssemblyQualifiedName);
            l.Param("log_Mensaje").Should().Be("info");
            l.Param("log_InfoAdicional").Should().Be("adicional");
            l.Param("log_Parameters").Should().Be("<pId>1</pId><pNombre>x</pNombre><pLista></pLista>");
            l.Param("log_fecha").Should().Be(FakeClock.Ahora);
            l.Param("log_app").Should().Be("web");
            l.Param("log_type").Should().Be("tipo");
            l.Param("log_file_type").Should().Be("pdf");
            ((byte[])l.Param("log_file_content")).Should().BeEmpty();
        }

        [Fact]
        public void deberia_enviar_contenido_de_archivo_cuando_se_informa()
        {
            Log.LogInfo(Metodo, "info", null, null, null, new byte[] { 1, 2, 3 }, 1, "x", null);

            ((byte[])FakeDb.Unica("LogRegistro.spLogInfo").Param("log_file_content")).Should().Equal(1, 2, 3);
        }

        [Fact]
        public void deberia_escribir_en_archivo_cuando_falla_el_log_de_info()
        {
            FakeDb.Cuando("LogRegistro.spLogInfo").Fallar();

            Log.LogInfo(Metodo, "info", "adic", null, null, null, 1, "x", null);

            File.ReadAllLines(ArchivoLog).Count(x => x.StartsWith("fecha: ")).Should().Be(2);
        }
    }
}
