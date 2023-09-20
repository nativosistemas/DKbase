using DKbase.dll;
using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace DKbase.web.capaDatos
{
    public class capaCAR_WebService_base
    {
        private static string msgCarritoRepetido = "Carrito ya se encuentra facturado.";
        private static string msgCarritoEnProceso = "Carrito se está procesando.";
        private static string msgValidarExistenciaDeCarritoWebPasado = "La función ValidarExistenciaDeCarritoWebPasado no reconoce el idCarrito procesado.";
        private static string msgRealizandoTareasMantenimiento = "En este momento estamos realizando tareas de mantenimiento, por favor intente más tarde.";
        private static string msgSeProdujoUnError = "Se produjo un error.";
        private static string msgSeProdujoUnError_ValidarExistenciaDeCarritoWebPasado = "Se produjo un error. Código interno: 01";
        public static List<cCarrito> RecuperarCarritosPorSucursalYProductos_generica(cClientes objClientes, string pTipo)
        {
            DataSet dsProductoCarrito = new DataSet();

            dsProductoCarrito = capaCAR_base.RecuperarCarritosPorSucursalYProductos_generica(objClientes.cli_codigo, pTipo);

            if (dsProductoCarrito != null)
            {
                List<cCarrito> listaSucursal = (from item in dsProductoCarrito.Tables[1].AsEnumerable()
                                                select new cCarrito { car_id = item.Field<int>("car_id"), lrc_id = item.Field<int>("car_id"), codSucursal = item.Field<string>("car_codSucursal") }).ToList();

                foreach (cCarrito item in listaSucursal)
                {
                    //item.proximoHorarioEntrega = DKbase.web.FuncionesPersonalizadas_base.ObtenerHorarioCierre(objClientes, objClientes.cli_codsuc, item.codSucursal, objClientes.cli_codrep);
                    List<cProductosGenerico> listaProductoCarrtios = new List<cProductosGenerico>();
                    foreach (DataRow itemProductoCarrtio in dsProductoCarrito.Tables[0].Select("cad_codCarrito = " + item.lrc_id))
                    {
                        cProductos objProducto = DKbase.web.acceso.ConvertToProductos(itemProductoCarrtio);
                        cProductosGenerico objProductosGenerico = new cProductosGenerico(objProducto);
                        if (itemProductoCarrtio.Table.Columns.Contains("stk_stock") && itemProductoCarrtio["stk_stock"] != DBNull.Value)
                            objProductosGenerico.stk_stock = itemProductoCarrtio["stk_stock"].ToString();
                        if (itemProductoCarrtio.Table.Columns.Contains("cad_codProducto") && itemProductoCarrtio["cad_codProducto"] != DBNull.Value)
                            objProductosGenerico.codProducto = Convert.ToInt32( itemProductoCarrtio["cad_codProducto"]);
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
                        listaProductoCarrtios[iPrecioFinal].PrecioFinal = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinal(objClientes, listaProductoCarrtios[iPrecioFinal]);
                        /// Nuevo
                        listaProductoCarrtios[iPrecioFinal].isProductoFacturacionDirecta = false;
                        if (listaTransferDetalle != null)
                        {
                            List<cTransferDetalle> listaAUXtransferDetalle = listaTransferDetalle.Where(x => x.tde_codpro == listaProductoCarrtios[iPrecioFinal].pro_codigo).ToList();
                            if (listaAUXtransferDetalle.Count > 0)
                            {
                                listaProductoCarrtios[iPrecioFinal].isProductoFacturacionDirecta = true;
                                listaProductoCarrtios[iPrecioFinal].CargarTransferYTransferDetalle(listaAUXtransferDetalle[0]);
                                listaProductoCarrtios[iPrecioFinal].PrecioFinalTransfer = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(objClientes, listaProductoCarrtios[iPrecioFinal].tfr_deshab, listaProductoCarrtios[iPrecioFinal].tfr_pordesadi, listaProductoCarrtios[iPrecioFinal].pro_neto, listaProductoCarrtios[iPrecioFinal].pro_codtpopro, listaProductoCarrtios[iPrecioFinal].pro_descuentoweb, listaProductoCarrtios[iPrecioFinal].tde_PrecioConDescuentoDirecto, listaProductoCarrtios[iPrecioFinal].tde_PorcARestarDelDtoDeCliente);
                            }
                        }
                        /// FIN Nuevo
                    }
                    item.listaProductos = listaProductoCarrtios;
                }
                listaSucursal.RemoveAll(x => x.listaProductos.Count == 0);
                return listaSucursal;
            }
            return null;
        }
        public static List<cSucursalCarritoTransfer> RecuperarCarritosTransferPorIdClienteOrdenadosPorSucursal(cClientes pCliente, string pTipo)
        {
            DataSet dsProductoCarrito = capaCAR_base.RecuperarCarritoTransferPorIdCliente(pCliente.cli_codigo, pTipo);
            return acceso.convertDataSetToSucursalCarritoTransfer(pCliente, dsProductoCarrito);
        }
        public static List<cProductosGenerico> ConvertToProductoBuscadorV5(cClientes pCliente, DataSet DataSetResultado, string pSucursalElejida)
        {
            List<cProductosGenerico> resultado = null;
            if (DataSetResultado != null)
            {
                if (DataSetResultado.Tables.Count > 1)
                {
                    DataTable tablaProductos = DataSetResultado.Tables[0];
                    DataTable tablaSucursalStocks = DataSetResultado.Tables[1];
                    DataTable tablaProductosNoEncontrado = DataSetResultado.Tables[2];
                    DataTable tablaTransferDetalle = DataSetResultado.Tables[3];
                    List<cTransferDetalle> listaTransferDetalle = new List<cTransferDetalle>();
                    foreach (DataRow itemTransferDetalle in tablaTransferDetalle.Rows)
                    {
                        cTransferDetalle objTransferDetalle = DKbase.web.acceso.ConvertToTransferDetalle(itemTransferDetalle);
                        objTransferDetalle.CargarTransfer(DKbase.web.acceso.ConvertToTransfer(itemTransferDetalle));
                        listaTransferDetalle.Add(objTransferDetalle);
                    }
                    resultado = DKbase.web.acceso.cargarProductosBuscadorArchivos(pCliente, tablaProductos, tablaSucursalStocks, listaTransferDetalle, DKbase.generales.Constantes.CargarProductosBuscador.isSubirArchivo, pSucursalElejida);
                    //////////////
                    foreach (DataRow item in tablaProductosNoEncontrado.Rows)
                    {
                        if (item["nombreNoEncontrado"] != DBNull.Value || item["codigobarra"] != DBNull.Value)
                        {
                            cProductosGenerico obj = new cProductosGenerico();
                            obj.isProductoNoEncontrado = true;
                            obj.pro_codigo = -1;
                            if (item["nombreNoEncontrado"] != DBNull.Value)
                            {
                                obj.pro_nombre = "<b> Código producto: </b>" + item["nombreNoEncontrado"].ToString();
                            }
                            string nombreVer = string.Empty;
                            if (item.Table.Columns.Contains("codigobarra"))
                            {
                                if (item["codigobarra"] != DBNull.Value)
                                {
                                    if (item["codigobarra"].ToString() != string.Empty)
                                    {
                                        nombreVer += "<b> Código Barra: </b>" + Convert.ToString(item["codigobarra"]);
                                    }
                                }
                            }
                            if (item.Table.Columns.Contains("troquel"))
                            {
                                if (item["troquel"] != DBNull.Value)
                                {
                                    if (item["troquel"].ToString() != string.Empty)
                                    {
                                        nombreVer += "<b> Troquel: </b>" + Convert.ToString(item["troquel"]);
                                    }
                                }
                            }
                            if (item.Table.Columns.Contains("codigoalfabeta"))
                            {
                                if (item["codigoalfabeta"] != DBNull.Value)
                                {
                                    if (item["codigoalfabeta"].ToString() != string.Empty)
                                    {
                                        nombreVer += "<b> Código Alfabeta: </b>" + Convert.ToString(item["codigoalfabeta"]);
                                    }
                                }
                            }
                            if (nombreVer != string.Empty)
                            {
                                obj.pro_nombre = nombreVer;
                            }
                            if (item.Table.Columns.Contains("cantidad"))
                            {
                                if (item["cantidad"] != DBNull.Value)
                                {
                                    obj.cantidad = Convert.ToInt32(item["cantidad"]);
                                }
                            }
                            if (item.Table.Columns.Contains("nroordenamiento"))
                            {
                                if (item["nroordenamiento"] != DBNull.Value)
                                {
                                    obj.nroordenamiento = Convert.ToInt32(item["nroordenamiento"]);
                                }
                            }
                            resultado.Add(obj);
                        }

                        //}

                    }
                    //////////////
                }
            }
            return resultado;
        }
        public static List<cProductosGenerico> AgregarProductoAlCarritoDesdeArchivoPedidosV5(cClientes pCliente, string pSucursalElejida, string pSucursalCliente, DataTable pTabla, string pTipoDeArchivo, int pIdCliente, string pCli_codprov, bool pCli_isGLN, int? pIdUsuario)
        {
            List<cProductosGenerico> resultado = null;

            DataSet DataSetResultado = capaLogRegistro_base.AgregarProductoAlCarritoDesdeArchivoPedidosV5(pSucursalElejida, pSucursalCliente, pTabla, pTipoDeArchivo, pIdCliente, pCli_codprov, pCli_isGLN, pIdUsuario);
            resultado = DKbase.web.capaDatos.capaCAR_WebService_base.ConvertToProductoBuscadorV5(pCliente, DataSetResultado, pSucursalElejida);

            if (resultado != null)
            {
                // TIPO CLIENTE
                if (pCliente.cli_tipo == Constantes.cTipoCliente_Perfumeria) // Solamente perfumeria
                {
                    foreach (var item in resultado.Where(x => x.pro_codtpopro != Constantes.cTIPOPRODUCTO_Perfumeria && x.pro_codtpopro != Constantes.cTIPOPRODUCTO_PerfumeriaCuentaYOrden))
                    {
                        item.isPermitirPedirProducto = false;
                    }
                }
                else if (pCliente.cli_tipo == Constantes.cTipoCliente_Todos) // Todos los productos
                {
                    if (!pCliente.cli_tomaPerfumeria)
                    {
                        foreach (var item in resultado.Where(x => x.pro_codtpopro == Constantes.cTIPOPRODUCTO_Perfumeria || x.pro_codtpopro == Constantes.cTIPOPRODUCTO_PerfumeriaCuentaYOrden))
                        {
                            item.isPermitirPedirProducto = false;
                        }
                    }
                }
                // FIN TIPO CLIENTE
                resultado = resultado.OrderBy(x => x.nroordenamiento).ToList();
            }

            return resultado;
        }
        public static List<cProductosGenerico> RecuperarTodosProductosDesdeBuscador_OfertaTransfer(cClientes pCliente, string pSucursal, int? pIdCliente, bool pIsOferta, bool pIsTransfer, string pCli_codprov)
        {
            List<cProductosGenerico> resultado = null;
            DataSet dsResultado = null;
            if (pIsOferta)
            {
                dsResultado = capaProductos_base.RecuperarTodosProductosBuscadorEnOferta(pSucursal, pIdCliente, pCli_codprov);
            }
            else if (pIsTransfer)
            {
                dsResultado = capaProductos_base.RecuperarTodosProductosBuscadorEnTransfer(pSucursal, pIdCliente, pCli_codprov);
            }
            if (dsResultado != null)
            {
                DataTable tablaProductos = dsResultado.Tables[0];
                DataTable tablaSucursalStocks = dsResultado.Tables[1];
                List<cTransferDetalle> listaTransferDetalle = null;
                if (dsResultado.Tables.Count > 2)
                {
                    listaTransferDetalle = new List<cTransferDetalle>();
                    DataTable tablaTransferDetalle = dsResultado.Tables[2];
                    foreach (DataRow itemTransferDetalle in tablaTransferDetalle.Rows)
                    {
                        cTransferDetalle objTransferDetalle = DKbase.web.acceso.ConvertToTransferDetalle(itemTransferDetalle);
                        objTransferDetalle.CargarTransfer(DKbase.web.acceso.ConvertToTransfer(itemTransferDetalle));
                        listaTransferDetalle.Add(objTransferDetalle);
                    }
                }
                resultado = DKbase.web.acceso.cargarProductosBuscadorArchivos(pCliente, tablaProductos, tablaSucursalStocks, listaTransferDetalle, DKbase.generales.Constantes.CargarProductosBuscador.isDesdeBuscador_OfertaTransfer, null);
            }
            if (resultado != null && pIsTransfer)
                resultado = resultado.Where(x => x.isMostrarTransfersEnClientesPerf).OrderBy(x => x.pro_nombre).ToList();

            return resultado;
        }
        public static List<cSucursal> RecuperarTodasSucursales()
        {
            List<cSucursal> resultado = null;
            DataTable tabla = capaClientes_base.RecuperarTodasSucursales();
            if (tabla != null)
            {
                resultado = new List<cSucursal>();
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    resultado.Add(DKbase.web.acceso.ConvertToSucursal(tabla.Rows[i]));
                }

            }
            return resultado;
        }
        public static bool ActualizarProductoCarritoSubirArchivo(List<cProductosAndCantidad> pListaValor, int pIdCliente, int pIdUsuario)
        {
            string strXML = string.Empty;
            strXML += "<Root>";
            foreach (cProductosAndCantidad item in pListaValor)
            {
                List<XAttribute> listaAtributos = new List<XAttribute>();

                listaAtributos.Add(new XAttribute("lcp_cantidad", item.cantidad));
                listaAtributos.Add(new XAttribute("codigo", item.codProducto));
                listaAtributos.Add(new XAttribute("nombre", item.codProductoNombre));
                listaAtributos.Add(new XAttribute("codTransfer", item.tde_codtfr));
                listaAtributos.Add(new XAttribute("isTransferFacturacionDirecta", item.isTransferFacturacionDirecta));
                listaAtributos.Add(new XAttribute("codSucursal", item.codSucursal));
                XElement nodo = new XElement("DetallePedido", listaAtributos);
                strXML += nodo.ToString();
            }
            strXML += "</Root>";
            return capaCAR_base.SubirPedido(strXML, pIdCliente, pIdUsuario, Constantes.cTipo_Carrito, Constantes.cTipo_CarritoTransfers);
        }
        public static cDllPedido TomarPedidoCarrito_generico(Usuario pUsuario, cClientes pCliente, List<cCarrito> pListaCarrito, string pHorarioCierre, string pTipo, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, bool pIsUrgente)
        {
            cDllPedido resultadoPedido = null;
            if (pListaCarrito == null)
                return null;
            try
            {
                //if (!capaCAR_WebService_base.IsBanderaCodigo(Constantes.cBAN_SERVIDORDLL))
                //{
                //    cDllPedido oEnProceso = new cDllPedido();
                //    oEnProceso.Error = msgRealizandoTareasMantenimiento;
                //    return oEnProceso;
                //}
                foreach (cCarrito item in pListaCarrito)
                {
                    if (item.codSucursal == pIdSucursal)
                    {
                        if (capaCAR_base.IsCarritoEnProceso(item.car_id))
                        {
                            cDllPedido oEnProceso = new cDllPedido();
                            oEnProceso.Error = msgCarritoEnProceso;
                            return oEnProceso;
                        }
                        if (capaDLL.ValidarExistenciaDeCarritoWebPasado(item.car_id))
                        {
                            DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), msgCarritoRepetido + " idCarrito: " + item.car_id, DateTime.Now, item.car_id);
                            capaCAR_base.BorrarCarritoPorId_SleepTimer(item.car_id, Constantes.cAccionCarrito_BORRAR_CARRRITO_REPETIDO);
                            cDllPedido oRepetido = new cDllPedido();
                            oRepetido.Error = msgCarritoRepetido;
                            return oRepetido;
                        }
                        List<cDllProductosAndCantidad> listaProductos = new List<cDllProductosAndCantidad>();
                        foreach (cProductosGenerico itemProductos in item.listaProductos)
                        {
                            listaProductos.Add(FuncionesPersonalizadas_base.ProductosEnCarrito_ToConvert_DllProductosAndCantidad(itemProductos));
                        }
                        resultadoPedido = capaDLL.TomarPedidoConIdCarrito(item.car_id, pCliente.cli_login, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, listaProductos, pIsUrgente);
                        bool isValidarExistenciaDeCarritoWebPasado = capaDLL.ValidarExistenciaDeCarritoWebPasado(item.car_id);
                        if (!isValidarExistenciaDeCarritoWebPasado)
                        {
                            DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), msgValidarExistenciaDeCarritoWebPasado + " idCarrito: " + item.car_id, DateTime.Now, item.car_id);
                            cDllPedido oRepetido = new cDllPedido();
                            oRepetido.Error = msgSeProdujoUnError_ValidarExistenciaDeCarritoWebPasado;
                            return oRepetido;
                        }
                        if (resultadoPedido != null)
                        {
                            bool isErrorPedido = false;
                            if (!string.IsNullOrEmpty(resultadoPedido.Error) ||
                                !string.IsNullOrEmpty(resultadoPedido.web_Error))
                            {
                                isErrorPedido = true;
                            }

                            // Si se genero error
                            if (isErrorPedido)
                            {
                                resultadoPedido.Error = FuncionesPersonalizadas_base.LimpiarStringErrorPedido(resultadoPedido.Error);
                            }
                            else
                            {
                                // Obtener horario cierre
                                resultadoPedido.Login = pHorarioCierre;
                                // OPTIMIZAR //////////////////
                                if (resultadoPedido.Items != null)
                                {
                                    foreach (cDllPedidoItem itemFaltantes in resultadoPedido.Items)
                                    {
                                        if (itemFaltantes.Faltas > 0)
                                        {
                                            capaLogRegistro_base.InsertarFaltantesProblemasCrediticios(item.lrc_id, pIdSucursal, pCliente.cli_codigo, itemFaltantes.NombreObjetoComercial, itemFaltantes.Faltas, Constantes.cPEDIDO_FALTANTES);
                                        }
                                    }
                                }
                                if (resultadoPedido.ItemsConProblemasDeCreditos != null)
                                {
                                    foreach (cDllPedidoItem itemConProblemasDeCreditos in resultadoPedido.ItemsConProblemasDeCreditos)
                                    {
                                        int cantidadProblemaCrediticia = itemConProblemasDeCreditos.Cantidad + itemConProblemasDeCreditos.Faltas;
                                        if (cantidadProblemaCrediticia > 0)
                                        {
                                            capaLogRegistro_base.InsertarFaltantesProblemasCrediticios(item.lrc_id, pIdSucursal, pCliente.cli_codigo, itemConProblemasDeCreditos.NombreObjetoComercial, cantidadProblemaCrediticia, Constantes.cPEDIDO_PROBLEMACREDITICIO);
                                        }
                                    }
                                }

                                capaCAR_base.GuardarPedidoBorrarCarrito(pUsuario, pCliente, item, pTipo, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pIsUrgente);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pUsuario, pCliente, pListaCarrito, pHorarioCierre, pTipo, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, pIsUrgente);
            }
            return resultadoPedido;
        }
        public static List<cDllPedidoTransfer> TomarTransferPedidoCarrito(Usuario pUsuario, cClientes pCliente, List<cCarritoTransfer> pListaCarrito, bool pIsDiferido, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio)
        {
            string tipo = pIsDiferido ? Constantes.cTipo_CarritoDiferidoTransfers : Constantes.cTipo_CarritoTransfers;
            List<cDllPedidoTransfer> resultadoPedido = null;
            int car_id_aux = 0;
            List<cDllProductosAndCantidad> listaProductos = new List<cDllProductosAndCantidad>();
            if (pListaCarrito == null)
                return null;
            try
            {
                //if (!capaCAR_WebService_base.IsBanderaCodigo(Constantes.cBAN_SERVIDORDLL))
                //{
                //    cDllPedidoTransfer oEnProceso = new cDllPedidoTransfer();
                //    oEnProceso.Error = msgRealizandoTareasMantenimiento; ;
                //    resultadoPedido = new List<cDllPedidoTransfer>();
                //    resultadoPedido.Add(oEnProceso);
                //    return resultadoPedido;
                //}
                List<cProductosGenerico> listaProductos_Auditoria = new List<cProductosGenerico>();
                foreach (cCarritoTransfer item in pListaCarrito)
                {
                    if (item.ctr_codSucursal == pIdSucursal)
                    {
                        car_id_aux = item.car_id_aux;
                        foreach (cProductosGenerico itemProductos in item.listaProductos)
                        {
                            cDllProductosAndCantidad objProductos = FuncionesPersonalizadas_base.ProductosEnCarrito_ToConvert_DllProductosAndCantidad(itemProductos);
                            objProductos.IdTransfer = item.tfr_codigo;
                            listaProductos.Add(objProductos);
                            itemProductos.tfr_codigo = item.tfr_codigo;
                            itemProductos.tde_codtfr = item.tfr_codigo;
                            listaProductos_Auditoria.Add(itemProductos);

                        }
                    }
                }
                // OJO CON <<<< car_id_aux  >>>>
                if (capaCAR_base.IsCarritoEnProceso(car_id_aux))
                {
                    cDllPedidoTransfer oEnProceso = new cDllPedidoTransfer();
                    oEnProceso.Error = msgCarritoEnProceso;
                    resultadoPedido = new List<cDllPedidoTransfer>();
                    resultadoPedido.Add(oEnProceso);
                    return resultadoPedido;
                }
                if (capaDLL.ValidarExistenciaDeCarritoWebPasado(car_id_aux))
                {
                    DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), msgCarritoRepetido + " idCarrito: " + car_id_aux, DateTime.Now, car_id_aux);
                    capaCAR_base.BorrarCarritoPorId_SleepTimer(car_id_aux, Constantes.cAccionCarrito_BORRAR_CARRRITO_REPETIDO);
                    cDllPedidoTransfer oRepetido = new cDllPedidoTransfer();
                    oRepetido.Error = msgCarritoRepetido;
                    resultadoPedido = new List<cDllPedidoTransfer>();
                    resultadoPedido.Add(oRepetido);
                    return resultadoPedido;
                }
                List<cDllPedidoTransfer> listaCarritoAux = capaDLL.TomarPedidoDeTransfersConIdCarrito(car_id_aux, pCliente.cli_login, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, listaProductos);
                bool isValidarExistenciaDeCarritoWebPasado = capaDLL.ValidarExistenciaDeCarritoWebPasado(car_id_aux);
                if (!isValidarExistenciaDeCarritoWebPasado)
                {
                    DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), msgValidarExistenciaDeCarritoWebPasado + " idCarrito: " + car_id_aux, DateTime.Now, car_id_aux);
                    cDllPedidoTransfer oRepetido = new cDllPedidoTransfer();
                    oRepetido.Error = msgSeProdujoUnError_ValidarExistenciaDeCarritoWebPasado;
                    resultadoPedido = new List<cDllPedidoTransfer>();
                    resultadoPedido.Add(oRepetido);
                    return resultadoPedido;
                }
                if (listaCarritoAux != null)
                {
                    resultadoPedido = listaCarritoAux;
                    bool isErrorPedido = false;
                    if (listaCarritoAux.Count > 0)
                    {
                        //listaCarritoAux[0].web_Error = "ffsdfdf";
                        if (!string.IsNullOrEmpty(listaCarritoAux[0].Error) ||
                             !string.IsNullOrEmpty(listaCarritoAux[0].web_Error))
                        {
                            isErrorPedido = true;
                        }
                        // INICIO FALTANTE
                        foreach (cDllPedidoTransfer itemPedidoTransferFaltante in listaCarritoAux)
                        {
                            if (itemPedidoTransferFaltante.Login == "REVISION")
                            {

                            }
                            else if (itemPedidoTransferFaltante.Login == "CONFIRMACION")
                            {

                            }
                            else
                            {
                                if (itemPedidoTransferFaltante.Items != null)
                                {
                                    if (itemPedidoTransferFaltante.Items.Count > 0)
                                    {
                                        for (int iArrayOfCDllPedidoItem = 0; iArrayOfCDllPedidoItem < itemPedidoTransferFaltante.Items.Count; iArrayOfCDllPedidoItem++)
                                        {
                                            if (itemPedidoTransferFaltante.Items[iArrayOfCDllPedidoItem].Faltas > 0)
                                            {
                                                capaLogRegistro_base.InsertarFaltantesProblemasCrediticios(null, pIdSucursal, pCliente.cli_codigo, itemPedidoTransferFaltante.Items[iArrayOfCDllPedidoItem].NombreObjetoComercial, itemPedidoTransferFaltante.Items[iArrayOfCDllPedidoItem].Faltas, Constantes.cPEDIDO_FALTANTES);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // FIN FALTANTE
                    }
                    // Si se genero error
                    if (isErrorPedido)
                    {
                        listaCarritoAux[0].Error = FuncionesPersonalizadas_base.LimpiarStringErrorPedido(listaCarritoAux[0].Error);
                    }
                    else
                    {
                        // borrar carrito transfer
                        capaCAR_base.GuardarPedidoBorrarCarrito(pUsuario, pCliente, listaProductos_Auditoria, car_id_aux, pIdSucursal, tipo, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, false);
                    }
                }
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pUsuario, pCliente, pListaCarrito, pIsDiferido, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio);
            }
            return resultadoPedido;
        }
        public static cDllPedido TomarPedidoCarritoFacturarseFormaHabitual(Usuario pUsuario, cClientes pCliente, string pHorarioCierre, string pIdSucursal, string pMensajeEnFactura, string pMensajeEnRemito, string pTipoEnvio, bool pIsUrgente, string[] pListaNombreComercial, int[] pListaCantidad)
        {
            cDllPedido resultadoPedido = null;
            //if (!capaCAR_WebService_base.IsBanderaCodigo(Constantes.cBAN_SERVIDORDLL))
            //{
            //    cDllPedido oEnProceso = new cDllPedido();
            //    oEnProceso.Error = msgRealizandoTareasMantenimiento;
            //    return oEnProceso;
            //}

            DataTable tablaBusqueda = FuncionesPersonalizadas_base.ObtenerDataTableProductosCarritoArchivosPedidos();
            for (int i = 0; i < pListaNombreComercial.Count(); i++)
            {
                DataRow fila = tablaBusqueda.NewRow();
                fila["codProducto"] = pListaNombreComercial[i];
                tablaBusqueda.Rows.Add(fila);
            }
            DataTable tablaProductos = capaProductos_base.RecuperarProductoPorTablaNombre(tablaBusqueda);

            List<cDllProductosAndCantidad> listaProductos = new List<cDllProductosAndCantidad>();
            for (int i = 0; i < pListaNombreComercial.Count(); i++)
            {
                cDllProductosAndCantidad obj = new cDllProductosAndCantidad();
                obj.codProductoNombre = pListaNombreComercial[i];
                obj.cantidad = pListaCantidad[i];
                DataRow filaFind = tablaProductos
    .AsEnumerable()
    .Where(myRow => myRow.Field<string>("pro_nombre") == obj.codProductoNombre).FirstOrDefault();
                if (filaFind != null)
                {
                    cProductos objProductoBD = DKbase.web.acceso.ConvertToProductos(filaFind);

                    obj.isOferta = (objProductoBD.pro_ofeunidades == 0 && objProductoBD.pro_ofeporcentaje == 0) ? false : true;
                    listaProductos.Add(obj);
                }
            }
            resultadoPedido = capaDLL.TomarPedido(pCliente.cli_login, pIdSucursal, pMensajeEnFactura, pMensajeEnRemito, pTipoEnvio, listaProductos, pIsUrgente);
            if (resultadoPedido != null)
            {
                bool isErrorPedido = false;
                if (!string.IsNullOrEmpty(resultadoPedido.Error) ||
                            !string.IsNullOrEmpty(resultadoPedido.web_Error))
                {
                    isErrorPedido = true;
                }
                // Si se genero error
                if (isErrorPedido)
                {
                    resultadoPedido.Error = FuncionesPersonalizadas_base.LimpiarStringErrorPedido(resultadoPedido.Error);
                }
                else
                {
                    // Obtener horario cierre
                    // string horarioCierre = ObtenerHorarioCierre_interno(pIdSucursal);
                    resultadoPedido.Login = pHorarioCierre;
                    // fin Obtener horario cierre
                    // OPTIMIZAR //////////////////
                    foreach (cDllPedidoItem itemFaltantes in resultadoPedido.Items)
                    {
                        if (itemFaltantes.Faltas > 0)
                        {
                            capaLogRegistro_base.InsertarFaltantesProblemasCrediticios(null, pIdSucursal, pCliente.cli_codigo, itemFaltantes.NombreObjetoComercial, itemFaltantes.Faltas, Constantes.cPEDIDO_FALTANTES);
                        }
                        //
                    }
                    foreach (cDllPedidoItem itemConProblemasDeCreditos in resultadoPedido.ItemsConProblemasDeCreditos)
                    {
                        int cantidadProblemaCrediticia = itemConProblemasDeCreditos.Cantidad + itemConProblemasDeCreditos.Faltas;
                        if (cantidadProblemaCrediticia > 0)
                        {
                            capaLogRegistro_base.InsertarFaltantesProblemasCrediticios(null, pIdSucursal, pCliente.cli_codigo, itemConProblemasDeCreditos.NombreObjetoComercial, cantidadProblemaCrediticia, Constantes.cPEDIDO_PROBLEMACREDITICIO);
                        }
                    }
                }
            }
            return resultadoPedido;
        }
        public static cProductos RecuperarProductoPorNombre(string pNombreProducto)
        {
            cProductos resultado = null;
            DataTable tablaProductos = capaProductos_base.RecuperarProductoPorNombre(pNombreProducto);
            if (tablaProductos != null)
            {
                if (tablaProductos.Rows.Count > 0)
                {
                    resultado = DKbase.web.acceso.ConvertToProductos(tablaProductos.Rows[0]);
                }
            }
            return resultado;
        }
        public static List<DKbase.dll.cDllPedido> ObtenerPedidosEntreFechas(string pLoginWeb, DateTime pDesde, DateTime pHasta)
        {
            return capaDLL.ObtenerPedidosEntreFechas(pLoginWeb, pDesde, pHasta);
        }
        public static bool IsBanderaCodigo(string pCodigoBandera)
        {
            bool resultado = true;
            DataTable tabla = capaSeguridad_base.RecuperarTablaBandera(pCodigoBandera);
            if (tabla != null)
            {
                if (tabla.Rows.Count > 0)
                {
                    if (tabla.Rows[0]["ban_estado"] != DBNull.Value)
                    {
                        resultado = Convert.ToBoolean(tabla.Rows[0]["ban_estado"]);
                    }
                }
            }
            return resultado;
        }
    }
}
