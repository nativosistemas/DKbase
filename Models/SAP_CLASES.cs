
using System.Collections.Generic;
using DKbase.generales;
using System.Text.Json.Serialization;

namespace DKbase.Models
{


    public class S_LEGADOS_WEB_OUT_class
    {
        public S_LEGADOS_WEB_OUT S_LEGADOS_WEB_OUT { get; set; }
    }
    public class S_LEGADOS_WEB_OUT
    {
        public POSICION POSICION { get; set; }
    }

    public class POSICION
    {
        [JsonConverter(typeof(SingleOrArrayConverter<Item>))]
        public List<Item> item { get; set; }
    }

    public class Item
    {
        public string PEDIDO_SAP { get; set; }
        public string ID_CARRITO { get; set; }
        public string ID_POSICION { get; set; }
        public string MATERIAL { get; set; }
        public string ACUERDO { get; set; }
        public string CANTIDAD_PEDIDA { get; set; }
        public string CANTIDAD_ACEPTADA { get; set; }
        public string CANTIDAD_R_CUPO { get; set; }
        public string CANTIDAD_R_FSTOCK { get; set; }
        public string CANTIDAD_R_PROMO { get; set; }
        public string CHECK_REGALO { get; set; }
        public string MOTIVO_RECHAZO { get; set; }
        public string IMPORTE_NETO_LINEA { get; set; }
        public string IMPORTE_IMPUESTO { get; set; }
        public string MONTO_R_CREDITO { get; set; }
    }

}