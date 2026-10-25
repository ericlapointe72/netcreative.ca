using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Newsletter : System.Web.UI.Page
    {
        private const string SignatureId = "Image";
        private string connectionString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            if (!IsPostBack)
            {
                // The page opens on the TEST mode. It used to open with neither choice ticked, and the send button then sent the
                // newsletter to every subscriber while its label said "test".
                RadioButton_Newsletter_Test.Checked = true;
                RadioButton_Newsletter.Checked = false;
            }

            ShowMode();
        }

        // the mode shown to the administrator is the mode used: everything below reads the same property
        private bool SendToEverybody
        {
            get { return RadioButton_Newsletter.Checked && !RadioButton_Newsletter_Test.Checked; }
        }

        private void ShowMode()
        {
            Panel_Test.Visible = !SendToEverybody;
            Panel_Warning.Visible = SendToEverybody;
            Button_Newsletter_Send.Text = SendToEverybody ? Global.Admin_NewsletterSendNewsletterButton : Global.Admin_NewsletterSendTestButton;
        }

        // sending to everybody asks first, with the number of subscribers (set after the events, when the mode is final)
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (SendToEverybody)
            {
                Button_Newsletter_Send.OnClientClick = AdminUi.ConfirmScript(Global.Format(Global.Text("Admin_NewsletterConfirm"), SubscriberCount));
            }
            else
            {
                Button_Newsletter_Send.OnClientClick = string.Empty;
            }
        }

        // the number of subscribers, counted once per request (the page shows it and the confirmation of the send repeats it)
        private int? subscriberCount;

        protected int SubscriberCount
        {
            get
            {
                if (subscriberCount == null)
                {
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SUBSCRIBE WHERE CONFIRMED IS NOT NULL", connection))
                        {
                            connection.Open();
                            subscriberCount = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex, "newsletter page");
                        subscriberCount = 0;
                    }
                }

                return subscriberCount.Value;
            }
        }

        // ---- the message ---------------------------------------------------------------------------------

        // what the administrator typed is TEXT: it is encoded, and the line breaks become <br> (before, a line break was lost in the
        // HTML and nothing typed could be formatted)
        private static string Html(string text)
        {
            return HttpUtility.HtmlEncode(text ?? string.Empty).Replace("\r\n", "<br>").Replace("\n", "<br>");
        }

        // One paragraph for each part that was filled in (an empty introduction or closing leaves no blank space), the signature, then
        // the small unsubscribe line. The preview of the page (newsletter.js) shows the same parts in the same order.
        // where the unsubscribe link of each recipient goes in the message (BuildMessage writes it: the link is made for one address)
        private const string UnsubscribePlaceholder = "{{unsubscribe}}";

        private string BodyHtml()
        {
            const string paragraph = "<p style=\"margin:0 0 16px\">";

            var html = new StringBuilder();
            html.Append("<html><head><meta charset=\"utf-8\"></head><body style=\"margin:0;padding:0\">")
                .Append("<div style=\"font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:1.5;color:#222222;max-width:600px;padding:16px\">");

            foreach (string part in new[] { TextBox_Newsletter_Intro.Text, TextBox_Newsletter_Body.Text, TextBox_Newsletter_Greeting.Text })
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    html.Append(paragraph).Append(Html(part.Trim())).Append("</p>");
                }
            }

            html.Append(paragraph).Append("<img src=\"cid:").Append(SignatureId).Append("\" alt=\"Net.Créative\" style=\"max-width:100%\"></p>")
                .Append("<p style=\"margin:24px 0 0;font-size:12px;color:#777777\">").Append(HttpUtility.HtmlEncode(Global.Admin_NewsletterUnsubscribe1))
                .Append("<a href=\"" + UnsubscribePlaceholder + "\" style=\"color:#4473c5\">").Append(HttpUtility.HtmlEncode(Global.Admin_NewsletterUnsubscribe2))
                .Append("</a>").Append(HttpUtility.HtmlEncode(Global.Admin_NewsletterUnsubscribe3)).Append("</p></div></body></html>");

            return html.ToString();
        }

        // One message per recipient, each with its OWN signature attachment: a message that was sent once has read its attachment to the
        // end, so the same message sent a second time carried an empty one (the signature image was broken from the second subscriber on).
        private MailMessage BuildMessage(string to, string subject, string body)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtp_user"], "Net.Créative.ca");
            mail.To.Add(new MailAddress(to));
            mail.IsBodyHtml = true;
            mail.Subject = subject;

            // the unsubscribe link of THIS recipient (the address, signed by the site); the "Unsubscribe" button of Gmail, Outlook and
            // Apple Mail uses the same link, posted in the background (RFC 8058), and the page unsubscribes at once
            string unsubscribeUrl = UnsubscribeLink.Url(to);
            mail.Body = body.Replace(UnsubscribePlaceholder, HttpUtility.HtmlAttributeEncode(unsubscribeUrl));
            mail.Headers.Add("List-Unsubscribe", "<" + unsubscribeUrl + ">");
            mail.Headers.Add("List-Unsubscribe-Post", "List-Unsubscribe=One-Click");

            var signature = new Attachment(Server.MapPath("~/Content/images/site/signature.png"));
            signature.ContentId = SignatureId;
            signature.ContentDisposition.Inline = true;
            signature.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            mail.Attachments.Add(signature);

            return mail;
        }

        private SmtpClient NewSmtpClient()
        {
            var client = new SmtpClient(ConfigurationManager.AppSettings["smtp_host"], Convert.ToInt32(ConfigurationManager.AppSettings["smtp_port"]));
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtp_user"], ConfigurationManager.AppSettings["smtp_password"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            return client;
        }

        // ---- sending ----------------------------------------------------------------------------------------

        private void Newsletter_Send_Test(string subject, string body)
        {
            string address = (TextBox_Newsletter_Email.Text ?? string.Empty).Trim();

            try
            {
                new MailAddress(address);
            }
            catch (Exception)
            {
                AdminUi.Toast(this, Global.Text("Admin_NewsletterTestEmailInvalid"), false);
                return;
            }

            try
            {
                using (SmtpClient client = NewSmtpClient())
                // a test says so in its subject, so it is never taken for the real mailing
                using (MailMessage mail = BuildMessage(address, "[Test] " + subject, body))
                {
                    client.Send(mail);
                }

                AdminUi.Toast(this, Global.Admin_NewsletterTestSentSuccess, true);
                ShowReport(Global.Format(Global.Text("News_ReportTest"), address), null);
                ClearFields();
            }
            catch (Exception ex)
            {
                // the details (they can name the mail server) go to the error log, never to the browser
                ErrorLogger.Log(ex, "newsletter test");
                AdminUi.Toast(this, Global.Text("Admin_Error"), false);
            }
        }

        private void Newsletter_Send_Newsletter(string subject, string body)
        {
            var addresses = new List<string>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT EMAIL FROM SUBSCRIBE WHERE CONFIRMED IS NOT NULL", connection))
                {
                    connection.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            addresses.Add(Convert.ToString(dr["EMAIL"]).Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "newsletter list");
                AdminUi.Toast(this, Global.Text("Admin_Error"), false);
                return;
            }

            if (addresses.Count == 0)
            {
                AdminUi.Toast(this, Global.Text("Admin_NewsletterNoSubscribers"), false);
                return;
            }

            // a long list takes longer than the default time a page may run
            Server.ScriptTimeout = 900;

            int sent = 0;
            var failed = new List<string>();

            using (SmtpClient client = NewSmtpClient())
            {
                foreach (string address in addresses)
                {
                    // one address that cannot be sent to (or a refusal of the mail server) no longer stops the whole mailing
                    try
                    {
                        using (MailMessage mail = BuildMessage(address, subject, body))
                        {
                            client.Send(mail);
                        }

                        sent++;
                    }
                    catch (Exception ex)
                    {
                        failed.Add(address);
                        ErrorLogger.Log(ex, "newsletter to subscriber");
                    }
                }
            }

            AdminAudit.Write("Newsletter sent", "\"" + subject + "\" sent=" + sent + " failed=" + failed.Count);

            AdminUi.Toast(this, Global.Format(Global.Text("Admin_NewsletterResult"), sent, failed.Count), failed.Count == 0);

            ShowReport(Global.Format(Global.Text("Admin_NewsletterResult"), sent, failed.Count),
                failed.Count > 0 ? Global.Format(Global.Text("Admin_NewsletterFailedList"), string.Join(", ", failed)) : null);

            ClearFields();
        }

        // what was just done, written under the button (the toast disappears after a few seconds, this stays)
        private void ShowReport(string headline, string details)
        {
            Panel_Report.Visible = true;
            Panel_Report.CssClass = details == null ? "news-report" : "news-report news-report--warn";
            Literal_Report.Text = "<strong>" + HttpUtility.HtmlEncode(headline) + "</strong>"
                + (details == null ? string.Empty : "<br>" + HttpUtility.HtmlEncode(details));
        }

        private void ClearFields()
        {
            TextBox_Newsletter_Object.Text = string.Empty;
            TextBox_Newsletter_Intro.Text = string.Empty;
            TextBox_Newsletter_Body.Text = string.Empty;
            TextBox_Newsletter_Greeting.Text = string.Empty;
        }

        // ---- events ---------------------------------------------------------------------------------------

        protected void RadioButton_Newsletter_Test_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_Newsletter_Test.Checked)
            {
                RadioButton_Newsletter.Checked = false;
            }

            ShowMode();
        }

        protected void RadioButton_Newsletter_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_Newsletter.Checked)
            {
                RadioButton_Newsletter_Test.Checked = false;
            }

            ShowMode();
        }

        protected void Button_Newsletter_Send_Click(object sender, EventArgs e)
        {
            string subject = (TextBox_Newsletter_Object.Text ?? string.Empty).Trim();

            if (subject.Length == 0 || (TextBox_Newsletter_Body.Text ?? string.Empty).Trim().Length == 0)
            {
                AdminUi.Toast(this, Global.Text("Admin_NewsletterFieldsRequired"), false);
                return;
            }

            // a subject is one line: no line break can be used to add headers to the message
            subject = subject.Replace("\r", " ").Replace("\n", " ");
            string body = BodyHtml();

            if (SendToEverybody)
            {
                Newsletter_Send_Newsletter(subject, body);
            }
            else
            {
                Newsletter_Send_Test(subject, body);
            }
        }
    }
}
