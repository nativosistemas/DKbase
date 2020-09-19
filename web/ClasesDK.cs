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
    }
}
