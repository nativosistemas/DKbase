using System;
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
        public static string getPathSiteWeb
        {
            set; get;
        }
        public static string getFolderLog
        {
            set; get;
        }
        public static string getTipoApp
        {
            set; get;
        }
    }
}
