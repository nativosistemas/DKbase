using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKbase.web
{
    public class ordenamientoExpresion
    {
        public ordenamientoExpresion(string sortExpression)
        {
            isOrderBy = false;
            OrderByField = string.Empty;
            OrderByAsc = true;
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Length > Constantes.cDESC.Length && sortExpression.Length > Constantes.cASC.Length)
                {
                    if (sortExpression.Substring(sortExpression.Length - Constantes.cASC.Length, Constantes.cASC.Length).Contains(Constantes.cASC))
                    {
                        OrderByField = sortExpression.Substring(0, sortExpression.Length - Constantes.cASC.Length).Trim();
                        isOrderBy = true;
                    }
                    else if (sortExpression.Substring(sortExpression.Length - Constantes.cDESC.Length, Constantes.cDESC.Length).Contains(Constantes.cDESC))
                    {
                        OrderByField = sortExpression.Substring(0, sortExpression.Length - Constantes.cDESC.Length).Trim();
                        OrderByAsc = false;
                        isOrderBy = true;
                    }
                }
                if (!isOrderBy)
                {
                    OrderByField = sortExpression.Trim();
                    isOrderBy = true;
                }
            }
        }
        public bool isOrderBy { get; set; }
        public string OrderByField { get; set; }
        public bool OrderByAsc { get; set; }
    }
    public class AccesoGrilla_base
    {
        public static List<cUsuario> GetUsuariosDeCliente(string sortExpression, int pIdCliente, string pFiltro)
        {
            ordenamientoExpresion order = new ordenamientoExpresion(sortExpression);
            string filtro = string.Empty;
            if (pFiltro != null)
            {
                filtro = pFiltro;
            }
            var query =DKbase.Util.RecuperarUsuariosDeCliente(Constantes.cROL_OPERADORCLIENTE, pIdCliente, filtro);
            if (order.isOrderBy)
            {
                if (order.OrderByAsc)
                {
                    switch (order.OrderByField)
                    {
                        case "usu_nombre":
                            query = query.OrderBy(b => b.usu_nombre).ToList();
                            break;
                        case "usu_apellido":
                            query = query.OrderBy(b => b.usu_apellido).ToList();
                            break;
                        case "usu_mail":
                            query = query.OrderBy(b => b.usu_mail).ToList();
                            break;
                        case "usu_login":
                            query = query.OrderBy(b => b.usu_login).ToList();
                            break;
                        case "NombreYapellido":
                            query = query.OrderBy(b => b.NombreYapellido).ToList();
                            break;
                        case "rol_Nombre":
                            query = query.OrderBy(b => b.rol_Nombre).ToList();
                            break;
                        case "usu_estadoToString":
                            query = query.OrderBy(b => b.usu_estadoToString).ToList();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (order.OrderByField)
                    {
                        case "usu_nombre":
                            query = query.OrderByDescending(b => b.usu_nombre).ToList();
                            break;
                        case "usu_apellido":
                            query = query.OrderByDescending(b => b.usu_apellido).ToList();
                            break;
                        case "usu_mail":
                            query = query.OrderByDescending(b => b.usu_mail).ToList();
                            break;
                        case "usu_login":
                            query = query.OrderByDescending(b => b.usu_login).ToList();
                            break;
                        case "NombreYapellido":
                            query = query.OrderByDescending(b => b.NombreYapellido).ToList();
                            break;
                        case "rol_Nombre":
                            query = query.OrderByDescending(b => b.rol_Nombre).ToList();
                            break;
                        case "usu_estadoToString":
                            query = query.OrderByDescending(b => b.usu_estadoToString).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }
            return query;
        }
    }
}
