using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cCatalogo
    {
        public int tbc_codigo { get; set; }
        public string tbc_titulo { get; set; }
        public string tbc_descripcion { get; set; }
        public int tbc_orden { get; set; }
        public DateTime? tbc_fecha { get; set; }
        public string tbc_fechaToString { get; set; }
        public int tbc_estado { get; set; }
        public bool? tbc_publicarHome { get; set; }
        public string tbc_publicarHomeToString { get; set; }
        public string tbc_estadoToString { get; set; }
        public int arc_rating { get; set; }
    }
    public class capaCatalogo_base
    {
        public static cCatalogo ConvertToCatalogo(DataRow pItem)
        {
            cCatalogo obj = new cCatalogo();

            if (pItem["tbc_codigo"] != DBNull.Value)
            {
                obj.tbc_codigo = Convert.ToInt32(pItem["tbc_codigo"]);
            }
            if (pItem["tbc_titulo"] != DBNull.Value)
            {
                obj.tbc_titulo = Convert.ToString(pItem["tbc_titulo"]);
            }
            if (pItem["tbc_descripcion"] != DBNull.Value)
            {
                obj.tbc_descripcion = Convert.ToString(pItem["tbc_descripcion"]);
            }
            if (pItem["tbc_orden"] != DBNull.Value)
            {
                obj.tbc_orden = Convert.ToInt32(pItem["tbc_orden"]);
            }
            if (pItem["tbc_fecha"] != DBNull.Value)
            {
                obj.tbc_fecha = Convert.ToDateTime(pItem["tbc_fecha"]);
                obj.tbc_fechaToString = ((DateTime)obj.tbc_fecha).ToString();
            }
            if (pItem["tbc_estado"] != DBNull.Value)
            {
                obj.tbc_estado = Convert.ToInt32(pItem["tbc_estado"]);
            }
            if (pItem["est_nombre"] != DBNull.Value)
            {
                obj.tbc_estadoToString = Convert.ToString(pItem["est_nombre"]);
            }
            //obj.tbc_publicarHomeToString = "No publicar";
            if (pItem.Table.Columns.Contains("tbc_publicarHome"))
            {
                if (pItem["tbc_publicarHome"] != DBNull.Value)
                {
                    obj.tbc_publicarHome = Convert.ToBoolean(pItem["tbc_publicarHome"]);
                    if (obj.tbc_publicarHome.Value)
                        obj.tbc_publicarHomeToString = "Publicar Home";
                }
            }
            if (pItem.Table.Columns.Contains("arc_rating") && pItem["arc_rating"] != DBNull.Value)
            {
                obj.arc_rating = Convert.ToInt32(pItem["arc_rating"]);
            }
            return obj;
        }
        public static DataTable RecuperarTodoCatalogo_PublicarHome()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Catalogo.spRecuperarTodoCatalogo_PublicarHome", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            try
            {
                Conn.Open();
                DataTable dt = new DataTable();
                SqlDataReader LectorSQLdata = cmdComandoInicio.ExecuteReader();
                dt.Load(LectorSQLdata);
                return dt;

            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
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
        public static DataTable RecuperarTodosCatalogos()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Catalogo.spRecuperarTodosCatalogos", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            try
            {
                Conn.Open();
                DataTable dt = new DataTable();
                SqlDataReader LectorSQLdata = cmdComandoInicio.ExecuteReader();
                dt.Load(LectorSQLdata);
                return dt;

            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
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
