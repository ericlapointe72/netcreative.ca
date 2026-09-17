using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        private string connectionString = string.Empty;
        private string mail_subject = string.Empty;
        private string mail_body = string.Empty;
        private string message_subscriber_error = string.Empty;
        private string message_subscriber_exist = string.Empty;
        private string message_subscriber_done = string.Empty;
        
        private bool subscriber = true;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string queryLanguage = Request.QueryString["lang"];

            if (queryLanguage == "fr" || queryLanguage == "en")
            {
                Session["language"] = queryLanguage;
            }
            else if (string.IsNullOrEmpty((string)Session["language"]))
            {
                Session["language"] = ConfigurationManager.AppSettings["app_language"].ToString();
            }

            Global.SetCulture(Session["language"].ToString());

            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            Load_Languages();
            Verify_CookieWarning();

            if (string.IsNullOrEmpty((string)Session["animation"]))
            {
                Session["animation"] = "1";
            }

            HideShow_Animation();

            if (string.IsNullOrEmpty((string)Session["role"]))
            {
                Image_Login.Visible = true;
                HyperLink_Login.Visible = true;
                Image_Logout.Visible = false;
                LinkButton_Logout.Visible = false;
                LinkButton_Hello.Visible = false;
            }
            else if (Session["role"].Equals("admin"))
            {
                Image_Login.Visible = false;
                HyperLink_Login.Visible = false;
                Image_Logout.Visible = true;
                LinkButton_Logout.Visible = true;
                LinkButton_Hello.Text = Session["name"].ToString();
            }
        }

        public void SetPageHero(string imageUrl, string eyebrow, string title)
        {
            PageHero.Attributes["class"] = "page-hero page-hero--photo";
            PageHero.Attributes["style"] = "background-image:url('" + imageUrl + "')";

            Span_PageHeroEyebrow.InnerText = eyebrow;
            Span_PageHeroEyebrow.Visible = true;

            H1_PageHero.InnerText = title;
            H1_PageHero.Visible = true;
        }

        private void HideShow_Animation()
        {
            if (Session["animation"].Equals("1"))
            {
                Label_Animation.Visible = true;
                Session["animation"] = "0";
            }
            else if (Session["animation"].Equals("0"))
            {
                Label_Animation.Visible = false;
            }
        }

        protected void LinkButton_Logo_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }

        protected void LinkButton_Logout_Click(object sender, EventArgs e)
        {
            Logout();

            Image_Login.Visible = true;
            HyperLink_Login.Visible = true;
            Image_Logout.Visible = false;
            LinkButton_Logout.Visible = false;
            LinkButton_Hello.Visible = false;

            Response.Redirect("default.aspx");
        }

        protected void LinkButton_Privacy_Click(object sender, EventArgs e)
        {
            Response.Redirect("privacy.aspx");
        }

        protected void Button_Subscribe_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.CultureInvariant | RegexOptions.Singleline);
            bool isValidEmail = regex.IsMatch(TextBox_Subscribe.Text);

            if (isValidEmail == true)
            {
                SubscriberExist();

                if (subscriber == false)
                {
                    Subscribe();
                    Send_Email();
                    subscriber = true;
                    Label_ErrorMessage.Text = string.Empty;
                }
                else
                {
                    Response.Write("<script>alert('" + message_subscriber_exist + "');</script>");
                }
            }
            else
            {
                Label_ErrorMessage.Text = message_subscriber_error;
            }
        }

        protected void LinkButton_Hello_Click(object sender, EventArgs e)
        {
            Response.Redirect("admin.aspx");
        }

        private void Verify_CookieWarning()
        {
            if (Request.Cookies["netcreative_cookie_accepted"] != null)
            {
                Panel_CookieWarning.Visible = false;
            }
        }

        private void Load_Languages()
        {
            try
            {
                string scheduleSeparator = ": ";
                string timeSeparetor = Global.Master_TimeSeparator;
                string hourSeparator = Global.Master_HourSeparator;
                string timeAm = Global.Master_TimeAm;
                string timePm = Global.Master_TimePm;
                string monday = Global.Master_Monday;
                string tuesday = Global.Master_Tuesday;
                string wednesday = Global.Master_Wednesday;
                string thursday = Global.Master_Thursday;
                string friday = Global.Master_Friday;
                string saturday = Global.Master_Saturday;
                string sunday = Global.Master_Sunday;
                string closed = Global.Master_Closed;
                int timeMod = Session["language"].ToString() == "fr" ? 0 : 12;

                Label_Slogan.Text = Global.Master_Slogan;
                Label_Home1.Text = Global.Master_Home;
                Label_Boutique1.Text = Global.Master_Boutique;
                Label_DutyFreeOps1.Text = Global.Master_DutyFreeOps;
                Label_Service1.Text = Global.Master_Service;
                Label_Portfolio1.Text = Global.Master_Portfolio;
                Label_Contact1.Text = Global.Master_Contact;
                HyperLink_Language.InnerText = Global.Master_LanguageToggle;
                HyperLink_Language.HRef = Request.Url.AbsolutePath + "?lang=" + (Session["language"].ToString() == "fr" ? "en" : "fr");
                Label_Home2.Text = Global.Master_Home;
                Label_Boutique2.Text = Global.Master_Boutique;
                Label_DutyFreeOps2.Text = Global.Master_DutyFreeOps;
                Label_Service2.Text = Global.Master_Service;
                Label_Portfolio2.Text = Global.Master_Portfolio;
                Label_Contact2.Text = Global.Master_Contact;
                Label_Menu.Text = Global.Master_Menu;
                Label_Opening.Text = Global.Master_OpeningHours;
                Label_Newsletter.Text = Global.Master_Newsletter;
                Label_Subscribe.Text = Global.Master_SubscribeText;
                Button_Subscribe.Text = Global.Master_SubscribeButton;
                message_subscriber_error = Global.Master_SubscribeErrorInvalid;
                message_subscriber_exist = Global.Master_SubscribeErrorExists;
                message_subscriber_done = Global.Master_SubscribeDone;
                Label_Contact.Text = Global.Master_Contact;
                Label_Footer.Text = Global.Format(Global.Master_FooterText, DateTime.Now.Year);
                LinkButton_Privacy.Text = Global.Master_PrivacyPolicy;
                TextBox_Subscribe.Attributes.Add("placeholder", Global.Master_SubscribePlaceholder);
                Label_CookieWarning.Text = Global.Master_CookieWarningText;
                HyperLink_CookieReadMore.InnerText = Global.Master_CookieReadMore;
                Button_CookieAccept.InnerText = Global.Master_CookieAccept;
                mail_subject = Global.Master_MailSubjectNewSubscriber;
                mail_body = Global.Master_MailBodyNewSubscriber;

                string oh_sun, oh_mon, oh_tue, oh_wed, oh_thu, oh_fri, oh_sat;
                string om_sun, om_mon, om_tue, om_wed, om_thu, om_fri, om_sat;
                string ch_sun, ch_mon, ch_tue, ch_wed, ch_thu, ch_fri, ch_sat;
                string cm_sun, cm_mon, cm_tue, cm_wed, cm_thu, cm_fri, cm_sat;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM OPENING";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        dr.Read();
                        oh_sun = dr["SUN"].ToString();
                        oh_mon = dr["MON"].ToString();
                        oh_tue = dr["TUE"].ToString();
                        oh_wed = dr["WED"].ToString();
                        oh_thu = dr["THU"].ToString();
                        oh_fri = dr["FRI"].ToString();
                        oh_sat = dr["SAT"].ToString();

                        dr.Read();
                        om_sun = dr["SUN"].ToString();
                        om_mon = dr["MON"].ToString();
                        om_tue = dr["TUE"].ToString();
                        om_wed = dr["WED"].ToString();
                        om_thu = dr["THU"].ToString();
                        om_fri = dr["FRI"].ToString();
                        om_sat = dr["SAT"].ToString();

                        dr.Read();
                        ch_sun = dr["SUN"].ToString();
                        ch_mon = dr["MON"].ToString();
                        ch_tue = dr["TUE"].ToString();
                        ch_wed = dr["WED"].ToString();
                        ch_thu = dr["THU"].ToString();
                        ch_fri = dr["FRI"].ToString();
                        ch_sat = dr["SAT"].ToString();

                        dr.Read();
                        cm_sun = dr["SUN"].ToString();
                        cm_mon = dr["MON"].ToString();
                        cm_tue = dr["TUE"].ToString();
                        cm_wed = dr["WED"].ToString();
                        cm_thu = dr["THU"].ToString();
                        cm_fri = dr["FRI"].ToString();
                        cm_sat = dr["SAT"].ToString();

                        connection.Close();

                        string hourSign1, hourSign2;

                        if (Convert.ToInt32(oh_mon) != 0)
                        {
                            if (Convert.ToInt32(oh_mon) < 12)
                            {
                                hourSign1 = timeAm;
                            }
                            else
                            {
                                hourSign1 = timePm;
                                oh_mon = (Convert.ToInt32(oh_mon) - timeMod).ToString();
                            }

                            if (Convert.ToInt32(ch_mon) < 12)
                            {
                                hourSign2 = timeAm;
                            }
                            else
                            {
                                hourSign2 = timePm;
                                ch_mon = (Convert.ToInt32(ch_mon) - timeMod).ToString();
                            }

                            Label_Monday.Text = monday + scheduleSeparator + oh_mon + timeSeparetor + om_mon + hourSign1 + hourSeparator + ch_mon + timeSeparetor + cm_mon + hourSign2;
                        }
                        else
                        {
                            Label_Monday.Text = monday + scheduleSeparator + closed;
                        }

                        if (Convert.ToInt32(oh_tue) != 0)
                        {
                            if (Convert.ToInt32(oh_tue) < 12)
                            {
                                hourSign1 = timeAm;
                            }
                            else
                            {
                                hourSign1 = timePm;
                                oh_tue = (Convert.ToInt32(oh_tue) - timeMod).ToString();
                            }

                            if (Convert.ToInt32(ch_tue) < 12)
                            {
                                hourSign2 = timeAm;
                            }
                            else
                            {
                                hourSign2 = timePm;
                                ch_tue = (Convert.ToInt32(ch_tue) - timeMod).ToString();
                            }

                            Label_Tuesday.Text = tuesday + scheduleSeparator + oh_tue + timeSeparetor + om_tue + hourSign1 + hourSeparator + ch_tue + timeSeparetor + cm_tue + hourSign2;
                        }
                        else
                        {
                            Label_Tuesday.Text = tuesday + scheduleSeparator + closed;
                        }

                        if (Convert.ToInt32(oh_wed) != 0)
                        {
                            if (Convert.ToInt32(oh_wed) < 12)
                            {
                                hourSign1 = timeAm;
                            }
                            else
                            {
                                hourSign1 = timePm;
                                oh_wed = (Convert.ToInt32(oh_wed) - timeMod).ToString();
                            }

                            if (Convert.ToInt32(ch_wed) < 12)
                            {
                                hourSign2 = timeAm;
                            }
                            else
                            {
                                hourSign2 = timePm;
                                ch_wed = (Convert.ToInt32(ch_wed) - timeMod).ToString();
                            }

                            Label_Wednesday.Text = wednesday + scheduleSeparator + oh_wed + timeSeparetor + om_wed + hourSign1 + hourSeparator + ch_wed + timeSeparetor + cm_wed + hourSign2;
                        }
                        else
                        {
                            Label_Wednesday.Text = wednesday + scheduleSeparator + closed;
                        }

                        if (Convert.ToInt32(oh_thu) != 0)
                        {
                            if (Convert.ToInt32(oh_thu) < 12)
                            {
                                hourSign1 = timeAm;
                            }
                            else
                            {
                                hourSign1 = timePm;
                                oh_thu = (Convert.ToInt32(oh_thu) - timeMod).ToString();
                            }

                            if (Convert.ToInt32(ch_thu) < 12)
                            {
                                hourSign2 = timeAm;
                            }
                            else
                            {
                                hourSign2 = timePm;
                                ch_thu = (Convert.ToInt32(ch_thu) - timeMod).ToString();
                            }

                            Label_Thursday.Text = thursday + scheduleSeparator + oh_thu + timeSeparetor + om_thu + hourSign1 + hourSeparator + ch_thu + timeSeparetor + cm_thu + hourSign2;
                        }
                        else
                        {
                            Label_Thursday.Text = thursday + scheduleSeparator + closed;
                        }

                        if (Convert.ToInt32(oh_fri) != 0)
                        {
                            if (Convert.ToInt32(oh_fri) < 12)
                            {
                                hourSign1 = timeAm;
                            }
                            else
                            {
                                hourSign1 = timePm;
                                oh_fri = (Convert.ToInt32(oh_fri) - timeMod).ToString();
                            }

                            if (Convert.ToInt32(ch_fri) < 12)
                            {
                                hourSign2 = timeAm;
                            }
                            else
                            {
                                hourSign2 = timePm;
                                ch_fri = (Convert.ToInt32(ch_fri) - timeMod).ToString();
                            }

                            Label_Friday.Text = friday + scheduleSeparator + oh_fri + timeSeparetor + om_fri + hourSign1 + hourSeparator + ch_fri + timeSeparetor + cm_fri + hourSign2;
                        }
                        else
                        {
                            Label_Friday.Text = friday + scheduleSeparator + closed;
                        }

                        if (Convert.ToInt32(oh_sat) != 0)
                        {
                            if (Convert.ToInt32(oh_sat) < 12)
                            {
                                hourSign1 = timeAm;
                            }
                            else
                            {
                                hourSign1 = timePm;
                                oh_sat = (Convert.ToInt32(oh_sat) - timeMod).ToString();
                            }

                            if (Convert.ToInt32(ch_sat) < 12)
                            {
                                hourSign2 = timeAm;
                            }
                            else
                            {
                                hourSign2 = timePm;
                                ch_sat = (Convert.ToInt32(ch_sat) - timeMod).ToString();
                            }

                            Label_Saturday.Text = saturday + scheduleSeparator + oh_sat + timeSeparetor + om_sat + hourSign1 + hourSeparator + ch_sat + timeSeparetor + cm_sat + hourSign2;
                        }
                        else
                        {
                            Label_Saturday.Text = saturday + scheduleSeparator + closed;
                        }

                        if (Convert.ToInt32(oh_sun) != 0)
                        {
                            if (Convert.ToInt32(oh_sun) < 12)
                            {
                                hourSign1 = timeAm;
                            }
                            else
                            {
                                hourSign1 = timePm;
                                oh_sun = (Convert.ToInt32(oh_sun) - timeMod).ToString();
                            }

                            if (Convert.ToInt32(ch_sun) < 12)
                            {
                                hourSign2 = timeAm;
                            }
                            else
                            {
                                hourSign2 = timePm;
                                ch_sun = (Convert.ToInt32(ch_sun) - timeMod).ToString();
                            }

                            Label_Sunday.Text = sunday + scheduleSeparator + oh_sun + timeSeparetor + om_sun + hourSign1 + hourSeparator + ch_sun + timeSeparetor + cm_sun + hourSign2;
                        }
                        else
                        {
                            Label_Sunday.Text = sunday + scheduleSeparator + closed;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }

        private void Logout()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand =
                        "INSERT INTO LOGIN (IP_ADDRESS, DATE, TIME, USER_NAME, PASSWORD, ACTION) VALUES (" +
                        "@IP_ADDRESS, @DATE, @TIME, @USER_NAME, @PASSWORD, @ACTION)";
                                        
                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.ToString("yyyy/MM/dd"));
                        cmd.Parameters.AddWithValue("@TIME", DateTime.Now.ToLongTimeString());
                        cmd.Parameters.AddWithValue("@IP_ADDRESS", User_IP.Get_UserIP());
                        cmd.Parameters.AddWithValue("@USER_NAME", ConfigurationManager.AppSettings["app_user"]);
                        cmd.Parameters.AddWithValue("@PASSWORD", string.Empty);
                        cmd.Parameters.AddWithValue("@ACTION", "LOGOUT");

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        Session["user"] = string.Empty;
                        Session["name"] = string.Empty;
                        Session["role"] = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }

        private void SubscriberExist()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM SUBSCRIBE WHERE EMAIL=@EMAIL";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@EMAIL", TextBox_Subscribe.Text);

                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            subscriber = true;
                        }
                        else
                        {
                            subscriber = false;
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

        private void Subscribe()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "INSERT INTO SUBSCRIBE (DATE, TIME, IP_ADDRESS, EMAIL) VALUES (@DATE, @TIME, @IP_ADDRESS, @EMAIL)";
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
                        cmd.Parameters.AddWithValue("@EMAIL", TextBox_Subscribe.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        Response.Write("<script>alert('" + message_subscriber_done + "');</script>");
                        TextBox_Subscribe.Text = string.Empty;
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
    }
}