using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    }
    public class cNombreArchivos
    {
        public string NombreOriginal { get; set; }
        public string NombreGrabado { get; set; }
    }
    public class capaRecurso
    {
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
    }
}
