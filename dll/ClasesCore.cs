using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.dll
{
   public class post_TomarPedidoConIdCarrito
    {
        public  int pIdCarrito { get; set; }
        public string pLoginCliente { get; set; }
        public string pIdSucursal { get; set; }
        public string pMensajeEnFactura { get; set; }
        public string pMensajeEnRemito { get; set; }
        public string pTipoEnvio { get; set; }
        public List<cDllProductosAndCantidad> pListaProducto { get; set; }
        public bool pIsUrgente { get; set; }
    }
}
