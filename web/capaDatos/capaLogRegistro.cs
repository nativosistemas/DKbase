using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cPalabraBusqueda
    {
        public int hbp_id { get; set; }
        public string hbp_Palabra { get; set; }
        //public string hbp_NombreTabla { get; set; }
        //public int hbp_codUsuario { get; set; }
        //public DateTime? hbp_Fecha { get; set; }
    }
    public class capaLogRegistro_base
    {
        public static string spUltimoProductoSeleccionado(int pIdUsuario)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("codCliente", pIdUsuario));
            object var = db.ExecuteScalar("LogRegistro.spUltimoProductoSeleccionado", l);
            return var != null? var.ToString(): null;
        }
    }
}
