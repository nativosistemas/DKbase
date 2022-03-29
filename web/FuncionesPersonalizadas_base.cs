using DKbase.generales;
using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DKbase.web
{
  public  class FuncionesPersonalizadas_base
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
                            if (pClientes.cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto == null)
                            {
                                resultado = resultado * (Convert.ToDecimal(1) - (pClientes.cli_pordesbetmed / Convert.ToDecimal(100)));
                            }
                            else
                            {
                                resultado = getPrecioConDescuentoTransfer(pTde_PrecioConDescuentoDirecto, pTde_PorcARestarDelDtoDeCliente, pClientes.cli_pordesbetmed);
                            }
                            break;
                        case "P": // Perfumeria
                        case "A": // Accesorio
                        case "V": // Accesorio
                            if (pClientes.cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto == null)
                            {
                                resultado = resultado * (Convert.ToDecimal(1) - (pClientes.cli_pordesnetos / Convert.ToDecimal(100)));
                            }
                            else
                            {
                                resultado = getPrecioConDescuentoTransfer(pTde_PrecioConDescuentoDirecto, pTde_PorcARestarDelDtoDeCliente, pClientes.cli_pordesnetos);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {  // No neto   
                    if (pClientes.cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto == null)
                    {
                        resultado = resultado * (Convert.ToDecimal(1) - (pClientes.cli_pordesespmed / Convert.ToDecimal(100)));
                    }
                    else
                    {
                        resultado = getPrecioConDescuentoTransfer(pTde_PrecioConDescuentoDirecto, pTde_PorcARestarDelDtoDeCliente, pClientes.cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto.Value);
                    }
                }
            }
            if (pPro_neto)
            {   // Neto        
                switch (pPro_codtpopro)
                {
                    case "M": // medicamento
                        if (pClientes.cli_deswebnetmed)
                        {
                            resultado = resultado * (Convert.ToDecimal(1) - (pPro_descuentoweb / Convert.ToDecimal(100)));
                        }
                        break;
                    case "P": // Perfumeria
                        if (pClientes.cli_deswebnetperpropio)
                        {
                            resultado = resultado * (Convert.ToDecimal(1) - (pPro_descuentoweb / Convert.ToDecimal(100)));
                        }
                        break;
                    case "A": // Accesorio
                        if (pClientes.cli_deswebnetacc)
                        {
                        }
                        break;
                    case "V": // Accesorio
                        if (pClientes.cli_deswebnetacc)
                        {
                            resultado = resultado * (Convert.ToDecimal(1) - (pPro_descuentoweb / Convert.ToDecimal(100)));
                        }
                        break;
                    case "F": // Perfumería Cuenta y Orden
                        if (pClientes.cli_deswebnetpercyo)
                        {
                            resultado = resultado * (Convert.ToDecimal(1) - (pPro_descuentoweb / Convert.ToDecimal(100)));
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {  // No neto      
                if (pClientes.cli_deswebespmed)
                {
                    resultado = resultado * (Convert.ToDecimal(1) - (pPro_descuentoweb / Convert.ToDecimal(100)));
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
        private static decimal getPrecioBaseConDescuento(cProductos pProductos, decimal? pDescuentoRestar)
        {
            decimal descuentoRestarTemp = pDescuentoRestar == null ? 0 : pDescuentoRestar.Value;
            decimal descuento = descuentoRestarTemp - pProductos.pro_PorcARestarDelDtoDeCliente;
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
        public static string ObtenerHorarioCierre_base(cClientes pClientes, string pSucursal, string pSucursalDependiente, string pCodigoReparto, DateTime fechaActual)
        {
            string resultado = null;
            List<cHorariosSucursal> listaHorariosSucursal = acceso.RecuperarTodosHorariosSucursalDependiente().Where(x => x.sdh_sucursal == pSucursal && x.sdh_sucursalDependiente == pSucursalDependiente && x.sdh_codReparto == pCodigoReparto).ToList();
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
            if (pClientes.cli_codrep == "S7" && pSucursalDependiente == "SF")
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
        public static string ObtenerHorarioCierre(cClientes pClientes, string pSucursal, string pSucursalDependiente, string pCodigoReparto)
        {
            return ObtenerHorarioCierre_base(pClientes, pSucursal, pSucursalDependiente, pCodigoReparto, DateTime.Now);
        }
        public static string ObtenerHorarioCierreAnterior(cClientes pClientes, string pSucursal, string pSucursalDependiente, string pCodigoReparto, string pHorarioCierre)
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
                result = ObtenerHorarioCierre_base(pClientes,pSucursal, pSucursalDependiente, pCodigoReparto, fechaCuentaRegresiva);
            }
            catch (Exception ex)
            {

                var oo = 1;
            }
            return result;
        }
    }

}
