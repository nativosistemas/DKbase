using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class capaTiposEnvios_base
    {
        public static DataTable RecuperarTodosCadeteriaRestricciones()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("TiposEnvios.spRecuperarTodosCadeteriaRestricciones", Conn);
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
        public static DataTable RecuperarTodosSucursalDependienteTipoEnvioCliente()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("TiposEnvios.spRecuperarTodosSucursalDependienteTipoEnvioCliente", Conn);
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
        public static DataTable RecuperarSucursalDependienteTipoEnvioCliente_TipoEnvios_TodasLasExcepciones(int pIdSucursalDependienteTipoEnvioCliente)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("TiposEnvios.spRecuperarSucursalDependienteTipoEnvioCliente_TipoEnvios_TodasLasExcepciones", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;
            SqlParameter paTdr_idSucursalDependienteTipoEnvioCliente = cmdComandoInicio.Parameters.Add("@tdr_idSucursalDependienteTipoEnvioCliente", SqlDbType.Int);
            paTdr_idSucursalDependienteTipoEnvioCliente.Value = pIdSucursalDependienteTipoEnvioCliente;
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
        public static DataTable RecuperarTipoEnviosExcepcionesPorSucursalDependiente(int pIdSucursalDependienteTipoEnvioCliente, string tdr_codReparto)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("TiposEnvios.spRecuperarSucursalDependienteTipoEnvioCliente_TipoEnvios_Excepciones", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;
            SqlParameter paTdr_idSucursalDependienteTipoEnvioCliente = cmdComandoInicio.Parameters.Add("@tdr_idSucursalDependienteTipoEnvioCliente", SqlDbType.Int);
            SqlParameter paTdr_codReparto = cmdComandoInicio.Parameters.Add("@tdr_codReparto", SqlDbType.NVarChar, 2);

            if (string.IsNullOrEmpty(tdr_codReparto))
            {
                paTdr_codReparto.Value = DBNull.Value;
            }
            else { paTdr_codReparto.Value = tdr_codReparto; }
            paTdr_idSucursalDependienteTipoEnvioCliente.Value = pIdSucursalDependienteTipoEnvioCliente;
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
        public static DataTable RecuperarTodosSucursalDependienteTipoEnvioCliente_TiposEnvios()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("TiposEnvios.spRecuperarTodosSucursalDependienteTipoEnvioCliente_TipoEnvios", Conn);
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
        public static cSucursalDependienteTipoEnviosCliente_TiposEnvios ConvertToTiposEnviosSucursalDependiente_TiposEnvios(DataRow pItem)
        {
            cSucursalDependienteTipoEnviosCliente_TiposEnvios obj = new cSucursalDependienteTipoEnviosCliente_TiposEnvios();

            if (pItem["tdt_id"] != DBNull.Value)
            {
                obj.tdt_id = Convert.ToInt32(pItem["tdt_id"]);
            }
            if (pItem["tdt_idSucursalDependienteTipoEnvioCliente"] != DBNull.Value)
            {
                obj.tdt_idSucursalDependienteTipoEnvioCliente = Convert.ToInt32(pItem["tdt_idSucursalDependienteTipoEnvioCliente"]);
            }
            if (pItem["tdt_idTipoEnvio"] != DBNull.Value)
            {
                obj.tdt_idTipoEnvio = Convert.ToInt32(pItem["tdt_idTipoEnvio"]);
            }
            if (pItem["env_id"] != DBNull.Value)
            {
                obj.env_id = Convert.ToInt32(pItem["env_id"]);
            }
            if (pItem["env_codigo"] != DBNull.Value)
            {
                obj.env_codigo = Convert.ToString(pItem["env_codigo"]);
            }
            if (pItem["env_nombre"] != DBNull.Value)
            {
                obj.env_nombre = Convert.ToString(pItem["env_nombre"]);
            }
            return obj;
        }
        public static cSucursalDependienteTipoEnviosCliente ConvertToTiposEnviosSucursalDependiente(DataRow pItem)
        {
            cSucursalDependienteTipoEnviosCliente obj = new cSucursalDependienteTipoEnviosCliente();

            if (pItem["sde_codigo"] != DBNull.Value)
            {
                obj.sde_codigo = Convert.ToInt32(pItem["sde_codigo"]);
            }
            if (pItem["sde_sucursal"] != DBNull.Value)
            {
                obj.sde_sucursal = Convert.ToString(pItem["sde_sucursal"]);
            }
            if (pItem["sde_sucursalDependiente"] != DBNull.Value)
            {
                obj.sde_sucursalDependiente = Convert.ToString(pItem["sde_sucursalDependiente"]);
            }
            if (pItem["tsd_id"] != DBNull.Value)
            {
                obj.tsd_id = Convert.ToInt32(pItem["tsd_id"]);
            }
            if (pItem["tsd_idSucursalDependiente"] != DBNull.Value)
            {
                obj.tsd_idSucursalDependiente = Convert.ToInt32(pItem["tsd_idSucursalDependiente"]);
            }
            if (pItem["tsd_idTipoEnvioCliente"] != DBNull.Value)
            {
                obj.tsd_idTipoEnvioCliente = Convert.ToInt32(pItem["tsd_idTipoEnvioCliente"]);
            }
            else
            {

                obj.env_nombre = "Todos tipos envíos";
            }
            if (pItem["env_id"] != DBNull.Value)
            {
                obj.env_id = Convert.ToInt32(pItem["env_id"]);
            }
            if (pItem["env_codigo"] != DBNull.Value)
            {
                obj.env_codigo = Convert.ToString(pItem["env_codigo"]);
            }
            if (pItem["env_nombre"] != DBNull.Value)
            {
                obj.env_nombre = Convert.ToString(pItem["env_nombre"]);
            }
            return obj;
        }
        public static cSucursalDependienteTipoEnviosCliente_TiposEnvios ConvertToTiposEnviosSucursalDependiente_TiposEnvios_Excepciones(DataRow pItem)
        {
            cSucursalDependienteTipoEnviosCliente_TiposEnvios obj = new cSucursalDependienteTipoEnviosCliente_TiposEnvios();

            if (pItem.Table.Columns.Contains("tdr_idSucursalDependienteTipoEnvioCliente") && pItem["tdr_idSucursalDependienteTipoEnvioCliente"] != DBNull.Value)
            {
                obj.tdt_idSucursalDependienteTipoEnvioCliente = Convert.ToInt32(pItem["tdr_idSucursalDependienteTipoEnvioCliente"]);
            }
            if (pItem.Table.Columns.Contains("tdr_idTipoEnvio") && pItem["tdr_idTipoEnvio"] != DBNull.Value)
            {
                obj.tdt_idTipoEnvio = Convert.ToInt32(pItem["tdr_idTipoEnvio"]);
            }
            if (pItem.Table.Columns.Contains("env_id") && pItem["env_id"] != DBNull.Value)
            {
                obj.env_id = Convert.ToInt32(pItem["env_id"]);
            }
            if (pItem.Table.Columns.Contains("env_codigo") && pItem["env_codigo"] != DBNull.Value)
            {
                obj.env_codigo = Convert.ToString(pItem["env_codigo"]);
            }
            if (pItem.Table.Columns.Contains("env_nombre") && pItem["env_nombre"] != DBNull.Value)
            {
                obj.env_nombre = Convert.ToString(pItem["env_nombre"]);
            }
            if (pItem.Table.Columns.Contains("tdr_codReparto") && pItem["tdr_codReparto"] != DBNull.Value)
            {
                obj.tdr_codReparto = Convert.ToString(pItem["tdr_codReparto"]);
            }
            return obj;
        }
        public static cCadeteriaRestricciones ConvertToCadeteriaRestricciones(DataRow pItem)
        {
            cCadeteriaRestricciones obj = new cCadeteriaRestricciones();

            if (pItem["tcr_id"] != DBNull.Value)
            {
                obj.tcr_id = Convert.ToInt32(pItem["tcr_id"]);
            }
            if (pItem["tcr_codigoSucursal"] != DBNull.Value)
            {
                obj.tcr_codigoSucursal = Convert.ToString(pItem["tcr_codigoSucursal"]);
            }
            if (pItem["tcr_MontoIgnorar"] != DBNull.Value)
            {
                obj.tcr_MontoIgnorar = Convert.ToDouble(pItem["tcr_MontoIgnorar"]);
            }
            if (pItem["tcr_MontoMinimo"] != DBNull.Value)
            {
                obj.tcr_MontoMinimo = Convert.ToDouble(pItem["tcr_MontoMinimo"]);
            }
            if (pItem["tcr_UnidadesMaximas"] != DBNull.Value)
            {
                obj.tcr_UnidadesMaximas = Convert.ToInt32(pItem["tcr_UnidadesMaximas"]);
            }
            if (pItem["tcr_UnidadesMinimas"] != DBNull.Value)
            {
                obj.tcr_UnidadesMinimas = Convert.ToInt32(pItem["tcr_UnidadesMinimas"]);
            }
            if (pItem["suc_nombre"] != DBNull.Value)
            {
                obj.suc_nombre = Convert.ToString(pItem["suc_nombre"]);
            }
            return obj;
        }
    }
}
