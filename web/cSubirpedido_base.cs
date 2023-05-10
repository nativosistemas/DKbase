﻿using DKbase.web.capaDatos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using OfficeOpenXml;
using System.Linq;

namespace DKbase.web
{
    public class cSubirpedido_base
    {
        const int longFilaArchivoS = 22;
        const int longFilaArchivoF = 24;

        public static cSubirPedido_return LeerArchivoPedido(Usuario oUsuario, cClientes oCliente, IFormFile pFileUpload, string pSucursal)
        {
            cSubirPedido_return resultado = null;
            if (oUsuario != null && oCliente != null)
            {
                string nombreCompletoOriginal = pFileUpload.FileName;
                if (nombreCompletoOriginal != string.Empty)
                {
                    //isRedireccionar = true;
                    string rutaTemporal = Path.Combine(Helper.getFolder, "archivos", "ArchivosPedidos");
                    DirectoryInfo DIR = new DirectoryInfo(rutaTemporal);
                    if (!DIR.Exists)
                    {
                        DIR.Create();
                    }
                    string[] listaNombre = pFileUpload.FileName.Split('.');
                    string NombreArchivo = string.Empty;
                    string ExtencionArchivo = string.Empty;
                    for (int i = 0; i < listaNombre.Length - 1; i++)
                    {
                        NombreArchivo += listaNombre[i];
                    }
                    ExtencionArchivo = listaNombre[listaNombre.Length - 1];
                    int count = 0;
                    string SegundaParteNombre = string.Empty;
                    //
                    bool isNombreRepetido = false;
                    List<cHistorialArchivoSubir> listaHistorialArchivoSubir = DKbase.Util.RecuperarHistorialSubirArchivoPorNombreArchivoOriginal(nombreCompletoOriginal);
                    if (listaHistorialArchivoSubir != null)
                    {
                        if (listaHistorialArchivoSubir.Count > 0)
                        {
                            isNombreRepetido = true;
                        }
                    }
                    while (System.IO.File.Exists(Path.Combine(rutaTemporal, NombreArchivo + SegundaParteNombre + "." + ExtencionArchivo)))
                    {
                        if (count > 0)
                        {
                            SegundaParteNombre = "_" + count.ToString();
                        }
                        count++;
                    }
                    string nombreCompleto = NombreArchivo + SegundaParteNombre + "." + ExtencionArchivo;
                    //pFileUpload.SaveAs(rutaTemporal + nombreCompleto);

                    //string filePath = Path.Combine(uploads, file.FileName);
                    using (Stream fileStream = new FileStream(Path.Combine(rutaTemporal, nombreCompleto), FileMode.Create))
                    {
                        pFileUpload.CopyToAsync(fileStream);
                    }

                    if (ExtencionArchivo.ToUpper() == "XLSX" || ExtencionArchivo.ToUpper() == "XLS")
                    {
                        resultado = LeerArchivoPedido_Excel(oUsuario, oCliente, nombreCompleto, pSucursal, nombreCompletoOriginal, isNombreRepetido);
                    } else {
                        resultado = LeerArchivoPedido_Generica(oUsuario, oCliente, nombreCompleto, pSucursal, nombreCompletoOriginal, isNombreRepetido);
                    }
                }
            }
            return resultado;
        }


        private static cSubirPedido_return LeerArchivoPedido_Generica(Usuario oUsuario, cClientes oCliente, string pNombreArchivo, string pSucursal, string pNombreArchivoOriginal, Boolean? pIsNombreRepetido)
        {
            cSubirPedido_return resultado = null;
            if (!string.IsNullOrWhiteSpace(pNombreArchivo))
            {
                string rutaTemporal = Path.Combine(Helper.getFolder, "archivos", "ArchivosPedidos");
                string rutaTemporalAndNombreArchivo = Path.Combine(rutaTemporal, pNombreArchivo);
                if (System.IO.File.Exists(rutaTemporalAndNombreArchivo))
                {
                    //resultado = true;
                    string sLine = string.Empty;
                    StreamReader objReader = new StreamReader(rutaTemporalAndNombreArchivo);
                    string ext = Path.GetExtension(rutaTemporalAndNombreArchivo);
                    if (ext == null)
                        ext = string.Empty;
                    ext = ext.Replace(".", "").ToUpper();
                    bool isArchivoErroneo = false;
                    string TipoArchivo = string.Empty;
                    if (pNombreArchivo.Length > 0)
                    {
                        TipoArchivo = pNombreArchivo[0].ToString();
                    }
                    if (ext == "TXT" || ext == "ASC")
                    {
                        TipoArchivo = "S";
                    }
                    DataTable tablaArchivoPedidos = FuncionesPersonalizadas_base.ObtenerDataTableProductosCarritoArchivosPedidos();

                    while (sLine != null)
                    {
                        sLine = objReader.ReadLine();
                        if (!string.IsNullOrEmpty(sLine))
                        {
                            if (ext == "TXT")
                            {
                                DataRow r = LeerRenglonArchivoTXT(tablaArchivoPedidos, sLine);
                                if (r != null)
                                    tablaArchivoPedidos.Rows.Add(r);
                            }
                            else if (ext == "ASC")
                            {
                                DataRow r = LeerRenglonArchivoASC(tablaArchivoPedidos, sLine);
                                if (r != null)
                                    tablaArchivoPedidos.Rows.Add(r);
                            }
                            else
                            {
                                // Leer Archivo
                                switch (TipoArchivo)
                                {
                                    case "S":
                                        if (sLine.Length >= longFilaArchivoS) //22
                                        {
                                            DataRow r = LeerRenglonArchivoS(tablaArchivoPedidos, sLine);
                                            if (r != null)
                                                tablaArchivoPedidos.Rows.Add(r);
                                        }
                                        break;
                                    case "F":
                                        if (sLine.Length >= longFilaArchivoF)//24
                                        {
                                            DataRow r = LeerRenglonArchivoF(tablaArchivoPedidos, sLine);
                                            if (r != null)
                                                tablaArchivoPedidos.Rows.Add(r);
                                        }
                                        break;
                                    default:
                                        isArchivoErroneo = true;
                                        break;
                                }
                            }
                        }
                        if (isArchivoErroneo)
                        {
                            resultado = null;
                            break;
                        }
                    }
                    objReader.Close();
                    if (!isArchivoErroneo)
                    {
                        string sucElegida = pSucursal;
                        cSubirPedido_return o = new cSubirPedido_return();
                        o.SucursalEleginda = sucElegida;
                        o.ListaProductos = DKbase.web.capaDatos.capaCAR_WebService_base.AgregarProductoAlCarritoDesdeArchivoPedidosV5(oCliente, sucElegida, oCliente.cli_codsuc, tablaArchivoPedidos, TipoArchivo, oCliente.cli_codigo, oCliente.cli_codprov, oCliente.cli_isGLN, oUsuario.id);
                        o.nombreArchivoCompleto = pNombreArchivo;
                        o.nombreArchivoCompletoOriginal = pNombreArchivoOriginal;
                        o.isCorrect = true;
                        DKbase.Util.AgregarHistorialSubirArchivo(oCliente.cli_codigo, sucElegida, pNombreArchivo, pNombreArchivoOriginal, DateTime.Now);
                        resultado = o;
                    }
                }
            }
            return resultado;
        }

        public static cSubirPedido_return LeerArchivoPedido_Excel(Usuario oUsuario, cClientes oCliente, string pNombreArchivo, string pSucursal, string pNombreArchivoOriginal, Boolean? pIsNombreRepetido)
        {
            string tipoArchivo = "E";
            cSubirPedido_return resultado = null;
            if (!string.IsNullOrWhiteSpace(pNombreArchivo))
            {
                string rutaTemporal = Path.Combine(Helper.getFolder, "archivos", "ArchivosPedidos");
                string rutaTemporalAndNombreArchivo = Path.Combine(rutaTemporal, pNombreArchivo);
                if (System.IO.File.Exists(rutaTemporalAndNombreArchivo))
                {
                    try
                    {
                        using (ExcelPackage package = new ExcelPackage(new FileInfo(rutaTemporalAndNombreArchivo)))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet != null)
                            {
                                DataTable tablaArchivoPedidos = FuncionesPersonalizadas_base.ObtenerDataTableProductosCarritoArchivosPedidos();

                                // Leer los datos del archivo de Excel y agregarlos a la tabla
                                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                                {
                                    DataRow r = LeerRenglonArchivoExcel(tablaArchivoPedidos, worksheet, row);
                                    if (r != null)
                                        tablaArchivoPedidos.Rows.Add(r);
                                }

                                // Procesar la tabla de pedidos
                                string sucElegida = pSucursal;
                                cSubirPedido_return o = new cSubirPedido_return();
                                o.SucursalEleginda = sucElegida;
                                o.ListaProductos = DKbase.web.capaDatos.capaCAR_WebService_base.AgregarProductoAlCarritoDesdeArchivoPedidosV5(oCliente, sucElegida, oCliente.cli_codsuc, tablaArchivoPedidos, tipoArchivo, oCliente.cli_codigo, oCliente.cli_codprov, oCliente.cli_isGLN, oUsuario.id);
                                o.nombreArchivoCompleto = pNombreArchivo;
                                o.nombreArchivoCompletoOriginal = pNombreArchivoOriginal;
                                o.isCorrect = true;
                                DKbase.Util.AgregarHistorialSubirArchivo(oCliente.cli_codigo, sucElegida, pNombreArchivo, pNombreArchivoOriginal, DateTime.Now);
                                resultado = o;
                            }
                        }
                    }
                    catch 
                    {
                        // Manejar cualquier excepción que ocurra al leer el archivo de Excel
                        Console.WriteLine("Hubo un error");
                        resultado = null;
                    }
                }
            }
            return resultado;
        }

        public static DataRow LeerRenglonArchivoExcel(DataTable tabla, ExcelWorksheet worksheet, int row)
        {
            string filaCompleta = worksheet.Cells[row, 1, row, tabla.Columns.Count].Select(c => c.Value == null ? string.Empty : c.Value.ToString()).Aggregate((s1, s2) => s1 + "\t" + s2);
            string[] columnas = filaCompleta.Split('\t');

            string strCodBarra = columnas[0];
            string strCantidad = columnas[1];

            if (!string.IsNullOrEmpty(strCodBarra))
                return FuncionesPersonalizadas_base.ConvertProductosCarritoArchivosPedidosToDataRow(tabla, null, Convert.ToInt32(strCantidad), strCodBarra, string.Empty, string.Empty, "E");

            return null;
        }

        public static DataRow LeerRenglonArchivoTXT(DataTable pTabla, string pRenglon)
        {
            //DataRow r = null;
            //Primer columna numerica?
            try
            {
                int n;
                if (int.TryParse(pRenglon[0].ToString(), out n))
                {
                    string[] partSplit = pRenglon.Split('\t');
                    string strCodBarra = partSplit[5];
                    string strCantidad = partSplit[2];
                    return FuncionesPersonalizadas_base.ConvertProductosCarritoArchivosPedidosToDataRow(pTabla, null, Convert.ToInt32(strCantidad), strCodBarra, string.Empty, string.Empty, "S");
                }
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pTabla, pRenglon);
            }
            return null;
        }
        public static DataRow LeerRenglonArchivoASC(DataTable pTabla, string pRenglon)
        {
            try
            {
                string[] partSplit = pRenglon.Split(',');
                string strCodBarra = partSplit[4];
                string strCantidad = partSplit[2];
                return FuncionesPersonalizadas_base.ConvertProductosCarritoArchivosPedidosToDataRow(pTabla, null, Convert.ToInt32(strCantidad), strCodBarra, string.Empty, string.Empty, "S");
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pTabla, pRenglon);
            }
            return null;
        }
        public static DataRow LeerRenglonArchivoS(DataTable pTabla, string pRenglon)
        {
            try
            {
                //Cantidad: 5 dígitos
                //Alfa-Beta: 10 dígitos
                //Troquel: 10 dígitos
                //Cod. Barra: 13 dígitos
                //Característica: 1 dígito (ya no se usa)
                string strCantidad = string.Empty;
                for (int i = 0; i < 5; i++)
                {
                    strCantidad += pRenglon[i].ToString();
                }
                string strAlfaBeta = string.Empty;
                for (int i = 5; i < 15; i++)
                {
                    strAlfaBeta += pRenglon[i].ToString();
                }
                string strTroquel = string.Empty;
                for (int i = 15; i < 25; i++)
                {
                    strTroquel += pRenglon[i].ToString();
                }
                string strCodBarra = string.Empty;
                for (int i = 25; i < 38; i++)
                {
                    strCodBarra += pRenglon[i].ToString();
                }
                return FuncionesPersonalizadas_base.ConvertProductosCarritoArchivosPedidosToDataRow(pTabla, null, Convert.ToInt32(strCantidad), strCodBarra, strAlfaBeta, strTroquel, "S");
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pTabla, pRenglon);
            }
            return null;
        }
        public static DataRow LeerRenglonArchivoF(DataTable pTabla, string pRenglon)
        {
            try
            {
                //Nro Cliente: 4 dígitos
                //Cantidad: 5 dígitos (0 a la izquierda)
                //Nro de Producto:13 dígitos
                string strNroCliente = string.Empty;
                for (int i = 0; i < 4; i++)
                {
                    strNroCliente += pRenglon[i].ToString();
                }
                string strCantidad = string.Empty;
                for (int i = 4; i < 9; i++)
                {
                    strCantidad += pRenglon[i].ToString();
                }
                string strNroProducto = string.Empty;
                for (int i = 9; i < 22; i++)
                {
                    strNroProducto += pRenglon[i].ToString();
                }
                return FuncionesPersonalizadas_base.ConvertProductosCarritoArchivosPedidosToDataRow(pTabla, strNroProducto, Convert.ToInt32(strCantidad), null, null, null, "F");
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, pTabla, pRenglon);
            }
            return null;
        }
        public static cSubirPedido_return CargarArchivoPedidoDeNuevo(Usuario oUsuario, cClientes oCliente, int has_id)
        {
            cSubirPedido_return result = null;
            if (oUsuario != null && oCliente != null)
            {
                capaDatos.cHistorialArchivoSubir objHistorialArchivoSubir = DKbase.Util.RecuperarHistorialSubirArchivoPorId(has_id);
                if (objHistorialArchivoSubir != null)

                    //Cambiar a LeerArchivoPedido y ver los parametros, para que dependa de la extension si llama a LeerArchivoPedido_Generica o a LeerArchivoPedido_Excel
                    result = LeerArchivoPedido_Generica(oUsuario, oCliente, objHistorialArchivoSubir.has_NombreArchivo, objHistorialArchivoSubir.has_sucursal, objHistorialArchivoSubir.has_NombreArchivoOriginal, null);
            }
            return result;
        }
    }
}

