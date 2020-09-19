using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class capaClientes
    {
        public static DataSet GestiónSucursal(int? sde_codigo, string sde_sucursal, string sde_sucursalDependiente, string accion)
        {
            string procedureName = "Clientes.spGestionSucursal";
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand(procedureName, Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paSde_codigo = cmdComandoInicio.Parameters.Add("@sde_codigo", SqlDbType.Int);
            SqlParameter paSde_sucursal = cmdComandoInicio.Parameters.Add("@sde_sucursal", SqlDbType.NVarChar, 2);
            SqlParameter paSde_sucursalDependiente = cmdComandoInicio.Parameters.Add("@sde_sucursalDependiente", SqlDbType.NVarChar, 2);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 50);

            if (sde_codigo == null)
            {
                paSde_codigo.Value = DBNull.Value;
            }
            else
            {
                paSde_codigo.Value = sde_codigo;
            }

            if (sde_sucursal == null)
            {
                paSde_sucursal.Value = DBNull.Value;
            }
            else
            {
                paSde_sucursal.Value = sde_sucursal;
            }

            if (sde_sucursalDependiente == null)
            {
                paSde_sucursalDependiente.Value = DBNull.Value;
            }
            else
            {
                paSde_sucursalDependiente.Value = sde_sucursalDependiente;
            }
            paAccion.Value = accion;

            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "Sucursal");
                return dsResultado;
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName, sde_codigo,  sde_sucursal,  sde_sucursalDependiente,  accion);
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
        public static DataTable RecuperarTodosCodigoReparto()
        {
            string procedureName = "Clientes.spRecuperarTodosCodigoReparto";
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand(procedureName, Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;
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
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName);

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
    }
}
