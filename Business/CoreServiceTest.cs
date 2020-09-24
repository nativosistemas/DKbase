using DKbase.dll;
using DKbase.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Business
{
    public class CoreServiceTest : ICoreService
    {
        public cDllPedido TomarPedidoConIdCarrito(TomarPedidoConIdCarritoRequest pValue)
        {
            DKbase.dll.cDllPedido o = new DKbase.dll.cDllPedido();
            o.Error = "Hola MundoAAAA";
            o.MensajeEnFactura = "Hola MundoAAAAJJJ";
            return o;
        }
    }
}
