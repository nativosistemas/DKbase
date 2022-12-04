using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace DKbase
{
    public class Util
    {
        public static List<cMensaje> RecuperarTodosMensajesPorIdCliente(int pIdCliente)
        {
            List<cMensaje> resultado = new List<cMensaje>();
            DataTable tabla = capaMensaje_base.RecuperartTodosMensajesPorIdCliente(pIdCliente);
            if (tabla != null)
            {
                foreach (DataRow item in tabla.Rows)
                {
                    resultado.Add(capaMensaje_base.ConvertToMensaje(item));
                }
            }
            return resultado;

        }
        public static List<cClientes> RecuperarTodosClientesByGrupoCliente(string pGC)
        {
            List<cClientes> resultado = null;
            DataTable tablaClientes = capaClientes_base.RecuperarTodosClientesByGrupoCliente(pGC);
            if (tablaClientes != null)
            {
                resultado = new List<cClientes>();
                foreach (DataRow item in tablaClientes.Rows)
                {
                    resultado.Add(DKbase.web.acceso.ConvertToCliente(item));
                }
            }
            return resultado;
        }
        public static List<cClientes> RecuperarTodosClientes()
        {
            List<cClientes> resultado = null;
            DataTable tablaClientes = capaClientes_base.RecuperarTodosClientes();
            if (tablaClientes != null)
            {
                resultado = new List<cClientes>();
                foreach (DataRow item in tablaClientes.Rows)
                {
                    resultado.Add(DKbase.web.acceso.ConvertToCliente(item));
                }
            }
            return resultado;
        }
        public static List<cClientes> spRecuperarTodosClientesByPromotor(string pPromotor)
        {
            List<cClientes> resultado = null;
            DataTable tablaClientes = DKbase.web.capaDatos.capaClientes_base.spRecuperarTodosClientesByPromotor(pPromotor);
            if (tablaClientes != null)
            {
                resultado = new List<cClientes>();
                foreach (DataRow item in tablaClientes.Rows)
                {
                    resultado.Add(DKbase.web.acceso.ConvertToCliente(item));
                }
            }
            return resultado;
        }
        public static List<cMensaje> RecuperartTodosMensajeNewPorSucursal(string pSucursal, string pReparto)
        {
            List<cMensaje> lista = new List<cMensaje>();
            List<cMensajeNew> lista_aux = new List<cMensajeNew>();
            DataTable tabla = capaMensaje_base.RecuperartTodosMensajeNewPorSucursal(pSucursal, pReparto);
            if (tabla != null)
            {
                foreach (DataRow item in tabla.Rows)
                {
                    cMensajeNew obj = capaMensaje_base.ConvertToMensajeNew(item);
                    lista_aux.Add(obj);
                }
                if (lista_aux.Count > 0)
                {
                    foreach (cMensajeNew m in lista_aux)
                    {
                        cMensaje o = new cMensaje() { tme_asunto = m.tmn_asunto, tme_mensaje = m.tmn_mensaje };
                        lista.Add(o);
                    }
                }
            }
            return lista;

        }
        public static List<cArchivo> RecuperarTodosArchivos(int pArc_codRelacion, string pArc_galeria, string pFiltro)
        {
            List<cArchivo> lista = new List<cArchivo>();
            DataSet dsResultado = DKbase.web.capaDatos.capaRecurso_base.GestiónArchivo(null, pArc_codRelacion, pArc_galeria, null, null, null, null, null, null, null, null, null, pFiltro, DKbase.generales.Constantes.cSQL_SELECT);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["Archivo"].Rows)
                {
                    lista.Add(DKbase.web.acceso.ConvertToArchivo(item));
                }
            }
            return lista;
        }
        public static List<cArchivo> RecuperarPopUpPorCliente(int pIdCliente, string pSucursal)
        {
            List<DKbase.web.capaDatos.cArchivo> result = null;
            result = RecuperarTodosArchivos(1, "popup", string.Empty).Where(x => x.arc_estado == DKbase.generales.Constantes.cESTADO_ACTIVO).ToList();
            result = result.Where(x => string.IsNullOrWhiteSpace(x.arc_descripcion) || x.arc_descripcion.Contains("<" + pSucursal + ">")).ToList();
            foreach (var item in result)
            {
                // item.arc_mime
                string RutaCompleta = Helper.getFolder + @"\archivos\" + "popup" + @"\";
                string RutaCompletaNombreArchivo = RutaCompleta + item.arc_nombre;
                if (System.IO.File.Exists(RutaCompletaNombreArchivo) && System.Text.RegularExpressions.Regex.IsMatch(item.arc_nombre.ToLower(), @"^.*\.(jpg|gif|png|jpeg|bmp)$"))
                {
                    System.Drawing.Image origImagen = DKbase.web.cThumbnail_base.obtenerImagen("popup", item.arc_nombre, "1024", "768", "", false);
                    item.ancho = origImagen.Width;
                    item.alto = origImagen.Height;
                }
            }
            return result;
        }
        public static List<cClientes> RecuperarTodosClientesBySucursal(string pSucursal)
        {
            List<cClientes> resultado = null;
            DataTable tablaClientes = capaClientes_base.RecuperarTodosClientesBySucursal(pSucursal);
            if (tablaClientes != null)
            {
                resultado = new List<cClientes>();
                foreach (DataRow item in tablaClientes.Rows)
                {
                    resultado.Add(DKbase.web.acceso.ConvertToCliente(item));
                }
            }
            return resultado;
        }
    }
}
