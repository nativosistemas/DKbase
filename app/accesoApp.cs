using DKbase.Entities;
using DKbase.web;
using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DKbase.app
{
    public class accesoApp
    {
        public static List<Farmacia> RecuperarFarmacias(string pPromotor)
        {
            List<Farmacia> resultado = null;
            DataTable tablaClientes = DKbase.web.capaDatos.capaClientes.spRecuperarTodosClientesByPromotor(pPromotor);
            if (tablaClientes != null)
            {
                resultado = new List<Farmacia>();
                foreach (DataRow item in tablaClientes.Rows)
                {
                    cClientes oCliente = acceso.ConvertToCliente(item);
                    Farmacia o = new Farmacia();
                    o.direccion = oCliente.cli_dirección;
                    o.id = oCliente.cli_codigo;
                    o.nombre = oCliente.cli_nombre;
                    resultado.Add(o);
                }
            }
            return resultado;
        }
        public static Modulo ConvertToModulo(DataRow pItem)
        {
            Modulo obj = new Modulo();
            if (pItem.Table.Columns.Contains("tfr_codigo") && pItem["tfr_codigo"] != DBNull.Value)
            {
                obj.id = Convert.ToInt32(pItem["tfr_codigo"]);
            }
            if (pItem.Table.Columns.Contains("tfr_descripcion") && pItem["tfr_descripcion"] != DBNull.Value)
            {
                obj.descripcion = pItem["tfr_descripcion"].ToString();
            }
            if (pItem.Table.Columns.Contains("tfr_nombre") && pItem["tfr_nombre"] != DBNull.Value)
            {
                obj.nombre = pItem["tfr_nombre"].ToString();
            }
            return obj;
        }
        public static ModuloDetalle ConvertToModuloDetalle(DataRow pItem)
        {
            ModuloDetalle obj = new ModuloDetalle();
            if (pItem.Table.Columns.Contains("pro_codigo") && pItem["pro_codigo"] != DBNull.Value
                && pItem.Table.Columns.Contains("tde_codtfr") && pItem["tde_codtfr"] != DBNull.Value)
            {
                obj.id = pItem["tde_codtfr"].ToString() + "_" + pItem["pro_codigo"].ToString();
            }
            if (pItem.Table.Columns.Contains("tde_codpro") && pItem["tde_codpro"] != DBNull.Value)
            {
                obj.producto = pItem["tde_codpro"].ToString();
            }
            if (pItem.Table.Columns.Contains("tde_codtfr") && pItem["tde_codtfr"] != DBNull.Value)
            {
                obj.idModulo = Convert.ToInt32(pItem["tde_codtfr"]);
            }
            if (pItem.Table.Columns.Contains("tde_descripcion") && pItem["tde_descripcion"] != DBNull.Value)
            {
                obj.descripcion = pItem["tde_descripcion"].ToString();
            }
            if (pItem.Table.Columns.Contains("tde_predescuento") && pItem["tde_predescuento"] != DBNull.Value)
            {
                obj.precioDescuento = Convert.ToDouble(pItem["tde_predescuento"]);
            }
            if (pItem.Table.Columns.Contains("tde_prepublico") && pItem["tde_prepublico"] != DBNull.Value)
            {
                obj.precio = Convert.ToDouble(pItem["tde_prepublico"]);
            }
            return obj;
        }
        public static List<Modulo> RecuperarTodosModulos()
        {
            List<Modulo> resultado = null;
            DataSet dsResultado = capaModulo.RecuperarTodosTransferMasDetalle();
            if (dsResultado != null)
            {
                resultado = new List<Modulo>();
                DataTable tbTransfer = dsResultado.Tables[0];
                for (int i = 0; i < tbTransfer.Rows.Count; i++)
                {
                    Modulo obj = ConvertToModulo(tbTransfer.Rows[i]);
                    List<ModuloDetalle> listaDetalle = null;
                    if (dsResultado.Tables.Count > 1)
                    {
                        listaDetalle = new List<ModuloDetalle>();
                        DataTable tablaDetalle = dsResultado.Tables[1];
                        DataRow[] listaFila = tablaDetalle.Select("tde_codtfr =" + obj.id);// obj.id == obj.tfr_codigo
                        foreach (DataRow itemTransferDetalle in listaFila)
                        {
                            ModuloDetalle objDetalle = ConvertToModuloDetalle(itemTransferDetalle);
                            listaDetalle.Add(objDetalle);
                        }
                        obj.moduloDetalle = listaDetalle;
                    }
                    resultado.Add(obj);
                }

            }
            return resultado;
        }
    }
}
