using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Entities
{
    public class Farmacia
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
    }
    public class Modulo
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public List<ModuloDetalle> moduloDetalle { get; set; }
    }
    public class ModuloDetalle
    {
        public string id { get; set; }
        public int idModulo { get; set; }
        public string producto { get; set; }
        public string descripcion { get; set; }
        public double precio { get; set; }
        public double precioDescuento { get; set; }
    }
}
