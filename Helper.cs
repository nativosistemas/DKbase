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
        public static string getFolder
        {
            set; get;
        }
        public static string getTipoApp
        {
            set; get;
        }
    }
}
