﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DKbase
{
  public sealed class Helper
    {
        private Helper()
        {
        }

        public static string getConnectionStringSQL
        {
            set; get;
        }
        public static string getConnectionStringSQL_Desarrollo
        {
            set; get;
        }
        public static string getConnectionStringSQL_intranet
        {
            set; get;
        }
        public static string getFolder
        {
            set; get;
        }
        public static string getTipoApp
        {
            set; get;
        }
        public static string getMail_from
        {
            set; get;
        }
        public static string getMail_pass
        {
            set; get;
        }
        public static string getSMTP_SERVER
        {
            set; get;
        }
        public static int getSMTP_PORT
        {
            set; get;
        }
        //
        public static string getUrl_DKcore
        {
            set; get;
        }
        public static string getUrl_DKdll
        {
            set; get;
        }
    }
}
