using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Entities
{

    public class Farmacia
    {
        public web.capaDatos.cClientes objCliente { get; set; }
        public int id { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
    }
    public class Laboratorio
    {
        public int idParaArchivo { get; set; }
        public long id { get; set; }
        public string nombre { get; set; }
        public string imagen { get; set; }
        public string imagenBase64 { get; set; }
    }
    public class Modulo
    {
        public int id { get; set; }
        public string nombre_laboratorio { get; set; }
        public string descripcion { get; set; }
        public int cantidadMinimos { get; set; }
        public long idLaboratorio { get; set; }
        public Laboratorio laboratorio { get; set; }
        public List<ModuloDetalle> moduloDetalle { get; set; }
    }
    public class ModuloDetalle
    {
        public web.capaDatos.cProductos objProducto { get; set; }
        //public int id { get; set; }
        public int idModulo { get; set; }
        public int orden { get; set; }
        public string producto { get; set; }
        public string descripcion { get; set; }
        public double precio { get; set; }
        public double precioDescuento { get; set; }
        public int cantidadUnidades { get; set; }
        private bool _dmo_TieneEnCuentaDescuentoCliente = false;
        public bool isTieneEnCuentaDescuentoCliente { get { return _dmo_TieneEnCuentaDescuentoCliente; } set { _dmo_TieneEnCuentaDescuentoCliente = value; } }
    }
    public class AppPedido
    {
        public int id { get; set; }
        public string promotor { get; set; }
        public List<AppPedidoModulo> pedidoModulos { get; set; }
        public DateTime? fechaApp { get; set; }
    }
    public class AppPedidoModulo
    {
        public int id { get; set; }
        public int idModulo { get; set; }
        public int idFarmacia { get; set; }
        public int cantidad { get; set; }
        public DateTime? fechaApp { get; set; }
    }
    public class AppInfoPedido
    {
        public int pea_id { get; set; }
        public Guid pea_guid { get; set; }
        public string pea_promotor { get; set; }
        public int pea_numeroModulo { get; set; }
        public int pea_codCliente { get; set; }
        public int pea_cantidad { get; set; }
        public DateTime? pea_fecha { get; set; }
        public bool pea_procesado { get; set; }
        public DateTime? pea_procesado_fecha { get; set; }
        public int? pea_procesado_cantidad { get; set; }
        public string pea_procesado_descripcion { get; set; }
    }
    public class cSincronizadorApp
    {
        public List<AppInfoPedido> listaAppInfoPedido { get; set; }
        public List<Laboratorio> listaLaboratorio { get; set; }
        public List<Modulo> listaModulo { get; set; }
        public List<Farmacia> listaFarmacia { get; set; }
    }
}
