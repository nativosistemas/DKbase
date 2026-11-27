using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DKbase.Tests.TestInfra
{
    /// <summary>
    /// Base de todos los tests: instala los interceptores, deja todos los fakes en cero,
    /// fija el reloj, la cultura y la configuración global (Helper), y usa una carpeta temporal propia.
    /// Así cada test es independiente del orden de ejecución.
    /// </summary>
    public abstract class TestBase : IDisposable
    {
        public const string ConnFake = "Server=fake-sql;Database=fake;User Id=u;Password=p;";
        protected string Carpeta { get; }
        private readonly CultureInfo _cultura = CultureInfo.CurrentCulture;
        private readonly CultureInfo _culturaUi = CultureInfo.CurrentUICulture;
        private readonly TextWriter _consola = Console.Out;

        protected TestBase()
        {
            Interceptores.Instalar();
            Console.SetOut(TextWriter.Null); // el código de producción escribe excepciones en consola
            FakeDb.Reset();
            FakeClock.Reset();
            FakeSmtp.Reset();
            FakeWeb.Reset();
            FakeSleep.Reset();

            Carpeta = Path.Combine(Path.GetTempPath(), "dkbase-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Carpeta);

            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            ResetHelper();
            Helper.getConnectionStringSQL = ConnFake;
            Helper.getConnectionStringSQL_Desarrollo = ConnFake;
            Helper.getConnectionStringSQL_intranet = ConnFake;
            Helper.getFolder = Carpeta;
            Helper.getTipoApp = "test";
        }

        private static void ResetHelper()
        {
            foreach (var p in typeof(Helper).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (!p.CanWrite) continue;
                p.SetValue(null, p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null);
            }
        }

        protected static void UsarCultura(string nombre)
        {
            CultureInfo.CurrentCulture = new CultureInfo(nombre);
        }

        public virtual void Dispose()
        {
            Console.SetOut(_consola);
            CultureInfo.CurrentCulture = _cultura;
            CultureInfo.CurrentUICulture = _culturaUi;
            try { Directory.Delete(Carpeta, true); } catch { /* best effort */ }
        }
    }
}
