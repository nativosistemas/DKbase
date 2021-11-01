using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cMensaje: cMensajeNew
    {
        public int tme_codigo { get; set; }
        public string tme_asunto { get; set; }
        public string tme_mensaje { get; set; }
        public int tme_codClienteDestinatario { get; set; }
        public string cli_nombre { get; set; }
        public int tme_codUsuario { get; set; }
        public DateTime tme_fecha { get; set; }
        public string tme_fechaToString { get; set; }
        public int tme_estado { get; set; }
        public int? tme_todos { get; set; }
        public string est_nombre { get; set; }
        public DateTime? tme_fechaDesde { get; set; }
        public string tme_fechaDesdeToString { get; set; }
        public DateTime? tme_fechaHasta { get; set; }
        public string tme_fechaHastaToString { get; set; }
        public bool tme_importante { get; set; }
        public string tme_importanteToString { get; set; }
        public int? tme_todosSucursales { get; set; }

    }
    public class cMensajeNew
    {
        public int tmn_codigo { get; set; }
        public string tmn_asunto { get; set; }
        public string tmn_mensaje { get; set; }
        public DateTime tmn_fecha { get; set; }
        public string tmn_fechaToString { get; set; }
        public DateTime? tmn_fechaDesde { get; set; }
        public string tmn_fechaDesdeToString { get; set; }
        public DateTime? tmn_fechaHasta { get; set; }
        public string tmn_fechaHastaToString { get; set; }
        public bool tmn_importante { get; set; }
        public string tmn_importanteToString { get; set; }
        public string tmn_todosSucursales { get; set; }
        public string tmn_todosRepartos { get; set; }
        public string tmn_tipo { get; set; }
    }
}
