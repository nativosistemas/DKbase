using DKbase.generales;
using System;
using System.Collections.Generic;
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
                return false;
            }
        }
    }
}
