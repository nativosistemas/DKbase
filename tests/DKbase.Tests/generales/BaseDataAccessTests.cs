using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DKbase.generales;
using DKbase.Tests.TestInfra;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.generales
{
    public class BaseDataAccessTests : TestBase
    {
        private readonly BaseDataAccess _db = new BaseDataAccess(ConnFake);

        [Fact]
        public void deberia_convertir_null_a_DBNull_cuando_crea_parametro()
        {
            var p = _db.GetParameter("x", null);

            p.Value.Should().Be(DBNull.Value);
            p.Direction.Should().Be(ParameterDirection.Input);
        }

        [Fact]
        public void deberia_conservar_valor_y_tipo_cuando_crea_parametro_tipado()
        {
            var p = _db.GetParameter("x", 5, SqlDbType.BigInt);

            p.Value.Should().Be(5);
            p.SqlDbType.Should().Be(SqlDbType.BigInt);
        }

        [Theory]
        [InlineData(SqlDbType.NVarChar, -1)]
        [InlineData(SqlDbType.VarChar, -1)]
        [InlineData(SqlDbType.NText, -1)]
        [InlineData(SqlDbType.Text, -1)]
        [InlineData(SqlDbType.Int, 0)]
        [InlineData(SqlDbType.Char, 0)]
        public void deberia_usar_tamanio_maximo_solo_para_tipos_de_texto_cuando_crea_parametro_de_salida(SqlDbType tipo, int tamanio)
        {
            var p = _db.GetParameterOut("o", tipo);

            p.Size.Should().Be(tamanio);
            p.Direction.Should().Be(ParameterDirection.InputOutput);
            p.Value.Should().Be(DBNull.Value);
        }

        [Fact]
        public void deberia_respetar_valor_y_direccion_cuando_crea_parametro_de_salida()
        {
            var p = _db.GetParameterOut("o", SqlDbType.Int, 3, ParameterDirection.Output);

            p.Value.Should().Be(3);
            p.Direction.Should().Be(ParameterDirection.Output);
        }

        [Fact]
        public void deberia_crear_comando_con_texto_y_tipo()
        {
            var cmd = _db.GetCommand(new SqlConnection(ConnFake), "sp", CommandType.Text);

            cmd.CommandText.Should().Be("sp");
            cmd.CommandType.Should().Be(CommandType.Text);
        }

        [Fact]
        public void deberia_devolver_filas_afectadas_y_enviar_parametros_cuando_ejecuta_non_query()
        {
            FakeDb.Cuando("sp.X").Afectar(3);

            _db.ExecuteNonQuery("sp.X", new List<SqlParameter> { _db.GetParameter("a", 1) }).Should().Be(3);
            FakeDb.Unica("sp.X").Param("a").Should().Be(1);
        }

        [Fact]
        public void deberia_ejecutar_sin_parametros_cuando_lista_es_null_o_vacia()
        {
            _db.ExecuteNonQuery("sp.X", null).Should().Be(0);
            _db.ExecuteNonQuery("sp.X", new List<SqlParameter>()).Should().Be(0);
            FakeDb.De("sp.X").Should().HaveCount(2);
        }

        [Fact]
        public void deberia_devolver_menos_uno_y_loguear_cuando_falla_non_query()
        {
            FakeDb.Cuando("sp.X").Fallar();

            _db.ExecuteNonQuery("sp.X").Should().Be(-1);
            FakeDb.ErroresLogueados.Should().ContainSingle().Which.Param("err_Nombre").Should().Be("ExecuteNonQuery");
        }

        [Fact]
        public void deberia_propagar_la_excepcion_cuando_falla_non_query_para_errores()
        {
            FakeDb.Cuando("sp.X").Fallar(new TimeoutException("timeout"));

            var act = () => _db.ExecuteNonQuery_forError("sp.X", null);

            act.Should().Throw<TimeoutException>().WithMessage("timeout");
        }

        [Fact]
        public void deberia_devolver_escalar_o_null_cuando_falla()
        {
            FakeDb.CuandoUnaVez("sp.S").Escalar(42);
            FakeDb.CuandoUnaVez("sp.S").Fallar();

            _db.ExecuteScalar("sp.S").Should().Be(42);
            _db.ExecuteScalar("sp.S").Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle();
        }

        [Fact]
        public void deberia_cargar_tabla_cuando_ejecuta_GetDataTable()
        {
            FakeDb.Cuando("sp.T").Devolver(Tabla.De(new { a = 1 }, new { a = 2 }));

            _db.GetDataTable("sp.T").Rows.Should().HaveCount(2);
        }

        [Fact]
        public void deberia_devolver_null_y_loguear_cuando_falla_GetDataTable()
        {
            FakeDb.Cuando("sp.T").Fallar();

            _db.GetDataTable("sp.T").Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle();
        }

        [Fact]
        public void deberia_nombrar_tablas_dsGeneric_cuando_ejecuta_GetDataSet()
        {
            FakeDb.Cuando("sp.D").Devolver(Tabla.De(new { a = 1 }), Tabla.De(new { b = 2 }));

            var ds = _db.GetDataSet("sp.D");

            ds.Tables[0].TableName.Should().Be("dsGeneric");
            ds.Tables[1].TableName.Should().Be("dsGeneric1");
        }

        [Fact]
        public void deberia_devolver_null_cuando_falla_GetDataSet_o_GetDataReader()
        {
            FakeDb.ErrorAlConectar = new InvalidOperationException("caída");

            _db.GetDataSet("sp.D").Should().BeNull();
            _db.GetDataReader("sp.D").Should().BeNull();
        }

        [Fact]
        public void deberia_fallar_y_devolver_menos_uno_cuando_cadena_de_conexion_es_invalida()
        {
            var db = new BaseDataAccess("esto no es una cadena de conexión");

            db.ExecuteNonQuery("sp.X").Should().Be(-1);
            FakeDb.SinLogs.Should().BeEmpty();
        }

        [Fact]
        public void deberia_fallar_cuando_se_construye_sin_cadena_de_conexion()
        {
            new BaseDataAccess().ExecuteScalar("sp").Should().BeNull();
        }
    }

    public class Serializador_baseTests : TestBase
    {
        public class Dto { public int Id { get; set; } public string Nombre { get; set; } public Dto Hijo { get; set; } }

        [Fact]
        public void deberia_serializar_y_deserializar_ida_y_vuelta()
        {
            var json = new Dto { Id = 1, Nombre = "ñandú" }.SerializarAJson();

            json.Should().Be("{\"Id\":1,\"Nombre\":\"\\u00F1and\\u00FA\",\"Hijo\":null}");
            json.DeserializarJson<Dto>().Should().BeEquivalentTo(new Dto { Id = 1, Nombre = "ñandú" });
        }

        [Fact]
        public void deberia_serializar_null_como_texto_null()
        {
            ((object)null).SerializarAJson().Should().Be("null");
        }

        [Fact]
        public void deberia_devolver_vacio_y_loguear_cuando_hay_referencia_circular()
        {
            var d = new Dto(); d.Hijo = d;

            d.SerializarAJson().Should().BeEmpty();
            FakeDb.ErroresLogueados.Should().Contain(l => (string)l.Param("err_Nombre") == "SerializarAJson");
        }

        [Theory]
        [InlineData("{no es json")]
        [InlineData("")]
        [InlineData(null)]
        public void deberia_devolver_default_y_loguear_cuando_json_es_invalido(string json)
        {
            json.DeserializarJson<Dto>().Should().BeNull();
            json.DeserializarJson<int>().Should().Be(0);
            FakeDb.ErroresLogueados.Should().HaveCount(2);
        }

        [Fact]
        public void deberia_ser_sensible_a_mayusculas_cuando_deserializa()
        {
            "{\"id\":5}".DeserializarJson<Dto>().Id.Should().Be(0);
        }
    }
}
