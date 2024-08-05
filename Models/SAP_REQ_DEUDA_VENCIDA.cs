using System.Collections.Generic;

namespace DKbase.Models
{
    public class SAP_REQ_DEUDA_VENCIDA
    {
        public string FECHA { get; set; }
        public string VENCIMIENTO { get; set; }
        public string COMPROBANTE { get; set; }
        public string NUMERO_COMPROBANTE { get; set; }
        public string SEMANA { get; set; }
        public string IMPORTE { get; set; }
    }
    public class SAP_REQ_DEUDA_VENCIDA_WRAPPER
    {
        public SAP_REQ_DEUDA_VENCIDA_LIST ET_DEUDA_VENCIDA { get; set; }
    }

}