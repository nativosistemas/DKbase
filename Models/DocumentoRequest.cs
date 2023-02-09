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
    }
}
