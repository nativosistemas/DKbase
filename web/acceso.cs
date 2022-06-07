using DKbase.Entities;
using DKbase.generales;
using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Reflection;

namespace DKbase.web
{
    public class acceso
    {
        public static Usuario Login(string pLogin, string pPassword, string pIp, string pHostName, string pUserAgent)
        {
            DataSet dsResultado = capaSeguridad_base.Login(pLogin, pPassword, pIp, pHostName, pUserAgent);
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
            DataTable tablaAcciones = capaDatos.capaSeguridad_base.RecuperarTodasAccionesRol(pIdRol);
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
            DataSet dsResultado = capaClientes_base.GestiónSucursal(null, null, null, Constantes.cSQL_SELECT);
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
            DataTable tabla = capaClientes_base.RecuperarTodosCodigoReparto();
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

        public static cClientes ConvertToCliente(DataRow pItem)
        {
            cClientes obj = new cClientes();
            if (pItem["cli_codigo"] != DBNull.Value)
            {
                obj.cli_codigo = Convert.ToInt32(pItem["cli_codigo"]);
            }
            if (pItem["cli_nombre"] != DBNull.Value)
            {
                obj.cli_nombre = pItem["cli_nombre"].ToString();
            }
            if (pItem["cli_dirección"] != DBNull.Value)
            {
                obj.cli_dirección = pItem["cli_dirección"].ToString();
            }
            if (pItem["cli_Telefono"] != DBNull.Value)
            {
                obj.cli_telefono = pItem["cli_Telefono"].ToString();
            }
            if (pItem["cli_localidad"] != DBNull.Value)
            {
                obj.cli_localidad = pItem["cli_localidad"].ToString();
            }
            if (pItem["cli_codprov"] != DBNull.Value)
            {
                obj.cli_codprov = pItem["cli_codprov"].ToString();
            }
            if (pItem["cli_email"] != DBNull.Value)
            {
                obj.cli_email = pItem["cli_email"].ToString();
            }
            if (pItem["cli_paginaweb"] != DBNull.Value)
            {
                obj.cli_paginaweb = pItem["cli_paginaweb"].ToString();
            }
            if (pItem["cli_codsuc"] != DBNull.Value)
            {
                obj.cli_codsuc = pItem["cli_codsuc"].ToString();
            }
            if (pItem["cli_pordesespmed"] != DBNull.Value)
            {
                obj.cli_pordesespmed = Convert.ToDecimal(pItem["cli_pordesespmed"]);
            }
            if (pItem["cli_pordesbetmed"] != DBNull.Value)
            {
                obj.cli_pordesbetmed = Convert.ToDecimal(pItem["cli_pordesbetmed"]);
            }
            if (pItem["cli_pordesnetos"] != DBNull.Value)
            {
                obj.cli_pordesnetos = Convert.ToDecimal(pItem["cli_pordesnetos"]);
            }
            if (pItem["cli_pordesfinperfcyo"] != DBNull.Value)
            {
                obj.cli_pordesfinperfcyo = Convert.ToDecimal(pItem["cli_pordesfinperfcyo"]);
            }
            if (pItem["cli_pordescomperfcyo"] != DBNull.Value)
            {
                obj.cli_pordescomperfcyo = Convert.ToDecimal(pItem["cli_pordescomperfcyo"]);
            }
            if (pItem["cli_deswebespmed"] != DBNull.Value)
            {
                obj.cli_deswebespmed = Convert.ToBoolean(pItem["cli_deswebespmed"]);
            }
            if (pItem["cli_deswebnetmed"] != DBNull.Value)
            {
                obj.cli_deswebnetmed = Convert.ToBoolean(pItem["cli_deswebnetmed"]);
            }
            if (pItem["cli_deswebnetacc"] != DBNull.Value)
            {
                obj.cli_deswebnetacc = Convert.ToBoolean(pItem["cli_deswebnetacc"]);
            }
            if (pItem["cli_deswebnetperpropio"] != DBNull.Value)
            {
                obj.cli_deswebnetperpropio = Convert.ToBoolean(pItem["cli_deswebnetperpropio"]);
            }
            if (pItem["cli_deswebnetpercyo"] != DBNull.Value)
            {
                obj.cli_deswebnetpercyo = Convert.ToBoolean(pItem["cli_deswebnetpercyo"]);
            }
            if (pItem["cli_destransfer"] != DBNull.Value)
            {
                obj.cli_destransfer = Convert.ToDecimal(pItem["cli_destransfer"]);
            }
            if (pItem["cli_login"] != DBNull.Value)
            {
                obj.cli_login = pItem["cli_login"].ToString();
            }
            if (pItem["cli_codtpoenv"] != DBNull.Value)
            {
                obj.cli_codtpoenv = pItem["cli_codtpoenv"].ToString();
            }
            if (pItem["cli_codrep"] != DBNull.Value)
            {
                obj.cli_codrep = pItem["cli_codrep"].ToString();
            }
            if (pItem.Table.Columns.Contains("cli_isGLN"))
            {
                if (pItem["cli_isGLN"] != DBNull.Value)
                {
                    obj.cli_isGLN = Convert.ToBoolean(pItem["cli_isGLN"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_tomaOfertas"))
            {
                if (pItem["cli_tomaOfertas"] != DBNull.Value)
                {
                    obj.cli_tomaOfertas = Convert.ToBoolean(pItem["cli_tomaOfertas"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_tomaPerfumeria"))
            {
                if (pItem["cli_tomaPerfumeria"] != DBNull.Value)
                {
                    obj.cli_tomaPerfumeria = Convert.ToBoolean(pItem["cli_tomaPerfumeria"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_tomaTransfers"))
            {
                if (pItem["cli_tomaTransfers"] != DBNull.Value)
                {
                    obj.cli_tomaTransfers = Convert.ToBoolean(pItem["cli_tomaTransfers"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_password"))
            {
                if (pItem["cli_password"] != DBNull.Value)
                {
                    obj.cli_password = Convert.ToString(pItem["cli_password"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_estado"))
            {
                if (pItem["cli_estado"] != DBNull.Value)
                {
                    obj.cli_estado = Convert.ToString(pItem["cli_estado"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_tipo"))
            {
                if (pItem["cli_tipo"] != DBNull.Value)
                {
                    obj.cli_tipo = Convert.ToString(pItem["cli_tipo"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_mostrardireccion"))
            {
                if (pItem["cli_mostrardireccion"] != DBNull.Value)
                {
                    obj.cli_mostrardireccion = Convert.ToBoolean(pItem["cli_mostrardireccion"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_mostrarweb"))
            {
                if (pItem["cli_mostrarweb"] != DBNull.Value)
                {
                    obj.cli_mostrarweb = Convert.ToBoolean(pItem["cli_mostrarweb"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_mostrartelefono"))
            {
                if (pItem["cli_mostrartelefono"] != DBNull.Value)
                {
                    obj.cli_mostrartelefono = Convert.ToBoolean(pItem["cli_mostrartelefono"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_mostraremail"))
            {
                if (pItem["cli_mostraremail"] != DBNull.Value)
                {
                    obj.cli_mostraremail = Convert.ToBoolean(pItem["cli_mostraremail"]);
                }
            }
            if (pItem.Table.Columns.Contains("cli_IdSucursalAlternativa") && pItem["cli_IdSucursalAlternativa"] != DBNull.Value)
                obj.cli_IdSucursalAlternativa = Convert.ToString(pItem["cli_IdSucursalAlternativa"]);

            if (pItem.Table.Columns.Contains("cli_AceptaPsicotropicos") && pItem["cli_AceptaPsicotropicos"] != DBNull.Value)
                obj.cli_AceptaPsicotropicos = Convert.ToBoolean(pItem["cli_AceptaPsicotropicos"]);
            if (pItem.Table.Columns.Contains("cli_promotor") && pItem["cli_promotor"] != DBNull.Value)
                obj.cli_promotor = Convert.ToString(pItem["cli_promotor"]);
            if (pItem.Table.Columns.Contains("cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto") && pItem["cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto"] != DBNull.Value)
                obj.cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto = Convert.ToDecimal(pItem["cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto"]);
            if (pItem.Table.Columns.Contains("cli_GrupoCliente") && pItem["cli_GrupoCliente"] != DBNull.Value)
                obj.cli_GrupoCliente = Convert.ToString(pItem["cli_GrupoCliente"]);
            if (pItem.Table.Columns.Contains("cli_NumeroCentralTelefonica") && pItem["cli_NumeroCentralTelefonica"] != DBNull.Value)
                obj.cli_NumeroCentralTelefonica = Convert.ToString(pItem["cli_NumeroCentralTelefonica"]);
            return obj;
        }
        public static cTransferDetalle ConvertToTransferDetalle(DataRow pItem)
        {
            cTransferDetalle obj = new cTransferDetalle();
            if (pItem["tde_codpro"] != DBNull.Value)
            {
                obj.tde_codpro = pItem["tde_codpro"].ToString();
            }
            if (pItem["tde_codtfr"] != DBNull.Value)
            {
                obj.tde_codtfr = Convert.ToInt32(pItem["tde_codtfr"]);
            }
            if (pItem["tde_descripcion"] != DBNull.Value)
            {
                obj.tde_descripcion = pItem["tde_descripcion"].ToString();
            }
            if (pItem["tde_fijuni"] != DBNull.Value)
            {
                obj.tde_fijuni = Convert.ToInt32(pItem["tde_fijuni"]);
            }
            if (pItem["tde_maxuni"] != DBNull.Value)
            {
                obj.tde_maxuni = Convert.ToInt32(pItem["tde_maxuni"]);
            }
            if (pItem["tde_minuni"] != DBNull.Value)
            {
                obj.tde_minuni = Convert.ToInt32(pItem["tde_minuni"]);
            }
            if (pItem["tde_muluni"] != DBNull.Value)
            {
                obj.tde_muluni = Convert.ToInt32(pItem["tde_muluni"]);
            }
            if (pItem["tde_predescuento"] != DBNull.Value)
            {
                obj.tde_predescuento = Convert.ToDecimal(pItem["tde_predescuento"]);
            }
            if (pItem["tde_prepublico"] != DBNull.Value)
            {
                obj.tde_prepublico = Convert.ToDecimal(pItem["tde_prepublico"]);
            }
            if (pItem["tde_proobligatorio"] != DBNull.Value)
            {
                obj.tde_proobligatorio = Convert.ToBoolean(pItem["tde_proobligatorio"]);
            }
            if (pItem.Table.Columns.Contains("tde_PorcDtoSobrePVP") && pItem["tde_PorcDtoSobrePVP"] != DBNull.Value)
            {
                obj.tde_PorcDtoSobrePVP = Convert.ToDecimal(pItem["tde_PorcDtoSobrePVP"]);
            }
            if (pItem.Table.Columns.Contains("pro_neto"))
            {
                if (pItem["pro_neto"] != DBNull.Value)
                {
                    obj.pro_neto = Convert.ToBoolean(pItem["pro_neto"]);
                }
            }
            if (pItem.Table.Columns.Contains("pro_codtpopro"))
            {
                if (pItem["pro_codtpopro"] != DBNull.Value)
                {
                    obj.pro_codtpopro = Convert.ToString(pItem["pro_codtpopro"]);
                }
            }
            if (pItem.Table.Columns.Contains("pro_codigo"))
            {
                if (pItem["pro_codigo"] != DBNull.Value)
                {
                    obj.pro_codigo = Convert.ToString(pItem["pro_codigo"]);
                }
            }
            if (pItem.Table.Columns.Contains("pro_descuentoweb"))
            {
                if (pItem["pro_descuentoweb"] != DBNull.Value)
                {
                    obj.pro_descuentoweb = Convert.ToDecimal(pItem["pro_descuentoweb"]);
                }
            }
            if (pItem.Table.Columns.Contains("pro_isTrazable"))
            {
                if (pItem["pro_isTrazable"] != DBNull.Value)
                {
                    obj.pro_isTrazable = Convert.ToBoolean(pItem["pro_isTrazable"]);
                }
            }
            if (pItem.Table.Columns.Contains("tde_unidadesbonificadas"))
            {
                if (pItem["tde_unidadesbonificadas"] != DBNull.Value)
                {
                    obj.tde_unidadesbonificadas = Convert.ToInt32(pItem["tde_unidadesbonificadas"]);
                }
            }
            if (pItem.Table.Columns.Contains("tde_unidadesbonificadasdescripcion"))
            {
                if (pItem["tde_unidadesbonificadasdescripcion"] != DBNull.Value)
                {
                    obj.tde_unidadesbonificadasdescripcion = Convert.ToString(pItem["tde_unidadesbonificadasdescripcion"]);
                }
            }
            if (pItem.Table.Columns.Contains("tcl_IdTransfer"))
            {
                if (pItem["tcl_IdTransfer"] != DBNull.Value)
                {
                    obj.isTablaTransfersClientes = true;
                }
            }
            if (pItem.Table.Columns.Contains("tde_DescripcionDeProducto"))
            {
                if (pItem["tde_DescripcionDeProducto"] != DBNull.Value)
                {
                    obj.tde_DescripcionDeProducto = Convert.ToString(pItem["tde_DescripcionDeProducto"]);
                }
            }
            if (pItem.Table.Columns.Contains("pro_codtpovta"))
            {
                if (pItem["pro_codtpovta"] != DBNull.Value)
                {
                    obj.pro_codtpovta = Convert.ToString(pItem["pro_codtpovta"]);
                }
            }
            if (pItem.Table.Columns.Contains("tde_PrecioConDescuentoDirecto") && pItem["tde_PrecioConDescuentoDirecto"] != DBNull.Value)
                obj.tde_PrecioConDescuentoDirecto = Convert.ToDecimal(pItem["tde_PrecioConDescuentoDirecto"]);
            if (pItem.Table.Columns.Contains("tde_PorcARestarDelDtoDeCliente") && pItem["tde_PorcARestarDelDtoDeCliente"] != DBNull.Value)
                obj.tde_PorcARestarDelDtoDeCliente = Convert.ToDecimal(pItem["tde_PorcARestarDelDtoDeCliente"]);
            if (pItem.Table.Columns.Contains("pro_PrecioBase") && pItem["pro_PrecioBase"] != DBNull.Value)
                obj.pro_PrecioBase = Convert.ToDecimal(pItem["pro_PrecioBase"]);
            if (pItem.Table.Columns.Contains("pro_PorcARestarDelDtoDeCliente") && pItem["pro_PorcARestarDelDtoDeCliente"] != DBNull.Value)
                obj.pro_PorcARestarDelDtoDeCliente = Convert.ToDecimal(pItem["pro_PorcARestarDelDtoDeCliente"]);
            if (pItem.Table.Columns.Contains("pro_AltoCosto") && pItem["pro_AltoCosto"] != DBNull.Value)
                obj.pro_AltoCosto = Convert.ToBoolean(pItem["pro_AltoCosto"]);
            if (pItem.Table.Columns.Contains("pro_UbicacionPrincipal") && pItem["pro_UbicacionPrincipal"] != DBNull.Value)
                obj.pro_UbicacionPrincipal = Convert.ToString(pItem["pro_UbicacionPrincipal"]);
            return obj;
        }
        public static List<cProductosGenerico> cargarProductosBuscadorArchivos(cClientes pCliente, DataTable tablaProductos, DataTable tablaSucursalStocks, List<cTransferDetalle> listaTransferDetalle, Constantes.CargarProductosBuscador pCargarProductosBuscador, string pSucursalElejida)
        {
            List<cProductosGenerico> resultado = new List<cProductosGenerico>();
            foreach (DataRow item in tablaProductos.Rows)
            {
                if (item["pro_codigo"] != DBNull.Value)
                {
                    List<cSucursalStocks> tempListaSucursalStocks = new List<cSucursalStocks>();
                    tempListaSucursalStocks = (from r in tablaSucursalStocks.Select("stk_codpro = '" + item["pro_codigo"].ToString() + "'").AsEnumerable()
                                               select new cSucursalStocks { stk_codpro = r["stk_codpro"].ToString(), stk_codsuc = r["stk_codsuc"].ToString(), stk_stock = r["stk_stock"].ToString() }).ToList();

                    if (tempListaSucursalStocks.Count > 0)
                    {
                        cProductosGenerico obj = new cProductosGenerico();
                        obj.listaSucursalStocks = tempListaSucursalStocks;
                        obj.pro_codigo = item["pro_codigo"].ToString();
                        if (item["pro_nombre"] != DBNull.Value)
                        {
                            obj.pro_nombre = item["pro_nombre"].ToString();
                        }
                        if (item["pro_precio"] != DBNull.Value)
                        {
                            obj.pro_precio = Convert.ToDecimal(item["pro_precio"]);
                        }
                        if (item["pro_preciofarmacia"] != DBNull.Value)
                        {
                            obj.pro_preciofarmacia = Convert.ToDecimal(item["pro_preciofarmacia"]);
                        }
                        if (item["pro_ofeunidades"] != DBNull.Value)
                        {
                            obj.pro_ofeunidades = Convert.ToInt32(item["pro_ofeunidades"]);
                        }
                        if (item["pro_ofeporcentaje"] != DBNull.Value)
                        {
                            obj.pro_ofeporcentaje = Convert.ToDecimal(item["pro_ofeporcentaje"]);
                        }
                        if (item["pro_neto"] != DBNull.Value)
                        {
                            obj.pro_neto = Convert.ToBoolean(item["pro_neto"]);
                        }
                        if (item["pro_codtpopro"] != DBNull.Value)
                        {
                            obj.pro_codtpopro = item["pro_codtpopro"].ToString().ToUpper();
                        }
                        if (item["pro_descuentoweb"] != DBNull.Value)
                        {
                            obj.pro_descuentoweb = Convert.ToDecimal(item["pro_descuentoweb"]);
                        }
                        if (item["pro_laboratorio"] != DBNull.Value)
                        {
                            obj.pro_laboratorio = item["pro_laboratorio"].ToString();
                        }
                        if (item["pro_monodroga"] != DBNull.Value)
                        {
                            obj.pro_monodroga = item["pro_monodroga"].ToString();
                        }
                        if (item["pro_codigobarra"] != DBNull.Value)
                        {
                            obj.pro_codigobarra = item["pro_codigobarra"].ToString();
                        }
                        if (item["pro_codigoalfabeta"] != DBNull.Value)
                        {
                            obj.pro_codigoalfabeta = item["pro_codigoalfabeta"].ToString();
                        }
                        if (item["pro_troquel"] != DBNull.Value)
                        {
                            obj.pro_troquel = item["pro_troquel"].ToString();
                        }
                        if (item.Table.Columns.Contains("pro_Ranking") && item["pro_Ranking"] != DBNull.Value)
                        {
                            obj.pro_Ranking = Convert.ToInt32(item["pro_Ranking"]);
                        }
                        if (item["pro_codtpovta"] != DBNull.Value)
                        {
                            obj.pro_codtpovta = Convert.ToString(item["pro_codtpovta"]);
                        }
                        if (item["isTransfer"] == DBNull.Value)
                        {
                            obj.isTieneTransfer = false;
                        }
                        else
                        {
                            obj.isTieneTransfer = true;
                        }
                        if (item["pro_isTrazable"] != DBNull.Value)
                        {
                            obj.pro_isTrazable = Convert.ToBoolean(item["pro_isTrazable"]);
                        }
                        if (item["pro_isCadenaFrio"] != DBNull.Value)
                        {
                            obj.pro_isCadenaFrio = Convert.ToBoolean(item["pro_isCadenaFrio"]);
                        }
                        if (item["pro_AceptaVencidos"] != DBNull.Value)
                        {
                            obj.pro_AceptaVencidos = Convert.ToBoolean(item["pro_AceptaVencidos"]);
                        }
                        if (item["pro_ProductoRequiereLote"] != DBNull.Value)
                        {
                            obj.pro_ProductoRequiereLote = Convert.ToBoolean(item["pro_ProductoRequiereLote"]);
                        }
                        if (item["pro_canmaxima"] != DBNull.Value)
                        {
                            obj.pro_canmaxima = Convert.ToInt32(item["pro_canmaxima"]);
                        }
                        // Default = false
                        if (item["pro_entransfer"] != DBNull.Value)
                        {
                            obj.pro_entransfer = Convert.ToBoolean(item["pro_entransfer"]);
                        }
                        if (item["pro_vtasolotransfer"] != DBNull.Value)
                        {
                            obj.pro_vtasolotransfer = Convert.ToBoolean(item["pro_vtasolotransfer"]);
                        }
                        if (item["RequiereVale"] != DBNull.Value)
                        {
                            obj.isValePsicotropicos = Convert.ToBoolean(item["RequiereVale"]);
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
                        if (item.Table.Columns.Contains("pro_Familia"))
                        {
                            if (item["pro_Familia"] != DBNull.Value)
                            {
                                obj.pro_Familia = Convert.ToString(item["pro_Familia"]);
                            }
                        }
                        if (item.Table.Columns.Contains("pro_PackDeVenta"))
                        {
                            if (item["pro_PackDeVenta"] != DBNull.Value)
                            {
                                obj.pro_PackDeVenta = Convert.ToInt32(item["pro_PackDeVenta"]);
                            }
                        }
                        if (item.Table.Columns.Contains("pro_PrecioBase") && item["pro_PrecioBase"] != DBNull.Value)
                        {
                            obj.pro_PrecioBase = Convert.ToDecimal(item["pro_PrecioBase"]);
                        }
                        if (item.Table.Columns.Contains("pro_PorcARestarDelDtoDeCliente") && item["pro_PorcARestarDelDtoDeCliente"] != DBNull.Value)
                        {
                            obj.pro_PorcARestarDelDtoDeCliente = Convert.ToDecimal(item["pro_PorcARestarDelDtoDeCliente"]);
                        }
                        if (item.Table.Columns.Contains("pro_AltoCosto") && item["pro_AltoCosto"] != DBNull.Value)
                        {
                            obj.pro_AltoCosto = Convert.ToBoolean(item["pro_AltoCosto"]);
                        }
                        if (item.Table.Columns.Contains("pro_UbicacionPrincipal") && item["pro_UbicacionPrincipal"] != DBNull.Value)
                        {
                            obj.pro_UbicacionPrincipal = Convert.ToString(item["pro_UbicacionPrincipal"]);
                        }
                        obj.isProductoFacturacionDirecta = false;
                        if (item.Table.Columns.Contains("pro_NoTransfersEnClientesPerf") && item["pro_NoTransfersEnClientesPerf"] != DBNull.Value)
                        {
                            obj.pro_NoTransfersEnClientesPerf = Convert.ToBoolean(item["pro_NoTransfersEnClientesPerf"]);
                            if (pCliente != null && pCliente.cli_tipo == "P" && obj.pro_NoTransfersEnClientesPerf)
                                obj.isMostrarTransfersEnClientesPerf = false;
                        }
                        if ((listaTransferDetalle != null && obj.isMostrarTransfersEnClientesPerf && pCargarProductosBuscador == Constantes.CargarProductosBuscador.isDesdeBuscador) ||
                        (listaTransferDetalle != null && (pCargarProductosBuscador == Constantes.CargarProductosBuscador.isDesdeBuscador_OfertaTransfer || pCargarProductosBuscador == Constantes.CargarProductosBuscador.isSubirArchivo || pCargarProductosBuscador == Constantes.CargarProductosBuscador.isDesdeTabla || pCargarProductosBuscador == Constantes.CargarProductosBuscador.isRecuperadorFaltaCredito)))
                        {
                            List<cTransferDetalle> listaAUXtransferDetalle = listaTransferDetalle.Where(x => x.tde_codpro == obj.pro_nombre).ToList();
                            if (listaAUXtransferDetalle.Count > 0)
                            {
                                int indexTransfer = 0;
                                if (listaAUXtransferDetalle.Count > 1)
                                {
                                    for (int iTablaTransfersClientes = 0; iTablaTransfersClientes < listaAUXtransferDetalle.Count; iTablaTransfersClientes++)
                                    {
                                        if (listaAUXtransferDetalle[iTablaTransfersClientes].isTablaTransfersClientes)
                                        {
                                            indexTransfer = iTablaTransfersClientes;
                                        }
                                    }

                                }
                                obj.isProductoFacturacionDirecta = true;
                                obj.CargarTransferYTransferDetalle(listaAUXtransferDetalle[indexTransfer]);
                                if (pCliente != null)
                                {
                                    obj.PrecioFinalTransfer = FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(pCliente, obj.tfr_deshab, obj.tfr_pordesadi, obj.pro_neto, obj.pro_codtpopro, obj.pro_descuentoweb, obj.tde_predescuento == null ? 0 : (decimal)obj.tde_predescuento, obj.tde_PrecioConDescuentoDirecto, obj.tde_PorcARestarDelDtoDeCliente);
                                }
                            }
                        }

                        if (pCargarProductosBuscador == Constantes.CargarProductosBuscador.isDesdeTabla || pCargarProductosBuscador == Constantes.CargarProductosBuscador.isRecuperadorFaltaCredito)
                        {
                            obj.PrecioFinal = FuncionesPersonalizadas_base.ObtenerPrecioFinal(pCliente, obj);
                            obj.PrecioConDescuentoOferta = FuncionesPersonalizadas_base.ObtenerPrecioUnitarioConDescuentoOferta(obj.PrecioFinal, obj);
                        }
                        if (pCargarProductosBuscador == Constantes.CargarProductosBuscador.isRecuperadorFaltaCredito)
                        {
                            if (item.Table.Columns.Contains("fpc_cantidad") && item["fpc_cantidad"] != DBNull.Value)
                                obj.fpc_cantidad = Convert.ToInt32(item["fpc_cantidad"]);
                            if (item.Table.Columns.Contains("fpc_nombreProducto") && item["fpc_nombreProducto"] != DBNull.Value)
                                obj.fpc_nombreProducto = Convert.ToString(item["fpc_nombreProducto"]);
                            if (item.Table.Columns.Contains("stk_stock") && item["stk_stock"] != DBNull.Value)
                                obj.stk_stock = Convert.ToString(item["stk_stock"]);

                            obj.PrecioFinalRecuperador = obj.PrecioFinal;
                            if (obj.PrecioFinalTransfer > 0 && obj.PrecioFinalTransfer < obj.PrecioFinalRecuperador)
                            {
                                obj.PrecioFinalRecuperador = obj.PrecioFinalTransfer;
                            }
                            if (obj.PrecioConDescuentoOferta > 0 && obj.PrecioConDescuentoOferta < obj.PrecioFinalRecuperador)
                            {
                                obj.PrecioFinalRecuperador = obj.PrecioConDescuentoOferta;
                            }
                        }
                        if (pCargarProductosBuscador == Constantes.CargarProductosBuscador.isSubirArchivo)
                        {
                            for (int iSucursalStocks = 0; iSucursalStocks < obj.listaSucursalStocks.Count; iSucursalStocks++)
                            {
                                if (pSucursalElejida == obj.listaSucursalStocks[iSucursalStocks].stk_codsuc)
                                {
                                    obj.listaSucursalStocks[iSucursalStocks].cantidadSucursal = obj.cantidad;
                                    break;
                                }
                            }

                        }
                        resultado.Add(obj);
                    }
                }

            }
            return resultado;
        }
        public static List<cArchivo> RecuperarTodosArchivos(int pArc_codRelacion, string pArc_galeria, string pFiltro)
        {
            List<cArchivo> lista = new List<cArchivo>();
            DataSet dsResultado = capaRecurso_base.GestiónArchivo(null, pArc_codRelacion, pArc_galeria, null, null, null, null, null, null, null, null, null, pFiltro, Constantes.cSQL_SELECT);
            if (dsResultado != null)
            {
                foreach (DataRow item in dsResultado.Tables["Archivo"].Rows)
                {
                    lista.Add(ConvertToArchivo(item));
                }
            }
            return lista;

        }
        public static cArchivo ConvertToArchivo(DataRow pItem)
        {
            cArchivo obj = new cArchivo();

            if (pItem["arc_codRecurso"] != DBNull.Value)
            {
                obj.arc_codRecurso = Convert.ToInt32(pItem["arc_codRecurso"]);
            }
            if (pItem["arc_codRelacion"] != DBNull.Value)
            {
                obj.arc_codRelacion = Convert.ToInt32(pItem["arc_codRelacion"]);
            }
            if (pItem["arc_galeria"] != DBNull.Value)
            {
                obj.arc_galeria = pItem["arc_galeria"].ToString();
            }
            if (pItem["arc_orden"] != DBNull.Value)
            {
                obj.arc_orden = Convert.ToInt32(pItem["arc_orden"]);
            }
            if (pItem["arc_tipo"] != DBNull.Value)
            {
                obj.arc_tipo = pItem["arc_tipo"].ToString();
            }
            if (pItem["arc_mime"] != DBNull.Value)
            {
                obj.arc_mime = pItem["arc_mime"].ToString();
            }
            if (pItem["arc_nombre"] != DBNull.Value)
            {
                obj.arc_nombre = pItem["arc_nombre"].ToString();
            }
            if (pItem["arc_titulo"] != DBNull.Value)
            {
                obj.arc_titulo = pItem["arc_titulo"].ToString();
            }
            if (pItem["arc_descripcion"] != DBNull.Value)
            {
                obj.arc_descripcion = pItem["arc_descripcion"].ToString();
            }
            if (pItem["arc_tipo"] != DBNull.Value)
            {
                obj.arc_tipo = pItem["arc_tipo"].ToString();
            }
            if (pItem["arc_codUsuarioUltMov"] != DBNull.Value)
            {
                obj.arc_codUsuarioUltMov = Convert.ToInt32(pItem["arc_codUsuarioUltMov"]);
            }
            if (pItem["NombreYapellido"] != DBNull.Value)
            {
                obj.NombreYapellido = pItem["NombreYapellido"].ToString();
            }
            if (pItem["arc_hash"] != DBNull.Value)
            {
                obj.arc_hash = pItem["arc_hash"].ToString();
            }
            if (pItem["arc_fecha"] != DBNull.Value)
            {
                obj.arc_fecha = Convert.ToDateTime(pItem["arc_fecha"]);
                obj.arc_fechaToString = obj.arc_fecha.ToString();
            }
            if (pItem["arc_fechaUltMov"] != DBNull.Value)
            {
                obj.arc_fechaUltMov = Convert.ToDateTime(pItem["arc_fechaUltMov"]);
            }
            if (pItem["arc_estado"] != DBNull.Value)
            {
                obj.arc_estado = Convert.ToInt32(pItem["arc_estado"]);
                obj.arc_estadoToString = capaSeguridad_base.obtenerStringEstado(obj.arc_estado);
            }
            if (pItem["arc_accion"] != DBNull.Value)
            {
                obj.arc_accion = Convert.ToInt32(pItem["arc_accion"]);
            }
            if (pItem.Table.Columns.Contains("arc_rating") && pItem["arc_rating"] != DBNull.Value)
            {
                obj.arc_rating = Convert.ToInt32(pItem["arc_rating"]);
            }
            return obj;
        }
        public static cProductos ConvertToProductos(DataRow pItem)
        {
            cProductos obj = new cProductos();
            if (pItem.Table.Columns.Contains("pro_codigo") && pItem["pro_codigo"] != DBNull.Value)
            {
                obj.pro_codigo = pItem["pro_codigo"].ToString();
            }
            if (pItem.Table.Columns.Contains("pro_nombre") && pItem["pro_nombre"] != DBNull.Value)
            {
                obj.pro_nombre = pItem["pro_nombre"].ToString();
            }
            if (pItem.Table.Columns.Contains("pro_precio") && pItem["pro_precio"] != DBNull.Value)
            {
                obj.pro_precio = Convert.ToDecimal(pItem["pro_precio"]);
            }
            if (pItem.Table.Columns.Contains("pro_preciofarmacia") && pItem["pro_preciofarmacia"] != DBNull.Value)
            {
                obj.pro_preciofarmacia = Convert.ToDecimal(pItem["pro_preciofarmacia"]);
            }
            if (pItem.Table.Columns.Contains("pro_ofeunidades") && pItem["pro_ofeunidades"] != DBNull.Value)
            {
                obj.pro_ofeunidades = Convert.ToInt32(pItem["pro_ofeunidades"]);
            }
            if (pItem.Table.Columns.Contains("pro_ofeporcentaje") && pItem["pro_ofeporcentaje"] != DBNull.Value)
            {
                obj.pro_ofeporcentaje = Convert.ToDecimal(pItem["pro_ofeporcentaje"]);
            }
            if (pItem.Table.Columns.Contains("pro_neto") && pItem["pro_neto"] != DBNull.Value)
            {
                obj.pro_neto = Convert.ToBoolean(pItem["pro_neto"]);
            }
            if (pItem.Table.Columns.Contains("pro_codtpopro") && pItem["pro_codtpopro"] != DBNull.Value)
            {
                obj.pro_codtpopro = pItem["pro_codtpopro"].ToString().ToUpper();
            }
            if (pItem.Table.Columns.Contains("pro_descuentoweb") && pItem["pro_descuentoweb"] != DBNull.Value)
            {
                obj.pro_descuentoweb = Convert.ToDecimal(pItem["pro_descuentoweb"]);
            }
            if (pItem.Table.Columns.Contains("pro_laboratorio") && pItem["pro_laboratorio"] != DBNull.Value)
            {
                obj.pro_laboratorio = pItem["pro_laboratorio"].ToString();
            }
            if (pItem.Table.Columns.Contains("pro_monodroga") && pItem["pro_monodroga"] != DBNull.Value)
            {
                obj.pro_monodroga = pItem["pro_monodroga"].ToString();
            }
            if (pItem.Table.Columns.Contains("pro_codigobarra") && pItem["pro_codigobarra"] != DBNull.Value)
            {
                obj.pro_codigobarra = pItem["pro_codigobarra"].ToString();
            }
            if (pItem.Table.Columns.Contains("pro_codigoalfabeta") && pItem["pro_codigoalfabeta"] != DBNull.Value)
            {
                obj.pro_codigoalfabeta = pItem["pro_codigoalfabeta"].ToString();
            }
            if (pItem.Table.Columns.Contains("pro_isTrazable") && pItem["pro_isTrazable"] != DBNull.Value)
            {
                obj.pro_isTrazable = Convert.ToBoolean(pItem["pro_isTrazable"]);
            }
            if (pItem.Table.Columns.Contains("pro_isCadenaFrio") && pItem["pro_isCadenaFrio"] != DBNull.Value)
            {
                obj.pro_isCadenaFrio = Convert.ToBoolean(pItem["pro_isCadenaFrio"]);
            }
            if (pItem.Table.Columns.Contains("pro_acuerdo") && pItem["pro_acuerdo"] != DBNull.Value)
            {
                obj.pro_acuerdo = Convert.ToInt32(pItem["pro_acuerdo"]);
            }
            if (pItem.Table.Columns.Contains("pro_PrecioBase") && pItem["pro_PrecioBase"] != DBNull.Value)
            {
                obj.pro_PrecioBase = Convert.ToDecimal(pItem["pro_PrecioBase"]);
            }
            if (pItem.Table.Columns.Contains("pro_PorcARestarDelDtoDeCliente") && pItem["pro_PorcARestarDelDtoDeCliente"] != DBNull.Value)
            {
                obj.pro_PorcARestarDelDtoDeCliente = Convert.ToDecimal(pItem["pro_PorcARestarDelDtoDeCliente"]);
            }
            if (pItem.Table.Columns.Contains("pro_AltoCosto") && pItem["pro_AltoCosto"] != DBNull.Value)
            {
                obj.pro_AltoCosto = Convert.ToBoolean(pItem["pro_AltoCosto"]);
            }
            if (pItem.Table.Columns.Contains("pro_UbicacionPrincipal") && pItem["pro_UbicacionPrincipal"] != DBNull.Value)
            {
                obj.pro_UbicacionPrincipal = Convert.ToString(pItem["pro_UbicacionPrincipal"]);
            }
            return obj;
        }
        public static cTransfer ConvertToTransfer(DataRow pItem)
        {
            cTransfer obj = new cTransfer();
            if (pItem["tfr_codigo"] != DBNull.Value)
            {
                obj.tfr_codigo = Convert.ToInt32(pItem["tfr_codigo"]);
            }
            if (pItem["tfr_accion"] != DBNull.Value)
            {
                obj.tfr_accion = pItem["tfr_accion"].ToString();
            }
            if (pItem["tfr_descripcion"] != DBNull.Value)
            {
                obj.tfr_descripcion = pItem["tfr_descripcion"].ToString();
            }
            if (pItem["tfr_deshab"] != DBNull.Value)
            {
                obj.tfr_deshab = Convert.ToBoolean(pItem["tfr_deshab"]);
            }
            if (pItem["tfr_nombre"] != DBNull.Value)
            {
                obj.tfr_nombre = pItem["tfr_nombre"].ToString();
            }
            if (pItem["tfr_tipo"] != DBNull.Value)
            {
                obj.tfr_tipo = pItem["tfr_tipo"].ToString();
            }
            if (pItem["tfr_mospap"] != DBNull.Value)
            {
                obj.tfr_mospap = Convert.ToBoolean(pItem["tfr_mospap"]);
            }
            if (pItem["tfr_fijunidades"] != DBNull.Value)
            {
                obj.tfr_fijunidades = Convert.ToInt32(pItem["tfr_fijunidades"]);
            }
            if (pItem["tfr_maxunidades"] != DBNull.Value)
            {
                obj.tfr_maxunidades = Convert.ToInt32(pItem["tfr_maxunidades"]);
            }
            if (pItem["tfr_mulunidades"] != DBNull.Value)
            {
                obj.tfr_mulunidades = Convert.ToInt32(pItem["tfr_mulunidades"]);
            }
            if (pItem["tfr_minrenglones"] != DBNull.Value)
            {
                obj.tfr_minrenglones = Convert.ToInt32(pItem["tfr_minrenglones"]);
            }
            if (pItem["tfr_minunidades"] != DBNull.Value)
            {
                obj.tfr_minunidades = Convert.ToInt32(pItem["tfr_minunidades"]);
            }
            if (pItem["tfr_pordesadi"] != DBNull.Value)
            {
                obj.tfr_pordesadi = Convert.ToDecimal(pItem["tfr_pordesadi"]);
            }
            else
            {
                obj.tfr_pordesadi = 0;
            }
            if (pItem.Table.Columns.Contains("tfr_facturaciondirecta"))
            {
                if (pItem["tfr_facturaciondirecta"] != DBNull.Value)
                {
                    obj.tfr_facturaciondirecta = Convert.ToBoolean(pItem["tfr_facturaciondirecta"]);
                }
            }
            if (pItem.Table.Columns.Contains("tfr_provincia"))
            {
                if (pItem["tfr_provincia"] != DBNull.Value)
                {
                    obj.tfr_provincia = Convert.ToString(pItem["tfr_provincia"]);
                }
            }
            return obj;
        }
        public static List<cSucursalCarritoTransfer> convertDataSetToSucursalCarritoTransfer(cClientes pCliente, DataSet dsProductoCarrito)
        {
            if (dsProductoCarrito != null && dsProductoCarrito.Tables.Count > 1)
            {
                List<cSucursalCarritoTransfer> resultado = new List<cSucursalCarritoTransfer>();
                List<cCarritoTransfer> listaSucursal = (from item in dsProductoCarrito.Tables[1].AsEnumerable()
                                                        select new cCarritoTransfer { car_id_aux = item.Table.Columns.Contains("car_id") ? item.Field<int>("car_id") : -1, ctr_id = item.Field<int>("ctr_id"), ctr_codSucursal = item.Field<string>("ctr_codSucursal"), tfr_codigo = item.Field<int>("tfr_codigo"), tfr_nombre = item.Field<string>("tfr_nombre"), tfr_deshab = item.Field<Boolean>("tfr_deshab"), tfr_pordesadi = item.IsNull("tfr_pordesadi") ? null : (decimal?)item.Field<decimal>("tfr_pordesadi"), tfr_tipo = item.Field<string>("tfr_tipo") }).OrderBy(x => x.ctr_codSucursal).ToList();

                /// Nuevo
                List<cTransferDetalle> listaTransferDetalle = null;
                if (dsProductoCarrito.Tables.Count > 2)
                {
                    listaTransferDetalle = new List<cTransferDetalle>();
                    DataTable tablaTransferDetalle = dsProductoCarrito.Tables[2];
                    foreach (DataRow itemTransferDetalle in tablaTransferDetalle.Rows)
                    {
                        cTransferDetalle objTransferDetalle = ConvertToTransferDetalle(itemTransferDetalle);
                        objTransferDetalle.CargarTransfer(ConvertToTransfer(itemTransferDetalle));
                        listaTransferDetalle.Add(objTransferDetalle);

                    }
                }
                /// FIN Nuevo


                foreach (cCarritoTransfer item in listaSucursal)
                {
                    List<cProductosGenerico> listaProductoCarrtios = (from itemProductoCarrtios in dsProductoCarrito.Tables[0].AsEnumerable()
                                                                      where itemProductoCarrtios.Field<int>("ctd_idCarritoTransfers") == item.ctr_id
                                                                      select new cProductosGenerico
                                                                      {
                                                                          codProducto = itemProductoCarrtios.Field<string>("ctd_codProducto"),
                                                                          cantidad = itemProductoCarrtios.Field<int>("ctd_Cantidad"),
                                                                          pro_nombre = itemProductoCarrtios.Field<string>("pro_nombre"),
                                                                          tde_codpro = itemProductoCarrtios.Field<string>("pro_nombre"),
                                                                          pro_precio = itemProductoCarrtios.Field<decimal>("pro_precio"),
                                                                          pro_preciofarmacia = itemProductoCarrtios.Field<decimal>("pro_preciofarmacia"),
                                                                          pro_neto = itemProductoCarrtios.Field<bool>("pro_neto"),
                                                                          pro_codtpopro = itemProductoCarrtios.Field<string>("pro_codtpopro"),
                                                                          pro_descuentoweb = itemProductoCarrtios.IsNull("pro_descuentoweb") ? 0 : itemProductoCarrtios.Field<decimal>("pro_descuentoweb"),
                                                                          pro_ofeunidades = itemProductoCarrtios.IsNull("pro_ofeunidades") ? 0 : itemProductoCarrtios.Field<int>("pro_ofeunidades"),
                                                                          pro_ofeporcentaje = itemProductoCarrtios.IsNull("pro_ofeporcentaje") ? 0 : itemProductoCarrtios.Field<decimal>("pro_ofeporcentaje"),
                                                                          tde_prepublico = itemProductoCarrtios.IsNull("tde_prepublico") ? 0 : itemProductoCarrtios.Field<decimal>("tde_prepublico"),
                                                                          tde_predescuento = itemProductoCarrtios.IsNull("tde_predescuento") ? 0 : itemProductoCarrtios.Field<decimal>("tde_predescuento"),
                                                                          stk_stock = itemProductoCarrtios.Table.Columns.Contains("stk_stock") && !itemProductoCarrtios.IsNull("stk_stock") ? itemProductoCarrtios.Field<string>("stk_stock") : null,
                                                                          PrecioFinalTransfer = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinalTransferBase(pCliente, item.tfr_deshab, item.tfr_pordesadi, itemProductoCarrtios.Field<bool>("pro_neto"), itemProductoCarrtios.Field<string>("pro_codtpopro"), itemProductoCarrtios.IsNull("pro_descuentoweb") ? 0 : itemProductoCarrtios.Field<decimal>("pro_descuentoweb"), itemProductoCarrtios.Field<decimal>("tde_predescuento"), itemProductoCarrtios.IsNull("tde_PrecioConDescuentoDirecto") ? 0 : itemProductoCarrtios.Field<decimal>("tde_PrecioConDescuentoDirecto"), itemProductoCarrtios.IsNull("tde_PorcARestarDelDtoDeCliente") ? 0 : itemProductoCarrtios.Field<decimal>("tde_PorcARestarDelDtoDeCliente"))
                                                                      }).ToList();

                    for (int iProductoCarrtios = 0; iProductoCarrtios < listaProductoCarrtios.Count; iProductoCarrtios++)
                    {
                        listaProductoCarrtios[iProductoCarrtios].isProductoFacturacionDirecta = false;
                        if (listaTransferDetalle != null)
                        {
                            List<cTransferDetalle> listaAUXtransferDetalle = listaTransferDetalle.Where(x => x.tde_codpro == listaProductoCarrtios[iProductoCarrtios].pro_nombre && x.tfr_codigo == item.tfr_codigo).ToList();
                            if (listaAUXtransferDetalle.Count > 0)
                            {
                                listaProductoCarrtios[iProductoCarrtios].isProductoFacturacionDirecta = true;
                                listaProductoCarrtios[iProductoCarrtios].CargarTransferYTransferDetalle(listaAUXtransferDetalle[0]);
                            }
                        }

                    }
                    item.listaProductos = listaProductoCarrtios;
                    bool isAgregoResultado = false;
                    for (int i = 0; i < resultado.Count; i++)
                    {
                        if (resultado[i].Sucursal == item.ctr_codSucursal)
                        {
                            resultado[i].listaTransfer.Add(item);
                            isAgregoResultado = true;
                            break;
                        }
                    }
                    if (!isAgregoResultado)
                    {
                        cSucursalCarritoTransfer obj = new cSucursalCarritoTransfer();
                        obj.listaTransfer = new List<cCarritoTransfer>();
                        obj.car_id = item.car_id_aux;
                        obj.Sucursal = item.ctr_codSucursal;
                        obj.proximoHorarioEntrega = FuncionesPersonalizadas_base.ObtenerHorarioCierre(pCliente, pCliente.cli_codsuc, item.ctr_codSucursal, pCliente.cli_codrep);
                        obj.listaTransfer.Add(item);
                        resultado.Add(obj);
                    }
                }
                resultado.RemoveAll(x => x.listaTransfer.Sum(x1 => x1.listaProductos.Count) == 0);
                return resultado;
            }
            return null;
        }
        public static List<cHorariosSucursal> RecuperarTodosHorariosSucursalDependiente()
        {
            List<cHorariosSucursal> resultado = null;
            DataSet dsResultado = capaClientes_base.GestiónSucursalDependienteHorarios(null, null, null, null, null, null, Constantes.cSQL_SELECT);

            if (dsResultado != null)
            {
                resultado = new List<cHorariosSucursal>();

                foreach (DataRow item in dsResultado.Tables["SucursalHorario"].Rows)
                {
                    cHorariosSucursal obj = new cHorariosSucursal();
                    if (item["sdh_SucursalDependienteHorario"] != DBNull.Value)
                    {
                        obj.sdh_SucursalDependienteHorario = Convert.ToInt32(item["sdh_SucursalDependienteHorario"]);
                    }
                    if (item["sdh_sucursal"] != DBNull.Value)
                    {
                        obj.sdh_sucursal = item["sdh_sucursal"].ToString();
                    }
                    if (item["sdh_sucursalDependiente"] != DBNull.Value)
                    {
                        obj.sdh_sucursalDependiente = item["sdh_sucursalDependiente"].ToString();
                    }
                    if (item["sdh_codReparto"] != DBNull.Value)
                    {
                        obj.sdh_codReparto = item["sdh_codReparto"].ToString();
                    }
                    if (item["sdh_diaSemana"] != DBNull.Value)
                    {
                        obj.sdh_diaSemana = item["sdh_diaSemana"].ToString();
                    }
                    if (item["sdh_horario"] != DBNull.Value)
                    {
                        obj.sdh_horario = item["sdh_horario"].ToString();
                        string[] arrayHorarios = obj.sdh_horario.Split('-');
                        obj.listaHorarios = arrayHorarios.ToList();
                    }
                    resultado.Add(obj);
                }

            }
            return resultado;
        }
        public static cProductos ConvertToProductosImagen(DataRow pItem)
        {
            cProductos obj = new cProductos();
            if (pItem["pro_codigo"] != DBNull.Value)
            {
                obj.pro_codigo = Convert.ToString(pItem["pro_codigo"]);
            }
            if (pItem.Table.Columns.Contains("pro_nombre"))
            {
                if (pItem["pro_nombre"] != DBNull.Value)
                {
                    obj.pro_nombre = Convert.ToString(pItem["pro_nombre"]);
                }
            }
            if (pItem["pri_nombreArchivo"] != DBNull.Value)
            {
                obj.pri_nombreArchivo = Convert.ToString(pItem["pri_nombreArchivo"]);
            }
            if (pItem.Table.Columns.Contains("pri_ancho_ampliar") && pItem["pri_ancho_ampliar"] != DBNull.Value)
            {
                obj.pri_ancho_ampliar_original = Convert.ToInt32(pItem["pri_ancho_ampliar"]);
                obj.pri_ancho_ampliar = Convert.ToInt32(pItem["pri_ancho_ampliar"]);
            }
            if (pItem.Table.Columns.Contains("pri_alto_ampliar") && pItem["pri_alto_ampliar"] != DBNull.Value)
            {
                obj.pri_alto_ampliar_original = Convert.ToInt32(pItem["pri_alto_ampliar"]);
                obj.pri_alto_ampliar = Convert.ToInt32(pItem["pri_alto_ampliar"]);
            }
            return obj;
        }
        public static cPalabraBusqueda ConvertToPalabrasBusquedas(DataRow pItem)
        {
            cPalabraBusqueda obj = new cPalabraBusqueda();
            if (pItem.Table.Columns.Contains("hbp_id") && pItem["hbp_id"] != DBNull.Value)
            {
                obj.hbp_id = Convert.ToInt32(pItem["hbp_id"]);
            }
            if (pItem.Table.Columns.Contains("hbp_Palabra") && pItem["hbp_Palabra"] != DBNull.Value)
            {
                obj.hbp_Palabra = pItem["hbp_Palabra"].ToString();
            }
            return obj;
        }
        public static string RecuperarUltimoProductoSeleccionado(int pIdUsuario)
        {
            return capaLogRegistro_base.spUltimoProductoSeleccionado(pIdUsuario);
        }
        public static List<cClientes> RecuperarIdClientesConCarritos(int pIdUsuario)
        {
            List<cClientes> result = null;
            DataTable tb = capaCAR_intranet_base.spRecuperarIdClientesConCarritos(pIdUsuario);
            if (tb != null && tb.Rows.Count > 0)
            {
                result = new List<cClientes>();
                foreach (DataRow item in tb.Rows)
                {
                    cClientes oCliente = acceso.ConvertToCliente(item);
                    result.Add(oCliente);
                }
            }
            return result;
        }
    }
}
