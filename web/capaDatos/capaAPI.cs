using DKbase.dll;
using DKbase.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DKbase.web.capaDatos
{
    public class capaAPI
    {
        private static readonly HttpClient client = new HttpClient() { Timeout = TimeSpan.FromMinutes(30) };
        public static string url_DKcore = Helper.getUrl_DKcore;
        public static string url_DKdll = Helper.getUrl_DKdll;
        private static JsonSerializerOptions oJsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private static string _pass { get; set; }
        private static string _login { get; set; }
        public static void setDatosLogin(string login, string pass)
        {
            _login = login;
            _pass = pass;
            var t = Task.Run(() => SetAuthorization());
        }
        public static async Task SetAuthorization()
        {
            if (_login != null && _pass != null)
            {
                try
                {
                    string resultResponse = await authenticate(_login, _pass);
                    DKbase.Models.AuthenticateResponse result = JsonSerializer.Deserialize<DKbase.Models.AuthenticateResponse>(resultResponse, oJsonSerializerOptions);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                }
                catch (Exception ex)
                {
                    DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                    //FuncionesPersonalizadas.grabarLog(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                }
            }
        }
     
        private static async Task<HttpResponseMessage> GetAsync(string pUrl, string name, string pParameter, bool isRepeatBecauseNotAuthorized = true)
        {
            try
            {
                string url_api = pUrl + name + "?" + pParameter;
                HttpResponseMessage response = await client.GetAsync(url_api);
                if (response.IsSuccessStatusCode)
                    return response;
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), "StatusCode == HttpStatusCode.Unauthorized", DateTime.Now, name, pParameter);
                    if (pUrl == url_DKcore)
                    {
                        if (isRepeatBecauseNotAuthorized)
                        {
                            await SetAuthorization();
                            return await GetAsync(pUrl, name, pParameter, false);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pUrl, name, pParameter);
                return null;
            }
        }
        private static async Task<HttpResponseMessage> PostAsync(string pUrl, string name, object pParameter, bool isRepeatBecauseNotAuthorized = true)
        {
            try
            {
                string url_api = pUrl + name;

                var myContent = JsonSerializer.Serialize(pParameter);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync(url_api, byteContent);
                if (response.IsSuccessStatusCode)
                    return response;
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), "StatusCode == HttpStatusCode.Unauthorized", DateTime.Now, name, pParameter);
                    if (pUrl == url_DKcore)
                    {
                        if (isRepeatBecauseNotAuthorized)
                        {
                            await SetAuthorization();
                            return await PostAsync(pUrl, name, pParameter, false);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, name, pParameter);
                //FuncionesPersonalizadas.grabarLog(MethodBase.GetCurrentMethod(), ex, DateTime.Now, name, pParameter);
                return null;
            }
        }
        public static bool isNotNull(object pp)
        {
            bool result = true;//null
            if (pp is string)
            {
                var a = pp as string;
                if (string.IsNullOrEmpty(a) || a.ToLower() == "null")
                {
                    result = false;
                }
            }
            return result;

        }
        public static async Task<string> authenticate(string pLogin, string pPass)
        {
            string result = null;
            string name = @"Authenticate";
            DKbase.Models.AuthenticateRequest parameter = new DKbase.Models.AuthenticateRequest() { login = pLogin, pass = pPass };

            HttpResponseMessage response = await PostAsync(url_DKcore, name, parameter, false);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    return resultResponse;
                }
            }
            return result;
        }
        public static async Task<DKbase.dll.cDllPedido> TomarPedidoConIdCarritoAsync(int pIdCarrito, string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<DKbase.dll.cDllProductosAndCantidad> pListaProducto, bool pIsUrgente)
        {
            DKbase.dll.cDllPedido result = null;
            string name = "TomarPedidoConIdCarrito";
            DKbase.Models.TomarPedidoConIdCarritoRequest parameter = new DKbase.Models.TomarPedidoConIdCarritoRequest() { pIdCarrito = pIdCarrito, pLoginCliente = pLoginCliente, pIdSucursal = pIdSucursal, pMensajeEnFactura = pMensajeEnFactura, pMensajeEnRemito = pMensajeEnRemito, pTipoEnvio = pTipoEnvio, pListaProducto = pListaProducto, pIsUrgente = pIsUrgente };

            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<DKbase.dll.cDllPedido>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<List<DKbase.dll.cDllPedidoTransfer>> TomarPedidoDeTransfersConIdCarritoAsync(int pIdCarrito, string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<cDllProductosAndCantidad> pListaProducto)
        {
            List<DKbase.dll.cDllPedidoTransfer> result = null;
            string name = "TomarPedidoDeTransfersConIdCarrito";
            DKbase.Models.TomarPedidoConIdCarritoRequest parameter = new DKbase.Models.TomarPedidoConIdCarritoRequest() { pIdCarrito = pIdCarrito, pLoginCliente = pLoginCliente, pIdSucursal = pIdSucursal, pMensajeEnFactura = pMensajeEnFactura, pMensajeEnRemito = pMensajeEnRemito, pTipoEnvio = pTipoEnvio, pListaProducto = pListaProducto };

            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<List<DKbase.dll.cDllPedidoTransfer>>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<DKbase.dll.cDllPedido> TomarPedidoAsync(string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<cDllProductosAndCantidad> pListaProducto, bool pIsUrgente)
        {
            DKbase.dll.cDllPedido result = null;
            string name = "TomarPedido";
            DKbase.Models.TomarPedidoConIdCarritoRequest parameter = new DKbase.Models.TomarPedidoConIdCarritoRequest() { pLoginCliente = pLoginCliente, pIdSucursal = pIdSucursal, pMensajeEnFactura = pMensajeEnFactura, pMensajeEnRemito = pMensajeEnRemito, pTipoEnvio = pTipoEnvio, pListaProducto = pListaProducto, pIsUrgente = pIsUrgente };

            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<DKbase.dll.cDllPedido>(resultResponse);
                }
            }
            return result;
        }

        public static async Task<bool> ValidarExistenciaDeCarritoWebPasadoAsync(int pIdCarrito)
        {
            bool result = false;
            string name = "ValidarExistenciaDeCarritoWebPasado";
            Models.TomarPedidoConIdCarritoRequest parameter = new Models.TomarPedidoConIdCarritoRequest()
            {
                pIdCarrito = pIdCarrito
            };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<bool>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<List<DKbase.dll.cDllPedido>> ObtenerPedidosEntreFechasAsync(string pLoginWeb, DateTime pDesde, DateTime pHasta)
        {
            List<DKbase.dll.cDllPedido> result = null;
            string name = "ObtenerPedidosEntreFechas";
            DKbase.Models.ObtenerPedidosEntreFechasRequest parameter = new DKbase.Models.ObtenerPedidosEntreFechasRequest() { pLoginWeb = pLoginWeb, pDesde = pDesde, pHasta = pHasta };

            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<List<DKbase.dll.cDllPedido>>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<DKbase.dll.cDllPedido> TomarPedidoTelefonistaAsync(int pIdCarrito, string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<DKbase.dll.cDllProductosAndCantidad> pListaProducto, string pLoginTelefonista)
        {
            DKbase.dll.cDllPedido result = null;
            string name = "TomarPedidoTelefonista";
            DKbase.Models.TomarPedidoConIdCarritoRequest parameter = new DKbase.Models.TomarPedidoConIdCarritoRequest() { pIdCarrito = pIdCarrito, pLoginCliente = pLoginCliente, pIdSucursal = pIdSucursal, pMensajeEnFactura = pMensajeEnFactura, pMensajeEnRemito = pMensajeEnRemito, pTipoEnvio = pTipoEnvio, pListaProducto = pListaProducto, pLoginTelefonista = pLoginTelefonista };
            HttpResponseMessage response = await PostAsync(url_DKcore, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                result = JsonSerializer.Deserialize<DKbase.dll.cDllPedido>(resultResponse);
            }
            return result;
        }
        public static async Task<List<DKbase.dll.cDllPedidoTransfer>> TomarPedidoDeTransfersTelefonistaAsync(int pIdCarrito, string pLoginCliente, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, List<DKbase.dll.cDllProductosAndCantidad> pListaProducto, string pLoginTelefonista)
        {
            List<DKbase.dll.cDllPedidoTransfer> result = null;
            string name = "TomarPedidoDeTransfersTelefonista";
            DKbase.Models.TomarPedidoConIdCarritoRequest parameter = new DKbase.Models.TomarPedidoConIdCarritoRequest() { pIdCarrito = pIdCarrito, pLoginCliente = pLoginCliente, pIdSucursal = pIdSucursal, pMensajeEnFactura = pMensajeEnFactura, pMensajeEnRemito = pMensajeEnRemito, pTipoEnvio = pTipoEnvio, pListaProducto = pListaProducto, pLoginTelefonista = pLoginTelefonista };
            HttpResponseMessage response = await PostAsync(url_DKcore, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                result = JsonSerializer.Deserialize<List<DKbase.dll.cDllPedidoTransfer>>(resultResponse);
            }
            return result;
        }
        public static async Task<bool> AgregarVacunasAsync(List<DKbase.dll.cVacuna> pListaVacunas, string pLoginTelefonista)
        {
            bool result = false;
            string name = "AgregarVacunas";
            DKbase.Models.VacunasRequest parameter = new DKbase.Models.VacunasRequest() { pVacunas = pListaVacunas, pLoginTelefonista = pLoginTelefonista };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<bool>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<List<DKbase.dll.cVacuna>> ObtenerTotalReservasDeVacunasPorClienteEntreFechasAsync(DateTime pDesde, DateTime pHasta, String pLoginWEB)
        {
            List<DKbase.dll.cVacuna> result = null;
            string name = "ObtenerTotalReservasDeVacunasPorClienteEntreFechas";
            DKbase.Models.VacunasRequest parameter = new DKbase.Models.VacunasRequest() { pDesde = pDesde, pHasta = pHasta, pLoginWEB = pLoginWEB };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<List<DKbase.dll.cVacuna>>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<List<DKbase.dll.cReservaVacuna>> ObtenerReservasDeVacunasPorClienteEntreFechasAsync(DateTime pDesde, DateTime pHasta, String pLoginWEB)
        {
            List<DKbase.dll.cReservaVacuna> result = null;
            string name = "ObtenerReservasDeVacunasPorClienteEntreFechas";
            DKbase.Models.VacunasRequest parameter = new DKbase.Models.VacunasRequest() { pDesde = pDesde, pHasta = pHasta, pLoginWEB = pLoginWEB };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<List<DKbase.dll.cReservaVacuna>>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<string> ModificarPasswordWEBAsync(string pIdentificadorCliente, string pPassActual, string pPassNueva)
        {
            string result = null;
            string name = "ModificarPasswordWEB";
            var parameter = new { pIdentificadorCliente = pIdentificadorCliente, pPassActual = pPassActual, pPassNueva = pPassNueva };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            result = string.Empty;
            //if (response != null)
            //{
            //    var resultResponse = response.Content.ReadAsStringAsync().Result;
            //    if (isNotNull(resultResponse))
            //    {
            //        result = JsonSerializer.Deserialize<bool>(resultResponse);
            //    }
            //}
            return result;
        }
        public static async Task<cFactura> ObtenerFacturaAsync(string pNroFactura, string pLoginWeb)
        {
            cFactura result = null;
            string name = "ObtenerFactura";
            var parameter = new DocumentoRequest { documentoID = pNroFactura, loginWeb = pLoginWeb };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            ////(string pNumeroFactura, string pLoginWeb)
            //string parameter = "?" + "pNumeroFactura=" + pNroFactura + "&" + "pLoginWeb=" + pLoginWeb;
            // HttpResponseMessage response = await GetAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<cFactura>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<cNotaDeCredito> ObtenerNotaDeCreditoAsync(string pNroNotaDeCredito, string pLoginWeb)
        {
            cNotaDeCredito result = null;
            string name = "ObtenerNotaDeCredito";
            var parameter = new DocumentoRequest { documentoID = pNroNotaDeCredito, loginWeb = pLoginWeb }; //new { pNroNotaDeCredito = pNroNotaDeCredito, pLoginWeb = pLoginWeb };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);      
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<cNotaDeCredito>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<cNotaDeDebito> ObtenerNotaDeDebitoAsync(string pNroNotaDeDebito, string pLoginWeb)
        {
            cNotaDeDebito result = null;
            string name = "ObtenerNotaDeDebito";
            var parameter = new DocumentoRequest { documentoID = pNroNotaDeDebito, loginWeb = pLoginWeb }; //new { pNroNotaDeDebito = pNroNotaDeDebito, pLoginWeb = pLoginWeb };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<cNotaDeDebito>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<cResumen> ObtenerResumenCerradoAsync(string pNroResumen, string pLoginWeb)
        {
            cResumen result = null;
            string name = "ObtenerResumenCerrado";
            var parameter = new DocumentoRequest { documentoID = pNroResumen, loginWeb = pLoginWeb }; //new { pNroResumen = pNroResumen, pLoginWeb = pLoginWeb };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<cResumen>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<cObraSocialCliente> ObtenerObraSocialClienteAsync(string pNumeroObraSocialCliente, string pLoginWeb)
        {
            cObraSocialCliente result = null;
            string name = "ObtenerObraSocialCliente";
            var parameter = new DocumentoRequest { documentoID = pNumeroObraSocialCliente, loginWeb = pLoginWeb }; //new { pNumeroObraSocialCliente = pNumeroObraSocialCliente, pLoginWeb = pLoginWeb };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<cObraSocialCliente>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<cRecibo> ObtenerReciboAsync(string pNumeroDoc, string pLoginWeb)
        {
            cRecibo result = null;
            string name = "ObtenerRecibo";
            var parameter = new DocumentoRequest { documentoID = pNumeroDoc, loginWeb = pLoginWeb }; //new { pNumeroDoc = pNumeroDoc, pLoginWeb = pLoginWeb };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<cRecibo>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<string> ImprimirComprobanteAsync(string pTipoComprobante, string pNroComprobante)
        {
            string result = null;//ImprimirComprobante(string pTipoComprobante, string pNroComprobante)
            string name = "ImprimirComprobante";
            var parameter = new DocumentoRequest { documentoTipo = pTipoComprobante, documentoID = pNroComprobante }; //new { pTipoComprobante = pTipoComprobante, pNroComprobante = pNroComprobante };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            result = string.Empty;
            //if (response != null)
            //{
            //    var resultResponse = response.Content.ReadAsStringAsync().Result;
            //    if (isNotNull(resultResponse))
            //    {
            //        result = JsonSerializer.Deserialize<bool>(resultResponse);
            //    }
            //}
            return result;
        }
        public static async Task<cDllSaldosComposicion> ObtenerSaldosPresentacionParaComposicionAsync(string pLoginWeb, DateTime pFecha)
        {
            cDllSaldosComposicion result = null;
            string name = "ObtenerSaldosPresentacionParaComposicion";
            var parameter = new DocumentoRequest { loginWeb = pLoginWeb, fecha = pFecha };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<cDllSaldosComposicion>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<List<cCtaCteMovimiento>> ObtenerMovimientosDeCuentaCorrienteAsync(bool pIsIncluyeCancelado, DateTime pFechaDesde, DateTime pFechaHasta, string pLoginWeb)
        {
            List<cCtaCteMovimiento> result = null;
            string name = "ObtenerMovimientosDeCuentaCorriente";
            var parameter = new DocumentoRequest { loginWeb = pLoginWeb, fechaDesde = pFechaDesde,fechaHasta = pFechaHasta,isIncluyeCancelado = pIsIncluyeCancelado };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<List<cCtaCteMovimiento>>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<cDllRespuestaResumenAbierto> ObtenerResumenAbiertoAsync(string pLoginWeb)
        {
            cDllRespuestaResumenAbierto result = null;
            string name = "ObtenerResumenAbierto";
            var parameter = new DocumentoRequest { loginWeb = pLoginWeb};
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<cDllRespuestaResumenAbierto>(resultResponse);
                }
            }
            return result;
        }
        public static async Task<List<cDllChequeRecibido>> ObtenerChequesEnCarteraAsync(string pLoginWeb)
        {
            List<cDllChequeRecibido> result = null;
            string name = "ObtenerChequesEnCartera";
            var parameter = new DocumentoRequest { loginWeb = pLoginWeb };
            HttpResponseMessage response = await PostAsync(url_DKdll, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<List<cDllChequeRecibido>>(resultResponse);
                }
            }
            return result;
        }
    }
}
