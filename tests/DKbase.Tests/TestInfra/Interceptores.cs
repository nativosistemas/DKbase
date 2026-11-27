using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Threading;
using HarmonyLib;

namespace DKbase.Tests.TestInfra
{
    /// <summary>
    /// Instala (una sola vez por proceso) los interceptores de Harmony sobre los límites externos:
    /// SQL Server (ADO.NET), reloj, SMTP, WebClient y Thread.Sleep. No se toca código de producción:
    /// se reemplaza en memoria el comportamiento de las APIs del framework.
    /// </summary>
    public static class Interceptores
    {
        private static readonly object _lock = new object();
        private static bool _instalado;

        public static void Instalar()
        {
            lock (_lock)
            {
                if (_instalado) return;
                var h = new Harmony("dkbase.tests");
                Prefijo(h, AccessTools.Method(typeof(SqlConnection), nameof(SqlConnection.Open), Type.EmptyTypes), nameof(Open));
                Prefijo(h, AccessTools.Method(typeof(SqlCommand), nameof(SqlCommand.ExecuteNonQuery), Type.EmptyTypes), nameof(ExecuteNonQuery));
                Prefijo(h, AccessTools.Method(typeof(SqlCommand), nameof(SqlCommand.ExecuteScalar), Type.EmptyTypes), nameof(ExecuteScalar));
                Prefijo(h, AccessTools.Method(typeof(SqlCommand), nameof(SqlCommand.ExecuteReader), Type.EmptyTypes), nameof(ExecuteReader));
                Prefijo(h, AccessTools.Method(typeof(SqlCommand), nameof(SqlCommand.ExecuteReader), new[] { typeof(CommandBehavior) }), nameof(ExecuteReader));
                Prefijo(h, AccessTools.Method(typeof(DataTable), nameof(DataTable.Load), new[] { typeof(IDataReader) }), nameof(DataTableLoad));
                Prefijo(h, AccessTools.Method(typeof(DbDataAdapter), nameof(DbDataAdapter.Fill), new[] { typeof(DataSet), typeof(string) }), nameof(FillDataSet));
                Prefijo(h, AccessTools.PropertyGetter(typeof(DateTime), nameof(DateTime.Now)), nameof(Now));
                Prefijo(h, AccessTools.PropertyGetter(typeof(DateTime), nameof(DateTime.Today)), nameof(Today));
                Prefijo(h, AccessTools.Method(typeof(SmtpClient), nameof(SmtpClient.Send), new[] { typeof(MailMessage) }), nameof(SmtpSend));
                Prefijo(h, AccessTools.Method(typeof(System.Net.WebClient), nameof(System.Net.WebClient.DownloadString), new[] { typeof(string) }), nameof(DownloadString));
                Prefijo(h, AccessTools.Method(typeof(Thread), nameof(Thread.Sleep), new[] { typeof(int) }), nameof(Sleep));
                _instalado = true;
            }
        }

        private static void Prefijo(Harmony h, System.Reflection.MethodBase original, string prefijo)
        {
            if (original == null) throw new InvalidOperationException("No se encontró el método a interceptar para " + prefijo);
            h.Patch(original, prefix: new HarmonyMethod(typeof(Interceptores), prefijo));
        }

        // ---------- SQL ----------
        private static bool Open(SqlConnection __instance) { FakeDb.RegistrarApertura(__instance); return false; }
        private static bool ExecuteNonQuery(SqlCommand __instance, ref int __result) { __result = FakeDb.Registrar("NonQuery", __instance).FilasAfectadas; return false; }
        private static bool ExecuteScalar(SqlCommand __instance, ref object __result) { __result = FakeDb.Registrar("Scalar", __instance).Escalar; return false; }
        private static bool ExecuteReader(SqlCommand __instance, ref SqlDataReader __result)
        {
            var r = FakeDb.Registrar("Reader", __instance);
            FakeDb.LecturaPendiente = r.Tablas.Count > 0 ? r.Tablas[0] : new DataTable();
            __result = null;
            return false;
        }
        private static bool DataTableLoad(DataTable __instance, IDataReader reader)
        {
            if (reader != null || FakeDb.LecturaPendiente == null) return true;
            var origen = FakeDb.LecturaPendiente;
            FakeDb.LecturaPendiente = null;
            __instance.Merge(origen.Copy());
            return false;
        }
        private static bool FillDataSet(DbDataAdapter __instance, DataSet dataSet, string srcTable, ref int __result)
        {
            var cmd = (SqlCommand)((IDbDataAdapter)__instance).SelectCommand;
            var r = FakeDb.Registrar("Fill", cmd);
            int filas = 0;
            if (r.Tablas.Count == 0)
            {
                if (!dataSet.Tables.Contains(srcTable)) dataSet.Tables.Add(new DataTable(srcTable));
            }
            for (int i = 0; i < r.Tablas.Count; i++)
            {
                var t = r.Tablas[i].Copy();
                t.TableName = i == 0 ? srcTable : srcTable + i;
                dataSet.Tables.Add(t);
                filas += t.Rows.Count;
            }
            __result = filas;
            return false;
        }

        // ---------- Reloj ----------
        private static bool Now(ref DateTime __result) { __result = FakeClock.Ahora; return false; }
        private static bool Today(ref DateTime __result) { __result = FakeClock.Ahora.Date; return false; }

        // ---------- SMTP ----------
        private static bool SmtpSend(SmtpClient __instance, MailMessage message)
        {
            FakeSmtp.Registrar(__instance, message);
            return false;
        }

        // ---------- WebClient ----------
        private static bool DownloadString(string address, ref string __result)
        {
            __result = FakeWeb.Descargar(address);
            return false;
        }

        // ---------- Sleep ----------
        private static bool Sleep(int millisecondsTimeout)
        {
            // Solo se intercepta en el hilo del test: el runtime también llama a Thread.Sleep en hilos propios.
            if (Thread.CurrentThread.ManagedThreadId != FakeSleep.HiloDelTest) return true;
            FakeSleep.Esperas.Add(millisecondsTimeout);
            return false;
        }
    }

    public static class FakeClock
    {
        /// <summary>Viernes 15/03/2024 10:00:00 (hora local) por defecto.</summary>
        public static readonly DateTime PorDefecto = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Local);
        public static DateTime Ahora { get; set; } = PorDefecto;
        public static void Reset() => Ahora = PorDefecto;
    }

    public sealed class MailEnviado
    {
        public string Host { get; init; }
        public int Puerto { get; init; }
        public string De { get; init; }
        public List<string> Para { get; init; }
        public string Asunto { get; init; }
        public string Cuerpo { get; init; }
        public bool EsHtml { get; init; }
    }

    public static class FakeSmtp
    {
        public static List<MailEnviado> Enviados { get; } = new List<MailEnviado>();
        public static Exception Error { get; set; }
        public static void Reset() { Enviados.Clear(); Error = null; }
        internal static void Registrar(SmtpClient c, MailMessage m)
        {
            if (Error != null) throw Error;
            var para = new List<string>();
            foreach (var a in m.To) para.Add(a.Address);
            Enviados.Add(new MailEnviado { Host = c.Host, Puerto = c.Port, De = m.From?.Address, Para = para, Asunto = m.Subject, Cuerpo = m.Body, EsHtml = m.IsBodyHtml });
        }
    }

    public static class FakeWeb
    {
        public static List<string> Urls { get; } = new List<string>();
        public static Func<string, string> Responder { get; set; }
        public static void Reset() { Urls.Clear(); Responder = null; }
        internal static string Descargar(string url)
        {
            Urls.Add(url);
            if (Responder == null) throw new System.Net.WebException("Sin red en tests");
            return Responder(url);
        }
    }

    public static class FakeSleep
    {
        public static List<int> Esperas { get; } = new List<int>();
        internal static int HiloDelTest { get; private set; } = -1;
        public static void Reset() { Esperas.Clear(); HiloDelTest = Thread.CurrentThread.ManagedThreadId; }
    }
}
