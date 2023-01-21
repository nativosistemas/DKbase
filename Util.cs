using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using DKbase.web;
using DKbase.generales;

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
                //tbl_Oferta_Rating o = null;
                //KellerhoffEntities ctx = new KellerhoffEntities();
                //o = ctx.tbl_Oferta_Rating.Create();
                //o.ofr_fecha = DateTime.Now;
                //o.ofr_idCliente = ofr_idCliente;
                //o.ofr_idOferta = ofr_idOferta;
                //o.ofr_isDesdeHome = ofr_isDesdeHome;
                //ctx.tbl_Oferta_Rating.Add(o);
                //ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
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
        public static string verExisteArchivo() {
            return System.IO.File.Exists(verRutaArchivo()) ? "Si":"No" ;
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
    }
}