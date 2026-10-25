using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class contact : System.Web.UI.Page
    {
        // messages accepted from one IP address per hour (the form must not be usable to fill the list of messages or the owner's mailbox)
        private const int MaxMessagesPerHour = 5;

        // a person needs more than this to fill the form; a robot posts it at once
        private const int MinSecondsToFill = 4;

        private string connectionString = string.Empty;
        private string mail_subject = string.Empty;
        private string message_error = string.Empty;
        private string message_email_error = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Contact_Title, Global.Contact_Description);
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Load_Languages();

            // the stamp of the time the form was shown (a retry after a mistake keeps the first one)
            if (!IsPostBack)
            {
                HiddenField_Stamp.Value = FormTimer.Issue();
            }

            if (Session[ContactCaptcha.SessionTextKey] == null || Session[SessionAnswerKey] == null)
            {
                PickNewQuestion();
            }
        }

        // the picture of the sum (drawn by ContactCaptcha.ashx), shown after the events: a wrong answer has just drawn a new sum
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Image_Question.ImageUrl = "~/ContactCaptcha.ashx?v=" + (Session[SessionNonceKey] as string);
            Image_Question.AlternateText = Global.Contact_VerifyLabel;
        }

        // The anti-spam question is a small sum (1 to 9, added or subtracted) drawn as a picture, different for every visitor and
        // every attempt: it used to be one fixed picture whose answer (6) any robot could learn. The sum stays in the visitor's session.
        private const string SessionAnswerKey = "Contact.AntiSpam.Answer";
        private const string SessionNonceKey = "Contact.AntiSpam.Nonce";

        private void PickNewQuestion()
        {
            var bytes = new byte[3];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            int a = bytes[0] % 9 + 1;
            int b = bytes[1] % 9 + 1;
            bool plus = bytes[2] % 2 == 0;

            if (!plus && b > a)
            {
                int swap = a;
                a = b;
                b = swap;
            }

            Session[ContactCaptcha.SessionTextKey] = a + (plus ? " + " : " - ") + b + " =";
            Session[SessionAnswerKey] = (plus ? a + b : a - b).ToString();
            Session[SessionNonceKey] = Guid.NewGuid().ToString("N"); // so the browser does not keep the old picture
        }

        protected void Button_Send_Click(object sender, EventArgs e)
        {
            // the field only a robot fills: it is told the message was sent, and nothing is kept or sent
            if (!string.IsNullOrWhiteSpace(TextBox_Website.Text))
            {
                Response.Redirect(ResolveUrl("~/confirmation.aspx"));
                return;
            }

            FormTimer.Verdict timing = FormTimer.Check(HiddenField_Stamp.Value, MinSecondsToFill);

            if (timing == FormTimer.Verdict.Invalid)
            {
                AdminUi.Toast(this, Global.Text("Contact_Reload"), false);
            }
            else if (timing == FormTimer.Verdict.TooFast)
            {
                AdminUi.Toast(this, Global.Text("Contact_TooFast"), false);
            }
            else if (TextBox_Question.Text.Trim() != (Session[SessionAnswerKey] as string))
            {
                // a wrong answer draws a new sum (a robot cannot try the same picture again)
                PickNewQuestion();
                AdminUi.Toast(this, message_error, false);
            }
            else if (!EmailAddressCheck.IsValid(TextBox_EMail.Text))
            {
                AdminUi.Toast(this, message_email_error, false);
            }
            else if (TooManyMessagesFromThisAddress())
            {
                AdminUi.Toast(this, Global.Text("Contact_TooMany"), false);
            }
            else if (!Send_Message())
            {
                // the message was NOT stored: the visitor is told (the confirmation page used to come up even then) and keeps what was typed
                AdminUi.Toast(this, Global.Text("Contact_SendFailed"), false);
            }
            else
            {
                // fire-and-forget: the SMTP round-trip can take several seconds and the visitor doesn't need to wait on it, the message
                // is already stored, this e-mail is only a notification to the owner. Everything it says is read here, on the request.
                string subject = mail_subject + " : " + Mailer.OneLine(TextBox_FirstName.Text + " " + TextBox_LastName.Text);
                string body = NotificationBody();
                string replyTo = TextBox_EMail.Text.Trim();

                System.Threading.Tasks.Task.Run(() => Send_Email(subject, body, replyTo));
                PickNewQuestion();
                Response.Redirect(ResolveUrl("~/confirmation.aspx"));
            }

            TextBox_Question.Text = string.Empty;
        }

        private void Load_Languages()
        {
            // the lists are filled once: on a postback the page already has them from its view state
            if (DropDownList_Type.Items.Count == 0)
            {
                DropDownList_Type.Items.Add(Global.Contact_ProjectType1);
                DropDownList_Type.Items.Add(Global.Contact_ProjectType2);
                DropDownList_Type.Items.Add(Global.Contact_ProjectType3);
                DropDownList_Type.Items.Add(Global.Contact_ProjectType4);
                DropDownList_Type.Items.Add(Global.Contact_ProjectType5);
                DropDownList_Budjet.Items.Add(Global.Contact_Budget1);
                DropDownList_Budjet.Items.Add(Global.Contact_Budget2);
                DropDownList_Budjet.Items.Add(Global.Contact_Budget3);
                DropDownList_Budjet.Items.Add(Global.Contact_Budget4);
                DropDownList_Deadline.Items.Add(Global.Contact_Deadline1);
                DropDownList_Deadline.Items.Add(Global.Contact_Deadline2);
                DropDownList_Deadline.Items.Add(Global.Contact_Deadline3);
            }

            Button_Send.Text = Global.Contact_SendButton;
            message_error = Global.Contact_ErrorMessage;
            message_email_error = Global.Contact_EmailError;
            mail_subject = Global.Contact_MailSubject;
        }

        // more than MaxMessagesPerHour messages stored from this address during the last hour (a failure to count never blocks a visitor)
        private bool TooManyMessagesFromThisAddress()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM CONTACT WHERE IP_ADDRESS = @IP_ADDRESS AND CAST([DATE] AS datetime) + CAST([TIME] AS datetime) >= @SINCE", connection))
                {
                    cmd.Parameters.Add("@IP_ADDRESS", SqlDbType.VarChar, 50).Value = User_IP.Get_UserIP();
                    cmd.Parameters.Add("@SINCE", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-1);
                    connection.Open();

                    return Convert.ToInt32(cmd.ExecuteScalar()) >= MaxMessagesPerHour;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "contact form limit");
                return false;
            }
        }

        // true when the message is stored in the CONTACT table
        private bool Send_Message()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "INSERT INTO CONTACT (DATE, TIME, IP_ADDRESS, LAST_NAME, FIRST_NAME, EMAIL, PHONE, COMPANY, PROJECT_TYPE, " +
                        "PROJECT_BUDGET, PROJECT_DEADLINE, DESCRIPTION) VALUES (@DATE, @TIME, @IP_ADDRESS, @LAST_NAME, @FIRST_NAME, @EMAIL, @PHONE, @COMPANY, " +
                        "@PROJECT_TYPE, @PROJECT_BUDGET, @PROJECT_DEADLINE, @DESCRIPTION)";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.Date);
                        cmd.Parameters.AddWithValue("@TIME", DateTime.Now.TimeOfDay);
                        cmd.Parameters.AddWithValue("@IP_ADDRESS", User_IP.Get_UserIP());
                        // parameters need no doubled apostrophes (D'Amours used to be stored as D''Amours)
                        cmd.Parameters.AddWithValue("@LAST_NAME", TextBox_LastName.Text);
                        cmd.Parameters.AddWithValue("@FIRST_NAME", TextBox_FirstName.Text);
                        cmd.Parameters.AddWithValue("@EMAIL", TextBox_EMail.Text.Trim());
                        cmd.Parameters.AddWithValue("@PHONE", TextBox_Phone.Text);
                        cmd.Parameters.AddWithValue("@COMPANY", TextBox_Company.Text);
                        cmd.Parameters.AddWithValue("@PROJECT_TYPE", DropDownList_Type.SelectedValue);
                        cmd.Parameters.AddWithValue("@PROJECT_BUDGET", DropDownList_Budjet.SelectedValue);
                        cmd.Parameters.AddWithValue("@PROJECT_DEADLINE", DropDownList_Deadline.SelectedValue);
                        cmd.Parameters.AddWithValue("@DESCRIPTION", TextBox_Description.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // the details go to the error log (App_Data\logs), never to the visitor
                ErrorLogger.Log(ex, "contact form");
                return false;
            }
        }

        // what the owner reads in the notification: who wrote, how to answer, the project and the message (the owner answers from the
        // mailbox with "Reply": the visitor's address is the reply address). Plain text, so nothing typed by the visitor can be markup.
        private string NotificationBody()
        {
            var body = new StringBuilder();
            body.AppendLine(Mailer.OneLine(TextBox_FirstName.Text + " " + TextBox_LastName.Text));

            if (!string.IsNullOrWhiteSpace(TextBox_Company.Text))
            {
                body.AppendLine(Mailer.OneLine(TextBox_Company.Text));
            }

            body.AppendLine(Mailer.OneLine(TextBox_EMail.Text) + "  |  " + Mailer.OneLine(TextBox_Phone.Text));
            body.AppendLine();
            body.AppendLine(DropDownList_Type.SelectedValue + "  |  " + DropDownList_Budjet.SelectedValue + "  |  " + DropDownList_Deadline.SelectedValue);
            body.AppendLine();
            body.AppendLine(TextBox_Description.Text);

            return body.ToString();
        }

        private void Send_Email(string subject, string body, string replyTo)
        {
            try
            {
                Mailer.Send(Mailer.FromAddress, subject, body, false, replyTo);
            }
            catch (Exception ex)
            {
                // runs on a background thread after the visitor has already been redirected to the confirmation page: there is no
                // response left to write to, and the message itself is already stored, so a failed notification is only logged
                ErrorLogger.Log(ex, "contact notification e-mail");
            }
        }
    }
}
