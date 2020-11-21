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
        //public static DataSet RecuperarTodosTransferMasDetalle()
        //{
        //    BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
        //    return db.GetDataSet("Transfers.spRecuperarTodosTransferMasDetalle");
        //}
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
