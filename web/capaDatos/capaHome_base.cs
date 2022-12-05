using DKbase.generales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DKbase.web.capaDatos
{
    public class cOferta
    {
        public int ofe_idOferta { get; set; }
        public string ofe_titulo { get; set; }
        public string ofe_descr { get; set; }
        public int ofe_tipo { get; set; }
        public string ofe_nombreTransfer { get; set; }
        public int? tfr_codigo { get; set; }
        public string ofe_descuento { get; set; }
        public string ofe_etiqueta { get; set; }
        public string ofe_etiquetaColor { get; set; }
        public bool ofe_publicar { get; set; }
        public bool ofe_activo { get; set; }
        public DateTime ofe_fecha { get; set; }
        public string ofe_fechaToString { get; set; }
        public string nameImagen { get; set; }
        public string nameImagenAmpliar { get; set; }
        public string namePdf { get; set; }
        public int countOfertaDetalles { get; set; }
        public int Rating { get; set; }
        public DateTime? ofe_fechaFinOferta { get; set; }
        public string ofe_fechaFinOfertaToString { get; set; }
        public bool ofe_nuevosLanzamiento { get; set; }
        public string ofe_descrHtml { get; set; }

    }

    public class cOfertaDetalle
    {
        public int ofd_idOfertaDetalle { get; set; }
        public int ofd_idOferta { get; set; }
        public string ofd_productoCodigo { get; set; }
        public string ofd_productoNombre { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
    }
    public class cOfertaHome : cOferta
    {
        public int ofh_idOfertaHome { get; set; }
        public int ofh_orden { get; set; }
        public int ofh_idOferta { get; set; }
    }
    public class cHomeSlide
    {
        public int hsl_idHomeSlide { get; set; }
        public string hsl_titulo { get; set; }
        public string hsl_descr { get; set; }
        public string hsl_descrReducido { get; set; }
        public string hsl_descrHtml { get; set; }
        public string hsl_descrHtmlReducido { get; set; }
        public int hsl_tipo { get; set; }
        public string tipoRecurso { get; set; }
        public int hsl_idRecursoDoc { get; set; }
        public string hsl_NombreRecursoDoc { get; set; }
        public int hsl_idRecursoImgPC { get; set; }
        public string arc_nombrePC { get; set; }
        public int hsl_idRecursoImgMobil { get; set; }
        public string arc_nombreMobil { get; set; }
        public int hsl_idOferta { get; set; }
        public string hsl_etiqueta { get; set; }
        public bool hsl_publicar { get; set; }
        public bool hsl_activo { get; set; }
        public DateTime hsl_fecha { get; set; }
        public string hsl_fechaToString { get; set; }
        public int? hsl_orden { get; set; }
    }
    public class capaHome_base
    {
        public static cOferta ConvertToOferta(DataRow pItem)
        {
            cOferta obj = new cOferta();

            if (pItem["ofe_idOferta"] != DBNull.Value)
            {
                obj.ofe_idOferta = Convert.ToInt32(pItem["ofe_idOferta"]);
            }
            if (pItem["ofe_titulo"] != DBNull.Value)
            {
                obj.ofe_titulo = Convert.ToString(pItem["ofe_titulo"]);
            }
            if (pItem["ofe_descuento"] != DBNull.Value)
            {
                obj.ofe_descuento = Convert.ToString(pItem["ofe_descuento"]);
            }
            if (pItem["ofe_tipo"] != DBNull.Value)
            {
                obj.ofe_tipo = Convert.ToInt32(pItem["ofe_tipo"]);
            }
            if (pItem["ofe_descr"] != DBNull.Value)
            {
                obj.ofe_descr = Convert.ToString(pItem["ofe_descr"]);
            }
            if (pItem["ofe_publicar"] != DBNull.Value)
            {
                obj.ofe_publicar = Convert.ToBoolean(pItem["ofe_publicar"]);
            }
            if (pItem["ofe_activo"] != DBNull.Value)
            {
                obj.ofe_activo = Convert.ToBoolean(pItem["ofe_activo"]);
            }
            if (pItem["ofe_fecha"] != DBNull.Value)
            {
                obj.ofe_fecha = Convert.ToDateTime(pItem["ofe_fecha"]);
                obj.ofe_fechaToString = obj.ofe_fecha.ToString();
            }
            if (pItem["ofe_etiqueta"] != DBNull.Value)
            {
                obj.ofe_etiqueta = Convert.ToString(pItem["ofe_etiqueta"]);
            }
            if (pItem["ofe_etiquetaColor"] != DBNull.Value)
            {
                obj.ofe_etiquetaColor = Convert.ToString(pItem["ofe_etiquetaColor"]);
            }
            if (pItem.Table.Columns.Contains("countOfertaDetalles") && pItem["countOfertaDetalles"] != DBNull.Value)
                obj.countOfertaDetalles = Convert.ToInt32(pItem["countOfertaDetalles"]);
            if (pItem.Table.Columns.Contains("Rating") && pItem["Rating"] != DBNull.Value)
                obj.Rating = Convert.ToInt32(pItem["Rating"]);
            if (pItem.Table.Columns.Contains("ofe_nombreTransfer") && pItem["ofe_nombreTransfer"] != DBNull.Value)
                obj.ofe_nombreTransfer = Convert.ToString(pItem["ofe_nombreTransfer"]);
            if (pItem.Table.Columns.Contains("tfr_codigo") && pItem["tfr_codigo"] != DBNull.Value)
                obj.tfr_codigo = Convert.ToInt32(pItem["tfr_codigo"]);
            if (pItem.Table.Columns.Contains("nameImagen") && pItem["nameImagen"] != DBNull.Value)
                obj.nameImagen = Convert.ToString(pItem["nameImagen"]);
            if (pItem.Table.Columns.Contains("namePdf") && pItem["namePdf"] != DBNull.Value)
                obj.namePdf = Convert.ToString(pItem["namePdf"]);
            if (pItem.Table.Columns.Contains("nameImagenAmpliar") && pItem["nameImagenAmpliar"] != DBNull.Value)
                obj.nameImagenAmpliar = Convert.ToString(pItem["nameImagenAmpliar"]);
            if (pItem.Table.Columns.Contains("ofe_fechaFinOferta") && pItem["ofe_fechaFinOferta"] != DBNull.Value)
            {
                obj.ofe_fechaFinOferta = Convert.ToDateTime(pItem["ofe_fechaFinOferta"]);
                obj.ofe_fechaFinOfertaToString = obj.ofe_fechaFinOferta.Value.ToString("dd'/'MM'/'yyyy");
            }
            if (pItem.Table.Columns.Contains("ofe_nuevosLanzamiento") && pItem["ofe_nuevosLanzamiento"] != DBNull.Value)
                obj.ofe_nuevosLanzamiento = Convert.ToBoolean(pItem["ofe_nuevosLanzamiento"]);

            if (pItem.Table.Columns.Contains("ofe_descrHtml") && pItem["ofe_descrHtml"] != DBNull.Value)
                obj.ofe_descrHtml = Convert.ToString(pItem["ofe_descrHtml"]);

            return obj;
        }
        public static DataTable RecuperarTodasOfertaPublicar()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("spRecuperarTodasOfertaPublicar", Conn);
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
    }
}
