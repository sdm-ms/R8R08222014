using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace R8R.Common
{
    public class EmailService
    {
        public static bool SendMailMessage(string subject, string body, string to)
        {
            MailMessage _MailMessage = null;
            NetworkCredential _BasicAuthenticationInfo = null;
            SmtpClient _Smtp = null;
            try
            {
                _MailMessage = new MailMessage();

                _MailMessage.To.Add(new MailAddress(to));
                _MailMessage.Subject = subject;
                _MailMessage.Body = body;
                _MailMessage.IsBodyHtml = true;
                _MailMessage.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"], ConfigurationManager.AppSettings["MailName"]);
                _MailMessage.Priority = MailPriority.Normal;
                _BasicAuthenticationInfo = new NetworkCredential(ConfigurationManager.AppSettings["SMTPUserName"], ConfigurationManager.AppSettings["SMTPPassword"]);
                _Smtp = new SmtpClient();
                _Smtp.Host = ConfigurationManager.AppSettings["SMTPHostName"];
                _Smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                _Smtp.UseDefaultCredentials = false;
                _Smtp.Credentials = _BasicAuthenticationInfo;
                _Smtp.EnableSsl = true;
                _Smtp.Send(_MailMessage);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _MailMessage.Dispose();
                _BasicAuthenticationInfo = null;
                _Smtp.Dispose();

            }
            return true;
        }
    }
}
