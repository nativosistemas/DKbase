using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Xml.Linq;

namespace DKbase.web.capaDatos
{
    public class capaCAR_base
    {
        public static bool AgregarProductoAlCarrito(string pSucursal, string pIdProducto, int pCantidadProducto, int pIdCliente, int? pIdUsuario)
        {
            return AgregarProductoAlCarrito_generica(pSucursal, pIdProducto, pCantidadProducto, Constantes.cTipo_Carrito, pIdCliente, pIdUsuario);
        }
        public static bool CargarCarritoDiferido(string pSucursal, string pIdProducto, int pCantidadProducto, int pIdCliente, int? pIdUsuario)
        {
            return AgregarProductoAlCarrito_generica(pSucursal, pIdProducto, pCantidadProducto, Constantes.cTipo_CarritoDiferido, pIdCliente, pIdUsuario);
        }
        public static bool AgregarProductoAlCarrito_generica(string pSucursal, string pIdProducto, int pCantidadProducto, string pTipo, int pIdCliente, int? pIdUsuario)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spCargarCarritoProductoSucursal", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paSucursal = cmdComandoInicio.Parameters.Add("@codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@codCliente", SqlDbType.Int);
            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@codUsuario", SqlDbType.Int);
            //SqlParameter paCodTransfer = cmdComandoInicio.Parameters.Add("@car_codTransfer", SqlDbType.Int);
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
            //if (car_codTransfer == null)
            //{
            //    paCodTransfer.Value = DBNull.Value;
            //}
            //else
            //{
            //    paCodTransfer.Value = car_codTransfer;
            //}
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
        public static DataSet RecuperarCarritosPorSucursalYProductos(int pIdCliente)
        {
            return RecuperarCarritosPorSucursalYProductos_generica(pIdCliente, Constantes.cTipo_Carrito);
        }
        public static DataSet RecuperarCarritosDiferidosPorCliente(int pIdCliente)
        {
            return RecuperarCarritosPorSucursalYProductos_generica(pIdCliente, Constantes.cTipo_CarritoDiferido);
        }
        public static DataSet RecuperarCarritosPorSucursalYProductos_generica(int pIdCliente, string pTipo)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spRecuperarCarritosPorSucursalYProductos", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@car_codCliente", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@car_tipo", SqlDbType.NVarChar, 100);
            paTipo.Value = pTipo;
            paIdCliente.Value = pIdCliente;

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
        public static int BorrarCarrito(int pIdCliente, string pSucursal, string pTipo, string pAccion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spBorrarCarrito", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paCtr_codCliente = cmdComandoInicio.Parameters.Add("@codCliente", SqlDbType.Int);
            SqlParameter paSucursal = cmdComandoInicio.Parameters.Add("@sucursal", SqlDbType.NVarChar, 2);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@tipo", SqlDbType.NVarChar, 100);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 100);
            SqlParameter paIdCarrito = cmdComandoInicio.Parameters.Add("@idCarrito", SqlDbType.Int);
            paIdCarrito.Direction = ParameterDirection.Output;

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
        public static bool BorrarCarritoPorId_SleepTimer(int car_id, string pAccion)
        {
            bool isBorrar = capaCAR_base.BorrarCarritoPorId(car_id, pAccion);
            if (isBorrar)
                return isBorrar;
            else
            {
                Random rd = new Random(car_id);
                int n = 0;
                int minValue = rd.Next(100, 800);
                int maxValue = rd.Next(1000, 7500);
                while (n < 10)
                {
                    int time = rd.Next(minValue, maxValue);
                    System.Threading.Thread.Sleep(time);
                    isBorrar = capaCAR_base.BorrarCarritoPorId(car_id, pAccion);
                    if (isBorrar)
                        return isBorrar;
                    n++;
                    minValue = rd.Next(maxValue, maxValue + 800);
                    maxValue = rd.Next(minValue + 1000, minValue + 7500);
                    if (minValue >= maxValue)
                        maxValue += minValue;
                }
            }
            return false;
        }
        public static bool BorrarCarritoPorId(int car_id, string pAccion)
        {
            //return false;
            // /*
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spBorrarCarritoPorId", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paCar_id = cmdComandoInicio.Parameters.Add("@car_id", SqlDbType.Int);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 100);
            SqlParameter paIsOk = cmdComandoInicio.Parameters.Add("@isOk", SqlDbType.Bit);
            paIsOk.Direction = ParameterDirection.Output;


            paCar_id.Value = car_id;
            paAccion.Value = pAccion;
            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                return Convert.ToBoolean(paIsOk.Value);
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
            //  */
        }

        public static void guardarPedido(int pCodigoCliente, cCarrito pCarrito, string pTipo, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, bool pIsUrgente)
        {
            string strXML = string.Empty;
            strXML += "<Root>";
            foreach (cProductosGenerico item in pCarrito.listaProductos)
            {
                List<XAttribute> listaAtributos = new List<XAttribute>();

                listaAtributos.Add(new XAttribute("lcp_cantidad", item.cantidad));
                listaAtributos.Add(new XAttribute("codigo", item.codProducto));
                listaAtributos.Add(new XAttribute("nombre", item.pro_nombre));
                listaAtributos.Add(new XAttribute("codTransfer", item.tde_codtfr));
                XElement nodo = new XElement("DetallePedido", listaAtributos);
                strXML += nodo.ToString();
            }
            strXML += "</Root>";
            guardarPedido_base(pCodigoCliente,strXML, pCarrito.lrc_id, pCarrito.codSucursal, pTipo, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pIsUrgente);
        }
        public static void guardarPedido(int pCodigoCliente, List<cProductosGenerico> pListaProductos, int car_id, string codSucursal, string pTipo, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, bool pIsUrgente)
        {
            string strXML = string.Empty;
            strXML += "<Root>";
            foreach (cProductosGenerico item in pListaProductos)
            {
                List<XAttribute> listaAtributos = new List<XAttribute>();

                listaAtributos.Add(new XAttribute("lcp_cantidad", item.cantidad));
                listaAtributos.Add(new XAttribute("codigo", item.codProducto));
                listaAtributos.Add(new XAttribute("nombre", item.pro_nombre));
                listaAtributos.Add(new XAttribute("codTransfer", item.tde_codtfr));
                XElement nodo = new XElement("DetallePedido", listaAtributos);
                strXML += nodo.ToString();
            }
            strXML += "</Root>";
            guardarPedido_base(pCodigoCliente,strXML, car_id, codSucursal, pTipo, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pIsUrgente);
        }
        public static void guardarPedido_base(int pCodigoCliente, string strXML, int car_id, string codSucursal, string pTipo, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, bool pIsUrgente)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spCargarPedido", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;
            SqlParameter paLrc_id = cmdComandoInicio.Parameters.Add("@car_id", SqlDbType.Int);
            SqlParameter paLrc_codSucursal = cmdComandoInicio.Parameters.Add("@car_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter palrc_codCliente = cmdComandoInicio.Parameters.Add("@car_codCliente", SqlDbType.Int);
            // SqlParameter paCodTransfer = cmdComandoInicio.Parameters.Add("@codTransfer", SqlDbType.Int);
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
        public static DataSet RecuperarCarritoTransferPorIdCliente(int pIdCliente, string pTipo)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spRecuperarCarritoTransferPorIdCliente", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@lrc_codCliente", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@tipo", SqlDbType.NVarChar, 100);
            paTipo.Value = pTipo;
            paIdCliente.Value = pIdCliente;

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
        public static bool AgregarProductosTransfersAlCarrito(DataTable pTablaDetalleProductos, int pIdCliente, int pIdUsuario, int pIdTransfers, string pCodSucursal, string pTipo)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spCargarCarritoTransferPorDetalles", Conn);
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
                //object objResultado = cmdComandoInicio.ExecuteNonQuery();
                //return true;

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
        public static bool SubirPedido(string pTablaXml, int pIdCliente, int pIdUsuario, string pTipoCarrito, string pTipoCarritoTransfer)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spSubirPedido", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@idUsuario", SqlDbType.Int);
            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@idCliente", SqlDbType.Int);
            SqlParameter paStrXML = cmdComandoInicio.Parameters.Add("@strXML", SqlDbType.Xml);
            SqlParameter paTipoCarrito = cmdComandoInicio.Parameters.Add("@tipoCarrito", SqlDbType.NVarChar, 100);
            SqlParameter paTipoCarritoTransfer = cmdComandoInicio.Parameters.Add("@tipoCarritoTransfer", SqlDbType.NVarChar, 100);

            SqlParameter paSumar = cmdComandoInicio.Parameters.Add("@isOk", SqlDbType.Bit);
            paSumar.Direction = ParameterDirection.Output;

            paTipoCarrito.Value = pTipoCarrito;
            paTipoCarritoTransfer.Value = pTipoCarritoTransfer;

            paStrXML.Value = pTablaXml;
            paIdUsuario.Value = pIdUsuario;
            paIdCliente.Value = pIdCliente;


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
        public static bool IsCarritoEnProceso(int car_id)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spIsCarritoEnProceso", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paCar_id = cmdComandoInicio.Parameters.Add("@car_id", SqlDbType.Int);
            SqlParameter paIsOk = cmdComandoInicio.Parameters.Add("@isOk", SqlDbType.Bit);
            paIsOk.Direction = ParameterDirection.Output;
            paCar_id.Value = car_id;

            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                if (paIsOk.Value == DBNull.Value)
                    return false;
                return Convert.ToBoolean(paIsOk.Value);
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
        public static bool InicioCarritoEnProceso(int car_id, string pAccion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spInicioCarritoEnProceso", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paCar_id = cmdComandoInicio.Parameters.Add("@car_id", SqlDbType.Int);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 100);
            SqlParameter paIsOk = cmdComandoInicio.Parameters.Add("@isOk", SqlDbType.Bit);
            paIsOk.Direction = ParameterDirection.Output;
            paCar_id.Value = car_id;
            paAccion.Value = pAccion;
            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                return Convert.ToBoolean(paIsOk.Value);
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
        public static bool EndCarritoEnProceso(int car_id)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("CAR.spEndCarritoEnProceso", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;
            SqlParameter paCar_id = cmdComandoInicio.Parameters.Add("@car_id", SqlDbType.Int);
            SqlParameter paIsOk = cmdComandoInicio.Parameters.Add("@isOk", SqlDbType.Bit);
            paIsOk.Direction = ParameterDirection.Output;
            paCar_id.Value = car_id;

            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                return Convert.ToBoolean(paIsOk.Value);
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
    }
}
