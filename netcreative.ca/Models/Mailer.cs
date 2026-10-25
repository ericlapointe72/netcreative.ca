using System;
using System.Configuration;
using System.Net.Mail;

namespace netcreative.ca
{
    // The mail server of the site (the settings of App_Data\appSettings.secrets.config) and the messages the site sends to a visitor
    // or to its owner.
    public static class Mailer
    {
        public const string SiteBase = "https://www.netcreative.ca";

        public static string FromAddress
        {
            get { return ConfigurationManager.AppSettings["smtp_user"]; }
        }

        public static SmtpClient NewClient()
        {
            var client = new SmtpClient(ConfigurationManager.AppSettings["smtp_host"], Convert.ToInt32(ConfigurationManager.AppSettings["smtp_port"]));
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtp_user"], ConfigurationManager.AppSettings["smtp_password"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            return client;
        }

        // A one-line text for a subject or a name in a header: a line break there could add headers to the message.
        public static string OneLine(string text)
        {
            return (text ?? string.Empty).Replace("\r", " ").Replace("\n", " ").Trim();
        }

        // Sends one message (throws when the mail server refuses it). replyTo is optional.
        public static void Send(string to, string subject, string body, bool isHtml, string replyTo = null)
        {
            using (SmtpClient client = NewClient())
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(FromAddress, "Net.Créative.ca");
                mail.To.Add(new MailAddress(to));
                mail.Subject = OneLine(subject);
                mail.Body = body;
                mail.IsBodyHtml = isHtml;

                if (!string.IsNullOrEmpty(replyTo))
                {
                    try
                    {
                        mail.ReplyToList.Add(new MailAddress(replyTo));
                    }
                    catch (FormatException)
                    {
                        // an address that cannot be a reply address is simply not one
                    }
                }

                client.Send(mail);
            }
        }
    }
}
