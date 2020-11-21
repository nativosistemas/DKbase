﻿using DKbase.Entities;
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
            if (pItem.Table.Columns.Contains("mod_nroModulo") && pItem["mod_nroModulo"] != DBNull.Value)
            {
                obj.id = Convert.ToInt32(pItem["mod_nroModulo"]);
            }
            if (pItem.Table.Columns.Contains("mod_cantidadMinimos") && pItem["mod_cantidadMinimos"] != DBNull.Value)
            {
                obj.cantidadMinimos = Convert.ToInt32(pItem["mod_cantidadMinimos"]);
            }
            if (pItem.Table.Columns.Contains("mod_idLaboratorio") && pItem["mod_idLaboratorio"] != DBNull.Value)
            {
                obj.idLaboratorio = Convert.ToInt32(pItem["mod_idLaboratorio"]);
            }
            return obj;
        }
        public static ModuloDetalle ConvertToModuloDetalle(DataRow pItem)
        {
            ModuloDetalle obj = new ModuloDetalle();
            if (pItem.Table.Columns.Contains("dmo_id") && pItem["dmo_id"] != DBNull.Value)
            {
                obj.id = Convert.ToInt32(pItem["dmo_id"].ToString());
            }
            if (pItem.Table.Columns.Contains("pro_nombre") && pItem["pro_nombre"] != DBNull.Value)
            {
                obj.producto = pItem["pro_nombre"].ToString();
            }
            if (pItem.Table.Columns.Contains("dmo_idModulo") && pItem["dmo_idModulo"] != DBNull.Value)
            {
                obj.idModulo = Convert.ToInt32(pItem["dmo_idModulo"]);
            }
            if (pItem.Table.Columns.Contains("dmo_orden") && pItem["dmo_orden"] != DBNull.Value)
            {
                obj.orden = Convert.ToInt32(pItem["dmo_orden"]);
            }            
            if (pItem.Table.Columns.Contains("dmo_descripcion") && pItem["dmo_descripcion"] != DBNull.Value)
            {
                obj.descripcion = pItem["dmo_descripcion"].ToString();
            }
            if (pItem.Table.Columns.Contains("dmo_precio") && pItem["dmo_precio"] != DBNull.Value)
            {
                obj.precio = Convert.ToDouble(pItem["dmo_precio"]);
            }
            if (pItem.Table.Columns.Contains("dmo_precioDescuento") && pItem["dmo_precioDescuento"] != DBNull.Value)
            {
                obj.precioDescuento = Convert.ToDouble(pItem["dmo_precioDescuento"]);
            }
            if (pItem.Table.Columns.Contains("dmo_cantidadUnidades") && pItem["dmo_cantidadUnidades"] != DBNull.Value)
            {
                obj.cantidadUnidades = Convert.ToInt32(pItem["dmo_cantidadUnidades"]);
            }            
            return obj;
        }
        public static Laboratorio ConvertToLaboratorio(DataRow pItem)
        {
            Laboratorio obj = new Laboratorio();
            if (pItem.Table.Columns.Contains("lab_id") && pItem["lab_id"] != DBNull.Value)
            {
                obj.id = Convert.ToInt32(pItem["lab_id"]);
            }
            if (pItem.Table.Columns.Contains("lab_laboratorio") && pItem["lab_laboratorio"] != DBNull.Value)
            {
                obj.nombre = pItem["lab_laboratorio"].ToString();
            }
            if (pItem.Table.Columns.Contains("lab_nombreImagen") && pItem["lab_nombreImagen"] != DBNull.Value)
            {
                obj.imagen = pItem["lab_nombreImagen"].ToString();
            }
            return obj;
        }
        public static List<Laboratorio> GetLaboratorios()
        {
            List<Laboratorio> resultado = null;
            DataSet dsResultado = capaModulo.spGetLaboratorios();
            if (dsResultado != null)
            {
                resultado = new List<Laboratorio>();
                DataTable tbTransfer = dsResultado.Tables[0];
                for (int i = 0; i < tbTransfer.Rows.Count; i++)
                {
                    Laboratorio obj = ConvertToLaboratorio(tbTransfer.Rows[i]);
                    resultado.Add(obj);
                }

            }
            return resultado;
        }
        public static List<Modulo> RecuperarTodosModulos()
        {
            List<Modulo> resultado = null;
            DataSet dsResultado = capaModulo.spGetModulos();
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
                        DataRow[] listaFila = tablaDetalle.Select("dmo_idModulo =" + obj.id);
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
