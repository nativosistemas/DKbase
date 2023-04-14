using DKbase.dll;
using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class capaDevoluciones_base
    {
        public static DataTable RecuperarItemsReclamoFacturadoNoEnviado(int pIdCliente)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Devoluciones.spRecuperarItemsReclamoFacturadoNoEnviado", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@numerocliente", SqlDbType.Int);
            paIdCliente.Value = pIdCliente;

            try
            {
                Conn.Open();
                DataTable dt = new DataTable();
                SqlDataReader LectorSQLdata = cmdComandoInicio.ExecuteReader();
                dt.Load(LectorSQLdata);
                return dt;
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                return null;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static DataTable RecuperarItemsDevolucionPrecargaVencidosPorCliente(int pIdCliente)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Devoluciones.spRecuperarItemsDevolucionesPrecargaVencidosPorCliente", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@numerocliente", SqlDbType.Int);
            paIdCliente.Value = pIdCliente;

            try
            {
                Conn.Open();
                DataTable dt = new DataTable();
                SqlDataReader LectorSQLdata = cmdComandoInicio.ExecuteReader();
                dt.Load(LectorSQLdata);
                return dt;
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                return null;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static DataTable RecuperarItemsDevolucionPrecargaPorCliente(int pIdCliente)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Devoluciones.spRecuperarItemsDevolucionesPrecargaPorCliente", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@numerocliente", SqlDbType.Int);
            paIdCliente.Value = pIdCliente;

            try
            {
                Conn.Open();
                DataTable dt = new DataTable();
                SqlDataReader LectorSQLdata = cmdComandoInicio.ExecuteReader();
                dt.Load(LectorSQLdata);
                return dt;
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                return null;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static DataTable RecuperarItemsDevolucionPrecargaFacturaCompletaPorCliente(int pIdCliente)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Devoluciones.spRecuperarItemsDevolucionesPrecargaFacturaCompletaPorCliente", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@numerocliente", SqlDbType.Int);
            paIdCliente.Value = pIdCliente;

            try
            {
                Conn.Open();
                DataTable dt = new DataTable();
                SqlDataReader LectorSQLdata = cmdComandoInicio.ExecuteReader();
                dt.Load(LectorSQLdata);
                return dt;
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                return null;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static cDevolucionItemPrecarga ConvertToItemDevPrecarga(DataRow pItem)
        {
            cDevolucionItemPrecarga obj = new cDevolucionItemPrecarga();

            if (pItem["dev_numeroitem"] != DBNull.Value)
            {
                obj.dev_numeroitem = Convert.ToInt32(pItem["dev_numeroitem"]);
            }

            if (pItem["dev_numerocliente"] != DBNull.Value)
            {
                obj.dev_numerocliente = Convert.ToInt32(pItem["dev_numerocliente"]);
            }
            if (pItem["dev_numerofactura"] != DBNull.Value)
            {
                obj.dev_numerofactura = pItem["dev_numerofactura"].ToString();
            }
            if (pItem["dev_nombreproductodevolucion"] != DBNull.Value)
            {
                obj.dev_nombreproductodevolucion = pItem["dev_nombreproductodevolucion"].ToString();
            }
            if (pItem["dev_fecha"] != DBNull.Value)
            {
                obj.dev_fecha = Convert.ToDateTime(pItem["dev_fecha"]);
                obj.dev_fechaToString = Convert.ToDateTime(pItem["dev_fecha"]).ToShortDateString();
            }
            if (pItem["dev_motivo"] != DBNull.Value)
            {
                obj.dev_motivo = (dllMotivoDevolucion)Convert.ToInt32(pItem["dev_motivo"]);
               // obj.dev_motivo_int = Convert.ToInt32(pItem["dev_motivo"]);
            }
            if (pItem["dev_numeroitemfactura"] != DBNull.Value)
            {
                obj.dev_numeroitemfactura = Convert.ToInt32(pItem["dev_numeroitemfactura"]);
            }
            if (pItem["dev_nombreproductofactura"] != DBNull.Value)
            {
                obj.dev_nombreproductofactura = pItem["dev_nombreproductofactura"].ToString();
            }
            if (pItem["dev_cantidad"] != DBNull.Value)
            {
                obj.dev_cantidad = Convert.ToInt32(pItem["dev_cantidad"]);
            }
            if (pItem["dev_numerolote"] != DBNull.Value)
            {
                obj.dev_numerolote = pItem["dev_numerolote"].ToString();
            }
            if (pItem["dev_fechavencimientolote"] != DBNull.Value)
            {
                obj.dev_fechavencimientolote = Convert.ToDateTime(pItem["dev_fechavencimientolote"]);
                obj.dev_fechavencimientoloteToString = Convert.ToDateTime(pItem["dev_fechavencimientolote"]).ToShortDateString();
            }
            if (pItem["dev_idsucursal"] != DBNull.Value)
            {
                obj.dev_idsucursal = pItem["dev_idsucursal"].ToString();
            }
            return obj;
        }

    }
}
