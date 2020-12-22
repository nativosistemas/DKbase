using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DKbase.generales;

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
        public static DataSet spGetLaboratorios()
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            return db.GetDataSet("app.spGetLaboratorios");
        }
        public static DataSet spGetModulos()
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            return db.GetDataSet("app.spGetModulos");
        }
    }
}
