using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using DKbase.dll;
using DKbase.Tests.TestInfra;
using DKbase.web;
using DKbase.web.capaDatos;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.capaDatos
{
    /// <summary>
    /// Flujo crítico de toma de pedido (capaCAR_WebService_base): bloqueo del carrito, validación contra DKdll,
    /// envío del pedido, registro de faltantes y borrado del carrito.
    /// </summary>
    public class TomarPedidoTests : capaAPITestBase
    {
        private const string SpEnProceso = "CAR.spIsCarritoEnProceso";
        private const string SpBorrar = "CAR.spBorrarCarritoPorId";
        private const string SpGuardar = "CAR.spCargarPedido";
        private const string SpFaltas = "LogRegistro.spGestionFaltasProblemasCrediticios";

        // Respuestas de DKdll por endpoint; ValidarExistencia se responde en orden (antes y después de tomar el pedido).
        private readonly Queue<string> _validaciones = new();
        private string _respuestaPedido = "{\"Error\":\"\"}";

        public TomarPedidoTests()
        {
            FakeDb.Cuando(SpEnProceso).Salida("@isOk", false);
            FakeDb.Cuando(SpBorrar).Salida("@isOk", true);
            Http.PorDefecto = req =>
            {
                var url = req.RequestUri.ToString();
                var cuerpo = url.EndsWith("ValidarExistenciaDeCarritoWebPasado") ? (_validaciones.Count > 0 ? _validaciones.Dequeue() : "false") : _respuestaPedido;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(cuerpo, Encoding.UTF8, "application/json") };
            };
        }

        private static List<cCarrito> Carritos(params (int id, string suc)[] c) => c.Select(x => new cCarrito
        {
            car_id = x.id,
            codSucursal = x.suc,
            lrc_id = 900 + x.id,
            listaProductos = new List<cProductosGenerico> { Crear.ProductoGenerico(p => { p.codProducto = "P001"; p.pro_nombre = "IBU"; p.cantidad = 3; }) },
        }).ToList();

        private cDllPedido Tomar(List<cCarrito> carritos, string sucursal = "CC") =>
            capaCAR_WebService_base.TomarPedidoCarrito_generico(Crear.Usuario(), Crear.Cliente(), carritos, "Cierra 14:00", "C", sucursal, "fac", "rem", "E", false);

        private List<string> UrlsDll() => Http.Llamadas.Select(l => l.Url.Replace(Dll, "")).ToList();

        [Fact]
        public void deberia_devolver_null_sin_llamar_a_nadie_cuando_la_lista_de_carritos_es_null()
        {
            Tomar(null).Should().BeNull();
            FakeDb.Llamadas.Should().BeEmpty();
            Http.Llamadas.Should().BeEmpty();
        }

        [Fact]
        public void deberia_devolver_null_sin_tomar_pedido_cuando_ningun_carrito_es_de_la_sucursal()
        {
            Tomar(Carritos((1, "XX"))).Should().BeNull();
            Http.Llamadas.Should().BeEmpty();
        }

        [Fact]
        public void deberia_devolver_error_carrito_en_proceso_cuando_el_carrito_esta_bloqueado()
        {
            FakeDb.Cuando(SpEnProceso).Salida("@isOk", true);

            var r = Tomar(Carritos((5, "CC")));

            r.Error.Should().Be("Carrito se está procesando.");
            Http.Llamadas.Should().BeEmpty();
            FakeDb.Unica(SpEnProceso).Param("@car_id").Should().Be(5);
        }

        [Fact]
        public void deberia_borrar_el_carrito_y_devolver_error_cuando_el_carrito_ya_fue_facturado()
        {
            _validaciones.Enqueue("true");

            var r = Tomar(Carritos((5, "CC")));

            r.Error.Should().Be("Carrito ya se encuentra facturado.");
            UrlsDll().Should().Equal("ValidarExistenciaDeCarritoWebPasado");
            FakeDb.Unica(SpBorrar).Param("@accion").Should().Be(DKbase.generales.Constantes.cAccionCarrito_BORRAR_CARRRITO_REPETIDO);
            FakeDb.ErroresLogueados.Single(l => (string)l.Param("err_Nombre") == "TomarPedidoCarrito_generico")
                .Param("err_Message").Should().Be("Carrito ya se encuentra facturado. idCarrito: 5");
        }

        [Fact]
        public void deberia_tomar_pedido_guardar_y_borrar_carrito_cuando_todo_sale_bien()
        {
            _validaciones.Enqueue("false"); _validaciones.Enqueue("true");

            var r = Tomar(Carritos((4, "XX"), (5, "CC"), (6, "CC")));

            r.Error.Should().BeEmpty();
            r.Login.Should().Be("Cierra 14:00", "el horario de cierre viaja en la propiedad Login");
            UrlsDll().Should().Equal("ValidarExistenciaDeCarritoWebPasado", "TomarPedidoConIdCarrito", "ValidarExistenciaDeCarritoWebPasado");
            Http.Llamadas[1].Cuerpo.Should().Contain("\"pIdCarrito\":5").And.Contain("IBU");
            FakeDb.Unica(SpBorrar).Param("@car_id").Should().Be(5, "solo se procesa el primer carrito de la sucursal");
            FakeDb.Unica(SpBorrar).Param("@accion").Should().Be(DKbase.generales.Constantes.cAccionCarrito_TOMAR);
            FakeDb.De(SpGuardar).Should().ContainSingle();
            FakeDb.ErroresLogueados.Should().BeEmpty();
        }

        [Fact]
        public void deberia_devolver_error_codigo_01_cuando_dkdll_no_reconoce_el_carrito_despues_de_tomarlo()
        {
            _validaciones.Enqueue("false"); _validaciones.Enqueue("false");

            var r = Tomar(Carritos((5, "CC")));

            r.Error.Should().Be("Se produjo un error. Código interno: 01");
            FakeDb.De(SpGuardar).Should().BeEmpty();
            FakeDb.De(SpBorrar).Should().BeEmpty();
        }

        [Fact]
        public void deberia_limpiar_digitos_y_guiones_del_error_y_no_guardar_cuando_el_pedido_vuelve_con_error()
        {
            _validaciones.Enqueue("false"); _validaciones.Enqueue("true");
            _respuestaPedido = "{\"Error\":\"E-123 Sin stock\"}";

            var r = Tomar(Carritos((5, "CC")));

            r.Error.Should().Be("E Sin stock");
            r.Login.Should().BeNull();
            FakeDb.De(SpGuardar).Should().BeEmpty();
        }

        [Fact]
        public void deberia_no_guardar_cuando_solo_viene_web_error()
        {
            _validaciones.Enqueue("false"); _validaciones.Enqueue("true");
            _respuestaPedido = "{\"Error\":null,\"web_Error\":\"timeout\"}";

            var r = Tomar(Carritos((5, "CC")));

            // web_Error y web_Error_StackTrace comparten el mismo campo.
            r.web_Error_StackTrace.Should().Be("timeout");
            r.Error.Should().BeNull();
            FakeDb.De(SpGuardar).Should().BeEmpty();
        }

        [Fact]
        public void deberia_registrar_faltantes_y_problemas_crediticios_cuando_el_pedido_los_informa()
        {
            _validaciones.Enqueue("false"); _validaciones.Enqueue("true");
            _respuestaPedido = "{\"Error\":\"\",\"Items\":[{\"NombreObjetoComercial\":\"IBU\",\"Faltas\":2},{\"NombreObjetoComercial\":\"OK\",\"Faltas\":0}]," +
                               "\"ItemsConProblemasDeCreditos\":[{\"NombreObjetoComercial\":\"AMOX\",\"Cantidad\":1,\"Faltas\":3},{\"NombreObjetoComercial\":\"CERO\",\"Cantidad\":0,\"Faltas\":0}]}";

            FakeDb.Cuando(SpFaltas).Devolver(Tabla.DeNombre("FaltantesProblemasCrediticios", new { fpc_id = 1 }));

            Tomar(Carritos((5, "CC")));

            var registros = FakeDb.De(SpFaltas).ToList();
            registros.Select(l => (l.Param("fpc_nombreProducto"), l.Param("fpc_cantidad"), l.Param("fpc_tipo"), l.Param("fpc_codCarrito")))
                .Should().Equal(("IBU", 2, DKbase.generales.Constantes.cPEDIDO_FALTANTES, 905), ("AMOX", 4, DKbase.generales.Constantes.cPEDIDO_PROBLEMACREDITICIO, 905));
        }

        [Fact]
        public void deberia_devolver_el_pedido_sin_guardarlo_ni_borrar_el_carrito_cuando_falla_el_registro_de_faltantes()
        {
            // CARACTERIZACIÓN: posible bug - si el SP de faltantes no devuelve la tabla esperada, InsertarFaltantesProblemasCrediticios
            // lanza excepción; el pedido ya fue tomado en DKdll pero no se guarda en CAR.spCargarPedido ni se borra el carrito.
            _validaciones.Enqueue("false"); _validaciones.Enqueue("true");
            _respuestaPedido = "{\"Error\":\"\",\"Items\":[{\"NombreObjetoComercial\":\"IBU\",\"Faltas\":2}]}";

            var r = Tomar(Carritos((5, "CC")));

            r.Should().NotBeNull();
            r.Login.Should().Be("Cierra 14:00");
            FakeDb.De(SpGuardar).Should().BeEmpty();
            FakeDb.De(SpBorrar).Should().BeEmpty();
            FakeDb.ErroresLogueados.Single().Param("err_Message").Should().Be("There is no row at position 0.");
        }

        [Fact]
        public void deberia_devolver_el_pedido_sin_guardarlo_cuando_un_producto_del_carrito_no_tiene_codigo()
        {
            // CARACTERIZACIÓN: posible bug - el XML de guardarPedido no admite codProducto null: el pedido queda tomado en DKdll
            // y el carrito borrado, pero CAR.spCargarPedido nunca se ejecuta (solo queda un log).
            _validaciones.Enqueue("false"); _validaciones.Enqueue("true");
            var carritos = Carritos((5, "CC"));
            carritos[0].listaProductos[0].codProducto = null;

            var r = Tomar(carritos);

            r.Should().NotBeNull();
            FakeDb.De(SpBorrar).Should().ContainSingle();
            FakeDb.De(SpGuardar).Should().BeEmpty();
            FakeDb.ErroresLogueados.Single().Param("err_Message").Should().Be("Value cannot be null. (Parameter 'value')");
        }

        [Fact]
        public void deberia_guardar_el_pedido_con_xml_de_detalle_fecha_actual_y_mensajes()
        {
            _validaciones.Enqueue("false"); _validaciones.Enqueue("true");

            Tomar(Carritos((5, "CC")));

            var g = FakeDb.Unica(SpGuardar);
            g.Param("@car_id").Should().Be(905, "guardarPedido usa lrc_id, no car_id");
            g.Param("@car_codSucursal").Should().Be("CC");
            g.Param("@car_codCliente").Should().Be(1234);
            g.Param("@tipo").Should().Be("C");
            g.Param("@FechaPedido").Should().Be(FakeClock.Ahora);
            g.Param("@MensajeEnFactura").Should().Be("fac");
            g.Param("@MensajeEnRemito").Should().Be("rem");
            g.Param("@TipoEnvio").Should().Be("E");
            g.Param("@IsUrgente").Should().Be(false);
            g.Param("@strXML").Should().Be("<Root><DetallePedido lcp_cantidad=\"3\" codigo=\"P001\" nombre=\"IBU\" codTransfer=\"0\" /></Root>");
        }

        [Fact]
        public void deberia_loguear_y_devolver_null_cuando_un_carrito_no_tiene_lista_de_productos()
        {
            _validaciones.Enqueue("false");
            var carritos = Carritos((5, "CC"));
            carritos[0].listaProductos = null;

            Tomar(carritos).Should().BeNull();
            FakeDb.ErroresLogueados.Single().Param("err_Nombre").Should().Be("TomarPedidoCarrito_generico");
        }

        // ---------- BorrarCarritoPorId_SleepTimer ----------
        [Fact]
        public void deberia_borrar_sin_esperar_cuando_el_primer_intento_funciona()
        {
            capaCAR_base.BorrarCarritoPorId_SleepTimer(5, "A").Should().BeTrue();
            FakeDb.De(SpBorrar).Should().ContainSingle();
            FakeSleep.Esperas.Should().BeEmpty();
        }

        [Fact]
        public void deberia_reintentar_diez_veces_con_esperas_crecientes_y_devolver_false_cuando_nunca_se_puede_borrar()
        {
            FakeDb.Cuando(SpBorrar).Salida("@isOk", false);

            capaCAR_base.BorrarCarritoPorId_SleepTimer(5, "A").Should().BeFalse();

            FakeDb.De(SpBorrar).Should().HaveCount(11);
            FakeSleep.Esperas.Should().HaveCount(10);
            // Las esperas dependen de Random(car_id): son deterministas por carrito y suman varios minutos en el peor caso.
            FakeSleep.Esperas.Sum().Should().BeGreaterThan(60_000);
            capaCAR_base.BorrarCarritoPorId_SleepTimer(5, "A");
            FakeSleep.Esperas.Skip(10).Should().Equal(FakeSleep.Esperas.Take(10), "misma semilla, mismas esperas");
        }

        [Fact]
        public void deberia_cortar_los_reintentos_cuando_un_reintento_funciona()
        {
            FakeDb.CuandoUnaVez(SpBorrar).Salida("@isOk", false);
            FakeDb.CuandoUnaVez(SpBorrar).Salida("@isOk", false);

            capaCAR_base.BorrarCarritoPorId_SleepTimer(5, "A").Should().BeTrue();

            FakeDb.De(SpBorrar).Should().HaveCount(3);
            FakeSleep.Esperas.Should().HaveCount(2);
        }

        // ---------- IsBanderaCodigo ----------
        [Fact]
        public void deberia_considerar_bandera_activa_cuando_no_hay_filas()
        {
            capaCAR_WebService_base.IsBanderaCodigo("BAN").Should().BeTrue();
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(null, true)]
        public void deberia_leer_ban_estado_cuando_la_bandera_existe(bool? estado, bool esperado)
        {
            FakeDb.Cuando(FakeDb_SpBandera()).Devolver(Tabla.De(new { ban_estado = (object)estado }));
            capaCAR_WebService_base.IsBanderaCodigo("BAN").Should().Be(esperado);
        }

        private static string FakeDb_SpBandera()
        {
            capaCAR_WebService_base.IsBanderaCodigo("probe");
            var sp = FakeDb.Llamadas.Last(l => l.Tipo != "Open").Comando;
            FakeDb.Llamadas.Clear();
            return sp;
        }
    }
}
