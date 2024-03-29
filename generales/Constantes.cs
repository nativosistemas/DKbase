﻿using System;
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
        public static string cTIPOPRODUCTO_Medicamento
        {
            get { return "M"; }
        }
        public static string cTIPOPRODUCTO_Perfumeria
        {
            get { return "P"; }
        }
        public static string cTIPOPRODUCTO_Accesorio
        {
            get { return "A"; }
        }
        public static string cTIPOPRODUCTO_PerfumeriaCuentaYOrden
        {
            get { return "F"; }
        }
        public static string cTipoCliente_Perfumeria
        {
            get { return "P"; }
        }
        public static string cTipoCliente_Todos
        {
            get { return "D"; }
        }
        public static string cAccionCarrito_VACIAR
        {
            get { return "VACIAR CARRITO"; }
        }
        public static string cAccionCarrito_TOMAR
        {
            get { return "TOMAR PEDIDO"; }
        }
        public static string cAccionCarrito_BORRAR_CARRRITO_REPETIDO
        {
            get { return "BORRAR CARRRITO REPETIDO"; }
        }
        public static int cPEDIDO_FALTANTES
        {
            get { return 1; }
        }
        public static int cPEDIDO_PROBLEMACREDITICIO
        {
            get { return 2; }
        }
        public static string cBAN_SERVIDORDLL
        {
            get { return "servidorDLL"; }
        }
        public static int cROL_ADMINISTRADOR
        {
            get { return 1; }
        }
        public static int cROL_ADMINISTRADORCLIENTE
        {
            get { return 2; }
        }
        public static int cROL_OPERADORCLIENTE
        {
            get { return 3; }
        }
        public static int cROL_PROMOTOR
        {
            get { return 10; }
        }
        public static int cROL_ENCSUCURSAL
        {
            get { return 11; }
        }
        public static int cROL_ENCGRAL
        {
            get { return 12; }
        }
        public static int cROL_GRUPOCLIENTE
        {
            get { return 13; }
        }
        public static string cSECCION_PEDIDOS
        {
            get { return "PEDIDOS"; }
        }
        public static string cSECCION_DESCARGAS
        {
            get { return "DESCARGAS"; }
        }
        public static string cSECCION_CUENTASCORRIENTES
        {
            get { return "CUENTASCORRIENTES"; }
        }
        public static string cSECCION_DEVOLUCIONES
        {
            get { return "DEVOLUCIONES"; }
        }
        public static int cESTADO_SINESTADO
        {
            get { return 1; }
        }
        public static int cESTADO_ACTIVO
        {
            get { return 2; }
        }
        public static int cESTADO_INACTIVO
        {
            get { return 3; }
        }
        public static int cESTADO_SINLEER
        {
            get { return 4; }
        }
        public static int cESTADO_LEIDO
        {
            get { return 5; }
        }
        public static string cESTADO_HAB
        {
            get { return "HAB"; }
        }
        public static string cESTADO_INH
        {
            get { return "INH"; }
        }
        public static int cCantidadFilaPorPaginaSubirPedido
        {
            get
            {
                return 10;
            }
        }
        public static int cCantidadFilaPorPagina
        {
            get
            {
                return 100;
            }
        }
        public static int cLimiteDePaginador
        {
            get { return 8; }
        }
        public static string cTABLA_CATALOGO
        {
            get { return "catalogo"; }
        }
        public static string cTABLA_CV
        {
            get { return "curriculumvitae"; }
        }
        public static string cTABLA_PRODUCTO
        {
            get { return "producto"; }
        }
        public static string cTABLA_NOTICIA
        {
            get { return "noticia"; }
        }
        public const string cROL_const_ADMINISTRADORCLIENTE = "2";

        public const string cROL_const_OPERADORCLIENTE = "3";

        public const string cROL_const_PROMOTOR = "10";

        public const string cROL_const_ENCSUCURSAL = "11";

        public const string cROL_const_ENCGRAL = "12";

        public const string cROL_const_GRUPOCLIENTE = "13";
        public static int cACCION_ALTA
        {
            get { return 1; }
        }
        public static int cACCION_MODIFICACION
        {
            get { return 2; }
        }
        public static int cACCION_CAMBIOESTADO
        {
            get { return 3; }
        }
        public static int cACCION_CAMBIOCONTRASEÑA
        {
            get { return 4; }
        }
        public static int cACCION_CAMBIOORDEN
        {
            get { return 5; }
        }
        public static int cACCION_ISPUBLICAR
        {
            get { return 6; }
        }
        public static int cACCION_ELIMINAR
        {
            get { return 7; }
        }
        public static string cSeparadorCSV
        {
            get { return ";"; }
        }
        public static string cSaldoSinImputar
        {
            get { return "saldoSinImputar"; }
        }
        public static string cDeudaVencida
        {
            get { return "deudaVencida"; }
        }
        public static string cLog_type_subirArchivo
        {
            get { return "subirArchivo"; }
        }
    }
}
