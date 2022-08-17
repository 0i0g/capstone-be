using System;
using System.Net;
using System.Net.Mail;

namespace Utilities.Helper
{
    public static class EmailHelpers
    {
        private const string Host = "smtp.gmail.com";
        private const int Port = 587;
        
        public static bool SendEmail(string to, string subject, string body, string userName, string password)
        {
            var credential = new NetworkCredential(userName, password);
            var client = new SmtpClient(Host, Port);
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = credential;

            var mailMessage = new MailMessage(userName, to, subject, body);
            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

    public class SmtpConfiguration
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}