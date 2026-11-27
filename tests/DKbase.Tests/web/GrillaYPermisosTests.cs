using System;
using System.Collections.Generic;
using System.Linq;
using DKbase.Tests.TestInfra;
using DKbase.web;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web
{
    /// <summary>Ordenamiento de grillas (ordenamientoExpresion / GetUsuariosDeCliente) y permisos por rol (ListaAcccionesRol).</summary>
    public class GrillaYPermisosTests : TestBase
    {
        // ---------- ordenamientoExpresion ----------
        [Theory]
        [InlineData(null, false, "", true)]
        [InlineData("", false, "", true)]
        [InlineData("usu_nombre", true, "usu_nombre", true)]
        [InlineData("usu_nombre ASC", true, "usu_nombre", true)]
        [InlineData("usu_nombre DESC", true, "usu_nombre", false)]
        [InlineData("usu_nombreDESC", true, "usu_nombre", false)]       // sin espacio también se reconoce
        [InlineData("  usu_nombre  ", true, "usu_nombre", true)]
        [InlineData("usu_nombre desc", true, "usu_nombre desc", true)] // sensible a mayúsculas
        [InlineData("DESC", true, "DESC", true)]                        // largo <= 4: se toma como campo
        [InlineData("xASC", true, "xASC", true)]
        [InlineData("yDESC", true, "y", false)]
        [InlineData("CLASC", true, "CL", true)]
        public void deberia_interpretar_la_expresion_de_orden_cuando_se_construye(string expr, bool isOrder, string campo, bool asc)
        {
            var o = new ordenamientoExpresion(expr);

            o.isOrderBy.Should().Be(isOrder);
            o.OrderByField.Should().Be(campo);
            o.OrderByAsc.Should().Be(asc);
        }

        [Fact]
        public void deberia_tratar_como_ascendente_un_campo_que_termina_en_ASC()
        {
            // CARACTERIZACIÓN: posible bug - un campo cuyo nombre termina en "ASC" (p. ej. "cod_BASC") pierde esas letras.
            new ordenamientoExpresion("cod_BASC").OrderByField.Should().Be("cod_B");
        }

        // ---------- GetUsuariosDeCliente (dos implementaciones duplicadas) ----------
        public static IEnumerable<object[]> Implementaciones() => new[]
        {
            new object[] { "AccesoGrilla_base" },
            new object[] { "Util" },
        };

        private static List<cUsuario> Usuarios(string impl, string orden, string filtro = "f") => impl == "Util"
            ? DKbase.Util.GetUsuariosDeCliente(orden, 1234, filtro)
            : AccesoGrilla_base.GetUsuariosDeCliente(orden, 1234, filtro);

        private static object FilaUsuario(int codigo, string nombre, string apellido, string mail, string login, string nya, string rol, int estado) => new
        {
            usu_codigo = codigo, usu_codRol = 3, usu_codCliente = (object)1234, cli_nombre = "Farmacia", usu_nombre = nombre, usu_apellido = apellido,
            NombreYapellido = nya, usu_login = login, usu_mail = mail, usu_pswDesencriptado = "pw", usu_observacion = "obs", usu_estado = estado, rol_Nombre = rol,
        };

        private void ConUsuarios()
        {
            FakeDb.Cuando("Recursos.spRecuperarUsuariosPorIdCliente").Devolver(Tabla.De(
                FilaUsuario(1, "Beto", "Zapata", "c@x", "lb", "Beto Zapata", "R2", 3),
                FilaUsuario(2, "Ana", "Moreno", "a@x", "lc", "Ana Moreno", "R3", 0),
                FilaUsuario(3, "Carla", "Alvarez", "b@x", "la", "Carla Alvarez", "R1", 2)));
        }

        public static IEnumerable<object[]> Ordenes()
        {
            foreach (var impl in new[] { "AccesoGrilla_base", "Util" })
            {
                yield return new object[] { impl, "usu_nombre", new[] { 2, 1, 3 } };
                yield return new object[] { impl, "usu_nombre DESC", new[] { 3, 1, 2 } };
                yield return new object[] { impl, "usu_apellido", new[] { 3, 2, 1 } };
                yield return new object[] { impl, "usu_apellido DESC", new[] { 1, 2, 3 } };
                yield return new object[] { impl, "usu_mail", new[] { 2, 3, 1 } };
                yield return new object[] { impl, "usu_mail DESC", new[] { 1, 3, 2 } };
                yield return new object[] { impl, "usu_login", new[] { 3, 1, 2 } };
                yield return new object[] { impl, "usu_login DESC", new[] { 2, 1, 3 } };
                yield return new object[] { impl, "NombreYapellido", new[] { 2, 1, 3 } };
                yield return new object[] { impl, "NombreYapellido DESC", new[] { 3, 1, 2 } };
                yield return new object[] { impl, "rol_Nombre", new[] { 3, 1, 2 } };
                yield return new object[] { impl, "rol_Nombre DESC", new[] { 2, 1, 3 } };
                yield return new object[] { impl, "campo_desconocido", new[] { 1, 2, 3 } };
                yield return new object[] { impl, "campo_desconocido DESC", new[] { 1, 2, 3 } };
                yield return new object[] { impl, null, new[] { 1, 2, 3 } };
            }
        }

        [Theory]
        [MemberData(nameof(Ordenes))]
        public void deberia_ordenar_usuarios_por_el_campo_pedido_cuando_hay_expresion(string impl, string orden, int[] esperado)
        {
            ConUsuarios();

            Usuarios(impl, orden).Select(u => u.usu_codigo).Should().Equal(esperado);
        }

        [Theory]
        [MemberData(nameof(Implementaciones))]
        public void deberia_ordenar_por_estado_como_texto_cuando_se_pide_usu_estadoToString(string impl)
        {
            ConUsuarios();

            // estados: 1 → 3 (Inactivo), 2 → 0 (sin texto), 3 → 2 (Activo)
            var r = Usuarios(impl, "usu_estadoToString");
            r.Select(u => u.usu_estadoToString).Should().Equal("", DKbase.generales.Constantes.cESTADO_STRING_ACTIVO, DKbase.generales.Constantes.cESTADO_STRING_INACTIVO);
            r.Select(u => u.usu_codigo).Should().Equal(2, 3, 1);
            Usuarios(impl, "usu_estadoToString DESC").Select(u => u.usu_codigo).Should().Equal(1, 3, 2);
        }

        [Theory]
        [MemberData(nameof(Implementaciones))]
        public void deberia_enviar_rol_operador_cliente_y_filtro_vacio_cuando_filtro_es_null(string impl)
        {
            Usuarios(impl, null, null).Should().BeEmpty();

            var l = FakeDb.Unica("Recursos.spRecuperarUsuariosPorIdCliente");
            l.Param("@usu_codRol").Should().Be(DKbase.generales.Constantes.cROL_OPERADORCLIENTE);
            l.Param("@usu_codCliente").Should().Be(1234);
            l.Param("@filtro").Should().Be("");
        }

        [Theory]
        [MemberData(nameof(Implementaciones))]
        public void deberia_cargar_permisos_denegados_en_mayusculas_por_cada_usuario(string impl)
        {
            ConUsuarios();
            FakeDb.Cuando(SpSinPermisos()).Devolver(Tabla.De(new { usp_id = 1, usp_codUsuario = 1, usp_nombreSeccion = "pedidos" }));

            var r = Usuarios(impl, null);

            r.Should().OnlyContain(u => u.listaPermisoDenegados.SequenceEqual(new[] { "PEDIDOS" }));
            FakeDb.De("Seguridad.spRecuperarSinPermisoUsuarioIntranetPorIdUsuario").Select(l => l.Param("@idUsuario")).Should().Equal(1, 2, 3);
        }

        private static string SpSinPermisos() => "Seguridad.spRecuperarSinPermisoUsuarioIntranetPorIdUsuario";

        [Theory]
        [InlineData(1, "SIN")]
        [InlineData(2, "ACT")]
        [InlineData(3, "INA")]
        [InlineData(0, "")]
        [InlineData(4, "")]
        [InlineData(-1, "")]
        public void deberia_traducir_el_estado_a_texto_cuando_es_conocido(int estado, string esperado)
        {
            var mapa = new Dictionary<string, string>
            {
                ["SIN"] = DKbase.generales.Constantes.cESTADO_STRING_SINESTADO,
                ["ACT"] = DKbase.generales.Constantes.cESTADO_STRING_ACTIVO,
                ["INA"] = DKbase.generales.Constantes.cESTADO_STRING_INACTIVO,
                [""] = "",
            };
            DKbase.web.capaDatos.capaSeguridad_base.obtenerStringEstado(estado).Should().Be(mapa[esperado]);
        }

        // ---------- ListaAcccionesRol ----------
        private static ListaAcccionesRol Permisos()
        {
            var l = new ListaAcccionesRol();
            l.Agregar(new AcccionesRol { palabraClave = "PEDIDOS", isActivo = true, isAgregar = true, isEditar = false, isEliminar = true });
            l.Agregar(new AcccionesRol { palabraClave = "PEDIDOS", isActivo = false });
            return l;
        }

        [Theory]
        [InlineData("PEDIDOS", true, true, false, true)]
        [InlineData("pedidos", false, false, false, false)] // sensible a mayúsculas
        [InlineData("OTRA", false, false, false, false)]
        [InlineData(null, false, false, false, false)]
        public void deberia_responder_permisos_de_la_primera_regla_que_coincide(string clave, bool activo, bool agregar, bool editar, bool eliminar)
        {
            var p = Permisos();

            p.isActivo(clave).Should().Be(activo);
            p.isAgregar(clave).Should().Be(agregar);
            p.isEditar(clave).Should().Be(editar);
            p.isEliminar(clave).Should().Be(eliminar);
        }

        [Fact]
        public void deberia_devolver_una_accion_vacia_nueva_cuando_no_encuentra_la_clave()
        {
            var r = Permisos().Buscar("NADA");
            r.Should().BeEquivalentTo(new AcccionesRol());
        }

        [Fact]
        public void deberia_mapear_acciones_del_rol_con_nulos_como_false()
        {
            FakeDb.Cuando("Seguridad.spRecuperarAccionesUsuario").Devolver(Tabla.De(
                new { rgl_codRegla = 1, rgl_PalabraClave = "A", rrr_codRelacionRolRegla = (object)10, rrr_IsActivo = (object)true, rrr_IsAgregar = (object)true, rrr_IsEditar = (object)false, rrr_IsEliminar = (object)true },
                new { rgl_codRegla = 2, rgl_PalabraClave = "B", rrr_codRelacionRolRegla = (object)null, rrr_IsActivo = (object)null, rrr_IsAgregar = (object)null, rrr_IsEditar = (object)null, rrr_IsEliminar = (object)null }));

            var r = DKbase.Util.RecuperarTodasAccionesPorIdRol(3);

            FakeDb.Unica("Seguridad.spRecuperarAccionesUsuario").Param("@IdRol").Should().Be(3);
            r.lista.Should().BeEquivalentTo(new[]
            {
                new AcccionesRol { idRegla = 1, palabraClave = "A", idReglaRol = 10, isActivo = true, isAgregar = true, isEditar = false, isEliminar = true },
                new AcccionesRol { idRegla = 2, palabraClave = "B", idReglaRol = null },
            }, o => o.WithStrictOrdering());
        }

        [Fact]
        public void deberia_lanzar_excepcion_cuando_la_base_de_acciones_falla()
        {
            // CARACTERIZACIÓN: posible bug - RecuperarTodasAccionesRol devuelve null ante un error y el foreach lanza NullReferenceException.
            FakeDb.ErrorAlConectar = new InvalidOperationException("caída");

            FluentActions.Invoking(() => DKbase.Util.RecuperarTodasAccionesPorIdRol(3)).Should().Throw<NullReferenceException>();
        }
    }
}
