using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace DKbase.web.generales
{
    public class cMail_base
    {
        public static bool enviarMail(string pCorreoMail, string pAsunto, string pCuerpo)
        {
            List<string> listaCorreoMail = new List<string>();
            listaCorreoMail.Add(pCorreoMail);
            return enviarMail_generico(listaCorreoMail, pAsunto, pCuerpo);
        }
        public static bool enviarMail_generico(List<string> pListaCorreoMail, string pAsunto, string pCuerpo)
        {
            bool resultado = true;
            try
            {
                
                String mail_from = Helper.getMail_from;
                String mail_pass = Helper.getMail_pass;
                string SMTP_SERVER = Helper.getSMTP_SERVER;
                int SMTP_PORT = Helper.getSMTP_PORT;

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                string asunto = pAsunto;
                correo.From = new MailAddress(mail_from);
                foreach (var itemCorreoMail in pListaCorreoMail)
                {
                    correo.To.Add(itemCorreoMail);
                }

                correo.Subject = asunto;
                correo.Body = pCuerpo;
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;


                SmtpClient smtp = new System.Net.Mail.SmtpClient(SMTP_SERVER, SMTP_PORT);

                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(mail_from, mail_pass);
                // smtp.EnableSsl = true;

                smtp.Send(correo);
            }
            catch (Exception ex)
            {
                resultado = false;
            }
            return resultado;
        }
    }
}
