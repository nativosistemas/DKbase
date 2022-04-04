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
}
