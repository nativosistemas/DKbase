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
        private static string getParameters(MethodBase method, params object[] values)
        {
            string Parameters = string.Empty;
            try
            {
               // ParameterInfo[] parms = method.GetParameters();
                //object[] namevalues = new object[2 * parms.Length];
                if (values != null && values.Length > 0)
                {
                    for (int i = 0, j = 0; i < values.Length; i++, j += 2)
                    {
                        Type typeParam = values[i].GetType();
                        string nameType = typeParam.ToString();
                        Parameters += "<" + "" + ">";//nameType
                        if (values[i] != null && typeParam == typeof(List<cDllProductosAndCantidad>))
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
                        Parameters += "</" + "" + ">"; //nameType
                    }
                }
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.grabarLog_generico(MethodBase.GetCurrentMethod().Name, ex, DateTime.Now, Parameters, Helper.getTipoApp);
            }
            return Parameters;
        }
        public static void LogError(MethodBase method, string pMensaje, DateTime pFechaActual, params object[] values)
        {
            try
            {
                //System.Console.WriteLine(pMensaje);
                string Parameters = getParameters(method, values);
                bool isNotGeneroError = baseDatos.StoredProcedure.spError(method.Name, Parameters, null,
                      null,
                     null,
                    pMensaje,
                   null,
                   null, DateTime.Now, Helper.getTipoApp);
            }
            catch (Exception ex)
            {
                LogErrorFile(method.Name, pMensaje);
                LogErrorFile(MethodBase.GetCurrentMethod().ToString(), ex.ToString());
            }
        }
        public static void LogError(MethodBase method, Exception pException, DateTime pFechaActual, params object[] values)
        {
            try
            {
                System.Console.WriteLine(pException);
                string Parameters = getParameters(method, values);
                grabarLog_generico(method.Name, pException, pFechaActual, Parameters, Helper.getTipoApp);

            }
            catch (Exception ex)
            {
                LogErrorFile(pException.ToString(), pException.Message);
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
                System.Console.WriteLine(pException);
                LogErrorFile(MethodBase.GetCurrentMethod().ToString(), ex.ToString());
            }
        }
        public static void saveInFile(string pMensaje, string pNombreArchivo)
        {
            try
            {
                string path = Path.Combine(Helper.getFolder, "log");
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                string FilePath = Path.Combine(path, pNombreArchivo);
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
                System.Console.WriteLine(ex);
            }
        }
        public static void LogInfo(MethodBase method, string pMensaje, string pInfoAdicional, string pType, string pFile_type, byte[] pFile_content, params object[] values)
        {
            try
            {
                System.Console.WriteLine(pMensaje);
                string Parameters = getParameters(method, values);
                string method_Name = method.DeclaringType.AssemblyQualifiedName;
                bool isNotGeneroError = baseDatos.StoredProcedure.spLogInfo(method_Name, pMensaje, pInfoAdicional, Parameters, DateTime.Now, Helper.getTipoApp, pType, pFile_type, pFile_content);
            }
            catch (Exception ex)
            {
                Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, values);
            }
        }
    }
}
