using DKbase.dll;
using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DKbase.web.capaDatos
{
    public class capaDLL
    {
        public static bool ValidarExistenciaDeCarritoWebPasado(int pIdCarrito)
        {
            bool result = false;
            try
            {
                var t = Task.Run(() => capaAPI.ValidarExistenciaDeCarritoWebPasadoAsync(pIdCarrito));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIdCarrito);
            }
            return result;
            //try
            //{
            //    //ServiceReferenceDLL.ServiceSoapClient objServicio = Instacia();
            //    //return objServicio.ValidarExistenciaDeCarritoWebPasado(pIdCarrito);
            //}
            //catch (Exception ex)
            //{
            //    //FuncionesPersonalizadas.grabarLog(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIdCarrito);
            //    //return false;
            //}
            //return true;
        }
        public static cDllPedido TomarPedidoConIdCarrito(int pIdCarrito, string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<cDllProductosAndCantidad> pListaProducto, bool pIsUrgente)
        {
            DKbase.dll.cDllPedido result = null;
            try
            {
                capaCAR_base.InicioCarritoEnProceso(pIdCarrito, Constantes.cAccionCarrito_TOMAR);
                List<DKbase.dll.cDllProductosAndCantidad> l_Productos = pListaProducto;
                var t = Task.Run(() => capaAPI.TomarPedidoConIdCarritoAsync(pIdCarrito, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, l_Productos, pIsUrgente));
                t.Wait();
                if (t.Result == null)
                {
                    throw new Exception("Result == null");
                }
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIdCarrito, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pIsUrgente);
            }
            finally
            {
                capaCAR_base.EndCarritoEnProceso(pIdCarrito);
            }
            return result;
        }
        public static List<cDllPedidoTransfer> TomarPedidoDeTransfersConIdCarrito(int pIdCarrito, string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<cDllProductosAndCantidad> pListaProducto)
        {
            List<cDllPedidoTransfer> result = null;
            try
            {
                capaCAR_base.InicioCarritoEnProceso(pIdCarrito, Constantes.cAccionCarrito_TOMAR);
                var t = Task.Run(() => capaAPI.TomarPedidoDeTransfersConIdCarritoAsync(pIdCarrito,  pLoginCliente,  pIdSucursal,  pMensajeEnFactura,  pMensajeEnRemito,  pTipoEnvio,  pListaProducto));
                t.Wait();
                if (t.Result == null)
                {
                    throw new Exception("Result == null");
                }
                result = t.Result;
                //ServiceReferenceDLL.ServiceSoapClient objServicio = Instacia();
                //ServiceReferenceDLL.ArrayOfCDllProductosAndCantidad listaArray = new ServiceReferenceDLL.ArrayOfCDllProductosAndCantidad();
                //foreach (var item in pListaProducto)
                //{
                //    listaArray.Add(item);
                //}
                //List<ServiceReferenceDLL.cDllPedidoTransfer> listaResultado = objServicio.TomarPedidoDeTransfersConIdCarrito(pIdCarrito, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, listaArray);
                //return listaResultado;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIdCarrito, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto);
          
            }
            finally
            {
                capaCAR_base.EndCarritoEnProceso(pIdCarrito);
            }
            return result;
        }
        public static cDllPedido TomarPedido(string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<cDllProductosAndCantidad> pListaProducto, bool pIsUrgente)
        {
            DKbase.dll.cDllPedido result = null;
            try
            {
                var t = Task.Run(() => capaAPI.TomarPedidoAsync(pLoginCliente,  pIdSucursal,  pMensajeEnFactura,  pMensajeEnRemito,  pTipoEnvio,  pListaProducto,  pIsUrgente));
                t.Wait();
                if (t.Result == null)
                {
                    throw new Exception("Result == null");
                }
                result = t.Result;
                
                //ServiceReferenceDLL.ServiceSoapClient objServicio = Instacia();
                //ServiceReferenceDLL.ArrayOfCDllProductosAndCantidad listaArray = new ServiceReferenceDLL.ArrayOfCDllProductosAndCantidad();
                //foreach (var item in pListaProducto)
                //{
                //    listaArray.Add(item);
                //}
                //ServiceReferenceDLL.cDllPedido objResultado = objServicio.TomarPedido(pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, listaArray, pIsUrgente);
                //ServiceReferenceDLL.cDllPedido objResultado = null;

                //FuncionesPersonalizadas.grabarLog(MethodBase.GetCurrentMethod(), new Exception("eeee"), DateTime.Now, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pIsUrgente);

               //return objResultado;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pIsUrgente);
            }
            return result;
        }
        public cDllPedido TomarPedido(string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<cDllProductosAndCantidad> pListaProducto, bool pIsUrgente)
        {
            cDllPedido ResultadoFinal = null;
            //if (VerificarPermisos(CredencialAutenticacion))
            //{
                classTiempo tiempo = new classTiempo("TomarPedido");
                //ResultadoFinal = new cDllPedido();
                try
                {
                    dkInterfaceWeb.Pedido Resultado;
                    dkInterfaceWeb.ServiciosWEB objServWeb = new dkInterfaceWeb.ServiciosWEB();
                    dkInterfaceWeb.Pedido pedido = new dkInterfaceWeb.Pedido();

                    // llenar datos pedidos
                    pedido.Login = pLoginCliente;
                    pedido.MensajeEnFactura = pMensajeEnFactura;
                    pedido.MensajeEnRemito = pMensajeEnRemito;

                    //List<cDllProductosAndCantidad> ListaProducto = Serializador.DeserializarJson<List<cDllProductosAndCantidad>>(pListaProducto);
                    // Cargar productos al carrito
                    foreach (cDllProductosAndCantidad item in pListaProducto)
                    {
                        pedido.Add(item.codProductoNombre, item.cantidad, item.isOferta ? "$" : " ");
                    }
                    dkInterfaceWeb.TipoEnvio tipoEnvio = dkInterfaceWeb.TipoEnvio.Reparto;
                    switch (pTipoEnvio)
                    {
                        case "E":
                            tipoEnvio = dkInterfaceWeb.TipoEnvio.Encomienda;
                            break;
                        case "R":
                            tipoEnvio = dkInterfaceWeb.TipoEnvio.Reparto;
                            break;
                        case "C":
                            tipoEnvio = dkInterfaceWeb.TipoEnvio.Cadeteria;
                            break;
                        case "M":
                            tipoEnvio = dkInterfaceWeb.TipoEnvio.Mostrador;
                            break;
                            //default:
                            //    break;
                    }

                    Resultado = objServWeb.TomarPedido(pedido, tipoEnvio, pIdSucursal, @"C:\RutaArchivoDLL", pIsUrgente);
                    if (Resultado != null)
                    {
                        ResultadoFinal = dllFuncionesGenerales.ToConvert(Resultado);
                    }
                }
                catch (Exception ex)
                {
                    DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pIsUrgente);
                    return null;
                }
                finally
                {
                    tiempo.Parar();
                }
            //}
            return ResultadoFinal;
        }
    }
}
