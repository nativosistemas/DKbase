using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Models
{
    public class ObtenerPedidosEntreFechasRequest
    {
        public string pLoginWeb { get; set; }
        public DateTime pDesde { get; set; }
        public DateTime pHasta { get; set; }
    }
}
