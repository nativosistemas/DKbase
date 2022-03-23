using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
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
    }

}
