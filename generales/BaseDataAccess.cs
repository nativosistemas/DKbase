using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;

namespace DKbase.generales
{
    public class BaseDataAccess
    {
        protected string ConnectionString { get; set; }

        public BaseDataAccess() { }

        public BaseDataAccess(string connectionString)
        {

            this.ConnectionString = connectionString;
        }
        public SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;

        }

        public SqlCommand GetCommand(SqlConnection connection, string commandText, CommandType commandType)
        {
            SqlCommand command = new SqlCommand(commandText, connection as SqlConnection);
            command.CommandType = commandType;
            return command;
        }
        public SqlParameter GetParameter(string parameter, object value)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, value != null ? value : DBNull.Value);
            parameterObject.Direction = ParameterDirection.Input;
            return parameterObject;
        }
        public SqlParameter GetParameter(string parameter, object value, SqlDbType type)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, value != null ? value : DBNull.Value);
            parameterObject.SqlDbType = type;
            parameterObject.Direction = ParameterDirection.Input;
            return parameterObject;
        }
        public SqlParameter GetParameterOut(string parameter, SqlDbType type, object value = null, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, type);
            if (type == SqlDbType.NVarChar || type == SqlDbType.VarChar || type == SqlDbType.NText || type == SqlDbType.Text)
            {
                parameterObject.Size = -1;
            }

            parameterObject.Direction = parameterDirection;

            if (value != null)
            {
                parameterObject.Value = value;
            }
            else
            {
                parameterObject.Value = DBNull.Value;
            }

            return parameterObject;
        }
        public int ExecuteNonQuery_forError(string procedureName, List<SqlParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            int returnValue = -1;
            using (SqlConnection connection = this.GetConnection())
            {
                SqlCommand cmd = this.GetCommand(connection, procedureName, commandType);
                if (parameters != null && parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                returnValue = cmd.ExecuteNonQuery();
            }
            return returnValue;
        }
        public int ExecuteNonQuery(string procedureName, List<SqlParameter> parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {

            int returnValue = -1;

            try
            {

                using (SqlConnection connection = this.GetConnection())
                {
                    SqlCommand cmd = this.GetCommand(connection, procedureName, commandType);
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    returnValue = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName, parameters, commandType);
            }

            return returnValue;
        }
        public async Task<int?> ExecuteNonQueryAsync(string procedureName, List<SqlParameter> parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {

            try
            {
                using (SqlConnection connection = this.GetConnection())
                {
                    using (SqlCommand command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Count > 0)
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                        }

                        //  await connection.OpenAsync();

                        return await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                return null;
            }
        }
        public object ExecuteScalar(string procedureName, List<SqlParameter> parameters = null)
        {
            object returnValue = null;

            try
            {
                using (SqlConnection connection = this.GetConnection())
                {

                    SqlCommand cmd = this.GetCommand(connection, procedureName, CommandType.StoredProcedure);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    returnValue = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName, parameters);
            }
            return returnValue;
        }

        public SqlDataReader GetDataReader(string procedureName, List<SqlParameter> parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                using (SqlConnection connection = this.GetConnection())
                {
                    SqlCommand cmd = this.GetCommand(connection, procedureName, commandType);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return dr;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName, parameters, commandType);
            }

            return null;
        }
        public DataTable GetDataTable(string procedureName, List<SqlParameter> parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                using (SqlConnection connection = this.GetConnection())
                {
                    SqlCommand cmd = this.GetCommand(connection, procedureName, commandType);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName, parameters, commandType);
            }

            return null;
        }
        public DataSet GetDataSet(string procedureName, List<SqlParameter> parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                using (SqlConnection connection = this.GetConnection())
                {
                    SqlCommand cmd = this.GetCommand(connection, procedureName, commandType);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    DataSet dsResultado = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dsResultado, "dsGeneric");
                    return dsResultado;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, procedureName, parameters, commandType);
            }

            return null;
        }
    }
}

