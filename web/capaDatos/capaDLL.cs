using DKbase.dll;
using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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
                var t = Task.Run(() => capaAPI.TomarPedidoDeTransfersConIdCarritoAsync(pIdCarrito, pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto));
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
                var t = Task.Run(() => capaAPI.TomarPedidoAsync(pLoginCliente, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pListaProducto, pIsUrgente));
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
        public static DKbase.dll.cDllPedido TomarPedidoTelefonistaAsync(Usuario pUsuario, int pIdCarrito, string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<DKbase.dll.cDllProductosAndCantidad> pListaProducto, bool pIsUrgente)
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
        public static bool AgregarVacunas(List<DKbase.dll.cVacuna> pListaVacunas, string pLoginTelefonista = null)
        {
            bool result = false;
            try
            {
                List<DKbase.dll.cVacuna> l = pListaVacunas.Where(x => x.UnidadesVendidas > 0).ToList();
                var t = Task.Run(() => capaAPI.AgregarVacunasAsync(l, pLoginTelefonista));
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
                var t = Task.Run(() => capaAPI.ObtenerTotalReservasDeVacunasPorClienteEntreFechasAsync(pDesde, pHasta, pLoginWEB));
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
        public static void ModificarPasswordWEB(string pIdentificadorCliente, string pPassActual, string pPassNueva)
        {
            try
            {
                var t = Task.Run(() => capaAPI.ModificarPasswordWEBAsync(pIdentificadorCliente, pPassActual, pPassNueva));
                t.Wait();
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIdentificadorCliente, pPassActual, pPassNueva);
            }
        }
        public static cFactura ObtenerFactura(string pNroFactura, string pLoginWeb)
        {
            cFactura result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerFacturaAsync(pNroFactura, pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNroFactura, pLoginWeb);
            }
            return result;
        }
        public static cNotaDeCredito ObtenerNotaDeCredito(string pNroNotaDeCredito, string pLoginWeb)
        {
            cNotaDeCredito result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerNotaDeCreditoAsync(pNroNotaDeCredito, pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNroNotaDeCredito, pLoginWeb);
            }
            return result;
        }
        public static cNotaDeDebito ObtenerNotaDeDebito(string pNroNotaDeDebito, string pLoginWeb)
        {
            cNotaDeDebito result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerNotaDeDebitoAsync(pNroNotaDeDebito, pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNroNotaDeDebito, pLoginWeb);
            }
            return result;
        }
        public static cResumen ObtenerResumenCerrado(string pNroResumen, string pLoginWeb)
        {
            cResumen result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerResumenCerradoAsync(pNroResumen, pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNroResumen, pLoginWeb);
            }
            return result;
        }
        public static cObraSocialCliente ObtenerObraSocialCliente(string pNumeroObraSocialCliente, string pLoginWeb)
        {
            cObraSocialCliente result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerObraSocialClienteAsync(pNumeroObraSocialCliente, pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNumeroObraSocialCliente, pLoginWeb);
            }
            return result;
        }
        public static cRecibo ObtenerRecibo(string pNumeroDoc, string pLoginWeb)
        {
            cRecibo result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerReciboAsync(pNumeroDoc, pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNumeroDoc, pLoginWeb);
            }
            return result;
        }
        public static void ImprimirComprobante(string pTipoComprobante, string pNroComprobante)
        {
            try
            {
                var t = Task.Run(() => capaAPI.ImprimirComprobanteAsync(pTipoComprobante, pNroComprobante));
                t.Wait();
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pTipoComprobante, pNroComprobante);
            }
        }
        public static cDllSaldosComposicion ObtenerSaldosPresentacionParaComposicion(string pLoginWeb, DateTime pFecha)
        {
            cDllSaldosComposicion result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerSaldosPresentacionParaComposicionAsync(pLoginWeb, pFecha));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pFecha, pLoginWeb);
            }
            return result;
        }
        public static List<cCtaCteMovimiento> ObtenerMovimientosDeCuentaCorriente(bool pIsIncluyeCancelado, DateTime pFechaDesde, DateTime pFechaHasta, string pLoginWeb)
        {
            List<cCtaCteMovimiento> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerMovimientosDeCuentaCorrienteAsync(pIsIncluyeCancelado, pFechaDesde, pFechaHasta, pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIsIncluyeCancelado, pFechaDesde, pFechaHasta, pLoginWeb);
            }
            return result;
        }
        public static cDllRespuestaResumenAbierto ObtenerResumenAbierto(string pLoginWeb)
        {
            cDllRespuestaResumenAbierto result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerResumenAbiertoAsync(pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginWeb);
            }
            return result;
        }
        public static List<cDllChequeRecibido> ObtenerChequesEnCartera(string pLoginWeb)
        {
            List<cDllChequeRecibido> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerChequesEnCarteraAsync(pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginWeb);
            }
            return result;
        }
        public static decimal? ObtenerCreditoDisponibleSemanal(string pLoginWeb)
        {
            decimal? result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerCreditoDisponibleSemanalAsync(pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginWeb);
            }
            return result;
        }
        public static decimal? ObtenerCreditoDisponibleTotal(string pLoginWeb)
        {
            decimal? result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerCreditoDisponibleTotalAsync(pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginWeb);
            }
            return result;
        }
        public static List<cFichaCtaCte> ObtenerMovimientosDeFichaCtaCte(string pLoginWeb, DateTime pFechaDesde, DateTime pFechaHasta)
        {
            List<cFichaCtaCte> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerMovimientosDeFichaCtaCteAsync(pLoginWeb, pFechaDesde, pFechaHasta));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginWeb, pFechaDesde, pFechaHasta);
            }
            return result;
        }
        public static List<string> ObtenerTiposDeComprobantesAMostrar(string pLoginWeb)
        {
            List<string> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerTiposDeComprobantesAMostrarAsync(pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginWeb);
            }
            return result;
        }
        public static List<cPlan> ObtenerPlanesDeObrasSociales()
        {
            List<cPlan> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerPlanesDeObrasSocialesAsync());
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
            }
            return result;
        }
        public static List<cCbteParaImprimir> ObtenerComprobantesAImprimirEnBaseAResumen(string pNumeroResumen)
        {
            List<cCbteParaImprimir> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerComprobantesAImprimirEnBaseAResumenAsync(pNumeroResumen));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNumeroResumen);
            }
            return result;
        }
        public static List<cResumen> ObtenerUltimos10ResumenesDePuntoDeVenta(string pLoginWeb)
        {
            List<cResumen> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerUltimos10ResumenesDePuntoDeVentaAsync(pLoginWeb));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pLoginWeb);
            }
            return result;
        }
        public static List<cPlanillaObSoc> ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioMes(string pNombrePlan, string pLoginWeb, int pAnio, int pMes)
        {
            List<cPlanillaObSoc> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioMesAsync( pNombrePlan,  pLoginWeb,  pAnio,  pMes));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNombrePlan, pLoginWeb, pAnio, pMes);
            }
            return result;
        }
        public static List<cPlanillaObSoc> ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioMesQuincena(string pNombrePlan, string pLoginWeb, int pAnio, int pMes, int pQuincena)
        {
            List<cPlanillaObSoc> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioMesQuincenaAsync(pNombrePlan,  pLoginWeb,  pAnio,  pMes,  pQuincena));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNombrePlan, pLoginWeb, pAnio, pMes, pQuincena);
            }
            return result;
        }
        //List<cPlanillaObSoc> ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioSemana(string pNombrePlan, string pLoginWeb, int pAnio, int pSemana)
        public static List<cPlanillaObSoc> ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioSemana(string pNombrePlan, string pLoginWeb, int pAnio, int pSemana)
        {
            List<cPlanillaObSoc> result = null;
            try
            {
                var t = Task.Run(() => capaAPI.ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioSemanaAsync(pNombrePlan,  pLoginWeb,  pAnio,  pSemana));
                t.Wait();
                result = t.Result;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pNombrePlan, pLoginWeb, pAnio, pSemana);
            }
            return result;
        }
    }
}
