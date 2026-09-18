using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class contact : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private string mail_subject = string.Empty;
        private string mail_body = string.Empty;
        private string message_succes = string.Empty;
        private string message_error = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((string)Session["language"]))
            {
                Session["language"] = ConfigurationManager.AppSettings["app_language"].ToString();
            }

            Global.SetCulture(Session["language"].ToString());

            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Image_QuestionFR.Visible = false;
            Image_QuestionEN.Visible = false;
            Load_Languages();

            if (User_IP.Get_UserIP() != "207.167.217.203" && User_IP.Get_UserIP() != "216.208.120.130")
            {
                if (!IsPostBack)
                {
                    VerifyIfDayExist();
                    WriteVisitorInfo();
                }
            }
        }

        protected void Button_Send_Click(object sender, EventArgs e)
        {
            if (TextBox_Question.Text.Trim() == "6")
            {
                Send_Message();
                Response.Write("<script>alert('" + message_succes + "');</script>");
                Send_Email();

            }
            else
            {
                Response.Write("<script>alert('" + message_error + "');</script>");
            }

            TextBox_Question.Text = string.Empty;
        }

        private void Load_Languages()
        {
            Page.Title = Global.Contact_Title;
            Master.SetPageHero("Imgs_Site/hero-contact.jpg", Global.Contact_HeroEyebrow, Global.Contact_H1Title, Global.Contact_HeroText);
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
            message_succes = Global.Contact_SuccessMessage;
            message_error = Global.Contact_ErrorMessage;
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
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.ToString("yyyy/MM/dd"));
                        cmd.Parameters.AddWithValue("@TIME", DateTime.Now.ToLongTimeString());
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
                Response.Write("<script>alert('" + ex.Message + "');</script>");
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
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }

        private void VerifyIfDayExist()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM NAVIGATION WHERE DATE=@DATE";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.ToString("yyyy/MM/dd"));

                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        dr.Read();

                        if (dr.HasRows)
                        {
                            UpdateDay();
                        }
                        else
                        {
                            InsertDay();
                            UpdateDay();
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }

        private void InsertDay()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "INSERT INTO NAVIGATION (DATE, HOME, SERVICE, PORTFOLIO, CONTACT, TOTAL) " +
                        "VALUES (@DATE, @HOME, @SERVICE, @PORTFOLIO, @CONTACT, @TOTAL)";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.ToString("yyyy/MM/dd"));
                        cmd.Parameters.AddWithValue("@HOME", 0);
                        cmd.Parameters.AddWithValue("@SERVICE", 0);
                        cmd.Parameters.AddWithValue("@PORTFOLIO", 0);
                        cmd.Parameters.AddWithValue("@CONTACT", 0);
                        cmd.Parameters.AddWithValue("@TOTAL", 0);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }

        private void UpdateDay()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "UPDATE NAVIGATION SET CONTACT=CONTACT+1, TOTAL=TOTAL+1 WHERE DATE=@DATE";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.ToString("yyyy/MM/dd"));

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Open();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }

        private void WriteVisitorInfo()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "INSERT INTO VISITOR (DATE, TIME, IP_ADDRESS, PAGE) VALUES (@DATE, @TIME, @IP_ADDRESS, @PAGE)";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.ToString("yyyy/MM/dd"));
                        cmd.Parameters.AddWithValue("@TIME", DateTime.Now.ToLongTimeString());
                        cmd.Parameters.AddWithValue("@IP_ADDRESS", User_IP.Get_UserIP());
                        cmd.Parameters.AddWithValue("@PAGE", "CONTACT");

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }
    }
}