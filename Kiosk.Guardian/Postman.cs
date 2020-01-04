using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Kiosk.Guardian
{
    public class Postman
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string FromMail { get; set; }
        public string ToMail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void Send(string subject, string body)
        {
            MailMessage message = new MailMessage();
            message.Subject = subject + " - " + Environment.MachineName.ToUpper();
            message.IsBodyHtml = false;
            MailAddress to = new MailAddress(ToMail);
            message.To.Add(to);
            message.From = new MailAddress(FromMail);
            message.Body = body;

            SmtpClient client = new SmtpClient();
            client.Host = Server;
            client.Port = Port;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = false;
            client.Credentials = new NetworkCredential(Username, Password);
            client.Send(message);
        }
    }
}
