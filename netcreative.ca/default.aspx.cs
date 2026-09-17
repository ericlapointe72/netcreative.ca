using System;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Default : System.Web.UI.Page
    {
        private string connectionString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((string)Session["language"]))
            {
                Session["language"] = ConfigurationManager.AppSettings["app_language"].ToString();
            }

            Global.SetCulture(Session["language"].ToString());

            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
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

        private void Load_Languages()
        {
            Page.Title = Global.Home_Title;
            Master.SetPageHero("Imgs_Site/hero-home.jpg", Global.Home_HeroEyebrow, Global.Home_Welcome);
            Label_PillarsLabel.Text = Global.Home_PillarsLabel;
            Label_Pillar1Title.Text = Global.Home_Pillar1Title;
            Label_Pillar1Text.Text = Global.Home_Pillar1Text;
            Label_Pillar2Title.Text = Global.Home_Pillar2Title;
            Label_Pillar2Text.Text = Global.Home_Pillar2Text;
            Label_Pillar3Title.Text = Global.Home_Pillar3Title;
            Label_Pillar3Text.Text = Global.Home_Pillar3Text;
            H2slideshowtitle.InnerText = Global.Home_SlideshowTitle;
            Label_Slide1_Title.Text = Global.Home_Slide1Title;
            Label_Slide1_Text.Text = Global.Home_Slide1Text;
            Label_Slide2_Title.Text = Global.Home_Slide2Title;
            Label_Slide2_Text.Text = Global.Home_Slide2Text;
            Label_Slide3_Title.Text = Global.Home_Slide3Title;
            Label_Slide3_Text.Text = Global.Home_Slide3Text;
            Label_Slide4_Title.Text = Global.Home_Slide4Title;
            Label_Slide4_Text.Text = Global.Home_Slide4Text;
            Label_Slide5_Title.Text = Global.Home_Slide5Title;
            Label_Slide5_Text.Text = Global.Home_Slide5Text;
            H2whoiamtitle.InnerText = Global.Home_WhoIamTitle;
            Label_WhoIamParagraphe1.Text = Global.Home_WhoIamParagraph1;
            Label_WhoIamParagraphe2.Text = Global.Home_WhoIamParagraph2;
            Label_WhoIamParagraphe3.Text = Global.Home_WhoIamParagraph3;
            Label_WhoIamParagraphe4.Text = Global.Home_WhoIamParagraph4;
            Label_Facebook.Text = Global.Home_Facebook;
            Label_WhoIamName.Text = Global.Home_WhoIamName;
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
                    string sqlCommand = "UPDATE NAVIGATION SET HOME=HOME+1, TOTAL=TOTAL+1 WHERE DATE=@DATE";

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
                        cmd.Parameters.AddWithValue("@PAGE", "DEFAULT");

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