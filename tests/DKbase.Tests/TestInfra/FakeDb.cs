using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DKbase.Tests.TestInfra
{
    /// <summary>Una llamada a la base registrada por el fake (el "efecto observable" hacia SQL Server).</summary>
    public sealed class DbLlamada
    {
        public string Tipo { get; init; }            // NonQuery | Scalar | Reader | Fill | Open
        public string Comando { get; init; }         // nombre del SP o texto SQL
        public CommandType CommandType { get; init; }
        public IReadOnlyDictionary<string, object> Parametros { get; init; }
        public object Param(string nombre)
        {
            var clave = Parametros.Keys.FirstOrDefault(k => k.TrimStart('@').Equals(nombre.TrimStart('@'), StringComparison.OrdinalIgnoreCase));
            if (clave == null) throw new KeyNotFoundException($"El parámetro '{nombre}' no se envió a {Comando}. Enviados: {string.Join(", ", Parametros.Keys)}");
            return Parametros[clave];
        }
        public override string ToString() => $"{Tipo} {Comando}({string.Join(", ", Parametros.Select(p => p.Key + "=" + (p.Value ?? "NULL")))})";
    }

    /// <summary>
    /// Respuesta configurada para un comando. Las tablas se cargan en el DataSet/DataTable
    /// tal como lo haría ADO.NET; <see cref="AlEjecutar"/> permite escribir parámetros de salida.
    /// </summary>
    public sealed class DbRespuesta
    {
        public List<DataTable> Tablas { get; } = new List<DataTable>();
        public int FilasAfectadas { get; set; }
        public object Escalar { get; set; }
        public Exception Error { get; set; }
        public Action<SqlCommand> AlEjecutar { get; set; }
    }

    /// <summary>
    /// SQL Server falso. Se activa con los interceptores de Harmony sobre SqlConnection/SqlCommand/DbDataAdapter/DataTable.Load.
    /// Por defecto cualquier comando no configurado devuelve 0 filas afectadas, escalar null y tablas vacías.
    /// </summary>
    public static class FakeDb
    {
        private static readonly Dictionary<string, Queue<DbRespuesta>> _respuestas = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, DbRespuesta> _respuestasFijas = new(StringComparer.OrdinalIgnoreCase);
        public static List<DbLlamada> Llamadas { get; } = new List<DbLlamada>();
        /// <summary>Si no es null, toda apertura de conexión falla con esta excepción (base caída).</summary>
        public static Exception ErrorAlConectar { get; set; }
        internal static DataTable LecturaPendiente { get; set; }

        public static void Reset()
        {
            _respuestas.Clear();
            _respuestasFijas.Clear();
            Llamadas.Clear();
            ErrorAlConectar = null;
            LecturaPendiente = null;
        }

        /// <summary>Respuesta para todas las ejecuciones de <paramref name="comando"/>.</summary>
        public static DbRespuesta Cuando(string comando)
        {
            var r = new DbRespuesta();
            _respuestasFijas[comando] = r;
            return r;
        }

        /// <summary>Respuesta para la próxima ejecución (se encolan en orden).</summary>
        public static DbRespuesta CuandoUnaVez(string comando)
        {
            var r = new DbRespuesta();
            if (!_respuestas.TryGetValue(comando, out var q)) _respuestas[comando] = q = new Queue<DbRespuesta>();
            q.Enqueue(r);
            return r;
        }

        public static DbRespuesta Devolver(this DbRespuesta r, params DataTable[] tablas) { r.Tablas.AddRange(tablas); return r; }
        public static DbRespuesta Afectar(this DbRespuesta r, int filas) { r.FilasAfectadas = filas; return r; }
        public static DbRespuesta Escalar(this DbRespuesta r, object valor) { r.Escalar = valor; return r; }
        public static DbRespuesta Fallar(this DbRespuesta r, Exception ex = null) { r.Error = ex ?? new InvalidOperationException("Fallo simulado de la base"); return r; }
        public static DbRespuesta Salida(this DbRespuesta r, string parametro, object valor)
        {
            var anterior = r.AlEjecutar;
            r.AlEjecutar = cmd => { anterior?.Invoke(cmd); cmd.Parameters[parametro].Value = valor; };
            return r;
        }

        public static IEnumerable<DbLlamada> De(string comando) => Llamadas.Where(l => l.Tipo != "Open" && string.Equals(l.Comando, comando, StringComparison.OrdinalIgnoreCase));
        public static DbLlamada Unica(string comando)
        {
            var l = De(comando).ToList();
            if (l.Count != 1) throw new InvalidOperationException($"Se esperaba 1 llamada a {comando} y hubo {l.Count}. Llamadas: {string.Join(" | ", Llamadas)}");
            return l[0];
        }
        public static IEnumerable<DbLlamada> SinLogs => Llamadas.Where(l => l.Tipo != "Open" && !l.Comando.StartsWith("LogRegistro.spError") && !l.Comando.StartsWith("LogRegistro.spLogInfo"));
        public static IEnumerable<DbLlamada> ErroresLogueados => De("LogRegistro.spError");

        internal static DbRespuesta Registrar(string tipo, SqlCommand cmd)
        {
            var parametros = new Dictionary<string, object>();
            foreach (SqlParameter p in cmd.Parameters)
                parametros[p.ParameterName] = p.Value is DBNull ? null : p.Value;
            Llamadas.Add(new DbLlamada { Tipo = tipo, Comando = cmd.CommandText, CommandType = cmd.CommandType, Parametros = parametros });

            DbRespuesta r = null;
            if (_respuestas.TryGetValue(cmd.CommandText, out var q) && q.Count > 0) r = q.Dequeue();
            else _respuestasFijas.TryGetValue(cmd.CommandText, out r);
            r ??= new DbRespuesta();
            if (r.Error != null) throw r.Error;
            r.AlEjecutar?.Invoke(cmd);
            return r;
        }

        internal static void RegistrarApertura(SqlConnection c)
        {
            Llamadas.Add(new DbLlamada { Tipo = "Open", Comando = "(conexión)", Parametros = new Dictionary<string, object>() });
            if (ErrorAlConectar != null) throw ErrorAlConectar;
        }
    }
}
