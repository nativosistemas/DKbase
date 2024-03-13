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
using System.Dynamic;

namespace DKbase.web.capaDatos
{
    public class capaSAP
    {
        private static readonly HttpClient client = new HttpClient() { Timeout = TimeSpan.FromMinutes(30) };
        public static string url_SAP = Helper.getUrl_SAP;

        private static async Task<HttpResponseMessage> PostAsync(string pUrl, string name, object pParameter, bool isRepeatBecauseNotAuthorized = true)
        {
            try
            {
                string url_api = pUrl + name;

                HttpContent oHttpContent = null;
                if (pParameter == null)
                {
                    oHttpContent = new StringContent(string.Empty);
                }
                else
                {
                    var myContent = JsonSerializer.Serialize(pParameter);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    oHttpContent = byteContent;
                }
                HttpResponseMessage response = await client.PostAsync(url_api, oHttpContent);
                if (response.IsSuccessStatusCode)
                    return response;
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), "StatusCode == HttpStatusCode.Unauthorized", DateTime.Now, name, pParameter);

                }
                else
                {
                    DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), "StatusCode: " + response.StatusCode.ToString(), DateTime.Now, name, pParameter);
                }
                return null;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, name, pParameter);
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
        public static async Task<double> CreditoDisponibleAsync(int pIdCliente)
        {
            double result = 0;
            string name = "ZFI_WS_CRED_DISP_SET";
            //dynamic parameter = new ExpandoObject();
            //parameter.CLIENTE = pIdCliente;
                DKbase.Models.AuthenticateRequest parameter = new DKbase.Models.AuthenticateRequest() { login = "pLogin", pass = "pPass" };
            HttpResponseMessage response = await PostAsync(url_SAP, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    result = JsonSerializer.Deserialize<double>(resultResponse);
                }
            }
            return result;
        }
    }
}