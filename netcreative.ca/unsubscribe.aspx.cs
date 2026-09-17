using System;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class unsubscribe : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private bool subscriber = false;
        private string message_success = string.Empty;
        private string message_error = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((string)Session["language"]))
            {
                Session["language"] = ConfigurationManager.AppSettings["app_language"].ToString();
            }

            Global.SetCulture(Session["language"].ToString());

            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Load_Languages();
        }

        private void Load_Languages()
        {
            Page.Title = Global.Unsubscribe_Title;
            H1title.InnerText = Global.Unsubscribe_H1Title;
            Label_Unsubscribe.Text = Global.Unsubscribe_Label;
            Button_Unsubscribe.Text = Global.Unsubscribe_Button;
            message_success = Global.Unsubscribe_SuccessMessage;
            message_error = Global.Unsubscribe_ErrorMessage;
        }

        protected void Button_Unsubscribe_Click(object sender, EventArgs e)
        {
            VerifySubscriberExist();

            if (subscriber == true)
            {
                Unsubscribe();
            }
            else
            {
                Response.Write("<script>alert('" + message_error + "');</script>");
            }
        }

        private void VerifySubscriberExist()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM SUBSCRIBE WHERE EMAIL=@EMAIL";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@EMAIL", TextBox_Unsubscribe.Text);

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

        private void Unsubscribe()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "DELETE FROM SUBSCRIBE WHERE EMAIL=@EMAIL";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@EMAIL", TextBox_Unsubscribe.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        Response.Write("<script>alert('" + message_success + "');</script>");
                        TextBox_Unsubscribe.Text = string.Empty;
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