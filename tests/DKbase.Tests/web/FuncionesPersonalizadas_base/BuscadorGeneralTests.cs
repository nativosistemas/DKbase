using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DKbase.generales;
using DKbase.Tests.TestInfra;
using DKbase.web;
using DKbase.web.capaDatos;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.FuncionesPersonalizadas
{
    /// <summary>Buscador general de productos: origen de datos (texto / oferta / transfer) y filtros por tipo de cliente.</summary>
    public class BuscadorGeneralTests : TestBase
    {
        private const string SpBuscador = "Productos.spRecuperarTodosProductosBuscadorV3";
        private const string SpOferta = "Productos.spRecuperarTodosProductosBuscadorEnOferta";
        private const string SpTransfer = "Productos.spRecuperarTodosProductosBuscadorEnTransfer";

        private static DataTable Productos(params (string nombre, string tipo)[] p)
        {
            var t = FakeDb.TablaUniversal();
            var plantilla = t.Rows[0].ItemArray;
            t.Rows.Clear();
            foreach (var x in p)
            {
                var f = t.NewRow();
                f.ItemArray = plantilla;
                f["pro_nombre"] = x.nombre;
                f["pro_codigo"] = "C_" + x.nombre;
                f["pro_codtpopro"] = x.tipo;
                t.Rows.Add(f);
            }
            return t;
        }

        private static DataTable Stocks()
        {
            var t = FakeDb.TablaUniversal();
            t.Rows.Clear();
            foreach (var c in new[] { "stk_codpro", "stk_codsuc", "stk_stock" })
                if (!t.Columns.Contains(c)) t.Columns.Add(c, typeof(string));
            // cargarProductosBuscadorArchivos descarta los productos sin fila de stock.
            foreach (var n in new[] { "MEDICAMENTO", "PERFUME", "CUENTA_Y_ORDEN", "A", "B", "T" })
            {
                var f = t.NewRow();
                f["stk_codpro"] = "C_" + n; f["stk_codsuc"] = "CC"; f["stk_stock"] = "S";
                t.Rows.Add(f);
            }
            return t;
        }

        private static readonly (string, string)[] Mezcla = { ("MEDICAMENTO", "M"), ("PERFUME", Constantes.cTIPOPRODUCTO_Perfumeria), ("CUENTA_Y_ORDEN", Constantes.cTIPOPRODUCTO_PerfumeriaCuentaYOrden) };

        private static cClientes Cliente(string tipo, bool tomaPerfumeria) => Crear.Cliente(c =>
        {
            c.cli_tipo = tipo switch { "PERFUMERIA" => Constantes.cTipoCliente_Perfumeria, "TODOS" => Constantes.cTipoCliente_Todos, _ => "?" };
            c.cli_tomaPerfumeria = tomaPerfumeria;
        });

        public static IEnumerable<object[]> Filtros() => new[]
        {
            new object[] { "PERFUMERIA", true, new[] { "PERFUME", "CUENTA_Y_ORDEN" } },
            new object[] { "PERFUMERIA", false, new[] { "PERFUME", "CUENTA_Y_ORDEN" } },
            new object[] { "TODOS", true, new[] { "MEDICAMENTO", "PERFUME", "CUENTA_Y_ORDEN" } },
            new object[] { "TODOS", false, new[] { "MEDICAMENTO" } },
            new object[] { "OTRO", false, new[] { "MEDICAMENTO", "PERFUME", "CUENTA_Y_ORDEN" } },
        };

        [Theory]
        [MemberData(nameof(Filtros))]
        public void deberia_filtrar_productos_por_tipo_de_cliente_cuando_busca_por_texto(string tipo, bool tomaPerfumeria, string[] esperados)
        {
            // Tables[2] (coincidencias exactas) + Tables[0] (resto), Tables[1] stocks.
            FakeDb.Cuando(SpBuscador).Devolver(Productos(Mezcla[0]), Stocks(), Productos(Mezcla[1], Mezcla[2]));

            var r = FuncionesPersonalizadas_base.RecuperarProductosGeneral_V3(new List<string> { "CC" }, Cliente(tipo, tomaPerfumeria), null, "ibu", new List<string> { "pro_nombre" }, false, false);

            r.listaSucursal.Should().Equal("CC");
            r.listaProductos.Select(p => p.pro_nombre).Should().BeEquivalentTo(esperados);
        }

        [Fact]
        public void deberia_poner_primero_las_coincidencias_de_la_tabla_2()
        {
            FakeDb.Cuando(SpBuscador).Devolver(Productos(("B", "M")), Stocks(), Productos(("A", "M")));

            var r = FuncionesPersonalizadas_base.RecuperarProductosGeneral_V3(new List<string> { "CC" }, Cliente("OTRO", true), null, "x", new List<string>(), false, false);

            r.listaProductos.Select(p => p.pro_nombre).Should().Equal("A", "B");
            FakeDb.De(SpBuscador).Should().ContainSingle();
        }

        [Fact]
        public void deberia_usar_el_sp_de_oferta_cuando_se_informa_id_de_oferta()
        {
            // CARACTERIZACIÓN: posible bug - si la base devuelve menos de 3 tablas, Tables[2] lanza IndexOutOfRangeException sin manejar.
            FluentActions.Invoking(() => FuncionesPersonalizadas_base.RecuperarProductosGeneral_V3(new List<string> { "CC" }, Cliente("OTRO", true), 15, "x", new List<string>(), true, false))
                .Should().Throw<IndexOutOfRangeException>();

            FakeDb.SinLogs.Select(l => l.Comando).Should().Equal("Productos.spRecuperarTodosProductosBuscadorOferta");
            FakeDb.Unica("Productos.spRecuperarTodosProductosBuscadorOferta").Parametros.Values.Should().Contain(15);
        }

        [Fact]
        public void deberia_devolver_null_sin_consultar_cuando_no_hay_cliente()
        {
            FuncionesPersonalizadas_base.RecuperarProductosGeneral_V3(new List<string>(), null, null, "x", new List<string>(), false, false).Should().BeNull();
            FakeDb.Llamadas.Should().BeEmpty();
        }

        [Theory]
        [MemberData(nameof(Filtros))]
        public void deberia_filtrar_por_tipo_de_cliente_cuando_lista_productos_en_oferta(string tipo, bool tomaPerfumeria, string[] esperados)
        {
            FakeDb.Cuando(SpOferta).Devolver(Productos(Mezcla), Stocks());

            var r = FuncionesPersonalizadas_base.RecuperarProductosGeneral_OfertaTransfer(new List<string> { "CC" }, Cliente(tipo, tomaPerfumeria), true, false);

            r.listaProductos.Select(p => p.pro_nombre).Should().BeEquivalentTo(esperados);
            FakeDb.SinLogs.Select(l => l.Comando).Should().StartWith(SpOferta);
        }

        [Fact]
        public void deberia_usar_el_sp_de_transfer_y_devolver_null_cuando_no_es_oferta_ni_transfer()
        {
            FuncionesPersonalizadas_base.RecuperarProductosGeneral_OfertaTransfer(new List<string> { "CC" }, Cliente("OTRO", true), false, false).Should().BeNull();
            FakeDb.SinLogs.Should().BeEmpty();

            FakeDb.Cuando(SpTransfer).Devolver(Productos(("T", "M")), Stocks());
            FuncionesPersonalizadas_base.RecuperarProductosGeneral_OfertaTransfer(new List<string> { "CC" }, Cliente("OTRO", true), false, true);
            FakeDb.De(SpTransfer).Should().ContainSingle();
        }
    }
}
