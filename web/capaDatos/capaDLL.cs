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
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pIsUrgente);
            }
            return result;
        }

    }
}
