using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using DKbase.dll;
using DKbase.Tests.TestInfra;
using DKbase.web;
using DKbase.web.capaDatos;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace DKbase.Tests.web.capaDatos
{
    /// <summary>Fachada síncrona sobre capaAPI con bloqueo de carrito en la base.</summary>
    public class capaDLLTests : capaAPITestBase
    {
        private const string SpInicio = "CAR.spInicioCarritoEnProceso";
        private const string SpFin = "CAR.spEndCarritoEnProceso";

        public capaDLLTests()
        {
            FakeDb.Cuando(SpInicio).Salida("@isOk", true);
            FakeDb.Cuando(SpFin).Salida("@isOk", true);
        }

        private static List<cDllProductosAndCantidad> Productos() => new() { new cDllProductosAndCantidad { codProductoNombre = "IBU", cantidad = 2 } };

        [Fact]
        public void deberia_bloquear_carrito_tomar_pedido_y_liberar_cuando_todo_sale_bien()
        {
            Http.ResponderJson("{\"Error\":\"\",\"NumeroRemito\":\"R-1\"}");

            var r = capaDLL.TomarPedidoConIdCarrito(55, "login", "CC", "fac", "rem", "E", Productos(), false);

            r.Should().NotBeNull();
            FakeDb.SinLogs.Select(l => l.Comando).Should().Equal(SpInicio, SpFin);
            FakeDb.Unica(SpInicio).Param("@car_id").Should().Be(55);
            FakeDb.Unica(SpInicio).Param("@accion").Should().Be(DKbase.generales.Constantes.cAccionCarrito_TOMAR);
            FakeDb.Unica(SpFin).Param("@car_id").Should().Be(55);
            Http.Llamadas.Single().Url.Should().Be(Dll + "TomarPedidoConIdCarrito");
        }

        [Fact]
        public void deberia_tomar_pedido_aunque_no_se_pueda_bloquear_el_carrito()
        {
            // CARACTERIZACIÓN: posible bug - el resultado de InicioCarritoEnProceso se ignora; el pedido se envía igual.
            FakeDb.Cuando(SpInicio).Salida("@isOk", false);
            Http.ResponderJson("{\"Error\":\"\"}");

            capaDLL.TomarPedidoConIdCarrito(55, "l", "CC", "f", "r", "E", Productos(), false).Should().NotBeNull();
            Http.Llamadas.Should().ContainSingle();
        }

        [Fact]
        public void deberia_liberar_el_carrito_y_loguear_cuando_la_respuesta_es_invalida()
        {
            Http.ResponderJson("no-json");

            capaDLL.TomarPedidoConIdCarrito(55, "l", "CC", "f", "r", "E", Productos(), true).Should().BeNull();
            FakeDb.De(SpFin).Should().ContainSingle();
            FakeDb.ErroresLogueados.Should().ContainSingle().Which.Param("err_Nombre").Should().Be("TomarPedidoConIdCarrito");
        }

        [Fact]
        public void deberia_devolver_null_y_liberar_cuando_la_api_falla()
        {
            Http.ResponderStatus(HttpStatusCode.ServiceUnavailable);

            capaDLL.TomarPedidoDeTransfersConIdCarrito(9, "l", "CC", "f", "r", "E", Productos()).Should().BeNull();
            FakeDb.De(SpFin).Should().ContainSingle();
        }

        [Fact]
        public void deberia_completar_sucursal_web_cuando_toma_pedido_de_telefonista()
        {
            Http.ResponderJson("{\"Error\":\"\"}");

            var r = capaDLL.TomarPedidoTelefonistaAsync(Crear.Usuario(u => u.usu_login = "tel1"), 3, "l", "SF", "f", "r", "E", Productos(), false);

            r.web_Sucursal.Should().Be("SF");
            Http.Llamadas.Single().Url.Should().Be(Core + "TomarPedidoTelefonista");
            Http.Llamadas.Single().Cuerpo.Should().Contain("\"pLoginTelefonista\":\"tel1\"");
        }

        [Fact]
        public void deberia_devolver_null_loguear_y_liberar_cuando_la_api_de_telefonista_no_responde()
        {
            Http.ResponderStatus(HttpStatusCode.InternalServerError);

            capaDLL.TomarPedidoTelefonistaAsync(Crear.Usuario(), 3, "l", "SF", "f", "r", "E", Productos(), false).Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle();
            FakeDb.De(SpFin).Should().ContainSingle();
        }

        [Fact]
        public void deberia_completar_sucursal_en_cada_transfer_cuando_toma_transfers_de_telefonista()
        {
            Http.ResponderJson("[{\"Error\":\"\"},{\"Error\":\"x\"}]");

            var r = capaDLL.TomarPedidoDeTransfersTelefonistaAsync(Crear.Usuario(), 3, "l", "RO", "f", "r", "E", Productos());

            r.Select(x => x.web_Sucursal).Should().Equal("RO", "RO");
        }

        [Fact]
        public void deberia_lanzar_NullReferenceException_cuando_usuario_es_null_en_pedido_de_telefonista()
        {
            var act = () => capaDLL.TomarPedidoTelefonistaAsync(null, 3, "l", "RO", "f", "r", "E", Productos(), false);

            act.Should().Throw<NullReferenceException>();
            FakeDb.Llamadas.Should().BeEmpty();
        }

        [Fact]
        public void deberia_enviar_solo_vacunas_con_unidades_vendidas()
        {
            Http.ResponderJson("true");
            var vacunas = new List<cVacuna>
            {
                new cVacuna { NombreProducto = "A", UnidadesVendidas = 0 },
                new cVacuna { NombreProducto = "B", UnidadesVendidas = 1 },
                new cVacuna { NombreProducto = "C", UnidadesVendidas = -1 },
            };

            capaDLL.AgregarVacunas(vacunas, "tel").Should().BeTrue();

            var cuerpo = JsonDocument.Parse(Http.Llamadas.Single().Cuerpo).RootElement;
            cuerpo.GetProperty("pVacunas").EnumerateArray().Select(v => v.GetProperty("NombreProducto").GetString()).Should().Equal("B");
        }

        [Fact]
        public void deberia_devolver_false_y_loguear_cuando_lista_de_vacunas_es_null()
        {
            capaDLL.AgregarVacunas(null).Should().BeFalse();
            // El log recibe menos valores que parámetros: además se registra un error de getParameters.
            FakeDb.ErroresLogueados.Select(l => l.Param("err_Nombre")).Should().Equal("getParameters", "AgregarVacunas");
            Http.Llamadas.Should().BeEmpty();
        }

        [Fact]
        public void deberia_lanzar_NullReferenceException_desde_el_catch_cuando_cliente_es_null()
        {
            // CARACTERIZACIÓN: posible bug - el catch vuelve a usar pCliente.cli_login para loguear y lanza otra NullReferenceException.
            var act = () => capaDLL.ObtenerSolicitudesDevolucionCliente(null);

            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void deberia_devolver_false_y_loguear_cuando_falla_validacion_de_carrito_pasado()
        {
            Http.ResponderJson("not json");

            capaDLL.ValidarExistenciaDeCarritoWebPasado(1).Should().BeFalse();
            FakeDb.ErroresLogueados.Should().ContainSingle();
        }
    }

    /// <summary>
    /// Golden master de capaDLL: qué endpoint llama cada método, qué llamadas a la base hace y qué devuelve
    /// ante una respuesta con objeto por defecto y ante un error HTTP 500.
    /// </summary>
    public class capaDLLContratoTests : capaAPITestBase
    {
        public capaDLLContratoTests()
        {
            FakeDb.Cuando("CAR.spInicioCarritoEnProceso").Salida("@isOk", true);
            FakeDb.Cuando("CAR.spEndCarritoEnProceso").Salida("@isOk", true);
        }

        private static object ValorDe(Type t, string nombre)
        {
            if (t == typeof(string)) return "v_" + nombre;
            if (t == typeof(int)) return 7;
            if (t == typeof(bool)) return true;
            if (t == typeof(DateTime)) return new DateTime(2024, 3, 15);
            if (t == typeof(cClientes)) return Crear.Cliente();
            if (t == typeof(Usuario)) return Crear.Usuario(u => u.usu_login = "tel");
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                var lista = (IList)Activator.CreateInstance(t);
                var item = Activator.CreateInstance(t.GetGenericArguments()[0]);
                if (item is cVacuna v) v.UnidadesVendidas = 1;
                lista.Add(item);
                return lista;
            }
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }

        private static string Respuesta(Type t)
        {
            if (t == typeof(void)) return "{}";
            if (t == typeof(string)) return "\"texto\"";
            if (t == typeof(bool)) return "true";
            if (t == typeof(List<string>)) return "[\"texto\"]";
            var sub = Nullable.GetUnderlyingType(t);
            if (sub == typeof(long)) return "12";
            if (sub != null) return "12.5";
            if (typeof(IEnumerable).IsAssignableFrom(t)) return "[{}]";
            return "{}";
        }

        [Fact]
        public async Task deberia_mantener_el_contrato_de_todos_los_metodos()
        {
            var metodos = typeof(capaDLL).GetMethods(BindingFlags.Public | BindingFlags.Static).OrderBy(m => m.Name).ThenBy(m => m.GetParameters().Length).ToList();
            var contrato = new List<object>();
            foreach (var m in metodos)
            {
                var args = m.GetParameters().Select(p => ValorDe(p.ParameterType, p.Name)).ToArray();

                Http.Llamadas.Clear(); FakeDb.Llamadas.Clear();
                Http.ResponderJson(Respuesta(m.ReturnType));
                var ok = Invocar(m, args);
                var endpoints = Http.Llamadas.Select(l => l.Url.Replace(Core, "DKcore:").Replace(Dll, "DKdll:")).ToList();
                var db = FakeDb.Llamadas.Where(l => l.Tipo != "Open").Select(l => l.Comando).ToList();

                Http.Llamadas.Clear(); FakeDb.Llamadas.Clear();
                Http.ResponderStatus(HttpStatusCode.InternalServerError);
                var error = Invocar(m, args);
                var dbError = FakeDb.Llamadas.Where(l => l.Tipo != "Open").Select(l => l.Comando).ToList();

                contrato.Add(new { Metodo = m.Name, Endpoints = endpoints, Base = db, ConRespuesta = ok, ConError500 = error, BaseConError = dbError });
            }
            metodos.Should().HaveCountGreaterThan(45);
            await Verifier.Verify(contrato);
        }

        private static string Invocar(MethodInfo m, object[] args)
        {
            try
            {
                var r = m.Invoke(null, args);
                return m.ReturnType == typeof(void) ? "void" : r == null ? "null" : r is string s ? s : r.GetType().Name + " " + JsonSerializer.Serialize(r);
            }
            catch (TargetInvocationException ex) { return "EXCEPCION " + ex.InnerException.GetType().Name; }
        }
    }
}
