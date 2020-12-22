using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
   public class capaSeguridad
    {
        public static DataSet Login(string pNombreLogin, string pPassword, string pIp, string pHostName, string pUserAgent)
        {
            string procedureName = "Seguridad.spInicioSession";
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand(procedureName, Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paLogin = cmdComandoInicio.Parameters.Add("@login", SqlDbType.NVarChar, 255);
            SqlParameter paPassword = cmdComandoInicio.Parameters.Add("@Password", SqlDbType.NVarChar, 255);
            SqlParameter paIp = cmdComandoInicio.Parameters.Add("@Ip", SqlDbType.NVarChar, 255);
            SqlParameter paHost = cmdComandoInicio.Parameters.Add("@Host", SqlDbType.NVarChar, 255);
            SqlParameter paUserName = cmdComandoInicio.Parameters.Add("@UserName", SqlDbType.NVarChar, 255);

            paLogin.Value = pNombreLogin;
            paPassword.Value = pPassword;
            paIp.Value = pIp;
            paHost.Value = pHostName;
            paUserName.Value = pUserAgent;

            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "Login");
                return dsResultado;
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName, pNombreLogin, pPassword,  pIp,  pHostName,  pUserAgent);
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
        public static void CerrarSession(int pIdUsuarioLog)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("IdUsuarioLog", pIdUsuarioLog));
            db.ExecuteNonQuery("Seguridad.spCerrarSession", l);
        }
        public static DataTable RecuperarTodasAccionesRol(int pIdRol)
        {
            string procedureName = "Seguridad.spRecuperarAccionesUsuario";
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand(procedureName, Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdRol = cmdComandoInicio.Parameters.Add("@IdRol", SqlDbType.Int);
            paIdRol.Direction = ParameterDirection.Input;

            paIdRol.Value = pIdRol;

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
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName, pIdRol);
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
        public static string obtenerStringEstado(int pIdEstado)
        {
            string resultado = string.Empty;
            switch (pIdEstado.ToString())
            {
                case "1":
                    return Constantes.cESTADO_STRING_SINESTADO;
                case "2":
                    return Constantes.cESTADO_STRING_ACTIVO;
                case "3":
                    return Constantes.cESTADO_STRING_INACTIVO;
                default:
                    break;
            }
            return resultado;
        }
    }
}
