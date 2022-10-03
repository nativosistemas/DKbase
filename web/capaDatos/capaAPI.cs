using DKbase.dll;
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
        private static readonly HttpClient client = new HttpClient();
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
                if (string.IsNullOrEmpty(a) || a == "null")
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
    }
}
