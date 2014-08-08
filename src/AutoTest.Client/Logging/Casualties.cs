using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace AutoTest.Client.Logging
{
    public class Casualties
    {
        public static void ReportCasualty(Exception ex)
        {
            send(ex.ToString());
        }

        public static void ReportCasualty(string information)
        {
            ThreadPool.QueueUserWorkItem(send, information);
        }

        private static void send(object data)
        {
            try
            {
                var text = data.ToString();
			    var mail = new MailMessage();
                mail.To.Add("moosecasualties@gmail.com");
			    mail.From = new MailAddress("moosecasualties@gmail.com");
			    mail.Subject = getHeader(text);
			    mail.Body = text;

                var smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("moosecasualties@gmail.com", "mooseoncebitmysister");
                smtp.EnableSsl = true;
			    smtp.Send(mail);
            }
            catch
            {
            }
        }

        private static string getHeader(string text)
        {
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
                return lines[0];
            return text;
        }
    }
}
