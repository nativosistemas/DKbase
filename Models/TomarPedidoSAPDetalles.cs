using System;

namespace DKbase.Models
{
    public class TomarPedidoSAPDetalles
    {
        public int tpd_id { get; set; }
        public int tpd_codTomarPedidoSAP { get; set; }
        public int? tpd_codCarritosDetalles { get; set; }
        public int tpd_codUsuario { get; set; }
        public decimal tpd_codProducto { get; set; }
        public long? tpd_codTransfers { get; set; }
        public int tpd_cantidad { get; set; }
        public string tpd_status { get; set; }
        public DateTime? tpd_statusUpdate { get; set; }
        public DateTime tpd_createDate { get; set; }
        public string tpd_resultResponseContent { get; set; }
        public web.capaDatos.cProductosGenerico oProductosGenerico { get; set; }
    }
}