using DKbase.dll;
using DKbase.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Business
{
    public interface ICoreService
    {
        cDllPedido TomarPedidoConIdCarrito(TomarPedidoConIdCarritoRequest pValue);
    }
}
