using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cHistorialArchivoSubir
    {
        public int has_id { get; set; }
        public int has_codCliente { get; set; }
        public string has_NombreArchivo { get; set; }
        public string has_NombreArchivoOriginal { get; set; }
        public string has_sucursal { get; set; }
        public string suc_nombre { get; set; }
        public DateTime has_fecha { get; set; }
        public string has_fechaToString { get; set; }
    }
    public class cFaltantesConProblemasCrediticiosPadre
    {
        public string fpc_codSucursal { get; set; }
        public string suc_nombre { get; set; }
        public int fpc_tipo { get; set; }
        public List<cProductosGenerico> listaProductos { get; set; }
    }
    public class cPalabraBusqueda
    {
        public int hbp_id { get; set; }
        public string hbp_Palabra { get; set; }
        //public string hbp_NombreTabla { get; set; }
        //public int hbp_codUsuario { get; set; }
        //public DateTime? hbp_Fecha { get; set; }
    }
    public class capaLogRegistro_base
    {
        public static DataTable spForceChangePasswordFindCliente(int pIdCliente)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("codCliente", pIdCliente));
            return db.GetDataTable("spForceChangePasswordFindCliente", l);
        }
        public static int spForceChangePasswordDeleteCliente(int pIdCliente)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("codCliente", pIdCliente));
            return db.ExecuteNonQuery("spForceChangePasswordDeleteCliente", l);
        }
        public static int spForceChangePasswordHistoryAdd(int pIdCliente, int pIdUsuario, string pAction)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("codCliente", pIdCliente));
            l.Add(db.GetParameter("codUsuario", pIdUsuario));
            l.Add(db.GetParameter("action", pAction));
            return db.ExecuteNonQuery("spForceChangePasswordHistoryAdd", l);
        }
        public static string spUltimoProductoSeleccionado(int pIdUsuario)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("codCliente", pIdUsuario));
            object var = db.ExecuteScalar("LogRegistro.spUltimoProductoSeleccionado", l);
            return var != null ? var.ToString() : null;
        }
        public static DataSet AgregarProductoAlCarritoDesdeArchivoPedidosV5(string pSucursalElejida, string pSucursalCliente, DataTable pTabla, string pTipoDeArchivo, int pIdCliente, string pCli_codprov, bool pCli_isGLN, int? pIdUsuario)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spCargarCarritoProductoSucursalDesdeArchivoPedidosV5", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paSucursalElejida = cmdComandoInicio.Parameters.Add("@lrc_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paSucursalCliente = cmdComandoInicio.Parameters.Add("@Sucursal", SqlDbType.NVarChar, 2);
            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@lrc_codCliente", SqlDbType.Int);
            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@lcp_codUsuario", SqlDbType.Int);
            SqlParameter paTabla_Detalle = cmdComandoInicio.Parameters.Add("@Tabla_Detalle", SqlDbType.Structured);
            SqlParameter paTipoDeArchivo = cmdComandoInicio.Parameters.Add("@TipoDeArchivo", SqlDbType.NVarChar, 1);
            SqlParameter paCli_codprov = cmdComandoInicio.Parameters.Add("@cli_codprov", SqlDbType.NVarChar, 75);
            SqlParameter paCli_isGLN = cmdComandoInicio.Parameters.Add("@cli_isGLN", SqlDbType.Bit);

            if (pIdUsuario == null)
            {
                paIdUsuario.Value = DBNull.Value;
            }
            else
            {
                paIdUsuario.Value = pIdUsuario;
            }
            paSucursalCliente.Value = pSucursalCliente;
            paSucursalElejida.Value = pSucursalElejida;
            paIdCliente.Value = pIdCliente;
            paTabla_Detalle.Value = pTabla;
            paTipoDeArchivo.Value = pTipoDeArchivo;
            paCli_codprov.Value = pCli_codprov;
            paCli_isGLN.Value = pCli_isGLN;

            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "ProductosBuscador");
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
        public static bool BorrarPorProductosFaltasProblemasCrediticiosV3(DataTable pTablaProducto, string fpc_codSucursal, int fpc_codCliente, int fpc_tipo, int pCantidadDia)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spBorrarPorProductosFaltasProblemasCrediticiosV3", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paTablaProductos = cmdComandoInicio.Parameters.Add("@Tabla_Detalle", SqlDbType.Structured);
            SqlParameter paCodSucursal = cmdComandoInicio.Parameters.Add("@fpc_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paCodCliente = cmdComandoInicio.Parameters.Add("@fpc_codCliente", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@fpc_tipo", SqlDbType.Int);
            //SqlParameter paFpc_nombreProducto = cmdComandoInicio.Parameters.Add("@fpc_nombreProducto", SqlDbType.NVarChar, 75);
            SqlParameter paCantidadDia = cmdComandoInicio.Parameters.Add("@cantidadDia", SqlDbType.Int);
            // SqlParameter paCantidadProductoGrabarNuevo = cmdComandoInicio.Parameters.Add("@cantidadProductoGrabarNuevo", SqlDbType.Int);

            if (pTablaProducto == null)
            {
                paTablaProductos.Value = DBNull.Value;
            }
            else
            {
                paTablaProductos.Value = pTablaProducto;
            }
            paCodSucursal.Value = fpc_codSucursal;
            paCodCliente.Value = fpc_codCliente;
            paTipo.Value = fpc_tipo;
            //paFpc_nombreProducto.Value = fpc_nombreProducto;
            paCantidadDia.Value = pCantidadDia;
            //paCantidadProductoGrabarNuevo.Value = pCantidadProductoGrabarNuevo;

            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                return false;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static DataSet GestiónFaltantesProblemasCrediticios(int? fpc_id, int? fpc_codCarrito, string fpc_codSucursal, int? fpc_codCliente, string fpc_nombreProducto, int? fpc_cantidad, int? fpc_tipo, DateTime? fpc_fecha, string accion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spGestionFaltasProblemasCrediticios", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paFpc_id = cmdComandoInicio.Parameters.Add("@fpc_id", SqlDbType.Int);
            SqlParameter paFpc_codCarrito = cmdComandoInicio.Parameters.Add("@fpc_codCarrito", SqlDbType.Int);
            SqlParameter paFpc_codSucursal = cmdComandoInicio.Parameters.Add("@fpc_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paFpc_codCliente = cmdComandoInicio.Parameters.Add("@fpc_codCliente", SqlDbType.Int);
            SqlParameter paFpc_nombreProducto = cmdComandoInicio.Parameters.Add("@fpc_nombreProducto", SqlDbType.NVarChar, 75);
            SqlParameter paFpc_cantidad = cmdComandoInicio.Parameters.Add("@fpc_cantidad", SqlDbType.Int);
            SqlParameter paFpc_tipo = cmdComandoInicio.Parameters.Add("@fpc_tipo", SqlDbType.Int);
            SqlParameter paFpc_fecha = cmdComandoInicio.Parameters.Add("@fpc_fecha", SqlDbType.DateTime);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 50);

            if (fpc_id == null)
            {
                paFpc_id.Value = DBNull.Value;
            }
            else
            {
                paFpc_id.Value = fpc_id;
            }
            if (fpc_codCarrito == null)
            {
                paFpc_codCarrito.Value = DBNull.Value;
            }
            else
            {
                paFpc_codCarrito.Value = fpc_codCarrito;
            }
            if (fpc_codSucursal == null)
            {
                paFpc_codSucursal.Value = DBNull.Value;
            }
            else
            {
                paFpc_codSucursal.Value = fpc_codSucursal;
            }
            if (fpc_codCliente == null)
            {
                paFpc_codCliente.Value = DBNull.Value;
            }
            else
            {
                paFpc_codCliente.Value = fpc_codCliente;
            }
            if (fpc_nombreProducto == null)
            {
                paFpc_nombreProducto.Value = DBNull.Value;
            }
            else
            {
                paFpc_nombreProducto.Value = fpc_nombreProducto;
            }

            if (fpc_cantidad == null)
            {
                paFpc_cantidad.Value = DBNull.Value;
            }
            else
            {
                paFpc_cantidad.Value = fpc_cantidad;
            }
            if (fpc_tipo == null)
            {
                paFpc_tipo.Value = DBNull.Value;
            }
            else
            {
                paFpc_tipo.Value = fpc_tipo;
            }
            if (fpc_fecha == null)
            {
                paFpc_fecha.Value = DBNull.Value;
            }
            else
            {
                paFpc_fecha.Value = fpc_fecha;
            }
            paAccion.Value = accion;
            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "FaltantesProblemasCrediticios");
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
        public static int InsertarFaltantesProblemasCrediticios(int? fpc_codCarrito, string fpc_codSucursal, int fpc_codCliente, string fpc_nombreProducto, int fpc_cantidad, int fpc_tipo)
        {
            DataSet dsResultado = capaLogRegistro_base.GestiónFaltantesProblemasCrediticios(null, fpc_codCarrito, fpc_codSucursal, fpc_codCliente, fpc_nombreProducto, fpc_cantidad, fpc_tipo, null, Constantes.cSQL_INSERT);
            int resultado = -1;
            if (dsResultado != null)
            {
                if (dsResultado.Tables["FaltantesProblemasCrediticios"].Rows[0]["fpc_id"] != DBNull.Value)
                {
                    resultado = Convert.ToInt32(dsResultado.Tables["FaltantesProblemasCrediticios"].Rows[0]["fpc_id"]);
                }
            }
            return resultado;
        }
        public static int IngresarPalabraBusqueda(int? pIdUsuario, string pNombreTabla, string pPalabra)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spIngresarPalabraBusqueda", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@idUsuario", SqlDbType.Int);
            SqlParameter paNombreTabla = cmdComandoInicio.Parameters.Add("@nombreTabla", SqlDbType.NVarChar, 75);
            SqlParameter paPalabra = cmdComandoInicio.Parameters.Add("@hbp_Palabra", SqlDbType.NVarChar, 200);


            if (pIdUsuario == null)
            {
                paIdUsuario.Value = DBNull.Value;
            }
            else
            {
                paIdUsuario.Value = pIdUsuario;
            }
            if (pNombreTabla == null)
            {
                paNombreTabla.Value = DBNull.Value;
            }
            else
            {
                paNombreTabla.Value = pNombreTabla;
            }
            if (pPalabra == null)
            {
                paPalabra.Value = DBNull.Value;
            }
            else
            {
                paPalabra.Value = pPalabra;
            }
            try
            {
                Conn.Open();
                object objResultado = cmdComandoInicio.ExecuteScalar();
                return Convert.ToInt32(objResultado);
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
        public static void AgregarProductosBuscadosDelCarrito(int pIdCliente, string pIdProducto, int? pIdUsuario)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spAgregarProductosBuscadosDelCarrito", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@hpb_codUsuario", SqlDbType.Int);
            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@hpb_codCliente", SqlDbType.Int);
            SqlParameter paIdProducto = cmdComandoInicio.Parameters.Add("@hpb_codProducto", SqlDbType.NVarChar, 75);

            if (pIdUsuario == null)
            {
                paIdUsuario.Value = DBNull.Value;
            }
            else
            {
                paIdUsuario.Value = pIdUsuario;
            }
            paIdCliente.Value = pIdCliente;

            if (paIdProducto == null)
            {
                paIdProducto.Value = DBNull.Value;
            }
            else
            {
                paIdProducto.Value = pIdProducto;
            }
            try
            {
                Conn.Open();
                object objResultado = cmdComandoInicio.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static void AgregarProductosBuscadosDelCarritoTransfer(int pIdCliente, DataTable pTablaProducto, int? pIdUsuario)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spAgregarProductosBuscadosDelCarritoTransferPorNombreProducto", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdUsuario = cmdComandoInicio.Parameters.Add("@hpb_codUsuario", SqlDbType.Int);
            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@hpb_codCliente", SqlDbType.Int);
            SqlParameter paTablaProductos = cmdComandoInicio.Parameters.Add("@Tabla_Detalle", SqlDbType.Structured);

            if (pIdUsuario == null)
            {
                paIdUsuario.Value = DBNull.Value;
            }
            else
            {
                paIdUsuario.Value = pIdUsuario;
            }
            paIdCliente.Value = pIdCliente;

            if (pTablaProducto == null)
            {
                paTablaProductos.Value = DBNull.Value;
            }
            else
            {
                paTablaProductos.Value = pTablaProducto;
            }
            try
            {
                Conn.Open();
                object objResultado = cmdComandoInicio.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static DataTable RecuperarHistorialSubirArchivo(int pIdCliente)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spRecuperarHistorialSubirArchivo", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paIdCliente = cmdComandoInicio.Parameters.Add("@has_codCliente", SqlDbType.Int);
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
        public static cHistorialArchivoSubir ConvertToHistorialArchivoSubir(DataRow pFila)
        {
            cHistorialArchivoSubir obj = null;

            if (pFila != null)
            {
                obj = new cHistorialArchivoSubir();
                if (pFila["has_id"] != DBNull.Value)
                {
                    obj.has_id = Convert.ToInt32(pFila["has_id"]);
                }
                if (pFila["has_NombreArchivo"] != DBNull.Value)
                {
                    obj.has_NombreArchivo = pFila["has_NombreArchivo"].ToString();
                }
                if (pFila["has_NombreArchivoOriginal"] != DBNull.Value)
                {
                    obj.has_NombreArchivoOriginal = pFila["has_NombreArchivoOriginal"].ToString();
                }
                if (pFila["has_sucursal"] != DBNull.Value)
                {
                    obj.has_sucursal = pFila["has_sucursal"].ToString();
                }
                if (pFila.Table.Columns.Contains("suc_nombre"))
                {
                    if (pFila["suc_nombre"] != DBNull.Value)
                    {
                        obj.suc_nombre = pFila["suc_nombre"].ToString();
                    }
                }
                if (pFila["has_codCliente"] != DBNull.Value)
                {
                    obj.has_codCliente = Convert.ToInt32(pFila["has_codCliente"]);
                }
                if (pFila["has_fecha"] != DBNull.Value)
                {
                    obj.has_fecha = Convert.ToDateTime(pFila["has_fecha"]);
                }
                if (pFila["has_fecha"] != DBNull.Value)
                {
                    obj.has_fechaToString = Convert.ToDateTime(pFila["has_fecha"]).ToString();
                }
            }

            return obj;
        }
        public static bool AgregarHistorialSubirArchivo(int has_codCliente, string has_NombreArchivo, string has_NombreArchivoOriginal, string has_sucursal, DateTime has_fecha)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spAgregarHistorialSubirArchivo", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paHas_NombreArchivo = cmdComandoInicio.Parameters.Add("@has_NombreArchivo", SqlDbType.NVarChar, 100);
            SqlParameter paHas_NombreArchivoOriginal = cmdComandoInicio.Parameters.Add("@has_NombreArchivoOriginal", SqlDbType.NVarChar, 100);
            SqlParameter paHas_codCliente = cmdComandoInicio.Parameters.Add("@has_codCliente", SqlDbType.Int);
            SqlParameter paHas_Sucursal = cmdComandoInicio.Parameters.Add("@has_sucursal", SqlDbType.NVarChar, 2);
            SqlParameter paHas_fecha = cmdComandoInicio.Parameters.Add("@has_fecha", SqlDbType.DateTime);

            paHas_NombreArchivo.Value = has_NombreArchivo;
            paHas_NombreArchivoOriginal.Value = has_NombreArchivoOriginal;
            paHas_codCliente.Value = has_codCliente;
            paHas_Sucursal.Value = has_sucursal;
            paHas_fecha.Value = has_fecha;

            try
            {
                Conn.Open();
                object objResultado = cmdComandoInicio.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                return false;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public static DataTable RecuperarHistorialSubirArchivoPorNombreArchivoOriginal(string pNombreArchivoOriginal)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spRecuperarHistorialSubirArchivoPorNombreArchivoOriginal", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paNombreArchivoOriginal = cmdComandoInicio.Parameters.Add("@has_NombreArchivoOriginal", SqlDbType.NVarChar, 100);
            paNombreArchivoOriginal.Value = pNombreArchivoOriginal;

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
        public static DataTable RecuperarHistorialSubirArchivoPorId(int pId)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spRecuperarHistorialSubirArchivoPorId", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paId = cmdComandoInicio.Parameters.Add("@has_id", SqlDbType.Int);
            paId.Value = pId;

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
        public static DataSet RecuperarFaltasProblemasCrediticios(int fpc_codCliente, int fpc_tipo, int pDia, string pSucursal)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spRecuperarFaltasProblemasCrediticiosV2", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paCodCliente = cmdComandoInicio.Parameters.Add("@fpc_codCliente", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@fpc_tipo", SqlDbType.Int);
            SqlParameter paCantidadDia = cmdComandoInicio.Parameters.Add("@cantidadDia", SqlDbType.Int);
            SqlParameter paSucursal = cmdComandoInicio.Parameters.Add("@Sucursal", SqlDbType.NVarChar, 2);

            paCantidadDia.Value = pDia;
            paCodCliente.Value = fpc_codCliente;
            paTipo.Value = fpc_tipo;
            paSucursal.Value = pSucursal;

            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "ProductosBuscador");
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
        public static DataSet RecuperarFaltasProblemasCrediticios_TodosEstados(int fpc_codCliente, int fpc_tipo, int pDia, string pSucursal)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spRecuperarFaltasProblemasCrediticiosTodosEstadosV2", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paCodCliente = cmdComandoInicio.Parameters.Add("@fpc_codCliente", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@fpc_tipo", SqlDbType.Int);
            SqlParameter paCantidadDia = cmdComandoInicio.Parameters.Add("@cantidadDia", SqlDbType.Int);
            SqlParameter paSucursal = cmdComandoInicio.Parameters.Add("@Sucursal", SqlDbType.NVarChar, 2);

            paCantidadDia.Value = pDia;
            paCodCliente.Value = fpc_codCliente;
            paTipo.Value = fpc_tipo;
            paSucursal.Value = pSucursal;
            try
            {
                Conn.Open();
                DataSet dsResultado = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmdComandoInicio);
                da.Fill(dsResultado, "ProductosBuscador");
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
        public static bool BorrarPorProductosFaltasProblemasCrediticios(string fpc_codSucursal, int fpc_codCliente, int fpc_tipo, string fpc_nombreProducto)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("LogRegistro.spBorrarPorProductosFaltasProblemasCrediticios", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paCodSucursal = cmdComandoInicio.Parameters.Add("@fpc_codSucursal", SqlDbType.NVarChar, 2);
            SqlParameter paCodCliente = cmdComandoInicio.Parameters.Add("@fpc_codCliente", SqlDbType.Int);
            SqlParameter paTipo = cmdComandoInicio.Parameters.Add("@fpc_tipo", SqlDbType.Int);
            SqlParameter paFpc_nombreProducto = cmdComandoInicio.Parameters.Add("@fpc_nombreProducto", SqlDbType.NVarChar, 75);

            paCodSucursal.Value = fpc_codSucursal;
            paCodCliente.Value = fpc_codCliente;
            paTipo.Value = fpc_tipo;
            paFpc_nombreProducto.Value = fpc_nombreProducto;
            try
            {
                Conn.Open();
                cmdComandoInicio.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
                return false;
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
