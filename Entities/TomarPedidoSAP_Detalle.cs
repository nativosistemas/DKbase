using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase
{

    public class TomarPedidoSAP_Detalle
    {
        public int tpd_codCarritosDetalles { get; set; }
      //  public double tpd_codCliente { get; set; }
        public int tpd_codUsuario { get; set; }
        public double tpd_codProducto { get; set; }
        public double tpd_codTransfers { get; set; }
        public int tpd_cantidad { get; set; }
        public string tpd_status { get; set; }
        public string tpd_resultResponseContent { get; set; }

    }
}