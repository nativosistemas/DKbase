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
    public class AppInfoPedido : Modulo
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
        public List<AppInfoPedido> listaAppPedidoModuloHistorial { get; set; }
        public List<Laboratorio> listaLaboratorio { get; set; }
        public List<Modulo> listaModulo { get; set; }
        public List<Farmacia> listaFarmacia { get; set; }
        public Guid pedidoGuid { get; set; }
    }
    public class AppCargaDatosClientes
    {
        public string cdc_promotor { get; set; }
        public int cdc_id { get; set; }
        public Guid cdc_guid { get; set; }
        public string cdc_NombreFantasia { get; set; }
        public string cdc_NombreFarmaceutico { get; set; }
        public string cdc_NumeroMatricula { get; set; }
        public string cdc_Direccion { get; set; }
        public string cdc_Localidad { get; set; }
        public string cdc_Provincia { get; set; }
        public string cdc_CPA { get; set; }
        public string cdc_Telefono { get; set; }
        public string cdc_Email { get; set; }
        public string cdc_CUIT { get; set; }
        public string cdc_IVA { get; set; }
        public string cdc_NroInscripcionDGR { get; set; }
        public string cdc_CodigoPAMI { get; set; }
        public string cdc_GLN { get; set; }
        public string cdc_DescuentoPlazoPago { get; set; }
        public string cdc_MontoDeCreditoAcordado { get; set; }
        public string cdc_MontoDeCreditoAcordado_Periodo { get; set; }
        public string cdc_Reparto { get; set; }
        public DateTime? cdc_fecha { get; set; }
        public List<AppCargaDatosClientes_Responsable> listaResponsable { get; set; }
        public List<AppCargaDatosClientes_Proveedor> listaProveedor { get; set; }
    }
    public class AppCargaDatosClientes_Responsable
    {
        public int cdr_id { get; set; }
        public string cdr_idCargarCliente { get; set; }
        public string cdr_NombreApellido { get; set; }
        public string cdr_Direccion { get; set; }
        public string cdr_Localidad { get; set; }
        public string cdr_Provincia { get; set; }
        public string cdr_CPA { get; set; }
        public string cdr_Telefono { get; set; }
        public string cdr_Email { get; set; }
        public string cdr_CUIT { get; set; }
        public string cdr_DNI { get; set; }
        public string cdr_FechaNacimiento { get; set; }
        public string cdr_EstadoCivil { get; set; }
        public string cdr_NombreConyuge { get; set; }
        public string cdr_Nacionalidad { get; set; }
        public string cdr_CargoOcupa { get; set; }
        public DateTime? cdr_fecha { get; set; }
    }
    public class AppCargaDatosClientes_Proveedor
    {
        public string cdc_Proveedor_Nombre { get; set; }
        public string cdc_Proveedor_Direccion { get; set; }
        public string cdc_Proveedor_Localidad { get; set; }
        public string cdc_Proveedor_Provincia { get; set; }
        public string cdc_Proveedor_CPA { get; set; }
        public string cdc_Proveedor_Telefono { get; set; }
    }
}
