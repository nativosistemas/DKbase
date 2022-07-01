using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
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
        public static DataSet AgregarProductoAlCarritoDesdeArchivoPedidosV5(string pSucursalElejida, string pSucursalCliente, DataTable pTabla, string pTipoDeArchivo, int pIdCliente, string pCli_codprov, bool pCli_isGLN, int? pIdUsuario)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spCargarCarritoProductoSucursalDesdeArchivoPedidosV5", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paSucursalElejida = cmdComandoInicio.Parameters.Add("@lrc_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paSucursalCliente = cmdComandoInicio.Parameters.Add("@Sucursal", SqlDbType.NVarChar, 2);
            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@lrc_codCliente", SqlDbType.Int);
            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@lcp_codUsuario", SqlDbType.Int);
            SqlParameter paTabla_Detalle = cmdComandoInicio.Parameters.Add("@Tabla_Detalle", SqlDbType.Structured);
            SqlParameter paTipoDeArchivo = cmdComandoInicio.Parameters.Add("@TipoDeArchivo", SqlDbType.NVarChar, 1);
            SqlParameter paCli_codprov = cmdComandoInicio.Parameters.Add("@cli_codprov", SqlDbType.NVarChar, 75);
            SqlParameter paCli_isGLN = cmdComandoInicio.Parameters.Add("@cli_isGLN", SqlDbType.Bit);

            if (pIdUsuario == null)
            {
                paIdUsuario.Value = DBNull.Value;
            }
            else
            {
                paIdUsuario.Value = pIdUsuario;
            }
            paSucursalCliente.Value = pSucursalCliente;
            paSucursalElejida.Value = pSucursalElejida;
            paIdCliente.Value = pIdCliente;
            paTabla_Detalle.Value = pTabla;
            paTipoDeArchivo.Value = pTipoDeArchivo;
            paCli_codprov.Value = pCli_codprov;
            paCli_isGLN.Value = pCli_isGLN;

            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "ProductosBuscador");
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
