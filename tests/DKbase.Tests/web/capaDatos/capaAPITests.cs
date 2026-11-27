using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using DKbase.Tests.TestInfra;
using DKbase.web.capaDatos;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace DKbase.Tests.web.capaDatos
{
    /// <summary>Base: conecta el HttpClient estático de capaAPI a un handler falso y fija las URLs.</summary>
    public abstract class capaAPITestBase : TestBase
    {
        public const string Core = "https://core.test/api/";
        public const string Dll = "https://dll.test/api/";
        protected FakeHttpHandler Http { get; }

        protected capaAPITestBase()
        {
            Http = FakeHttpHandler.InstalarEnCapaApi();
            capaAPI.url_DKcore = Core;
            capaAPI.url_DKdll = Dll;
            Credenciales(null, null);
        }

        /// <summary>Carga las credenciales sin disparar la autenticación en segundo plano de setDatosLogin.</summary>
        protected static void Credenciales(string login, string pass)
        {
            typeof(capaAPI).GetProperty("_login", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, login);
            typeof(capaAPI).GetProperty("_pass", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, pass);
        }
    }

    /// <summary>Gateway HTTP hacia DKcore y DKdll.</summary>
    public class capaAPITests : capaAPITestBase
    {

        // ---------- isNotNull ----------
        [Theory]
        [InlineData(null, true)]        // null no es string: se considera "no nulo"
        [InlineData("", false)]
        [InlineData("null", false)]
        [InlineData("NULL", false)]
        [InlineData("Null", false)]
        [InlineData(" null", true)]
        [InlineData("{}", true)]
        [InlineData(5, true)]
        public void deberia_considerar_nulo_solo_string_vacio_o_texto_null(object valor, bool esperado)
        {
            capaAPI.isNotNull(valor).Should().Be(esperado);
        }

        // ---------- POST genérico ----------
        [Fact]
        public async Task deberia_postear_json_al_endpoint_de_dll_y_deserializar_respuesta()
        {
            Http.ResponderJson("{\"Error\":\"sin stock\",\"MensajeEnFactura\":\"m\"}");

            var r = await capaAPI.TomarPedidoAsync("login", "CC", "fac", "rem", "R", new List<DKbase.dll.cDllProductosAndCantidad>(), true);

            r.Error.Should().Be("sin stock");
            var req = Http.Llamadas.Should().ContainSingle().Subject;
            req.Metodo.Should().Be(HttpMethod.Post);
            req.Url.Should().Be(Dll + "TomarPedido");
            req.ContentType.Should().Be("application/json");
            req.Cuerpo.Should().Contain("\"pLoginCliente\":\"login\"").And.Contain("\"pIsUrgente\":true");
        }

        [Theory]
        [InlineData("null")]
        [InlineData("")]
        public async Task deberia_devolver_null_cuando_la_respuesta_es_vacia_o_null(string cuerpo)
        {
            Http.ResponderJson(cuerpo);

            (await capaAPI.ObtenerFacturaAsync("F1", "login")).Should().BeNull();
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadRequest)]
        public async Task deberia_devolver_null_sin_reintentar_cuando_el_status_no_es_exitoso(HttpStatusCode status)
        {
            Http.ResponderStatus(status);

            (await capaAPI.ObtenerFacturaAsync("F1", "login")).Should().BeNull();
            Http.Llamadas.Should().HaveCount(1);
            FakeDb.ErroresLogueados.Should().BeEmpty();
        }

        [Fact]
        public async Task deberia_devolver_null_y_loguear_cuando_hay_timeout_o_error_de_red()
        {
            Http.ResponderExcepcion(new TaskCanceledException("timeout"));

            (await capaAPI.ObtenerFacturaAsync("F1", "login")).Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle().Which.Param("err_Nombre").Should().Be("MoveNext");
        }

        [Fact]
        public async Task deberia_lanzar_JsonException_cuando_la_respuesta_no_es_json_valido()
        {
            Http.ResponderJson("<html>error</html>");

            var act = () => capaAPI.ObtenerFacturaAsync("F1", "login");

            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task deberia_mandar_cuerpo_vacio_cuando_no_hay_parametros()
        {
            Http.ResponderJson("[]");

            (await capaAPI.ObtenerPlanesDeObrasSocialesAsync()).Should().BeEmpty();
            Http.Llamadas.Single().Cuerpo.Should().BeEmpty();
            Http.Llamadas.Single().ContentType.Should().Be("text/plain");
        }

        // ---------- 401 y reautenticación ----------
        [Fact]
        public async Task deberia_reautenticar_y_reintentar_una_vez_cuando_DKcore_responde_401()
        {
            Credenciales("usr", "pwd");
            Http.ResponderStatus(HttpStatusCode.Unauthorized)
                .ResponderJson("{\"token\":\"nuevo-token\"}")
                .ResponderJson("{\"Error\":\"ok\"}");

            var r = await capaAPI.TomarPedidoTelefonistaAsync(1, "l", "CC", "f", "r", "E", null, "tel");

            r.Error.Should().Be("ok");
            Http.Llamadas.Select(l => l.Url).Should().Equal(Core + "TomarPedidoTelefonista", Core + "Authenticate", Core + "TomarPedidoTelefonista");
            Http.Llamadas[1].Cuerpo.Should().Be("{\"login\":\"usr\",\"pass\":\"pwd\",\"token\":null}");
            Http.Llamadas[2].Authorization.Should().Be("Bearer nuevo-token");
            FakeDb.ErroresLogueados.Should().ContainSingle().Which.Param("err_Message").Should().Be("StatusCode == HttpStatusCode.Unauthorized");
        }

        [Fact]
        public async Task deberia_reintentar_solo_una_vez_cuando_DKcore_sigue_respondiendo_401()
        {
            Http.PorDefecto = _ => new HttpResponseMessage(HttpStatusCode.Unauthorized);

            (await capaAPI.TomarPedidoTelefonistaAsync(1, "l", "CC", "f", "r", "E", null, "tel")).Should().BeNull();

            // sin credenciales cargadas no se llama a Authenticate: solo pedido original + reintento
            Http.Llamadas.Select(l => l.Url).Should().Equal(Core + "TomarPedidoTelefonista", Core + "TomarPedidoTelefonista");
        }

        [Fact]
        public async Task deberia_no_reintentar_cuando_DKdll_responde_401()
        {
            Http.ResponderStatus(HttpStatusCode.Unauthorized);

            (await capaAPI.ObtenerFacturaAsync("F", "l")).Should().BeNull();
            Http.Llamadas.Should().HaveCount(1);
            FakeDb.ErroresLogueados.Should().ContainSingle();
        }

        [Fact]
        public async Task deberia_devolver_token_crudo_cuando_autentica()
        {
            Http.ResponderJson("{\"token\":\"abc\"}");

            (await capaAPI.authenticate("u", "p")).Should().Be("{\"token\":\"abc\"}");
            Http.Llamadas.Single().Url.Should().Be(Core + "Authenticate");
        }

        [Theory]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task deberia_devolver_null_sin_reintentar_cuando_falla_la_autenticacion(HttpStatusCode status)
        {
            Http.ResponderStatus(status);

            (await capaAPI.authenticate("u", "p")).Should().BeNull();
            Http.Llamadas.Should().HaveCount(1);
        }

        [Fact]
        public async Task deberia_no_llamar_a_la_api_cuando_no_hay_credenciales()
        {
            await capaAPI.SetAuthorization();

            Http.Llamadas.Should().BeEmpty();
        }

        [Fact]
        public void deberia_autenticar_en_segundo_plano_cuando_se_cargan_credenciales()
        {
            Http.ResponderJson("{\"token\":\"t1\"}");

            capaAPI.setDatosLogin("u", "p");

            // Espera a que termine la tarea en segundo plano (header cargado) para no contaminar otros tests.
            var client = (HttpClient)typeof(capaAPI).GetField("client", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            var limite = DateTime.UtcNow.AddSeconds(10);
            while (client.DefaultRequestHeaders.Authorization?.Parameter != "t1" && DateTime.UtcNow < limite) System.Threading.Thread.Yield();
            client.DefaultRequestHeaders.Authorization.ToString().Should().Be("Bearer t1");
            Http.Llamadas.Should().ContainSingle().Which.Url.Should().Be(Core + "Authenticate");
        }

        [Fact]
        public async Task deberia_loguear_y_no_lanzar_cuando_la_autenticacion_devuelve_null()
        {
            Credenciales("u", "p");
            Http.ResponderStatus(HttpStatusCode.InternalServerError);

            await capaAPI.SetAuthorization();

            FakeDb.ErroresLogueados.Should().ContainSingle(); // Deserialize(null) lanza y se loguea
        }

        [Fact]
        public async Task deberia_devolver_vacio_siempre_cuando_modifica_password()
        {
            Http.ResponderStatus(HttpStatusCode.InternalServerError);

            (await capaAPI.ModificarPasswordWEBAsync("id", "a", "b")).Should().BeEmpty();
            Http.Llamadas.Single().Cuerpo.Should().Be("{\"pIdentificadorCliente\":\"id\",\"pPassActual\":\"a\",\"pPassNueva\":\"b\"}");
        }

        [Fact]
        public async Task deberia_devolver_true_siempre_cuando_imprime_comprobante()
        {
            // CARACTERIZACIÓN: posible bug - ImprimirComprobante devuelve true aunque la API falle.
            Http.ResponderStatus(HttpStatusCode.InternalServerError);

            (await capaAPI.ImprimirComprobanteAsync("FAC", "1")).Should().BeTrue();
        }

        [Fact]
        public void deberia_mantener_url_cargada_al_iniciar_la_clase_aunque_cambie_Helper()
        {
            // CARACTERIZACIÓN: posible bug - url_DKcore/url_DKdll se copian de Helper en el inicializador estático;
            // si Helper se configura después de tocar capaAPI, las URLs quedan viejas (o null).
            Helper.getUrl_DKcore = "https://otra-url/";

            capaAPI.url_DKcore.Should().Be(Core);
        }
    }

    /// <summary>
    /// Golden master: para cada método público async de capaAPI se registra el endpoint, la URL base y el cuerpo JSON
    /// que envía, y lo que devuelve ante una respuesta "null" y ante una respuesta con un objeto por defecto.
    /// Cualquier cambio de contrato con DKcore/DKdll durante la refactorización rompe el snapshot.
    /// </summary>
    public class capaAPIContratoTests : capaAPITestBase
    {
        private static readonly DateTime Fecha = new DateTime(2024, 3, 15, 10, 30, 0);

        private static object ValorDe(Type t, string nombre)
        {
            if (t == typeof(string)) return "v_" + nombre;
            if (t == typeof(int)) return 7;
            if (t == typeof(long)) return 7L;
            if (t == typeof(bool)) return true;
            if (t == typeof(DateTime)) return Fecha;
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                var lista = (IList)Activator.CreateInstance(t);
                lista.Add(Activator.CreateInstance(t.GetGenericArguments()[0]));
                return lista;
            }
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }

        private static string RespuestaPorDefecto(Type tipoResultado)
        {
            if (tipoResultado == typeof(string)) return "\"texto\"";
            if (tipoResultado == typeof(bool)) return "true";
            var subyacente = Nullable.GetUnderlyingType(tipoResultado);
            if (subyacente == typeof(long)) return "12";
            if (subyacente != null) return "12.5";
            if (tipoResultado == typeof(List<string>)) return "[\"texto\"]";
            if (typeof(IEnumerable).IsAssignableFrom(tipoResultado)) return "[{}]";
            return "{}";
        }

        [Fact]
        public async Task deberia_mantener_el_contrato_http_de_todos_los_metodos()
        {
            var metodos = typeof(capaAPI).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) && m.Name != nameof(capaAPI.authenticate))
                .OrderBy(m => m.Name).ThenBy(m => m.GetParameters().Length).ToList();
            var contrato = new List<object>();

            foreach (var m in metodos)
            {
                var args = m.GetParameters().Select(p => ValorDe(p.ParameterType, p.Name)).ToArray();
                var tipoResultado = m.ReturnType.GetGenericArguments()[0];

                Http.Llamadas.Clear();
                Http.ResponderJson("null");
                var conNull = await Invocar(m, args);
                var req = Http.Llamadas.Single();

                Http.ResponderJson(RespuestaPorDefecto(tipoResultado));
                var conObjeto = await Invocar(m, args);

                contrato.Add(new
                {
                    Metodo = m.Name,
                    Destino = req.Url.StartsWith(Core) ? "DKcore" : req.Url.StartsWith(Dll) ? "DKdll" : req.Url,
                    Endpoint = req.Url.Replace(Core, "").Replace(Dll, ""),
                    Cuerpo = req.Cuerpo,
                    RespuestaNull = Describir(conNull),
                    RespuestaObjeto = Describir(conObjeto),
                });
            }

            metodos.Should().HaveCountGreaterThan(45);
            await Verifier.Verify(contrato);
        }

        private static async Task<object> Invocar(MethodInfo m, object[] args)
        {
            try
            {
                var task = (Task)m.Invoke(null, args);
                await task;
                return task.GetType().GetProperty("Result").GetValue(task);
            }
            catch (Exception ex) { return "EXCEPCION " + (ex is TargetInvocationException tie ? tie.InnerException : ex).GetType().Name; }
        }

        private static string Describir(object r) => r switch
        {
            null => "null",
            string s => s,
            _ => r.GetType().Name + " " + JsonSerializer.Serialize(r),
        };
    }
}
