using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;

namespace DKbase.generales
{
    public class cThumbnail
    {
        public static System.Drawing.Image obtenerImagen(string ruta, string nombre, string strAncho, string strAlto, string strColor, bool boolAlto)
        {
            string RutaCompleta = Helper.getFolder + @"\archivos\" + ruta + @"\";
            string RutaCompletaNombreArchivo = RutaCompleta + nombre;
            if (System.IO.File.Exists(RutaCompletaNombreArchivo) && Validaciones.IsNumeric(strAncho) && Validaciones.IsNumeric(strAlto))
            {
                try
                {
                    Color objColor;
                    if (strColor != string.Empty)
                    {
                        objColor = System.Drawing.ColorTranslator.FromHtml(strColor);
                    }
                    else
                    {
                        objColor = Color.Empty;//Color.White;
                    }
                    string[] listaParteNombre = nombre.Split('.');
                    string CacheNombreArchivo = string.Empty;
                    string CacheExtencionArchivo = string.Empty;
                    for (int i = 0; i < listaParteNombre.Length - 1; i++)
                    {
                        CacheNombreArchivo += listaParteNombre[i];
                    }
                    CacheExtencionArchivo = listaParteNombre[listaParteNombre.Length - 1];
                    string ChacheRutaDeImagenRedimencionada = RutaCompleta + "resize" + @"\";
                    string ChacheRutaYNombreArchivoRedimencionado = ChacheRutaDeImagenRedimencionada + CacheNombreArchivo + "_" + strAncho + "x" + strAlto + "_" + boolAlto.ToString() + "_" + strColor + "." + CacheExtencionArchivo;
                    if (System.IO.File.Exists(ChacheRutaYNombreArchivoRedimencionado))
                    {
                        System.Drawing.Image oImgCache = System.Drawing.Image.FromFile(ChacheRutaYNombreArchivoRedimencionado);
                        return oImgCache;
                    }
                    else
                    {
                        int ancho = Convert.ToInt32(strAncho);
                        int alto = Convert.ToInt32(strAlto);
                        System.Drawing.Image origImagen = System.Drawing.Image.FromFile(RutaCompletaNombreArchivo);
                        int anchoRelacion;
                        int altoRelacion;
                        if (ancho == -1)
                        {
                            ancho = origImagen.Width;
                        }
                        if (alto == -1)
                        {
                            alto = origImagen.Height;
                        }
                        if (!boolAlto)
                        {
                            // Default
                            // el ancho predomina            
                            if (origImagen.Width > ancho)
                            {
                                altoRelacion = (origImagen.Height * ancho) / origImagen.Width;
                                anchoRelacion = ancho;
                                if (altoRelacion > alto)
                                {
                                    anchoRelacion = (origImagen.Width * alto) / origImagen.Height;
                                    altoRelacion = alto;
                                }
                            }
                            else if (origImagen.Height > alto)
                            {
                                anchoRelacion = (origImagen.Width * alto) / origImagen.Height;
                                altoRelacion = alto;
                                if (anchoRelacion > ancho)
                                {
                                    altoRelacion = (origImagen.Height * ancho) / origImagen.Width;
                                    anchoRelacion = ancho;
                                }
                            }
                            else
                            {
                                anchoRelacion = origImagen.Width;// ancho;
                                altoRelacion = origImagen.Height;// alto;
                            }
                        }
                        else
                        {
                            // alto es predomina                        
                            if (origImagen.Height > alto)
                            {
                                anchoRelacion = (origImagen.Width * alto) / origImagen.Height;
                                altoRelacion = alto;
                                if (anchoRelacion > ancho)
                                {
                                    altoRelacion = (origImagen.Height * ancho) / origImagen.Width;
                                    anchoRelacion = ancho;
                                }
                            }
                            else if (origImagen.Width > ancho)
                            {
                                altoRelacion = (origImagen.Height * ancho) / origImagen.Width;
                                anchoRelacion = ancho;
                                if (altoRelacion > alto)
                                {
                                    anchoRelacion = (origImagen.Width * alto) / origImagen.Height;
                                    altoRelacion = alto;
                                }
                            }
                            else
                            {
                                anchoRelacion = origImagen.Width;// ancho;
                                altoRelacion = origImagen.Height;// alto;
                            }
                        }
                        //
                        DirectoryInfo DIR = new DirectoryInfo(ChacheRutaDeImagenRedimencionada);
                        if (!DIR.Exists)
                            DIR.Create();
                        System.Drawing.Image oImg = objColor == Color.Empty ? RedimencionarImagen(origImagen, anchoRelacion, altoRelacion) : RedimencionarImagen(origImagen, anchoRelacion, altoRelacion, ancho, alto, objColor);
                        oImg.Save(ChacheRutaYNombreArchivoRedimencionado);
                        return oImg;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, ruta,  nombre,  strAncho,  strAlto,  strColor,  boolAlto);
                }
            }
            return null;
        }
        public static System.Drawing.Image RedimencionarImagen(System.Drawing.Image srcImage, int newWidth, int newHeight, int pAnchoOriginal, int pAltoOriginal, Color pColor)
        {
            Bitmap objBitmap = new Bitmap(pAnchoOriginal, pAltoOriginal);
            Graphics imagenGraphics = Graphics.FromImage(objBitmap);
            //Definimos el fondo de color blanco
            SolidBrush whiteBrush = new SolidBrush(pColor);
            //Aquí es donde creamos el fondo, de color
            //blanco tal y como especificamos anteriormente
            imagenGraphics.FillRectangle(whiteBrush, 0, 0, pAnchoOriginal, pAltoOriginal);
            int posX = 0;
            int posY = 0;
            if (pAnchoOriginal > newWidth)
            {
                posX = (pAnchoOriginal - newWidth) / 2;
            }
            if (pAltoOriginal > newHeight)
            {
                posY = (pAltoOriginal - newHeight) / 2;
            }
            imagenGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            imagenGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            imagenGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            imagenGraphics.DrawImage(srcImage, new Rectangle(posX, posY, newWidth, newHeight), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
            MemoryStream imagenMemoryStream = new MemoryStream();
            objBitmap.Save(imagenMemoryStream, ImageFormat.Jpeg);
            srcImage = System.Drawing.Image.FromStream(imagenMemoryStream);

            return srcImage;
        }
        public static System.Drawing.Image RedimencionarImagen(System.Drawing.Image srcImage, int newWidth, int newHeight)
        {
            using (Bitmap imagenBitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppRgb))
            {
                imagenBitmap.SetResolution(Convert.ToInt32(srcImage.HorizontalResolution), Convert.ToInt32(srcImage.HorizontalResolution));
                using (Graphics imagenGraphics = Graphics.FromImage(imagenBitmap))
                {
                    imagenGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                    imagenGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    imagenGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    imagenGraphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                    MemoryStream imagenMemoryStream = new MemoryStream();
                    imagenBitmap.Save(imagenMemoryStream, ImageFormat.Jpeg);
                    srcImage = System.Drawing.Image.FromStream(imagenMemoryStream);
                }
            }
            return srcImage;
        }
    }
}
