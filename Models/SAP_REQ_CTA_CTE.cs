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
        public string MONTO_GRAVADO { get; set; }
        public string IVA { get; set; }
        public string PERCEPCION_DGR { get; set; }
        public string PERCEPCION_MUNICIPAL { get; set; }
        public string TOTAL { get; set; }
        public string MONTO_EXENTO { get; set; }
        public string PERCEPCION_IVA { get; set; }
    }
    public class SAP_REQ_CTA_CTE_WRAPPER
    {
        public SAP_REQ_CTA_CTE_LIST ET_LISTA { get; set; }
    }

}
