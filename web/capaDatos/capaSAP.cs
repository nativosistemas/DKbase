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
using System.Xml.Linq;
using DKbase.Entities;
using System.Data.SqlClient;

namespace DKbase //namespace DKbase.web.capaDatos
{
    public class capaSAP
    {
        private static string msgRealizandoTareasMantenimiento = "En este momento estamos realizando tareas de mantenimiento, por favor intente más tarde.";
        private static string msgErrorAlRecuperarCreditoDisponible = "Error al recuperar crédito disponible.";
        private static string msgErrorGenerico = "Se produjo un error. Código interno: 01";
        private static string msgNoHayProductosAProcesar = "No hay producto para procesar o el crédito disponible no es suficiente.";
        private static string msgCarritoRepetido = "Carrito ya se encuentra facturado.";
        private static string msgCarritoEnProceso = "Carrito se está procesando.";
        private static string msgError_carritoNoSeEncontro = "Carrito no encontrado.";
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
        public static Dictionary<string, int> dictionarySucursal = new Dictionary<string, int>()
        {
            { "CC",1001},
            {"SF",1002},
            {"CH", 1003},
            {"VH", 1004},
            {"CB", 2001},
            {"RC", 2002},
            {"VM", 2003},
            {"CO", 3001},
            {"CD", 3002},
            {"SN", 4001}
        };
        public static int convertSAPformat_SUCURSAL(string pValue)
        {
            int result = 0;
            if (dictionarySucursal.ContainsKey(pValue))
            {
                result = dictionarySucursal[pValue];
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
        //

        public static object convertSAPformat_TomarPedido(cCarrito pCarrito, cClientes pCliente)
        {
            //   pCarrito.listaProductos
            // Crear el objeto JSON dinámico
            dynamic jsonObject = new
            {
                S_LEGADOS_WEB_IN = new
                {
                    CABECERA = new
                    {
                        ID_CARRITO = pCarrito.car_id.ToString(),
                        SUCURSAL = convertSAPformat_SUCURSAL(pCarrito.codSucursal).ToString(),
                        FECHA_CREACION = DateTime.Now.ToString("dd/MM/yyyy"),
                        CLIENTE = pCliente.cli_codigo.ToString(),
                        WS_PEDIDO = pCarrito.car_id.ToString(),
                        PEDIDO_VALE = "",  // [va así]
                        RETIRA_MOSTRADOR = "",//   [va así]
                        CENTRO = convertSAPformat_SUCURSAL(pCarrito.codSucursal).ToString(),///  [idem Sucursal]
                        IDVENDEDOR = "",    //[estp se va a usar para la APP, para la web no, pasa asi]
                        COND_EXPEDICION = "01",//[01 (Reparto) / 02 (Mostrador) / 03 Cadetería / 04(Encomienda)]

                    },
                    POSICION = convertSAPformat_TomarPedido_detalle(pCarrito, pCliente),
                }
            };

            //var json = Serializador_base.SerializarAJson(jsonObject);
            //string jsonString = JsonSerializer.Serialize<dynamic>(jsonObject);

            return jsonObject;
        }
        public static object convertSAPformat_TomarPedido_detalle(cCarrito pCarrito, cClientes pCliente)
        {
            var result = new List<dynamic>();
            int cont = 0;
            if (pCarrito.listaProductos != null)
            {
                foreach (cProductosGenerico obj in pCarrito.listaProductos.Where(x => x.cantidad > 0))
                {
                    cont++;
                    result.Add(new
                    {
                        item = new
                        {
                            ID_CARRITO = pCarrito.car_id.ToString(),
                            ID_POSICION = cont.ToString(),//"01",
                            MATERIAL = obj.pro_codigo.ToString(),
                            CANTIDAD = obj.cantidad.ToString(),
                            ACUERDO = "",   // [se va a usar para las promociones, en el pedido habitual va vacío]
                            COMBO = "",  //  [va vacío en el pedido habitual]
                            //POSICION_PEDIDO = 0,//  [posición en la web, si querés pasar algo distinto a posición]
                            REGALO = "",//  [vacío]

                        }
                    });
                }
            }
            return result;
        }
        //
        public static async Task<decimal?> PEDIDOS_LEGADO_WEB(cCarrito pCarrito, cClientes pCliente)
        {
            decimal? result = null;
            try
            {
                string name = "Z_SD_PEDIDOS_LEGADO_WEB";
                dynamic parameter = convertSAPformat_TomarPedido(pCarrito, pCliente);
                HttpResponseMessage response = await PostAsync(url_SAP, name, parameter);
                if (response != null)
                {
                    var resultResponse = response.Content.ReadAsStringAsync().Result;
                    if (isNotNull(resultResponse))
                    {
                        try
                        {
                            // SAP_REQ_CRES_DISP oResponse = JsonSerializer.Deserialize<SAP_REQ_CRES_DISP>(resultResponse);
                            result = 10;// convertSAPformat_Decimal(oResponse.CREDITO_DISP);
                        }
                        catch (Exception ex)
                        {
                            DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, name, pCarrito, pCliente, resultResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now,pCarrito, pCliente);
            }
            return result;
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
            try
            {
                if (!capaCAR_WebService_base.IsBanderaCodigo(DKbase.generales.Constantes.cBAN_servidorSAP))
                {
                    result.tipo = Constantes.cTomarPedido_type_noSeProcesoMostrarMsg;
                    result.msg = msgRealizandoTareasMantenimiento;
                }
                else
                {
                    decimal? creditoDisponible = 10000000;// await CRED_DISP(pCliente.cli_codigo);
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
                        if (oCarrito == null || oCarrito.listaProductos == null)
                        {
                            result.tipo = Constantes.cTomarPedido_type_noSeProcesoMostrarMsg;
                            result.msg = msgError_carritoNoSeEncontro;
                        }
                        else //if (oCarrito.listaProductos != null)
                        {
                            List<cProductosGenerico> l_Procesar = new List<cProductosGenerico>();
                            List<cProductosGenerico> l_sin_Procesar_ProblemasDeCreditos = new List<cProductosGenerico>();
                            decimal sumaTotal_sap = 0;
                            foreach (cProductosGenerico o in oCarrito.listaProductos)
                            {
                                if (creditoDisponible.Value >= (sumaTotal_sap + o.PrecioFinal_MasCantidad))
                                {
                                    sumaTotal_sap += o.PrecioFinal_MasCantidad;
                                    //o.cad_id = cad_id
                                    l_Procesar.Add(o);
                                }
                                else
                                {
                                    l_sin_Procesar_ProblemasDeCreditos.Add(o);
                                }
                            }
                            if (l_Procesar.Count == 0)
                            {
                                result.tipo = Constantes.cTomarPedido_type_noSeProcesoMostrarMsg;
                                result.msg = msgNoHayProductosAProcesar;
                            }
                            else
                            {
                                //if (Helper.isSAP)           {
                                var ddd = convertSAPformat_TomarPedido(oCarrito, pCliente);
                                result = TomarPedidoCarrito_sap(oCarrito, l_Procesar, l_sin_Procesar_ProblemasDeCreditos, pUsuario, pCliente, pTipo, pCodSucursal);


                                /* }
                                 else
                                 {
                                     result.tipo = Constantes.cTomarPedido_type_SeProceso_dll;
                                     // inicio dll
                                     string pTipoEnvio = string.Empty;
                                     string horarioCierre = string.Empty;
                                     List<cCarrito> l_Carrito = new List<cCarrito>();
                                     l_Carrito.Add(oCarrito);
                                     result.result_dll = DKbase.web.capaDatos.capaCAR_WebService_base.TomarPedidoCarrito_generico(pUsuario, pCliente, l_Carrito, horarioCierre, pTipo, pCodSucursal, "", "", pTipoEnvio, false);
                                     // fin dll
                                 }*/

                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pUsuario, pCliente, pTipo, pCodSucursal);
            }
            return result;
        }

        public static bool recuperador_ProblemasDeCreditos(cClientes pCliente, DKbase.web.Usuario pUsuario, cCarrito pCarrito, int pTipo, string pCodSucursal, List<cProductosGenerico> pItemsConProblemasDeCreditos)
        {
            bool result = true;
            try
            {
                if (pItemsConProblemasDeCreditos != null)
                {
                    string strXML = string.Empty;
                    strXML += "<Root>";
                    foreach (cProductosGenerico item in pItemsConProblemasDeCreditos)
                    {
                        List<XAttribute> listaAtributos = new List<XAttribute>();
                        listaAtributos.Add(new XAttribute("cantidad", item.cantidad));
                        listaAtributos.Add(new XAttribute("codProducto", item.codProducto));
                        listaAtributos.Add(new XAttribute("codTransfers", item.tfr_codigo));
                        listaAtributos.Add(new XAttribute("tipo", pTipo));

                        XElement nodo = new XElement("DetallePedido", listaAtributos);
                        strXML += nodo.ToString();
                    }

                    strXML += "</Root>";


                    BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
                    List<SqlParameter> l = new List<SqlParameter>();
                    l.Add(db.GetParameter("fpc_codCarrito", pCarrito.car_id));
                    l.Add(db.GetParameter("fpc_CarritoTipo", pCarrito.tipo));
                    l.Add(db.GetParameter("fpc_codSucursal", pCodSucursal));
                    l.Add(db.GetParameter("fpc_codUsuario", pUsuario.id));
                    l.Add(db.GetParameter("fpc_codCliente", pCliente.cli_codigo));
                    //l.Add(db.GetParameter("fpc_tipo",pTipo));
                    l.Add(db.GetParameter("strXML", strXML, SqlDbType.Xml));
                    SqlParameter ParameterOut_isOk = db.GetParameterOut("isOk", SqlDbType.Int);
                    l.Add(ParameterOut_isOk);
                    db.ExecuteNonQuery("CAR.spFaltasProblemasCrediticios", l);
                    if (ParameterOut_isOk.Value != DBNull.Value)
                    {
                        result = Convert.ToBoolean(ParameterOut_isOk.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pCliente, pUsuario, pCarrito, pTipo, pCodSucursal, pItemsConProblemasDeCreditos);
            }
            return result;
        }
        public static List<cFaltantesConProblemasCrediticiosPadre> RecuperarFaltasProblemasCrediticios_TodosEstados(cClientes pCliente, int fpc_tipo, int pCantidadDia, string pSucursal)
        {
            List<cFaltantesConProblemasCrediticiosPadre> resultado = null;
            try
            {
                //DataSet dsResultado = capaLogRegistro_base.RecuperarFaltasProblemasCrediticios_TodosEstados(pCliente.cli_codigo, fpc_tipo, pCantidadDia, pSucursal);
                //
                BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(db.GetParameter("fpc_tipo", fpc_tipo));
                l.Add(db.GetParameter("cantidadDia", pCantidadDia));
                l.Add(db.GetParameter("Sucursal", pSucursal));
                l.Add(db.GetParameter("fpc_codCliente", pCliente.cli_codigo));
                DataSet dsResultado = db.GetDataSet("CAR.spRecuperarFaltasProblemasCrediticiosTodosEstados", l);
                //
                List<cTransferDetalle> listaTransferDetalle = new List<cTransferDetalle>();
                DataTable tablaTransferDetalle = dsResultado.Tables[1];
                foreach (DataRow itemTransferDetalle in tablaTransferDetalle.Rows)
                {
                    cTransferDetalle objTransferDetalle = DKbase.web.acceso.ConvertToTransferDetalle(itemTransferDetalle);
                    objTransferDetalle.CargarTransfer(DKbase.web.acceso.ConvertToTransfer(itemTransferDetalle));
                    listaTransferDetalle.Add(objTransferDetalle);
                }
                resultado = DKbase.Util.ConvertDataTableAClase(dsResultado.Tables[0], dsResultado.Tables[2], listaTransferDetalle, pCliente);
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pCliente, fpc_tipo, pCantidadDia, pSucursal);
            }
            return resultado;
        }
        public static bool BorrarPorProductosFaltasProblemasCrediticios(cClientes pCliente, DKbase.web.Usuario pUsuario, int fpc_tipo, string pSucursal, string[] pArrayCodigoProducto)
        {
            bool result = false;
            try
            {
                string strXML = string.Empty;
                strXML += "<Root>";
                foreach (string item in pArrayCodigoProducto)
                {
                    List<XAttribute> listaAtributos = new List<XAttribute>();
                    listaAtributos.Add(new XAttribute("codProducto", Convert.ToDouble(item)));
                    XElement nodo = new XElement("DetallePedido", listaAtributos);
                    strXML += nodo.ToString();
                }

                strXML += "</Root>";
                //
                BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(db.GetParameter("fpc_tipo", fpc_tipo));
                l.Add(db.GetParameter("fpc_codSucursal", pSucursal));//fpc_codSucursal
                l.Add(db.GetParameter("fpc_codCliente", pCliente.cli_codigo));
                l.Add(db.GetParameter("strXML", strXML, SqlDbType.Xml));
                DataSet dsResultado = db.GetDataSet("CAR.spBorrarPorProductosFaltasProblemasCrediticios", l);
                //
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pCliente, fpc_tipo, pSucursal, pArrayCodigoProducto);
            }
            return result;
        }
        public static bool BorrarPorProductosFaltasProblemasCrediticios_Carrito(cClientes pCliente, DKbase.web.Usuario pUsuario, int fpc_tipo, string pSucursal, List<cProductosAndCantidad> pListaProducto, int pCantidadDia)
        {//(List<cProductosAndCantidad> pListaProducto, string fpc_codSucursal, int fpc_codCliente, int fpc_tipo, int pCantidadDia)
            bool result = false;
            try
            {
                DataTable pTablaDetalle = web.FuncionesPersonalizadas_base.ObtenerDataTableProductosCarritoArchivosPedidos();
                if (pListaProducto.Count > 0)
                {
                    foreach (cProductosAndCantidad itemProductosAndCantidad in pListaProducto)
                    {
                        DataRow fila = pTablaDetalle.NewRow();
                        fila["codProducto"] = itemProductosAndCantidad.codProducto;
                        fila["cantidad"] = itemProductosAndCantidad.cantidad;
                        pTablaDetalle.Rows.Add(fila);
                    }
                }
                BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(db.GetParameter("fpc_tipo", fpc_tipo));
                l.Add(db.GetParameter("fpc_codSucursal", pSucursal));
                l.Add(db.GetParameter("fpc_codCliente", pCliente.cli_codigo));
                l.Add(db.GetParameter("cantidadDia", pCantidadDia));
                l.Add(db.GetParameter("Tabla_Detalle", pTablaDetalle));
                DataSet dsResultado = db.GetDataSet("CAR.spBorrarPorProductosFaltasProblemasCrediticios_Carrito", l);
                //
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pCliente, pUsuario, fpc_tipo, pSucursal, pListaProducto, pCantidadDia);
            }
            return result;
        }
        public static List<cFaltantesConProblemasCrediticiosPadre> RecuperarFaltasProblemasCrediticios(cClientes pCliente, int fpc_tipo, int pCantidadDia, string pSucursal)
        {
            List<cFaltantesConProblemasCrediticiosPadre> resultado = null;
            try
            {
                //DataSet dsResultado = capaLogRegistro_base.RecuperarFaltasProblemasCrediticios(pCliente.cli_codigo, fpc_tipo, pCantidadDia, pSucursal);
                //
                BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(db.GetParameter("fpc_tipo", fpc_tipo));
                l.Add(db.GetParameter("cantidadDia", pCantidadDia));
                l.Add(db.GetParameter("Sucursal", pSucursal));
                l.Add(db.GetParameter("fpc_codCliente", pCliente.cli_codigo));
                DataSet dsResultado = db.GetDataSet("CAR.spRecuperarFaltasProblemasCrediticios", l);
                //
                List<cTransferDetalle> listaTransferDetalle = new List<cTransferDetalle>();
                DataTable tablaTransferDetalle = dsResultado.Tables[1];
                foreach (DataRow itemTransferDetalle in tablaTransferDetalle.Rows)
                {
                    cTransferDetalle objTransferDetalle = DKbase.web.acceso.ConvertToTransferDetalle(itemTransferDetalle);
                    objTransferDetalle.CargarTransfer(DKbase.web.acceso.ConvertToTransfer(itemTransferDetalle));
                    listaTransferDetalle.Add(objTransferDetalle);
                }

                resultado = DKbase.Util.ConvertDataTableAClase(dsResultado.Tables[0], dsResultado.Tables[2], listaTransferDetalle, pCliente);
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pCliente, fpc_tipo, pCantidadDia, pSucursal);
            }
            return resultado;
        }
        public static TomarPedidoResponse TomarPedidoCarrito_sap(cCarrito pCarrito, List<cProductosGenerico> pL_Procesar, List<cProductosGenerico> pL_ItemsConProblemasDeCreditos, DKbase.web.Usuario pUsuario, DKbase.web.capaDatos.cClientes pCliente, string pTipo, string pCodSucursal)
        {
            TomarPedidoResponse result = new TomarPedidoResponse();
            if (pCarrito == null)
                return null;
            try
            {
                if (capaCAR_base.IsCarritoEnProceso(pCarrito.car_id))
                {
                    result.tipo = Constantes.cTomarPedido_type_SeProcesoMostrarMsg;
                    result.msg = msgCarritoEnProceso;
                }
                capaCAR_base.InicioCarritoEnProceso(pCarrito.car_id, Constantes.cAccionCarrito_TOMAR);

                // ACA va logica cuando se llama a toma pedido sap
                result.tipo = Constantes.cTomarPedido_type_SeProceso;
                result.msg = "Ok";

                int tbl_tomarPedido = spTomarPedido(pCarrito, pL_Procesar, pUsuario, pCliente, pTipo, pCodSucursal);

                spTomarPedidoUpdate(tbl_tomarPedido, pUsuario, Constantes.cTomarPedido_type_LlegoRespuestaSAP, "respuesta sap");

                // Fin llamada sap
                if (pL_ItemsConProblemasDeCreditos.Count > 0)
                {
                    recuperador_ProblemasDeCreditos(pCliente, pUsuario, pCarrito, Constantes.cPEDIDO_PROBLEMACREDITICIO, pCodSucursal, pL_ItemsConProblemasDeCreditos);
                }
                capaCAR_base.GuardarPedidoBorrarCarrito(pUsuario, pCliente, pCarrito, pTipo, "", "", "", false);

            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pCarrito, pUsuario, pCliente, pTipo, pCodSucursal);
            }
            return result;
        }

        public static int spTomarPedidoUpdate(int pTpc_id, DKbase.web.Usuario pUsuario, string pStatus, string pResultJson)
        {
            int result = 0;
            try
            {
                BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(db.GetParameter("tpc_id", pTpc_id));
                l.Add(db.GetParameter("tpc_codUsuario", pUsuario.id));
                l.Add(db.GetParameter("tpc_status", Constantes.cTomarPedido_type_LlegoRespuestaSAP));
                l.Add(db.GetParameter("tpc_resultResponseContent", pResultJson));
                db.ExecuteNonQuery("CAR.spTomarPedidoUpdate", l);

            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pTpc_id, pUsuario, pStatus, pResultJson);
            }
            return result;

        }
        public static int spTomarPedido(cCarrito pCarrito, List<cProductosGenerico> pL_Procesar, DKbase.web.Usuario pUsuario, DKbase.web.capaDatos.cClientes pCliente, string pTipo, string pCodSucursal)
        {
            int result = 0;
            try
            {
                string strXML = string.Empty;
                strXML += "<Root>";
                foreach (cProductosGenerico item in pL_Procesar)
                {
                    List<XAttribute> listaAtributos = new List<XAttribute>();
                    listaAtributos.Add(new XAttribute("tpd_cantidad", item.cantidad));
                    listaAtributos.Add(new XAttribute("tpd_codProducto", item.codProducto));
                    listaAtributos.Add(new XAttribute("tpd_codTransfers", item.tfr_codigo));
                    listaAtributos.Add(new XAttribute("tpd_codUsuario", pUsuario.id));
                    listaAtributos.Add(new XAttribute("tpd_status", Constantes.cTomarPedido_type_SeEnvioSAP));
                    listaAtributos.Add(new XAttribute("tpd_codCarritosDetalles", 0));

                    XElement nodo = new XElement("DetallePedido", listaAtributos);
                    strXML += nodo.ToString();
                }
                strXML += "</Root>";
                //return capaModulo.spAddPedido(pPedido.promotor, strXML, pConnectionStringSQL);




                BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(db.GetParameter("tpc_codCarrito", pCarrito.car_id));
                l.Add(db.GetParameter("tpc_CarritoTipo", pCarrito.tipo));
                l.Add(db.GetParameter("tpc_codSucursal", pCodSucursal));
                l.Add(db.GetParameter("tpc_codUsuario", pUsuario.id));
                l.Add(db.GetParameter("tpc_codCliente", pCliente.cli_codigo));
                l.Add(db.GetParameter("tpc_status", Constantes.cTomarPedido_type_SeEnvioSAP));
                l.Add(db.GetParameter("strXML", strXML, SqlDbType.Xml));
                SqlParameter ParameterOut_tpc_id = db.GetParameterOut("tpc_id", SqlDbType.Int);
                l.Add(ParameterOut_tpc_id);
                db.ExecuteNonQuery("CAR.spTomarPedido", l);
                if (ParameterOut_tpc_id.Value != DBNull.Value)
                {
                    result = Convert.ToInt32(ParameterOut_tpc_id.Value);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pCarrito, pL_Procesar, pUsuario, pCliente, pTipo, pCodSucursal);
            }
            return result;

        }
    }
}
