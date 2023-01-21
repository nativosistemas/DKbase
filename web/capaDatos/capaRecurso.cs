using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cArchivo
    {
        public int arc_codRecurso { get; set; }
        public int arc_codRelacion { get; set; }
        public string arc_galeria { get; set; }
        public int arc_orden { get; set; }
        public string arc_tipo { get; set; }
        public string arc_mime { get; set; }
        public string arc_nombre { get; set; }
        public string arc_titulo { get; set; }
        public string arc_descripcion { get; set; }
        public string arc_hash { get; set; }
        public DateTime arc_fecha { get; set; }
        public string arc_fechaToString { get; set; }
        public int arc_accion { get; set; }
        public int arc_estado { get; set; }
        public string arc_estadoToString { get; set; }
        public DateTime arc_fechaUltMov { get; set; }
        public int arc_codUsuarioUltMov { get; set; }
        public string NombreYapellido { get; set; }
        public int ancho { get; set; }
        public int alto { get; set; }
        public int arc_rating { get; set; }

    }
    public class cNombreArchivos
    {
        public string NombreOriginal { get; set; }
        public string NombreGrabado { get; set; }
    }
    public class capaRecurso_base
    {
        public static string obtenerExtencion(string pNombreArchivo)
        {
            string resultado = string.Empty;
            if (!string.IsNullOrEmpty(pNombreArchivo))
            {
                string[] listaNombre = pNombreArchivo.Split('.');
                if (listaNombre.Length > 0)
                {
                    resultado = listaNombre[listaNombre.Length - 1].Trim().ToLower();
                }
            }
            return resultado;
        }
        public static string nombreArchivoSinRepetir(string pPath, string pNombreArchivo)
        {
            string resultado = string.Empty;
            string[] listaNombre = pNombreArchivo.Split('.');
            string NombreArchivo = string.Empty;
            string ExtencionArchivo = string.Empty;
            for (int i = 0; i < listaNombre.Length - 1; i++)
            {
                NombreArchivo += listaNombre[i];
            }
            NombreArchivo = remplazarCaracteresEspeciales(NombreArchivo);
            ExtencionArchivo = listaNombre[listaNombre.Length - 1];
            int contNombre = -1;
            string NombreTemporal = NombreArchivo + "." + ExtencionArchivo;
            string Path_NombreTemporal = Path.Combine(pPath, NombreTemporal);
            while (System.IO.File.Exists(Path_NombreTemporal))
            {
                contNombre++;
                NombreTemporal = NombreArchivo + "_" + contNombre.ToString() + "." + ExtencionArchivo;
                Path_NombreTemporal = Path.Combine(pPath, NombreTemporal);
            }
            if (contNombre == -1)
            {
                resultado = NombreArchivo + "." + ExtencionArchivo;
            }
            else
            {
                resultado = NombreTemporal;
            }
            return resultado;
        }
        public static string remplazarCaracteresEspeciales(string pStr)
        {
            const string pStrOriginal = "áéíóúàèìòùâêîôûäëïöüãõñÁÉÍÓÚÀÈÌÒÙÂÊÎÔÛÄËÏÖÜÑçÇ";
            const string pStrARemplazar = "aeiouaeiouaeiouaeiouaonaeiouaeiouaeiouaeiouncc";
            if (pStr != null)
            {
                string resultado = string.Empty;
                for (int i = 0; i < pStrOriginal.Length; i++)
                {
                    pStr.Replace(pStrOriginal[i], pStrARemplazar[i]);
                }

                for (int i = 0; i < pStr.Length; i++)
                {
                    char[] CharEspeciales = new char[] { '\r', '\n', '\t' }; ;
                    if (pStr[i] == CharEspeciales[0] || pStr[i] == CharEspeciales[1] || pStr[i] == CharEspeciales[2])
                    {
                    }
                    else
                    {
                        resultado += pStr[i];
                    }

                }
                return resultado;
            }
            else
            {
                return null;
            }
        }
        public static DataSet GestiónArchivo(int? arc_codRecurso, int? arc_codRelacion, string arc_galeria, string arc_tipo, string arc_mime, string arc_nombre, string arc_titulo, string arc_descripcion, string arc_hash, int? arc_codUsuarioUltMov, int? arc_estado, int? arc_accion, string filtro, string accion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Recursos.spGestionArchivo", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paArc_codRecurso = cmdComandoInicio.Parameters.Add("@arc_codRecurso", SqlDbType.Int);
            SqlParameter paArc_codRelacion = cmdComandoInicio.Parameters.Add("@arc_codRelacion", SqlDbType.Int);
            SqlParameter paArc_galeria = cmdComandoInicio.Parameters.Add("@arc_galeria", SqlDbType.NVarChar, 50);
            SqlParameter paArc_tipo = cmdComandoInicio.Parameters.Add("@arc_tipo", SqlDbType.NVarChar, 50);
            SqlParameter paArc_mime = cmdComandoInicio.Parameters.Add("@arc_mime", SqlDbType.NVarChar, 100);
            SqlParameter paArc_nombre = cmdComandoInicio.Parameters.Add("@arc_nombre", SqlDbType.NVarChar, 150);
            SqlParameter paArc_titulo = cmdComandoInicio.Parameters.Add("@arc_titulo", SqlDbType.NVarChar, 200);
            SqlParameter paArc_descripcion = cmdComandoInicio.Parameters.Add("@arc_descripcion", SqlDbType.NVarChar, -1);
            SqlParameter paArc_hash = cmdComandoInicio.Parameters.Add("@arc_hash", SqlDbType.NVarChar, 50);
            SqlParameter paArc_codUsuarioUltMov = cmdComandoInicio.Parameters.Add("@arc_codUsuarioUltMov", SqlDbType.Int);
            SqlParameter paArc_estado = cmdComandoInicio.Parameters.Add("@arc_estado", SqlDbType.Int);
            SqlParameter paArc_accion = cmdComandoInicio.Parameters.Add("@arc_accion", SqlDbType.Int);
            SqlParameter paFiltro = cmdComandoInicio.Parameters.Add("@filtro", SqlDbType.NVarChar, 50);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 50);

            if (arc_codRecurso == null)
            {
                paArc_codRecurso.Value = DBNull.Value;
            }
            else
            {
                paArc_codRecurso.Value = arc_codRecurso;
            }
            if (arc_codRelacion == null)
            {
                paArc_codRelacion.Value = DBNull.Value;
            }
            else
            {
                paArc_codRelacion.Value = arc_codRelacion;
            }
            if (arc_galeria == null)
            {
                paArc_galeria.Value = DBNull.Value;
            }
            else
            {
                paArc_galeria.Value = arc_galeria;
            }
            if (arc_tipo == null)
            {
                paArc_tipo.Value = DBNull.Value;
            }
            else
            {
                paArc_tipo.Value = arc_tipo;
            }
            if (arc_mime == null)
            {
                paArc_mime.Value = DBNull.Value;
            }
            else
            {
                paArc_mime.Value = arc_mime;
            }
            if (arc_nombre == null)
            {
                paArc_nombre.Value = DBNull.Value;
            }
            else
            {
                paArc_nombre.Value = arc_nombre;
            }
            if (arc_titulo == null)
            {
                paArc_titulo.Value = DBNull.Value;
            }
            else
            {
                paArc_titulo.Value = arc_titulo;
            }

            if (arc_descripcion == null)
            {
                paArc_descripcion.Value = DBNull.Value;
            }
            else
            {
                paArc_descripcion.Value = arc_descripcion;
            }
            if (arc_hash == null)
            {
                paArc_hash.Value = DBNull.Value;
            }
            else
            {
                paArc_hash.Value = arc_hash;
            }
            if (arc_codUsuarioUltMov == null)
            {
                paArc_codUsuarioUltMov.Value = DBNull.Value;
            }
            else
            {
                paArc_codUsuarioUltMov.Value = arc_codUsuarioUltMov;
            }
            if (arc_estado == null)
            {
                paArc_estado.Value = DBNull.Value;
            }
            else
            {
                paArc_estado.Value = arc_estado;
            }
            if (arc_accion == null)
            {
                paArc_accion.Value = DBNull.Value;
            }
            else
            {
                paArc_accion.Value = arc_accion;
            }
            if (filtro == null)
            {
                paFiltro.Value = DBNull.Value;
            }
            else
            {
                paFiltro.Value = filtro;
            }
            paAccion.Value = accion;

            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "Archivo");
                return dsResultado;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static int spRatingArchivos(string arc_galeria, string arc_nombre)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Recursos.spRatingArchivos", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paArc_galeria = cmdComandoInicio.Parameters.Add("@arc_galeria", SqlDbType.NVarChar, 50);
            SqlParameter paArc_nombre = cmdComandoInicio.Parameters.Add("@arc_nombre", SqlDbType.NVarChar, 150);
            SqlParameter paArc_ratingOUTPUT = cmdComandoInicio.Parameters.Add("@arc_ratingOUTPUT", SqlDbType.Int);
            paArc_ratingOUTPUT.Direction = ParameterDirection.Output;

            if (arc_galeria == null)
            {
                paArc_galeria.Value = DBNull.Value;
            }
            else
            {
                paArc_galeria.Value = arc_galeria;
            }
            if (arc_nombre == null)
            {
                paArc_nombre.Value = DBNull.Value;
            }
            else
            {
                paArc_nombre.Value = arc_nombre;
            }

            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                return Convert.ToInt32(paArc_ratingOUTPUT.Value);

            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), "Error al sumar en rating", DateTime.Now, arc_galeria, arc_nombre);
                return 0;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
    }
}
