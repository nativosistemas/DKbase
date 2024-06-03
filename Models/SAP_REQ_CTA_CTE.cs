using System.Collections.Generic;

namespace DKbase.Models
{
    public class SAP_REQ_CTA_CTE
    {
        public string CLIENTE { get; set; }
        public string FECHA_DOC { get; set; }
        public string CLASE_DOC { get; set; }
        public string NRO_DOCUMENTO { get; set; }
        public string NRO_COMP { get; set; }
        public decimal MONTO_GRAVADO { get; set; }
        public decimal IVA { get; set; }
        public decimal PERCEPCION_DGR { get; set; }
        public decimal PERCEPCION_MUNICIPAL { get; set; }
        public decimal TOTAL { get; set; }
        public decimal MONTO_EXENTO { get; set; }
        public decimal PERCEPCION_IVA { get; set; }
    }
    public class SAP_REQ_CTA_CTE_WRAPPER
    {
        public SAP_REQ_CTA_CTE_LIST ET_LISTA { get; set; }
    }

}
