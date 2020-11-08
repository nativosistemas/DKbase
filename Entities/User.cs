using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DKbase.Entities
{
    public class User
    {
        public int usu_codigo { get; set; }
        public int cli_codigo { get; set; }

        public int idRol { get; set; }
        public string login { get; set; }

    }
}
