using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.web
{
    public class Usuario
    {
        public Usuario()
        {
            id = -1;
            //User = string.Empty;
            NombreYApellido = string.Empty;
            idUsuarioLog = -1;
        }
        public int id
        {
            get;
            set;
        }
        public int idRol
        {
            get;
            set;
        }
        public string NombreYApellido
        {
            get;
            set;
        }
        public string ApNombre
        {
            get;
            set;
        }
        public string usu_pswDesencriptado
        {
            get;
            set;
        }
        public int idUsuarioLog
        {
            get;
            set;
        }
        public int usu_estado
        {
            get;
            set;
        }
        public int? usu_codCliente
        {
            get;
            set;
        }
        public string usu_login
        {
            get;
            set;
        }
        public string rol_Nombre
        {
            get;
            set;
        }
    }
    public class cUsuario
    {
        public int usu_codigo { get; set; }
        public int usu_codRol { get; set; }
        public int? usu_codCliente { get; set; }
        public string rol_Nombre { get; set; }
        public string NombreYapellido { get; set; }
        public string ApNombre { get; set; }
        public string usu_nombre { get; set; }
        public string usu_apellido { get; set; }
        public string usu_login { get; set; }
        public string usu_mail { get; set; }
        public string usu_pswDesencriptado { get; set; }
        public string usu_observacion { get; set; }
        public int usu_estado { get; set; }
        public string usu_estadoToString { get; set; }
        public string cli_nombre { get; set; }
        public List<string> listaPermisoDenegados { get; set; }
    }
    public class cRegla
    {
        public cRegla()
        {

        }
        public int rgl_codRegla { get; set; }
        public string rgl_Descripcion { get; set; }
        public string rgl_PalabraClave { get; set; }
        public bool? rgl_IsAgregarSoporta { get; set; }
        public bool? rgl_IsEditarSoporta { get; set; }
        public bool? rgl_IsEliminarSoporta { get; set; }
        public int? rgl_codReglaPadre { get; set; }
    }
    public class cRol
    {
        public int rol_codRol { get; set; }
        public string rol_Nombre { get; set; }
    }
    public class AcccionesRol
    {
        public int idRegla
        {
            get;
            set;
        }
        public int? idReglaRol
        {
            get;
            set;
        }
        public string palabraClave
        {
            get;
            set;
        }
        public bool isActivo
        {
            get;
            set;
        }
        public bool isAgregar
        {
            get;
            set;
        }
        public bool isEditar
        {
            get;
            set;
        }
        public bool isEliminar
        {
            get;
            set;
        }

    }
    public class ListaAcccionesRol
    {
        public ListaAcccionesRol()
        {
            lista = new List<AcccionesRol>();
        }
        public List<AcccionesRol> lista { get; set; }

        public void Agregar(AcccionesRol pAcccionesRol)
        {
            lista.Add(pAcccionesRol);
        }
        public AcccionesRol Buscar(string pPalabraClave)
        {
            AcccionesRol resultado = new AcccionesRol();
            foreach (AcccionesRol item in lista)
            {
                if (item.palabraClave == pPalabraClave)
                {
                    resultado = item;
                    break;
                }
            }
            return resultado;
        }
        public bool isActivo(string pPalabraClave)
        {
            return Buscar(pPalabraClave).isActivo;
        }
        public bool isAgregar(string pPalabraClave)
        {
            return Buscar(pPalabraClave).isAgregar;
        }
        public bool isEditar(string pPalabraClave)
        {
            return Buscar(pPalabraClave).isEditar;
        }
        public bool isEliminar(string pPalabraClave)
        {
            return Buscar(pPalabraClave).isEliminar;
        }
    }
    public class cCombo
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }
    public class cPdfComprobante
    {
        public int index { get; set; }
        public string nombreArchivo { get; set; }
        public bool isOk { get; set; }
    }
}
