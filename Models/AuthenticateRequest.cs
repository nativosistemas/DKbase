using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Models
{
    public class AuthenticateRequest
    {
        public string login { get; set; }

        public string pass { get; set; }
        public string token { get; set; }
    }
}
