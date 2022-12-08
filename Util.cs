using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using DKbase.web;

namespace DKbase
{
    public class Util
    {
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
        public static List<DKbase.web.capaDatos.cOferta> RecuperarTodasOfertaPublicar(bool isNuevosLanzamiento = false)
        {
            List<cOferta> resultado = null;
            DataTable tabla = capaHome_base.RecuperarTodasOfertaPublicar();
            if (tabla != null)
            {
                resultado = new List<cOferta>();
                foreach (DataRow fila in tabla.Rows)
                {
                    resultado.Add(capaHome_base.ConvertToOferta(fila));
                }
            }
            if (resultado != null)
            {
                resultado = resultado.Where(x => x.ofe_nuevosLanzamiento == isNuevosLanzamiento).ToList();
            }
            return resultado;
        }
        public static int RecuperarProductoParametrizadoCantidad()
        {
            int resultado = -1;
            resultado = 0;
            DataTable dtResultado = capaProductos_base.RecuperarProductoParametrizadoCantidad();
            if (dtResultado != null)
            {
                foreach (DataRow item in dtResultado.Rows)
                {
                    if (item["cpc_cantidadParametrizada"] != DBNull.Value)
                    {
                        resultado = Convert.ToInt32(item["cpc_cantidadParametrizada"]);
                        break;
                    }
                }
            }
            return resultado;
        }
        public static List<cSucursalDependienteTipoEnviosCliente> RecuperarTodosSucursalDependienteTipoEnvioCliente()
        {
            List<cSucursalDependienteTipoEnviosCliente> resultado = null;
            DataTable tabla = capaTiposEnvios_base.RecuperarTodosSucursalDependienteTipoEnvioCliente();
            if (tabla != null)
            {
                resultado = new List<cSucursalDependienteTipoEnviosCliente>();
                foreach (DataRow item in tabla.Rows)
                {
                    cSucursalDependienteTipoEnviosCliente obj = capaTiposEnvios_base.ConvertToTiposEnviosSucursalDependiente(item);
                    if (obj != null)
                    {
                        resultado.Add(obj);
                    }
                }
            }
            return resultado;
        }
        public static List<cSucursalDependienteTipoEnviosCliente> RecuperarTodosSucursalDependienteTipoEnvioCliente_cliente(cClientes pCliente)
        {
            List<cSucursalDependienteTipoEnviosCliente> result = null;
            result = RecuperarTodosSucursalDependienteTipoEnvioCliente().Where(x => x.sde_sucursal == pCliente.cli_codsuc && (x.tsd_idTipoEnvioCliente == null || x.env_codigo == pCliente.cli_codtpoenv)).ToList(); ;
            return result;
        }
        public static List<cSucursalDependienteTipoEnviosCliente_TiposEnvios> RecuperarSucursalDependienteTipoEnvioCliente_TipoEnvios_TodasLasExcepciones(int pIdSucursalDependienteTipoEnvioCliente)
        {
            List<cSucursalDependienteTipoEnviosCliente_TiposEnvios> resultado = null;
            DataTable tabla = capaTiposEnvios_base.RecuperarSucursalDependienteTipoEnvioCliente_TipoEnvios_TodasLasExcepciones(pIdSucursalDependienteTipoEnvioCliente);
            if (tabla != null)
            {
                resultado = new List<cSucursalDependienteTipoEnviosCliente_TiposEnvios>();
                foreach (DataRow item in tabla.Rows)
                {
                    cSucursalDependienteTipoEnviosCliente_TiposEnvios obj = capaTiposEnvios_base.ConvertToTiposEnviosSucursalDependiente_TiposEnvios_Excepciones(item);
                    if (obj != null)
                    {
                        resultado.Add(obj);
                    }
                }
            }
            return resultado;
        }
        public static List<cSucursalDependienteTipoEnviosCliente_TiposEnvios> RecuperarTodosSucursalDependienteTipoEnvioCliente_TiposEnvios_Excepciones(int pIdSucursalDependienteTipoEnvioCliente, string tdr_codReparto)
        {
            List<cSucursalDependienteTipoEnviosCliente_TiposEnvios> resultado = null;
            DataTable tabla = capaTiposEnvios_base.RecuperarTipoEnviosExcepcionesPorSucursalDependiente(pIdSucursalDependienteTipoEnvioCliente, tdr_codReparto);
            if (tabla != null)
            {
                resultado = new List<cSucursalDependienteTipoEnviosCliente_TiposEnvios>();
                foreach (DataRow item in tabla.Rows)
                {
                    cSucursalDependienteTipoEnviosCliente_TiposEnvios obj = capaTiposEnvios_base.ConvertToTiposEnviosSucursalDependiente_TiposEnvios_Excepciones(item);
                    if (obj != null)
                    {
                        resultado.Add(obj);
                    }
                }
            }
            return resultado;
        }
        public static List<cSucursalDependienteTipoEnviosCliente_TiposEnvios> RecuperarTodosSucursalDependienteTipoEnvioCliente_TiposEnvios()
        {
            List<cSucursalDependienteTipoEnviosCliente_TiposEnvios> resultado = null;
            DataTable tabla = capaTiposEnvios_base.RecuperarTodosSucursalDependienteTipoEnvioCliente_TiposEnvios();
            if (tabla != null)
            {
                resultado = new List<cSucursalDependienteTipoEnviosCliente_TiposEnvios>();
                foreach (DataRow item in tabla.Rows)
                {
                    cSucursalDependienteTipoEnviosCliente_TiposEnvios obj = capaTiposEnvios_base.ConvertToTiposEnviosSucursalDependiente_TiposEnvios(item);
                    if (obj != null)
                    {
                        resultado.Add(obj);
                    }
                }
            }
            return resultado;
        }
        public static List<cTipoEnvioClienteFront> RecuperarTiposDeEnvios(cClientes pCliente)
        {
            List<cTipoEnvioClienteFront> resultado = null;
            if (pCliente != null)
            {
                List<cSucursalDependienteTipoEnviosCliente> lista = RecuperarTodosSucursalDependienteTipoEnvioCliente_cliente(pCliente);
                if (lista != null)
                {
                    resultado = new List<cTipoEnvioClienteFront>();
                    for (int i = 0; i < lista.Count; i++)
                    {
                        cTipoEnvioClienteFront obj = new cTipoEnvioClienteFront();
                        obj.sucursal = lista[i].sde_sucursalDependiente;
                        obj.tipoEnvio = lista[i].env_codigo;

                        List<cSucursalDependienteTipoEnviosCliente_TiposEnvios> listaTiposEnvios = RecuperarTodosSucursalDependienteTipoEnvioCliente_TiposEnvios_Excepciones(lista[i].tsd_id, pCliente.cli_codrep);
                        if (listaTiposEnvios == null || listaTiposEnvios.Count == 0)
                        {
                            listaTiposEnvios = RecuperarTodosSucursalDependienteTipoEnvioCliente_TiposEnvios().Where(x => x.tdt_idSucursalDependienteTipoEnvioCliente == lista[i].tsd_id).ToList();
                        }
                        if (listaTiposEnvios != null)
                        {
                            obj.lista = new List<cTiposEnvios>();
                            foreach (cSucursalDependienteTipoEnviosCliente_TiposEnvios itemSucursalDependienteTipoEnviosCliente_TiposEnvios in listaTiposEnvios)
                            {
                                cTiposEnvios objTipoEnvio = new cTiposEnvios();
                                objTipoEnvio.env_codigo = itemSucursalDependienteTipoEnviosCliente_TiposEnvios.env_codigo;
                                objTipoEnvio.env_nombre = itemSucursalDependienteTipoEnviosCliente_TiposEnvios.env_nombre;
                                objTipoEnvio.env_id = itemSucursalDependienteTipoEnviosCliente_TiposEnvios.env_id;
                                obj.lista.Add(objTipoEnvio);
                            }
                        }
                        //
                        resultado.Add(obj);
                    }
                }
                // Inicio S7
                if (pCliente.cli_codrep == "S7")
                {
                    cTipoEnvioClienteFront obj = new cTipoEnvioClienteFront();
                    obj.sucursal = "SF";
                    obj.tipoEnvio = null;
                    obj.lista = new List<cTiposEnvios>();
                    cTiposEnvios objTipoEnvioR = new cTiposEnvios();
                    objTipoEnvioR.env_codigo = "R";
                    objTipoEnvioR.env_nombre = "Reparto";
                    obj.lista.Add(objTipoEnvioR);
                    cTiposEnvios objTipoEnvioE = new cTiposEnvios();
                    objTipoEnvioE.env_codigo = "E";
                    objTipoEnvioE.env_nombre = "Encomienda";
                    obj.lista.Add(objTipoEnvioE);
                    cTiposEnvios objTipoEnvioM = new cTiposEnvios();
                    objTipoEnvioM.env_codigo = "M";
                    objTipoEnvioM.env_nombre = "Mostrador";
                    obj.lista.Add(objTipoEnvioM);
                    cTiposEnvios objTipoEnvioC = new cTiposEnvios();
                    objTipoEnvioC.env_codigo = "C";
                    objTipoEnvioC.env_nombre = "Cadeteria";
                    obj.lista.Add(objTipoEnvioC);
                    resultado.Add(obj);
                }
                // Fin S7
                if (!string.IsNullOrEmpty(pCliente.cli_IdSucursalAlternativa)
                    && resultado.FirstOrDefault(x => x.sucursal == pCliente.cli_IdSucursalAlternativa) == null)
                {
                    cTipoEnvioClienteFront obj = new cTipoEnvioClienteFront();
                    obj.sucursal = pCliente.cli_IdSucursalAlternativa;
                    obj.tipoEnvio = null;
                    obj.lista = new List<cTiposEnvios>();
                    cTiposEnvios objTipoEnvioR = new cTiposEnvios();
                    objTipoEnvioR.env_codigo = "R";
                    objTipoEnvioR.env_nombre = "Reparto";
                    obj.lista.Add(objTipoEnvioR);
                    resultado.Add(obj);
                }
            }
            return resultado;
        }
        public static List<cCadeteriaRestricciones> RecuperarTodosCadeteriaRestricciones()
        {
            List<cCadeteriaRestricciones> resultado = null;
            DataTable tabla = capaTiposEnvios_base.RecuperarTodosCadeteriaRestricciones();
            if (tabla != null)
            {
                resultado = new List<cCadeteriaRestricciones>();
                foreach (DataRow item in tabla.Rows)
                {
                    cCadeteriaRestricciones obj = capaTiposEnvios_base.ConvertToCadeteriaRestricciones(item);
                    if (obj != null)
                    {
                        resultado.Add(obj);
                    }
                }
            }
            return resultado;
        }
        public static Usuario Login(string pLogin, string pPassword, string pIp, string pHostName, string pUserAgent)
        {
            DataSet dsResultado = DKbase.web.capaDatos.capaSeguridad_base.Login(pLogin, pPassword, pIp, pHostName, pUserAgent);
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
                    us.usu_login = pLogin;
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
        public static void CerrarSession(int pIdUsuarioLog)
        {
            DKbase.web.capaDatos.capaSeguridad_base.CerrarSession(pIdUsuarioLog);
        }
        public static cClientes RecuperarClientePorId(int pIdCliente)
        {
            cClientes resultado = null;
            DataTable tablaClientes = capaClientes_base.RecuperarClientePorId(pIdCliente);
            if (tablaClientes != null)
            {
                foreach (DataRow item in tablaClientes.Rows)
                {
                    resultado = DKbase.web.acceso.ConvertToCliente(item);
                }
            }
            return resultado;
        }
        public static List<cUsuarioSinPermisosIntranet> RecuperarTodosSinPermisosIntranetPorIdUsuario(int pIdUsuario)
        {
            List<cUsuarioSinPermisosIntranet> resultado = null;
            DataTable tabla = capaSeguridad_base.RecuperarSinPermisoUsuarioIntranetPorIdUsuario(pIdUsuario);
            if (tabla != null)
            {
                resultado = new List<cUsuarioSinPermisosIntranet>();
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    cUsuarioSinPermisosIntranet obj = new cUsuarioSinPermisosIntranet();
                    if (tabla.Rows[i]["usp_id"] != DBNull.Value)
                    {
                        obj.usp_id = Convert.ToInt32(tabla.Rows[i]["usp_id"]);
                    }
                    if (tabla.Rows[i]["usp_codUsuario"] != DBNull.Value)
                    {
                        obj.usp_codUsuario = Convert.ToInt32(tabla.Rows[i]["usp_codUsuario"]);
                    }
                    if (tabla.Rows[i]["usp_nombreSeccion"] != DBNull.Value)
                    {
                        obj.usp_nombreSeccion = Convert.ToString(tabla.Rows[i]["usp_nombreSeccion"]);
                    }
                    resultado.Add(obj);
                }
            }
            return resultado;
        }
        public static List<string> RecuperarSinPermisosSecciones(int pIdUsuario)
        {
            List<string> resultado = null;
            List<cUsuarioSinPermisosIntranet> lista = RecuperarTodosSinPermisosIntranetPorIdUsuario(pIdUsuario);
            if (lista != null)
            {
                resultado = new List<string>();
                foreach (cUsuarioSinPermisosIntranet item in lista)
                {
                    resultado.Add(item.usp_nombreSeccion.ToUpper());
                }
            }
            return resultado;
        }
        public static ListaAcccionesRol RecuperarTodasAccionesPorIdRol(int pIdRol)
        {
            DataTable tablaAcciones = DKbase.web.capaDatos.capaSeguridad_base.RecuperarTodasAccionesRol(pIdRol);
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
    }
}
