using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DKbase.generales;
using DKbase.Tests.TestInfra;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace DKbase.Tests.generales
{
    /// <summary>
    /// Las constantes viajan como parámetros a los stored procedures y como códigos a DKdll:
    /// cambiar una por error en la refactorización rompe el contrato con la base. Se congelan todas con un snapshot.
    /// </summary>
    public class ConstantesYHelperTests : TestBase
    {
        [Fact]
        public Task deberia_mantener_el_valor_de_todas_las_constantes()
        {
            var valores = typeof(Constantes).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .OrderBy(p => p.Name, StringComparer.Ordinal)
                .ToDictionary(p => p.Name, p => p.PropertyType.Name + " " + Convert.ToString(p.GetValue(null), System.Globalization.CultureInfo.InvariantCulture));
            var campos = typeof(Constantes).GetFields(BindingFlags.Public | BindingFlags.Static)
                .OrderBy(f => f.Name, StringComparer.Ordinal)
                .ToDictionary(f => f.Name, f => f.FieldType.Name + " " + Convert.ToString(f.GetValue(null), System.Globalization.CultureInfo.InvariantCulture));

            return Verifier.Verify(new { Propiedades = valores, Campos = campos });
        }

        public static IEnumerable<object[]> PropiedadesHelper() => typeof(Helper)
            .GetProperties(BindingFlags.Public | BindingFlags.Static).Where(p => p.CanWrite)
            .OrderBy(p => p.Name).Select(p => new object[] { p.Name });

        [Theory]
        [MemberData(nameof(PropiedadesHelper))]
        public void deberia_devolver_lo_que_se_asigna_cuando_se_configura_helper(string propiedad)
        {
            var p = typeof(Helper).GetProperty(propiedad);
            object valor = p.PropertyType == typeof(int) ? 2525 : p.PropertyType == typeof(bool) ? true : (object)("valor_" + propiedad);

            p.SetValue(null, valor);

            p.GetValue(null).Should().Be(valor);
        }

        [Fact]
        public void deberia_arrancar_sin_SAP_y_sin_modo_desarrollo()
        {
            // TestBase restaura los valores por defecto de Helper antes de cada test.
            Helper.isSAP.Should().BeFalse();
            Helper.isModoDev.Should().BeFalse();
            Helper.getSMTP_PORT.Should().Be(0);
            Helper.getUrl_DKcore.Should().BeNull();
        }
    }
}
