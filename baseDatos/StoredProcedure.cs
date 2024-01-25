using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DKbase.baseDatos
{
    public class StoredProcedure
    {
        public static bool spError(string err_Nombre, string err_Parameters, string err_Data, string err_HelpLink, string err_InnerException, string err_Message, string err_Source, string err_StackTrace, DateTime err_fecha, string err_tipo)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("err_Nombre", err_Nombre));
            l.Add(db.GetParameter("err_Parameters", err_Parameters));
            l.Add(db.GetParameter("err_Data", err_Data));
            l.Add(db.GetParameter("err_HelpLink", err_HelpLink));
            l.Add(db.GetParameter("err_InnerException", err_InnerException));
            l.Add(db.GetParameter("err_Message", err_Message));
            l.Add(db.GetParameter("err_Source", err_Source));
            l.Add(db.GetParameter("err_StackTrace", err_StackTrace));
            l.Add(db.GetParameter("err_fecha", err_fecha));
            l.Add(db.GetParameter("err_tipo", err_tipo));
            try
            {
                int result = db.ExecuteNonQuery_forError("LogRegistro.spError", l);
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                DKbase.generales.Log.LogErrorFile(err_Nombre, "err_Parameters:" + err_Parameters + " - err_Message: " + err_Message);
                DKbase.generales.Log.LogErrorFile(System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.ToString());
                return false;
            }
        }
        public static bool spLogInfo(string pNombreOrigen, string  pMensaje, string pInfoAdicional, string   pParameters, DateTime pFecha, string pTipo)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("log_Nombre", pNombreOrigen));
            l.Add(db.GetParameter("log_Mensaje", pMensaje));
            l.Add(db.GetParameter("log_InfoAdicional", pInfoAdicional));
            l.Add(db.GetParameter("log_Parameters", pParameters));
            l.Add(db.GetParameter("log_fecha", pFecha));
            l.Add(db.GetParameter("log_tipo", pTipo));
            try
            {
                int result = db.ExecuteNonQuery_forError("LogRegistro.spLogInfo", l);
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                DKbase.generales.Log.LogErrorFile(pNombreOrigen, "Parameters:" + pParameters + " - Message: " + pMensaje + " - InfoAdicional: " + pInfoAdicional );
                DKbase.generales.Log.LogErrorFile(System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.ToString());
                return false;
            }
        }
        public static DataTable RecuperarTodasSucursales()
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            return db.GetDataTable("Clientes.spRecuperarTodasSucursal", l);
        }
        public static DataTable Login(string pNombreLogin, string pPassword, string pIp, string pHostName, string pUserAgent)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("login", pNombreLogin));
            l.Add(db.GetParameter("Password", pPassword));
            l.Add(db.GetParameter("Ip", pIp));
            l.Add(db.GetParameter("Host", pHostName));
            l.Add(db.GetParameter("UserName", pUserAgent));
            return db.GetDataTable("Seguridad.spInicioSession", l);
        }
    }
}
