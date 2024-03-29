﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cCarrito
    {
        public cCarrito() { listaProductos = new List<cProductosGenerico>(); }
        public int car_id { get; set; }
        public int lrc_id { get; set; }
        public string codSucursal { get; set; }
        public string proximoHorarioEntrega { get; set; }
        public List<cProductosGenerico> listaProductos { get; set; }
    }
    public class cProductosAndCantidad
    {
        public string codSucursal { get; set; }
        public string codProducto { get; set; }
        public string codProductoNombre { get; set; }
        public int cantidad { get; set; }
        public bool isTransferFacturacionDirecta { get; set; }
        public int tde_codtfr { get; set; }
    }
    public class cSucursalCarritoTransfer
    {
        public int car_id { get; set; }
        public string Sucursal { get; set; }
        public string proximoHorarioEntrega { get; set; }
        public List<cCarritoTransfer> listaTransfer { get; set; }
    }
    public class cCarritoTransfer
    {
        public cCarritoTransfer() { listaProductos = new List<cProductosGenerico>(); }
        public int ctr_id { get; set; }
        public int car_id_aux { get; set; }
        public string ctr_codSucursal { get; set; }
        public int tfr_codigo { get; set; }
        public string tfr_nombre { get; set; }
        public bool tfr_deshab { get; set; }
        public decimal? tfr_pordesadi { get; set; }
        public string tfr_tipo { get; set; }
        public List<cProductosGenerico> listaProductos { get; set; }
    }
    public class capaCarrito_base
    {
    }
}
