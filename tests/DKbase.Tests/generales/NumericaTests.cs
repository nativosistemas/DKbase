using System.Globalization;
using DKbase.generales;
using DKbase.Tests.TestInfra;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.generales
{
    /// <summary>Formatos numéricos usados en los archivos TXT de facturas (8 enteros + 2 decimales) y en pantallas.</summary>
    public class NumericaTests : TestBase
    {
        private static decimal D(string s) => decimal.Parse(s, CultureInfo.InvariantCulture);

        [Theory]
        [InlineData("0", "0000000000")]
        [InlineData("1", "0000000100")]
        [InlineData("123.45", "0000012345")]
        [InlineData("10.05", "0000001005")]
        [InlineData("100.10", "0000010010")]
        [InlineData("0.01", "0000000001")]
        [InlineData("0.005", "0000000001")]          // redondeo a 2 decimales "away from zero"
        [InlineData("0.004", "0000000000")]
        [InlineData("99999999.99", "9999999999")]   // máximo que entra en 10 posiciones
        [InlineData("123456789", "12345678900")]    // más de 8 enteros: no se trunca, queda de 11
        public void deberia_formatear_8_enteros_y_2_decimales_sin_separadores(string valor, string esperado)
        {
            Numerica.toString_NumeroTXT_N10(D(valor)).Should().Be(esperado);
        }

        [Theory]
        [InlineData("0.995", "0000000010")]   // debería ser 0000000100 (1,00)
        [InlineData("9.999", "000000091000")] // debería ser 0000001000 (10,00); además queda de 12 caracteres
        public void deberia_corromper_decimales_cuando_el_redondeo_sube_la_parte_entera(string valor, string esperado)
        {
            // CARACTERIZACIÓN: posible bug - la parte decimal se obtiene quitando la parte entera TRUNCADA del texto redondeado;
            // si F2 redondea hacia arriba (0.995 → "1.00") el resultado queda mal.
            Numerica.toString_NumeroTXT_N10(D(valor)).Should().Be(esperado);
        }

        [Theory]
        [InlineData("-5.5", "000000-550")]
        [InlineData("-0.5", "00000000-50")]
        public void deberia_poner_el_signo_en_el_medio_cuando_el_numero_es_negativo(string valor, string esperado)
        {
            // CARACTERIZACIÓN: posible bug - los negativos quedan con el "-" entre ceros y con largo variable.
            Numerica.toString_NumeroTXT_N10(D(valor)).Should().Be(esperado);
        }

        [Theory]
        [InlineData("es-AR")]
        [InlineData("en-US")]
        [InlineData("de-DE")]
        public void deberia_dar_el_mismo_resultado_en_cualquier_cultura(string cultura)
        {
            UsarCultura(cultura);

            Numerica.toString_NumeroTXT_N10(1234.56m).Should().Be("0000123456");
        }

        [Theory]
        [InlineData(null, "0000000000")]
        [InlineData("", "0000000000")]
        [InlineData("123", "0000012300")]
        [InlineData("1.234,5", "0000123450")]   // quita puntos de miles y completa decimales
        [InlineData("1.234,56", "0000123456")]
        [InlineData("0,1", "0000000010")]
        [InlineData("1,234", "00000001234")]    // más de 2 decimales: no se recorta
        [InlineData("1,2,3", "0000000120")]     // solo se mira la segunda parte
        [InlineData(",5", "0000000050")]
        [InlineData("abc", "00000abc00")]       // no valida que sea número
        [InlineData("   ", "00000   00")]
        [InlineData("-12,5", "00000-1250")]
        public void deberia_formatear_texto_con_coma_decimal_sin_separador_de_miles(string valor, string esperado)
        {
            Numerica.toString_NumeroTXT_N10(valor).Should().Be(esperado);
            Numerica.SinSeparaciónDeMilesConDecimal(valor).Should().Be(esperado);
        }

        [Theory]
        [InlineData(8, "123.99", "00000123")]
        [InlineData(8, "-123.99", "0000-123")]
        [InlineData(3, "123456", "123456")]
        [InlineData(0, "5", "5")]
        public void deberia_truncar_y_rellenar_con_ceros_la_parte_entera(int ancho, string valor, string esperado)
        {
            Numerica.toString_ParteEntera(ancho, D(valor)).Should().Be(esperado);
        }

        [Fact]
        public void deberia_usar_ancho_8_cuando_pide_parte_entera_8()
        {
            Numerica.toString_ParteEntera_8(42.7m).Should().Be("00000042");
        }

        [Theory]
        [InlineData("1.5", "50")]
        [InlineData("12.34", "34")]
        [InlineData("0", "00")]
        [InlineData("11.11", "11")]
        public void deberia_devolver_dos_digitos_decimales(string valor, string esperado)
        {
            Numerica.toString_ParteDecimal_2(D(valor)).Should().Be(esperado);
        }

        [Theory]
        [InlineData("abcabc", "b", "X", "aXcabc")]
        [InlineData("abc", "z", "X", "abc")]
        [InlineData("abc", "", "X", "Xabc")]
        [InlineData("", "a", "X", "")]
        [InlineData("aaa", "aa", "", "a")]
        public void deberia_reemplazar_solo_la_primera_aparicion(string texto, string buscar, string reemplazo, string esperado)
        {
            Numerica.ReplaceFirst(texto, buscar, reemplazo).Should().Be(esperado);
        }

        [Theory]
        [InlineData("0", "0,00")]
        [InlineData("1", "1,00")]
        [InlineData("0.5", "0,50")]
        [InlineData("1234.5", "1.234,50")]
        [InlineData("100", "100,00")]
        [InlineData("1000", "1.000,00")]
        [InlineData("999.99", "999,99")]
        [InlineData("1000000", "1.000.000,00")]
        [InlineData("-1234567.891", "-1.234.567,891")]   // no redondea a 2 decimales
        [InlineData("1.123456789123", "1,123456789")]    // máximo 9 decimales
        [InlineData("-1", "-1,00")]
        public void deberia_formatear_con_punto_de_miles_y_coma_decimal(string valor, string esperado)
        {
            Numerica.FormatoNumeroPuntoMilesComaDecimal(D(valor)).Should().Be(esperado);
        }

        [Fact]
        public void deberia_perder_el_cero_entero_cuando_es_negativo_menor_a_uno()
        {
            // CARACTERIZACIÓN: posible bug - -0,5 se formatea como "-,50" (falta el 0 entero).
            Numerica.FormatoNumeroPuntoMilesComaDecimal(-0.5m).Should().Be("-,50");
        }

        [Theory]
        [InlineData("es-AR")]
        [InlineData("en-US")]
        public void deberia_formatear_igual_en_cualquier_cultura(string cultura)
        {
            UsarCultura(cultura);

            Numerica.FormatoNumeroPuntoMilesComaDecimal(-1234.5m).Should().Be("-1.234,50");
        }
    }
}
