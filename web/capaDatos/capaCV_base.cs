using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cCurriculumVitae
    {
        public int tcv_codCV { get; set; }
        public string tcv_nombre { get; set; }
        public string tcv_comentario { get; set; }
        public string tcv_mail { get; set; }
        public string tcv_dni { get; set; }
        public DateTime tcv_fecha { get; set; }
        public string tcv_fechaToString { get; set; }
        public int tcv_estado { get; set; }
        public string tcv_estadoToString { get; set; }

        public string tcv_puesto { get; set; }
        public string tcv_sucursal { get; set; }
        public DateTime tcv_fechaPresentacion { get; set; }
        public string tcv_fechaPresentacionToString { get; set; }
        public string arc_nombre
        {
            get; set;
        }
    }
    public class capaCV_base
    {
        public static cCurriculumVitae ConvertToCurriculumVitae(DataRow pItem)
        {
            cCurriculumVitae obj = new cCurriculumVitae();

            if (pItem["tcv_codCV"] != DBNull.Value)
            {
                obj.tcv_codCV = Convert.ToInt32(pItem["tcv_codCV"]);
            }
            if (pItem["tcv_nombre"] != DBNull.Value)
            {
                obj.tcv_nombre = Convert.ToString(pItem["tcv_nombre"]);
            }
            if (pItem["tcv_comentario"] != DBNull.Value)
            {
                obj.tcv_comentario = Convert.ToString(pItem["tcv_comentario"]);
            }
            if (pItem["tcv_dni"] != DBNull.Value)
            {
                obj.tcv_dni = Convert.ToString(pItem["tcv_dni"]);
            }
            if (pItem["tcv_mail"] != DBNull.Value)
            {
                obj.tcv_mail = Convert.ToString(pItem["tcv_mail"]);
            }
            if (pItem["tcv_fecha"] != DBNull.Value)
            {
                obj.tcv_fecha = Convert.ToDateTime(pItem["tcv_fecha"]);
                obj.tcv_fechaToString = ((DateTime)obj.tcv_fecha).ToString();
            }
            if (pItem["tcv_estado"] != DBNull.Value)
            {
                obj.tcv_estado = Convert.ToInt32(pItem["tcv_estado"]);
            }
            if (pItem["est_nombre"] != DBNull.Value)
            {
                obj.tcv_estadoToString = Convert.ToString(pItem["est_nombre"]);
            }
            if (pItem["tcv_puesto"] != DBNull.Value)
            {
                obj.tcv_puesto = Convert.ToString(pItem["tcv_puesto"]);
            }
            if (pItem["tcv_sucursal"] != DBNull.Value)
            {
                obj.tcv_sucursal = Convert.ToString(pItem["tcv_sucursal"]);
            }
            if (pItem["tcv_fechaPresentacion"] != DBNull.Value)
            {
                obj.tcv_fechaPresentacion = Convert.ToDateTime(pItem["tcv_fechaPresentacion"]);
                obj.tcv_fechaPresentacionToString = ((DateTime)obj.tcv_fechaPresentacion).ToString();
            }
            List<cArchivo> listaArchivo = DKbase.Util.RecuperarTodosArchivos(obj.tcv_codCV, Constantes.cTABLA_CV, string.Empty);
            if (listaArchivo != null)
            {
                if (listaArchivo.Count > 0)
                {
                    obj.arc_nombre = listaArchivo[0].arc_nombre;
                }
            }
            return obj;
        }
        public static DataSet GestiónCurriculumVitae(int? tcv_codCV, DateTime? tcv_fecha, string tcv_nombre, string tcv_comentario, string tcv_mail, string tcv_dni, int? tcv_estado, string filtro, string accion, string tcv_puesto, string tcv_sucursal, DateTime? tcv_fechaPresentacion)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("spCurriculumVitae", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;

            SqlParameter paTcv_codCV = cmdComandoInicio.Parameters.Add("@tcv_codCV", SqlDbType.Int);
            SqlParameter paTcv_fecha = cmdComandoInicio.Parameters.Add("@tcv_fecha", SqlDbType.DateTime);
            SqlParameter paTcv_nombre = cmdComandoInicio.Parameters.Add("@tcv_nombre", SqlDbType.NVarChar, 500);
            SqlParameter paTcv_comentario = cmdComandoInicio.Parameters.Add("@tcv_comentario", SqlDbType.NVarChar, 500);
            SqlParameter paTcv_mail = cmdComandoInicio.Parameters.Add("@tcv_mail", SqlDbType.NVarChar, 500);
            SqlParameter paTcv_dni = cmdComandoInicio.Parameters.Add("@tcv_dni", SqlDbType.NVarChar, 50);
            SqlParameter paTcv_estado = cmdComandoInicio.Parameters.Add("@tcv_estado", SqlDbType.Int);
            SqlParameter paFiltro = cmdComandoInicio.Parameters.Add("@filtro", SqlDbType.NVarChar, 50);
            SqlParameter paAccion = cmdComandoInicio.Parameters.Add("@accion", SqlDbType.NVarChar, 50);

            SqlParameter paTcv_puesto = cmdComandoInicio.Parameters.Add("@tcv_puesto", SqlDbType.NVarChar, 500);
            SqlParameter paTcv_sucursal = cmdComandoInicio.Parameters.Add("@tcv_sucursal", SqlDbType.NVarChar, 500);
            SqlParameter paTcv_fechaPresentacion = cmdComandoInicio.Parameters.Add("@tcv_fechaPresentacion", SqlDbType.DateTime);

            if (tcv_puesto == null)
            {
                paTcv_puesto.Value = DBNull.Value;
            }
            else
            {
                paTcv_puesto.Value = tcv_puesto;
            }
            if (tcv_sucursal == null)
            {
                paTcv_sucursal.Value = DBNull.Value;
            }
            else
            {
                paTcv_sucursal.Value = tcv_sucursal;
            }
            if (tcv_fechaPresentacion == null)
            {
                paTcv_fechaPresentacion.Value = DBNull.Value;
            }
            else
            {
                paTcv_fechaPresentacion.Value = tcv_fechaPresentacion;
            }
            ////


            if (tcv_codCV == null)
            {
                paTcv_codCV.Value = DBNull.Value;
            }
            else
            {
                paTcv_codCV.Value = tcv_codCV;
            }
            if (tcv_fecha == null)
            {
                paTcv_fecha.Value = DBNull.Value;
            }
            else
            {
                paTcv_fecha.Value = tcv_fecha;
            }
            if (tcv_nombre == null)
            {
                paTcv_nombre.Value = DBNull.Value;
            }
            else
            {
                paTcv_nombre.Value = tcv_nombre;
            }
            if (tcv_comentario == null)
            {
                paTcv_comentario.Value = DBNull.Value;
            }
            else
            {
                paTcv_comentario.Value = tcv_comentario;
            }
            if (tcv_mail == null)
            {
                paTcv_mail.Value = DBNull.Value;
            }
            else
            {
                paTcv_mail.Value = tcv_mail;
            }
            if (tcv_dni == null)
            {
                paTcv_dni.Value = DBNull.Value;
            }
            else
            {
                paTcv_dni.Value = tcv_dni;
            }

            if (tcv_estado == null)
            {
                paTcv_estado.Value = DBNull.Value;
            }
            else
            {
                paTcv_estado.Value = tcv_estado;
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
                da.Fill(dsResultado, "CurriculumVitae");
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
        public static string GenerarHtmlCuerpoMail(string tcv_nombre, string tcv_comentario, string tcv_mail)
        {
            string resultado = string.Empty;
            resultado += "<table>";
            resultado += "<tr>";
            resultado += "<td>";
            resultado += "Nombre:";
            resultado += "</td>";
            resultado += "<td>";
            resultado += tcv_nombre;
            resultado += "</td>";
            resultado += "</tr>";
            resultado += "<tr>";
            resultado += "<td>";
            resultado += "Mail:";
            resultado += "</td>";
            resultado += "<td>";
            resultado += tcv_mail;
            resultado += "</td>";
            resultado += "</tr>";
            resultado += "<tr>";
            resultado += "<td>";
            resultado += "Comentario:";
            resultado += "</td>";
            resultado += "<td>";
            resultado += tcv_comentario;
            resultado += "</td>";
            resultado += "</tr>";
            resultado += "</table>";
            return resultado;
        }
    }
}
