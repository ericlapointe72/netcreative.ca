using System;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca.MasterPages
{
    public partial class Site : System.Web.UI.MasterPage
    {
        private string connectionString = string.Empty;

        protected void Page_Init(object sender, EventArgs e)
        {
            // Master.Page_Load fires *after* the content page's own Page_Load, but content
            // pages call Master.SetPageHero(...) from their Page_Load, which bakes Global.X
            // resource strings into control properties immediately (unlike the <%: %> markup
            // elsewhere, which is only evaluated at render time). So the ?lang= switch has to
            // be resolved here, in Init, which always completes before any Load runs — otherwise
            // the hero text keeps showing the culture from before the switch, one click behind.
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
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            Load_Languages();
            Verify_CookieWarning();

            if (string.IsNullOrEmpty((string)Session["animation"]))
            {
                Session["animation"] = "1";
            }

            HideShow_Animation();

            // logged-in admins get sent straight to the dashboard instead of the
            // (now logout-less) login form, since there is no other way back into
            // the admin section from the public pages
            HyperLink_Login.HRef = !string.IsNullOrEmpty((string)Session["role"]) && Session["role"].Equals("admin")
                ? ResolveUrl("~/Admin/Dashboard.aspx")
                : ResolveUrl("~/Admin/login.aspx");
        }

        public void SetPageHero(string imageUrl, string eyebrow, string title, string text = null, string ctaText = null, string ctaUrl = null, string ctaTarget = null)
        {
            PageHero.Attributes["class"] = "page-hero page-hero--photo";
            PageHero.Attributes["style"] = "background-image:url('" + imageUrl + "')";

            Span_PageHeroEyebrow.InnerText = eyebrow;
            Span_PageHeroEyebrow.Visible = true;

            H1_PageHero.InnerText = title;
            H1_PageHero.Visible = true;

            if (!string.IsNullOrEmpty(text))
            {
                Span_PageHeroText.InnerText = text;
                Span_PageHeroText.Visible = true;
            }

            if (!string.IsNullOrEmpty(ctaText) && !string.IsNullOrEmpty(ctaUrl))
            {
                HyperLink_PageHeroCta.InnerText = ctaText;
                HyperLink_PageHeroCta.HRef = ctaUrl;
                if (!string.IsNullOrEmpty(ctaTarget))
                {
                    HyperLink_PageHeroCta.Attributes["target"] = ctaTarget;
                }
                HyperLink_PageHeroCta.Visible = true;
            }
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
            Response.Redirect(ResolveUrl("~/default.aspx"));
        }

        protected void LinkButton_Privacy_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/privacy.aspx"));
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

                HyperLink_Language.HRef = Request.Url.AbsolutePath + "?lang=" + (Session["language"].ToString() == "fr" ? "en" : "fr");
                TextBox_Subscribe.Attributes.Add("placeholder", Global.Master_SubscribePlaceholder);

                string oh_sun, oh_mon, oh_tue, oh_wed, oh_thu, oh_fri, oh_sat;
                string om_sun, om_mon, om_tue, om_wed, om_thu, om_fri, om_sat;
                string ch_sun, ch_mon, ch_tue, ch_wed, ch_thu, ch_fri, ch_sat;
                string cm_sun, cm_mon, cm_tue, cm_wed, cm_thu, cm_fri, cm_sat;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM OPENING WHERE OPEN_CLOSE=@OPEN_CLOSE";

                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@OPEN_CLOSE", "OPEN_HOUR");

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            oh_sun = dr["SUN"].ToString();
                            oh_mon = dr["MON"].ToString();
                            oh_tue = dr["TUE"].ToString();
                            oh_wed = dr["WED"].ToString();
                            oh_thu = dr["THU"].ToString();
                            oh_fri = dr["FRI"].ToString();
                            oh_sat = dr["SAT"].ToString();
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@OPEN_CLOSE", "OPEN_MIN");

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            om_sun = dr["SUN"].ToString();
                            om_mon = dr["MON"].ToString();
                            om_tue = dr["TUE"].ToString();
                            om_wed = dr["WED"].ToString();
                            om_thu = dr["THU"].ToString();
                            om_fri = dr["FRI"].ToString();
                            om_sat = dr["SAT"].ToString();
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@OPEN_CLOSE", "CLOSE_HOUR");

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            ch_sun = dr["SUN"].ToString();
                            ch_mon = dr["MON"].ToString();
                            ch_tue = dr["TUE"].ToString();
                            ch_wed = dr["WED"].ToString();
                            ch_thu = dr["THU"].ToString();
                            ch_fri = dr["FRI"].ToString();
                            ch_sat = dr["SAT"].ToString();
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@OPEN_CLOSE", "CLOSE_MIN");

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            cm_sun = dr["SUN"].ToString();
                            cm_mon = dr["MON"].ToString();
                            cm_tue = dr["TUE"].ToString();
                            cm_wed = dr["WED"].ToString();
                            cm_thu = dr["THU"].ToString();
                            cm_fri = dr["FRI"].ToString();
                            cm_sat = dr["SAT"].ToString();
                        }
                    }

                    connection.Close();

                    {
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
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

    }
}