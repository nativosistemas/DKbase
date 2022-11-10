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
                result = t.Result;                
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pIsUrgente);
            }
            return result;
        }
        public static List<DKbase.dll.cDllPedido> ObtenerPedidosEntreFechas(string pLoginWeb, DateTime pDesde, DateTime pHasta)
        {
            List<DKbase.dll.cDllPedido> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerPedidosEntreFechasAsync(pLoginWeb, pDesde, pHasta));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginWeb, pDesde, pHasta);
            }
                    return result;
        }
        public static DKbase.dll.cDllPedido TomarPedidoTelefonistaAsync(Usuario pUsuario,int pIdCarrito, string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<DKbase.dll.cDllProductosAndCantidad> pListaProducto, bool pIsUrgente)
        {
            string pLoginTelefonista = pUsuario.usu_login;
            try
            {
                capaCAR_base.InicioCarritoEnProceso(pIdCarrito, Constantes.cAccionCarrito_TOMAR);
                var t = Task.Run(() => capaAPI.TomarPedidoTelefonistaAsync(pIdCarrito, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pLoginTelefonista));
                t.Wait();
                DKbase.dll.cDllPedido objResult = t.Result;
                objResult.web_Sucursal = pIdSucursal;
                return objResult;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIdCarrito, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pIsUrgente, pLoginTelefonista);
                return null;
            }
            finally
            {
                capaCAR_base.EndCarritoEnProceso(pIdCarrito);
            }
        }
        public static List<DKbase.dll.cDllPedidoTransfer> TomarPedidoDeTransfersTelefonistaAsync(Usuario pUsuario, int pIdCarrito, string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<DKbase.dll.cDllProductosAndCantidad> pListaProducto)
        {
            string pLoginTelefonista = pUsuario.usu_login;
            try
            {            
                capaCAR_base.InicioCarritoEnProceso(pIdCarrito, Constantes.cAccionCarrito_TOMAR);
                var t = Task.Run(() => capaAPI.TomarPedidoDeTransfersTelefonistaAsync(pIdCarrito, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pLoginTelefonista));
                t.Wait();
                List<DKbase.dll.cDllPedidoTransfer> objResult = t.Result;
                objResult.ForEach(o => o.web_Sucursal = pIdSucursal);
                return objResult;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIdCarrito, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pListaProducto, pLoginTelefonista);
                return null;
            }
            finally
            {
                capaCAR_base.EndCarritoEnProceso(pIdCarrito);
            }
        }
        public static bool AgregarVacunas(List<DKbase.dll.cVacuna> pListaVacunas)
        {
            bool result = false;
            try
            {
                var t = Task.Run(() => capaAPI.AgregarVacunasAsync(pListaVacunas));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pListaVacunas);
            }
            return result;
        }
        public static List<DKbase.dll.cVacuna> ObtenerTotalReservasDeVacunasPorClienteEntreFechas(DateTime pDesde, DateTime pHasta, String pLoginWEB)
        {
            List<DKbase.dll.cVacuna> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerTotalReservasDeVacunasPorClienteEntreFechasAsync(pDesde,  pHasta,  pLoginWEB));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pDesde, pHasta, pLoginWEB);
            }
            return result;
        }
        public static List<DKbase.dll.cReservaVacuna> ObtenerReservasDeVacunasPorClienteEntreFechas(DateTime pDesde, DateTime pHasta, String pLoginWEB)
        {
            List<DKbase.dll.cReservaVacuna> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerReservasDeVacunasPorClienteEntreFechasAsync(pDesde, pHasta, pLoginWEB));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pDesde, pHasta, pLoginWEB);
            }
            return result;
        }
    }
}
