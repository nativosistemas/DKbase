using System;
using System.Data;
using DKbase.baseDatos;
using DKbase.Tests.TestInfra;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.baseDatos
{
    public class StoredProcedureTests : TestBase
    {
        [Theory]
        [InlineData(1, true)]
        [InlineData(5, true)]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        public void deberia_indicar_exito_del_spError_segun_filas_afectadas(int filas, bool esperado)
        {
            FakeDb.Cuando("LogRegistro.spError").Afectar(filas);

            StoredProcedure.spError("n", "p", "d", "h", "i", "m", "s", "st", new DateTime(2024, 1, 2), "t").Should().Be(esperado);
        }

        [Fact]
        public void deberia_enviar_los_diez_parametros_del_error()
        {
            StoredProcedure.spError("n", null, "d", "h", "i", "m", "s", "st", new DateTime(2024, 1, 2), "t");

            var l = FakeDb.Unica("LogRegistro.spError");
            l.Parametros.Keys.Should().Equal("err_Nombre", "err_Parameters", "err_Data", "err_HelpLink", "err_InnerException", "err_Message", "err_Source", "err_StackTrace", "err_fecha", "err_tipo");
            l.Param("err_Parameters").Should().BeNull();
            l.Param("err_fecha").Should().Be(new DateTime(2024, 1, 2));
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        public void deberia_indicar_exito_del_spLogInfo_segun_filas_afectadas(int filas, bool esperado)
        {
            FakeDb.Cuando("LogRegistro.spLogInfo").Afectar(filas);

            StoredProcedure.spLogInfo("o", "m", "i", "p", DateTime.Now, "app", "t", "ft", null).Should().Be(esperado);
        }

        [Fact]
        public void deberia_devolver_false_y_escribir_archivo_cuando_falla_la_conexion()
        {
            FakeDb.ErrorAlConectar = new InvalidOperationException("sin conexión");

            StoredProcedure.spError("n", "p", null, null, null, "m", null, null, DateTime.Now, "t").Should().BeFalse();
            System.IO.Directory.GetFiles(System.IO.Path.Combine(Carpeta, "log")).Should().ContainSingle();
        }

        [Fact]
        public void deberia_devolver_tabla_de_sucursales()
        {
            FakeDb.Cuando("Clientes.spRecuperarTodasSucursal").Devolver(Tabla.De(new { suc_codigo = "CC" }));

            StoredProcedure.RecuperarTodasSucursales().Rows[0]["suc_codigo"].Should().Be("CC");
            FakeDb.Unica("Clientes.spRecuperarTodasSucursal").Parametros.Should().BeEmpty();
        }

        [Fact]
        public void deberia_enviar_credenciales_y_datos_de_conexion_al_iniciar_sesion()
        {
            FakeDb.Cuando("Seguridad.spInicioSession").Devolver(Tabla.De(new { usu_codigo = 1 }));

            var t = StoredProcedure.Login("user", "pass", "1.2.3.4", "host", "agent");

            t.Rows.Should().ContainSingle();
            var l = FakeDb.Unica("Seguridad.spInicioSession");
            l.Param("login").Should().Be("user");
            l.Param("Password").Should().Be("pass");
            l.Param("Ip").Should().Be("1.2.3.4");
            l.Param("Host").Should().Be("host");
            l.Param("UserName").Should().Be("agent"); // el user agent viaja como "UserName"
        }

        [Fact]
        public void deberia_devolver_null_y_loguear_cuando_falla_el_login()
        {
            FakeDb.Cuando("Seguridad.spInicioSession").Fallar();

            StoredProcedure.Login("u", "p", "i", "h", "a").Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle().Which.Param("err_Nombre").Should().Be("GetDataTable");
        }
    }
}
