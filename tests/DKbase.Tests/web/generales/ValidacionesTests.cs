using System;
using System.Collections.Generic;
using DKbase.Tests.TestInfra;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.generales
{
    /// <summary>
    /// Validaciones y Validaciones_base son copias idénticas: los mismos casos se corren contra ambas
    /// para detectar si en la refactorización divergen.
    /// </summary>
    public class ValidacionesTests : TestBase
    {
        public static IEnumerable<object[]> Implementaciones() => new[]
        {
            new object[] { "generales.Validaciones" },
            new object[] { "web.generales.Validaciones_base" },
        };

        private static bool IsMail(string impl, string mail) => impl == "generales.Validaciones"
            ? DKbase.generales.Validaciones.isMail(mail) : DKbase.web.generales.Validaciones_base.isMail(mail);
        private static bool IsNumeric(string impl, object o) => impl == "generales.Validaciones"
            ? DKbase.generales.Validaciones.IsNumeric(o) : DKbase.web.generales.Validaciones_base.IsNumeric(o);

        public static IEnumerable<object[]> Mails()
        {
            var casos = new (string, bool)[]
            {
                ("juan@example.com", true),
                ("juan.perez+tag@sub.example.com.ar", true),
                ("j-p@ex-ample.com", true),
                ("o'neil@example.com", true),
                ("ñandu@example.com", true),     // \w acepta letras unicode
                ("juan@example", false),
                ("juan@@example.com", false),
                ("@example.com", false),
                ("juan example.com", false),
                (" juan@example.com", false),   // espacio antes
                ("juan@example.com ", false),
                ("a@b.c", true),
                ("a@b.c,d@e.f", false),
                ("a@b.cd@e.fg", false),
                ("a@b.c.d.e", true),
                ("", false),
            };
            foreach (var impl in Implementaciones())
                foreach (var (mail, ok) in casos)
                    yield return new object[] { impl[0], mail, ok };
        }

        [Theory]
        [MemberData(nameof(Mails))]
        public void deberia_validar_formato_de_mail(string impl, string mail, bool esperado)
        {
            IsMail(impl, mail).Should().Be(esperado);
        }

        [Theory]
        [MemberData(nameof(Implementaciones))]
        public void deberia_lanzar_ArgumentNullException_cuando_mail_es_null(string impl)
        {
            // CARACTERIZACIÓN: posible bug - isMail(null) lanza excepción en vez de devolver false.
            var act = () => IsMail(impl, null);
            act.Should().Throw<ArgumentNullException>();
        }

        public static IEnumerable<object[]> Numeros()
        {
            var casos = new (object, bool)[]
            {
                (null, false),
                (DateTime.Now, false),
                ((short)1, true), (1, true), (1L, true), (1.5m, true), (1.5f, true), (1.5d, true),
                (true, true),                    // los booleanos cuentan como numéricos
                ("12", true), ("-12.5", true), ("1e3", true), ("1,000", true), (" 7 ", true),
                ("", false), ("abc", false), ("12abc", false),
                ("NaN", true), ("Infinity", true),
                ((byte)1, true),                 // byte no está en la lista pero su ToString se parsea
                ('7', true),
                (new object(), false),
            };
            foreach (var impl in Implementaciones())
                foreach (var (valor, ok) in casos)
                    yield return new object[] { impl[0], valor, ok };
        }

        [Theory]
        [MemberData(nameof(Numeros))]
        public void deberia_indicar_si_el_valor_es_numerico_en_cultura_invariante(string impl, object valor, bool esperado)
        {
            IsNumeric(impl, valor).Should().Be(esperado);
        }

        [Theory]
        [MemberData(nameof(Implementaciones))]
        public void deberia_depender_de_la_cultura_cuando_el_texto_tiene_separadores(string impl)
        {
            UsarCultura("es-AR");

            IsNumeric(impl, "1,5").Should().BeTrue();
            IsNumeric(impl, "1.234,5").Should().BeTrue();
            IsNumeric(impl, "1,2,3").Should().BeFalse();
        }
    }
}
