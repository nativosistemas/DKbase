﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.web.capaDatos
{

    public class cTransfer
    {
        public int tfr_codigo { get; set; }
        public string tfr_accion { get; set; }
        public string tfr_nombre { get; set; }
        public bool tfr_deshab { get; set; }
        public decimal? tfr_pordesadi { get; set; }
        public string tfr_tipo { get; set; }
        public bool tfr_mospap { get; set; }
        public int? tfr_minrenglones { get; set; }
        public int? tfr_minunidades { get; set; }
        public int? tfr_maxunidades { get; set; }
        public int? tfr_mulunidades { get; set; }
        public int? tfr_fijunidades { get; set; }
        public bool? tfr_facturaciondirecta { get; set; }
        public string tfr_descripcion { get; set; }
        public string tfr_provincia { get; set; }
        public List<cTransferDetalle> listaDetalle { get; set; }

    }

    public class cTransferDetalle : cProductos
    {
        public cTransferDetalle()
        {
            listaSucursalStocks = new List<cSucursalStocks>();
        }
        public int tde_codtfr { get; set; }
        public string tde_codpro { get; set; }
        public string tde_descripcion { get; set; }
        public decimal? tde_prepublico { get; set; }
        public decimal? tde_predescuento { get; set; }
        public int? tde_minuni { get; set; }
        public int? tde_maxuni { get; set; }
        public int? tde_muluni { get; set; }
        public int? tde_fijuni { get; set; }
        public bool tde_proobligatorio { get; set; }
        public decimal PrecioFinalTransfer { get; set; }
        public int? tde_unidadesbonificadas { get; set; }
        public string tde_unidadesbonificadasdescripcion { get; set; }
        public string tde_DescripcionDeProducto { get; set; }
        public decimal tde_PrecioConDescuentoDirecto { get; set; }
        public decimal tde_PorcARestarDelDtoDeCliente { get; set; }
        public decimal? tde_PorcDtoSobrePVP { get; set; }
        private bool _isTablaTransfersClientes = false;
        public bool isTablaTransfersClientes { get { return _isTablaTransfersClientes; } set { _isTablaTransfersClientes = value; } }
        public List<cSucursalStocks> listaSucursalStocks { get; set; }

        public int tfr_codigo { get; set; }
        public string tfr_accion { get; set; }
        public string tfr_nombre { get; set; }
        public bool tfr_deshab { get; set; }
        public decimal? tfr_pordesadi { get; set; }
        public string tfr_tipo { get; set; }
        public bool tfr_mospap { get; set; }
        public int? tfr_minrenglones { get; set; }
        public int? tfr_minunidades { get; set; }
        public int? tfr_maxunidades { get; set; }
        public int? tfr_mulunidades { get; set; }
        public int? tfr_fijunidades { get; set; }
        public bool? tfr_facturaciondirecta { get; set; }
        public string tfr_descripcion { get; set; }
        public string tfr_provincia { get; set; }
        public void CargarTransfer(cTransfer pValor)
        {
            tfr_codigo = pValor.tfr_codigo;
            tfr_accion = pValor.tfr_accion;
            tfr_nombre = pValor.tfr_nombre;
            tfr_deshab = pValor.tfr_deshab;
            tfr_pordesadi = pValor.tfr_pordesadi;
            tfr_tipo = pValor.tfr_tipo;
            tfr_mospap = pValor.tfr_mospap;
            tfr_minrenglones = pValor.tfr_minrenglones;
            tfr_minunidades = pValor.tfr_minunidades;
            tfr_maxunidades = pValor.tfr_maxunidades;
            tfr_mulunidades = pValor.tfr_mulunidades;
            tfr_fijunidades = pValor.tfr_fijunidades;
            tfr_facturaciondirecta = pValor.tfr_facturaciondirecta;
            tfr_descripcion = pValor.tfr_descripcion;
            tfr_provincia = pValor.tfr_provincia;
        }
    }
    class capaTransfer_base
    {
    }
}
