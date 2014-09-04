using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace ClassLibrary1.Model
{
   public class Email
    {
        static string server = "smtp.gmail.com";
        static int port = 587;
        static string password = "finch86lives";

        public static void SendMessage(string to, string messageSubject, string messageBody)
        {
            string from = "rateroomail@gmail.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = messageSubject;
            message.Body = messageBody;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient(server, port);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, password);
            client.Send(message);
        }
    }
}
