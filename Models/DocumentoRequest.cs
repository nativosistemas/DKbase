using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Models
{
    public class DocumentoRequest
    {
        public string documentoID { get; set; }
        public string documentoTipo { get; set; }
        public string loginWeb { get; set; }
        public DateTime fecha { get; set; }
        public bool isIncluyeCancelado { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public string nombreProducto { get; set; }
        public List<dll.cDevolucionItemPrecarga_java> itemDevolucionPrecarga { get; set; }
    }
}
