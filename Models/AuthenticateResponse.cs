using DKbase.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.Models
{
    public class AuthenticateResponse
    {
        public int id { get; set; }
        public int cli_codigo { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse()
        { }
        public AuthenticateResponse(User user, string token)
        {
            id = user.id;
            cli_codigo = user.cli_codigo;
            Token = token;
        }
    }
}
