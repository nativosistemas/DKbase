using DKbase.Entities;
using DKbase.generales;
using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DKbase.web
{
    public class acceso
    {
        public static Usuario Login(string pLogin, string pPassword, string pIp, string pHostName, string pUserAgent)
        {
            DataSet dsResultado = capaSeguridad.Login(pLogin, pPassword, pIp, pHostName, pUserAgent);
            if (dsResultado != null)
            {
                if (dsResultado.Tables["Login"].Rows.Count == 0)
                {
                    Usuario usSin = new Usuario();
                    usSin.id = -1;
                    return usSin;
                }
                else
                {
                    Usuario us = new Usuario();
                    us.id = Convert.ToInt32(dsResultado.Tables["Login"].Rows[0]["usu_codigo"]);
                    us.idRol = Convert.ToInt32(dsResultado.Tables["Login"].Rows[0]["usu_codRol"]);
                    us.NombreYApellido = Convert.ToString(dsResultado.Tables["Login"].Rows[0]["NombreYapellido"]).Trim();
                    us.ApNombre = Convert.ToString(dsResultado.Tables["Login"].Rows[0]["ApNombre"]).Trim();
                    us.idUsuarioLog = Convert.ToInt32(dsResultado.Tables["Login"].Rows[0]["ulg_codUsuarioLog"]);
                    if (dsResultado.Tables["Login"].Rows[0]["usu_estado"] != DBNull.Value)
                    {
                        us.usu_estado = Convert.ToInt32(dsResultado.Tables["Login"].Rows[0]["usu_estado"]);
                    }
                    if (dsResultado.Tables["Login"].Rows[0]["usu_codCliente"] == DBNull.Value)
                    {
                        us.usu_codCliente = null;
                    }
                    else
                    {
                        us.usu_codCliente = Convert.ToInt32(dsResultado.Tables["Login"].Rows[0]["usu_codCliente"]);
                    }
                    if (dsResultado.Tables["Login"].Rows[0]["usu_pswDesencriptado"] != DBNull.Value)
                    {
                        us.usu_pswDesencriptado = Convert.ToString(dsResultado.Tables["Login"].Rows[0]["usu_pswDesencriptado"]);
                    }
                    return us;
                }
            }
            else
            {
                return null;
            }
        }
        public static ListaAcccionesRol RecuperarTodasAccionesPorIdRol(int pIdRol)
        {
            DataTable tablaAcciones = capaDatos.capaSeguridad.RecuperarTodasAccionesRol(pIdRol);
            ListaAcccionesRol listaAcciones = new ListaAcccionesRol();
            foreach (DataRow item in tablaAcciones.Rows)
            {
                AcccionesRol acRol = new AcccionesRol();

                acRol.idRegla = Convert.ToInt32(item["rgl_codRegla"]);
                acRol.palabraClave = item["rgl_PalabraClave"].ToString();

                if (DBNull.Value.Equals(item["rrr_codRelacionRolRegla"]))
                {
                    acRol.idReglaRol = null;
                }
                else
                {
                    acRol.idReglaRol = Convert.ToInt32(item["rrr_codRelacionRolRegla"]);
                }

                if (DBNull.Value.Equals(item["rrr_IsActivo"]))
                {
                    acRol.isActivo = false;
                }
                else
                {
                    acRol.isActivo = Convert.ToBoolean(item["rrr_IsActivo"]);
                }

                if (DBNull.Value.Equals(item["rrr_IsAgregar"]))
                {
                    acRol.isAgregar = false;
                }
                else
                {
                    acRol.isAgregar = Convert.ToBoolean(item["rrr_IsAgregar"]);
                }

                if (DBNull.Value.Equals(item["rrr_IsEditar"]))
                {
                    acRol.isEditar = false;
                }
                else
                {
                    acRol.isEditar = Convert.ToBoolean(item["rrr_IsEditar"]);
                }

                if (DBNull.Value.Equals(item["rrr_IsEliminar"]))
                {
                    acRol.isEliminar = false;
                }
                else
                {
                    acRol.isEliminar = Convert.ToBoolean(item["rrr_IsEliminar"]);
                }

                listaAcciones.Agregar(acRol);
            }
            return listaAcciones;
        }
        public static List<cSucursal> RecuperarTodasSucursalesDependientes()
        {
            List<cSucursal> resultado = null;
            DataSet dsResultado = capaClientes.GestiónSucursal(null, null, null, Constantes.cSQL_SELECT);
            if (dsResultado != null)
            {
                resultado = new List<cSucursal>();
                for (int i = 0; i < dsResultado.Tables["Sucursal"].Rows.Count; i++)
                {
                    cSucursal obj = new cSucursal();
                    if (dsResultado.Tables["Sucursal"].Rows[i]["sde_codigo"] != DBNull.Value)
                    {
                        obj.sde_codigo = Convert.ToInt32(dsResultado.Tables["Sucursal"].Rows[i]["sde_codigo"]);
                    }
                    if (dsResultado.Tables["Sucursal"].Rows[i]["sde_sucursal"] != DBNull.Value)
                    {
                        obj.sde_sucursal = dsResultado.Tables["Sucursal"].Rows[i]["sde_sucursal"].ToString();
                    }
                    if (dsResultado.Tables["Sucursal"].Rows[i]["sde_sucursalDependiente"] != DBNull.Value)
                    {
                        obj.sde_sucursalDependiente = dsResultado.Tables["Sucursal"].Rows[i]["sde_sucursalDependiente"].ToString();
                    }
                    if (dsResultado.Tables["Sucursal"].Rows[i]["sde_sucursalDependiente"] != DBNull.Value && dsResultado.Tables["Sucursal"].Rows[i]["sde_sucursal"] != DBNull.Value)
                    {
                        obj.sucursal_sucursalDependiente = obj.sde_sucursal + " - " + obj.sde_sucursalDependiente;
                    }
                    resultado.Add(obj);
                }

            }
            return resultado;
        }
        public static List<string> RecuperarTodosCodigoReparto()
        {
            List<string> resultado = null;
            DataTable tabla = capaClientes.RecuperarTodosCodigoReparto();
            if (tabla != null)
            {
                resultado = new List<string>();
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    if (tabla.Rows[i]["cli_codrep"] != DBNull.Value)
                    {
                        resultado.Add(tabla.Rows[i]["cli_codrep"].ToString());
                    }
                }
            }
            return resultado;
        }
        private static Farmacia ConvertToFarmacia(DataRow pItem)
        {
            Farmacia obj = new Farmacia();
            if (pItem.Table.Columns.Contains("cli_codigo") && pItem["cli_codigo"] != DBNull.Value)
                obj.id = Convert.ToInt32(pItem["cli_codigo"]);
            if (pItem.Table.Columns.Contains("cli_nombre") && pItem["cli_nombre"] != DBNull.Value)
                obj.nombre = Convert.ToString(pItem["cli_nombre"]);
            if (pItem.Table.Columns.Contains("cli_dirección") && pItem["cli_dirección"] != DBNull.Value)
                obj.direccion = Convert.ToString(pItem["cli_dirección"]);
            return obj;
        }
        public static List<Farmacia> RecuperarFarmacias()
        {
            List<Farmacia> resultado = null;
            DataTable tabla = capaClientes.RecuperarTodosClientes();
            if (tabla != null)
            {
                resultado = new List<Farmacia>();
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    resultado.Add(ConvertToFarmacia(tabla.Rows[i]));
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
            if (pItem.Table.Columns.Contains("tde_codpro") && pItem["tde_codpro"] != DBNull.Value)
            {
                obj.nombreProducto = pItem["tde_codpro"].ToString();
            }
            if (pItem.Table.Columns.Contains("tde_codtfr") && pItem["tde_codtfr"] != DBNull.Value)
            {
                obj.idModulo = Convert.ToInt32(pItem["tde_codtfr"]);
            }
            if (pItem.Table.Columns.Contains("tde_descripcion") && pItem["tde_descripcion"] != DBNull.Value)
            {
                obj.descripcionProducto = pItem["tde_descripcion"].ToString();
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
                        obj.listaModuloDetalle = listaDetalle;
                    }
                    resultado.Add(obj);
                }

            }
            return resultado;
        }
    }
}
