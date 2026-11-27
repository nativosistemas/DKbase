using System;
using DKbase.Tests.TestInfra;
using DKbase.web.generales;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.generales
{
    public class Texto_baseTests : TestBase
    {
        [Theory]
        [InlineData("hola mundo", 20, "hola mundo")]                      // más corto que el corte
        [InlineData("hola mundo", 10, "hola mundo")]                      // igual al corte
        [InlineData("hola mundo cruel", 6, "hola mundo&nbsp;... ")]       // corta en el siguiente espacio
        [InlineData("hola mundo cruel", 4, "hola&nbsp;... ")]             // el corte cae justo en un espacio
        [InlineData("holamundocruel", 4, "holamundocruel")]              // sin espacios después del corte: no corta
        [InlineData("", 0, "")]
        [InlineData("abc", 0, "abc")]
        [InlineData("a b", 0, "a&nbsp;... ")]
        public void deberia_cortar_en_el_primer_espacio_despues_del_corte(string texto, int corte, string esperado)
        {
            Texto_base.CortarBajada(texto, corte).Should().Be(esperado);
        }

        [Fact]
        public void deberia_lanzar_excepcion_cuando_corte_es_negativo_y_texto_no_vacio()
        {
            var act = () => Texto_base.CortarBajada("abc", -1);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void deberia_lanzar_NullReferenceException_cuando_texto_es_null()
        {
            var act = () => Texto_base.CortarBajada(null, 3);
            act.Should().Throw<NullReferenceException>();
        }

        [Theory]
        [InlineData("l'oreal", "l\"oreal")]
        [InlineData("''", "\"\"")]
        [InlineData("sin", "sin")]
        [InlineData("", "")]
        public void deberia_cambiar_comilla_simple_por_doble(string texto, string esperado)
        {
            Texto_base.CambiarComillaSimplePorDoble(texto).Should().Be(esperado);
        }

        [Theory]
        [InlineData("factura 001.pdf", "factura001.pdf")]
        [InlineData("año_2024-ñ.txt", "ao_2024.txt")]
        [InlineData("../../etc/passwd", "....etcpasswd")]
        [InlineData("", "")]
        [InlineData("!!!", "")]
        public void deberia_dejar_solo_letras_numeros_guion_bajo_y_punto(string nombre, string esperado)
        {
            Texto_base.limpiarNombreArchivo(nombre).Should().Be(esperado);
        }

        [Theory]
        [InlineData("áàäéèëíìïóòöúùñ", "aaaeeeiiiooouun")]
        [InlineData("ÁÀÄÉÈËÍÌÏÓÒÖÚÙÜÑ", "AAAEEEIIIOOOUUUN")]
        [InlineData("çÇ", "cC")]
        [InlineData("Pingüino", "Pingüino")]    // ver test de ü
        [InlineData("sin acentos 123", "sin acentos 123")]
        [InlineData("", "")]
        [InlineData("ãõ", "ãõ")]                // tildes no contempladas
        public void deberia_reemplazar_vocales_acentuadas_y_enie(string texto, string esperado)
        {
            Texto_base.removerSignosAcentos(texto).Should().Be(esperado);
        }

        [Fact]
        public void deberia_no_quitar_dieresis_de_u_minuscula()
        {
            // CARACTERIZACIÓN: posible bug - la tabla de origen tiene "u" en lugar de "ü" minúscula, así que "ü" no se convierte.
            Texto_base.removerSignosAcentos("ü").Should().Be("ü");
            Texto_base.removerSignosAcentos("Ü").Should().Be("U");
        }

        [Theory]
        [InlineData("<b>hola</b> & 'chau'", "&lt;b&gt;hola&lt;/b&gt; &amp; &#39;chau&#39;")]
        [InlineData("ñ", "&#241;")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void deberia_codificar_html(string texto, string esperado)
        {
            Texto_base.ConvertToStringHTML(texto).Should().Be(esperado);
        }

        [Theory]
        [InlineData("<p>hola <b>mundo</b></p>", "hola mundo")]
        [InlineData("<div\nclass='x'>a</div>", "a")]
        [InlineData("a < b > c", "a  c")]  // no distingue texto de etiquetas
        [InlineData("sin tags", "sin tags")]
        [InlineData("", "")]
        public void deberia_quitar_etiquetas_html(string texto, string esperado)
        {
            Texto_base.SacarTagHtml(texto).Should().Be(esperado);
        }
    }
}
