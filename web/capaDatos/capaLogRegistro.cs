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
            return var != null ? var.ToString() : null;
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
        public static bool BorrarPorProductosFaltasProblemasCrediticiosV3(DataTable pTablaProducto, string fpc_codSucursal, int fpc_codCliente, int fpc_tipo, int pCantidadDia)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spBorrarPorProductosFaltasProblemasCrediticiosV3", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paTablaProductos = cmdComandoInicio.Parameters.Add("@Tabla_Detalle", SqlDbType.Structured);
            SqlParameter paCodSucursal = cmdComandoInicio.Parameters.Add("@fpc_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paCodCliente = cmdComandoInicio.Parameters.Add("@fpc_codCliente", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@fpc_tipo", SqlDbType.Int);
            //SqlParameter paFpc_nombreProducto = cmdComandoInicio.Parameters.Add("@fpc_nombreProducto", SqlDbType.NVarChar, 75);
            SqlParameter paCantidadDia = cmdComandoInicio.Parameters.Add("@cantidadDia", SqlDbType.Int);
            // SqlParameter paCantidadProductoGrabarNuevo = cmdComandoInicio.Parameters.Add("@cantidadProductoGrabarNuevo", SqlDbType.Int);

            if (pTablaProducto == null)
            {
                paTablaProductos.Value = DBNull.Value;
            }
            else
            {
                paTablaProductos.Value = pTablaProducto;
            }
            paCodSucursal.Value = fpc_codSucursal;
            paCodCliente.Value = fpc_codCliente;
            paTipo.Value = fpc_tipo;
            //paFpc_nombreProducto.Value = fpc_nombreProducto;
            paCantidadDia.Value = pCantidadDia;
            //paCantidadProductoGrabarNuevo.Value = pCantidadProductoGrabarNuevo;

            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static DataSet GestiónFaltantesProblemasCrediticios(int? fpc_id, int? fpc_codCarrito, string fpc_codSucursal, int? fpc_codCliente, string fpc_nombreProducto, int? fpc_cantidad, int? fpc_tipo, DateTime? fpc_fecha, string accion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spGestionFaltasProblemasCrediticios", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paFpc_id = cmdComandoInicio.Parameters.Add("@fpc_id", SqlDbType.Int);
            SqlParameter paFpc_codCarrito = cmdComandoInicio.Parameters.Add("@fpc_codCarrito", SqlDbType.Int);
            SqlParameter paFpc_codSucursal = cmdComandoInicio.Parameters.Add("@fpc_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paFpc_codCliente = cmdComandoInicio.Parameters.Add("@fpc_codCliente", SqlDbType.Int);
            SqlParameter paFpc_nombreProducto = cmdComandoInicio.Parameters.Add("@fpc_nombreProducto", SqlDbType.NVarChar, 75);
            SqlParameter paFpc_cantidad = cmdComandoInicio.Parameters.Add("@fpc_cantidad", SqlDbType.Int);
            SqlParameter paFpc_tipo = cmdComandoInicio.Parameters.Add("@fpc_tipo", SqlDbType.Int);
            SqlParameter paFpc_fecha = cmdComandoInicio.Parameters.Add("@fpc_fecha", SqlDbType.DateTime);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 50);

            if (fpc_id == null)
            {
                paFpc_id.Value = DBNull.Value;
            }
            else
            {
                paFpc_id.Value = fpc_id;
            }
            if (fpc_codCarrito == null)
            {
                paFpc_codCarrito.Value = DBNull.Value;
            }
            else
            {
                paFpc_codCarrito.Value = fpc_codCarrito;
            }
            if (fpc_codSucursal == null)
            {
                paFpc_codSucursal.Value = DBNull.Value;
            }
            else
            {
                paFpc_codSucursal.Value = fpc_codSucursal;
            }
            if (fpc_codCliente == null)
            {
                paFpc_codCliente.Value = DBNull.Value;
            }
            else
            {
                paFpc_codCliente.Value = fpc_codCliente;
            }
            if (fpc_nombreProducto == null)
            {
                paFpc_nombreProducto.Value = DBNull.Value;
            }
            else
            {
                paFpc_nombreProducto.Value = fpc_nombreProducto;
            }

            if (fpc_cantidad == null)
            {
                paFpc_cantidad.Value = DBNull.Value;
            }
            else
            {
                paFpc_cantidad.Value = fpc_cantidad;
            }
            if (fpc_tipo == null)
            {
                paFpc_tipo.Value = DBNull.Value;
            }
            else
            {
                paFpc_tipo.Value = fpc_tipo;
            }
            if (fpc_fecha == null)
            {
                paFpc_fecha.Value = DBNull.Value;
            }
            else
            {
                paFpc_fecha.Value = fpc_fecha;
            }
            paAccion.Value = accion;
            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "FaltantesProblemasCrediticios");
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
        public static int InsertarFaltantesProblemasCrediticios(int? fpc_codCarrito, string fpc_codSucursal, int fpc_codCliente, string fpc_nombreProducto, int fpc_cantidad, int fpc_tipo)
        {
            DataSet dsResultado = capaLogRegistro_base.GestiónFaltantesProblemasCrediticios(null, fpc_codCarrito, fpc_codSucursal, fpc_codCliente, fpc_nombreProducto, fpc_cantidad, fpc_tipo, null, Constantes.cSQL_INSERT);
            int resultado = -1;
            if (dsResultado != null)
            {
                if (dsResultado.Tables["FaltantesProblemasCrediticios"].Rows[0]["fpc_id"] != DBNull.Value)
                {
                    resultado = Convert.ToInt32(dsResultado.Tables["FaltantesProblemasCrediticios"].Rows[0]["fpc_id"]);
                }
            }
            return resultado;
        }
    }
}
