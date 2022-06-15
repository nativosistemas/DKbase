using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        public static DataSet RecuperarCarritosPorSucursalYProductos_generica_intranet(int pIdCliente, int pCodUsuario, string pTipo)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spRecuperarCarritosPorSucursalYProductos_intranet", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@car_codCliente", SqlDbType.Int);
            SqlParameter paCodUsuario = cmdComandoInicio.Parameters.Add("@codUsuario", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@car_tipo", SqlDbType.NVarChar, 100);
            paTipo.Value = pTipo;
            paIdCliente.Value = pIdCliente;
            paCodUsuario.Value = pCodUsuario;
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
        public static List<cCarrito> RecuperarCarritosPorSucursalYProductos_generica(Usuario user, cClientes objClientes, string pTipo)
        {
            DataSet dsProductoCarrito = new DataSet();
            if (pTipo == Constantes.cTipo_Carrito)
            {
                dsProductoCarrito = RecuperarCarritosPorSucursalYProductos_generica_intranet(objClientes.cli_codigo, user.id, Constantes.cTipo_Carrito); 
            }
            else if (pTipo == Constantes.cTipo_CarritoDiferido)
            {
                dsProductoCarrito = RecuperarCarritosPorSucursalYProductos_generica_intranet(objClientes.cli_codigo, user.id, Constantes.cTipo_CarritoDiferido);
            }

            List<cCarrito> listaSucursal = (from item in dsProductoCarrito.Tables[1].AsEnumerable()
                                            select new cCarrito { car_id = item.Field<int>("car_id"), lrc_id = item.Field<int>("car_id"), codSucursal = item.Field<string>("car_codSucursal") }).ToList();

            foreach (cCarrito item in listaSucursal)
            {
                item.proximoHorarioEntrega = DKbase.web.FuncionesPersonalizadas_base.ObtenerHorarioCierre(objClientes, objClientes.cli_codsuc, item.codSucursal, objClientes.cli_codrep);
                List<cProductosGenerico> listaProductoCarrtios = new List<cProductosGenerico>();
                foreach (DataRow itemProductoCarrtio in dsProductoCarrito.Tables[0].Select("cad_codCarrito = " + item.lrc_id))
                {
                    cProductos objProducto = DKbase.web.acceso.ConvertToProductos(itemProductoCarrtio);
                    cProductosGenerico objProductosGenerico = new cProductosGenerico(objProducto);
                    if (itemProductoCarrtio.Table.Columns.Contains("stk_stock") && itemProductoCarrtio["stk_stock"] != DBNull.Value)
                        objProductosGenerico.stk_stock = itemProductoCarrtio["stk_stock"].ToString();
                    if (itemProductoCarrtio.Table.Columns.Contains("cad_codProducto") && itemProductoCarrtio["cad_codProducto"] != DBNull.Value)
                        objProductosGenerico.codProducto = itemProductoCarrtio["cad_codProducto"].ToString();
                    if (itemProductoCarrtio.Table.Columns.Contains("cad_cantidad") && itemProductoCarrtio["cad_cantidad"] != DBNull.Value)
                        objProductosGenerico.cantidad = Convert.ToInt32(itemProductoCarrtio["cad_cantidad"]);
                    listaProductoCarrtios.Add(objProductosGenerico);
                }
                /// Nuevo
                List<cTransferDetalle> listaTransferDetalle = null;
                if (dsProductoCarrito.Tables.Count > 2)
                {
                    listaTransferDetalle = new List<cTransferDetalle>();
                    DataTable tablaTransferDetalle = dsProductoCarrito.Tables[2];
                    foreach (DataRow itemTransferDetalle in tablaTransferDetalle.Rows)
                    {
                        cTransferDetalle objTransferDetalle = DKbase.web.acceso.ConvertToTransferDetalle(itemTransferDetalle);
                        objTransferDetalle.CargarTransfer(DKbase.web.acceso.ConvertToTransfer(itemTransferDetalle));
                        listaTransferDetalle.Add(objTransferDetalle);
                    }
                }
                /// FIN Nuevo
                for (int iPrecioFinal = 0; iPrecioFinal < listaProductoCarrtios.Count; iPrecioFinal++)
                {
                    listaProductoCarrtios[iPrecioFinal].PrecioFinal = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinal(objClientes, listaProductoCarrtios[iPrecioFinal]);
                    /// Nuevo
                    listaProductoCarrtios[iPrecioFinal].isProductoFacturacionDirecta = false;
                    if (listaTransferDetalle != null)
                    {
                        List<cTransferDetalle> listaAUXtransferDetalle = listaTransferDetalle.Where(x => x.tde_codpro == listaProductoCarrtios[iPrecioFinal].pro_nombre).ToList();
                        if (listaAUXtransferDetalle.Count > 0)
                        {
                            listaProductoCarrtios[iPrecioFinal].isProductoFacturacionDirecta = true;
                            listaProductoCarrtios[iPrecioFinal].CargarTransferYTransferDetalle(listaAUXtransferDetalle[0]);
                            listaProductoCarrtios[iPrecioFinal].PrecioFinalTransfer = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(objClientes, listaProductoCarrtios[iPrecioFinal].tfr_deshab, listaProductoCarrtios[iPrecioFinal].tfr_pordesadi, listaProductoCarrtios[iPrecioFinal].pro_neto, listaProductoCarrtios[iPrecioFinal].pro_codtpopro, listaProductoCarrtios[iPrecioFinal].pro_descuentoweb, listaProductoCarrtios[iPrecioFinal].tde_predescuento == null ? 0 : (decimal)listaProductoCarrtios[iPrecioFinal].tde_predescuento, listaProductoCarrtios[iPrecioFinal].tde_PrecioConDescuentoDirecto, listaProductoCarrtios[iPrecioFinal].tde_PorcARestarDelDtoDeCliente);
                        }
                    }
                    /// FIN Nuevo
                }
                item.listaProductos = listaProductoCarrtios;
            }
            listaSucursal.RemoveAll(x => x.listaProductos.Count == 0);
            return listaSucursal;
        }
    }
}
