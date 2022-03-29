using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cClientes
    {
        public cClientes()
        {
            cli_tipo = string.Empty;
        }
        public cClientes(int pCli_codigo, string pCli_nombre)
        {
            cli_codigo = pCli_codigo;
            cli_nombre = pCli_nombre;
            cli_tipo = string.Empty;
        }
        public int cli_codigo { get; set; }
        public string cli_nombre { get; set; }
        public string cli_dirección { get; set; }
        public string cli_estado { get; set; }
        public string cli_telefono { get; set; }
        public string cli_codprov { get; set; }
        public string cli_localidad { get; set; }
        public string cli_email { get; set; }
        public string cli_password { get; set; }
        public string cli_paginaweb { get; set; }
        public string cli_codsuc { get; set; }
        public decimal cli_pordesespmed { get; set; }
        public decimal cli_pordesbetmed { get; set; }
        public decimal cli_pordesnetos { get; set; }
        public decimal cli_pordesfinperfcyo { get; set; }
        public decimal cli_pordescomperfcyo { get; set; }
        public bool cli_deswebespmed { get; set; }
        public bool cli_deswebnetmed { get; set; }
        public bool cli_deswebnetacc { get; set; }
        public bool cli_deswebnetperpropio { get; set; }
        public bool cli_deswebnetpercyo { get; set; }
        public bool cli_mostraremail { get; set; }
        public bool cli_mostrartelefono { get; set; }
        public bool cli_mostrardireccion { get; set; }
        public bool cli_mostrarweb { get; set; }
        public decimal cli_destransfer { get; set; }
        public string cli_login { get; set; }
        public string cli_codtpoenv { get; set; }
        public string cli_codrep { get; set; }
        public bool cli_isGLN { get; set; }
        public bool cli_tomaOfertas { get; set; }
        public bool cli_tomaPerfumeria { get; set; }
        public bool cli_tomaTransfers { get; set; }
        public string cli_tipo { get; set; }
        public string cli_IdSucursalAlternativa { get; set; }
        private bool _cli_AceptaPsicotropicos = true;
        public bool cli_AceptaPsicotropicos { get { return _cli_AceptaPsicotropicos; } set { _cli_AceptaPsicotropicos = value; } }
        public string cli_promotor { get; set; }
        public decimal? cli_PorcentajeDescuentoDeEspecialidadesMedicinalesDirecto { get; set; }
        public string cli_GrupoCliente { get; set; }
        public string cli_NumeroCentralTelefonica { get; set; }
}
    public class cHorariosSucursal
    {
        public cHorariosSucursal()
        {
        }
        public int sdh_SucursalDependienteHorario { get; set; }
        public string sdh_sucursal { get; set; }
        public string sdh_sucursalDependiente { get; set; }
        public string sdh_codReparto { get; set; }
        public string sdh_diaSemana { get; set; }
        public string sdh_horario { get; set; }
        public List<string> listaHorarios { get; set; }
        //public string sde_sucursal { get; set; }
        //public string sde_sucursalDependiente { get; set; }
    }
    public class capaClientes_base
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
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName, sde_codigo, sde_sucursal, sde_sucursalDependiente, accion);
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
        public static DataTable RecuperarTodosClientes()
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            return db.GetDataTable("Clientes.spRecuperarTodosClientes");
        }
        public static DataTable spRecuperarTodosClientesByPromotor(string pPromotor, string pConnectionStringSQL = null)
        {
            if (pConnectionStringSQL == null)
            {
                pConnectionStringSQL = Helper.getConnectionStringSQL;
            }
            SqlConnection Conn = new SqlConnection(pConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Clientes.spRecuperarTodosClientesByPromotor", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paPromotor = cmdComandoInicio.Parameters.Add("@cli_promotor", SqlDbType.NVarChar, 75);
            paPromotor.Value = pPromotor;
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
        public static DataSet GestiónSucursalDependienteHorarios(int? sdh_SucursalDependienteHorario, string sdh_sucursal, string sdh_sucursalDependiente, string sdh_codReparto, string sdh_diaSemana, string sdh_horario, string accion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("Clientes.spGestionSucursalHorarios", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paSdh_SucursalDependienteHorario = cmdComandoInicio.Parameters.Add("@sdh_SucursalDependienteHorario", SqlDbType.Int);
            SqlParameter paSdh_sucursal = cmdComandoInicio.Parameters.Add("@sdh_sucursal", SqlDbType.NVarChar, 2);
            SqlParameter paSdh_sucursalDependiente = cmdComandoInicio.Parameters.Add("@sdh_sucursalDependiente", SqlDbType.NVarChar, 2);
            SqlParameter paSdh_codReparto = cmdComandoInicio.Parameters.Add("@sdh_codReparto", SqlDbType.NVarChar, 2);
            SqlParameter paSdh_diaSemana = cmdComandoInicio.Parameters.Add("@sdh_diaSemana", SqlDbType.NVarChar, 2);
            SqlParameter paSdh_horario = cmdComandoInicio.Parameters.Add("@sdh_horario", SqlDbType.NVarChar, 500);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 50);

            if (sdh_SucursalDependienteHorario == null)
            {
                paSdh_SucursalDependienteHorario.Value = DBNull.Value;
            }
            else
            {
                paSdh_SucursalDependienteHorario.Value = sdh_SucursalDependienteHorario;
            }
            if (sdh_sucursal == null)
            {
                paSdh_sucursal.Value = DBNull.Value;
            }
            else
            {
                paSdh_sucursal.Value = sdh_sucursal;
            }
            if (sdh_sucursalDependiente == null)
            {
                paSdh_sucursalDependiente.Value = DBNull.Value;
            }
            else
            {
                paSdh_sucursalDependiente.Value = sdh_sucursalDependiente;
            }
            if (sdh_codReparto == null)
            {
                paSdh_codReparto.Value = DBNull.Value;
            }
            else
            {
                paSdh_codReparto.Value = sdh_codReparto;
            }

            if (sdh_diaSemana == null)
            {
                paSdh_diaSemana.Value = DBNull.Value;
            }
            else
            {
                paSdh_diaSemana.Value = sdh_diaSemana;
            }
            if (sdh_horario == null)
            {
                paSdh_horario.Value = DBNull.Value;
            }
            else
            {
                paSdh_horario.Value = sdh_horario;
            }
            paAccion.Value = accion;

            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "SucursalHorario");
                return dsResultado;
            }
            catch (Exception ex)
            {
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
