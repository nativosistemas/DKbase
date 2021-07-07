using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DKbase.generales;
using System.Reflection;

namespace DKbase.web.capaDatos
{
    public class capaModulo
    {
        //public static int spAddUpdateLaboratorios(int lab_id, string lab_laboratorio)
        //{
        //    BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
        //    List<SqlParameter> l = new List<SqlParameter>();
        //    l.Add(db.GetParameter("lab_id", lab_id));
        //    l.Add(db.GetParameter("lab_laboratorio", lab_laboratorio));
        //    return Convert.ToInt32(db.ExecuteScalar("app.spAddUpdateLaboratorios", l));
        //}
        //public static void spDeleteLaboratorios(int lab_id)
        //{
        //    BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
        //    List<SqlParameter> l = new List<SqlParameter>();
        //    l.Add(db.GetParameter("lab_id", lab_id));
        //    db.ExecuteNonQuery("app.spDeleteLaboratorios", l);
        //}
        public static void spDeleteModulo(int lab_id)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("id", lab_id));
            db.ExecuteNonQuery("app.spDeleteModulo", l);
        }
        public static DataSet spGetLaboratorios(string pConnectionStringSQL = null)
        {
            if (pConnectionStringSQL == null)
            {
                pConnectionStringSQL = Helper.getConnectionStringSQL;
            }
            BaseDataAccess db = new BaseDataAccess(pConnectionStringSQL);
            return db.GetDataSet("app.spGetLaboratorios");
        }
        public static DataSet spGetModulos(string pConnectionStringSQL = null)
        {
            if (pConnectionStringSQL == null)
            {
                pConnectionStringSQL = Helper.getConnectionStringSQL;
            }
            BaseDataAccess db = new BaseDataAccess(pConnectionStringSQL);
            return db.GetDataSet("app.spGetModulos");
        }
        public static Guid spAddPedido(string pPromotor, string pTablaXml, string pConnectionStringSQL = null)
        {
            if (pConnectionStringSQL == null)
            {
                pConnectionStringSQL = Helper.getConnectionStringSQL;
            }
            Guid result = Guid.Empty;
            try
            {
                BaseDataAccess db = new BaseDataAccess(pConnectionStringSQL);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(db.GetParameter("promotor", pPromotor));
                l.Add(db.GetParameter("strXML", pTablaXml, SqlDbType.Xml));
                SqlParameter ParameterOut_GUID = db.GetParameterOut("GUID", SqlDbType.UniqueIdentifier);
                l.Add(ParameterOut_GUID);
                db.ExecuteNonQuery("app.spAddPedido", l);
                if (ParameterOut_GUID.Value != DBNull.Value)
                {
                    result = Guid.Parse(ParameterOut_GUID.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pPromotor, pTablaXml);
            }
            return result;
        }
        public static DataSet spGetInfoPedidos(string pPromotor, string pConnectionStringSQL = null)
        {
            if (pConnectionStringSQL == null)
            {
                pConnectionStringSQL = Helper.getConnectionStringSQL;
            }
            BaseDataAccess db = new BaseDataAccess(pConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("promotor", pPromotor));
            return db.GetDataSet("app.spGetInfoPedidos", l);
        }
        public static DataSet spGetHistorialPedidos(string pPromotor, string pConnectionStringSQL = null)
        {
            if (pConnectionStringSQL == null)
            {
                pConnectionStringSQL = Helper.getConnectionStringSQL;
            }
            BaseDataAccess db = new BaseDataAccess(pConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("promotor", pPromotor));
            return db.GetDataSet("app.spGetHistorialPedidos", l);
        }
    }
}
