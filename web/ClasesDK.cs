using DKbase.web.capaDatos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.web
{
    public class cSucursal
    {
        public cSucursal()
        {
        }
        public cSucursal(int pSde_codigo, string pSde_sucursal, string pSde_sucursalDependiente)
        {
            sde_codigo = pSde_codigo;
            sde_sucursal = pSde_sucursal;
            sde_sucursalDependiente = pSde_sucursalDependiente;
        }
        public int sde_codigo { get; set; }
        public string sde_sucursal { get; set; }
        public decimal suc_montoMinimo { get; set; }
        public string suc_codigo { get; set; }
        public string suc_nombre { get; set; }
        public string suc_provincia { get; set; }
        public bool suc_facturaTrazables { get; set; }
        public bool suc_facturaTrazablesEnOtrasProvincias { get; set; }
        public string sde_sucursalDependiente { get; set; }
        public string sucursal_sucursalDependiente { get; set; }
        public bool suc_pedirCC_ok { get; set; }
        public string suc_pedirCC_sucursalReferencia { get; set; }
        public bool suc_pedirCC_tomaSoloPerfumeria { get; set; }
        public bool suc_trabajaPerfumeria { get; set; }
    }
    public class ResultTransfer
    {
        public bool isNotError { get; set; }
        public string codSucursal { get; set; }
        public cSucursalCarritoTransfer oSucursalCarritoTransfer { get; set; }
        public List<cProductosAndCantidad> listProductosAndCantidadError { get; set; }
    }
    public class ResultCargaProducto
    {
        public bool isOk { get; set; }
        public cCarrito oCarrito { get; set; }
    }
    public class ResultCreditoDisponible
    {
        public decimal? CreditoDisponibleSemanal { get; set; }
        public decimal? CreditoDisponibleTotal { get; set; }
    }
    public class cjSonBuscadorProductos //: ICloneable
    {
        public cjSonBuscadorProductos() { }
        public cjSonBuscadorProductos(cjSonBuscadorProductos pValue)
        {
            listaSucursal = pValue.listaSucursal;
            listaProductos = pValue.listaProductos;
            CantidadRegistroTotal = pValue.CantidadRegistroTotal;
        }
        public List<string> listaSucursal { get; set; }
        public List<cProductosGenerico> listaProductos { get; set; }
        public int CantidadRegistroTotal { get; set; }
    }
    public class cTiposEnvios
    {
        public cTiposEnvios()
        {
        }
        public cTiposEnvios(int id, string nombreSucursal)
        {
            env_id = id;
            env_codigo = string.Empty;
            env_nombre = nombreSucursal;
        }
        public int env_id { get; set; }
        public string env_codigo { get; set; }
        public string env_nombre { get; set; }
    }
    public class cTipoEnvioClienteFront
    {
        public string sucursal { get; set; }
        public string tipoEnvio { get; set; }
        public List<cTiposEnvios> lista { get; set; }
    }
    public class cSucursalDependienteTipoEnviosCliente
    {
        public int tsd_id { get; set; }
        public int tsd_idSucursalDependiente { get; set; }
        public int? tsd_idTipoEnvioCliente { get; set; }
        public int sde_codigo { get; set; }
        public string sde_sucursal { get; set; }
        public string sde_sucursalDependiente { get; set; }
        public int env_id { get; set; }
        public string env_codigo { get; set; }
        public string env_nombre { get; set; }
        List<cSucursalDependienteTipoEnviosCliente_TiposEnvios> listaTiposEnvios { get; set; }
    }
    public class cSucursalDependienteTipoEnviosCliente_TiposEnvios
    {
        public int tdt_id { get; set; }
        public int tdt_idSucursalDependienteTipoEnvioCliente { get; set; }
        public int tdt_idTipoEnvio { get; set; }
        public int env_id { get; set; }
        public string env_codigo { get; set; }
        public string env_nombre { get; set; }
        public string tdr_codReparto { get; set; }
    }
    public class cCadeteriaRestricciones
    {
        public int tcr_id { get; set; }
        public string tcr_codigoSucursal { get; set; }
        public int tcr_UnidadesMinimas { get; set; }
        public int tcr_UnidadesMaximas { get; set; }
        public double tcr_MontoMinimo { get; set; }
        public double tcr_MontoIgnorar { get; set; }
        public string suc_nombre { get; set; }
    }
    public class cUsuarioSinPermisosIntranet
    {
        public int usp_id { get; set; }
        public int usp_codUsuario { get; set; }
        public string usp_nombreSeccion { get; set; }
    }
    public class cSubirPedido_return
    {
        public string SucursalEleginda { get; set; }
        public List<cProductosGenerico> ListaProductos { get; set; }
        public string nombreArchivoCompleto { get; set; }
        public string nombreArchivoCompletoOriginal{ get; set; }
        public bool isCorrect { get; set; }
    }
    public class cRangoFecha_Pedidos
    {
        public List<string> lista { get; set; }
        public List<DKbase.dll.cDllPedido> resultadoObj  { get; set; }
    }
}
