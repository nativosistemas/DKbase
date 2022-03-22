using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cProductos
    {
        public cProductos()
        {
        }
        public cProductos(string pPro_codigo, string pPro_nombre)
        {
            pro_codigo = pPro_codigo;
            pro_nombre = pPro_nombre;
        }
        public string pro_codigo { get; set; }
        public string pro_nombre { get; set; }
        //public string pro_codtpopro { get; set; }
        //public decimal pro_descuentoweb { get; set; }
        public decimal pro_precio { get; set; }
        public decimal pro_preciofarmacia { get; set; }
        public int pro_ofeunidades { get; set; }
        public decimal pro_ofeporcentaje { get; set; }
        public bool pro_neto { get; set; }
        public string pro_codtpopro { get; set; }
        public decimal pro_descuentoweb { get; set; }
        public string pro_laboratorio { get; set; }
        public string pro_monodroga { get; set; }
        public string pro_codtpovta { get; set; }
        public string pro_codigobarra { get; set; }
        public string pro_troquel { get; set; }
        public string pro_codigoalfabeta { get; set; }
        public decimal PrecioFinal { get; set; }
        public decimal PrecioConDescuentoOferta { get; set; }
        public bool pro_isTrazable { get; set; }
        public bool pro_isCadenaFrio { get; set; }
        public int? pro_canmaxima { get; set; }
        public bool pro_entransfer { get; set; }
        public bool pro_vtasolotransfer { get; set; }
        public int pro_acuerdo { get; set; }
        public string pri_nombreArchivo { get; set; }
        private int _pri_ancho_ampliar = DKbase.generales.Constantes.cImg_ancho_ampliar_dafault;
        private int _pri_alto_ampliar = DKbase.generales.Constantes.cImg_alto_ampliar_dafault;
        public int? pri_ancho_ampliar_original { get; set; }
        public int? pri_alto_ampliar_original { get; set; }
        public int pri_ancho_ampliar { get { return _pri_ancho_ampliar; } set { _pri_ancho_ampliar = value; } }
        public int pri_alto_ampliar { get { return _pri_alto_ampliar; } set { _pri_alto_ampliar = value; } }
        public bool pro_NoTransfersEnClientesPerf { get; set; }
        public bool pro_AceptaVencidos { get; set; }
        private bool _isMostrarTransfersEnClientesPerf = true;
        public bool isMostrarTransfersEnClientesPerf
        {
            get { return _isMostrarTransfersEnClientesPerf; }
            set { _isMostrarTransfersEnClientesPerf = value; }
        }
        private bool _isPermitirPedirProducto = true;
        public bool isPermitirPedirProducto
        {
            get { return _isPermitirPedirProducto; }
            set { _isPermitirPedirProducto = value; }
        }
        public string pro_Familia { get; set; }
        public int? pro_PackDeVenta { get; set; }
        public decimal pro_PorcARestarDelDtoDeCliente { get; set; }
        public decimal pro_PrecioBase { get; set; }
        public bool pro_ProductoRequiereLote { get; set; }
        public bool pro_AltoCosto { get; set; }
        public string pro_UbicacionPrincipal { get; set; }
    }
    public class cSucursalStocks
    {
        public string stk_codpro { get; set; }
        public string stk_codsuc { get; set; }
        public string stk_stock { get; set; }
        public int cantidadSucursal { get; set; }
    }
    public class cProductosGenerico : cTransferDetalle
    {
        public cProductosGenerico()
        {
            listaSucursalStocks = new List<DKbase.web.capaDatos.cSucursalStocks>();
            isProductoNoEncontrado = false;
        }
        public cProductosGenerico(cProductos pProducto)
        {
            base.pro_codigo = pProducto.pro_codigo;
            base.pro_nombre = pProducto.pro_nombre;
            base.PrecioFinal = pProducto.PrecioFinal;
            base.pro_codigoalfabeta = pProducto.pro_codigoalfabeta;
            base.pro_codigobarra = pProducto.pro_codigobarra;
            base.pro_codtpopro = pProducto.pro_codtpopro;
            base.pro_descuentoweb = pProducto.pro_descuentoweb;
            base.pro_laboratorio = pProducto.pro_laboratorio;
            base.pro_monodroga = pProducto.pro_monodroga;
            base.pro_codtpovta = pProducto.pro_codtpovta;
            base.pro_neto = pProducto.pro_neto;
            base.pro_ofeporcentaje = pProducto.pro_ofeporcentaje;
            base.pro_ofeunidades = pProducto.pro_ofeunidades;
            base.pro_precio = pProducto.pro_precio;
            base.pro_preciofarmacia = pProducto.pro_preciofarmacia;
            base.pro_isTrazable = pProducto.pro_isTrazable;
            base.pro_NoTransfersEnClientesPerf = pProducto.pro_NoTransfersEnClientesPerf;
            base.pro_Familia = pProducto.pro_Familia;
            base.pro_AceptaVencidos = pProducto.pro_AceptaVencidos;
            base.pro_PackDeVenta = pProducto.pro_PackDeVenta;
            base.pro_PrecioBase = pProducto.pro_PrecioBase;
            base.pro_PorcARestarDelDtoDeCliente = pProducto.pro_PorcARestarDelDtoDeCliente;
            base.pro_ProductoRequiereLote = pProducto.pro_ProductoRequiereLote;
            listaSucursalStocks = new List<DKbase.web.capaDatos.cSucursalStocks>();
            isProductoNoEncontrado = false;
        }
        public int pro_Ranking { get; set; }
        public bool isTieneTransfer { get; set; }
        public bool isValePsicotropicos { get; set; }
        public int cantidad { get; set; }
        public int nroordenamiento { get; set; }
        public bool isProductoNoEncontrado { get; set; }
        public bool isProductoFacturacionDirecta { get; set; }
        public string codProducto { get; set; }
        public int idUsuario { get; set; }
        public string stk_stock { get; set; }
        //
        public string fpc_nombreProducto { get; set; }
        public int fpc_cantidad { get; set; }
        public decimal PrecioFinalRecuperador { get; set; }
        //
        public void CargarTransferYTransferDetalle(cTransferDetalle pValor)
        {
            base.tde_codpro = pValor.tde_codpro;
            base.tde_codtfr = pValor.tde_codtfr;
            base.tde_descripcion = pValor.tde_descripcion;
            base.tde_fijuni = pValor.tde_fijuni;
            base.tde_maxuni = pValor.tde_maxuni;
            base.tde_minuni = pValor.tde_minuni;
            base.tde_muluni = pValor.tde_muluni;
            base.tde_predescuento = pValor.tde_predescuento;
            base.tde_prepublico = pValor.tde_prepublico;
            base.tde_proobligatorio = pValor.tde_proobligatorio;
            base.tde_unidadesbonificadas = pValor.tde_unidadesbonificadas;
            base.tde_unidadesbonificadasdescripcion = pValor.tde_unidadesbonificadasdescripcion;
            base.tde_PrecioConDescuentoDirecto = pValor.tde_PrecioConDescuentoDirecto;
            base.tde_PorcARestarDelDtoDeCliente = pValor.tde_PorcARestarDelDtoDeCliente;
            base.tde_PorcDtoSobrePVP = pValor.tde_PorcDtoSobrePVP;
            base.tfr_codigo = pValor.tfr_codigo;
            base.tfr_accion = pValor.tfr_accion;
            base.tfr_nombre = pValor.tfr_nombre;
            base.tfr_deshab = pValor.tfr_deshab;
            base.tfr_pordesadi = pValor.tfr_pordesadi;
            base.tfr_tipo = pValor.tfr_tipo;
            base.tfr_mospap = pValor.tfr_mospap;
            base.tfr_minrenglones = pValor.tfr_minrenglones;
            base.tfr_minunidades = pValor.tfr_minunidades;
            base.tfr_maxunidades = pValor.tfr_maxunidades;
            base.tfr_mulunidades = pValor.tfr_mulunidades;
            base.tfr_fijunidades = pValor.tfr_fijunidades;
            base.tfr_facturaciondirecta = pValor.tfr_facturaciondirecta;
            base.tfr_descripcion = pValor.tfr_descripcion;
            base.isTablaTransfersClientes = pValor.isTablaTransfersClientes;
        }

    }
    public class capaProductos_base
    {
    }
}
