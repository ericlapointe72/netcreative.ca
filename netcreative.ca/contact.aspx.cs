using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class contact : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private string mail_subject = string.Empty;
        private string mail_body = string.Empty;
        private string message_error = string.Empty;
        private string message_email_error = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Image_QuestionFR.Visible = false;
            Image_QuestionEN.Visible = false;
            Load_Languages();
        }

        protected void Button_Send_Click(object sender, EventArgs e)
        {
            if (TextBox_Question.Text.Trim() != "6")
            {
                Response.Write("<script>showToast('" + message_error + "', 'error');</script>");
            }
            else if (!IsValidEmail(TextBox_EMail.Text))
            {
                Response.Write("<script>showToast('" + message_email_error + "', 'error');</script>");
            }
            else
            {
                Send_Message();
                // fire-and-forget: the SMTP round-trip can take several seconds and the
                // visitor doesn't need to wait on it — their submission is already safely
                // stored above, this email is just a best-effort notification to me
                System.Threading.Tasks.Task.Run(() => Send_Email());
                Response.Redirect(ResolveUrl("~/confirmation.aspx"));
            }

            TextBox_Question.Text = string.Empty;
        }

        private bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.CultureInvariant | RegexOptions.Singleline);
            return regex.IsMatch(email);
        }

        private void Load_Languages()
        {
            DropDownList_Type.Items.Add(Global.Contact_ProjectType1);
            DropDownList_Type.Items.Add(Global.Contact_ProjectType2);
            DropDownList_Type.Items.Add(Global.Contact_ProjectType3);
            DropDownList_Type.Items.Add(Global.Contact_ProjectType4);
            DropDownList_Budjet.Items.Add(Global.Contact_Budget1);
            DropDownList_Budjet.Items.Add(Global.Contact_Budget2);
            DropDownList_Budjet.Items.Add(Global.Contact_Budget3);
            DropDownList_Budjet.Items.Add(Global.Contact_Budget4);
            DropDownList_Deadline.Items.Add(Global.Contact_Deadline1);
            DropDownList_Deadline.Items.Add(Global.Contact_Deadline2);
            DropDownList_Deadline.Items.Add(Global.Contact_Deadline3);
            Button_Send.Text = Global.Contact_SendButton;
            message_error = Global.Contact_ErrorMessage;
            message_email_error = Global.Contact_EmailError;
            mail_subject = Global.Contact_MailSubject;
            mail_body = Global.Contact_MailBody;

            if (Session["language"].ToString() == "fr")
            {
                Image_QuestionFR.Visible = true;
            }
            else
            {
                Image_QuestionEN.Visible = true;
            }
        }

        private void Send_Message()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "INSERT INTO CONTACT (DATE, TIME, IP_ADDRESS, LAST_NAME, FIRST_NAME, EMAIL, PHONE, COMPANY, PROJECT_TYPE, " +
                        "PROJECT_BUDGET, PROJECT_DEADLINE, DESCRIPTION) VALUES (@DATE, @TIME, @IP_ADDRESS, @LAST_NAME, @FIRST_NAME, @EMAIL, @PHONE, @COMPANY, " +
                        "@PROJECT_TYPE, @PROJECT_BUDGET, @PROJECT_DEADLINE, @DESCRIPTION)";

                    string user_ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (user_ip == null)
                    {
                        user_ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    else
                    {
                        user_ip = user_ip.Split(',')[0];
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.Date);
                        cmd.Parameters.AddWithValue("@TIME", DateTime.Now.TimeOfDay);
                        cmd.Parameters.AddWithValue("@IP_ADDRESS", user_ip);
                        cmd.Parameters.AddWithValue("@LAST_NAME", TextBox_LastName.Text.Replace("'", "''"));
                        cmd.Parameters.AddWithValue("@FIRST_NAME", TextBox_FirstName.Text.Replace("'", "''"));
                        cmd.Parameters.AddWithValue("@EMAIL", TextBox_EMail.Text);
                        cmd.Parameters.AddWithValue("@PHONE", TextBox_Phone.Text);
                        cmd.Parameters.AddWithValue("@COMPANY", TextBox_Company.Text);
                        cmd.Parameters.AddWithValue("@PROJECT_TYPE", DropDownList_Type.SelectedValue);
                        cmd.Parameters.AddWithValue("@PROJECT_BUDGET", DropDownList_Budjet.SelectedValue);
                        cmd.Parameters.AddWithValue("@PROJECT_DEADLINE", DropDownList_Deadline.SelectedValue);
                        cmd.Parameters.AddWithValue("@DESCRIPTION", TextBox_Description.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        TextBox_FirstName.Text = string.Empty;
                        TextBox_LastName.Text = string.Empty;
                        TextBox_EMail.Text = string.Empty;
                        TextBox_Phone.Text = string.Empty;
                        TextBox_Description.Text = string.Empty;

                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Send_Email()
        {
            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["smtp_host"], Convert.ToInt32(ConfigurationManager.AppSettings["smtp_port"]));
            smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtp_user"], ConfigurationManager.AppSettings["smtp_password"]);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtp_user"], "Net.Créative.ca");
            mail.To.Add(new MailAddress(ConfigurationManager.AppSettings["smtp_user"]));

            mail.Subject = mail_subject;
            mail.Body = mail_body;

            try
            {
                smtpClient.Send(mail);
            }
            catch
            {
                // runs on a background thread after the visitor has already been
                // redirected to the confirmation page — there's no response left to
                // write a toast into, and the contact submission itself is already
                // safely stored, so a failed notification email is silently dropped
            }
        }

    }
}