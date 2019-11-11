using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Xml.Serialization;
using System.Net;

namespace jimBot {
    class Helpers {
        public static string GetDate(string format) {
            return DateTime.Now.ToString(format);
        }

        public static object Write(string inputString = "") {
            if (inputString.Equals(string.Empty)) {
                Logger.Log("ERROR", "No data for output");
                return null;
            }

            Console.WriteLine(inputString);

            return null;
        }

        public static object GetInput() {
            return Console.ReadLine();
        }

        public static int GetRandom(int start = 1, int end = 100000) {
            return new Random().Next(start, end);
        }

        public static async Task SendEmailAsync(string toUser, string subject, string body) {
            Logger.Log("INFO", "Send user email");

            try {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("alexey.porotikov@gmail.com");
                mail.To.Add(new MailAddress(toUser));
                mail.Subject = subject;
                mail.Body = body;
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("alexey.porotikov@gmail.com".Split('@')[0], "ghbdtn106512");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            } catch (Exception e) {
                throw new Exception("Mail.Send: " + e.Message);
            }
        }
    }
}
