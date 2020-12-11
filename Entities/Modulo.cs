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
    public class Laboratorio
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string imagen { get; set; }
    }
    public class Modulo
    {
        public int id { get; set; }
        public string nombre_laboratorio { get; set; }
        public string descripcion { get; set; }
        public int cantidadMinimos { get; set; }
        public int idLaboratorio { get; set; }
        public Laboratorio laboratorio { get; set; }
        public List<ModuloDetalle> moduloDetalle { get; set; }
    }
    public class ModuloDetalle
    {
        public int id { get; set; }
        public int idModulo { get; set; }
        public int orden { get; set; }
        public string producto { get; set; }
        public string descripcion { get; set; }
        public double precio { get; set; }
        public double precioDescuento { get; set; }
        public int cantidadUnidades { get; set; }
    }
}
