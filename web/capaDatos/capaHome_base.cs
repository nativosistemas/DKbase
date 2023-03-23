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
    public  class cRecall
    {
        public int rec_id { get; set; }
        public string rec_titulo { get; set; }
        public string rec_descripcion { get; set; }
        public string rec_descripcionReducido { get; set; }
        public string rec_descripcionHTML { get; set; }
        public Nullable<System.DateTime> rec_FechaNoticia { get; set; }
        public Nullable<System.DateTime> rec_FechaFinNoticia { get; set; }
        public Nullable<System.DateTime> rec_Fecha { get; set; }
        public Nullable<bool> rec_visible { get; set; }
        public string rec_FechaNoticiaToString
        {

            get { return this.rec_FechaNoticia == null ? string.Empty : this.rec_FechaNoticia.Value.ToShortDateString(); }
            set { }
        }
        public string descripcionReducidoMostrar
        {

            get { return this.rec_descripcionReducido == null ? string.Empty : generales.Texto_base.CortarBajada(this.rec_descripcionReducido, 120); }
            set { }
        }
        public string descripcionReducidoMostrarMediano
        {

            get { return this.rec_descripcionReducido == null ? string.Empty : generales.Texto_base.CortarBajada(this.rec_descripcionReducido, 380); }
            set { }
        }
        private string _arc_nombre = null;
        public string arc_nombre
        {
            get { return _arc_nombre; }
            set { _arc_nombre = value; }
        }
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
        public static cOfertaHome ConvertAddOferta(DataRow pItem, cOfertaHome obj)
        {

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
        public static cOfertaHome ConvertTocOfertaHome(DataRow pItem)
        {
            cOfertaHome obj = new cOfertaHome();

            if (pItem["ofh_idOfertaHome"] != DBNull.Value)
            {
                obj.ofh_idOfertaHome = Convert.ToInt32(pItem["ofh_idOfertaHome"]);
            }
            if (pItem["ofh_orden"] != DBNull.Value)
            {
                obj.ofh_orden = Convert.ToInt32(pItem["ofh_orden"]);
            }
            if (pItem["ofh_idOferta"] != DBNull.Value)
            {
                obj.ofh_idOferta = Convert.ToInt32(pItem["ofh_idOferta"]);
            }

            return obj;
        }
        public static cRecall ConvertToRecall(DataRow item) {
            cRecall o = new cRecall();
            if (item.Table.Columns.Contains("rec_id") && item["rec_id"] != DBNull.Value)
            {
                o.rec_id = Convert.ToInt32(item["rec_id"]);
            }
            if (item.Table.Columns.Contains("rec_titulo") && item["rec_titulo"] != DBNull.Value)
            {
                o.rec_titulo = Convert.ToString(item["rec_titulo"]);
            }
            if (item.Table.Columns.Contains("rec_descripcion") && item["rec_descripcion"] != DBNull.Value)
            {
                o.rec_descripcion = Convert.ToString(item["rec_descripcion"]);
            }
            if (item.Table.Columns.Contains("rec_descripcionReducido") && item["rec_descripcionReducido"] != DBNull.Value)
            {
                o.rec_descripcionReducido = Convert.ToString(item["rec_descripcionReducido"]);
            }
            if (item.Table.Columns.Contains("rec_descripcionHTML") && item["rec_descripcionHTML"] != DBNull.Value)
            {
                o.rec_descripcionHTML = Convert.ToString(item["rec_descripcionHTML"]);
            }
            if (item.Table.Columns.Contains("rec_FechaNoticia") && item["rec_FechaNoticia"] != DBNull.Value)
            {
                o.rec_FechaNoticia = Convert.ToDateTime(item["rec_FechaNoticia"]);
            }
            if (item.Table.Columns.Contains("rec_FechaFinNoticia") && item["rec_FechaFinNoticia"] != DBNull.Value)
            {
                o.rec_FechaFinNoticia = Convert.ToDateTime(item["rec_FechaFinNoticia"]);
            }
            if (item.Table.Columns.Contains("rec_Fecha") && item["rec_Fecha"] != DBNull.Value)
            {
                o.rec_Fecha = Convert.ToDateTime(item["rec_Fecha"]);
            }
            if (item.Table.Columns.Contains("rec_visible") && item["rec_visible"] != DBNull.Value)
            {
                o.rec_visible = Convert.ToBoolean(item["rec_visible"]);
            }
            return o;
        }
        public static cHomeSlide ConvertToHomeSlide(DataRow pItem)
        {
            cHomeSlide obj = new cHomeSlide();

            if (pItem["hsl_idHomeSlide"] != DBNull.Value)
            {
                obj.hsl_idHomeSlide = Convert.ToInt32(pItem["hsl_idHomeSlide"]);
            }
            if (pItem["hsl_titulo"] != DBNull.Value)
            {
                obj.hsl_titulo = Convert.ToString(pItem["hsl_titulo"]);
            }
            if (pItem["hsl_descr"] != DBNull.Value)
            {
                obj.hsl_descr = Convert.ToString(pItem["hsl_descr"]);
            }
            if (pItem["hsl_descrReducido"] != DBNull.Value)
            {
                obj.hsl_descrReducido = Convert.ToString(pItem["hsl_descrReducido"]);
            }
            if (pItem["hsl_descrHtml"] != DBNull.Value)
            {
                obj.hsl_descrHtml = Convert.ToString(pItem["hsl_descrHtml"]);
            }
            if (pItem["hsl_descrHtmlReducido"] != DBNull.Value)
            {
                obj.hsl_descrHtmlReducido = Convert.ToString(pItem["hsl_descrHtmlReducido"]);
            }
            if (pItem["hsl_tipo"] != DBNull.Value)
            {
                obj.hsl_tipo = Convert.ToInt32(pItem["hsl_tipo"]);
            }
            obj.tipoRecurso = "slider";
            if (pItem["hsl_idRecursoDoc"] != DBNull.Value)
            {
                obj.hsl_idRecursoDoc = Convert.ToInt32(pItem["hsl_idRecursoDoc"]);
            }
            if (pItem["hsl_NombreRecursoDoc"] != DBNull.Value)
            {
                obj.hsl_NombreRecursoDoc = Convert.ToString(pItem["hsl_NombreRecursoDoc"]);
            }
            if (pItem["hsl_idRecursoImgPC"] != DBNull.Value)
            {
                obj.hsl_idRecursoImgPC = Convert.ToInt32(pItem["hsl_idRecursoImgPC"]);
            }
            if (pItem["arc_nombrePC"] != DBNull.Value)
            {
                obj.arc_nombrePC = Convert.ToString(pItem["arc_nombrePC"]);
            }
            if (pItem["hsl_idRecursoImgMobil"] != DBNull.Value)
            {
                obj.hsl_idRecursoImgMobil = Convert.ToInt32(pItem["hsl_idRecursoImgMobil"]);
            }
            if (pItem["arc_nombreMobil"] != DBNull.Value)
            {
                obj.arc_nombreMobil = Convert.ToString(pItem["arc_nombreMobil"]);
            }
            if (pItem["hsl_idOferta"] != DBNull.Value)
            {
                obj.hsl_idOferta = Convert.ToInt32(pItem["hsl_idOferta"]);
            }
            if (pItem["hsl_etiqueta"] != DBNull.Value)
            {
                obj.hsl_etiqueta = Convert.ToString(pItem["hsl_etiqueta"]);
            }
            if (pItem["hsl_publicar"] != DBNull.Value)
            {
                obj.hsl_publicar = Convert.ToBoolean(pItem["hsl_publicar"]);
            }
            if (pItem["hsl_activo"] != DBNull.Value)
            {
                obj.hsl_activo = Convert.ToBoolean(pItem["hsl_activo"]);
            }
            if (pItem["hsl_fecha"] != DBNull.Value)
            {
                obj.hsl_fecha = Convert.ToDateTime(pItem["hsl_fecha"]);
                obj.hsl_fechaToString = obj.hsl_fecha.ToString();
            }
            if (pItem.Table.Columns.Contains("hsl_orden") && pItem["hsl_orden"] != DBNull.Value)
            {
                obj.hsl_orden = Convert.ToInt32(pItem["hsl_orden"]);
            }
            return obj;
        }
        public static DataTable RecuperarTodasHomeSlidePublicar()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("spRecuperarTodasHomeSlide", Conn);
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
        public static DataTable RecuperarTodasOfertas()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("spRecuperarTodasOferta", Conn);
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
        public static DataTable RecuperarOfertaPorId(int pIdOferta)
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("spRecuperarOfertaPorId", Conn);
            cmdComandoInicio.CommandType = CommandType.StoredProcedure;
            SqlParameter paIdOferta = cmdComandoInicio.Parameters.Add("@ofe_idOferta", SqlDbType.Int);
            paIdOferta.Value = pIdOferta;

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
        public static DataTable RecuperarTodasOfertaParaHome()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("spRecuperarTodasOfertaParaHome", Conn);
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
        public static DataTable RecuperarTodaReCall_aux()
        {
            SqlConnection Conn = new SqlConnection(Helper.getConnectionStringSQL);
            SqlCommand cmdComandoInicio = new SqlCommand("spGetRecall", Conn);
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
        public static DataTable spOferta_Rating(int pIdCliente, int pIdOferta ,bool pIsDesdeHome)
        {
            BaseDataAccess db = new BaseDataAccess(Helper.getConnectionStringSQL);
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(db.GetParameter("@ofr_idCliente", pIdCliente));
            l.Add(db.GetParameter("@ofr_idOferta", pIdOferta));
            l.Add(db.GetParameter("@ofr_isDesdeHome", pIsDesdeHome));
            return db.GetDataTable("dbo.spOferta_Rating", l);
        }

    }
}
