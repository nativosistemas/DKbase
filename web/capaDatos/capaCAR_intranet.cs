using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class capaCAR_intranet_base
    {
        public static bool CargarCarritoDiferido(string pSucursal, string pIdProducto, int pCantidadProducto, int pIdCliente, int? pIdUsuario)
        {
            return AgregarProductoAlCarrito_generica_intranet(pSucursal, pIdProducto, pCantidadProducto, Constantes.cTipo_CarritoDiferido, pIdCliente, pIdUsuario);
        }
        public static bool AgregarProductoAlCarrito(string pSucursal, string pIdProducto, int pCantidadProducto, int pIdCliente, int? pIdUsuario)
        {
            return AgregarProductoAlCarrito_generica_intranet(pSucursal, pIdProducto, pCantidadProducto, Constantes.cTipo_Carrito, pIdCliente, pIdUsuario);
        }
        public static bool AgregarProductosTransfersAlCarrito(List<cProductosAndCantidad> pListaProductosMasCantidad, int pIdCliente, int pIdUsuario, int pIdTransfers, string pCodSucursal, string pTipo)
        {
            DataTable pTablaDetalle = FuncionesPersonalizadas_base.ConvertProductosAndCantidadToDataTable(pListaProductosMasCantidad);
            return AgregarProductosTransfersAlCarrito_intranet(pTablaDetalle, pIdCliente, pIdUsuario, pIdTransfers, pCodSucursal, pTipo);
        }
        public static List<cSucursalCarritoTransfer> RecuperarCarritosTransferPorIdClienteOrdenadosPorSucursal(Usuario pUsuario, cClientes pCliente, string pTipo)
        {
            int usu_codigo = pUsuario.id;
            DataSet dsProductoCarrito = RecuperarCarritoTransferPorIdCliente_intranet(pCliente.cli_codigo, usu_codigo, pTipo);
            return acceso.convertDataSetToSucursalCarritoTransfer(pCliente, dsProductoCarrito);
        }
        public static DataSet RecuperarCarritosPorSucursalYProductos(Usuario pUsuario, int pIdCliente)
        {
            int usu_codigo = pUsuario.id;
            return RecuperarCarritosPorSucursalYProductos_generica_intranet(pIdCliente, usu_codigo, Constantes.cTipo_Carrito);
        }
        public static DataSet RecuperarCarritosDiferidosPorCliente(Usuario pUsuario, int pIdCliente)
        {
            int usu_codigo = pUsuario.id;
            return RecuperarCarritosPorSucursalYProductos_generica_intranet(pIdCliente, usu_codigo, Constantes.cTipo_CarritoDiferido);
        }
        public static bool AgregarProductoAlCarrito_generica_intranet(string pSucursal, string pIdProducto, int pCantidadProducto, string pTipo, int pIdCliente, int? pIdUsuario)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spCargarCarritoProductoSucursal_intranet", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paSucursal = cmdComandoInicio.Parameters.Add("@codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@codCliente", SqlDbType.Int);
            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@codUsuario", SqlDbType.Int);
            SqlParameter paIdProducto = cmdComandoInicio.Parameters.Add("@codProducto", SqlDbType.NVarChar, 75);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@car_tipo", SqlDbType.NVarChar, 100);
            SqlParameter paCantidad = cmdComandoInicio.Parameters.Add("@cantidad", SqlDbType.Int);
            SqlParameter paSumar = cmdComandoInicio.Parameters.Add("@isOk", SqlDbType.Bit);
            paSumar.Direction = ParameterDirection.Output;
            if (pIdUsuario == null)
            {
                paIdUsuario.Value = DBNull.Value;
            }
            else
            {
                paIdUsuario.Value = pIdUsuario;
            }
            paSucursal.Value = pSucursal;
            paIdCliente.Value = pIdCliente;
            paIdProducto.Value = pIdProducto;
            paCantidad.Value = pCantidadProducto;
            paTipo.Value = pTipo;

            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                return Convert.ToBoolean(paSumar.Value);
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
        public static bool AgregarProductosTransfersAlCarrito_intranet(DataTable pTablaDetalleProductos, int pIdCliente, int pIdUsuario, int pIdTransfers, string pCodSucursal, string pTipo)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spCargarCarritoTransferPorDetalles_intranet", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@idUsuario", SqlDbType.Int);
            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@idCliente", SqlDbType.Int);
            SqlParameter paTabla_Detalle = cmdComandoInicio.Parameters.Add("@Tabla_Detalle", SqlDbType.Structured);
            SqlParameter paIdTransfer = cmdComandoInicio.Parameters.Add("@idTransfer", SqlDbType.Int);
            SqlParameter paCodSucursal = cmdComandoInicio.Parameters.Add("@ctr_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@tipo", SqlDbType.NVarChar, 100);
            SqlParameter paSumar = cmdComandoInicio.Parameters.Add("@isOk", SqlDbType.Bit);
            paSumar.Direction = ParameterDirection.Output;

            paTipo.Value = pTipo;
            paTabla_Detalle.TypeName = "TransferDetalleTableType";
            paTabla_Detalle.Value = pTablaDetalleProductos;
            paIdUsuario.Value = pIdUsuario;
            paIdCliente.Value = pIdCliente;
            paIdTransfer.Value = pIdTransfers;
            paCodSucursal.Value = pCodSucursal;
            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                return Convert.ToBoolean(paSumar.Value);
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

        public static DataSet RecuperarCarritoTransferPorIdCliente_intranet(int pIdCliente, int pCodCliente, string pTipo)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spRecuperarCarritoTransferPorIdCliente_intranet", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@lrc_codCliente", SqlDbType.Int);
            SqlParameter paCodUsuario = cmdComandoInicio.Parameters.Add("@codUsuario", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@tipo", SqlDbType.NVarChar, 100);
            paTipo.Value = pTipo;
            paIdCliente.Value = pIdCliente;
            paCodUsuario.Value = pCodCliente;
            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "Transfers");
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
        public static DataSet RecuperarCarritosPorSucursalYProductos_generica_intranet(int pIdCliente, int pCodCliente, string pTipo)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spRecuperarCarritosPorSucursalYProductos_intranet", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@car_codCliente", SqlDbType.Int);
            SqlParameter paCodUsuario = cmdComandoInicio.Parameters.Add("@codUsuario", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@car_tipo", SqlDbType.NVarChar, 100);
            paTipo.Value = pTipo;
            paIdCliente.Value = pIdCliente;
            paCodUsuario.Value = pCodCliente;
            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "CarritosPorSucursalYProductos");
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
        public static DataTable spRecuperarIdClientesConCarritos(int pCodUsuario)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("codUsuario", pCodUsuario));
            return db.GetDataTable("CAR.spRecuperarIdClientesConCarritos", l);
        }
        public static int BorrarCarrito(Usuario pUsuario, int pIdCliente, string pSucursal, string pTipo, string pAccion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spBorrarCarrito_intranet", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paCtr_codCliente = cmdComandoInicio.Parameters.Add("@codCliente", SqlDbType.Int);
            SqlParameter paSucursal = cmdComandoInicio.Parameters.Add("@sucursal", SqlDbType.NVarChar, 2);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@tipo", SqlDbType.NVarChar, 100);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 100);
            SqlParameter paCodUsuario = cmdComandoInicio.Parameters.Add("@codUsuario", SqlDbType.Int);
            SqlParameter paIdCarrito = cmdComandoInicio.Parameters.Add("@idCarrito", SqlDbType.Int);
            paIdCarrito.Direction = ParameterDirection.Output;
            paCodUsuario.Value = pUsuario.id;
            paCtr_codCliente.Value = pIdCliente;
            paSucursal.Value = pSucursal;
            paTipo.Value = pTipo;
            paAccion.Value = pAccion;
            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                if (paIdCarrito.Value == DBNull.Value)
                    return -1;
                return Convert.ToInt32(paIdCarrito.Value);
                //return true;
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static void guardarPedido_base(Usuario pUsuario, int pCodigoCliente, string strXML, int car_id, string codSucursal, string pTipo, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, bool pIsUrgente)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spCargarPedido_intranet", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;
            SqlParameter paLrc_id = cmdComandoInicio.Parameters.Add("@car_id", SqlDbType.Int);
            SqlParameter paLrc_codSucursal = cmdComandoInicio.Parameters.Add("@car_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter palrc_codCliente = cmdComandoInicio.Parameters.Add("@car_codCliente", SqlDbType.Int);
            SqlParameter paCodUsuario = cmdComandoInicio.Parameters.Add("@codUsuario", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@tipo", SqlDbType.NVarChar, 100);
            SqlParameter paFechaPedido = cmdComandoInicio.Parameters.Add("@FechaPedido", SqlDbType.DateTime);
            SqlParameter paMensajeEnFactura = cmdComandoInicio.Parameters.Add("@MensajeEnFactura", SqlDbType.NVarChar, -1);
            SqlParameter paMensajeEnRemito = cmdComandoInicio.Parameters.Add("@MensajeEnRemito", SqlDbType.NVarChar, -1);
            SqlParameter paTipoEnvio = cmdComandoInicio.Parameters.Add("@TipoEnvio", SqlDbType.NVarChar, -1);
            SqlParameter paIsUrgente = cmdComandoInicio.Parameters.Add("@IsUrgente", SqlDbType.Bit);
            SqlParameter paStrXML = cmdComandoInicio.Parameters.Add("@strXML", SqlDbType.Xml);
            paLrc_id.Value = car_id;
            paLrc_codSucursal.Value = codSucursal;
            palrc_codCliente.Value = pCodigoCliente;
            paFechaPedido.Value = DateTime.Now;
            paCodUsuario.Value = pUsuario.id;
            if (pMensajeEnFactura == null)
            {
                paMensajeEnFactura.Value = DBNull.Value;
            }
            else
            {
                paMensajeEnFactura.Value = pMensajeEnFactura;
            }
            if (pMensajeEnRemito == null)
            {
                paMensajeEnRemito.Value = DBNull.Value;
            }
            else
            {
                paMensajeEnRemito.Value = pMensajeEnRemito;
            }
            if (pTipoEnvio == null)
            {
                paTipoEnvio.Value = DBNull.Value;
            }
            else
            {
                paTipoEnvio.Value = pTipoEnvio;
            }
            paIsUrgente.Value = pIsUrgente;
            //paCodTransfer.Value = DBNull.Value;
            paTipo.Value = pTipo;
            paStrXML.Value = strXML;
            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //return -1;
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
