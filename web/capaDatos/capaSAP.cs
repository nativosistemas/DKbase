using DKbase.dll;
using DKbase.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Dynamic;
using DKbase.web.capaDatos;
using System.Data;
using DKbase.generales;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace DKbase //namespace DKbase.web.capaDatos
{
    public class capaSAP
    {
        private static string msgRealizandoTareasMantenimiento = "En este momento estamos realizando tareas de mantenimiento, por favor intente más tarde.";
        private static string msgErrorAlRecuperarCreditoDisponible = "Error al recuperar crédito disponible.";
        private static string msgErrorGenerico = "Se produjo un error. Código interno: 01";
        private static string msgNoHayProductosAProcesar = "No hay producto para procesar o el crédito disponible no es suficiente.";
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
        public static decimal convertSAPformat_Decimal(string pValue)
        {
            return decimal.Parse(pValue, culture_enUS);
        }
        public static async Task<decimal?> CRED_DISP(int pIdCliente)
        {
            decimal? result = null;
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
                        result = convertSAPformat_Decimal(oResponse.CREDITO_DISP);
                    }
                    catch (Exception ex)
                    {
                        DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, name, pIdCliente, resultResponse);
                    }
                }
            }
            return result;
        }
        public static cCarrito GetCarrito(DKbase.web.Usuario pUsuario, DKbase.web.capaDatos.cClientes pCliente, string pTipo, string pCodSucursal)
        {
            DataSet dsProductoCarrito = new DataSet();
            dsProductoCarrito = capaCAR_base.GetCarrito(pCliente.cli_codigo, pTipo, pCodSucursal);
            if (dsProductoCarrito != null)
            {
                List<cCarrito> listaSucursal = (from item in dsProductoCarrito.Tables[1].AsEnumerable()
                                                select new cCarrito { car_id = item.Field<int>("car_id"), lrc_id = item.Field<int>("car_id"), codSucursal = item.Field<string>("car_codSucursal") }).ToList();
                foreach (cCarrito item in listaSucursal)
                {
                    item.tipo = pTipo;
                    List<cProductosGenerico> listaProductoCarrtios = new List<cProductosGenerico>();
                    foreach (DataRow itemProductoCarrtio in dsProductoCarrito.Tables[0].Select("cad_codCarrito = " + item.lrc_id))
                    {
                        cProductos objProducto = DKbase.web.acceso.ConvertToProductos(itemProductoCarrtio);
                        cProductosGenerico objProductosGenerico = new cProductosGenerico(objProducto);
                        objProductosGenerico.tipoCarrito = pTipo;
                        if (itemProductoCarrtio.Table.Columns.Contains("stk_stock") && itemProductoCarrtio["stk_stock"] != DBNull.Value)
                            objProductosGenerico.stk_stock = itemProductoCarrtio["stk_stock"].ToString();
                        if (itemProductoCarrtio.Table.Columns.Contains("cad_codProducto") && itemProductoCarrtio["cad_codProducto"] != DBNull.Value)
                            objProductosGenerico.codProducto = Convert.ToInt32(itemProductoCarrtio["cad_codProducto"]);
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
                        listaProductoCarrtios[iPrecioFinal].PrecioFinal = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinal(pCliente, listaProductoCarrtios[iPrecioFinal]);
                        /// Nuevo
                        listaProductoCarrtios[iPrecioFinal].isProductoFacturacionDirecta = false;
                        if (listaTransferDetalle != null)
                        {
                            List<cTransferDetalle> listaAUXtransferDetalle = listaTransferDetalle.Where(x => x.tde_codpro == listaProductoCarrtios[iPrecioFinal].pro_codigo).ToList();
                            if (listaAUXtransferDetalle.Count > 0)
                            {
                                listaProductoCarrtios[iPrecioFinal].isProductoFacturacionDirecta = true;
                                listaProductoCarrtios[iPrecioFinal].CargarTransferYTransferDetalle(listaAUXtransferDetalle[0]);
                                listaProductoCarrtios[iPrecioFinal].PrecioFinalTransfer = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(pCliente, listaProductoCarrtios[iPrecioFinal].tfr_deshab, listaProductoCarrtios[iPrecioFinal].tfr_pordesadi, listaProductoCarrtios[iPrecioFinal].pro_neto, listaProductoCarrtios[iPrecioFinal].pro_codtpopro, listaProductoCarrtios[iPrecioFinal].pro_descuentoweb, listaProductoCarrtios[iPrecioFinal].tde_PrecioConDescuentoDirecto, listaProductoCarrtios[iPrecioFinal].tde_PorcARestarDelDtoDeCliente);
                            }
                        }
                        /// FIN Nuevo
                    }
                    item.listaProductos = listaProductoCarrtios;
                }
                listaSucursal.RemoveAll(x => x.listaProductos.Count == 0);
                return listaSucursal.FirstOrDefault();
            }
            return null;
        }
        public static async Task<TomarPedidoResponse> TomarPedidoCarrito(DKbase.web.Usuario pUsuario, DKbase.web.capaDatos.cClientes pCliente, string pTipo, string pCodSucursal)
        {
            TomarPedidoResponse result = new TomarPedidoResponse();
            result.tipo = Constantes.cTomarPedido_type_noSeProcesoMostrarMsg;
            result.msg = msgErrorGenerico;
            if (!capaCAR_WebService_base.IsBanderaCodigo(DKbase.generales.Constantes.cBAN_servidorSAP))
            {
                result.tipo = Constantes.cTomarPedido_type_noSeProcesoMostrarMsg;
                result.msg = msgRealizandoTareasMantenimiento;
            }
            else
            {
                decimal? creditoDisponible = await CRED_DISP(pCliente.cli_codigo);
                if (creditoDisponible == null)
                {
                    result.tipo = Constantes.cTomarPedido_type_noSeProcesoMostrarMsg;
                    result.msg = msgErrorAlRecuperarCreditoDisponible;
                }
                else
                {
                    result.tipo = Constantes.cTomarPedido_type_SeProceso;
                    result.msg = string.Empty;
                    cCarrito oCarrito = GetCarrito(pUsuario, pCliente, pTipo, pCodSucursal);
                    if (oCarrito.listaProductos != null)
                    {
                        List<cProductosGenerico> l_Procesar = new List<cProductosGenerico>();
                        List<cProductosGenerico> l_sin_Procesar = new List<cProductosGenerico>();
                        decimal sumaTotal_sap = 0;
                        foreach (cProductosGenerico o in oCarrito.listaProductos)
                        {
                            if (creditoDisponible.Value < (sumaTotal_sap + o.PrecioFinal_MasCantidad))
                            {
                                sumaTotal_sap += o.PrecioFinal_MasCantidad;
                                l_Procesar.Add(o);
                            }
                            else
                            {
                                l_sin_Procesar.Add(o);
                            }
                        }
                        if (l_Procesar.Count == 0)
                        {
                            result.tipo = Constantes.cTomarPedido_type_noSeProcesoMostrarMsg;
                            result.msg = msgNoHayProductosAProcesar;
                        }
                    }


                }
            }

            return result;
        }
    }
}