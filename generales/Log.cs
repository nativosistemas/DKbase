using DKbase.dll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DKbase.generales
{
    public class Log
    {
        public static void LogErrorFile(string pantalla, string mensaje)
        {
            DateTime now = DateTime.Now;
            string nombreFile = now.Year.ToString("0000") + now.Month.ToString("00") + now.Day.ToString("00") + Helper.getTipoApp + ".txt";
            Log.saveInFile("fecha: " + now.ToString() + " - pantalla: " + pantalla + " - mensaje :" + mensaje, nombreFile);
        }
        public static void LogError(MethodBase method, Exception pException, DateTime pFechaActual, params object[] values)
        {
            try
            {
                ParameterInfo[] parms = method.GetParameters();
                object[] namevalues = new object[2 * parms.Length];

                string Parameters = string.Empty;
                if (values.Length > 0)
                {
                    for (int i = 0, j = 0; i < parms.Length; i++, j += 2)
                    {
                        Parameters += "<" + parms[i].Name + ">";
                        if (values[i].GetType() == typeof(List<cDllProductosAndCantidad>))
                        {
                            List<cDllProductosAndCantidad> list = (List<cDllProductosAndCantidad>)values[i];
                            for (int y = 0; y < list.Count; y++)
                            {
                                Parameters += String.Format("codProductoNombre = {0} || cantidad = {1} || IdTransfer = {2} || isOferta = {3}", list[y].codProductoNombre, list[y].cantidad, list[y].IdTransfer, list[y].isOferta);
                            }
                        }
                        else
                        {
                            Parameters += values[i];
                        }
                        Parameters += "</" + parms[i].Name + ">";
                        Parameters += "</" + parms[i].Name + ">";
                    }
                }
                grabarLog_generico(method.Name, pException, pFechaActual, Parameters, Helper.getTipoApp);

            }
            catch (Exception ex)
            {
                LogErrorFile(MethodBase.GetCurrentMethod().ToString(), ex.ToString());
            }
        }
        public static void grabarLog_generico(string nombre, Exception pException, DateTime pFechaActual, string Parameters, string pTipo)
        {
            try
            {
                bool isNotGeneroError = baseDatos.StoredProcedure.spError(nombre, Parameters, pException.Data != null ? pException.Data.ToString() : null,
                     pException.HelpLink != null ? pException.HelpLink.ToString() : null,
                     pException.InnerException != null ? pException.InnerException.ToString() : null,
                     pException.Message != null ? pException.Message.ToString() : null,
                    pException.Source != null ? pException.Source.ToString() : null,
                   pException.StackTrace != null ? pException.StackTrace.ToString() : null, DateTime.Now, pTipo);
            }
            catch (Exception ex)
            {
                LogErrorFile(MethodBase.GetCurrentMethod().ToString(), ex.ToString());
            }
        }
        public static void saveInFile(string pMensaje, string pNombreArchivo)
        {
            try
            {
                string path = Helper.getPathSiteWeb + Helper.getFolderLog + "\\";//"";// HttpContext.Current.Server.MapPath(@"../" + Constantes.cArchivo_log + @"/");
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                string FilePath = path + pNombreArchivo;
                if (!File.Exists(FilePath))
                {
                    using (StreamWriter sw = File.CreateText(FilePath))
                    {
                        sw.WriteLine(pMensaje);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        sw.WriteLine(pMensaje);
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
