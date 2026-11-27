using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DKbase.web.capaDatos;

namespace DKbase.Tests.TestInfra
{
    /// <summary>Factories de datos de prueba con valores por defecto realistas y personalización por lambda.</summary>
    public static class Crear
    {
        public static cClientes Cliente(Action<cClientes> personalizar = null)
        {
            var c = new cClientes
            {
                cli_codigo = 1234,
                cli_nombre = "FARMACIA DE PRUEBA",
                cli_login = "farmacia1234",
                cli_codsuc = "CC",
                cli_codrep = "R1",
                cli_codprov = "Santa Fe",
                cli_tipo = "T",
                cli_pordesbetmed = 10m,
                cli_pordesnetos = 5m,
                cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto = 20m,
                cli_tomaOfertas = true,
                cli_tomaPerfumeria = true,
                cli_tomaTransfers = true,
                cli_email = "farmacia@example.com",
            };
            personalizar?.Invoke(c);
            return c;
        }

        public static cProductos Producto(Action<cProductos> personalizar = null)
        {
            var p = new cProductos("P001", "IBUPROFENO 400 X 10")
            {
                pro_PrecioBase = 1000m,
                pro_precio = 1200m,
                pro_neto = false,
                pro_codtpopro = "M",
            };
            personalizar?.Invoke(p);
            return p;
        }

        public static cProductosGenerico ProductoGenerico(Action<cProductosGenerico> personalizar = null)
        {
            var p = new cProductosGenerico
            {
                pro_codigo = "P001",
                pro_nombre = "IBUPROFENO 400 X 10",
                PrecioFinal = 1000m,
                PrecioFinalTransfer = 800m,
                pro_codtpopro = "M",
            };
            personalizar?.Invoke(p);
            return p;
        }

        public static DKbase.web.Usuario Usuario(Action<DKbase.web.Usuario> personalizar = null)
        {
            var u = new DKbase.web.Usuario { id = 77, idRol = 3, usu_codCliente = 1234, NombreYApellido = "Juan Perez", ApNombre = "Perez Juan", idUsuarioLog = 999 };
            personalizar?.Invoke(u);
            return u;
        }
    }

    /// <summary>Construye DataTables en memoria a partir de objetos anónimos (una fila por objeto). null → DBNull.</summary>
    public static class Tabla
    {
        public static DataTable De(params object[] filas) => DeNombre("Tabla", filas);

        public static DataTable DeNombre(string nombre, params object[] filas)
        {
            var dt = new DataTable(nombre);
            if (filas.Length == 0) return dt;
            var props = filas[0].GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                var tipo = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (tipo == typeof(object))
                {
                    var muestra = filas.Select(f => p.GetValue(f)).FirstOrDefault(v => v != null && v != DBNull.Value);
                    tipo = muestra?.GetType() ?? typeof(string);
                }
                dt.Columns.Add(p.Name, tipo);
            }
            foreach (var f in filas)
            {
                var row = dt.NewRow();
                foreach (var p in props)
                    row[p.Name] = p.GetValue(f) ?? DBNull.Value;
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>Tabla vacía con las columnas indicadas (tipo string salvo que se indique).</summary>
        public static DataTable Columnas(params string[] columnas)
        {
            var dt = new DataTable("Tabla");
            foreach (var c in columnas) dt.Columns.Add(c, typeof(string));
            return dt;
        }

        public static DataRow Fila(object fila) => De(fila).Rows[0];
    }
}
