using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using DKbase.web;
using DKbase.generales;
using System.IO;
using DKbase.dll;

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
        public static int InsertarPalabraBuscada(string pPalabra, int pIdUsuario, string pNombreTabla)
        {
            return capaLogRegistro_base.IngresarPalabraBusqueda(pIdUsuario, pNombreTabla, pPalabra);
        }
        public static void AgregarHistorialProductoCarritoTransfer(int pIdCliente, List<cProductosAndCantidad> pListaProductosMasCantidad, int? pIdUsuario)
        {
            DataTable pTablaDetalle = DKbase.web.FuncionesPersonalizadas_base.ConvertProductosAndCantidadToDataTable(pListaProductosMasCantidad);
            capaLogRegistro_base.AgregarProductosBuscadosDelCarritoTransfer(pIdCliente, pTablaDetalle, pIdUsuario);
        }
        public static decimal ObtenerPrecioFinalTransfer(cClientes pClientes, cTransfer pTransfer, cTransferDetalle pTransferDetalle)
        {
            return DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(pClientes, pTransfer.tfr_deshab, pTransfer.tfr_pordesadi, pTransferDetalle.pro_neto, pTransferDetalle.pro_codtpopro, pTransferDetalle.pro_descuentoweb, (decimal)pTransferDetalle.tde_predescuento, pTransferDetalle.tde_PrecioConDescuentoDirecto, pTransferDetalle.tde_PorcARestarDelDtoDeCliente);
        }
        public static List<cSucursalStocks> ActualizarStockListaProductos_Transfer(cClientes pClientes, List<string> pListaSucursal, string pro_codigo, List<cSucursalStocks> pSucursalStocks)
        {
            List<cSucursalStocks> result = pSucursalStocks;
            bool isActualizar = false;
            if (pClientes.cli_codrep == "S7")
                isActualizar = true;
            else if (pClientes.cli_IdSucursalAlternativa != null)
                isActualizar = true;
            if (isActualizar)
            {
                List<cProductosAndCantidad> listaProductos = new List<cProductosAndCantidad>();

                listaProductos.Add(new cProductosAndCantidad { codProductoNombre = pro_codigo });
                //List<string> ListaSucursal = RecuperarSucursalesDelCliente();
                DataTable table = DKbase.web.capaDatos.capaProductos_base.RecuperarStockPorProductosAndSucursal(DKbase.web.FuncionesPersonalizadas_base.ConvertNombresSeccionToDataTable(pListaSucursal), DKbase.web.FuncionesPersonalizadas_base.ConvertProductosAndCantidadToDataTable(listaProductos));
                if (table != null)
                    result = (from r in table.Select("stk_codpro = '" + pro_codigo + "'").AsEnumerable()
                              select new cSucursalStocks { stk_codpro = r["stk_codpro"].ToString(), stk_codsuc = r["stk_codsuc"].ToString(), stk_stock = r["stk_stock"].ToString() }).ToList();

            }
            return result;
        }
        public static List<cTransfer> RecuperarTodosTransferMasDetallePorIdProducto(string pNombreProducto, cClientes pClientes, List<string> pListaSucursal)
        {
            List<cTransfer> resultado = null;
            DataSet dsResultado = DKbase.web.capaDatos.capaTransfer_base.RecuperarTodosTransferMasDetallePorIdProducto(pClientes.cli_codsuc, pNombreProducto, pClientes.cli_codigo);
            if (dsResultado != null)
            {
                resultado = new List<cTransfer>();
                DataTable tbTransfer = dsResultado.Tables[0];
                DataTable tbTransferDetalle = dsResultado.Tables[1];
                DataTable tbSucursalStocks = dsResultado.Tables[2];
                for (int i = 0; i < tbTransfer.Rows.Count; i++)
                {
                    cTransfer obj = DKbase.web.acceso.ConvertToTransfer(tbTransfer.Rows[i]);
                    if (obj.tfr_provincia == null || obj.tfr_provincia == pClientes.cli_codprov)
                    {
                        obj.listaDetalle = new List<cTransferDetalle>();
                        DataRow[] listaFila = tbTransferDetalle.Select("tde_codtfr =" + obj.tfr_codigo);
                        foreach (DataRow item in listaFila)
                        {
                            List<cSucursalStocks> tempListaSucursalStocks = new List<cSucursalStocks>();
                            tempListaSucursalStocks = (from r in tbSucursalStocks.Select("stk_codpro = '" + item["pro_codigo"].ToString() + "'").AsEnumerable()
                                                       select new cSucursalStocks { stk_codpro = r["stk_codpro"].ToString(), stk_codsuc = r["stk_codsuc"].ToString(), stk_stock = r["stk_stock"].ToString() }).ToList();
                            if (tempListaSucursalStocks.Count > 0)
                            {
                                obj.listaDetalle.Add(DKbase.web.acceso.ConvertToTransferDetalle(item));
                                obj.listaDetalle[obj.listaDetalle.Count - 1].PrecioFinalTransfer = ObtenerPrecioFinalTransfer(pClientes, obj, obj.listaDetalle[obj.listaDetalle.Count - 1]);
                                obj.listaDetalle[obj.listaDetalle.Count - 1].listaSucursalStocks = ActualizarStockListaProductos_Transfer(pClientes, pListaSucursal, item["pro_codigo"].ToString(), tempListaSucursalStocks);
                            }
                        }
                        resultado.Add(obj);
                    }
                }

            }

            return resultado;
        }

        public static List<cTransfer> RecuperarTodosTransfer()
        {
            List<cTransfer> resultado = null;
            DataSet dsResultado = capaTransfer_base.RecuperarTodosTransfer();
            if (dsResultado != null)
            {
                resultado = new List<cTransfer>();
                DataTable tbTransfer = dsResultado.Tables[0];
                for (int i = 0; i < tbTransfer.Rows.Count; i++)
                {
                    cTransfer obj = DKbase.web.acceso.ConvertToTransfer(tbTransfer.Rows[i]);
                    resultado.Add(obj);
                }
            }
            return resultado;
        }
        public static cTransfer RecuperarTransferMasDetallePorIdTransfer(int pIdTransfer, cClientes pClientes, List<string> pListaSucursal)
        {
            cTransfer resultado = null;
            DataSet dsResultado = capaTransfer_base.RecuperarTransferMasDetallePorIdTransfer(pClientes.cli_codsuc, pIdTransfer, pClientes.cli_codigo);
            if (dsResultado != null && dsResultado.Tables[0].Rows.Count > 0)
            {
                resultado = new cTransfer();
                DataTable tbTransfer = dsResultado.Tables[0];
                DataTable tbTransferDetalle = dsResultado.Tables[1];
                DataTable tbSucursalStocks = dsResultado.Tables[2];

                for (int i = 0; i < tbTransfer.Rows.Count; i++)
                {
                    resultado = DKbase.web.acceso.ConvertToTransfer(tbTransfer.Rows[i]);
                    resultado.listaDetalle = new List<cTransferDetalle>();
                    DataRow[] listaFila = tbTransferDetalle.Select("tde_codtfr =" + resultado.tfr_codigo);
                    foreach (DataRow item in listaFila)
                    {
                        List<cSucursalStocks> tempListaSucursalStocks = new List<cSucursalStocks>();
                        tempListaSucursalStocks = (from r in tbSucursalStocks.Select("stk_codpro = '" + item["pro_codigo"].ToString() + "'").AsEnumerable()
                                                   select new cSucursalStocks { stk_codpro = r["stk_codpro"].ToString(), stk_codsuc = r["stk_codsuc"].ToString(), stk_stock = r["stk_stock"].ToString() }).ToList();
                        if (tempListaSucursalStocks.Count > 0)
                        {
                            resultado.listaDetalle.Add(DKbase.web.acceso.ConvertToTransferDetalle(item));
                            resultado.listaDetalle[resultado.listaDetalle.Count - 1].PrecioFinalTransfer = ObtenerPrecioFinalTransfer(pClientes, resultado, resultado.listaDetalle[resultado.listaDetalle.Count - 1]);
                            resultado.listaDetalle[resultado.listaDetalle.Count - 1].listaSucursalStocks = ActualizarStockListaProductos_Transfer(pClientes, pListaSucursal, item["pro_codigo"].ToString(), tempListaSucursalStocks);

                        }
                    }
                }
            }
            return resultado;
        }
        public static List<cHistorialArchivoSubir> RecuperarHistorialSubirArchivo(int pIdCliente)
        {
            List<cHistorialArchivoSubir> resultado = null;
            resultado = new List<cHistorialArchivoSubir>();
            DataTable tabla = capaLogRegistro_base.RecuperarHistorialSubirArchivo(pIdCliente);
            if (tabla != null)
            {
                foreach (DataRow item in tabla.Rows)
                {
                    resultado.Add(DKbase.web.capaDatos.capaLogRegistro_base.ConvertToHistorialArchivoSubir(item));
                }

            }
            return resultado;
        }
        public static cjSonBuscadorProductos RecuperarProductosGeneralSubirPedidos(cClientes pClientes, string pSucursalEleginda, List<string> pListaSucursal, List<cProductosGenerico> pListaProveedor)
        {
            cjSonBuscadorProductos resultado = null;
            List<cProductosGenerico> listaProductosBuscador = ActualizarStockListaProductos_SubirArchico(pClientes, pListaSucursal, pListaProveedor, pSucursalEleginda);
            cjSonBuscadorProductos ResultadoObj = new cjSonBuscadorProductos();
            ResultadoObj.listaSucursal = pListaSucursal;
            ResultadoObj.listaProductos = listaProductosBuscador;
            resultado = ResultadoObj;
            return resultado;
        }
        public static List<cProductosGenerico> ActualizarStockListaProductos_SubirArchico(cClientes pClientes, List<string> pListaSucursal, List<cProductosGenerico> pListaProductos, string pSucursalElegida)
        {
            return DKbase.web.FuncionesPersonalizadas_base.ActualizarStockListaProductos_SubirArchico(pClientes, pListaSucursal, pListaProductos, pSucursalElegida);
        }
        public static bool AgregarHistorialSubirArchivo(int pIdCliente, string pSucursal, string pNombreArchivo, string pNombreArchivoOriginal, DateTime pFecha)
        {
            return capaLogRegistro_base.AgregarHistorialSubirArchivo(pIdCliente, pNombreArchivo, pNombreArchivoOriginal, pSucursal, pFecha);
        }
        public static List<cHistorialArchivoSubir> RecuperarHistorialSubirArchivoPorNombreArchivoOriginal(string pNombreArchivoOriginal)
        {
            List<cHistorialArchivoSubir> resultado = null;
            DataTable tabla = capaLogRegistro_base.RecuperarHistorialSubirArchivoPorNombreArchivoOriginal(pNombreArchivoOriginal);
            if (tabla != null)
            {
                resultado = new List<cHistorialArchivoSubir>();
                foreach (DataRow item in tabla.Rows)
                {
                    resultado.Add(DKbase.web.capaDatos.capaLogRegistro_base.ConvertToHistorialArchivoSubir(item));
                }
            }
            return resultado;
        }
        public static cHistorialArchivoSubir RecuperarHistorialSubirArchivoPorId(int pId)
        {
            cHistorialArchivoSubir resultado = null;
            DataTable tabla = capaLogRegistro_base.RecuperarHistorialSubirArchivoPorId(pId);
            if (tabla != null)
            {
                if (tabla.Rows.Count > 0)
                {
                    resultado = DKbase.web.capaDatos.capaLogRegistro_base.ConvertToHistorialArchivoSubir(tabla.Rows[0]);

                }
            }
            return resultado;
        }
        public static cRangoFecha_Pedidos ObtenerRangoFecha_pedidos(cClientes pCliente, int pDia)
        {
            List<string> lista = new List<string>();
            DateTime fechaDesdeAUX = DateTime.Now.AddDays(pDia * -1);
            DateTime fechaDesde = new DateTime(fechaDesdeAUX.Year, fechaDesdeAUX.Month, fechaDesdeAUX.Day, 0, 0, 0);
            DateTime fechaHasta = DateTime.Now.AddDays(1);
            lista.Add(fechaDesde.Day.ToString());
            lista.Add((fechaDesde.Month).ToString());
            lista.Add((fechaDesde.Year).ToString());

            lista.Add(fechaHasta.Day.ToString());
            lista.Add((fechaHasta.Month).ToString());
            lista.Add((fechaHasta.Year).ToString());

            List<DKbase.dll.cDllPedido> resultadoObj = capaCAR_WebService_base.ObtenerPedidosEntreFechas(pCliente.cli_login, fechaDesde, fechaHasta);

            return new cRangoFecha_Pedidos() { lista = lista, resultadoObj = resultadoObj };
        }
        public static List<cFaltantesConProblemasCrediticiosPadre> ConvertDataTableAClase(DataTable tabla, DataTable tablaSucursalStocks, List<cTransferDetalle> listaTransferDetalle, cClientes pCliente)
        {
            List<cFaltantesConProblemasCrediticiosPadre> resultado = null;
            if (tabla != null)
            {
                resultado = new List<cFaltantesConProblemasCrediticiosPadre>();
                var resultadoTemporal = (from item in tabla.AsEnumerable()
                                         select new { fpc_tipo = item.Field<int>("fpc_tipo"), fpc_codSucursal = item.Field<string>("fpc_codSucursal"), suc_nombre = item.IsNull("suc_nombre") ? item.Field<string>("fpc_codSucursal") : item.Field<string>("suc_nombre") }).Distinct().ToList();

                var resultadoTemporalDistinct = (from t in resultadoTemporal select new { fpc_tipo = t.fpc_tipo, fpc_codSucursal = t.fpc_codSucursal, suc_nombre = t.suc_nombre }).Distinct().ToList();
                for (int i = 0; i < resultadoTemporalDistinct.Count; i++)
                {
                    cFaltantesConProblemasCrediticiosPadre obj = new cFaltantesConProblemasCrediticiosPadre();
                    obj.fpc_codSucursal = resultadoTemporalDistinct[i].fpc_codSucursal;
                    obj.suc_nombre = resultadoTemporalDistinct[i].suc_nombre;
                    obj.fpc_tipo = resultadoTemporalDistinct[i].fpc_tipo;
                    obj.listaProductos = DKbase.web.acceso.cargarProductosBuscadorArchivos(pCliente, tabla.AsEnumerable().Where(xRow => xRow.Field<string>("fpc_codSucursal") == obj.fpc_codSucursal).CopyToDataTable(), tablaSucursalStocks, listaTransferDetalle, DKbase.generales.Constantes.CargarProductosBuscador.isRecuperadorFaltaCredito, obj.fpc_codSucursal);
                    resultado.Add(obj);
                }
            }
            return resultado;
        }
        public static List<cFaltantesConProblemasCrediticiosPadre> RecuperarFaltasProblemasCrediticios_TodosEstados(cClientes pCliente, int fpc_tipo, int pCantidadDia, string pSucursal)
        {
            List<cFaltantesConProblemasCrediticiosPadre> resultado = null;
            DataSet dsResultado = capaLogRegistro_base.RecuperarFaltasProblemasCrediticios_TodosEstados(pCliente.cli_codigo, fpc_tipo, pCantidadDia, pSucursal);
            List<cTransferDetalle> listaTransferDetalle = new List<cTransferDetalle>();
            DataTable tablaTransferDetalle = dsResultado.Tables[1];
            foreach (DataRow itemTransferDetalle in tablaTransferDetalle.Rows)
            {
                cTransferDetalle objTransferDetalle = DKbase.web.acceso.ConvertToTransferDetalle(itemTransferDetalle);
                objTransferDetalle.CargarTransfer(DKbase.web.acceso.ConvertToTransfer(itemTransferDetalle));
                listaTransferDetalle.Add(objTransferDetalle);
            }
            resultado = DKbase.Util.ConvertDataTableAClase(dsResultado.Tables[0], dsResultado.Tables[2], listaTransferDetalle, pCliente);
            return resultado;
        }
        public static List<cFaltantesConProblemasCrediticiosPadre> RecuperarFaltasProblemasCrediticios(cClientes pCliente, int fpc_tipo, int pCantidadDia, string pSucursal)
        {
            List<cFaltantesConProblemasCrediticiosPadre> resultado = null;
            DataSet dsResultado = capaLogRegistro_base.RecuperarFaltasProblemasCrediticios(pCliente.cli_codigo, fpc_tipo, pCantidadDia, pSucursal);
            List<cTransferDetalle> listaTransferDetalle = new List<cTransferDetalle>();
            DataTable tablaTransferDetalle = dsResultado.Tables[1];
            foreach (DataRow itemTransferDetalle in tablaTransferDetalle.Rows)
            {
                cTransferDetalle objTransferDetalle = DKbase.web.acceso.ConvertToTransferDetalle(itemTransferDetalle);
                objTransferDetalle.CargarTransfer(DKbase.web.acceso.ConvertToTransfer(itemTransferDetalle));
                listaTransferDetalle.Add(objTransferDetalle);
            }

            resultado = DKbase.Util.ConvertDataTableAClase(dsResultado.Tables[0], dsResultado.Tables[2], listaTransferDetalle, pCliente);
            return resultado;
        }
        public static List<cOfertaHome> RecuperarTodasOfertaParaHome()
        {
            List<cOfertaHome> resultado = null;
            DataTable tabla = capaHome_base.RecuperarTodasOfertaParaHome();
            if (tabla != null)
            {
                resultado = new List<cOfertaHome>();
                foreach (DataRow fila in tabla.Rows)
                {
                    cOfertaHome o = DKbase.web.capaDatos.capaHome_base.ConvertTocOfertaHome(fila);
                    resultado.Add(DKbase.web.capaDatos.capaHome_base.ConvertAddOferta(fila, o));
                }
            }
            return resultado;
        }
        public static List<cHomeSlide> RecuperarTodasHomeSlidePublicar()
        {
            List<cHomeSlide> resultado = null;
            DataTable tabla = capaHome_base.RecuperarTodasHomeSlidePublicar();
            if (tabla != null)
            {
                resultado = new List<cHomeSlide>();
                foreach (DataRow fila in tabla.Rows)
                {
                    resultado.Add(DKbase.web.capaDatos.capaHome_base.ConvertToHomeSlide(fila));
                }
            }
            return resultado;
        }
        public static List<cRecall> RecuperarTodaReCall()
        {
            List<cRecall> resultado = null;

            DataTable tb = DKbase.web.capaDatos.capaHome_base.RecuperarTodaReCall_aux();
            if (tb != null)
            {
                resultado = new List<cRecall>();
                foreach (DataRow item in tb.Rows)
                {
                    resultado.Add(DKbase.web.capaDatos.capaHome_base.ConvertToRecall(item));
                }
            }
            return resultado;
        }
        public static cOferta RecuperarOfertaPorId(int pIdOferta)
        {
            cOferta resultado = null;
            DataTable tabla = capaHome_base.RecuperarOfertaPorId(pIdOferta);
            if (tabla != null && tabla.Rows.Count > 0)
                resultado = DKbase.web.capaDatos.capaHome_base.ConvertToOferta(tabla.Rows[0]);
            return resultado;
        }
        public static bool? SubirCountadorHomeSlideRating(int hsl_idHomeSlide)
        {
            bool? resultado = null;
            try
            {
                //tbl_HomeSlide o = null;
                //KellerhoffEntities ctx = new KellerhoffEntities();
                //o = ctx.tbl_HomeSlide.FirstOrDefault(x => x.hsl_idHomeSlide == hsl_idHomeSlide);
                //o.hsl_RatingCount = o.hsl_RatingCount + 1;
                //ctx.SaveChanges();
                resultado = true;
            }
            catch (Exception ex)
            { generales.Log.LogError(System.Reflection.MethodBase.GetCurrentMethod(), ex, DateTime.Now); return null; }
            return resultado;
        }
        public static bool? InsertarOfertaRating(int ofr_idOferta, int ofr_idCliente, bool ofr_isDesdeHome)
        {
            try
            {
                capaHome_base.spOferta_Rating(ofr_idCliente, ofr_idOferta, ofr_isDesdeHome);
                return true;
            }
            catch (Exception ex)
            {
                generales.Log.LogError(System.Reflection.MethodBase.GetCurrentMethod(), ex, DateTime.Now, ofr_idCliente, ofr_idOferta, ofr_isDesdeHome);
                return null;
            }
        }
        public static List<cCatalogo> RecuperarTodosCatalogos()
        {
            List<cCatalogo> resultado = null;
            DataTable tabla = capaCatalogo_base.RecuperarTodosCatalogos();
            if (tabla != null)
            {
                resultado = new List<cCatalogo>();
                foreach (DataRow fila in tabla.Rows)
                {
                    resultado.Add(DKbase.web.capaDatos.capaCatalogo_base.ConvertToCatalogo(fila));
                }
            }
            return resultado;
        }
        public static cCatalogo RecuperarTodoCatalogo_PublicarHome()
        {
            cCatalogo resultado = null;
            DataTable tabla = capaCatalogo_base.RecuperarTodoCatalogo_PublicarHome();
            if (tabla != null && tabla.Rows.Count > 0)
            {
                resultado = DKbase.web.capaDatos.capaCatalogo_base.ConvertToCatalogo(tabla.Rows[0]);
            }
            return resultado;
        }

        public static string verRutaArchivo()
        {
            return System.IO.Path.Combine(Helper.getFolder, "archivos", "ofertas", "aIMG_20210603_081731660.jpg");
        }
        public static string verExisteArchivo()
        {
            return System.IO.File.Exists(verRutaArchivo()) ? "Si" : "No";
        }
        public static string verCurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }
        public static int InsertarActualizarArchivo(int arc_codRecurso, int arc_codRelacion, string arc_galeria, string arc_tipo, string arc_mime, string arc_nombre, string arc_titulo, string arc_descripcion, string arc_hash, int arc_codUsuarioUltMov)
        {
            string accion = arc_codRecurso == 0 ? Constantes.cSQL_INSERT : Constantes.cSQL_UPDATE;
            int codigoAccion = arc_codRecurso == 0 ? Constantes.cACCION_ALTA : Constantes.cACCION_MODIFICACION;
            int? codigoEstado = arc_codRecurso == 0 ? Constantes.cESTADO_ACTIVO : (int?)null;
            DataSet dsResultado = DKbase.web.capaDatos.capaRecurso_base.GestiónArchivo(arc_codRecurso, arc_codRelacion, arc_galeria, arc_tipo, arc_mime, arc_nombre, arc_titulo, arc_descripcion, arc_hash, arc_codUsuarioUltMov, codigoEstado, codigoAccion, null, accion);
            int resultado = -1;
            if (arc_codRecurso == 0)
            {
                if (dsResultado != null)
                {
                    if (dsResultado.Tables["Archivo"].Rows[0]["arc_codRecurso"] != DBNull.Value)
                    {
                        resultado = Convert.ToInt32(dsResultado.Tables["Archivo"].Rows[0]["arc_codRecurso"]);
                    }
                }
            }
            else
            {
                resultado = arc_codRecurso;
            }
            return resultado;
        }
        public static int InsertarCurriculumVitae(string tcv_nombre, string tcv_comentario, string tcv_mail, string tcv_dni, string tcv_puesto, string tcv_sucursal, DateTime? tcv_fechaPresentacion)
        {
            int resultado = 0;
            string accion = Constantes.cSQL_INSERT;
            int codigoEstado = Constantes.cESTADO_SINLEER;
            DataSet dsResultado = capaCV_base.GestiónCurriculumVitae(null, DateTime.Now, tcv_nombre, tcv_comentario, tcv_mail, tcv_dni, codigoEstado, null, accion, tcv_puesto, tcv_sucursal, tcv_fechaPresentacion);
            if (dsResultado != null)
            {
                if (dsResultado.Tables.Contains("CurriculumVitae"))
                {
                    if (dsResultado.Tables["CurriculumVitae"].Rows.Count > 0)
                    {
                        if (dsResultado.Tables["CurriculumVitae"].Rows[0]["tcv_codCV"] != DBNull.Value)
                        {
                            resultado = Convert.ToInt32(dsResultado.Tables["CurriculumVitae"].Rows[0]["tcv_codCV"]);
                            DKbase.web.generales.cMail_base.enviarMail(DKbase.Helper.getMail_cv, "Nuevo Curriculum Vitae", capaCV_base.GenerarHtmlCuerpoMail(tcv_nombre, tcv_comentario, tcv_mail));
                        }
                    }
                }
            }
            return resultado;
        }
        public static List<cCatalogo> getCatalogosParaDescarga()
        {
            List<cCatalogo> listaSession = null;
            List<cCatalogo> lista = DKbase.Util.RecuperarTodosCatalogos().Where(x => x.tbc_estado == Constantes.cESTADO_ACTIVO).OrderByDescending(x => x.tbc_orden).ToList();
            if (lista != null)
            {
                listaSession = new List<cCatalogo>();
                for (int i = 0; i < lista.Count; i++)
                {
                    if (lista[i].tbc_estado == Constantes.cESTADO_ACTIVO)
                    {
                        lista[i].tbc_descripcion = string.Empty;
                        List<cArchivo> listaArchivo = DKbase.Util.RecuperarTodosArchivos(lista[i].tbc_codigo, Constantes.cTABLA_CATALOGO, string.Empty);
                        if (listaArchivo != null)
                        {
                            if (listaArchivo.Count > 0)
                            {
                                if (listaArchivo[0].arc_estado == Constantes.cESTADO_ACTIVO)
                                {
                                    lista[i].tbc_descripcion = listaArchivo[0].arc_nombre;
                                    listaSession.Add(lista[i]);
                                }
                            }
                        }
                    }
                }
            }
            return listaSession;
        }
        public static void GenerarArchivo(string RutaNombreArchivo, DataTable pTabla)
        {
            if (pTabla != null && RutaNombreArchivo != null)
            {
                if (pTabla.Rows.Count > 0)
                {
                    string path = RutaNombreArchivo;
                    FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    //StreamWriter writer = new StreamWriter(stream);
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        string encabezado = string.Empty;
                        encabezado += "Tipo" + Constantes.cSeparadorCSV;
                        encabezado += "Producto" + Constantes.cSeparadorCSV;
                        encabezado += "AlfaBeta" + Constantes.cSeparadorCSV;
                        encabezado += "Troquel" + Constantes.cSeparadorCSV;
                        encabezado += "CodBarraPrinc" + Constantes.cSeparadorCSV;
                        encabezado += "Laboratorio" + Constantes.cSeparadorCSV;
                        encabezado += "Precio" + Constantes.cSeparadorCSV;
                        encabezado += "Neto" + Constantes.cSeparadorCSV;
                        encabezado += "CadenaFrio" + Constantes.cSeparadorCSV;
                        encabezado += "RequiereVale" + Constantes.cSeparadorCSV;
                        encabezado += "Trazable";

                        writer.WriteLine(encabezado);
                        //resultado += "\n";
                        foreach (DataRow item in pTabla.Rows)
                        {
                            string fila = string.Empty;
                            string Tipo = string.Empty;
                            if (item["Tipo"] != DBNull.Value)
                            {
                                Tipo = item["Tipo"].ToString();
                            }
                            fila += Tipo + Constantes.cSeparadorCSV;
                            string Producto = string.Empty;
                            if (item["Producto"] != DBNull.Value)
                            {
                                Producto = item["Producto"].ToString();
                            }
                            fila += Producto + Constantes.cSeparadorCSV;
                            string AlfaBeta = string.Empty;
                            if (item["AlfaBeta"] != DBNull.Value)
                            {
                                AlfaBeta = item["AlfaBeta"].ToString();
                            }
                            fila += AlfaBeta + Constantes.cSeparadorCSV;
                            string Troquel = string.Empty;
                            if (item["Troquel"] != DBNull.Value)
                            {
                                Troquel = item["Troquel"].ToString();
                            }
                            fila += Troquel + Constantes.cSeparadorCSV;
                            string CodBarraPrinc = string.Empty;
                            if (item["CodBarraPrinc"] != DBNull.Value)
                            {
                                CodBarraPrinc = item["CodBarraPrinc"].ToString();
                            }
                            fila += CodBarraPrinc + Constantes.cSeparadorCSV;
                            string Laboratorio = string.Empty;
                            if (item["Laboratorio"] != DBNull.Value)
                            {
                                Laboratorio = item["Laboratorio"].ToString();
                            }
                            fila += Laboratorio + Constantes.cSeparadorCSV;
                            string Precio = string.Empty;
                            if (item["Precio"] != DBNull.Value)
                            {
                                Precio = item["Precio"].ToString();
                            }
                            fila += Precio + Constantes.cSeparadorCSV;
                            string Neto = string.Empty;
                            if (item["Neto"] != DBNull.Value)
                            {
                                Neto = item["Neto"].ToString();
                            }
                            fila += Neto + Constantes.cSeparadorCSV;
                            string CadenaFrio = string.Empty;
                            if (item["CadenaFrio"] != DBNull.Value)
                            {
                                CadenaFrio = item["CadenaFrio"].ToString();
                            }
                            fila += CadenaFrio + Constantes.cSeparadorCSV;
                            string RequiereVale = string.Empty;
                            if (item.Table.Columns.Contains("RequiereVale") && item["RequiereVale"] != DBNull.Value)
                            {
                                RequiereVale = item["RequiereVale"].ToString();
                            }
                            if (item.Table.Columns.Contains("pro_requierevale") && item["pro_requierevale"] != DBNull.Value)
                            {
                                RequiereVale = item["pro_requierevale"].ToString();
                            }
                            fila += RequiereVale + Constantes.cSeparadorCSV;
                            string Trazable = string.Empty;
                            if (item["Trazable"] != DBNull.Value)
                            {
                                Trazable = item["Trazable"].ToString();
                            }
                            fila += Trazable;
                            writer.WriteLine(fila);
                        }
                    }
                }
            }
        }
        public static void GenerarArchivo_ProductosEnOferta(string RutaNombreArchivo, DataTable pTabla)
        {
            if (pTabla != null && RutaNombreArchivo != null)
            {
                if (pTabla.Rows.Count > 0)
                {
                    string path = RutaNombreArchivo;
                    FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    //StreamWriter writer = new StreamWriter(stream);
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        string encabezado = string.Empty;
                        encabezado += "Nombre producto" + Constantes.cSeparadorCSV;
                        encabezado += "Codigo Barra" + Constantes.cSeparadorCSV;
                        encabezado += "Unidades Mínimas" + Constantes.cSeparadorCSV;
                        encabezado += "% de descuento";
                        writer.WriteLine(encabezado);
                        foreach (DataRow item in pTabla.Rows)
                        {
                            string fila = string.Empty;
                            string NombreProducto = string.Empty;
                            if (item["Nombre producto"] != DBNull.Value)
                            {
                                NombreProducto = item["Nombre producto"].ToString();
                            }
                            fila += NombreProducto + Constantes.cSeparadorCSV;
                            string CodigoBarra = string.Empty;
                            if (item["Codigo Barra"] != DBNull.Value)
                            {
                                CodigoBarra = item["Codigo Barra"].ToString();
                            }
                            fila += CodigoBarra + Constantes.cSeparadorCSV;
                            string UnidadesMínimas = string.Empty;
                            if (item["Unidades Mínimas"] != DBNull.Value)
                            {
                                UnidadesMínimas = item["Unidades Mínimas"].ToString();
                            }
                            fila += UnidadesMínimas + Constantes.cSeparadorCSV;
                            string Descuento = string.Empty;
                            if (item["% de descuento"] != DBNull.Value)
                            {
                                Descuento = item["% de descuento"].ToString();
                            }
                            fila += Descuento;
                            writer.WriteLine(fila);
                        }
                    }
                }
            }
        }
        public static void GenerarArchivo_MedicamentosYAccesoriosNoIncluidosEnAlfaBeta(string RutaNombreArchivo, DataTable pTabla)
        {
            if (pTabla != null && RutaNombreArchivo != null)
            {
                if (pTabla.Rows.Count > 0)
                {
                    string path = RutaNombreArchivo;
                    FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    //StreamWriter writer = new StreamWriter(stream);
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        string encabezado = string.Empty;
                        encabezado += "Tipo" + Constantes.cSeparadorCSV;
                        encabezado += "Producto" + Constantes.cSeparadorCSV;
                        encabezado += "AlfaBeta" + Constantes.cSeparadorCSV;
                        encabezado += "Troquel" + Constantes.cSeparadorCSV;
                        encabezado += "CodBarraPrinc" + Constantes.cSeparadorCSV;
                        encabezado += "Laboratorio" + Constantes.cSeparadorCSV;
                        encabezado += "Precio" + Constantes.cSeparadorCSV;
                        encabezado += "Neto" + Constantes.cSeparadorCSV;
                        encabezado += "CadenaFrio" + Constantes.cSeparadorCSV;
                        encabezado += "RequiereVale" + Constantes.cSeparadorCSV;
                        encabezado += "Trazable";
                        writer.WriteLine(encabezado);
                        foreach (DataRow item in pTabla.Rows)
                        {
                            string fila = string.Empty;
                            fila += item["Tipo"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["Producto"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["AlfaBeta"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["Troquel"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["CodBarraPrinc"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["Laboratorio"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["Precio"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["Neto"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["CadenaFrio"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["RequiereVale"].ToString() + Constantes.cSeparadorCSV;
                            fila += item["Trazable"].ToString();

                            writer.WriteLine(fila);
                        }
                    }
                    //writer.Close();
                }
            }
        }
        public static void createFileClear(String pPathFile, DataTable pDataTable)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(pDataTable);
            ds.WriteXml(pPathFile);
        }
        public static string GenerateDocument_getNameFile(int id)
        {
            string nameFile = string.Empty;
            switch (id)
            {
                case 1:
                case 5:
                    if (id == 1)
                        nameFile = "Productos.xls";
                    else
                        nameFile = "Productos.csv";
                    break;
                case 2:
                case 6:
                    if (id == 2)
                        nameFile = "ProductosDrogueria.xls";
                    else
                        nameFile = "ProductosDrogueria.csv";
                    break;
                case 3:
                case 7:
                    if (id == 3)
                        nameFile = "ProductosPerfumeria.xls";
                    else
                        nameFile = "ProductosPerfumeria.csv";
                    break;
                case 4:
                case 8:
                    if (id == 4)
                        nameFile = "ProductosEnOferta.xls";
                    else
                        nameFile = "ProductosEnOferta.csv";
                    break;
                case 9:
                    nameFile = "MedicamentosYAccesoriosNoIncluidosEnAlfaBeta.csv";
                    break;
                default:
                    break;
            }
            return nameFile;
        }
        public static string GenerateDocument_getPathFile(int id)
        {
            string nameFile = GenerateDocument_getNameFile(id);
            string path = Path.Combine(DKbase.Helper.getFolder, "temp");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            String f = Path.Combine(path, nameFile);
            return f;
        }
        public static byte[] GenerateDocument(int id, DKbase.web.capaDatos.cClientes pCliente)
        {
            if (pCliente == null)
                return null;

            try
            {
                DataTable dt = null;
                string nameFile = GenerateDocument_getNameFile(id);
                switch (id)
                {
                    case 1:
                    case 5:
                        dt = capaProductos_base.DescargaTodosProductos(pCliente.cli_codprov);
                        //if (id == 1)
                        //    nameFile = "Productos.xls";
                        //else
                        //    nameFile = "Productos.csv";
                        break;
                    case 2:
                    case 6:
                        dt = capaProductos_base.DescargaTodosProductosDrogueria(pCliente.cli_codprov);
                        //if (id == 2)
                        //    nameFile = "ProductosDrogueria.xls";
                        //else
                        //    nameFile = "ProductosDrogueria.csv";
                        break;
                    case 3:
                    case 7:
                        dt = capaProductos_base.DescargaTodosProductosPerfumeria(pCliente.cli_codprov);
                        //if (id == 3)
                        //    nameFile = "ProductosPerfumeria.xls";
                        //else
                        //    nameFile = "ProductosPerfumeria.csv";
                        break;
                    case 4:
                    case 8:
                        dt = capaProductos_base.DescargaTodosProductosEnOferta();
                        //if (id == 4)
                        //    nameFile = "ProductosEnOferta.xls";
                        //else
                        //    nameFile = "ProductosEnOferta.csv";
                        break;
                    case 9:
                        dt = capaProductos_base.DescargaMedicamentosYAccesoriosNoIncluidosEnAlfaBeta();
                        //nameFile = "MedicamentosYAccesoriosNoIncluidosEnAlfaBeta.csv";
                        break;
                    default:
                        break;
                }
                if (dt != null)
                {
                    // string path = Path.Combine(DKbase.Helper.getFolder, "temp");
                    // if (!Directory.Exists(path))
                    //    Directory.CreateDirectory(path);
                    //String f = Path.Combine(path, nameFile);
                    String f = GenerateDocument_getPathFile(id);
                    if (id == 1 || id == 2 || id == 3 || id == 4)
                    {
                        DKbase.Util.createFileClear(f, dt);
                    }
                    else if (id == 5 || id == 6 || id == 7)
                    {
                        DKbase.Util.GenerarArchivo(f, dt);
                    }
                    else if (id == 8)
                    {
                        DKbase.Util.GenerarArchivo_ProductosEnOferta(f, dt);
                    }
                    else if (id == 9)
                    {
                        DKbase.Util.GenerarArchivo_MedicamentosYAccesoriosNoIncluidosEnAlfaBeta(f, dt);
                    }
                    var fileStream = new FileStream(f, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(System.Reflection.MethodBase.GetCurrentMethod(), ex, DateTime.Now, id);

            }
            return null;
        }
        public static List<cUsuario> RecuperarUsuariosDeCliente(int usu_codRol, int usu_codCliente, string filtro)
        {
            List<cUsuario> lista = new List<cUsuario>();
            DataSet dsResultado = capaClientes_base.RecuperarUsuariosDeCliente(usu_codRol, usu_codCliente, filtro);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["UsuariosCliente"].Rows)
                {
                    cUsuario o = capaSeguridad_base.ConvertToUsuario(item);
                    o.listaPermisoDenegados = DKbase.Util.RecuperarSinPermisosSecciones(o.usu_codigo);
                    lista.Add(o);
                }
            }
            return lista;

        }
        public static List<cUsuario> GetUsuariosDeCliente(string sortExpression, int pIdCliente, string pFiltro)
        {
            ordenamientoExpresion order = new ordenamientoExpresion(sortExpression);
            string filtro = string.Empty;
            if (pFiltro != null)
            {
                filtro = pFiltro;
            }
            var query = RecuperarUsuariosDeCliente(Constantes.cROL_OPERADORCLIENTE, pIdCliente, filtro);
            if (order.isOrderBy)
            {
                if (order.OrderByAsc)
                {
                    switch (order.OrderByField)
                    {
                        case "usu_nombre":
                            query = query.OrderBy(b => b.usu_nombre).ToList();
                            break;
                        case "usu_apellido":
                            query = query.OrderBy(b => b.usu_apellido).ToList();
                            break;
                        case "usu_mail":
                            query = query.OrderBy(b => b.usu_mail).ToList();
                            break;
                        case "usu_login":
                            query = query.OrderBy(b => b.usu_login).ToList();
                            break;
                        case "NombreYapellido":
                            query = query.OrderBy(b => b.NombreYapellido).ToList();
                            break;
                        case "rol_Nombre":
                            query = query.OrderBy(b => b.rol_Nombre).ToList();
                            break;
                        case "usu_estadoToString":
                            query = query.OrderBy(b => b.usu_estadoToString).ToList();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (order.OrderByField)
                    {
                        case "usu_nombre":
                            query = query.OrderByDescending(b => b.usu_nombre).ToList();
                            break;
                        case "usu_apellido":
                            query = query.OrderByDescending(b => b.usu_apellido).ToList();
                            break;
                        case "usu_mail":
                            query = query.OrderByDescending(b => b.usu_mail).ToList();
                            break;
                        case "usu_login":
                            query = query.OrderByDescending(b => b.usu_login).ToList();
                            break;
                        case "NombreYapellido":
                            query = query.OrderByDescending(b => b.NombreYapellido).ToList();
                            break;
                        case "rol_Nombre":
                            query = query.OrderByDescending(b => b.rol_Nombre).ToList();
                            break;
                        case "usu_estadoToString":
                            query = query.OrderByDescending(b => b.usu_estadoToString).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }
            return query;
        }
        public static bool IsRepetidoLogin(int pIdUsuario, string pLogin)
        {
            bool resultado = false;
            DataTable dtResultado = capaSeguridad_base.IsRepetidoLogin(pIdUsuario, pLogin);
            if (dtResultado != null)
            {
                if (dtResultado.Rows.Count > 0)
                {
                    resultado = true;
                }
            }
            return resultado;
        }
        public static int InsertarActualizarUsuario(int usu_codigo, int usu_codRol, int? usu_codCliente, string usu_nombre, string usu_apellido, string usu_mail, string usu_login, string usu_psw, string usu_observacion, int? usu_codUsuarioUltMov)
        {
            string accion = usu_codigo == 0 ? Constantes.cSQL_INSERT : Constantes.cSQL_UPDATE;
            int codigoAccion = usu_codigo == 0 ? Constantes.cACCION_ALTA : Constantes.cACCION_MODIFICACION;
            int? codigoEstado = usu_codigo == 0 ? Constantes.cESTADO_ACTIVO : (int?)null;
            DataSet dsResultado = capaSeguridad_base.GestiónUsuario(usu_codigo, usu_codRol, usu_codCliente, usu_nombre, usu_apellido, usu_mail, usu_login, usu_psw, usu_observacion, usu_codUsuarioUltMov, codigoAccion, codigoEstado, null, accion);
            int resultado = -1;
            if (usu_codigo == 0)
            {
                if (dsResultado != null)
                {
                    if (dsResultado.Tables["Usuario"].Rows[0]["usu_codigo"] != DBNull.Value)
                    {
                        resultado = Convert.ToInt32(dsResultado.Tables["Usuario"].Rows[0]["usu_codigo"]);
                    }
                }
            }
            else
            {
                resultado = usu_codigo;
            }
            return resultado;
        }
        public static bool InsertarSinPermisoUsuarioIntranetPorIdUsuario(int pIdUsuario, List<string> pListaNombreSeccion)
        {
            bool resultado = false;
            try
            {
                DataTable pTablaDetalle = DKbase.web.FuncionesPersonalizadas_base.ConvertNombresSeccionToDataTable(pListaNombreSeccion);
                capaSeguridad_base.InsertarSinPermisoUsuarioIntranetPorIdUsuario(pIdUsuario, pTablaDetalle);
                resultado = true;
            }
            catch (Exception ex)
            {
                Log.LogError(System.Reflection.MethodBase.GetCurrentMethod(), ex, DateTime.Now);
            }
            return resultado;
        }
        public static int GuardarUsuario(cClientes pCliente, int pIdUsuario, string pNombre, string pApellido, string pMail, string pLogin, string pContraseña, string pObservaciones1, List<string> pListaPermisos)
        {
            if (pCliente == null)
                return -1;
            int? codigoUsuarioEnSession = null;
            if (DKbase.Util.IsRepetidoLogin(pIdUsuario, pLogin))
                return -2;
            int codUsuarioInsertarActualizar = DKbase.Util.InsertarActualizarUsuario(pIdUsuario, Constantes.cROL_OPERADORCLIENTE, pCliente.cli_codigo, pNombre, pApellido, pMail, pLogin, pContraseña, pObservaciones1, codigoUsuarioEnSession);
            DKbase.Util.InsertarSinPermisoUsuarioIntranetPorIdUsuario(codUsuarioInsertarActualizar, pListaPermisos);
            return codUsuarioInsertarActualizar;
        }
        public static cUsuario RecuperarUsuarioPorId(int pIdUsuario)
        {
            cUsuario obj = null;
            DataSet dsResultado = capaSeguridad_base.GestiónUsuario(pIdUsuario, null, null, null, null, null, null, null, null, null, null, null, null, Constantes.cSQL_SELECT);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["Usuario"].Rows)
                {
                    obj = DKbase.web.capaDatos.capaSeguridad_base.ConvertToUsuario(item);
                    obj.listaPermisoDenegados = DKbase.Util.RecuperarSinPermisosSecciones(obj.usu_codigo);
                    break;
                }
            }
            return obj;
        }
        public static void CambiarEstadoUsuarioPorId(int pIdUsuario, int pIdEstado, int pIdUsuarioEnSession)
        {
            capaSeguridad_base.GestiónUsuario(pIdUsuario, null, null, null, null, null, null, null, null, pIdUsuarioEnSession, Constantes.cACCION_CAMBIOESTADO, pIdEstado, null, Constantes.cSQL_ESTADO);
        }
        public static int CambiarEstadoUsuario(Usuario pUsuario, int pIdUsuario)
        {
            if (pUsuario == null)
                return -1;
            int codigoUsuarioEnSession = pUsuario.id;
            cUsuario usuario = DKbase.Util.RecuperarUsuarioPorId(pIdUsuario);
            int estadoUsuario = usuario.usu_estado == Constantes.cESTADO_ACTIVO ? Constantes.cESTADO_INACTIVO : Constantes.cESTADO_ACTIVO;
            DKbase.Util.CambiarEstadoUsuarioPorId(usuario.usu_codigo, estadoUsuario, codigoUsuarioEnSession);
            return 0;
        }
        public static void EliminarUsuario(int usu_codigo)
        {
            DataSet dsResultado = capaSeguridad_base.GestiónUsuario(usu_codigo, null, null, null, null, null, null, null, null, null, null, null, null, Constantes.cSQL_DELETE);
        }
        public static void CambiarContraseñaUsuario(int pIdUsuario, string pConstraseña, int? pIdUsuarioEnSession)
        {
            DataSet dsResultado = capaSeguridad_base.GestiónUsuario(pIdUsuario, null, null, null, null, null, null, pConstraseña, null, pIdUsuarioEnSession, Constantes.cACCION_CAMBIOCONTRASEÑA, null, null, Constantes.cSQL_CAMBIOCONTRASEÑA);
        }
        public static int CambiarContraseñaUsuario(Usuario pUsuario, int pIdUsuario, string pPass)
        {
            if (pUsuario == null)
                return -1;
            int codigoUsuarioEnSession = pUsuario.id;
            DKbase.Util.CambiarContraseñaUsuario(pIdUsuario, pPass, codigoUsuarioEnSession);
            return 0;
        }
        public static int CambiarContraseñaPersonal(cClientes pCliente, Usuario pUsuario, string pContraseñaVieja, string pContraseñaNueva)
        {
            int id = -1;

            if (pCliente != null && pUsuario != null)
            {
                cUsuario objUsuario = null;
                objUsuario = DKbase.Util.RecuperarUsuarioPorId(pUsuario.id);
                if (pContraseñaVieja == objUsuario.usu_pswDesencriptado)
                {
                    id = capaSeguridad_base.CambiarContraseñaPersonal(pUsuario.id, pContraseñaVieja, pContraseñaNueva);
                    if (pUsuario.idRol == Constantes.cROL_ADMINISTRADORCLIENTE)
                    {
                        DKbase.web.capaDatos.capaDLL.ModificarPasswordWEB(pCliente.cli_login, objUsuario.usu_pswDesencriptado, pContraseñaNueva);
                    }
                }
                else { id = 0; }
            }
            return id;
        }
        public static void ImprimirComprobante(string pTipoComprobante, string pNroComprobante)
        {
            DKbase.web.capaDatos.capaDLL.ImprimirComprobante(pTipoComprobante, pNroComprobante);
        }
        public static cFactura ObtenerFactura(string pNroFactura, string pLoginWeb)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerFactura(pNroFactura, pLoginWeb);
        }
        public static cNotaDeCredito ObtenerNotaDeCredito(string pNroNotaDeCredito, string pLoginWeb)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerNotaDeCredito(pNroNotaDeCredito, pLoginWeb);
        }
        public static cNotaDeDebito ObtenerNotaDeDebito(string pNroNotaDeDebito, string pLoginWeb)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerNotaDeDebito(pNroNotaDeDebito, pLoginWeb);
        }
        public static cResumen ObtenerResumenCerrado(string pNroResumen, string pLoginWeb)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerResumenCerrado(pNroResumen, pLoginWeb);
        }
        public static cObraSocialCliente ObtenerObraSocialCliente(string pNumeroObraSocialCliente, string pLoginWeb)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerObraSocialCliente(pNumeroObraSocialCliente, pLoginWeb);
        }
        public static cRecibo ObtenerRecibo(string pNumeroDoc, string pLoginWeb)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerRecibo(pNumeroDoc, pLoginWeb);
        }
        public static string GrabarFacturaTXT(cClientes pClientes, string pRuta, string pFactura)
        {
            string resultado = string.Empty;
            if (pClientes != null && pFactura != null)
            {
                cFactura objFactura = ObtenerFactura(pFactura, pClientes.cli_login);
                if (objFactura != null)
                {
                    string nombreArchivoTXT = string.Empty;
                    string fechaArchivoTXT = string.Empty;
                    if (objFactura.Fecha != null)
                    {
                        fechaArchivoTXT = ((DateTime)objFactura.Fecha).Day.ToString("00") + ((DateTime)objFactura.Fecha).Month.ToString("00") + ((DateTime)objFactura.Fecha).Year.ToString("0000");
                    }
                    else
                    {
                        fechaArchivoTXT = "00" + "00" + "0000";
                    }
                    nombreArchivoTXT = "dk" + fechaArchivoTXT + "-" + pFactura + ".txt";
                    resultado = nombreArchivoTXT;
                    System.IO.StreamWriter FAC_txt = new System.IO.StreamWriter(Path.Combine(pRuta, nombreArchivoTXT), false, System.Text.Encoding.UTF8);
                    //[1] eeeeeeeedd (e - Entero / d - Decimal)
                    //1 número C(13) 
                    //2 fecha N(8) ddmmyyyy 
                    //3 monto total N(10) [1] 
                    //4 monto exento N(10) [1] 
                    //5 monto gravado N(10) [1] 
                    //6 monto IVA inscripto N(10) [1] 
                    //7 monto IVA no inscripto N(10) [1] 
                    //8 monto percepción DGR N(10) [1] 
                    //9 descuento especial N(10) [1] 
                    //10 descuento netos N(10) [1] 
                    //11 descuento perfumería N(10) [1] 
                    //12 descuento web N(10) [1] 
                    //13 Monto Percepcion Municipal N(10) [1] 
                    string strCabeceraFAC = string.Empty;
                    // número C(13) 
                    strCabeceraFAC += objFactura.Numero.PadRight(13, ' ');
                    // Fecha
                    if (objFactura.Fecha != null)
                    {
                        strCabeceraFAC += ((DateTime)objFactura.Fecha).Day.ToString("00") + ((DateTime)objFactura.Fecha).Month.ToString("00") + ((DateTime)objFactura.Fecha).Year.ToString("0000");
                    }
                    else
                    {
                        strCabeceraFAC += "00" + "00" + "0000";
                    }
                    // fin fecha
                    //monto total N(10) [1]        
                    string montoTotal = string.Empty;
                    montoTotal += Numerica.toString_NumeroTXT_N10(objFactura.MontoTotal);
                    strCabeceraFAC += montoTotal;
                    // fin monto total N(10) [1] 

                    //4 monto exento N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.MontoExento);
                    //5 monto gravado N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.MontoGravado);
                    //6 monto IVA inscripto N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.MontoIvaInscripto);
                    //7 monto IVA no inscripto N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.MontoIvaNoInscripto);
                    //8 monto percepción DGR N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.MontoPercepcionDGR);
                    //9 descuento especial N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.DescuentoEspecial);
                    //10 descuento netos N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.DescuentoNetos);
                    //11 descuento perfumería N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.DescuentoPerfumeria);
                    //12 descuento web N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.DescuentoWeb);
                    //13 Monto Percepcion Municipal N(10) [1] 
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.MontoPercepcionMunicipal);
                    //14 Monto Percepcion Municipal N(10) [1]
                    strCabeceraFAC += Numerica.toString_NumeroTXT_N10(objFactura.MontoPercepcionIVA);


                    FAC_txt.WriteLine(strCabeceraFAC);

                    foreach (cFacturaDetalle item in objFactura.lista)
                    {
                        if (item.Troquel != null)
                        {
                            if (item.Troquel != string.Empty)
                            {
                                //If NOT ISNULL(Importe)
                                if (item.Importe != null)
                                {
                                    if (item.Importe.Trim() != string.Empty)
                                    {
                                        string detalleFAC = string.Empty;
                                        //Nro. Campo Tipo Comentario
                                        //1 código de barras producto C(13)
                                        //2 descripción producto C(60)
                                        //3 cantidad N(5)
                                        //4 característica C(1)
                                        //Espacio en blanco - Sin característica
                                        //F - Farmabono
                                        //D - Tarjeta D
                                        //C - Colfacor
                                        //B - Bonos CIL
                                        //P - Bonos PAP
                                        //$ - Ofertas
                                        //T - Transfer
                                        //5 neto N(1) 0 - Normail / 1 - Neto + IVA
                                        //6 precio público N(10) [1]
                                        //7 precio unitario N(10) [1]
                                        //8 importe N(10) [1]   
                                        cProductos producto = capaCAR_WebService_base.RecuperarProductoPorNombre(item.Descripcion);
                                        bool isNoTieneCodigoBarra = true;//código de barras producto C(13)
                                        if (producto != null)
                                        {
                                            if (producto.pro_codigobarra != null)
                                            {
                                                isNoTieneCodigoBarra = false;
                                                detalleFAC += producto.pro_codigobarra.PadRight(13, ' ');
                                            }
                                        }
                                        if (isNoTieneCodigoBarra)
                                        {
                                            detalleFAC += " ".PadRight(13, ' ');
                                        }
                                        detalleFAC += item.Descripcion.PadRight(60, ' ');
                                        detalleFAC += item.Cantidad.PadLeft(5, '0');
                                        if (item.Caracteristica == null)
                                        {
                                            detalleFAC += " ";
                                        }
                                        else
                                        {
                                            if (item.Caracteristica == string.Empty)
                                            {
                                                detalleFAC += " ";
                                            }
                                            else
                                            {
                                                detalleFAC += item.Caracteristica.PadLeft(1, ' ');
                                            }
                                        }
                                        if (producto != null)
                                        {
                                            detalleFAC += producto.pro_neto ? "1" : "0"; // Neto --- neto N(1) 0 - Normail / 1 - Neto + IVA                            
                                        }
                                        else
                                        {
                                            detalleFAC += " ";
                                        }
                                        detalleFAC += Numerica.toString_NumeroTXT_N10(item.PrecioPublico);
                                        detalleFAC += Numerica.toString_NumeroTXT_N10(item.PrecioUnitario);
                                        detalleFAC += Numerica.toString_NumeroTXT_N10(item.Importe);

                                        //resultado += detalleFAC + "\n";
                                        FAC_txt.WriteLine(detalleFAC);
                                        //listaResultado.Add(resultado);

                                    }//   if (item.Importe.Trim() != string.Empty) { 
                                }//     if (item.Importe != null) { 

                            }// fin if (item.Troquel != string.Empty)

                        }// fin if (item.Troquel != null)
                    }
                    FAC_txt.Close();
                }
            }
            return resultado;
        }
        public static string GrabarFacturaCSV(cClientes pClientes, string pRuta, string pFactura)
        {
            string resultado = string.Empty;
            if (pClientes != null && pFactura != null)
            {
                cFactura objFactura = ObtenerFactura(pFactura, pClientes.cli_login);
                List<cPedidoItem> l_PedidoItem = ObtenerItemsDePedidoPorNumeroDeFactura(pFactura, pClientes.cli_login);
                if (objFactura != null)
                {
                    string nombreArchivoTXT = string.Empty;
                    string fechaArchivoTXT = string.Empty;
                    if (objFactura.Fecha != null)
                    {
                        fechaArchivoTXT = ((DateTime)objFactura.Fecha).Day.ToString("00") + ((DateTime)objFactura.Fecha).Month.ToString("00") + ((DateTime)objFactura.Fecha).Year.ToString("0000");
                    }
                    else
                    {
                        fechaArchivoTXT = "00" + "00" + "0000";
                    }
                    nombreArchivoTXT = "dk" + fechaArchivoTXT + "-" + pFactura + ".csv";
                    resultado = nombreArchivoTXT;
                    System.IO.StreamWriter FAC_txt = new System.IO.StreamWriter(Path.Combine(pRuta, nombreArchivoTXT), false, System.Text.Encoding.UTF8);
                    string strCabeceraCSV = string.Empty;

                    strCabeceraCSV += "Fecha";
                    strCabeceraCSV += ";";
                    strCabeceraCSV += "CodBarra";
                    strCabeceraCSV += ";";
                    strCabeceraCSV += "Producto";
                    strCabeceraCSV += ";";
                    strCabeceraCSV += "Cantidad";
                    strCabeceraCSV += ";";
                    strCabeceraCSV += "Precio Público";
                    strCabeceraCSV += ";";
                    strCabeceraCSV += "Precio Unit.";
                    strCabeceraCSV += ";";
                    strCabeceraCSV += "Importe";
                    strCabeceraCSV += ";";
                    strCabeceraCSV += "Faltas";
                    FAC_txt.WriteLine(strCabeceraCSV);
                    foreach (cPedidoItem item in l_PedidoItem)
                    {
                        string detalleCSV = string.Empty;

                        cProductos producto = capaCAR_WebService_base.RecuperarProductoPorNombre(item.NombreObjetoComercial);
                        bool isNoTieneCodigoBarra = true;//código de barras producto C(13)
                        detalleCSV += objFactura.FechaToString;
                        detalleCSV += ";";
                        if (producto != null)
                        {
                            if (producto.pro_codigobarra != null)
                            {
                                isNoTieneCodigoBarra = false;
                                detalleCSV += producto.pro_codigobarra.PadRight(13, ' ');
                                detalleCSV += ";";
                            }
                            if (isNoTieneCodigoBarra)
                            {
                                detalleCSV += " ".PadRight(13, ' ');
                                detalleCSV += ";";
                            }
                            detalleCSV += producto.pro_nombre;
                            detalleCSV += ";";
                            detalleCSV += item.Cantidad.ToString().PadLeft(5, '0');
                            detalleCSV += ";";
                        }

                        detalleCSV += !string.IsNullOrEmpty(item.PrecioPublico) ? Numerica.FormatoNumeroPuntoMilesComaDecimal(Convert.ToDecimal(item.PrecioPublico)) : "";
                        detalleCSV += ";";
                        detalleCSV += !string.IsNullOrEmpty(item.PrecioUnitario) ? Numerica.FormatoNumeroPuntoMilesComaDecimal(Convert.ToDecimal(item.PrecioUnitario)) : "";
                        detalleCSV += ";";
                        detalleCSV += !string.IsNullOrEmpty(item.Importe) ? Numerica.FormatoNumeroPuntoMilesComaDecimal(Convert.ToDecimal(item.Importe)) : ""; //Numerica.toString_NumeroTXT_N10(item.Importe);
                        detalleCSV += ";";
                        detalleCSV += item.Faltas.ToString().PadLeft(5, '0');
                        detalleCSV += ";";
                        FAC_txt.WriteLine(detalleCSV);
                    }
                    FAC_txt.Close();
                }
            }
            return resultado;
        }
        public static string generar_archivo(cClientes pCliente, string factura)
        {
            string result = null;
            string rutaTemporal = Path.Combine(DKbase.Helper.getFolder, "archivos", "facturas");

            DirectoryInfo DIR = new DirectoryInfo(rutaTemporal);
            if (!DIR.Exists)
            {
                DIR.Create();
            }
            string nombreTXT = GrabarFacturaTXT(pCliente, rutaTemporal, factura);
            if (!string.IsNullOrEmpty(nombreTXT))
            {
                result = Path.Combine(rutaTemporal, nombreTXT);
            }
            return result;
        }
        public static string generar_factura_csv(cClientes pCliente, string factura)
        {
            string result = null;
            string rutaTemporal = Path.Combine(DKbase.Helper.getFolder, "archivos", "facturas");

            DirectoryInfo DIR = new DirectoryInfo(rutaTemporal);
            if (!DIR.Exists)
            {
                DIR.Create();
            }
            string nombreTXT = GrabarFacturaCSV(pCliente, rutaTemporal, factura);
            if (!string.IsNullOrEmpty(nombreTXT))
            {
                result = Path.Combine(rutaTemporal, nombreTXT);
            }
            return result;
        }
        public static cDllSaldosComposicion ObtenerSaldosPresentacionParaComposicion(string pLoginWeb, DateTime pFecha)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerSaldosPresentacionParaComposicion(pLoginWeb, pFecha);
        }
        //public static List<string> ObtenerRangoFecha(cClientes pClientes, int pDia, int pPendiente, int pCancelado)
        //{
        //    if (pClientes == null)
        //        return null;
        //    List<string> lista = new List<string>();
        //    DateTime fechaActual = DateTime.Now;
        //    DateTime fechaDesde = fechaActual.AddDays(pDia * -1);
        //    DateTime fechaHasta = fechaActual;
        //    lista.Add(fechaDesde.Day.ToString());
        //    lista.Add((fechaDesde.Month).ToString());
        //    lista.Add((fechaDesde.Year).ToString());

        //    lista.Add(fechaHasta.Day.ToString());
        //    lista.Add((fechaHasta.Month).ToString());
        //    lista.Add((fechaHasta.Year).ToString());
        //    Session["CompocisionSaldo_ResultadoMovimientosDeCuentaCorriente"] = AgregarVariableSessionComposicionSaldo(fechaDesde, fechaHasta, pPendiente, pCancelado, pClientes.cli_login);
        //    return lista;
        //}
        public static List<cCtaCteMovimiento> ObtenerMovimientosDeCuentaCorriente(bool pIsIncluyeCancelado, DateTime pFechaDesde, DateTime pFechaHasta, string pLoginWeb)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerMovimientosDeCuentaCorriente(pIsIncluyeCancelado, pFechaDesde, pFechaHasta, pLoginWeb);
        }
        public static List<cCtaCteMovimiento> AgregarVariableSessionComposicionSaldo(DateTime pFechaDesde, DateTime pFechaHasta, int pPendiente, int pCancelado, string pCli_login)
        {
            string resultado = string.Empty;
            DateTime fechaDesde = pFechaDesde;
            DateTime fechaHasta = pFechaHasta;

            int pendiente = pPendiente;
            int cancelado = pCancelado;

            List<cCtaCteMovimiento> resultadoObj = ObtenerMovimientosDeCuentaCorriente((pendiente == 1 ? true : false), fechaDesde, fechaHasta, pCli_login);

            if (resultadoObj != null)
            {
                List<cCtaCteMovimiento> resultadoAUX = new List<cCtaCteMovimiento>();
                List<cCtaCteMovimiento> parteAUX = null;
                bool isPaso = false;
                bool isPasoPorPaso = false;
                for (int i = 0; i < resultadoObj.Count; i++)
                {
                    bool isAgregarAhora = false;
                    if (isPaso)
                    {
                        parteAUX.Add(resultadoObj[i]);
                        isPaso = false;
                        isPasoPorPaso = true;
                    }
                    if (resultadoObj[i].FechaVencimiento == null)
                    {
                        isAgregarAhora = true;
                    }
                    else
                    {
                        if (i == resultadoObj.Count - 1)
                        {
                            isAgregarAhora = true;
                        }
                        else
                        {
                            if (resultadoObj[i].NumeroComprobante != "" && Convert.ToInt32(resultadoObj[i].TipoComprobante) < 14)
                            {
                                if (resultadoObj[i].NumeroComprobante == resultadoObj[i + 1].NumeroComprobante && resultadoObj[i].TipoComprobante == resultadoObj[i + 1].TipoComprobante)
                                {
                                    if (parteAUX == null)
                                    {
                                        parteAUX = new List<cCtaCteMovimiento>();
                                    }
                                    if (!isPasoPorPaso)
                                    {
                                        parteAUX.Add(resultadoObj[i]);
                                    }
                                    isPaso = true;
                                }
                                else
                                {
                                    isAgregarAhora = true;
                                }
                            }
                            else
                            {
                                isAgregarAhora = true;
                            }
                        }
                    }
                    if (isAgregarAhora)
                    {
                        if (parteAUX != null)
                        {
                            resultadoAUX.AddRange(parteAUX.OrderBy(x => x.FechaVencimiento).ToList());
                            parteAUX = null;
                        }
                        if (!isPasoPorPaso)
                        {
                            resultadoAUX.Add(resultadoObj[i]);
                        }
                    }
                    isPasoPorPaso = false;
                }
                return resultadoAUX;
            }
            return null;
        }
        public static decimal? RecuperarLimiteSaldo()
        {
            return capaClientes_base.RecuperarLimiteSaldo(); ;
        }
        public static cDllRespuestaResumenAbierto ObtenerResumenAbierto(string pLoginWeb)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerResumenAbierto(pLoginWeb);
        }
        public static List<cDllChequeRecibido> ObtenerChequesEnCartera(string pLoginWeb)
        {
            return DKbase.web.capaDatos.capaDLL.ObtenerChequesEnCartera(pLoginWeb);
        }
        public static List<cCtaCteMovimiento> getHiddenDeudaVencida(string pCli_login)
        {
            int pDia = 7;
            int pPendiente = 1;
            int pCancelado = 0;
            DateTime fechaDesde = DateTime.Now.AddDays(pDia * -1);
            DateTime fechaHasta = DateTime.Now;
            List<cCtaCteMovimiento> l = AgregarVariableSessionComposicionSaldo(fechaDesde, fechaHasta, pPendiente, pCancelado, pCli_login);
            return l;
        }
        public static string getDeudaVencidaCSV(cClientes pClientes, List<cCtaCteMovimiento> pLista, string pTipo)
        {
            string resultado = string.Empty;

            if (pLista != null && pClientes != null)
            {
                string ruta = Path.Combine(DKbase.Helper.getFolder, "archivos", "csv");
                DirectoryInfo DIR = new DirectoryInfo(ruta);
                if (!DIR.Exists)
                {
                    DIR.Create();
                }

                string nombreArchivoCSV = string.Empty;
                nombreArchivoCSV = pClientes.cli_login + "-" + pTipo + ".csv";
                resultado = nombreArchivoCSV;
                System.IO.StreamWriter FAC_txt = new System.IO.StreamWriter(Path.Combine(ruta, nombreArchivoCSV), false, System.Text.Encoding.UTF8);

                string strCabeceraCSV = string.Empty;

                strCabeceraCSV += "Fecha";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Vencimiento";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Comprobante";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Semana";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Importe";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Saldo";
                FAC_txt.WriteLine(strCabeceraCSV);
                for (int i = 0; i < pLista.Count; i++)
                {
                    string detalleCSV = string.Empty;
                    detalleCSV += pLista[i].FechaToString;
                    detalleCSV += ";";
                    detalleCSV += pLista[i].FechaVencimientoToString;
                    detalleCSV += ";";
                    detalleCSV += pLista[i].TipoComprobanteToString + " " + pLista[i].NumeroComprobante;
                    detalleCSV += ";";
                    detalleCSV += pLista[i].Semana;
                    detalleCSV += ";";
                    detalleCSV += pLista[i].Importe != null ? Numerica.FormatoNumeroPuntoMilesComaDecimal(pLista[i].Importe.Value) : "";
                    detalleCSV += ";";
                    detalleCSV += pLista[i].Saldo != null ? Numerica.FormatoNumeroPuntoMilesComaDecimal(pLista[i].Saldo.Value) : "";
                    FAC_txt.WriteLine(detalleCSV);
                }
                FAC_txt.Close();
            }
            return resultado;
        }
        public static string getObraSocialEntreFechasCSV(cClientes pClientes, List<cConsObraSocial> pLista)
        {
            string resultado = string.Empty;

            if (pLista != null && pClientes != null)
            {
                string ruta = Path.Combine(DKbase.Helper.getFolder, "archivos", "csv");
                DirectoryInfo DIR = new DirectoryInfo(ruta);
                if (!DIR.Exists)
                {
                    DIR.Create();
                }

                string nombreArchivoCSV = string.Empty;
                nombreArchivoCSV = pClientes.cli_login + "-ObrasSociales" + ".csv";
                resultado = nombreArchivoCSV;
                System.IO.StreamWriter FAC_txt = new System.IO.StreamWriter(Path.Combine(ruta, nombreArchivoCSV), false, System.Text.Encoding.UTF8);

                string strCabeceraCSV = string.Empty;

                strCabeceraCSV += "Fecha";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Comprobante";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Detalle";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Importe";
                FAC_txt.WriteLine(strCabeceraCSV);
                for (int i = 0; i < pLista.Count; i++)
                {
                    string detalleCSV = string.Empty;
                    detalleCSV += pLista[i].FechaComprobanteToString;
                    detalleCSV += ";";
                    detalleCSV += pLista[i].TipoComprobante + " " + pLista[i].NumeroComprobante;
                    detalleCSV += ";";
                    detalleCSV += pLista[i].Detalle;
                    detalleCSV += ";";
                    detalleCSV += Numerica.FormatoNumeroPuntoMilesComaDecimal(pLista[i].Importe);
                    FAC_txt.WriteLine(detalleCSV);
                }
                FAC_txt.Close();
            }
            return resultado;
        }
        public static string getConsultaDeComprobantesEntreFechaCSV(cClientes pClientes, List<cComprobanteDiscriminado> pLista)
        {
            string resultado = string.Empty;

            if (pLista != null && pClientes != null)
            {
                string ruta = Path.Combine(DKbase.Helper.getFolder, "archivos", "csv");
                DirectoryInfo DIR = new DirectoryInfo(ruta);
                if (!DIR.Exists)
                {
                    DIR.Create();
                }

                string nombreArchivoCSV = string.Empty;
                nombreArchivoCSV = pClientes.cli_login + "-ConsultaDeComprobantes" + ".csv";
                resultado = nombreArchivoCSV;
                System.IO.StreamWriter FAC_txt = new System.IO.StreamWriter(Path.Combine(ruta, nombreArchivoCSV), false, System.Text.Encoding.UTF8);

                string strCabeceraCSV = string.Empty;

                strCabeceraCSV += "Fecha";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Tipo";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Comprobante";
                strCabeceraCSV += ";";
                strCabeceraCSV += "Importe";
                FAC_txt.WriteLine(strCabeceraCSV);
                for (int i = 0; i < pLista.Count; i++)
                {
                    string detalleCSV = string.Empty;
                    detalleCSV += pLista[i].FechaToString;
                    detalleCSV += ";";
                    detalleCSV += pLista[i].Comprobante;
                    detalleCSV += ";";
                    detalleCSV += pLista[i].NumeroComprobante;
                    detalleCSV += ";";
                    detalleCSV += Numerica.FormatoNumeroPuntoMilesComaDecimal(pLista[i].MontoTotal);
                    FAC_txt.WriteLine(detalleCSV);
                }
                FAC_txt.Close();
            }
            return resultado;
        }
        public static decimal? ObtenerCreditoDisponibleSemanal(string pLoginWeb)
        {
            return capaDLL.ObtenerCreditoDisponibleSemanal(pLoginWeb); ;
        }
        public static decimal? ObtenerCreditoDisponibleTotal(string pLoginWeb)
        {
            return capaDLL.ObtenerCreditoDisponibleTotal(pLoginWeb); ;
        }
        public static ResultCreditoDisponible ObtenerCreditoDisponible(string pCli_login)
        {
            ResultCreditoDisponible o = new ResultCreditoDisponible();
            o.CreditoDisponibleSemanal = ObtenerCreditoDisponibleSemanal(pCli_login);
            o.CreditoDisponibleTotal = ObtenerCreditoDisponibleTotal(pCli_login);
            return o;// Codigo.clases.Generales.Serializador.SerializarAJson(o);
        }
        public static List<cFichaCtaCte> ObtenerMovimientosDeFichaCtaCte(string pLoginWeb, DateTime pFechaDesde, DateTime pFechaHasta)
        {
            return capaDLL.ObtenerMovimientosDeFichaCtaCte(pLoginWeb, pFechaDesde, pFechaHasta);
        }
        public static List<string> ObtenerTiposDeComprobantesAMostrar(string pLoginWeb)
        {
            return capaDLL.ObtenerTiposDeComprobantesAMostrar(pLoginWeb);
        }
        public static List<cPlan> ObtenerPlanesDeObrasSociales()
        {
            return capaDLL.ObtenerPlanesDeObrasSociales();
        }
        public static List<cCbteParaImprimir> ObtenerComprobantesAImprimirEnBaseAResumen(string pNumeroResumen)
        {
            return capaDLL.ObtenerComprobantesAImprimirEnBaseAResumen(pNumeroResumen);
        }
        public static List<cResumen> ObtenerUltimos10ResumenesDePuntoDeVenta(string pLoginWeb)
        {
            return capaDLL.ObtenerUltimos10ResumenesDePuntoDeVenta(pLoginWeb);
        }
        public static List<cPlanillaObSoc> ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioMes(string pNombrePlan, string pLoginWeb, int pAnio, int pMes)
        {
            return capaDLL.ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioMes(pNombrePlan, pLoginWeb, pAnio, pMes);
        }
        public static List<cPlanillaObSoc> ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioMesQuincena(string pNombrePlan, string pLoginWeb, int pAnio, int pMes, int pQuincena)
        {
            return capaDLL.ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioMesQuincena(pNombrePlan, pLoginWeb, pAnio, pMes, pQuincena);
        }
        public static List<cPlanillaObSoc> ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioSemana(string pNombrePlan, string pLoginWeb, int pAnio, int pSemana)
        {
            return capaDLL.ObtenerPlanillasObraSocialClientesDeObraSocialPorAnioSemana(pNombrePlan, pLoginWeb, pAnio, pSemana);
        }
        public static List<cConsObraSocial> ObtenerComprobantesObrasSocialesDePuntoDeVentaEntreFechas(string pLoginWeb, string pPlan, DateTime pFechaDesde, DateTime pFechaHasta)
        {
            return capaDLL.ObtenerComprobantesObrasSocialesDePuntoDeVentaEntreFechas(pLoginWeb, pPlan, pFechaDesde, pFechaHasta);
        }
        public static List<cComprobantesDiscriminadosDePuntoDeVenta> ObtenerComprobantesDiscriminadosDePuntoDeVentaEntreFechas(string pLoginWeb, DateTime pFechaDesde, DateTime pFechaHasta)
        {
            return capaDLL.ObtenerComprobantesDiscriminadosDePuntoDeVentaEntreFechas(pLoginWeb, pFechaDesde, pFechaHasta);
        }
        public static double? ObtenerSaldoFinalADiciembrePorCliente(string pCli_login)
        {
            return capaDLL.ObtenerSaldoFinalADiciembrePorCliente(pCli_login);
        }
        public static int enviarSolicitudSobresRemesa(cClientes oCliente)
        {
            int resultado = 0;
            string nombre = string.Empty;
            string localidad = string.Empty;
            string reparto = string.Empty;
            string numeroCliente = string.Empty;
            string strHtml = string.Empty;
            if (oCliente != null)
            {
                nombre = oCliente.cli_nombre;
                localidad = oCliente.cli_localidad;
                reparto = oCliente.cli_codrep;
                numeroCliente = oCliente.cli_codigo.ToString();

                strHtml += "El cliente " + nombre + " a solicitado el envio Sobres/Remesas<br/>";
                strHtml += "Localidad: " + localidad + "<br/>";
                strHtml += "Código de reparto: " + reparto + "<br/>";
                strHtml += "Número de cliente: " + numeroCliente + "<br/>";
            }
            string l_mail = DKbase.Helper.getMail_solicitudSobresRemesa;
            if (!string.IsNullOrEmpty(l_mail))
            {
                string[] valores = l_mail.Split(';');
                web.generales.cMail_base.enviarMail_generico(valores.ToList(), "Solicitud Sobres/Remesa", strHtml);
            }
            return resultado;
        }
        public static int enviarConsultaCtaCte(DKbase.web.Usuario pUsuario, string pMail, string pComentario)
        {
            int resultado = 0;
            string NombreYApellido = string.Empty;
            if (pUsuario != null)
            {
                NombreYApellido = pUsuario.NombreYApellido;
            }
            web.generales.cMail_base.enviarMail(DKbase.Helper.getMail_ctacte, "Consultas cuentas corrientes", "Cliente: " + NombreYApellido + "<br/>Mail: " + pMail + "<br/>Comentario: " + pComentario);
            return resultado;
        }
        public static List<cVencimientoResumen> ObtenerVencimientosResumenPorFecha(string pNumeroResumen, DateTime pFechaVencimiento)
        {
            return capaDLL.ObtenerVencimientosResumenPorFecha(pNumeroResumen, pFechaVencimiento);
        }
        public static List<cDevolucionItemPrecarga_java> ObtenerSolicitudesDevolucionCliente(cClientes pCliente)
        {
            return capaDLL.ObtenerSolicitudesDevolucionCliente(pCliente);
        }
        public static List<cDevolucionItemPrecarga_java> RecuperarItemsDevolucionPrecargaFacturaCompletaPorCliente(int pIdCliente)
        {
            List<cDevolucionItemPrecarga_java> resultado = new List<cDevolucionItemPrecarga_java>();
            DataTable tabla = capaDevoluciones_base.RecuperarItemsDevolucionPrecargaFacturaCompletaPorCliente(pIdCliente);
            if (tabla != null)
            {
                foreach (DataRow item in tabla.Rows)
                {
                    resultado.Add(capaDevoluciones_base.ConvertToItemDevPrecarga(item));
                }
            }
            return resultado;
        }
        public static List<cDevolucionItemPrecarga_java> RecuperarItemsDevolucionPrecargaPorCliente(int pIdCliente)
        {

            List<cDevolucionItemPrecarga_java> resultado = new List<cDevolucionItemPrecarga_java>();
            DataTable tabla = capaDevoluciones_base.RecuperarItemsDevolucionPrecargaPorCliente(pIdCliente);
            if (tabla != null)
            {
                foreach (DataRow item in tabla.Rows)
                {
                    resultado.Add(capaDevoluciones_base.ConvertToItemDevPrecarga(item));
                }
            }
            return resultado;
        }
        public static List<cDevolucionItemPrecarga_java> RecuperarItemsDevolucionPrecargaVencidosPorCliente(int pIdCliente)
        {
            List<cDevolucionItemPrecarga_java> resultado = new List<cDevolucionItemPrecarga_java>();
            DataTable tabla = capaDevoluciones_base.RecuperarItemsDevolucionPrecargaVencidosPorCliente(pIdCliente);
            if (tabla != null)
            {
                foreach (DataRow item in tabla.Rows)
                {
                    resultado.Add(capaDevoluciones_base.ConvertToItemDevPrecarga(item));
                }
            }
            return resultado;
        }
        public static List<cDevolucionItemPrecarga_java> RecuperarItemsReclamoFacturadoNoEnviado(int pIdCliente)
        {

            List<cDevolucionItemPrecarga_java> resultado = new List<cDevolucionItemPrecarga_java>();
            DataTable tabla = capaDevoluciones_base.RecuperarItemsReclamoFacturadoNoEnviado(pIdCliente);
            if (tabla != null)
            {
                foreach (DataRow item in tabla.Rows)
                {
                    resultado.Add(capaDevoluciones_base.ConvertToItemDevPrecarga(item));
                }
            }
            return resultado;

        }
        public static List<cDevolucionItemPrecarga_java> ObtenerReclamosFacturadoNoEnviadoPorCliente(cClientes pCliente)
        {
            List<cDevolucionItemPrecarga_java> resultado = null;
            resultado = capaDLL.ObtenerReclamosFacturadoNoEnviadoPorCliente(pCliente);
            return resultado;
        }
        public static long ObtenerCantidadSolicitadaDevolucionPorProductoFacturaYCliente(string NombreProducto, string NumeroFactura, string pLoginWeb)
        {
            long? resultado = null;
            resultado = capaDLL.ObtenerCantidadSolicitadaDevolucionPorProductoFacturaYCliente(NombreProducto, NumeroFactura, pLoginWeb);
            return resultado == null ? 0 : resultado.Value;
        }
        public static int enviarConsultaReclamos(DKbase.web.Usuario pUsuario, string pMail, string pComentario, string pNombreProducto)
        {
            int resultado = 0;
            string NombreYApellido = string.Empty;
            if (pUsuario != null)
            {
                NombreYApellido = pUsuario.NombreYApellido;
            }
            DKbase.web.generales.cMail_base.enviarMail(DKbase.Helper.getMail_reclamos, "Consulta por el producto " + pNombreProducto + " con CADENA DE FRÍO", "Cliente: " + NombreYApellido + "<br/>Mail: " + pMail + "<br/>Comentario: " + pComentario);
            return resultado;
        }
        public static int enviarConsultaValePsicotropico(DKbase.web.Usuario pUsuario, string pMail, string pComentario, string pNombreProducto)
        {
            int resultado = 0;
            string NombreYApellido = string.Empty;
            if (pUsuario != null)
            {
                NombreYApellido = pUsuario.NombreYApellido;
            }
            DKbase.web.generales.cMail_base.enviarMail(DKbase.Helper.getMail_reclamos, "Consulta por el producto " + pNombreProducto + " el cual requiere VALE DE PSICOTRÓPICO", "Cliente: " + NombreYApellido + "<br/>Mail: " + pMail + "<br/>Comentario: " + pComentario);
            return resultado;
        }
        public static bool EsFacturaConDevolucionesEnProceso(string pNumeroFactura, string pLoginWeb)
        {
            return capaDLL.EsFacturaConDevolucionesEnProceso(pNumeroFactura, pLoginWeb);
        }
        public static List<cFactura> ObtenerFacturasPorUltimosNumeros(string pNumeroFactura, string pLoginWeb)
        {
            List<cFactura> resultado = null;
            resultado = capaDLL.ObtenerFacturasPorUltimosNumeros(pNumeroFactura, pLoginWeb);
            return resultado;
        }
        public static string AgregarReclamoFacturadoNoEnviadoCliente(List<cDevolucionItemPrecarga_java> Item, string pLoginWeb)
        {
            string resultado = null;
            resultado = capaDLL.AgregarReclamoFacturadoNoEnviado(Item, pLoginWeb);
            return resultado;
        }
        public static string AgregarSolicitudDevolucionCliente(List<cDevolucionItemPrecarga_java> Item, string pLoginWeb)
        {
            string resultado = null;
            resultado = capaDLL.AgregarSolicitudDevolucionCliente(Item, pLoginWeb);
            return resultado;
        }
        public static bool EliminarDevolucionItemPrecarga(int NumeroItem)
        {
            return capaDevoluciones_base.EliminarDevolucionItemPrecarga(NumeroItem);
        }
        public static bool ElimminarItemReclamoFNEPrecarga(int NumeroItem)
        {
            return capaDevoluciones_base.ElimminarItemReclamoFNEPrecarga(NumeroItem);
        }
        public static bool EliminarPrecargaDevolucionPorCliente(int NumeroCliente)
        {
            return capaDevoluciones_base.EliminarPrecargaDevolucionPorCliente(NumeroCliente);
        }
        public static bool EliminarPrecargaDevolucionVencidosPorCliente(int NumeroCliente)
        {
            return capaDevoluciones_base.EliminarPrecargaDevolucionVencidosPorCliente(NumeroCliente);
        }
        public static bool EliminarPrecargaDevolucionFacturaCompletaPorCliente(int NumeroCliente)
        {
            return capaDevoluciones_base.EliminarPrecargaDevolucionFacturaCompletaPorCliente(NumeroCliente);
        }
        public static bool EliminarPrecargaReclamoFNEPorCliente(int NumeroCliente)
        {
            return capaDevoluciones_base.EliminarPrecargaReclamoFNEPorCliente(NumeroCliente);
        }
        public static bool AgregarReclamoFacturadoNoEnviadoItemPrecarga(cDevolucionItemPrecarga_java Item)
        {
            return capaDevoluciones_base.AgregarReclamoFacturadoNoEnviadoItemPrecarga(Item);
        }
        public static bool AgregarDevolucionItemPrecarga(cDevolucionItemPrecarga_java Item)
        {
            return capaDevoluciones_base.AgregarDevolucionItemPrecarga(Item);
        }
        public static List<cLote> ObtenerNumerosLoteDeProductoDeFacturaProveedorLogLotesConCadena(string pNombreProducto, string pNumeroLote, string pLoginWeb)
        {
            List<cLote> resultado = null;
            resultado = capaDLL.ObtenerNumerosLoteDeProductoDeFacturaProveedorLogLotesConCadena(pNombreProducto, pNumeroLote, pLoginWeb);
            return resultado;
        }
        public static List<cDevolucionItemPrecarga_java> ObtenerReclamosFacturadoNoEnviadoPorClientePorNumero(string pNumeroDevolucion, string pLoginWeb)
        {
            List<cDevolucionItemPrecarga_java> resultado = null;
            resultado = capaDLL.ObtenerReclamosFacturadoNoEnviadoPorClientePorNumero(pNumeroDevolucion, pLoginWeb);
            return resultado;
        }
        public static List<cDevolucionItemPrecarga_java> ObtenerSolicitudesDevolucionClientePorNumero(string pNumeroDevolucion, string pLoginWeb)
        {
            List<cDevolucionItemPrecarga_java> resultado = null;
            resultado = capaDLL.ObtenerSolicitudesDevolucionClientePorNumero(pNumeroDevolucion, pLoginWeb);
            return resultado;
        }
        public static bool spForceChangePasswordFindCliente(int pIdCliente)
        {
            DataTable tb = capaLogRegistro_base.spForceChangePasswordFindCliente(pIdCliente);
            return (tb != null && tb.Rows.Count > 0) ? true : false;
        }
        public static bool spForceChangePasswordDeleteCliente(int pIdCliente)
        {
            return capaLogRegistro_base.spForceChangePasswordDeleteCliente(pIdCliente) > 0;
        }
        public static bool spForceChangePasswordHistoryAdd(int pIdCliente, int pIdUsuario, string pAction)
        {
            return capaLogRegistro_base.spForceChangePasswordHistoryAdd(pIdCliente, pIdUsuario, pAction) > 0;
        }
        public static List<cPedidoItem> ObtenerItemsDePedidoPorNumeroDeFactura(string pNumeroFactura, string pLoginWeb)
        {
            List<cPedidoItem> resultado = null;
            resultado = capaDLL.ObtenerItemsDePedidoPorNumeroDeFactura(pNumeroFactura, pLoginWeb);
            return resultado;
        }
        public static int InsertarActualizarUsuario(cUsuario pUsuario, int pCodUsuarioModif)
        {//int usu_codigo, int usu_codRol, int? usu_codCliente, string usu_nombre, string usu_apellido, string usu_mail, string usu_login, string usu_psw, string usu_observacion, int? usu_codUsuarioUltMov
            return DKbase.Util.InsertarActualizarUsuario(pUsuario.usu_codigo, pUsuario.usu_codRol, pUsuario.usu_codCliente, pUsuario.usu_nombre, pUsuario.usu_apellido, pUsuario.usu_mail, pUsuario.usu_login, "", pUsuario.usu_observacion, pCodUsuarioModif);
        }
        public static List<cRol> GetRoles(string sortExpression, string pFiltro)
        {
            ordenamientoExpresion order = new ordenamientoExpresion(sortExpression);
            string filtro = string.Empty;
            if (pFiltro != null)
            {
                filtro = pFiltro;
            }
            var query = RecuperarTodasRoles(filtro);
            if (order.isOrderBy)
            {
                if (order.OrderByAsc)
                {
                    switch (order.OrderByField)
                    {
                        case "rol_Nombre":
                            query = query.OrderBy(b => b.rol_Nombre).ToList();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (order.OrderByField)
                    {
                        case "rol_Nombre":
                            query = query.OrderByDescending(b => b.rol_Nombre).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }
            return query;
        }
        public static int InsertarActualizarRol(int rol_codRol, string rol_Nombre)
        {
            string accion = rol_codRol == 0 ? Constantes.cSQL_INSERT : Constantes.cSQL_UPDATE;
            DataSet dsResultado = capaSeguridad_base.GestiónRol(rol_codRol, rol_Nombre, null, accion);
            int resultado = -1;
            if (rol_codRol == 0)
            {
                if (dsResultado != null)
                {
                    if (dsResultado.Tables["Rol"].Rows[0]["rol_codRol"] != DBNull.Value)
                    {
                        resultado = Convert.ToInt32(dsResultado.Tables["Rol"].Rows[0]["rol_codRol"]);
                    }
                }
            }
            else
            {
                resultado = rol_codRol;
            }
            return resultado;
        }
        public static List<cRol> RecuperarTodasRoles(string pFiltro)
        {
            List<cRol> lista = new List<cRol>();
            DataSet dsResultado = capaSeguridad_base.GestiónRol(null, null, pFiltro, Constantes.cSQL_SELECT);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["Rol"].Rows)
                {
                    cRol obj = new cRol();
                    if (item["rol_codRol"] != DBNull.Value)
                    {
                        obj.rol_codRol = Convert.ToInt32(item["rol_codRol"]);
                    }
                    if (item["rol_Nombre"] != DBNull.Value)
                    {
                        obj.rol_Nombre = item["rol_Nombre"].ToString();
                    }
                    lista.Add(obj);
                }
            }
            return lista;
        }
        public static cRol RecuperarRolPorId(int pIdRol)
        {
            cRol resultado = new cRol();
            DataSet dsResultado = capaSeguridad_base.GestiónRol(pIdRol, null, null, Constantes.cSQL_SELECT);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["Rol"].Rows)
                {
                    if (item["rol_codRol"] != DBNull.Value)
                    {
                        resultado.rol_codRol = Convert.ToInt32(item["rol_codRol"]);
                    }
                    if (item["rol_Nombre"] != DBNull.Value)
                    {
                        resultado.rol_Nombre = item["rol_Nombre"].ToString();
                    }
                }
            }
            return resultado;
        }
        public static void EliminarRegla(int rgl_codRegla)
        {
            DataSet dsResultado = capaSeguridad_base.GestiónRegla(rgl_codRegla, null, null, null, null, null, null, null, Constantes.cSQL_DELETE);
        }
        public static int InsertarActualizarRegla(int rgl_codRegla, string rgl_Descripcion, string rgl_PalabraClave, bool rgl_IsAgregarSoporta, bool rgl_IsEditarSoporta, bool rgl_IsEliminarSoporta, int? rgl_codReglaPadre)
        {
            string accion = rgl_codRegla == 0 ? Constantes.cSQL_INSERT : Constantes.cSQL_UPDATE;
            DataSet dsResultado = capaSeguridad_base.GestiónRegla(rgl_codRegla, rgl_Descripcion, rgl_PalabraClave, rgl_IsAgregarSoporta, rgl_IsEditarSoporta, rgl_IsEliminarSoporta, rgl_codReglaPadre, null, accion);
            int resultado = -1;
            if (rgl_codRegla == 0)
            {
                if (dsResultado != null)
                {
                    if (dsResultado.Tables["Regla"].Rows[0]["rgl_codRegla"] != DBNull.Value)
                    {
                        resultado = Convert.ToInt32(dsResultado.Tables["Regla"].Rows[0]["rgl_codRegla"]);
                    }
                }
            }
            else
            {
                resultado = rgl_codRegla;
            }
            return resultado;
        }
        public static List<cRegla> RecuperarTodasReglas(string pFiltro)
        {
            List<cRegla> lista = new List<cRegla>();
            DataSet dsResultado = capaSeguridad_base.GestiónRegla(null, null, null, null, null, null, null, pFiltro, Constantes.cSQL_SELECT);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["Regla"].Rows)
                {
                    cRegla obj = new cRegla();
                    if (item["rgl_codRegla"] != DBNull.Value)
                    {
                        obj.rgl_codRegla = Convert.ToInt32(item["rgl_codRegla"]);
                    }
                    if (item["rgl_Descripcion"] != DBNull.Value)
                    {
                        obj.rgl_Descripcion = item["rgl_Descripcion"].ToString();
                    }
                    if (item["rgl_PalabraClave"] != DBNull.Value)
                    {
                        obj.rgl_PalabraClave = item["rgl_PalabraClave"].ToString();
                    }
                    if (item["rgl_IsAgregarSoporta"] != DBNull.Value)
                    {
                        obj.rgl_IsAgregarSoporta = Convert.ToBoolean(item["rgl_IsAgregarSoporta"]);
                    }
                    if (item["rgl_IsEditarSoporta"] != DBNull.Value)
                    {
                        obj.rgl_IsEditarSoporta = Convert.ToBoolean(item["rgl_IsEditarSoporta"]);
                    }
                    if (item["rgl_IsEliminarSoporta"] != DBNull.Value)
                    {
                        obj.rgl_IsEliminarSoporta = Convert.ToBoolean(item["rgl_IsEliminarSoporta"]);
                    }
                    if (item["rgl_codReglaPadre"] != DBNull.Value)
                    {
                        obj.rgl_codReglaPadre = Convert.ToInt32(item["rgl_codReglaPadre"]);
                    }
                    lista.Add(obj);
                }
            }
            return lista;
        }
        public static cRegla RecuperarReglaPorId(int pIdRegla)
        {
            cRegla obj = null;
            DataSet dsResultado = capaSeguridad_base.GestiónRegla(pIdRegla, null, null, null, null, null, null, null, Constantes.cSQL_SELECT);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["Regla"].Rows)
                {
                    obj = new cRegla();
                    if (item["rgl_codRegla"] != DBNull.Value)
                    {
                        obj.rgl_codRegla = Convert.ToInt32(item["rgl_codRegla"]);
                    }
                    if (item["rgl_Descripcion"] != DBNull.Value)
                    {
                        obj.rgl_Descripcion = item["rgl_Descripcion"].ToString();
                    }
                    if (item["rgl_PalabraClave"] != DBNull.Value)
                    {
                        obj.rgl_PalabraClave = item["rgl_PalabraClave"].ToString();
                    }
                    if (item["rgl_IsAgregarSoporta"] != DBNull.Value)
                    {
                        obj.rgl_IsAgregarSoporta = Convert.ToBoolean(item["rgl_IsAgregarSoporta"]);
                    }
                    if (item["rgl_IsEditarSoporta"] != DBNull.Value)
                    {
                        obj.rgl_IsEditarSoporta = Convert.ToBoolean(item["rgl_IsEditarSoporta"]);
                    }
                    if (item["rgl_IsEliminarSoporta"] != DBNull.Value)
                    {
                        obj.rgl_IsEliminarSoporta = Convert.ToBoolean(item["rgl_IsEliminarSoporta"]);
                    }
                    if (item["rgl_codReglaPadre"] != DBNull.Value)
                    {
                        obj.rgl_codReglaPadre = Convert.ToInt32(item["rgl_codReglaPadre"]);
                    }
                    break;
                }
            }
            return obj;
        }
        public static List<cCombo> RecuperarTodasReglas_Combo()
        {
            List<cCombo> lista = new List<cCombo>();
            DataSet dsResultado = capaSeguridad_base.GestiónRegla(null, null, null, null, null, null, null, null, Constantes.cSQL_COMBO);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["Regla"].Rows)
                {
                    cCombo obj = new cCombo();
                    if (item["rgl_codRegla"] != DBNull.Value)
                    {
                        obj.id = Convert.ToInt32(item["rgl_codRegla"]);
                    }
                    if (item["rgl_Descripcion"] != DBNull.Value)
                    {
                        obj.nombre = item["rgl_Descripcion"].ToString();
                    }
                    lista.Add(obj);
                }
            }
            return lista;
        }
        public static void InsertarActualizarRelacionRolRegla(int pIdRol, string pXML)
        {
            DataSet dsResultado = capaSeguridad_base.GestiónRoleRegla(pIdRol, null, pXML, Constantes.cSQL_UPDATE);
        }

        public static List<ReglaPorRol> RecuperarRelacionRolReglasPorRol(int pIdRol)
        {
            List<ReglaPorRol> listaResultado = new List<ReglaPorRol>();
            DataSet dsResultado = capaSeguridad_base.GestiónRoleRegla(pIdRol, null, null, Constantes.cSQL_SELECT);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["RelacionRoleRegla"].Rows)
                {
                    ReglaPorRol obj = new ReglaPorRol();
                    obj.idRegla = Convert.ToInt32(item["rrr_codRegla"]);
                    obj.idRelacionReglaRol = Convert.ToInt32(item["rrr_codRelacionRolRegla"]);
                    obj.isActivo = Convert.ToBoolean(item["rrr_IsActivo"]);
                    if (item["rrr_IsAgregar"] is DBNull)
                    {
                        obj.isAgregar = null;
                    }
                    else
                    {
                        obj.isAgregar = Convert.ToBoolean(item["rrr_IsAgregar"]);
                    }
                    if (item["rrr_IsEditar"] is DBNull)
                    {
                        obj.isEditar = null;
                    }
                    else
                    {
                        obj.isEditar = Convert.ToBoolean(item["rrr_IsEditar"]);
                    }
                    if (item["rrr_IsEliminar"] is DBNull)
                    {
                        obj.isEliminar = null;
                    }
                    else
                    {
                        obj.isEliminar = Convert.ToBoolean(item["rrr_IsEliminar"]);
                    }
                    listaResultado.Add(obj);
                }
            }
            return listaResultado;
        }
        public static List<cComprobanteDiscriminado> ObtenerComprobantesEntreFechas(string pTipoComprobante, DateTime pDesde, DateTime pHasta, string pLoginWeb)
        {
            List<cComprobanteDiscriminado> resultado = null;
            resultado = capaDLL.ObtenerComprobantesEntreFechas(pTipoComprobante, pDesde, pHasta, pLoginWeb);
            return resultado;
        }
        public static void LogInfo(System.Reflection.MethodBase method, string pMensaje, string pInfoAdicional, string pType, string pFile_type, byte[] pFile_content, params object[] values)
        {
            DKbase.generales.Log.LogInfo(method, pMensaje, pInfoAdicional, pType, pFile_type, pFile_content, values);
        }//
        public static string spInsertSessionApp(string pName)
        {
            string result = string.Empty;
            try
            {
                BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
                List<System.Data.SqlClient.SqlParameter> l = new List<System.Data.SqlClient.SqlParameter>();
                l.Add(db.GetParameter("name", pName));
                object var = db.ExecuteScalar("spInsertSessionApp", l);
                result = var != null ? var.ToString() : null;
            }
            catch (Exception ex)
            {
                Log.LogError(System.Reflection.MethodBase.GetCurrentMethod(), ex, DateTime.Now);
            }
            return result;
        }
        public static List<cOferta> RecuperarTodasOfertas_generico()
        {
            List<cOferta> resultado = null;
            DataTable tabla = capaHome_base.RecuperarTodasOfertas();
            if (tabla != null)
            {
                resultado = new List<cOferta>();
                foreach (DataRow fila in tabla.Rows)
                {
                    resultado.Add(DKbase.web.capaDatos.capaHome_base.ConvertToOferta(fila));
                }
            }
            return resultado;
        }

    }
}