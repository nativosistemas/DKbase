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
        private int _pri_ancho_ampliar = 1024;
        private int _pri_alto_ampliar = 768;
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
}
    public class capaProductos
    {
    }
}
