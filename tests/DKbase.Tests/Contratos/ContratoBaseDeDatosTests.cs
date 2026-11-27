using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DKbase.Tests.TestInfra;
using DKbase.web;
using DKbase.web.capaDatos;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace DKbase.Tests.Contratos
{
    /// <summary>
    /// Golden master de la capa de datos y de las fachadas que la usan.
    /// Para cada método público estático de cada clase se invoca con argumentos sintéticos en dos escenarios:
    ///  1. la base responde vacío (0 filas, escalar null)
    ///  2. la base falla en todas las llamadas
    ///  3. la base devuelve datos "universales" (todas las columnas conocidas, 2 filas por tabla; ver FakeDb.ModoUniversal)
    /// y se congela: qué stored procedures se llamaron, con qué parámetros, qué devolvió el método o qué excepción lanzó.
    /// Si durante la refactorización cambia un nombre de SP, un parámetro o el manejo de errores, el snapshot lo muestra.
    /// </summary>
    public class ContratoBaseDeDatosTests : TestBase
    {
        public static IEnumerable<object[]> Clases() => new[]
        {
            typeof(capaCAR_base), typeof(capaCAR_intranet_base), typeof(capaCAR_WebService_base), typeof(capaCV_base),
            typeof(capaCatalogo_base), typeof(capaClientes_base), typeof(capaDevoluciones_base), typeof(capaHome_base),
            typeof(capaLogRegistro_base), typeof(capaMensaje_base), typeof(capaModulo), typeof(capaProductos_base),
            typeof(capaRecurso_base), typeof(capaSeguridad_base), typeof(capaTiposEnvios_base), typeof(capaTransfer_base),
            typeof(acceso), typeof(DKbase.app.accesoApp), typeof(AccesoGrilla_base), typeof(DKbase.Util),
        }.Select(t => new object[] { t.Name });

        private static Type Resolver(string nombre) => typeof(DKbase.Helper).Assembly.GetTypes().Single(t => t.Name == nombre && t.IsPublic);

        // Métodos que no se pueden invocar de forma sintética (necesitan datos reales o tocan GDI+, etc.).
        private static readonly HashSet<string> Excluidos = new()
        {
            "Util.GenerateDocument", // usa OpenXml sobre plantillas en disco
        };

        public ContratoBaseDeDatosTests()
        {
            FakeHttpHandler.InstalarEnCapaApi();
            capaAPI.url_DKcore = "https://core.test/";
            capaAPI.url_DKdll = "https://dll.test/";
            Helper.getMail_from = "no-reply@example.com";
            Helper.getSMTP_SERVER = "smtp.test";
            Helper.getSMTP_PORT = 25;
            Helper.getMailContacto = "contacto@example.com";
        }

        [Theory]
        [MemberData(nameof(Clases))]
        public async Task deberia_mantener_el_contrato_con_la_base_de_datos(string clase)
        {
            var tipo = Resolver(clase);
            var metodos = tipo.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName && !m.ContainsGenericParameters && !Excluidos.Contains(tipo.Name + "." + m.Name))
                .OrderBy(m => m.Name).ThenBy(m => string.Join(",", m.GetParameters().Select(p => p.ParameterType.Name))).ToList();

            var resultado = new List<object>();
            foreach (var m in metodos)
            {
                var firma = m.Name + "(" + string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name)) + ")";

                FakeDb.Reset(); FakeClock.Reset(); FakeSmtp.Reset(); FakeSleep.Reset();
                var vacio = Invocar(m);
                var llamadasVacio = FakeDb.Llamadas.Where(l => l.Tipo != "Open").Select(Describir).ToList();

                FakeDb.Reset();
                FakeDb.ErrorAlConectar = new InvalidOperationException("base caída");
                var conError = Invocar(m);
                var llamadasError = FakeDb.Llamadas.Count(l => l.Tipo == "Open");

                FakeDb.Reset(); FakeClock.Reset(); FakeSmtp.Reset(); FakeSleep.Reset();
                FakeDb.ModoUniversal = true;
                var conDatos = Invocar(m);
                var llamadasDatos = FakeDb.Llamadas.Count(l => l.Tipo != "Open");
                var erroresDatos = FakeDb.ErroresLogueados.Select(l => l.Param("err_Nombre") + ": " + l.Param("err_Message")).ToList();

                resultado.Add(new
                {
                    Metodo = firma,
                    BaseVacia = new { Llamadas = llamadasVacio, Devuelve = vacio, Mails = FakeSmtp.Enviados.Count },
                    BaseCaida = new { Conexiones = llamadasError, Devuelve = conError },
                    BaseConDatos = new { Llamadas = llamadasDatos, Devuelve = conDatos, ErroresLogueados = erroresDatos, Mails = FakeSmtp.Enviados.Count },
                });
            }

            await Verifier.Verify(resultado).UseParameters(clase)
                .ScrubLinesWithReplace(l => System.Text.RegularExpressions.Regex.Replace(l, "dkbase-tests/[0-9a-f]{32}", "{Carpeta}"));
        }

        // ---------- argumentos sintéticos ----------
        // Los parámetros opcionales (p. ej. pConnectionStringSQL = null) se dejan en su valor por defecto, como los llama el código real.
        private static object[] Argumentos(MethodInfo m) => m.GetParameters().Select(p => p.HasDefaultValue && p.Name.IndexOf("Connection", StringComparison.OrdinalIgnoreCase) >= 0 ? p.DefaultValue : Valor(p.ParameterType, p.Name)).ToArray();

        internal static object Valor(Type t, string nombre)
        {
            var sub = Nullable.GetUnderlyingType(t);
            if (sub != null) return Valor(sub, nombre);
            if (t == typeof(string)) return "v_" + nombre;
            if (t == typeof(int)) return 7;
            if (t == typeof(long)) return 7L;
            if (t == typeof(short)) return (short)7;
            if (t == typeof(bool)) return true;
            if (t == typeof(decimal)) return 1.5m;
            if (t == typeof(double)) return 1.5d;
            if (t == typeof(DateTime)) return new DateTime(2024, 3, 15, 10, 0, 0);
            if (t == typeof(Guid)) return new Guid("11111111-2222-3333-4444-555555555555");
            if (t == typeof(byte[])) return new byte[] { 1, 2, 3 };
            if (t.IsEnum) return Enum.GetValues(t).GetValue(0);
            if (t == typeof(DataTable)) return Tabla.De(new { col = "x" });
            if (t == typeof(DataSet)) return new DataSet();
            if (t == typeof(cClientes)) return Crear.Cliente();
            if (t == typeof(Usuario)) return Crear.Usuario(u => u.usu_login = "usr");
            if (t == typeof(MethodBase)) return typeof(ContratoBaseDeDatosTests).GetMethod(nameof(Valor), BindingFlags.NonPublic | BindingFlags.Static);
            if (t.IsArray)
            {
                var arr = Array.CreateInstance(t.GetElementType(), 1);
                arr.SetValue(Valor(t.GetElementType(), nombre), 0);
                return arr;
            }
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                var lista = (IList)Activator.CreateInstance(t);
                var item = Valor(t.GetGenericArguments()[0], nombre);
                if (item != null) lista.Add(item);
                return lista;
            }
            if (t.IsInterface || t.IsAbstract) return null;
            if (t.IsValueType) return Activator.CreateInstance(t);
            return t.GetConstructor(Type.EmptyTypes) != null ? Activator.CreateInstance(t) : null;
        }

        private static string Invocar(MethodInfo m)
        {
            try
            {
                var r = m.Invoke(null, Argumentos(m));
                if (r is Task task)
                {
                    task.Wait(TimeSpan.FromSeconds(10)).Should().BeTrue("el método async no debe colgarse");
                    r = task.GetType().IsGenericType ? task.GetType().GetProperty("Result").GetValue(task) : null;
                }
                return m.ReturnType == typeof(void) ? "void" : DescribirValor(r);
            }
            catch (TargetInvocationException ex) { return "EXCEPCION " + ex.InnerException.GetType().Name; }
            catch (AggregateException ex) { return "EXCEPCION " + ex.InnerException.GetType().Name; }
        }

        private static readonly JsonSerializerOptions Json = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles, WriteIndented = false };

        internal static string DescribirValor(object r)
        {
            switch (r)
            {
                case null: return "null";
                case string s: return "\"" + s + "\"";
                case DataSet ds: return "DataSet[" + string.Join(", ", ds.Tables.Cast<DataTable>().Select(t => t.TableName + "(" + t.Rows.Count + ")")) + "]";
                case DataTable dt: return "DataTable " + dt.TableName + "(" + dt.Columns.Count + " cols, " + dt.Rows.Count + " filas)";
                case DataRow _: return "DataRow";
                case DateTime d: return d.ToString("s", CultureInfo.InvariantCulture);
                case IFormattable f when r.GetType().IsPrimitive || r is decimal: return r.GetType().Name + " " + f.ToString(null, CultureInfo.InvariantCulture);
                case bool b: return "Boolean " + b;
            }
            try
            {
                var json = JsonSerializer.Serialize(r, r.GetType(), Json);
                return r.GetType().Name + " " + (json.Length > 400 ? json.Substring(0, 400) + "…" : json);
            }
            catch (Exception ex) { return r.GetType().Name + " (no serializable: " + ex.GetType().Name + ")"; }
        }

        private static string Describir(DbLlamada l)
        {
            var ps = l.Parametros.Select(p => p.Key + "=" + DescribirParam(p.Value));
            return l.Tipo + " " + l.Comando + (l.CommandType == CommandType.StoredProcedure ? "" : " [" + l.CommandType + "]") + "(" + string.Join(", ", ps) + ")";
        }

        private static string DescribirParam(object v) => v switch
        {
            null => "NULL",
            DataTable dt => "TVP[" + string.Join("|", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName)) + "](" + dt.Rows.Count + ")",
            DateTime d => d.ToString("s", CultureInfo.InvariantCulture),
            byte[] b => "bytes(" + b.Length + ")",
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => v.ToString(),
        };
    }
}
