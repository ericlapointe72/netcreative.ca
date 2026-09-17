using System;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class portfolio : System.Web.UI.Page
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
            Page.Title = Global.Portfolio_Title;
            H1title1.InnerText = Global.Portfolio_Title1;
            Label_Description1.Text = Global.Portfolio_Description1;
            Label_Link1.Text = Global.Portfolio_Link1;
            H1title2.InnerText = Global.Portfolio_Title2;
            Label_Description2.Text = Global.Portfolio_Description2;
            Label_Link2.Text = Global.Portfolio_Link2;
            H1title3.InnerText = Global.Portfolio_Title3;
            Label_Description3.Text = Global.Portfolio_Description3;
            Label_Link3.Text = Global.Portfolio_Link3;
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
                    string sqlCommand = "UPDATE NAVIGATION SET PORTFOLIO=PORTFOLIO+1, TOTAL=TOTAL+1 WHERE DATE=@DATE";

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
                        cmd.Parameters.AddWithValue("@PAGE", "PORTFOLIO");

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