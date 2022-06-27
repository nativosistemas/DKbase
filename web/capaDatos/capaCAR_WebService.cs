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

    }
}
