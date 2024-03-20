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

namespace DKbase //namespace DKbase.web.capaDatos
{
    public class capaSAP
    {
        private static readonly HttpClient client = new HttpClient() { Timeout = TimeSpan.FromMinutes(30) };
        public static string url_SAP = Helper.getUrl_SAP;
        private static readonly System.Globalization.CultureInfo culture_enUS = new System.Globalization.CultureInfo("en-US");
        private static readonly string authenticationString = $"{Helper.getSAP_user}:{Helper.getSAP_pass}";
        private static readonly string base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
        private static async Task<HttpResponseMessage> PostAsync(string pUrl, string name, object pParameter)
        {
            HttpResponseMessage result = null;
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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $"{base64EncodedAuthenticationString}");
                HttpResponseMessage response = await client.PostAsync(url_api, oHttpContent);
                if (response.IsSuccessStatusCode)

                {
                    result = response;
                }
                else
                {
                    // (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), "StatusCode: " + response.StatusCode.ToString(), DateTime.Now, name, pParameter);
                }
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, name, pParameter);
            }
            return result;
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
        public static string convertSAPformat_CLIENTE(int pIdCliente)
        {
            return pIdCliente.ToString();
        }
        public static double convertSAPformat_Double(string pValue)
        {
            return double.Parse(pValue, culture_enUS);//Convert.ToDouble(pValue);
        }
        public static async Task<double> CRED_DISP(int pIdCliente)
        {
            double result = 0;
            string name = "ZFI_WS_CRED_DISP_SET";
            DKbase.Models.SAP_RES_CRED_DISP parameter = new DKbase.Models.SAP_RES_CRED_DISP() { CLIENTE = convertSAPformat_CLIENTE(pIdCliente) };
            HttpResponseMessage response = await PostAsync(url_SAP, name, parameter);
            if (response != null)
            {
                var resultResponse = response.Content.ReadAsStringAsync().Result;
                if (isNotNull(resultResponse))
                {
                    try
                    {
                        SAP_REQ_CRES_DISP oResponse = JsonSerializer.Deserialize<SAP_REQ_CRES_DISP>(resultResponse);
                        result = convertSAPformat_Double(oResponse.CREDITO_DISP);
                    }
                    catch (Exception ex)
                    {
                        DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, name, pIdCliente);
                    }
                }
            }
            return result;
        }
    }
}