using DKbase.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Models
{
    public class AuthenticateResponse
    {
        public int usu_codigo { get; set; }
        public int cli_codigo { get; set; }
        public string ApNombre { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse()
        { }
        public AuthenticateResponse(User user, string token)
        {
            usu_codigo = user.usu_codigo;
            cli_codigo = user.cli_codigo;
            Token = token;
        }
        public AuthenticateResponse(User user, string token, string pApNombre)
        {
            usu_codigo = user.usu_codigo;
            cli_codigo = user.cli_codigo;
            Token = token;
            ApNombre = pApNombre;
        }
    }
}
