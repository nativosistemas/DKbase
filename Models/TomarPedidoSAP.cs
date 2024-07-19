using System;
using System.Collections.Generic;

namespace DKbase.Models
{
    public class TomarPedidoSAP
    {
        public int tpc_id { get; set; }
        public int? tpc_codCarrito { get; set; }
        public string tpc_CarritoTipo { get; set; }
        public string tpc_codSucursal { get; set; }
        public decimal tpc_codCliente { get; set; }
        public int tpc_codUsuario { get; set; }
        public string tpc_status { get; set; }
        public DateTime? tpc_statusUpdate { get; set; }
        public DateTime tpc_createDate { get; set; }
        public string tpc_resultResponseContent { get; set; }
        public List<TomarPedidoSAPDetalles> l_detalle { get; set; }
        public DKbase.web.capaDatos.cCarrito oCarrito { get; set; }
    }
}