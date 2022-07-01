using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class capaCAR_WebService_base
    {
        public static List<cCarrito> RecuperarCarritosPorSucursalYProductos_generica(cClientes objClientes, string pTipo)
        {
            DataSet dsProductoCarrito = new DataSet();

            dsProductoCarrito = capaCAR_base.RecuperarCarritosPorSucursalYProductos_generica(objClientes.cli_codigo, pTipo);

            List<cCarrito> listaSucursal = (from item in dsProductoCarrito.Tables[1].AsEnumerable()
                                            select new cCarrito { car_id = item.Field<int>("car_id"), lrc_id = item.Field<int>("car_id"), codSucursal = item.Field<string>("car_codSucursal") }).ToList();

            foreach (cCarrito item in listaSucursal)
            {
                item.proximoHorarioEntrega = DKbase.web.FuncionesPersonalizadas_base.ObtenerHorarioCierre(objClientes, objClientes.cli_codsuc, item.codSucursal, objClientes.cli_codrep);
                List<cProductosGenerico> listaProductoCarrtios = new List<cProductosGenerico>();
                foreach (DataRow itemProductoCarrtio in dsProductoCarrito.Tables[0].Select("cad_codCarrito = " + item.lrc_id))
                {
                    cProductos objProducto = DKbase.web.acceso.ConvertToProductos(itemProductoCarrtio);
                    cProductosGenerico objProductosGenerico = new cProductosGenerico(objProducto);
                    if (itemProductoCarrtio.Table.Columns.Contains("stk_stock") && itemProductoCarrtio["stk_stock"] != DBNull.Value)
                        objProductosGenerico.stk_stock = itemProductoCarrtio["stk_stock"].ToString();
                    if (itemProductoCarrtio.Table.Columns.Contains("cad_codProducto") && itemProductoCarrtio["cad_codProducto"] != DBNull.Value)
                        objProductosGenerico.codProducto = itemProductoCarrtio["cad_codProducto"].ToString();
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
                        List<cTransferDetalle> listaAUXtransferDetalle = listaTransferDetalle.Where(x => x.tde_codpro == listaProductoCarrtios[iPrecioFinal].pro_nombre).ToList();
                        if (listaAUXtransferDetalle.Count > 0)
                        {
                            listaProductoCarrtios[iPrecioFinal].isProductoFacturacionDirecta = true;
                            listaProductoCarrtios[iPrecioFinal].CargarTransferYTransferDetalle(listaAUXtransferDetalle[0]);
                            listaProductoCarrtios[iPrecioFinal].PrecioFinalTransfer = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(objClientes, listaProductoCarrtios[iPrecioFinal].tfr_deshab, listaProductoCarrtios[iPrecioFinal].tfr_pordesadi, listaProductoCarrtios[iPrecioFinal].pro_neto, listaProductoCarrtios[iPrecioFinal].pro_codtpopro, listaProductoCarrtios[iPrecioFinal].pro_descuentoweb, listaProductoCarrtios[iPrecioFinal].tde_predescuento == null ? 0 : (decimal)listaProductoCarrtios[iPrecioFinal].tde_predescuento, listaProductoCarrtios[iPrecioFinal].tde_PrecioConDescuentoDirecto, listaProductoCarrtios[iPrecioFinal].tde_PorcARestarDelDtoDeCliente);
                        }
                    }
                    /// FIN Nuevo
                }
                item.listaProductos = listaProductoCarrtios;
            }
            listaSucursal.RemoveAll(x => x.listaProductos.Count == 0);
            return listaSucursal;
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
                        if (item["nombreNoEncontrado"] != DBNull.Value)
                        {
                            cProductosGenerico obj = new cProductosGenerico();
                            obj.isProductoNoEncontrado = true;
                            obj.pro_codigo = "-1";
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
    }
}
