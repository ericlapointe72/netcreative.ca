using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net.Mime;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Newsletter : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private string newsletter_message_test = string.Empty;
        private string newsletter_message_newsletter = string.Empty;
        private string newsletter_message_unsubscribe1 = string.Empty;
        private string newsletter_message_unsubscribe2 = string.Empty;
        private string newsletter_message_unsubscribe3 = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            RadioButton_Newsletter_Test.Text = Global.Admin_NewsletterTestRadio;
            RadioButton_Newsletter.Text = Global.Admin_NewsletterButton;
            Label_Newsletter_Email.Text = Global.Admin_NewsletterEmailLabel;
            Button_Newsletter_Send.Text = Global.Admin_NewsletterSendTestButton;
            newsletter_message_test = Global.Admin_NewsletterTestSentSuccess;
            newsletter_message_newsletter = Global.Admin_NewsletterSentSuccess;
            newsletter_message_unsubscribe1 = Global.Admin_NewsletterUnsubscribe1;
            newsletter_message_unsubscribe2 = Global.Admin_NewsletterUnsubscribe2;
            newsletter_message_unsubscribe3 = Global.Admin_NewsletterUnsubscribe3;
        }

        private void Newsletter_Send_Test()
        {
            var signature = "Image";
            var imageSignature = new Attachment(Server.MapPath("~/Content/images/site/signature.png"));
            imageSignature.ContentId = signature;
            imageSignature.ContentDisposition.Inline = true;
            imageSignature.ContentDisposition.DispositionType = DispositionTypeNames.Inline;

            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["smtp_host"], Convert.ToInt32(ConfigurationManager.AppSettings["smtp_port"]));
            smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtp_user"], ConfigurationManager.AppSettings["smtp_password"]);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtp_user"], "Net.Créative.ca");
            mail.To.Add(new MailAddress(TextBox_Newsletter_Email.Text));
            mail.IsBodyHtml = true;
            mail.Subject = TextBox_Newsletter_Object.Text;
            mail.Body =
                "<html><head></head><body>" + TextBox_Newsletter_Intro.Text + "<br><br>" + TextBox_Newsletter_Body.Text + "<br><br>" + TextBox_Newsletter_Greeting.Text +
                "<br><br><img src=\"cid:" + signature + "\"><br><br>" + newsletter_message_unsubscribe1 + "<a href=\"https://netcreative.ca/unsubscribe.aspx\">" +
                newsletter_message_unsubscribe2 + "</a>" + newsletter_message_unsubscribe3 + "</body></html>";
            mail.Attachments.Add(imageSignature);

            try
            {
                smtpClient.Send(mail);
                Response.Write("<script>showToast('" + newsletter_message_test + "', 'success');</script>");
                TextBox_Newsletter_Object.Text = string.Empty;
                TextBox_Newsletter_Intro.Text = string.Empty;
                TextBox_Newsletter_Body.Text = string.Empty;
                TextBox_Newsletter_Greeting.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Newsletter_Send_Newsletter()
        {
            var signature = "Image";
            var imageSignature = new Attachment(Server.MapPath("~/Content/images/site/signature.png"));
            imageSignature.ContentId = signature;
            imageSignature.ContentDisposition.Inline = true;
            imageSignature.ContentDisposition.DispositionType = DispositionTypeNames.Inline;

            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["smtp_host"], Convert.ToInt32(ConfigurationManager.AppSettings["smtp_port"]));
            smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtp_user"], ConfigurationManager.AppSettings["smtp_password"]);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtp_user"], "Net.Créative.ca");
            mail.IsBodyHtml = true;
            mail.Subject = TextBox_Newsletter_Object.Text;
            mail.Body =
                "<html><head></head><body>" + TextBox_Newsletter_Intro.Text + "<br><br>" + TextBox_Newsletter_Body.Text + "<br><br>" + TextBox_Newsletter_Greeting.Text +
                "<br><br><img src=\"cid:" + signature + "\"><br><br>" + newsletter_message_unsubscribe1 + "<a href=\"https://netcreative.ca/unsubscribe.aspx\">" +
                newsletter_message_unsubscribe2 + "</a>" + newsletter_message_unsubscribe3 + "</body></html>";
            mail.Attachments.Add(imageSignature);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT EMAIL FROM SUBSCRIBE";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            mail.To.Add(new MailAddress(dr["EMAIL"].ToString()));
                            smtpClient.Send(mail);
                            mail.To.Clear();
                        }

                        connection.Close();
                        Response.Write("<script>showToast('" + newsletter_message_newsletter + "', 'success');</script>");
                        TextBox_Newsletter_Object.Text = string.Empty;
                        TextBox_Newsletter_Intro.Text = string.Empty;
                        TextBox_Newsletter_Body.Text = string.Empty;
                        TextBox_Newsletter_Greeting.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        protected void RadioButton_Newsletter_Test_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_Newsletter_Test.Checked == true)
            {
                RadioButton_Newsletter.Checked = false;
                Label_Newsletter_Email.Visible = true;
                TextBox_Newsletter_Email.Visible = true;
                Button_Newsletter_Send.Text = Global.Admin_NewsletterSendTestButton;
            }
        }

        protected void RadioButton_Newsletter_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_Newsletter.Checked == true)
            {
                RadioButton_Newsletter_Test.Checked = false;
                Label_Newsletter_Email.Visible = false;
                TextBox_Newsletter_Email.Visible = false;
                Button_Newsletter_Send.Text = Global.Admin_NewsletterSendNewsletterButton;
            }
        }

        protected void Button_Newsletter_Send_Click(object sender, EventArgs e)
        {
            if (RadioButton_Newsletter_Test.Checked == true)
            {
                Newsletter_Send_Test();
            }
            else
            {
                Newsletter_Send_Newsletter();
            }
        }
    }
}
