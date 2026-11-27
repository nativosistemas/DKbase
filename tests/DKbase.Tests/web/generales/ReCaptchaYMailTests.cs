using System;
using System.Collections.Generic;
using System.Linq;
using DKbase.Tests.TestInfra;
using DKbase.web.generales;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.generales
{
    public class ReCaptchaClassTests : TestBase
    {
        [Fact]
        public void deberia_validar_sin_llamar_a_google_cuando_esta_en_modo_desarrollo()
        {
            Helper.isModoDev = true;

            ReCaptchaClass.Validate("token").Should().BeTrue();
            FakeWeb.Urls.Should().BeEmpty();
        }

        [Theory]
        [InlineData("{\"success\": true}", true)]
        [InlineData("{\"success\": TRUE}", true)]
        [InlineData("{\"success\": false}", false)]
        [InlineData("", false)]
        [InlineData("{\"success\": false, \"hostname\": \"true.example.com\"}", true)]
        public void deberia_aceptar_cuando_la_respuesta_contiene_true(string respuesta, bool esperado)
        {
            // El último caso documenta que basta con que aparezca "true" en cualquier parte de la respuesta.
            FakeWeb.Responder = _ => respuesta;

            ReCaptchaClass.Validate("tok").Should().Be(esperado);
        }

        [Fact]
        public void deberia_llamar_a_siteverify_con_el_token_sin_codificar()
        {
            FakeWeb.Responder = _ => "true";

            ReCaptchaClass.Validate("a b&c=d");

            FakeWeb.Urls.Should().ContainSingle().Which.Should()
                .StartWith("https://www.google.com/recaptcha/api/siteverify?secret=")
                .And.EndWith("&response=a b&c=d");
        }

        [Fact]
        public void deberia_usar_la_clave_leida_al_cargar_la_clase_aunque_cambie_la_configuracion()
        {
            // CARACTERIZACIÓN: posible bug - la clave secreta se copia de Helper en el inicializador estático;
            // cambios posteriores de Helper.getReCAPTCHA_ClaveSecreta no se ven.
            FakeWeb.Responder = _ => "true";
            ReCaptchaClass.Validate("x");
            var primera = FakeWeb.Urls[0];

            Helper.getReCAPTCHA_ClaveSecreta = "clave-nueva-" + Guid.NewGuid();
            ReCaptchaClass.Validate("x");

            FakeWeb.Urls[1].Should().Be(primera);
        }

        [Fact]
        public void deberia_devolver_false_y_loguear_cuando_falla_la_llamada()
        {
            FakeWeb.Responder = null; // sin red: lanza WebException

            ReCaptchaClass.Validate("x").Should().BeFalse();
            FakeDb.ErroresLogueados.Should().ContainSingle().Which.Param("err_Nombre").Should().Be("Validate");
        }

        [Fact]
        public void deberia_exponer_propiedad_success_de_lectura_y_escritura()
        {
            new ReCaptchaClass { success = "true" }.success.Should().Be("true");
        }
    }

    public class cMail_baseTests : TestBase
    {
        public cMail_baseTests()
        {
            Helper.getMail_from = "no-reply@kellerhoff.example";
            Helper.getMail_pass = "secreto";
            Helper.getSMTP_SERVER = "smtp.example";
            Helper.getSMTP_PORT = 587;
        }

        [Fact]
        public void deberia_enviar_mail_html_con_remitente_y_servidor_configurados()
        {
            cMail_base.enviarMail("cliente@example.com", "Asunto", "<b>Cuerpo</b>").Should().BeTrue();

            var m = FakeSmtp.Enviados.Should().ContainSingle().Subject;
            m.Should().BeEquivalentTo(new MailEnviado
            {
                Host = "smtp.example", Puerto = 587, De = "no-reply@kellerhoff.example",
                Para = new List<string> { "cliente@example.com" }, Asunto = "Asunto", Cuerpo = "<b>Cuerpo</b>", EsHtml = true,
            });
        }

        [Fact]
        public void deberia_enviar_un_solo_mail_a_todos_los_destinatarios()
        {
            cMail_base.enviarMail_generico(new List<string> { "a@example.com", "b@example.com" }, "s", "c").Should().BeTrue();

            FakeSmtp.Enviados.Single().Para.Should().Equal("a@example.com", "b@example.com");
        }

        [Fact]
        public void deberia_enviar_igual_cuando_lista_de_destinatarios_es_vacia()
        {
            // El SmtpClient real fallaría sin destinatarios; aquí se documenta que no hay validación previa.
            cMail_base.enviarMail_generico(new List<string>(), "s", "c").Should().BeTrue();
            FakeSmtp.Enviados.Single().Para.Should().BeEmpty();
        }

        [Theory]
        [InlineData("no-es-mail")]
        [InlineData("")]
        public void deberia_devolver_false_sin_log_cuando_destinatario_es_invalido(string destino)
        {
            cMail_base.enviarMail(destino, "s", "c").Should().BeFalse();
            FakeSmtp.Enviados.Should().BeEmpty();
            FakeDb.ErroresLogueados.Should().BeEmpty();
        }

        [Fact]
        public void deberia_devolver_false_cuando_no_hay_remitente_configurado()
        {
            Helper.getMail_from = null;

            cMail_base.enviarMail("a@example.com", "s", "c").Should().BeFalse();
        }

        [Fact]
        public void deberia_devolver_false_sin_log_cuando_falla_el_servidor_smtp()
        {
            // CARACTERIZACIÓN: posible bug - cualquier error de envío se traga sin registrar nada.
            FakeSmtp.Error = new System.Net.Mail.SmtpException("caído");

            cMail_base.enviarMail("a@example.com", "s", "c").Should().BeFalse();
            FakeDb.ErroresLogueados.Should().BeEmpty();
        }

        [Fact]
        public void deberia_devolver_false_cuando_lista_de_destinatarios_es_null()
        {
            // El foreach sobre null está dentro del try: se traga y devuelve false.
            cMail_base.enviarMail_generico(null, "s", "c").Should().BeFalse();
        }
    }
}
