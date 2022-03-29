using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.generales
{
   public class Constantes
    {
        public enum CargarProductosBuscador { isDesdeBuscador = 1, isDesdeBuscador_OfertaTransfer, isSubirArchivo, isDesdeTabla, isRecuperadorFaltaCredito };
        public static string cSQL_INSERT
        {
            get { return "INSERT"; }
        }
        public static string cSQL_UPDATE
        {
            get { return "UPDATE"; }
        }
        public static string cSQL_SELECT
        {
            get { return "SELECT"; }
        }
        public static string cSQL_COMBO
        {
            get { return "COMBO"; }
        }
        public static string cSQL_ESTADO
        {
            get { return "ESTADO"; }
        }
        public static string cSQL_DELETE
        {
            get { return "DELETE"; }
        }
        public static string cSQL_CAMBIOCONTRASEÑA
        {
            get { return "CAMBIOCONTRASEÑA"; }
        }
        public static string cESTADO_STRING_SINESTADO
        {
            get { return "Sin Estado"; }
        }
        public static string cESTADO_STRING_ACTIVO
        {
            get { return "Activo"; }
        }
        public static string cESTADO_STRING_INACTIVO
        {
            get { return "Inactivo"; }
        }
        public static string cSQL_PUBLICAR
        {
            get { return "PUBLICAR"; }
        }
        public static string cSQL_SUBIR
        {
            get { return "SUBIR"; }
        }
        public static string cSQL_BAJAR
        {
            get { return "BAJAR"; }
        }

        public static string cDESC
        {
            get { return "DESC"; }
        }
        public static string cASC
        {
            get { return "ASC"; }
        }
        public static string cLABORATORIO
        {
            get { return "laboratorio"; }
        }
        public static int cImg_ancho_ampliar_dafault
        {
            get { return 1024; }
        }
        public static int cImg_alto_ampliar_dafault
        {
            get { return 768; }
        }
        public static string cTomarPedidoCC
        {
            get { return "TomarPedidoCC"; }
        }
        public static string cTipoMensaje_Cliente
        {
            get { return "cliente"; }
        }
        public static string cTipoMensaje_Sucursal
        {
            get { return "sucursal"; }
        }
        public static string cTipoMensaje_Reparto
        {
            get { return "reparto"; }
        }
        public static string cTipo_Carrito
        {
            get { return "Carrito"; }
        }
        public static string cTipo_CarritoTransfers
        {
            get { return "CarritoTransfers"; }
        }
        public static string cTipo_CarritoDiferido
        {
            get { return "CarritoDiferido"; }
        }
        public static string cTipo_CarritoDiferidoTransfers
        {
            get { return "CarritoDiferidoTransfers"; }
        }
        public static string cDIASEMANA_Lunes
        {
            get { return "LU"; }
        }
        public static string cDIASEMANA_Martes
        {
            get { return "MA"; }
        }
        public static string cDIASEMANA_Miercoles
        {
            get { return "MI"; }
        }
        public static string cDIASEMANA_Jueves
        {
            get { return "JU"; }
        }
        public static string cDIASEMANA_Viernes
        {
            get { return "VI"; }
        }
        public static string cDIASEMANA_Sabado
        {
            get { return "SA"; }
        }
        public static string cDIASEMANA_Domingo
        {
            get { return "DO"; }
        }
    }
}
