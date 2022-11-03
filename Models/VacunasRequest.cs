using DKbase.dll;
using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Models
{
    public class VacunasRequest
    {
        public List<cVacuna> pVacunas { get; set; }
        public DateTime pDesde { get; set; }
        public DateTime pHasta { get; set; }
        public String pLoginWEB { get; set; }
    }
}
