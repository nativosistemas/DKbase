using System;
using System.Data;
using DKbase.Tests.TestInfra;
using DKbase.web;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.cSubirpedido
{
    /// <summary>Parseo de cada formato de línea de archivo de pedido.</summary>
    public class LeerRenglonesTests : TestBase
    {
        private static DataTable T() => FuncionesPersonalizadas_base.ObtenerDataTableProductosCarritoArchivosPedidos();
        private static string S(object o) => o == DBNull.Value ? null : (string)o;

        // ---------- TXT (tabulado) ----------
        [Fact]
        public void deberia_leer_cantidad_columna_2_y_codigo_barra_columna_5_cuando_es_TXT()
        {
            var r = cSubirpedido_base.LeerRenglonArchivoTXT(T(), "1\tx\t12\tx\tx\t7791234567890");

            r["cantidad"].Should().Be(12);
            S(r["codigobarra"]).Should().Be("7791234567890");
            S(r["codigoalfabeta"]).Should().BeEmpty();
            S(r["troquel"]).Should().BeEmpty();
            S(r["codProducto"]).Should().BeNull();
            r["nroordenamiento"].Should().Be(1);
        }

        [Theory]
        [InlineData("A\tx\t12\tx\tx\t779")]   // primera columna no numérica: es encabezado
        [InlineData(" 1\tx\t12\tx\tx\t779")]
        public void deberia_ignorar_linea_TXT_sin_loguear_cuando_no_empieza_con_digito(string linea)
        {
            cSubirpedido_base.LeerRenglonArchivoTXT(T(), linea).Should().BeNull();
            FakeDb.ErroresLogueados.Should().BeEmpty();
        }

        [Theory]
        [InlineData("1\tx\t12")]              // faltan columnas
        [InlineData("1\tx\tdoce\tx\tx\t779")] // cantidad no numérica
        [InlineData("")]                       // vacía: pRenglon[0] fuera de rango
        public void deberia_devolver_null_y_loguear_cuando_linea_TXT_es_invalida(string linea)
        {
            cSubirpedido_base.LeerRenglonArchivoTXT(T(), linea).Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle().Which.Param("err_Nombre").Should().Be("LeerRenglonArchivoTXT");
        }

        // ---------- ASC (coma) ----------
        [Fact]
        public void deberia_leer_cantidad_columna_2_y_codigo_barra_columna_4_cuando_es_ASC()
        {
            var r = cSubirpedido_base.LeerRenglonArchivoASC(T(), "a,b,3,d,7790001");

            r["cantidad"].Should().Be(3);
            S(r["codigobarra"]).Should().Be("7790001");
        }

        [Theory]
        [InlineData("a,b,3")]
        [InlineData("a,b,tres,d,779")]
        public void deberia_devolver_null_y_loguear_cuando_linea_ASC_es_invalida(string linea)
        {
            cSubirpedido_base.LeerRenglonArchivoASC(T(), linea).Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle();
        }

        // ---------- S (ancho fijo: 5 cantidad, 10 alfabeta, 10 troquel, 13 barra) ----------
        [Fact]
        public void deberia_leer_campos_de_ancho_fijo_cuando_es_formato_S()
        {
            var linea = "00012" + "ALFA000001" + "TROQ000002" + "7791234567890" + "X";

            var r = cSubirpedido_base.LeerRenglonArchivoS(T(), linea);

            r["cantidad"].Should().Be(12);
            S(r["codigoalfabeta"]).Should().Be("ALFA000001");
            S(r["troquel"]).Should().Be("TROQ000002");
            S(r["codigobarra"]).Should().Be("7791234567890");
        }

        [Fact]
        public void deberia_conservar_espacios_cuando_formato_S_tiene_campos_con_blancos()
        {
            var r = cSubirpedido_base.LeerRenglonArchivoS(T(), "    5" + "          " + "  TROQ    " + "779          ");

            r["cantidad"].Should().Be(5);
            S(r["troquel"]).Should().Be("  TROQ    ");
            S(r["codigobarra"]).Should().Be("779          ");
        }

        [Fact]
        public void deberia_devolver_null_y_loguear_cuando_linea_S_tiene_menos_de_38_caracteres()
        {
            // CARACTERIZACIÓN: posible bug - el lector de archivos acepta líneas S desde 22 caracteres, pero el parseo necesita 38.
            cSubirpedido_base.LeerRenglonArchivoS(T(), new string('0', 37)).Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle();
        }

        // ---------- F (4 cliente, 5 cantidad, 13 producto) ----------
        [Fact]
        public void deberia_leer_codigo_de_producto_y_cantidad_cuando_es_formato_F()
        {
            var r = cSubirpedido_base.LeerRenglonArchivoF(T(), "1234" + "00007" + "PRODUCTO00001" + "ZZ");

            r["cantidad"].Should().Be(7);
            S(r["codProducto"]).Should().Be("PRODUCTO00001");
            S(r["codigobarra"]).Should().BeNull();
        }

        [Theory]
        [InlineData("123400007PRODUCTO0000")]  // 21 caracteres
        [InlineData("1234ABCDEPRODUCTO00001")] // cantidad no numérica
        public void deberia_devolver_null_y_loguear_cuando_linea_F_es_invalida(string linea)
        {
            cSubirpedido_base.LeerRenglonArchivoF(T(), linea).Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle();
        }
    }
}
