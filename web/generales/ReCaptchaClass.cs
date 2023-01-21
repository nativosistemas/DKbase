using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DKbase.web.generales
{
  public class ReCaptchaClass
    {
        static string PrivateKey = Helper.getReCAPTCHA_ClaveSecreta;
        public static bool Validate(string EncodedResponse)
        {
            bool result = false;
            try
            {
                var client = new System.Net.WebClient();
                string GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse));
                if (!string.IsNullOrEmpty(GoogleReply) && GoogleReply.ToLower().Contains("true"))
                    result = true;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now);
            }
            return result;
        }
        public string success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }

        private string m_Success;
        private List<string> m_ErrorCodes;
    }
}
