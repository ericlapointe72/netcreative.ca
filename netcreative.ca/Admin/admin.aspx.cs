using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net.Mime;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class admin : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        //CONTACT
        private string contact_menu = string.Empty;
        private bool contact_result = false;
        private string contact_message_exist = string.Empty;
        private string contact_message_delete = string.Empty;
        //LOGIN
        private string login_menu = string.Empty;
        private bool login_result = false;
        private string login_message_exist = string.Empty;
        private string login_message_delete = string.Empty;
        //NEWSLETTER
        private string newsletter_menu = string.Empty;
        private string newsletter_message_test = string.Empty;
        private string newsletter_message_newsletter = string.Empty;
        private string newsletter_message_unsubscribe1 = string.Empty;
        private string newsletter_message_unsubscribe2 = string.Empty;
        private string newsletter_message_unsubscribe3 = string.Empty;
        //OPENING
        private string opening_menu = string.Empty;
        private string opening_message_save = string.Empty;
        //SUBSCRIBER
        private string subscriber_menu = string.Empty;
        private bool subscriber_result = false;
        private string subscriber_message_exist = string.Empty;
        private string subscriber_message_delete = string.Empty;
        //USER
        private string user_menu = string.Empty;
        private bool user_exist = false;
        private string user_message_use = string.Empty;
        private string user_message_save = string.Empty;
        private string user_message_delete = string.Empty;
        //SQL
        private string sql_menu = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((string)Session["role"]) || Session["role"].ToString() != "admin")
            {
                Response.Redirect(ResolveUrl("~/default.aspx"));
            }

            Global.SetCulture(Session["language"].ToString());

            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Load_Languages();

            if (!IsPostBack)
            {
                MultiView_Admin.ActiveViewIndex = 0;
                H1_Blank.InnerText = Page.Title;
            }
        }

        private void Load_Languages()
        {
            Page.Title = Global.Admin_Title;
            Master.SetPageHero("../Content/images/site/hero-login.jpg", Global.Admin_HeroEyebrow, Global.Admin_Title);
            //CONTACT
            Button_Contact.Text = Global.Admin_ContactButton;
            contact_menu = Global.Admin_ContactMenu;
            Button_Contact_Delete.Text = Global.Admin_Delete;
            contact_message_exist = Global.Format(Global.Admin_ContactExistsError, TextBox_Contact_Delete.Text);
            contact_message_delete = Global.Format(Global.Admin_ContactDeletedSuccess, TextBox_Contact_Delete.Text);
            //LOGIN
            Button_Login.Text = Global.Admin_LoginButton;
            login_menu = Global.Admin_LoginMenu;
            Button_Login_Delete.Text = Global.Admin_Delete;
            login_message_exist = Global.Format(Global.Admin_LoginExistsError, TextBox_Login_Delete.Text);
            login_message_delete = Global.Format(Global.Admin_LoginDeletedSuccess, TextBox_Login_Delete.Text);
            //NEWSLETTER
            Button_Newsletter.Text = Global.Admin_NewsletterButton;
            newsletter_menu = Global.Admin_NewsletterMenu;
            RadioButton_Newsletter_Test.Text = Global.Admin_NewsletterTestRadio;
            RadioButton_Newsletter.Text = Global.Admin_NewsletterButton;
            Label_Newsletter_Email.Text = Global.Admin_NewsletterEmailLabel;
            Button_Newsletter_Send.Text = Global.Admin_NewsletterSendTestButton;
            newsletter_message_test = Global.Admin_NewsletterTestSentSuccess;
            newsletter_message_newsletter = Global.Admin_NewsletterSentSuccess;
            newsletter_message_unsubscribe1 = Global.Admin_NewsletterUnsubscribe1;
            newsletter_message_unsubscribe2 = Global.Admin_NewsletterUnsubscribe2;
            newsletter_message_unsubscribe3 = Global.Admin_NewsletterUnsubscribe3;
            //OPENING
            Button_Opening.Text = Global.Admin_OpeningButton;
            opening_menu = Global.Admin_OpeningMenu;
            Button_Opening_Save.Text = Global.Admin_SaveButton;
            opening_message_save = Global.Admin_OpeningSavedSuccess;
            //SUBSCRIBER
            Button_Subscriber.Text = Global.Admin_SubscriberButton;
            subscriber_menu = Global.Admin_SubscriberMenu;
            Button_Subscriber_Delete.Text = Global.Admin_Delete;
            subscriber_message_exist = Global.Format(Global.Admin_SubscriberExistsError, TextBox_Subscriber_Delete.Text);
            subscriber_message_delete = Global.Format(Global.Admin_SubscriberDeletedSuccess, TextBox_Subscriber_Delete.Text);
            //USER
            Button_User.Text = Global.Admin_UserButton;
            user_menu = Global.Admin_UserMenu;
            Button_User_Save.Text = Global.Admin_SaveButton;
            Button_User_SaveNew.Text = Global.Admin_SaveButton;
            Button_User_New.Text = Global.Admin_AddButton;
            Button_User_Delete.Text = Global.Admin_Delete;
            user_message_use = Global.Format(Global.Admin_UserAlreadyUsedError, TextBox_User_Name.Text);
            user_message_save = Global.Format(Global.Admin_UserSavedSuccess, TextBox_User_Name.Text);
            user_message_delete = Global.Format(Global.Admin_UserDeletedSuccess, TextBox_User_Name.Text);
            //SQL
            Button_Sql.Text = Global.Admin_SqlButton;
            sql_menu = Global.Admin_SqlMenu;
            TextBox_Sql_Query.Attributes["placeholder"] = Global.Admin_SqlPlaceholder;
            Button_Sql_Execute.Text = Global.Admin_SqlExecuteButton;
            Button_Sql_Execute.OnClientClick = "return confirm('" + Global.Admin_SqlConfirmMessage.Replace("'", "\\'") + "');";
        }

        //CONTACT
        private void Contact_Load()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM CONTACT";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        connection.Open();

                        SqlDataReader dr = cmd.ExecuteReader();
                        List<Contact> contact = new List<Contact>();

                        while (dr.Read())
                        {
                            Contact c = new Contact();
                            c.line1 = dr["AUTO_NUMBER"].ToString() + " | " + ((DateTime)dr["DATE"]).ToString("yyyy-MM-dd") + " | " + ((TimeSpan)dr["TIME"]).ToString(@"hh\:mm\:ss");
                            c.line2 = dr["LAST_NAME"].ToString() + ", " + dr["FIRST_NAME"].ToString();
                            c.line3 = dr["EMAIL"].ToString() + " | " + dr["PHONE"].ToString();
                            c.line4 = dr["COMPANY"].ToString();
                            c.line5 = dr["PROJECT_TYPE"].ToString() + " | " + dr["PROJECT_BUDGET"].ToString() + " | " + dr["PROJECT_DEADLINE"].ToString();
                            c.line6 = dr["DESCRIPTION"].ToString();
                            contact.Add(c);
                        }

                        Repeater.DataSource = contact;
                        Repeater.DataBind();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Contact_VerifyIfExist()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM CONTACT WHERE AUTO_NUMBER=@NUMBER";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@NUMBER", TextBox_Contact_Delete.Text);

                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            contact_result = true;
                        }
                        else
                        {
                            contact_result = false;
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Contact_Delete()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "DELETE FROM CONTACT WHERE AUTO_NUMBER=@NUMBER";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@NUMBER", TextBox_Contact_Delete.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        Response.Write("<script>showToast('" + contact_message_delete + "', 'success');</script>");
                        Contact_Load();
                        TextBox_Contact_Delete.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        protected void Button_Contact_Delete_Click(object sender, EventArgs e)
        {
            Contact_VerifyIfExist();

            if (contact_result == true)
            {
                Contact_Delete();
            }
            else
            {
                Response.Write("<script>showToast('" + contact_message_exist + "', 'error');</script>");
            }
        }

        //LOGIN
        private void Login_Load()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM LOGIN";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        connection.Open();

                        SqlDataReader dr = cmd.ExecuteReader();
                        List<Login> login = new List<Login>();

                        while (dr.Read())
                        {
                            Login l = new Login();
                            l.Number = dr["AUTO_NUMBER"].ToString();
                            l.Date = ((DateTime)dr["DATE"]).ToString("yyyy-MM-dd");
                            l.Time = ((TimeSpan)dr["TIME"]).ToString(@"hh\:mm\:ss");
                            l.IP_Address = dr["IP_ADDRESS"].ToString();
                            l.User = dr["USER_NAME"].ToString();
                            l.Action = dr["ACTION"].ToString();
                            login.Add(l);
                        }

                        GridView_Login.DataSource = login;
                        GridView_Login.DataBind();

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Login_VerifyIfExist()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM LOGIN WHERE AUTO_NUMBER=@NUMBER";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@NUMBER", TextBox_Login_Delete.Text);

                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            login_result = true;
                        }
                        else
                        {
                            login_result = false;
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Login_Delete()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "DELETE FROM LOGIN WHERE AUTO_NUMBER=@NUMBER";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@NUMBER", TextBox_Login_Delete.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        Response.Write("<script>showToast('" + login_message_delete + "', 'success');</script>");
                        Login_Load();
                        TextBox_Login_Delete.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        protected void Button_Login_Delete_Click(object sender, EventArgs e)
        {
            Login_VerifyIfExist();

            if (login_result == true)
            {
                Login_Delete();
            }
            else
            {
                Response.Write("<script>showToast('" + login_message_exist + "', 'error');</script>");
            }
        }

        //NEWSLETTER
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

        //OPENING
        private void Opening_Load()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM OPENING";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        connection.Open();

                        SqlDataReader dr = cmd.ExecuteReader();

                        dr.Read();
                        mon_oh.Text = dr["MON"].ToString();
                        tue_oh.Text = dr["TUE"].ToString();
                        wed_oh.Text = dr["WED"].ToString();
                        thu_oh.Text = dr["THU"].ToString();
                        fri_oh.Text = dr["FRI"].ToString();
                        sat_oh.Text = dr["SAT"].ToString();
                        sun_oh.Text = dr["SUN"].ToString();

                        dr.Read();
                        mon_om.Text = dr["MON"].ToString();
                        tue_om.Text = dr["TUE"].ToString();
                        wed_om.Text = dr["WED"].ToString();
                        thu_om.Text = dr["THU"].ToString();
                        fri_om.Text = dr["FRI"].ToString();
                        sat_om.Text = dr["SAT"].ToString();
                        sun_om.Text = dr["SUN"].ToString();

                        dr.Read();
                        mon_ch.Text = dr["MON"].ToString();
                        tue_ch.Text = dr["TUE"].ToString();
                        wed_ch.Text = dr["WED"].ToString();
                        thu_ch.Text = dr["THU"].ToString();
                        fri_ch.Text = dr["FRI"].ToString();
                        sat_ch.Text = dr["SAT"].ToString();
                        sun_ch.Text = dr["SUN"].ToString();

                        dr.Read();
                        mon_cm.Text = dr["MON"].ToString();
                        tue_cm.Text = dr["TUE"].ToString();
                        wed_cm.Text = dr["WED"].ToString();
                        thu_cm.Text = dr["THU"].ToString();
                        fri_cm.Text = dr["FRI"].ToString();
                        sat_cm.Text = dr["SAT"].ToString();
                        sun_cm.Text = dr["SUN"].ToString();

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Opening_Save()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand1 = "UPDATE OPENING SET MON=@MON, TUE=@TUE, WED=@WED, THU=@THU, FRI=@FRI, SAT=@SAT, SUN=@SUN WHERE OPEN_CLOSE='OPEN_HOUR'";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand1, connection))
                    {
                        cmd.Parameters.AddWithValue("@MON", mon_oh.Text);
                        cmd.Parameters.AddWithValue("@TUE", tue_oh.Text);
                        cmd.Parameters.AddWithValue("@WED", wed_oh.Text);
                        cmd.Parameters.AddWithValue("@THU", thu_oh.Text);
                        cmd.Parameters.AddWithValue("@FRI", fri_oh.Text);
                        cmd.Parameters.AddWithValue("@SAT", sat_oh.Text);
                        cmd.Parameters.AddWithValue("@SUN", sun_oh.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }

                    string sqlCommand2 = "UPDATE OPENING SET MON=@MON, TUE=@TUE, WED=@WED, THU=@THU, FRI=@FRI, SAT=@SAT, SUN=@SUN WHERE OPEN_CLOSE='OPEN_MIN'";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand2, connection))
                    {
                        cmd.Parameters.AddWithValue("@MON", string.Format("{0:00}", Convert.ToInt32(mon_om.Text)));
                        cmd.Parameters.AddWithValue("@TUE", string.Format("{0:00}", Convert.ToInt32(tue_om.Text)));
                        cmd.Parameters.AddWithValue("@WED", string.Format("{0:00}", Convert.ToInt32(wed_om.Text)));
                        cmd.Parameters.AddWithValue("@THU", string.Format("{0:00}", Convert.ToInt32(thu_om.Text)));
                        cmd.Parameters.AddWithValue("@FRI", string.Format("{0:00}", Convert.ToInt32(fri_om.Text)));
                        cmd.Parameters.AddWithValue("@SAT", string.Format("{0:00}", Convert.ToInt32(sat_om.Text)));
                        cmd.Parameters.AddWithValue("@SUN", string.Format("{0:00}", Convert.ToInt32(sun_om.Text)));

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }

                    string sqlCommand3 = "UPDATE OPENING SET MON=@MON, TUE=@TUE, WED=@WED, THU=@THU, FRI=@FRI, SAT=@SAT, SUN=@SUN WHERE OPEN_CLOSE='CLOSE_HOUR'";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand3, connection))
                    {
                        cmd.Parameters.AddWithValue("@MON", mon_ch.Text);
                        cmd.Parameters.AddWithValue("@TUE", tue_ch.Text);
                        cmd.Parameters.AddWithValue("@WED", wed_ch.Text);
                        cmd.Parameters.AddWithValue("@THU", thu_ch.Text);
                        cmd.Parameters.AddWithValue("@FRI", fri_ch.Text);
                        cmd.Parameters.AddWithValue("@SAT", sat_ch.Text);
                        cmd.Parameters.AddWithValue("@SUN", sun_ch.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }

                    string sqlCommand4 = "UPDATE OPENING SET MON=@MON, TUE=@TUE, WED=@WED, THU=@THU, FRI=@FRI, SAT=@SAT, SUN=@SUN WHERE OPEN_CLOSE='CLOSE_MIN'";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand4, connection))
                    {
                        cmd.Parameters.AddWithValue("@MON", string.Format("{0:00}", Convert.ToInt32(mon_cm.Text)));
                        cmd.Parameters.AddWithValue("@TUE", string.Format("{0:00}", Convert.ToInt32(tue_cm.Text)));
                        cmd.Parameters.AddWithValue("@WED", string.Format("{0:00}", Convert.ToInt32(wed_cm.Text)));
                        cmd.Parameters.AddWithValue("@THU", string.Format("{0:00}", Convert.ToInt32(thu_cm.Text)));
                        cmd.Parameters.AddWithValue("@FRI", string.Format("{0:00}", Convert.ToInt32(fri_cm.Text)));
                        cmd.Parameters.AddWithValue("@SAT", string.Format("{0:00}", Convert.ToInt32(sat_cm.Text)));
                        cmd.Parameters.AddWithValue("@SUN", string.Format("{0:00}", Convert.ToInt32(sun_cm.Text)));

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                Response.Write("<script>showToast('" + opening_message_save + "', 'success');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        protected void Button_Opening_Save_Click(object sender, EventArgs e)
        {
            Opening_Save();
            Opening_Load();
        }

        //SUBSCRIBER
        private void Subscriber_Load()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM SUBSCRIBE";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        connection.Open();

                        SqlDataReader dr = cmd.ExecuteReader();
                        List<Subscribe> subscribes = new List<Subscribe>();

                        while (dr.Read())
                        {
                            Subscribe s = new Subscribe();
                            s.Number = dr["AUTO_NUMBER"].ToString();
                            s.Date = dr["DATE"].ToString();
                            s.Time = dr["TIME"].ToString();
                            s.IP_Address = dr["IP_ADDRESS"].ToString();
                            s.Email = dr["EMAIL"].ToString();
                            subscribes.Add(s);
                        }

                        GridView_Subscribe.DataSource = subscribes;
                        GridView_Subscribe.DataBind();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Subscriber_VerifyIfExist()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM SUBSCRIBE WHERE AUTO_NUMBER=@AUTO_NUMBER";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@AUTO_NUMBER", TextBox_Subscriber_Delete.Text);

                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            subscriber_result = true;
                        }
                        else
                        {
                            subscriber_result = false;
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Subscriber_Delete()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "DELETE FROM SUBSCRIBE WHERE AUTO_NUMBER=@AUTO_NUMBER";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@AUTO_NUMBER", TextBox_Subscriber_Delete.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        Response.Write("<script>showToast('" + subscriber_message_delete + "', 'success');</script>");
                        Subscriber_Load();
                        TextBox_Subscriber_Delete.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        protected void Button_Subscriber_Delete_Click(object sender, EventArgs e)
        {
            Subscriber_VerifyIfExist();

            if (subscriber_result == true)
            {
                Subscriber_Delete();
            }
            else
            {
                Response.Write("<script>showToast('" + subscriber_message_exist + "', 'error');</script>");
            }
        }

        //USER
        private void User_LoadList()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT USER_NAME FROM USERS";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        DropDown_User.Items.Clear();

                        while (dr.Read())
                        {
                            DropDown_User.Items.Add(dr["USER_NAME"].ToString());
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void User_Load()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM USERS WHERE USER_NAME=@USER_NAME";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@USER_NAME", DropDown_User.SelectedValue);

                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        dr.Read();

                        TextBox_User_Name.Text = dr["USER_NAME"].ToString();
                        TextBox_User_Name.ReadOnly = true;
                        TextBox_User_Password.Attributes.Add("value", string.Empty);
                        TextBox_User_LastName.Text = dr["LAST_NAME"].ToString();
                        TextBox_User_FirstName.Text = dr["FIRST_NAME"].ToString();

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void User_Save()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    bool changePassword = !string.IsNullOrEmpty(TextBox_User_Password.Text);

                    string sqlCommand =
                        (changePassword ? "UPDATE USERS SET PASSWORD=@PASSWORD, FIRST_NAME=@FIRST_NAME, LAST_NAME=@LAST_NAME " :
                        "UPDATE USERS SET FIRST_NAME=@FIRST_NAME, LAST_NAME=@LAST_NAME ") +
                        "WHERE USER_NAME=@USER_NAME";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@USER_NAME", TextBox_User_Name.Text);

                        if (changePassword)
                        {
                            cmd.Parameters.AddWithValue("@PASSWORD", PasswordHasher.Hash(TextBox_User_Password.Text));
                        }

                        cmd.Parameters.AddWithValue("@LAST_NAME", TextBox_User_LastName.Text.Replace("'", "''"));
                        cmd.Parameters.AddWithValue("@FIRST_NAME", TextBox_User_FirstName.Text.Replace("'", "''"));

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                        User_Load();

                        Response.Write("<script>showToast('" + user_message_save + "', 'success');</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void User_SaveNew()
        {
            User_VerifyIfExist();

            if (user_exist == true)
            {
                Response.Write("<script>showToast('" + user_message_use + "', 'error');</script>");
                TextBox_User_Name.Focus();
            }
            else
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string sqlCommand =
                            "INSERT INTO USERS (USER_NAME, PASSWORD, FIRST_NAME, LAST_NAME) " +
                            "VALUES (@USER_NAME, @PASSWORD, @FIRST_NAME, @LAST_NAME)";

                        using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                        {
                            cmd.Parameters.AddWithValue("@USER_NAME", TextBox_User_Name.Text);
                            cmd.Parameters.AddWithValue("@PASSWORD", PasswordHasher.Hash(TextBox_User_Password.Text));
                            cmd.Parameters.AddWithValue("@LAST_NAME", TextBox_User_LastName.Text);
                            cmd.Parameters.AddWithValue("@FIRST_NAME", TextBox_User_FirstName.Text);

                            connection.Open();
                            cmd.ExecuteNonQuery();
                            connection.Close();

                            User_LoadList();
                            User_Load();
                            Button_User_Save.Visible = true;
                            Button_User_SaveNew.Visible = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
                }
            }
        }

        private void User_New()
        {
            Button_User_Save.Visible = false;
            Button_User_SaveNew.Visible = true;
            TextBox_User_Name.ReadOnly = false;
            TextBox_User_Name.Text = string.Empty;
            TextBox_User_Password.Attributes.Add("value", string.Empty);
            TextBox_User_LastName.Text = string.Empty;
            TextBox_User_FirstName.Text = string.Empty;
            TextBox_User_Name.Focus();
        }

        private void User_VerifyIfExist()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM USERS WHERE USER_NAME=@USER";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@USER", TextBox_User_Name.Text);

                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            user_exist = true;
                        }
                        else
                        {
                            user_exist = false;
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void User_Delete()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "DELETE FROM USERS WHERE USER_NAME=@USER_NAME";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@USER_NAME", TextBox_User_Name.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                        User_LoadList();
                        User_Load();

                        Response.Write("<script>showToast('" + user_message_delete + "', 'success');</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        protected void DropDown_User_SelectedIndexChanged(object sender, EventArgs e)
        {
            User_Load();
        }

        protected void Button_User_Save_Click(object sender, EventArgs e)
        {
            User_Save();
        }

        protected void Button_User_SaveNew_Click(object sender, EventArgs e)
        {
            User_SaveNew();
        }

        protected void Button_User_New_Click(object sender, EventArgs e)
        {
            User_New();
        }

        protected void Button_User_Delete_Click(object sender, EventArgs e)
        {
            User_Delete();
        }


        protected void Button_Contact_Click(object sender, EventArgs e)
        {
            MultiView_Admin.ActiveViewIndex = 1;
            H1_Contact.InnerText = contact_menu;
            Contact_Load();
        }

        protected void Button_Login_Click(object sender, EventArgs e)
        {
            MultiView_Admin.ActiveViewIndex = 2;
            H1_Login.InnerText = login_menu;
            Login_Load();
        }

        protected void Button_Newsletter_Click(object sender, EventArgs e)
        {
            MultiView_Admin.ActiveViewIndex = 3;
            H1_Newsletter.InnerText = newsletter_menu;
        }

        protected void Button_Opening_Click(object sender, EventArgs e)
        {
            MultiView_Admin.ActiveViewIndex = 4;
            H1_Opening.InnerText = opening_menu;
            Opening_Load();
        }

        protected void Button_Subscriber_Click(object sender, EventArgs e)
        {
            MultiView_Admin.ActiveViewIndex = 5;
            H1_Subscriber.InnerText = subscriber_menu;
            Subscriber_Load();
        }

        protected void Button_User_Click(object sender, EventArgs e)
        {
            MultiView_Admin.ActiveViewIndex = 6;
            H1_User.InnerText = user_menu;
            Button_User_SaveNew.Visible = false;
            User_LoadList();
            User_Load();
        }

        protected void Button_Sql_Click(object sender, EventArgs e)
        {
            MultiView_Admin.ActiveViewIndex = 7;
            H1_Sql.InnerText = sql_menu;
            Label_Sql_Status.Text = string.Empty;
            GridView_Sql_Result.DataSource = null;
            GridView_Sql_Result.DataBind();
        }

        protected void Button_Sql_Execute_Click(object sender, EventArgs e)
        {
            string query = TextBox_Sql_Query.Text.Trim();

            if (string.IsNullOrEmpty(query))
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        if (query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                        {
                            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                            {
                                DataTable table = new DataTable();
                                adapter.Fill(table);

                                GridView_Sql_Result.DataSource = table;
                                GridView_Sql_Result.DataBind();
                                Label_Sql_Status.Text = Global.Format(Global.Admin_SqlRowsReturned, table.Rows.Count);
                            }
                        }
                        else
                        {
                            int affected = cmd.ExecuteNonQuery();

                            GridView_Sql_Result.DataSource = null;
                            GridView_Sql_Result.DataBind();
                            Label_Sql_Status.Text = Global.Format(Global.Admin_SqlRowsAffected, affected);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GridView_Sql_Result.DataSource = null;
                GridView_Sql_Result.DataBind();
                Label_Sql_Status.Text = ex.Message;
            }
        }
    }
}