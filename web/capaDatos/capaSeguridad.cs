using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
   public class capaSeguridad_base
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
        public static DataTable RecuperarTablaBandera(string ban_codigo)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Seguridad.spEstadoBandera", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paBan_codigo = cmdComandoInicio.Parameters.Add("@ban_codigo", SqlDbType.NVarChar, 50);
            paBan_codigo.Value = ban_codigo;

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
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, ban_codigo);
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
        public static DataTable RecuperarSinPermisoUsuarioIntranetPorIdUsuario(int pIdUsuario)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Seguridad.spRecuperarSinPermisoUsuarioIntranetPorIdUsuario", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@idUsuario", SqlDbType.Int);
            paIdUsuario.Direction = ParameterDirection.Input;

            paIdUsuario.Value = pIdUsuario;

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
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pIdUsuario);
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
        public static DataTable IsRepetidoLogin(int usu_codigo, string usu_login)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Seguridad.spIsRepetidoLogin", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paUsu_codigo = cmdComandoInicio.Parameters.Add("@usu_codigo", SqlDbType.Int);
            SqlParameter paUsu_login = cmdComandoInicio.Parameters.Add("@usu_login", SqlDbType.NVarChar, 255);

            paUsu_codigo.Value = usu_codigo;
            paUsu_login.Value = usu_login;

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
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, usu_codigo, usu_login);
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
        public static cUsuario ConvertToUsuario(DataRow pItem)
        {
            cUsuario obj = new cUsuario();
            if (pItem["usu_codigo"] != DBNull.Value)
            {
                obj.usu_codigo = Convert.ToInt32(pItem["usu_codigo"]);
            }
            if (pItem["usu_codRol"] != DBNull.Value)
            {
                obj.usu_codRol = Convert.ToInt32(pItem["usu_codRol"]);
            }
            if (pItem["usu_codCliente"] != DBNull.Value)
            {
                obj.usu_codCliente = Convert.ToInt32(pItem["usu_codCliente"]);
                if (pItem.Table.Columns.Contains("cli_nombre"))
                {
                    obj.cli_nombre = pItem["cli_nombre"].ToString();
                }
            }
            else
            {
                obj.usu_codCliente = null;
            }
            if (pItem["usu_nombre"] != DBNull.Value)
            {
                obj.usu_nombre = pItem["usu_nombre"].ToString();
            }
            if (pItem["usu_apellido"] != DBNull.Value)
            {
                obj.usu_apellido = pItem["usu_apellido"].ToString();
            }
            if (pItem["NombreYapellido"] != DBNull.Value)
            {
                obj.NombreYapellido = pItem["NombreYapellido"].ToString();
            }
            if (pItem["usu_login"] != DBNull.Value)
            {
                obj.usu_login = pItem["usu_login"].ToString();
            }
            if (pItem["usu_mail"] != DBNull.Value)
            {
                obj.usu_mail = pItem["usu_mail"].ToString();
            }
            if (pItem["usu_pswDesencriptado"] != DBNull.Value)
            {
                obj.usu_pswDesencriptado = pItem["usu_pswDesencriptado"].ToString();
            }
            if (pItem["rol_Nombre"] != DBNull.Value)
            {
                obj.rol_Nombre = pItem["rol_Nombre"].ToString();
            }
            if (pItem["usu_observacion"] != DBNull.Value)
            {
                obj.usu_observacion = pItem["usu_observacion"].ToString();
            }
            if (pItem["usu_estado"] != DBNull.Value)
            {
                obj.usu_estado = Convert.ToInt32(pItem["usu_estado"]);
                obj.usu_estadoToString = capaSeguridad_base.obtenerStringEstado(obj.usu_estado);
            }
            return obj;
        }
        public static DataSet GestiónUsuario(int? usu_codigo, int? usu_codRol, int? usu_codCliente, string usu_nombre, string usu_apellido, string usu_mail, string usu_login, string usu_psw, string usu_observacion, int? usu_codUsuarioUltMov, int? usu_codAccion, int? usu_estado, string filtro, string accion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Seguridad.spGestionUsuario", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paUsu_codigo = cmdComandoInicio.Parameters.Add("@usu_codigo", SqlDbType.Int);
            SqlParameter paUsu_codRol = cmdComandoInicio.Parameters.Add("@usu_codRol", SqlDbType.Int);
            SqlParameter paUsu_codCliente = cmdComandoInicio.Parameters.Add("@usu_codCliente", SqlDbType.Int);
            SqlParameter paUsu_nombre = cmdComandoInicio.Parameters.Add("@usu_nombre", SqlDbType.NVarChar, 50);
            SqlParameter paUsu_apellido = cmdComandoInicio.Parameters.Add("@usu_apellido", SqlDbType.NVarChar, 50);
            SqlParameter paUsu_mail = cmdComandoInicio.Parameters.Add("@usu_mail", SqlDbType.NVarChar, 255);
            SqlParameter paUsu_login = cmdComandoInicio.Parameters.Add("@usu_login", SqlDbType.NVarChar, 255);
            SqlParameter paUsu_psw = cmdComandoInicio.Parameters.Add("@usu_psw", SqlDbType.NVarChar, 255);// SqlDbType.VarBinary, 255
            SqlParameter paUsu_observacion = cmdComandoInicio.Parameters.Add("@usu_observacion", SqlDbType.NVarChar, -1);
            SqlParameter paUsu_codUsuarioUltMov = cmdComandoInicio.Parameters.Add("@usu_codUsuarioUltMov", SqlDbType.Int);
            SqlParameter paUsu_codAccion = cmdComandoInicio.Parameters.Add("@usu_codAccion", SqlDbType.Int);
            SqlParameter paUsu_estado = cmdComandoInicio.Parameters.Add("@usu_estado", SqlDbType.Int);
            SqlParameter paFiltro = cmdComandoInicio.Parameters.Add("@filtro", SqlDbType.NVarChar, 50);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 50);

            if (usu_codigo == null)
            {
                paUsu_codigo.Value = DBNull.Value;
            }
            else
            {
                paUsu_codigo.Value = usu_codigo;
            }
            if (usu_codRol == null)
            {
                paUsu_codRol.Value = DBNull.Value;
            }
            else
            {
                paUsu_codRol.Value = usu_codRol;
            }
            if (usu_codCliente == null)
            {
                paUsu_codCliente.Value = DBNull.Value;
            }
            else
            {
                paUsu_codCliente.Value = usu_codCliente;
            }
            if (usu_nombre == null)
            {
                paUsu_nombre.Value = DBNull.Value;
            }
            else
            {
                paUsu_nombre.Value = usu_nombre;
            }
            if (usu_apellido == null)
            {
                paUsu_apellido.Value = DBNull.Value;
            }
            else
            {
                paUsu_apellido.Value = usu_apellido;
            }
            if (usu_mail == null)
            {
                paUsu_mail.Value = DBNull.Value;
            }
            else
            {
                paUsu_mail.Value = usu_mail;
            }
            if (usu_login == null)
            {
                paUsu_login.Value = DBNull.Value;
            }
            else
            {
                paUsu_login.Value = usu_login;
            }
            if (usu_psw == null)
            {
                paUsu_psw.Value = DBNull.Value;
            }
            else
            {
                paUsu_psw.Value = usu_psw;
            }
            if (usu_observacion == null)
            {
                paUsu_observacion.Value = DBNull.Value;
            }
            else
            {
                paUsu_observacion.Value = usu_observacion;
            }
            if (usu_estado == null)
            {
                paUsu_estado.Value = DBNull.Value;
            }
            else
            {
                paUsu_estado.Value = usu_estado;
            }
            if (usu_codUsuarioUltMov == null)
            {
                paUsu_codUsuarioUltMov.Value = DBNull.Value;
            }
            else
            {
                paUsu_codUsuarioUltMov.Value = usu_codUsuarioUltMov;
            }
            if (usu_codAccion == null)
            {
                paUsu_codAccion.Value = DBNull.Value;
            }
            else
            {
                paUsu_codAccion.Value = usu_codAccion;
            }
            if (filtro == null)
            {
                paFiltro.Value = DBNull.Value;
            }
            else
            {
                paFiltro.Value = filtro;
            }
            paAccion.Value = accion;

            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "Usuario");
                return dsResultado;
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
        public static int InsertarSinPermisoUsuarioIntranetPorIdUsuario(int pIdUsuario, DataTable pTablaNombresSeccion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Seguridad.spInsertarSinPermisoUsuarioIntranet", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@idUsuario", SqlDbType.Int);
            SqlParameter paTablaNombresSeccion = cmdComandoInicio.Parameters.Add("@Tabla_NombreSeccion", SqlDbType.Structured);

            paIdUsuario.Value = pIdUsuario;
            paTablaNombresSeccion.Value = pTablaNombresSeccion;

            try
            {
                Conn.Open();
                int count = Convert.ToInt32(cmdComandoInicio.ExecuteScalar());
                return count;

            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                return -1;
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
