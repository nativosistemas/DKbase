
using System.Collections.Generic;
namespace DKbase.Models
{
    public class TomarPedidoResponse
    {
        public string tipo { get; set; }
        public string msg { get; set; }
        public dll.cDllPedido result_dll { get; set; }
        public List<Item> result_sap { get; set; }
    }
}
