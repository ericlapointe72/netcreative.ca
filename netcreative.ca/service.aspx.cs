using System;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class service : System.Web.UI.Page
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
            Page.Title = Global.Service_Title;
            H1title1.InnerText = Global.Service_H1Title1;
            Label_Service1.Text = Global.Service_Item1;
            Label_Service2.Text = Global.Service_Item2;
            Label_Service3.Text = Global.Service_Item3;
            Label_Service4.Text = Global.Service_Item4;
            Label_Service5.Text = Global.Service_Item5;
            Label_Service6.Text = Global.Service_Item6;
            Label_Service7.Text = Global.Service_Item7;
            H1title2.InnerText = Global.Service_H1Title2;
            Label_Client1.Text = Global.Service_Client1;
            Label_Client2.Text = Global.Service_Client2;
            Label_Client3.Text = Global.Service_Client3;
            Label_Client4.Text = Global.Service_Client4;
            Label_Client5.Text = Global.Service_Client5;
            Label_Client6.Text = Global.Service_Client6;
            H1title3.InnerText = Global.Service_H1Title3;
            h2title1.InnerText = Global.Service_TechTitle1;
            Label_Techno1.Text = Global.Service_TechText1;
            h2title2.InnerText = Global.Service_TechTitle2;
            Label_Techno2.Text = Global.Service_TechText2;
            h2title3.InnerText = Global.Service_TechTitle3;
            Label_Techno3.Text = Global.Service_TechText3;
            h2title4.InnerText = Global.Service_TechTitle4;
            Label_Techno4.Text = Global.Service_TechText4;
            h2title5.InnerText = Global.Service_TechTitle5;
            Label_Techno5.Text = Global.Service_TechText5;
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
                    string sqlCommand = "UPDATE NAVIGATION SET SERVICE=SERVICE+1, TOTAL=TOTAL+1 WHERE DATE=@DATE";

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
                        cmd.Parameters.AddWithValue("@PAGE", "SERVICE");

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