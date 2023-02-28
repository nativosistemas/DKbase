using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Models
{
    public class ObraSocialRequest
    {
        //string pNombrePlan, string pLoginWeb, int pAnio, int pMes
        //string pNombrePlan, string pLoginWeb, int pAnio, int pMes, int pQuincena
        //(string pNombrePlan, string pLoginWeb, int pAnio, int pSemana)
        public string nombrePlan { get; set; }
        public string loginWeb { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public int quincena { get; set; }
        public int semana { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }

    }
}
