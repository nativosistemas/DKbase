using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase.generales
{
    public class Numerica
    {
        public static string toString_NumeroTXT_N10(decimal n)
        {
            string result = string.Empty;
            result += Numerica.toString_ParteEntera_8(n);
            result += Numerica.toString_ParteDecimal_2(n);
            return result;
        }
        public static string toString_NumeroTXT_N10(string pValor)
        {
            //return AgregarSeparaciónDeMilesConDecimal(pValor);
            return SinSeparaciónDeMilesConDecimal(pValor);
        }
        public static string toString_ParteEntera_8(decimal n)
        {
            return toString_ParteEntera(8, n);
        }
        public static string toString_ParteEntera(int totalWidth, decimal n)
        {
            //return AgregarSeparaciónDeMiles(Math.Truncate(n).ToString()).PadLeft(totalWidth, '0');
            return Math.Truncate(n).ToString().PadLeft(totalWidth, '0');
        }
        public static string toString_ParteDecimal_2(decimal n)
        {
            return ReplaceFirst(n.ToString("F2"), Math.Truncate(n).ToString(), string.Empty).Replace(".", string.Empty).Replace(",", string.Empty);
        }
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        //public static string AgregarSeparaciónDeMiles(string pValor)
        //{
        //    string resultado = string.Empty;
        //    resultado = string.Format("{0:#,##0.##}", Convert.ToDouble(pValor));
        //    return resultado;
        //}

        /// <summary>
        /// #.###.###,## 
        /// //eeeeeeeedd
        /// </summary>
        /// <param name="pValor"></param>
        /// <returns></returns>
        //public static string AgregarSeparaciónDeMilesConDecimal(string pValor)
        //{
        //    bool isVacio = false;
        //    if (pValor == null)
        //    {
        //        isVacio = true;
        //    }
        //    else if (pValor == string.Empty)
        //    {
        //        isVacio = true;

        //    }
        //    string resultado = string.Empty;
        //    if (isVacio)
        //    {
        //        resultado = "0".PadLeft(8, '0') + "00";
        //    }
        //    else
        //    {
        //        string parteDecimal = string.Empty;
        //        string parteEntera = string.Empty;
        //        if (pValor.IndexOf(',') == -1)
        //        {
        //            parteDecimal = "00";
        //            parteEntera = pValor;
        //        }
        //        else
        //        {
        //            string[] formatoNroDecimal = pValor.Split(',');
        //            parteDecimal = formatoNroDecimal[1].PadRight(2, '0');
        //            parteEntera = formatoNroDecimal[0];
        //        }
        //        parteEntera = parteEntera.Replace(".", "");
        //        parteEntera = string.Format("{0:#,##0.##}", Convert.ToDouble(parteEntera));

        //        resultado = parteEntera.PadLeft(8, '0') + parteDecimal;
        //    }
        //    return resultado;
        //}
        public static string SinSeparaciónDeMilesConDecimal(string pValor)
        {
            bool isVacio = false;
            if (pValor == null)
            {
                isVacio = true;
            }
            else if (pValor == string.Empty)
            {
                isVacio = true;

            }
            string resultado = string.Empty;
            if (isVacio)
            {
                resultado = "0".PadLeft(8, '0') + "00";
            }
            else
            {
                string parteDecimal = string.Empty;
                string parteEntera = string.Empty;
                if (pValor.IndexOf(',') == -1)
                {
                    parteDecimal = "00";
                    parteEntera = pValor;
                }
                else
                {
                    string[] formatoNroDecimal = pValor.Split(',');
                    parteDecimal = formatoNroDecimal[1].PadRight(2, '0');
                    parteEntera = formatoNroDecimal[0];
                }
                parteEntera = parteEntera.Replace(".", "");
                //parteEntera = string.Format("{0:#,##0.##}", Convert.ToDouble(parteEntera));

                resultado = parteEntera.PadLeft(8, '0') + parteDecimal;
            }
            return resultado;
        }
        public static string FormatoNumeroPuntoMilesComaDecimal(decimal pValor)
        {
            bool isNroNegativo = false;
            string resultado = string.Empty;
            if (pValor.ToString().IndexOf("-") != -1)
            {
                isNroNegativo = true;
            }
            //pValor =  pValor.to
            string s = pValor.ToString("#.#########", System.Globalization.CultureInfo.InvariantCulture);
            if (s.IndexOf(".") == -1)
            {
                s += ".0";
            }
            string[] parteNumero = s.Split('.');
            //string parteEntera = string.Empty;
            string parteDecimal = string.Empty;
            if (parteNumero[1].Length == 1)
            {
                parteDecimal = parteNumero[1] + "0";
            }
            else
            {
                parteDecimal = parteNumero[1];
            }
            if (parteNumero[0].Length == 0)
            {
                parteNumero[0] = "0";
            }
            int cant = parteNumero[0].Length;
            string numeroPorParteAUX = string.Empty;
            string numeroPorParte = parteNumero[0].Replace("-", "");
            while (numeroPorParte.Length > 3)
            {
                numeroPorParteAUX = '.' + numeroPorParte.Substring(numeroPorParte.Length - 3) + numeroPorParteAUX;
                numeroPorParte = numeroPorParte.Substring(0, numeroPorParte.Length - 3);
            }
            numeroPorParteAUX = numeroPorParte + numeroPorParteAUX;
            //parteEntera = numeroPorParteAUX;

            string signo = string.Empty;
            if (isNroNegativo)
            {
                signo = "-";
            }
            resultado = signo + numeroPorParteAUX + "," + parteDecimal;
            return resultado;
        }
    }

}
