using DKbase.dll;
using DKbase.generales;
using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DKbase.web
{
    public class FuncionesPersonalizadas_base
    {
        public static decimal ObtenerPrecioFinalTransferBase(cClientes pClientes, bool pTfr_deshab, decimal? pTfr_pordesadi, bool pPro_neto, string pPro_codtpopro, decimal pPro_descuentoweb, decimal pTde_predescuento, decimal pTde_PrecioConDescuentoDirecto, decimal pTde_PorcARestarDelDtoDeCliente)
        {
            decimal resultado = new decimal(0.0);
            resultado = pTde_predescuento;
            if (pTfr_deshab)
            {
                if (pPro_neto)
                {   // Neto        
                    switch (pPro_codtpopro)
                    {
                        case "M": // medicamento
                            resultado = getPrecioConDescuentoTransfer(pTde_PrecioConDescuentoDirecto, pTde_PorcARestarDelDtoDeCliente, pClientes.cli_pordesbetmed);
                            break;
                        case "P": // Perfumeria
                        case "A": // Accesorio
                        case "V": // Accesorio
                            resultado = getPrecioConDescuentoTransfer(pTde_PrecioConDescuentoDirecto, pTde_PorcARestarDelDtoDeCliente, pClientes.cli_pordesnetos);
                            break;
                        default:
                            break;
                    }
                }
                else
                {  // No neto   
                    resultado = getPrecioConDescuentoTransfer(pTde_PrecioConDescuentoDirecto, pTde_PorcARestarDelDtoDeCliente, pClientes.cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto);
                }
            }
            if (pTfr_pordesadi != null)
            {
                resultado = resultado * (Convert.ToDecimal(1) - ((decimal)pTfr_pordesadi / Convert.ToDecimal(100)));
            }
            return resultado;
        }
        private static decimal getPrecioConDescuentoTransfer(decimal pTde_PrecioConDescuentoDirecto, decimal pTde_PorcARestarDelDtoDeCliente, decimal pDescuentoRestar)
        {
            decimal descuento = pDescuentoRestar - pTde_PorcARestarDelDtoDeCliente;
            if (descuento < 0)
                descuento = 0;
            decimal resultado = pTde_PrecioConDescuentoDirecto;
            resultado = resultado * (Convert.ToDecimal(1) - (descuento / Convert.ToDecimal(100)));
            return resultado;
        }
        public static decimal ObtenerPrecioFinal(cClientes pClientes, cProductos pProductos)
        {
            decimal resultado = new decimal(0.0);
            if (pProductos.pro_neto)
            {
                switch (pProductos.pro_codtpopro)
                {
                    case "M": // medicamento
                        resultado = getPrecioBaseConDescuento(pProductos, pClientes.cli_pordesbetmed);
                        break;
                    case "P": // Perfumeria
                    case "A": // Accesorio
                    case "V": // Accesorio
                        resultado = getPrecioBaseConDescuento(pProductos, pClientes.cli_pordesnetos);
                        break;
                    default:
                        break;
                }
            }
            else
            {  // No neto   
                resultado = getPrecioBaseConDescuento(pProductos, pClientes.cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto);
            }
            return resultado;
        }
        private static decimal getPrecioBaseConDescuento(cProductos pProductos, decimal pDescuentoRestar)
        {
            decimal descuento = pDescuentoRestar - pProductos.pro_PorcARestarDelDtoDeCliente;
            if (descuento < 0)
                descuento = 0;
            decimal resultado = pProductos.pro_PrecioBase;
            resultado = resultado * (Convert.ToDecimal(1) - (descuento / Convert.ToDecimal(100)));
            return resultado;
        }
        public static decimal ObtenerPrecioUnitarioConDescuentoOferta(decimal pPrecioFinal, cProductos pProductos)
        {
            decimal resultado = Convert.ToDecimal(0);
            if (pProductos.pro_ofeporcentaje == 0 || pProductos.pro_ofeunidades == 0)
            {
                resultado = pPrecioFinal;
            }
            else
            {
                resultado = (pPrecioFinal * (Convert.ToDecimal(1) - (pProductos.pro_ofeporcentaje / Convert.ToDecimal(100))));
            }
            return resultado;
        }
        public static DataTable ConvertProductosAndCantidadToDataTable(List<cProductosAndCantidad> pListaProductosMasCantidad)
        {

            DataTable pTablaDetalle = new DataTable();
            pTablaDetalle.Columns.Add(new DataColumn("codProducto", System.Type.GetType("System.String")));
            pTablaDetalle.Columns.Add(new DataColumn("cantidad", System.Type.GetType("System.Int32")));
            foreach (cProductosAndCantidad item in pListaProductosMasCantidad)
            {
                DataRow fila = pTablaDetalle.NewRow();
                fila["codProducto"] = item.codProductoNombre;
                fila["cantidad"] = item.cantidad;
                pTablaDetalle.Rows.Add(fila);
            }
            return pTablaDetalle;
        }
        public static string ObtenerHorarioCierre_base(cClientes pClientes, string pSucursal, DateTime fechaActual)
        {
            string resultado = null;
            List<cHorariosSucursal> listaHorariosSucursal = acceso.RecuperarTodosHorariosSucursalDependiente().Where(x => x.sdh_sucursal == pClientes.cli_codsuc && x.sdh_sucursalDependiente == pSucursal && x.sdh_codReparto == pClientes.cli_codrep).ToList();
            string day = string.Empty;
            bool isEncontroEnFechaHoy = false;
            switch (fechaActual.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    day = Constantes.cDIASEMANA_Lunes;
                    break;
                case DayOfWeek.Tuesday:
                    day = Constantes.cDIASEMANA_Martes;
                    break;
                case DayOfWeek.Wednesday:
                    day = Constantes.cDIASEMANA_Miercoles;
                    break;
                case DayOfWeek.Thursday:
                    day = Constantes.cDIASEMANA_Jueves;
                    break;
                case DayOfWeek.Friday:
                    day = Constantes.cDIASEMANA_Viernes;
                    break;
                case DayOfWeek.Saturday:
                    day = Constantes.cDIASEMANA_Sabado;
                    break;
                case DayOfWeek.Sunday:
                    day = Constantes.cDIASEMANA_Domingo;
                    break;
                default:
                    break;
            }
            if (day != string.Empty)
            {
                foreach (cHorariosSucursal itemHorariosSucursal in listaHorariosSucursal)
                {
                    if (itemHorariosSucursal.sdh_diaSemana == day)
                    {
                        if (itemHorariosSucursal.listaHorarios != null)
                        {
                            foreach (string itemHorarios in itemHorariosSucursal.listaHorarios)
                            {
                                string[] tiempo = itemHorarios.Split(':');
                                if (tiempo.Count() > 1)
                                {
                                    DateTime fechaCierre = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, Convert.ToInt32(tiempo[0]), Convert.ToInt32(tiempo[1]), 30);
                                    if (fechaCierre > fechaActual)
                                    {
                                        isEncontroEnFechaHoy = true;
                                        resultado = itemHorarios + " hs. ";
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }

            }
            int SumaDia = 0;
            while (!isEncontroEnFechaHoy)
            {
                day = string.Empty;
                SumaDia += 1;
                DateTime fechaOtroDia = fechaActual.AddDays(SumaDia);
                switch (fechaOtroDia.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        day = Constantes.cDIASEMANA_Lunes;
                        break;
                    case DayOfWeek.Tuesday:
                        day = Constantes.cDIASEMANA_Martes;
                        break;
                    case DayOfWeek.Wednesday:
                        day = Constantes.cDIASEMANA_Miercoles;
                        break;
                    case DayOfWeek.Thursday:
                        day = Constantes.cDIASEMANA_Jueves;
                        break;
                    case DayOfWeek.Friday:
                        day = Constantes.cDIASEMANA_Viernes;
                        break;
                    case DayOfWeek.Saturday:
                        day = Constantes.cDIASEMANA_Sabado;
                        break;
                    case DayOfWeek.Sunday:
                        day = Constantes.cDIASEMANA_Domingo;
                        break;
                    default:
                        break;
                }
                if (day != string.Empty)
                {
                    foreach (cHorariosSucursal itemHorariosSucursal in listaHorariosSucursal)
                    {
                        if (itemHorariosSucursal.sdh_diaSemana == day)
                        {
                            if (itemHorariosSucursal.listaHorarios != null)
                            {
                                foreach (string itemHorarios in itemHorariosSucursal.listaHorarios)
                                {
                                    string[] tiempo = itemHorarios.Split(':');
                                    if (tiempo.Count() > 1)
                                    {
                                        DateTime fechaCierre = new DateTime(fechaOtroDia.Year, fechaOtroDia.Month, fechaOtroDia.Day, Convert.ToInt32(tiempo[0]), Convert.ToInt32(tiempo[1]), 30);
                                        if (fechaCierre > fechaActual)
                                        {
                                            isEncontroEnFechaHoy = true;
                                            resultado = itemHorarios + " hs. " + day;
                                            break;
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
                if (SumaDia > 7)
                {
                    isEncontroEnFechaHoy = true;
                }
            } // fin while (!isEncontroEnFechaHoy)

            // Inicio S7
            if (pClientes.cli_codrep == "S7" && pSucursal == "SF")
            {
                DateTime fechaCierre = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 22, 15, 30);
                resultado = "22:15" + " hs. ";
                switch (fechaActual.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (fechaCierre < fechaActual)
                            resultado = "22:15" + " hs. " + "MA";
                        break;
                    case DayOfWeek.Tuesday:
                        if (fechaCierre < fechaActual)
                            resultado = "22:15" + " hs. " + "MI";
                        break;
                    case DayOfWeek.Wednesday:
                        if (fechaCierre < fechaActual)
                            resultado = "22:15" + " hs. " + "JU";
                        break;
                    case DayOfWeek.Thursday:
                        if (fechaCierre < fechaActual)
                            resultado = "22:15" + " hs. " + "VI";
                        break;
                    case DayOfWeek.Friday:
                        if (fechaCierre < fechaActual)
                            resultado = "22:15" + " hs. " + "LU";
                        break;
                    case DayOfWeek.Saturday:
                        resultado = "22:15" + " hs. " + "LU";
                        break;
                    case DayOfWeek.Sunday:
                        resultado = "22:15" + " hs. " + "LU";
                        break;
                    default:
                        break;
                }
            }
            // Fin S7
            return resultado;
        }
        public static DateTime? getFecha_Horario(string pHorarioCierre)
        {
            DateTime? result = null;
            if (!string.IsNullOrEmpty(pHorarioCierre))
            {
                try
                {
                    DateTime hoy = DateTime.Now;
                    if (pHorarioCierre.Length == 12)
                    {
                        //var diaSemana = pHorarioCierre.Substring(10, 2);
                        //pHorarioCierre = pHorarioCierre.Replace(" hs. " + diaSemana, "");
                        //var values = pHorarioCierre.Split(':');
                        //var d = new DateTime(hoy.Year, hoy.Month, hoy.Day, Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), 0);// mes 0 = enero
                        //result = d;
                        var diaSemana = pHorarioCierre.Substring(10, 2);
                        var diaSemanaNro = -1;
                        //Note: Sunday is 0, Monday is 1, and so on || from 0 to 6
                        // LU = 1
                        // MA = 2
                        // MI = 3
                        // JU = 4
                        // VI = 5
                        // SA = 6
                        // DO = 0
                        switch (diaSemana)
                        {
                            case "LU":
                                diaSemanaNro = 1;
                                break;
                            case "MA":
                                diaSemanaNro = 2;
                                break;
                            case "MI":
                                diaSemanaNro = 3;
                                break;
                            case "JU":
                                diaSemanaNro = 4;
                                break;
                            case "VI":
                                diaSemanaNro = 5;
                                break;
                            case "SA":
                                diaSemanaNro = 6;
                                break;
                            case "DO":
                                diaSemanaNro = 0;
                                break;
                            default:
                                break;
                        }
                        pHorarioCierre = pHorarioCierre.Replace(" hs. " + diaSemana, "");
                        var values = pHorarioCierre.Split(':');
                        var d = new DateTime(hoy.Year, hoy.Month, hoy.Day, Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), 0);// mes 0 = enero
                                                                                                                                      //var n = d.DayOfWeek;
                        var sumaDia = 0;
                        while ((int)d.DayOfWeek != diaSemanaNro)
                        {
                            sumaDia++;
                            d = new DateTime(hoy.Year, hoy.Month, hoy.Day, Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), 50);// mes 0 = enero
                            d = d.AddDays(sumaDia);
                            if (sumaDia > 7 || (int)d.DayOfWeek == diaSemanaNro)
                                break;
                        }
                        result = d;
                    }
                    else if (pHorarioCierre.Length == 10)
                    {
                        pHorarioCierre = pHorarioCierre.Replace(" hs. ", "");
                        var values = pHorarioCierre.Split(':');
                        var d = new DateTime(hoy.Year, hoy.Month, hoy.Day, Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), 0);// mes 0 = enero
                        result = d;
                    }

                }
                catch (Exception ex)
                {
                    Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pHorarioCierre);
                }
            }

            return result;
        }
        public static string ObtenerHorarioCierre(cClientes pClientes, string pSucursal, string pSucursalDependiente, string pCodigoReparto)
        {
            return ObtenerHorarioCierre_base(pClientes, pSucursalDependiente, DateTime.Now);
        }
        public static string ObtenerHorarioCierreAnterior(cClientes pClientes, string pSucursal, string pHorarioCierre)
        {
            if (string.IsNullOrEmpty(pHorarioCierre))
                return string.Empty;
            string result = string.Empty;
            try
            {


                DateTime fechaCuentaRegresiva;
                DateTime hoy = DateTime.Now;
                if (pHorarioCierre.Length == 12)
                {
                    var diaSemana = pHorarioCierre.Substring(10, 2);
                    var diaSemanaNro = -1;
                    //Note: Sunday is 0, Monday is 1, and so on || from 0 to 6
                    // LU = 1
                    // MA = 2
                    // MI = 3
                    // JU = 4
                    // VI = 5
                    // SA = 6
                    // DO = 0
                    switch (diaSemana)
                    {
                        case "LU":
                            diaSemanaNro = 1;
                            break;
                        case "MA":
                            diaSemanaNro = 2;
                            break;
                        case "MI":
                            diaSemanaNro = 3;
                            break;
                        case "JU":
                            diaSemanaNro = 4;
                            break;
                        case "VI":
                            diaSemanaNro = 5;
                            break;
                        case "SA":
                            diaSemanaNro = 6;
                            break;
                        case "DO":
                            diaSemanaNro = 0;
                            break;
                        default:
                            break;
                    }
                    pHorarioCierre = pHorarioCierre.Replace(" hs. " + diaSemana, "");
                    var values = pHorarioCierre.Split(':');
                    var d = new DateTime(hoy.Year, hoy.Month, hoy.Day, Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), 0);// mes 0 = enero
                                                                                                                                  //var n = d.DayOfWeek;
                    var sumaDia = 0;
                    while ((int)d.DayOfWeek != diaSemanaNro)
                    {
                        sumaDia++;
                        d = new DateTime(hoy.Year, hoy.Month, hoy.Day + sumaDia, Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), 50);// mes 0 = enero
                        if (sumaDia > 7 || (int)d.DayOfWeek == diaSemanaNro)
                            break;
                    }
                    fechaCuentaRegresiva = d;

                }
                else
                {
                    pHorarioCierre = pHorarioCierre.Replace(" hs.", "");
                    var values = pHorarioCierre.Split(':');
                    fechaCuentaRegresiva = new DateTime(hoy.Year, hoy.Month, hoy.Day, Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), 50);// mes 0 = enero
                }
                result = ObtenerHorarioCierre_base(pClientes, pSucursal, fechaCuentaRegresiva);
            }
            catch (Exception ex)
            {

                var oo = 1;
            }
            return result;
        }
        public static cjSonBuscadorProductos RecuperarProductosGeneral_V3(List<string> l_Sucursales, cClientes pClientes, int? pIdOferta, string pTxtBuscador, List<string> pListaColumna, bool pIsOrfeta, bool pIsTransfer)
        {
            cjSonBuscadorProductos resultado = null;
            if (pClientes != null)
            {

                List<cProductosGenerico> listaProductosBuscador = RecuperarTodosProductosDesdeBuscadorV3(pClientes, pIdOferta, pTxtBuscador, pListaColumna, pClientes.cli_codsuc, pClientes.cli_codigo, pIsOrfeta, pIsTransfer, pClientes.cli_codprov);
                if (listaProductosBuscador != null)
                {
                    // TIPO CLIENTE
                    if (pClientes.cli_tipo == Constantes.cTipoCliente_Perfumeria) // Solamente perfumeria
                    {
                        listaProductosBuscador = listaProductosBuscador.Where(x => x.pro_codtpopro == Constantes.cTIPOPRODUCTO_Perfumeria || x.pro_codtpopro == Constantes.cTIPOPRODUCTO_PerfumeriaCuentaYOrden).ToList();
                    }
                    else if (pClientes.cli_tipo == Constantes.cTipoCliente_Todos) // Todos los productos
                    {
                        // Si el cliente no toma perfumeria
                        if (!pClientes.cli_tomaPerfumeria)
                        {
                            listaProductosBuscador = listaProductosBuscador.Where(x => x.pro_codtpopro != Constantes.cTIPOPRODUCTO_Perfumeria && x.pro_codtpopro != Constantes.cTIPOPRODUCTO_PerfumeriaCuentaYOrden).ToList();
                        }
                        // fin Si el cliente no toma perfumeria
                    }
                    // FIN TIPO CLIENTE
                    //for (int iPrecioFinal = 0; iPrecioFinal < listaProductosBuscador.Count; iPrecioFinal++)
                    //{
                    //    listaProductosBuscador[iPrecioFinal].PrecioFinal = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioFinal(pClientes, listaProductosBuscador[iPrecioFinal]);
                    //    listaProductosBuscador[iPrecioFinal].PrecioConDescuentoOferta = DKbase.web.FuncionesPersonalizadas_base.ObtenerPrecioUnitarioConDescuentoOferta(listaProductosBuscador[iPrecioFinal].PrecioFinal, listaProductosBuscador[iPrecioFinal]);
                    //}

                    //List<cProductos> listaProductosConImagen = ObtenerProductosImagenes();
                    //for (int iImagen = 0; iImagen < listaProductosBuscador.Count; iImagen++)
                    //{
                    //    cProductos objImagen = listaProductosConImagen.Where(x => x.pro_codigo == listaProductosBuscador[iImagen].pro_codigo).FirstOrDefault();
                    //    if (objImagen != null)
                    //    {
                    //        listaProductosBuscador[iImagen].pri_nombreArchivo = objImagen.pri_nombreArchivo;
                    //        listaProductosBuscador[iImagen].pri_ancho_ampliar = objImagen.pri_ancho_ampliar;
                    //        listaProductosBuscador[iImagen].pri_alto_ampliar = objImagen.pri_alto_ampliar;
                    //        listaProductosBuscador[iImagen].pri_ancho_ampliar_original = objImagen.pri_ancho_ampliar_original;
                    //        listaProductosBuscador[iImagen].pri_alto_ampliar_original = objImagen.pri_alto_ampliar_original;
                    //    }
                    //}

                    // Inicio 17/02/2016
                    // List<string> ListaSucursal =  RecuperarSucursalesParaBuscadorDeCliente(pClientes);
                    listaProductosBuscador = ActualizarStockListaProductos(pClientes, l_Sucursales, listaProductosBuscador);
                    // Fin 17/02/2016

                    cjSonBuscadorProductos ResultadoObj = new cjSonBuscadorProductos();
                    ResultadoObj.listaSucursal = l_Sucursales;
                    ResultadoObj.listaProductos = listaProductosBuscador;
                    resultado = ResultadoObj;
                }
            }
            return resultado;
        }
        public static string GenerarWhereLikeConColumna(string pTxtBuscador, string pColumna)
        {
            string where = string.Empty;
            string[] palabras = pTxtBuscador.Split(new char[] { ' ' });
            bool isPrimerWhere = true;
            foreach (string item in palabras)
            {
                if (item != string.Empty)
                {
                    if (isPrimerWhere)
                    {
                        isPrimerWhere = false;
                    }
                    else
                    {
                        where += " AND ";
                    }
                    where += " " + pColumna + " collate SQL_Latin1_General_Cp1_CI_AI like '%" + item + "%' ";
                }
            }
            return where;
        }
        public static string GenerarWhereLikeConColumna_EmpiezaCon(string pTxtBuscador, string pColumna)
        {
            string where = string.Empty;
            string[] palabras = pTxtBuscador.Split(new char[] { ' ' });
            //bool isPrimerWhere = true;
            foreach (string item in palabras)
            {
                if (item != string.Empty)
                {
                    where += " " + pColumna + " collate SQL_Latin1_General_Cp1_CI_AI like '" + item + "%' ";
                    break;
                }
            }
            return where;
        }
        public static string GenerarWhereLikeConVariasColumnas(string pTxtBuscador, List<string> pListaColumna)
        {
            string where = string.Empty;
            string[] palabras = pTxtBuscador.Split(new char[] { ' ' });
            bool isPrimerWhere = true;
            foreach (string item in palabras)
            {
                if (item != string.Empty)
                {
                    if (isPrimerWhere)
                    {
                        isPrimerWhere = false;
                        where += " ( ";
                    }
                    else
                    {
                        where += " AND ( ";
                    }
                    for (int i = 0; i < pListaColumna.Count; i++)
                    {
                        if (i != 0)
                        {
                            where += " OR ";
                        }
                        where += " " + pListaColumna[i] + " collate SQL_Latin1_General_Cp1_CI_AI like '%" + item + "%' ";
                    }
                    where += " ) ";
                }
            }
            return where;
        }
        public static List<cProductosGenerico> RecuperarTodosProductosDesdeBuscadorV3(cClientes pClientes, int? pIdOferta, string pTxtBuscador, List<string> pListaColumna, string pSucursal, int? pIdCliente, bool pIsOferta, bool pIsTransfer, string pCli_codprov)
        {
            List<cProductosGenerico> resultado = null;
            DataSet dsResultado = null;
            if (pIdOferta == null)
                dsResultado = capaProductos_base.RecuperarTodosProductosBuscadorV3(pTxtBuscador, pListaColumna, pSucursal, pIdCliente, pCli_codprov);
            else
                dsResultado = capaProductos_base.RecuperarTodosProductosBuscadorOferta(pIdOferta.Value, pSucursal, pIdCliente, pCli_codprov);
            if (dsResultado != null)
            {
                DataTable tablaProductos = dsResultado.Tables[2];
                DataTable tablaSucursalStocks = dsResultado.Tables[1];
                tablaProductos.Merge(dsResultado.Tables[0]);
                List<cTransferDetalle> listaTransferDetalle = null;
                if (pIsTransfer)
                {
                    if (dsResultado.Tables.Count > 3)
                    {
                        listaTransferDetalle = new List<cTransferDetalle>();
                        DataTable tablaTransferDetalle = dsResultado.Tables[3];
                        foreach (DataRow itemTransferDetalle in tablaTransferDetalle.Rows)
                        {
                            cTransferDetalle objTransferDetalle = acceso.ConvertToTransferDetalle(itemTransferDetalle);
                            objTransferDetalle.CargarTransfer(acceso.ConvertToTransfer(itemTransferDetalle));
                            listaTransferDetalle.Add(objTransferDetalle);
                        }
                    }
                }
                resultado = acceso.cargarProductosBuscadorArchivos(pClientes, tablaProductos, tablaSucursalStocks, listaTransferDetalle, DKbase.generales.Constantes.CargarProductosBuscador.isDesdeBuscador, null);
            }
            return resultado;
        }
        public static List<string> RecuperarSucursalesParaBuscadorDeCliente(cClientes pClientes)
        {
            // Optimizar
            List<string> ListaSucursal = new List<string>();
            ListaSucursal.Add(pClientes.cli_codsuc);
            if (pClientes.cli_codrep == "S7")
            {
                if (!ListaSucursal.Contains("SF"))
                    ListaSucursal.Add("SF");
                if (!ListaSucursal.Contains("CC"))
                    ListaSucursal.Add("CC");
            }
            else
            {
                List<cSucursal> listaSucursalesAUX = RecuperarTodasSucursalesDependientes().Where(x => x.sde_sucursal == pClientes.cli_codsuc).ToList();
                foreach (cSucursal itemSucursalesAUX in listaSucursalesAUX)
                {
                    if (itemSucursalesAUX.sde_sucursalDependiente != pClientes.cli_codsuc)
                    {
                        ListaSucursal.Add(itemSucursalesAUX.sde_sucursalDependiente);
                    }
                }
                if (pClientes.cli_IdSucursalAlternativa != null &&
                    !ListaSucursal.Contains(pClientes.cli_IdSucursalAlternativa))
                {
                    ListaSucursal.Add(pClientes.cli_IdSucursalAlternativa);
                }
            }
            return ListaSucursal;
            // Fin Optimizar
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
        public static List<cProductos> ObtenerProductosImagenes()
        {
            List<cProductos> resultado = null;
            DataTable tabla = capaProductos_base.ObtenerProductosImagenes();
            if (tabla != null)
            {
                resultado = new List<cProductos>();
                foreach (DataRow item in tabla.Rows)
                {
                    cProductos obj = acceso.ConvertToProductosImagen(item);
                    if (obj != null)
                        resultado.Add(obj);
                }
            }
            return resultado;
        }
        public static List<cProductosGenerico> ActualizarStockListaProductos(cClientes pClientes, List<string> pListaSucursal, List<cProductosGenerico> pListaProductos)
        {
            bool isActualizar = false;
            if (pClientes.cli_codrep == "S7")
                isActualizar = true;

            else if (pClientes.cli_IdSucursalAlternativa != null)
                isActualizar = true;
            if (isActualizar)
            {
                List<cProductosAndCantidad> listaProductos = new List<cProductosAndCantidad>();
                foreach (cProductosGenerico item in pListaProductos)
                {
                    listaProductos.Add(new cProductosAndCantidad { codProductoNombre = item.pro_codigo });
                }
                DataTable table = capaProductos_base.RecuperarStockPorProductosAndSucursal(ConvertNombresSeccionToDataTable(pListaSucursal), DKbase.web.FuncionesPersonalizadas_base.ConvertProductosAndCantidadToDataTable(listaProductos));
                if (table != null)
                    for (int i = 0; i < pListaProductos.Count; i++)
                    {
                        pListaProductos[i].listaSucursalStocks = (from r in table.Select("stk_codpro = '" + pListaProductos[i].pro_codigo + "'").AsEnumerable()
                                                                  select new cSucursalStocks { stk_codpro = r["stk_codpro"].ToString(), stk_codsuc = r["stk_codsuc"].ToString(), stk_stock = r["stk_stock"].ToString() }).ToList();
                    }
            }
            return pListaProductos;
        }
        public static DataTable ConvertNombresSeccionToDataTable(List<string> pListaNombreSeccion)
        {
            DataTable pTablaDetalle = new DataTable();
            pTablaDetalle.Columns.Add(new DataColumn("NombreSeccion", System.Type.GetType("System.String")));
            if (pListaNombreSeccion != null)
            {
                foreach (string item in pListaNombreSeccion)
                {
                    DataRow fila = pTablaDetalle.NewRow();
                    fila["NombreSeccion"] = item;
                    pTablaDetalle.Rows.Add(fila);
                }
            }
            return pTablaDetalle;
        }
        public static cjSonBuscadorProductos RecuperarProductosGeneral_OfertaTransfer(List<string> l_Sucursales, cClientes pClientes, bool pIsOrfeta, bool pIsTransfer)
        {
            cjSonBuscadorProductos resultado = null;

            List<cProductosGenerico> listaProductosBuscador = listaProductosBuscador = capaCAR_WebService_base.RecuperarTodosProductosDesdeBuscador_OfertaTransfer(pClientes, pClientes.cli_codsuc, pClientes.cli_codigo, pIsOrfeta, pIsTransfer, pClientes.cli_codprov);
            if (listaProductosBuscador != null)
            {
                // TIPO CLIENTE
                if (pClientes.cli_tipo == Constantes.cTipoCliente_Perfumeria) // Solamente perfumeria
                {
                    listaProductosBuscador = listaProductosBuscador.Where(x => x.pro_codtpopro == Constantes.cTIPOPRODUCTO_Perfumeria || x.pro_codtpopro == Constantes.cTIPOPRODUCTO_PerfumeriaCuentaYOrden).ToList();
                }
                else if (pClientes.cli_tipo == Constantes.cTipoCliente_Todos) // Todos los productos
                {
                    // Si el cliente no toma perfumeria
                    if (!pClientes.cli_tomaPerfumeria)
                    {
                        listaProductosBuscador = listaProductosBuscador.Where(x => x.pro_codtpopro != Constantes.cTIPOPRODUCTO_Perfumeria && x.pro_codtpopro != Constantes.cTIPOPRODUCTO_PerfumeriaCuentaYOrden).ToList();
                    }
                    // fin Si el cliente no toma perfumeria
                }
                // FIN TIPO CLIENTE

                //List<string> ListaSucursal = DKbase.web.FuncionesPersonalizadas_base.RecuperarSucursalesParaBuscadorDeCliente(pClientes);
                listaProductosBuscador = ActualizarStockListaProductos(pClientes, l_Sucursales, listaProductosBuscador);

                // Fin 17/02/2016
                cjSonBuscadorProductos ResultadoObj = new cjSonBuscadorProductos();
                ResultadoObj.listaSucursal = l_Sucursales;
                ResultadoObj.listaProductos = listaProductosBuscador;
                resultado = ResultadoObj;
            }

            return resultado;
        }
        public static List<cProductosGenerico> ActualizarStockListaProductos_SubirArchico(cClientes pCliente, List<string> pListaSucursal, List<cProductosGenerico> pListaProductos, string pSucursalElegida)
        {
            pListaProductos = ActualizarStockListaProductos(pCliente, pListaSucursal, pListaProductos);
            List<DKbase.web.cSucursal> listaSucursal = capaCAR_WebService_base.RecuperarTodasSucursales();
            bool trabajaPerfumeria = true;
            for (int i = 0; i < listaSucursal.Count; i++)
            {
                if (listaSucursal[i].suc_codigo == pSucursalElegida)
                {
                    trabajaPerfumeria = listaSucursal[i].suc_trabajaPerfumeria;
                }
            }
            string sucElegida = pSucursalElegida;
            bool isActualizar = false;
            if (pCliente.cli_codrep == "S7")
                isActualizar = true;
            else if (pCliente.cli_IdSucursalAlternativa != null)
                isActualizar = true;
            if (isActualizar || !trabajaPerfumeria)
            {
                for (int i = 0; i < pListaProductos.Count; i++)
                {
                    if (pListaProductos[i].pro_codtpopro == "P" && !trabajaPerfumeria)
                    {
                        sucElegida = "CC";
                    }
                    else
                    {
                        sucElegida = pSucursalElegida;
                    }
                    foreach (cSucursalStocks item in pListaProductos[i].listaSucursalStocks)
                    {
                        if (item.stk_codsuc == sucElegida)
                        {
                            item.cantidadSucursal = pListaProductos[i].cantidad;
                        }
                    }
                }
            }
            return pListaProductos;
        }
        public static DataTable ObtenerDataTableProductosCarritoArchivosPedidos()
        {
            DataTable pTablaDetalle = new DataTable();
            pTablaDetalle.Columns.Add(new DataColumn("codProducto", System.Type.GetType("System.String")));
            pTablaDetalle.Columns.Add(new DataColumn("codigobarra", System.Type.GetType("System.String")));
            pTablaDetalle.Columns.Add(new DataColumn("codigoalfabeta", System.Type.GetType("System.String")));
            pTablaDetalle.Columns.Add(new DataColumn("troquel", System.Type.GetType("System.String")));
            pTablaDetalle.Columns.Add(new DataColumn("cantidad", System.Type.GetType("System.Int32")));
            pTablaDetalle.Columns.Add(new DataColumn("nroordenamiento", System.Type.GetType("System.Int32")));
            return pTablaDetalle;
        }
        public static List<int> CargarProductoCantidadDependiendoTransfer(cClientes pCliente, cProductosGenerico pProducto, int pCantidad)
        {
            List<int> resultado = new List<int>();
            bool isPasarDirectamente = false;
            int cantidadCarritoTransfer = 0;
            int cantidadCarritoComun = 0;

            if (pProducto.isProductoFacturacionDirecta)
            { // facturacion directa
              // Combiene transfer o promocion                      
                decimal precioConDescuentoDependiendoCantidad = ObtenerPrecioUnitarioConDescuentoOfertaSiLlegaConLaCantidad(pCliente, pProducto, pCantidad);
                if (precioConDescuentoDependiendoCantidad > pProducto.PrecioFinalTransfer)
                {
                    var isSumarTransfer = false;
                    if (pProducto.tde_muluni != null && pProducto.tde_unidadesbonificadas != null)
                    {
                        /// UNIDAD MULTIPLO Y BONIFICADA
                        if ((pCantidad >= (int)pProducto.tde_muluni) && (pCantidad <= ((int)pProducto.tde_muluni + (int)pProducto.tde_unidadesbonificadas)))
                        {
                            // es multiplo
                            isSumarTransfer = true;
                            cantidadCarritoTransfer = (int)pProducto.tde_muluni + (int)pProducto.tde_unidadesbonificadas;
                        }
                        else if (pCantidad > ((int)pProducto.tde_muluni + (int)pProducto.tde_unidadesbonificadas))
                        {
                            isSumarTransfer = true;
                            int cantidadMultiplicar = Convert.ToInt32(Math.Truncate(Convert.ToDouble(pCantidad) / Convert.ToDouble(pProducto.tde_muluni)));
                            cantidadCarritoTransfer = cantidadMultiplicar * ((int)pProducto.tde_muluni + (int)pProducto.tde_unidadesbonificadas);
                            //
                            for (int iCantMulti = 0; iCantMulti < cantidadMultiplicar; iCantMulti++)
                            {
                                int cantTemp = iCantMulti * ((int)pProducto.tde_muluni + (int)pProducto.tde_unidadesbonificadas);
                                if (cantTemp >= pCantidad)
                                {
                                    cantidadCarritoTransfer = cantTemp;
                                    break;
                                }
                            }
                            //
                            if (cantidadCarritoTransfer == pCantidad)
                            {

                            }
                            else
                            {
                                if (pCantidad < cantidadCarritoTransfer)
                                {
                                    cantidadCarritoComun = 0;
                                }
                                else
                                {
                                    cantidadCarritoComun = pCantidad - cantidadCarritoTransfer;
                                }
                                if ((cantidadCarritoComun >= (int)pProducto.tde_muluni) && (cantidadCarritoComun <= ((int)pProducto.tde_muluni + (int)pProducto.tde_unidadesbonificadas)))
                                {
                                    cantidadCarritoTransfer += (int)pProducto.tde_muluni + (int)pProducto.tde_unidadesbonificadas;
                                    cantidadCarritoComun = 0;
                                }
                            }
                        }
                        if (isSumarTransfer)
                        {

                        }
                        else
                        {
                            isPasarDirectamente = true;
                        }
                        /// FIN UNIDAD MULTIPLO Y BONIFICADA
                    } // fin if (listaProductosBuscados[pFila].tde_muluni != null && listaProductosBuscados[pFila].tde_unidadesbonificadas != null){
                    else if (pProducto.tde_fijuni != null)
                    {
                        // UNIDAD FIJA
                        if (pCantidad == (int)pProducto.tde_fijuni)
                        {
                            isSumarTransfer = true;
                            cantidadCarritoTransfer = (int)pProducto.tde_fijuni;
                        }
                        else if (pCantidad > (int)pProducto.tde_fijuni)
                        {
                            isSumarTransfer = true;
                            cantidadCarritoTransfer = (int)pProducto.tde_fijuni;
                            cantidadCarritoComun = pCantidad - (int)pProducto.tde_fijuni;
                        }
                        if (isSumarTransfer)
                        {

                        }
                        else
                        {
                            isPasarDirectamente = true;
                        }
                        // FIN UNIDAD FIJA
                    }
                    else if (pProducto.tde_minuni != null && pProducto.tde_maxuni != null)
                    {
                        // UNIDAD MAXIMA Y MINIMA
                        if ((int)pProducto.tde_minuni <= pCantidad && (int)pProducto.tde_maxuni >= pCantidad)
                        {
                            isSumarTransfer = true;
                            cantidadCarritoTransfer = pCantidad;
                        }
                        else if ((int)pProducto.tde_maxuni < pCantidad)
                        {
                            isSumarTransfer = true;
                            cantidadCarritoTransfer = (int)pProducto.tde_maxuni;
                            cantidadCarritoComun = pCantidad - (int)pProducto.tde_maxuni;
                        }
                        if (isSumarTransfer)
                        {

                        }
                        else
                        {
                            isPasarDirectamente = true;
                        }
                        // FIN UNIDAD MAXIMA Y MINIMA
                    }
                    else if (pProducto.tde_minuni != null)
                    {
                        // UNIDAD MINIMA
                        if ((int)pProducto.tde_minuni <= pCantidad)
                        {
                            isSumarTransfer = true;
                            cantidadCarritoTransfer = pCantidad;
                        }
                        if (isSumarTransfer)
                        {

                        }
                        else
                        {
                            isPasarDirectamente = true;
                        }
                        // FIN UNIDAD MINIMA
                    }
                } // fin if (listaProductosBuscados[pFila].PrecioConDescuentoOferta > listaProductosBuscados[pFila].PrecioFinalTransfer){
                else
                {
                    isPasarDirectamente = true;
                }
            }
            else
            {
                isPasarDirectamente = true;
            }
            if (isPasarDirectamente)
            {
                cantidadCarritoComun = pCantidad;
            }
            resultado.Add(cantidadCarritoComun);
            resultado.Add(cantidadCarritoTransfer);
            return resultado;
        }
        public static decimal ObtenerPrecioUnitarioConDescuentoOfertaSiLlegaConLaCantidad(cClientes pCliente, cProductosGenerico pProductos, int pCantidad)
        {
            decimal resultado = Convert.ToDecimal(0);
            bool isClienteTomaOferta = false;
            isClienteTomaOferta = pCliente.cli_tomaOfertas;
            if (isClienteTomaOferta)
            {
                if (pProductos.pro_ofeunidades == 0 || pProductos.pro_ofeporcentaje == 0)
                {
                    resultado = pProductos.PrecioFinal;
                }
                else
                {
                    if (pProductos.pro_ofeunidades > pCantidad)
                    {
                        resultado = pProductos.PrecioFinal;
                    }
                    else
                    {
                        resultado = pProductos.PrecioFinal * (Convert.ToDecimal(1) - (pProductos.pro_ofeporcentaje / Convert.ToDecimal(100)));
                    }
                }
            }
            else
            {
                // Cliente si permiso para tomar oferta
                resultado = pProductos.PrecioFinal;
            }
            return resultado;
        }
        public static cDllProductosAndCantidad ProductosEnCarrito_ToConvert_DllProductosAndCantidad(cProductosGenerico pValor)
        {
            cDllProductosAndCantidad resultado = new cDllProductosAndCantidad();
            resultado.cantidad = pValor.cantidad;
            resultado.codProductoNombre = pValor.pro_nombre;
            resultado.isOferta = (pValor.pro_ofeunidades == 0 && pValor.pro_ofeporcentaje == 0) ? false : true;
            return resultado;
        }
        public static string LimpiarStringErrorPedido(string pValor)
        {
            string resultado = pValor;
            if (!string.IsNullOrEmpty(resultado))
            {
                resultado = resultado.Replace("-", string.Empty);
                resultado = resultado.Replace("0", string.Empty);
                resultado = resultado.Replace("1", string.Empty);
                resultado = resultado.Replace("2", string.Empty);
                resultado = resultado.Replace("3", string.Empty);
                resultado = resultado.Replace("4", string.Empty);
                resultado = resultado.Replace("5", string.Empty);
                resultado = resultado.Replace("6", string.Empty);
                resultado = resultado.Replace("7", string.Empty);
                resultado = resultado.Replace("8", string.Empty);
                resultado = resultado.Replace("9", string.Empty);
            }
            return resultado;
        }
    }

}
