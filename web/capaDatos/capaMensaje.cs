using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cMensaje : cMensajeNew
    {
        public int tme_codigo { get; set; }
        public string tme_asunto { get; set; }
        public string tme_mensaje { get; set; }
        public int tme_codClienteDestinatario { get; set; }
        public string cli_nombre { get; set; }
        public int tme_codUsuario { get; set; }
        public DateTime tme_fecha { get; set; }
        public string tme_fechaToString { get; set; }
        public int tme_estado { get; set; }
        public int? tme_todos { get; set; }
        public string est_nombre { get; set; }
        public DateTime? tme_fechaDesde { get; set; }
        public string tme_fechaDesdeToString { get; set; }
        public DateTime? tme_fechaHasta { get; set; }
        public string tme_fechaHastaToString { get; set; }
        public bool tme_importante { get; set; }
        public string tme_importanteToString { get; set; }
        public int? tme_todosSucursales { get; set; }

    }
    public class cMensajeNew
    {
        public int tmn_codigo { get; set; }
        public string tmn_asunto { get; set; }
        public string tmn_mensaje { get; set; }
        public DateTime tmn_fecha { get; set; }
        public string tmn_fechaToString { get; set; }
        public DateTime? tmn_fechaDesde { get; set; }
        public string tmn_fechaDesdeToString { get; set; }
        public DateTime? tmn_fechaHasta { get; set; }
        public string tmn_fechaHastaToString { get; set; }
        public bool tmn_importante { get; set; }
        public string tmn_importanteToString { get; set; }
        public string tmn_todosSucursales { get; set; }
        public string tmn_todosRepartos { get; set; }
        public string tmn_tipo { get; set; }
    }
    public class capaMensaje_base
    {
        public static DataTable RecuperartTodosMensajesPorIdCliente(int pIdCliente)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spRecuperartTodosMensajesPorIdCliente", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@idClienteDestinatario", SqlDbType.Int);
            paIdCliente.Value = pIdCliente;

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
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIdCliente);
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
        public static DataTable RecuperartTodosMensajeNewPorSucursal(string pSucursal, string pReparto)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spRecuperartTodosMensajeNewPorSucursal", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paCodSucursal = cmdComandoInicio.Parameters.Add("@codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paCodReparto = cmdComandoInicio.Parameters.Add("@codReparto", SqlDbType.NVarChar, 2);
            paCodSucursal.Value = pSucursal;
            if (string.IsNullOrEmpty(pReparto))
            {
                paCodReparto.Value = DBNull.Value;
            }
            else
            {
                paCodReparto.Value = pReparto;
            }

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
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pSucursal, pReparto);
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
        public static cMensaje ConvertToMensaje(DataRow pItem)
        {
            cMensaje obj = new cMensaje();
            if (pItem["tme_codigo"] != DBNull.Value)
            {
                obj.tme_codigo = Convert.ToInt32(pItem["tme_codigo"]);
            }
            if (pItem["tme_fecha"] != DBNull.Value)
            {
                obj.tme_fecha = Convert.ToDateTime(pItem["tme_fecha"]);
                obj.tme_fechaToString = Convert.ToDateTime(pItem["tme_fecha"]).ToShortDateString();
            }
            if (pItem["tme_asunto"] != DBNull.Value)
            {
                obj.tme_asunto = pItem["tme_asunto"].ToString();
            }
            if (pItem["tme_mensaje"] != DBNull.Value)
            {
                obj.tme_mensaje = pItem["tme_mensaje"].ToString();
            }
            if (pItem["tme_codUsuario"] != DBNull.Value)
            {
                obj.tme_codUsuario = Convert.ToInt32(pItem["tme_codUsuario"]);
            }
            if (pItem["tme_estado"] != DBNull.Value)
            {
                obj.tme_estado = Convert.ToInt32(pItem["tme_estado"]);
            }
            if (pItem["tme_codClienteDestinatario"] != DBNull.Value)
            {
                obj.tme_codClienteDestinatario = Convert.ToInt32(pItem["tme_codClienteDestinatario"]);
            }
            if (pItem.Table.Columns.Contains("est_nombre"))
            {
                if (pItem["est_nombre"] != DBNull.Value)
                {
                    obj.est_nombre = Convert.ToString(pItem["est_nombre"]);
                }
            }

            if (pItem.Table.Columns.Contains("tme_importante"))
            {
                if (pItem["tme_importante"] != DBNull.Value)
                {
                    obj.tme_importante = Convert.ToBoolean(pItem["tme_importante"]);
                }
            }
            obj.tme_fechaDesde = DateTime.MinValue;
            if (pItem.Table.Columns.Contains("tme_fechaDesde"))
            {
                if (pItem["tme_fechaDesde"] != DBNull.Value)
                {
                    obj.tme_fechaDesde = Convert.ToDateTime(pItem["tme_fechaDesde"]);
                }
            }
            obj.tme_fechaHasta = DateTime.MinValue;
            if (pItem.Table.Columns.Contains("tme_fechaHasta"))
            {
                if (pItem["tme_fechaHasta"] != DBNull.Value)
                {
                    obj.tme_fechaHasta = Convert.ToDateTime(pItem["tme_fechaHasta"]);
                }
            }
            if (obj.tme_importante)
            {
                obj.tme_fechaDesdeToString = ((DateTime)obj.tme_fechaDesde).ToShortDateString();
                obj.tme_fechaHastaToString = ((DateTime)obj.tme_fechaHasta).ToShortDateString();
                obj.tme_importanteToString = "Si";
            }
            else
            {
                obj.tme_importanteToString = "No";
            }
            obj.tme_todos = null;
            if (pItem.Table.Columns.Contains("tme_todos"))
            {
                if (pItem["tme_todos"] != DBNull.Value)
                {
                    obj.tme_todos = Convert.ToInt32(pItem["tme_todos"]);
                }
            }
            if (pItem.Table.Columns.Contains("tme_todosSucursales"))
            {
                if (pItem["tme_todosSucursales"] != DBNull.Value)
                {
                    obj.tme_todosSucursales = Convert.ToInt32(pItem["tme_todosSucursales"]);
                }
            }
            return obj;
        }
        public static cMensajeNew ConvertToMensajeNew(DataRow pItem)
        {
            cMensajeNew obj = new cMensajeNew();
            if (pItem["tmn_codigo"] != DBNull.Value)
            {
                obj.tmn_codigo = Convert.ToInt32(pItem["tmn_codigo"]);
            }
            if (pItem["tmn_fecha"] != DBNull.Value)
            {
                obj.tmn_fecha = Convert.ToDateTime(pItem["tmn_fecha"]);
                obj.tmn_fechaToString = Convert.ToDateTime(pItem["tmn_fecha"]).ToShortDateString();
            }
            if (pItem["tmn_asunto"] != DBNull.Value)
            {
                obj.tmn_asunto = pItem["tmn_asunto"].ToString();
            }
            if (pItem["tmn_mensaje"] != DBNull.Value)
            {
                obj.tmn_mensaje = pItem["tmn_mensaje"].ToString();
            }
            if (pItem.Table.Columns.Contains("tmn_importante") && pItem["tmn_importante"] != DBNull.Value)
            {
                obj.tmn_importante = Convert.ToBoolean(pItem["tmn_importante"]);
            }
            obj.tmn_fechaDesde = DateTime.MinValue;
            if (pItem.Table.Columns.Contains("tmn_fechaDesde") && pItem["tmn_fechaDesde"] != DBNull.Value)
            {
                obj.tmn_fechaDesde = Convert.ToDateTime(pItem["tmn_fechaDesde"]);
            }
            obj.tmn_fechaHasta = DateTime.MinValue;
            if (pItem.Table.Columns.Contains("tmn_fechaHasta") && pItem["tmn_fechaHasta"] != DBNull.Value)
            {
                obj.tmn_fechaHasta = Convert.ToDateTime(pItem["tmn_fechaHasta"]);
            }
            if (obj.tmn_importante)
            {
                obj.tmn_fechaDesdeToString = ((DateTime)obj.tmn_fechaDesde).ToShortDateString();
                obj.tmn_fechaHastaToString = ((DateTime)obj.tmn_fechaHasta).ToShortDateString();
                obj.tmn_importanteToString = "Si";
            }
            else
            {
                obj.tmn_importanteToString = "No";
            }

            if (pItem.Table.Columns.Contains("tmn_todosSucursales") && pItem["tmn_todosSucursales"] != DBNull.Value)
            {
                obj.tmn_todosSucursales = pItem["tmn_todosSucursales"].ToString();
            }
            if (pItem.Table.Columns.Contains("tmn_tipo") && pItem["tmn_tipo"] != DBNull.Value)
            {
                obj.tmn_tipo = pItem["tmn_tipo"].ToString();
            }
            if (pItem.Table.Columns.Contains("tmn_todosRepartos") && pItem["tmn_todosRepartos"] != DBNull.Value)
            {
                obj.tmn_todosRepartos = pItem["tmn_todosRepartos"].ToString();
            }
            return obj;
        }
    }
}
