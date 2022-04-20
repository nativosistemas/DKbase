using DKbase.Entities;
using DKbase.web;
using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace DKbase.app
{
    public class accesoApp
    {
        public static List<Farmacia> RecuperarFarmacias(string pPromotor, string pConnectionStringSQL = null)
        {
            List<Farmacia> resultado = null;
            DataTable tablaClientes = DKbase.web.capaDatos.capaClientes_base.spRecuperarTodosClientesByPromotor(pPromotor, pConnectionStringSQL);
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
                    o.objCliente = oCliente;
                    resultado.Add(o);
                }
            }
            return resultado;
        }
        public static Modulo ConvertToModulo(DataRow pItem, Modulo obj = null)
        {
            //Modulo obj = new Modulo();
            if (obj == null)
            {
                obj = new Modulo();
            }
            if (pItem.Table.Columns.Contains("mod_numeroModulo") && pItem["mod_numeroModulo"] != DBNull.Value)
            {
                obj.id = Convert.ToInt32(pItem["mod_numeroModulo"]);
            }
            if (pItem.Table.Columns.Contains("mod_cantidadMinimos") && pItem["mod_cantidadMinimos"] != DBNull.Value)
            {
                obj.cantidadMinimos = Convert.ToInt32(pItem["mod_cantidadMinimos"]);
            }
            if (pItem.Table.Columns.Contains("mod_cuitLaboratorio") && pItem["mod_cuitLaboratorio"] != DBNull.Value)
            {
                obj.idLaboratorio = Convert.ToInt64(pItem["mod_cuitLaboratorio"]);
            }
            if (pItem.Table.Columns.Contains("mod_descripcion") && pItem["mod_descripcion"] != DBNull.Value)
            {
                obj.descripcion = pItem["mod_descripcion"].ToString();
            }
            if (pItem.Table.Columns.Contains("lab_laboratorio") && pItem["lab_laboratorio"] != DBNull.Value)
            {
                obj.nombre_laboratorio = pItem["lab_laboratorio"].ToString();
            }
            return obj;
        }
        public static ModuloDetalle ConvertToModuloDetalle(DataRow pItem)
        {
            ModuloDetalle obj = new ModuloDetalle();
            if (pItem.Table.Columns.Contains("pro_nombre") && pItem["pro_nombre"] != DBNull.Value)
            {
                obj.producto = pItem["pro_nombre"].ToString();
            }
            if (pItem.Table.Columns.Contains("dmo_numeroModulo") && pItem["dmo_numeroModulo"] != DBNull.Value)
            {
                obj.idModulo = Convert.ToInt32(pItem["dmo_numeroModulo"]);
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
            if (pItem.Table.Columns.Contains("dmo_TieneEnCuentaDescuentoCliente") && pItem["dmo_TieneEnCuentaDescuentoCliente"] != DBNull.Value)
            {
                obj.isTieneEnCuentaDescuentoCliente = Convert.ToBoolean(pItem["dmo_TieneEnCuentaDescuentoCliente"]);
            }
            obj.objProducto = acceso.ConvertToProductos(pItem);
            return obj;
        }
        public static Laboratorio ConvertToLaboratorio(DataRow pItem)
        {
            Laboratorio obj = new Laboratorio();
            if (pItem.Table.Columns.Contains("lab_cuit") && pItem["lab_cuit"] != DBNull.Value)
            {
                obj.id = Convert.ToInt64(pItem["lab_cuit"]);
            }
            if (pItem.Table.Columns.Contains("lab_codigo") && pItem["lab_codigo"] != DBNull.Value)
            {
                obj.idParaArchivo = Convert.ToInt32(pItem["lab_codigo"]);
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
        public static AppInfoPedido ConvertToInfoPedido(DataRow pItem, AppInfoPedido obj = null)
        {
            if (obj == null)
            {
                obj = new AppInfoPedido();
            }
            if (pItem.Table.Columns.Contains("pea_id") && pItem["pea_id"] != DBNull.Value)
            {
                obj.pea_id = Convert.ToInt32(pItem["pea_id"]);
            }
            if (pItem.Table.Columns.Contains("pea_guid") && pItem["pea_guid"] != DBNull.Value)
            {
                obj.pea_guid = Guid.Parse(pItem["pea_guid"].ToString());
            }
            if (pItem.Table.Columns.Contains("pea_promotor") && pItem["pea_promotor"] != DBNull.Value)
            {
                obj.pea_promotor = pItem["pea_promotor"].ToString();
            }
            if (pItem.Table.Columns.Contains("pea_cantidad") && pItem["pea_cantidad"] != DBNull.Value)
            {
                obj.pea_cantidad = Convert.ToInt32(pItem["pea_cantidad"]);
            }
            if (pItem.Table.Columns.Contains("pea_fecha") && pItem["pea_fecha"] != DBNull.Value)
            {
                obj.pea_fecha = Convert.ToDateTime(pItem["pea_fecha"]);
            }
            if (pItem.Table.Columns.Contains("pea_codCliente") && pItem["pea_codCliente"] != DBNull.Value)
            {
                obj.pea_codCliente = Convert.ToInt32(pItem["pea_codCliente"]);
            }
            if (pItem.Table.Columns.Contains("pea_numeroModulo") && pItem["pea_numeroModulo"] != DBNull.Value)
            {
                obj.pea_numeroModulo = Convert.ToInt32(pItem["pea_numeroModulo"]);
            }
            if (pItem.Table.Columns.Contains("pea_procesado") && pItem["pea_procesado"] != DBNull.Value)
            {
                obj.pea_procesado = Convert.ToBoolean(pItem["pea_procesado"]);
            }
            if (pItem.Table.Columns.Contains("pea_procesado_fecha") && pItem["pea_procesado_fecha"] != DBNull.Value)
            {
                obj.pea_procesado_fecha = Convert.ToDateTime(pItem["pea_procesado_fecha"]);
            }
            if (pItem.Table.Columns.Contains("pea_procesado_cantidad") && pItem["pea_procesado_cantidad"] != DBNull.Value)
            {
                obj.pea_procesado_cantidad = Convert.ToInt32(pItem["pea_procesado_cantidad"]);
            }
            if (pItem.Table.Columns.Contains("pea_procesado_descripcion") && pItem["pea_procesado_descripcion"] != DBNull.Value)
            {
                obj.pea_procesado_descripcion = pItem["pea_procesado_descripcion"].ToString();
            }
            return obj;
        }
        public static List<ModuloDetalle> getList_ModuloDetalle(DataRow[] listaFila)
        {           
            List<ModuloDetalle> listaDetalle = null;
            if (listaFila != null)
            {
                listaDetalle = new List<ModuloDetalle>();                
                foreach (DataRow itemTransferDetalle in listaFila)
                {
                    ModuloDetalle objDetalle = ConvertToModuloDetalle(itemTransferDetalle);
                    listaDetalle.Add(objDetalle);
                }
            }
            return listaDetalle;
        }
        public static List<Laboratorio> GetLaboratorios(string pConnectionStringSQL = null)
        {
            List<Laboratorio> resultado = null;
            DataSet dsResultado = capaModulo.spGetLaboratorios(pConnectionStringSQL);
            if (dsResultado != null)
            {
                resultado = new List<Laboratorio>();
                DataTable tbTransfer = dsResultado.Tables[0];
                for (int i = 0; i < tbTransfer.Rows.Count; i++)
                {
                    Laboratorio obj = ConvertToLaboratorio(tbTransfer.Rows[i]);
                    int codigoLab = 0;
                    if (tbTransfer.Rows[i].Table.Columns.Contains("lab_codigo") && tbTransfer.Rows[i]["lab_codigo"] != DBNull.Value)
                    {
                        codigoLab = Convert.ToInt32(tbTransfer.Rows[i]["lab_codigo"]);
                    }
                    List<cArchivo> l_archivo = acceso.RecuperarTodosArchivos(codigoLab, generales.Constantes.cLABORATORIO, string.Empty);
                    if (l_archivo != null && l_archivo.Count > 0)
                    {
                        obj.imagen = l_archivo[0].arc_nombre;
                    }
                    string imagen = obj.imagen;
                    if (string.IsNullOrEmpty(imagen))
                    {
                        imagen = "amissingthumbnail0.png";
                    }
                    using (System.Drawing.Image image = DKbase.generales.cThumbnail.obtenerImagen("laboratorio", imagen, "400", "400", "#FFFFFF", false))
                    {
                        if (image != null)
                        {
                            using (MemoryStream m = new MemoryStream())
                            {
                                image.Save(m, image.RawFormat);
                                byte[] imageBytes = m.ToArray();
                                string base64String = Convert.ToBase64String(imageBytes);
                                obj.imagenBase64 = base64String;
                            }
                        }
                    }
                    resultado.Add(obj);
                }

            }
            return resultado;
        }
        public static List<Modulo> RecuperarTodosModulos(string pConnectionStringSQL = null)
        {
            List<Modulo> resultado = null;
            DataSet dsResultado = capaModulo.spGetModulos(pConnectionStringSQL);
            if (dsResultado != null)
            {
                resultado = new List<Modulo>();
                DataTable tbTransfer = dsResultado.Tables[0];
                for (int i = 0; i < tbTransfer.Rows.Count; i++)
                {
                    Modulo obj = ConvertToModulo(tbTransfer.Rows[i]);
                    if (dsResultado.Tables.Count > 1)
                    {
                        DataRow[] listaFila = dsResultado.Tables[1].Select("dmo_numeroModulo =" + obj.id);
                        obj.moduloDetalle = getList_ModuloDetalle(listaFila);
                    }
                    resultado.Add(obj);
                }

            }
            return resultado;
        }
        public static void DeleteModulo(int id)
        {
            capaModulo.spDeleteModulo(id);
        }
        public static Guid AddPedido(AppPedido pPedido, string pConnectionStringSQL = null)
        {
            string strXML = string.Empty;
            strXML += "<Root>";
            foreach (AppPedidoModulo item in pPedido.pedidoModulos)
            {
                List<XAttribute> listaAtributos = new List<XAttribute>();
                listaAtributos.Add(new XAttribute("pmo_cantidad", item.cantidad));
                listaAtributos.Add(new XAttribute("pmo_nroModulo", item.idModulo));
                listaAtributos.Add(new XAttribute("pmo_codCliente", item.idFarmacia));
                XElement nodo = new XElement("DetallePedido", listaAtributos);
                strXML += nodo.ToString();
            }
            strXML += "</Root>";
            return capaModulo.spAddPedido(pPedido.promotor, strXML, pConnectionStringSQL);
        }
        public static List<AppInfoPedido> RecuperarTodoInfoPedidos(string pPromotor, string pConnectionStringSQL = null)
        {
            List<AppInfoPedido> resultado = null;
            DataSet dsResultado = capaModulo.spGetInfoPedidos(pPromotor, pConnectionStringSQL);
            if (dsResultado != null)
            {
                resultado = new List<AppInfoPedido>();
                DataTable tbInfo = dsResultado.Tables[0];
                for (int i = 0; i < tbInfo.Rows.Count; i++)
                {
                    AppInfoPedido obj = ConvertToInfoPedido(tbInfo.Rows[i]);
                    resultado.Add(obj);
                }

            }
            return resultado;
        }
        public static List<AppInfoPedido> RecuperarTodoPedidoModuloHistorial(string pPromotor, string pConnectionStringSQL = null)
        {
            List<AppInfoPedido> resultado = null;
            DataSet dsResultado = capaModulo.spGetHistorialPedidos(pPromotor, pConnectionStringSQL);
            if (dsResultado != null)
            {
                resultado = new List<AppInfoPedido>();
                DataTable tbInfo = dsResultado.Tables[0];
                for (int i = 0; i < tbInfo.Rows.Count; i++)
                {
                    AppInfoPedido obj = (AppInfoPedido)ConvertToModulo(tbInfo.Rows[i], new AppInfoPedido());
                    obj = ConvertToInfoPedido(tbInfo.Rows[i], obj);
                    if (dsResultado.Tables.Count > 1)
                    {
                        DataRow[] listaFila = dsResultado.Tables[1].Select("dmo_numeroModulo =" + obj.id + " and pmd_idPedido =" + obj.pea_id);
                        obj.moduloDetalle = getList_ModuloDetalle(listaFila);
                    }
                    resultado.Add(obj);
                }

            }
            return resultado;
        }
        public static Guid AddDatosCliente(AppCargaDatosClientes pDatosCliente, string pConnectionStringSQL = null)
        {
            return capaModulo.spAddDatosCliente(pDatosCliente, pConnectionStringSQL);
        }
    }
}
