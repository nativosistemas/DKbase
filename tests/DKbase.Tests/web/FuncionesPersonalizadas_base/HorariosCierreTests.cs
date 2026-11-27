using System;
using System.Linq;
using DKbase.Tests.TestInfra;
using DKbase.web;
using FluentAssertions;
using Xunit;

namespace DKbase.Tests.web.FuncionesPersonalizadas
{
    /// <summary>Base común: carga la tabla de horarios de sucursal dependiente que devuelve Clientes.spGestionSucursalHorarios.</summary>
    public abstract class HorariosTestBase : TestBase
    {
        protected const string SpHorarios = "Clientes.spGestionSucursalHorarios";
        // Viernes 15/03/2024
        protected static readonly DateTime Viernes10hs = new DateTime(2024, 3, 15, 10, 0, 0);

        protected static object H(string dia, string horario, string suc = "CC", string dep = "CC", string rep = "R1")
            => new { sdh_SucursalDependienteHorario = (int?)1, sdh_sucursal = suc, sdh_sucursalDependiente = dep, sdh_codReparto = rep, sdh_diaSemana = dia, sdh_horario = horario };

        protected static void Horarios(params object[] filas)
        {
            var t = filas.Length == 0
                ? Tabla.Columnas("sdh_SucursalDependienteHorario", "sdh_sucursal", "sdh_sucursalDependiente", "sdh_codReparto", "sdh_diaSemana", "sdh_horario")
                : Tabla.De(filas);
            FakeDb.Cuando(SpHorarios).Devolver(t);
        }
    }

    public class ObtenerHorarioCierre_baseTests : HorariosTestBase
    {
        private static string Cierre(DateTime ahora, string sucursal = "CC", Action<DKbase.web.capaDatos.cClientes> cli = null)
            => FuncionesPersonalizadas_base.ObtenerHorarioCierre_base(Crear.Cliente(cli), sucursal, ahora);

        [Fact]
        public void deberia_devolver_horario_de_hoy_sin_dia_cuando_todavia_no_cerro()
        {
            Horarios(H("VI", "12:00"));

            Cierre(Viernes10hs).Should().Be("12:00 hs. ");
        }

        [Fact]
        public void deberia_elegir_el_primer_horario_posterior_cuando_hay_varios_en_el_dia()
        {
            Horarios(H("VI", "09:00-12:00-18:00"));

            Cierre(Viernes10hs).Should().Be("12:00 hs. ");
        }

        [Theory]
        [InlineData(29, "10:00 hs. ")]   // el cierre es a las 10:00:30 → a las 10:00:29 sigue abierto
        [InlineData(30, "10:00 hs. SA")] // a las 10:00:30 ya no es posterior → pasa al día siguiente
        [InlineData(31, "10:00 hs. SA")]
        public void deberia_tomar_30_segundos_de_gracia_cuando_compara_horario_de_cierre(int segundos, string esperado)
        {
            Horarios(H("VI", "10:00"), H("SA", "10:00"));

            Cierre(new DateTime(2024, 3, 15, 10, 0, segundos)).Should().Be(esperado);
        }

        [Fact]
        public void deberia_devolver_horario_del_dia_siguiente_con_sigla_cuando_hoy_ya_cerro()
        {
            Horarios(H("VI", "08:00"), H("SA", "09:00"));

            Cierre(Viernes10hs).Should().Be("09:00 hs. SA");
        }

        [Fact]
        public void deberia_saltar_dias_sin_horario_cuando_busca_proximo_cierre()
        {
            Horarios(H("VI", "08:00"), H("LU", "08:30"));

            Cierre(Viernes10hs).Should().Be("08:30 hs. LU");
        }

        [Theory]
        [InlineData(2024, 3, 11, "LU")]
        [InlineData(2024, 3, 12, "MA")]
        [InlineData(2024, 3, 13, "MI")]
        [InlineData(2024, 3, 14, "JU")]
        [InlineData(2024, 3, 15, "VI")]
        [InlineData(2024, 3, 16, "SA")]
        [InlineData(2024, 3, 17, "DO")]
        public void deberia_mapear_cada_dia_de_la_semana_a_su_sigla(int anio, int mes, int dia, string sigla)
        {
            Horarios(H(sigla, "23:00"));

            Cierre(new DateTime(anio, mes, dia, 10, 0, 0)).Should().Be("23:00 hs. ");
        }

        [Fact]
        public void deberia_encontrar_el_mismo_dia_de_la_semana_siguiente_cuando_es_el_unico_horario()
        {
            Horarios(H("VI", "08:00"));

            Cierre(Viernes10hs).Should().Be("08:00 hs. VI");
        }

        [Fact]
        public void deberia_cruzar_fin_de_mes_y_anio_bisiesto_cuando_busca_el_dia_siguiente()
        {
            // Jueves 29/02/2024 (bisiesto) a la noche → viernes 01/03.
            Horarios(H("JU", "08:00"), H("VI", "08:00"));

            Cierre(new DateTime(2024, 2, 29, 23, 0, 0)).Should().Be("08:00 hs. VI");
        }

        [Fact]
        public void deberia_devolver_null_cuando_no_hay_horarios()
        {
            Horarios();

            Cierre(Viernes10hs).Should().BeNull();
        }

        [Theory]
        [InlineData("XX", "CC", "R1")]   // otra sucursal del cliente
        [InlineData("CC", "XX", "R1")]   // otra sucursal dependiente
        [InlineData("CC", "CC", "R9")]   // otro reparto
        public void deberia_ignorar_horarios_de_otra_sucursal_o_reparto(string suc, string dep, string rep)
        {
            Horarios(H("VI", "12:00", suc, dep, rep));

            Cierre(Viernes10hs).Should().BeNull();
        }

        [Fact]
        public void deberia_ignorar_horarios_sin_dos_puntos()
        {
            Horarios(H("VI", "1200-13:00"));

            Cierre(Viernes10hs).Should().Be("13:00 hs. ");
        }

        [Fact]
        public void deberia_ignorar_fila_sin_horario()
        {
            Horarios(H("VI", null), H("SA", "09:00"));

            Cierre(Viernes10hs).Should().Be("09:00 hs. SA");
        }

        [Fact]
        public void deberia_lanzar_FormatException_cuando_horario_no_es_numerico()
        {
            // CARACTERIZACIÓN: posible bug - un horario con formato "ab:cd" lanza FormatException sin manejar.
            Horarios(H("VI", "ab:cd"));

            var act = () => Cierre(Viernes10hs);

            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void deberia_usar_solo_la_primera_fila_del_dia_cuando_hay_filas_repetidas()
        {
            // CARACTERIZACIÓN: posible bug - si hay dos filas para el mismo día, solo se mira la primera (break);
            // el horario de las 18:00 de la segunda fila se ignora y se informa el del día siguiente.
            Horarios(H("VI", "08:00"), H("VI", "18:00"), H("SA", "09:00"));

            Cierre(Viernes10hs).Should().Be("09:00 hs. SA");
        }

        [Fact]
        public void deberia_lanzar_ArgumentNullException_cuando_falla_la_base()
        {
            // CARACTERIZACIÓN: posible bug - si la consulta de horarios falla, el método de datos devuelve null
            // y aquí se hace .Where() sobre null sin control.
            FakeDb.Cuando(SpHorarios).Fallar();

            var act = () => Cierre(Viernes10hs);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void deberia_consultar_horarios_con_accion_select_y_filtros_nulos()
        {
            Horarios(H("VI", "12:00"));

            Cierre(Viernes10hs);

            var llamada = FakeDb.Unica(SpHorarios);
            llamada.Param("@accion").Should().Be(DKbase.generales.Constantes.cSQL_SELECT);
            llamada.Param("@sdh_sucursal").Should().BeNull();
            llamada.Param("@sdh_horario").Should().BeNull();
        }

        // ----- Regla especial reparto S7 en sucursal SF -----
        [Theory]
        [InlineData(2024, 3, 11, 22, 15, 30, "22:15 hs. ")]   // lunes justo en el cierre (no es posterior)
        [InlineData(2024, 3, 11, 22, 15, 31, "22:15 hs. MA")] // lunes después
        [InlineData(2024, 3, 12, 23, 0, 0, "22:15 hs. MI")]
        [InlineData(2024, 3, 13, 23, 0, 0, "22:15 hs. JU")]
        [InlineData(2024, 3, 14, 23, 0, 0, "22:15 hs. VI")]
        [InlineData(2024, 3, 15, 23, 0, 0, "22:15 hs. LU")]   // viernes después → lunes
        [InlineData(2024, 3, 15, 10, 0, 0, "22:15 hs. ")]
        // Límite exacto: a las 22:15:30 todavía no pasó el cierre (comparación estricta); a las 22:15:31 sí.
        [InlineData(2024, 3, 11, 22, 15, 30, "22:15 hs. ")]
        [InlineData(2024, 3, 11, 22, 15, 31, "22:15 hs. MA")]
        [InlineData(2024, 3, 12, 22, 15, 30, "22:15 hs. ")]
        [InlineData(2024, 3, 12, 22, 15, 31, "22:15 hs. MI")]
        [InlineData(2024, 3, 13, 22, 15, 30, "22:15 hs. ")]
        [InlineData(2024, 3, 13, 22, 15, 31, "22:15 hs. JU")]
        [InlineData(2024, 3, 14, 22, 15, 30, "22:15 hs. ")]
        [InlineData(2024, 3, 14, 22, 15, 31, "22:15 hs. VI")]
        [InlineData(2024, 3, 15, 22, 15, 30, "22:15 hs. ")]
        [InlineData(2024, 3, 15, 22, 15, 31, "22:15 hs. LU")]
        [InlineData(2024, 3, 16, 9, 0, 0, "22:15 hs. LU")]    // sábado siempre lunes
        [InlineData(2024, 3, 17, 9, 0, 0, "22:15 hs. LU")]    // domingo siempre lunes
        public void deberia_aplicar_cierre_fijo_2215_cuando_reparto_es_S7_y_sucursal_SF(int a, int m, int d, int hh, int mm, int ss, string esperado)
        {
            Horarios(H("LU", "08:00", dep: "SF", rep: "S7"));

            var r = Cierre(new DateTime(a, m, d, hh, mm, ss), "SF", c => c.cli_codrep = "S7");

            r.Should().Be(esperado);
        }

        [Fact]
        public void deberia_no_aplicar_regla_S7_cuando_sucursal_no_es_SF()
        {
            Horarios(H("VI", "12:00", rep: "S7"));

            Cierre(Viernes10hs, "CC", c => c.cli_codrep = "S7").Should().Be("12:00 hs. ");
        }
    }

    public class ObtenerHorarioCierreTests : HorariosTestBase
    {
        [Fact]
        public void deberia_usar_sucursal_dependiente_y_reloj_actual()
        {
            FakeClock.Ahora = Viernes10hs;
            Horarios(H("VI", "12:00", dep: "SF"));

            var r = FuncionesPersonalizadas_base.ObtenerHorarioCierre(Crear.Cliente(), "ignorada", "SF", "ignorado");

            r.Should().Be("12:00 hs. ");
        }
    }

    public class getFecha_HorarioTests : TestBase
    {
        public getFecha_HorarioTests() { FakeClock.Ahora = new DateTime(2024, 3, 15, 10, 0, 0); } // viernes

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void deberia_devolver_null_cuando_horario_es_vacio(string horario)
        {
            FuncionesPersonalizadas_base.getFecha_Horario(horario).Should().BeNull();
        }

        [Fact]
        public void deberia_devolver_hoy_a_esa_hora_cuando_horario_no_tiene_dia()
        {
            FuncionesPersonalizadas_base.getFecha_Horario("12:30 hs. ").Should().Be(new DateTime(2024, 3, 15, 12, 30, 0));
        }

        [Theory]
        [InlineData("08:00 hs. LU", 2024, 3, 18, 50)]
        [InlineData("08:00 hs. MA", 2024, 3, 19, 50)]
        [InlineData("08:00 hs. MI", 2024, 3, 20, 50)]
        [InlineData("08:00 hs. JU", 2024, 3, 21, 50)]
        [InlineData("08:00 hs. SA", 2024, 3, 16, 50)]
        [InlineData("08:00 hs. DO", 2024, 3, 17, 50)]
        [InlineData("08:00 hs. VI", 2024, 3, 15, 0)]   // hoy es viernes → misma fecha y segundos en 0
        public void deberia_calcular_proxima_fecha_del_dia_indicado(string horario, int a, int m, int d, int segundos)
        {
            // CARACTERIZACIÓN: posible bug - cuando el día es otro, la hora queda con 50 segundos; cuando es hoy, con 0.
            FuncionesPersonalizadas_base.getFecha_Horario(horario).Should().Be(new DateTime(a, m, d, 8, 0, segundos));
        }

        [Fact]
        public void deberia_cruzar_fin_de_mes_cuando_el_dia_cae_en_el_mes_siguiente()
        {
            FakeClock.Ahora = new DateTime(2024, 2, 29, 10, 0, 0); // jueves bisiesto

            FuncionesPersonalizadas_base.getFecha_Horario("08:00 hs. LU").Should().Be(new DateTime(2024, 3, 4, 8, 0, 50));
        }

        [Fact]
        public void deberia_avanzar_8_dias_cuando_la_sigla_del_dia_es_desconocida()
        {
            FuncionesPersonalizadas_base.getFecha_Horario("08:00 hs. XX").Should().Be(new DateTime(2024, 3, 23, 8, 0, 50));
        }

        [Theory]
        [InlineData("8:00 hs. ")]     // 9 caracteres
        [InlineData("12:30 hs.  ")]   // 11 caracteres
        [InlineData("12:30")]
        public void deberia_devolver_null_cuando_largo_no_es_10_ni_12(string horario)
        {
            FuncionesPersonalizadas_base.getFecha_Horario(horario).Should().BeNull();
        }

        [Fact]
        public void deberia_devolver_null_y_loguear_error_cuando_horario_es_invalido()
        {
            var r = FuncionesPersonalizadas_base.getFecha_Horario("ab:cd hs. ");

            r.Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle()
                .Which.Param("err_Nombre").Should().Be("getFecha_Horario");
        }

        [Fact]
        public void deberia_devolver_null_y_loguear_cuando_hora_esta_fuera_de_rango()
        {
            FuncionesPersonalizadas_base.getFecha_Horario("25:00 hs. ").Should().BeNull();
            FakeDb.ErroresLogueados.Should().ContainSingle();
        }
    }

    public class ObtenerHorarioCierreAnteriorTests : HorariosTestBase
    {
        public ObtenerHorarioCierreAnteriorTests() { FakeClock.Ahora = Viernes10hs; }

        private static string Anterior(string horario) => FuncionesPersonalizadas_base.ObtenerHorarioCierreAnterior(Crear.Cliente(), "CC", horario);

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void deberia_devolver_vacio_cuando_horario_es_vacio(string horario)
        {
            Anterior(horario).Should().BeEmpty();
            FakeDb.Llamadas.Should().BeEmpty();
        }

        [Fact]
        public void deberia_devolver_el_cierre_siguiente_al_indicado_cuando_es_de_hoy()
        {
            Horarios(H("VI", "12:00-18:00"));

            Anterior("12:00 hs. ").Should().Be("18:00 hs. ");
        }

        [Fact]
        public void deberia_devolver_el_cierre_siguiente_al_indicado_cuando_es_de_otro_dia()
        {
            Horarios(H("LU", "08:00-13:00"));

            Anterior("08:00 hs. LU").Should().Be("13:00 hs. ");
        }

        [Fact]
        public void deberia_devolver_vacio_cuando_el_dia_cae_en_el_mes_siguiente()
        {
            // CARACTERIZACIÓN: posible bug - arma la fecha con hoy.Day + sumaDia: el sábado 30/03 buscando el lunes
            // intenta crear el 32/03, lanza ArgumentOutOfRangeException, se traga sin log y devuelve "".
            FakeClock.Ahora = new DateTime(2024, 3, 30, 10, 0, 0);
            Horarios(H("LU", "08:00-13:00"));

            Anterior("08:00 hs. LU").Should().BeEmpty();
            FakeDb.ErroresLogueados.Should().BeEmpty();
        }

        [Fact]
        public void deberia_devolver_vacio_sin_log_cuando_horario_es_invalido()
        {
            Anterior("xx:yy hs. ").Should().BeEmpty();
            FakeDb.ErroresLogueados.Should().BeEmpty();
        }
    }
}
