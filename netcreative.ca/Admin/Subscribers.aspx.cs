using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Subscribers : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private bool subscriber_result = false;
        private string subscriber_message_exist = string.Empty;
        private string subscriber_message_delete = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Button_Subscriber_Delete.Text = Global.Admin_Delete;
            subscriber_message_exist = Global.Format(Global.Admin_SubscriberExistsError, TextBox_Subscriber_Delete.Text);
            subscriber_message_delete = Global.Format(Global.Admin_SubscriberDeletedSuccess, TextBox_Subscriber_Delete.Text);

            if (!IsPostBack)
            {
                Subscriber_Load();
            }
        }

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
                            s.Date = ((DateTime)dr["DATE"]).ToString("yyyy-MM-dd");
                            s.Time = ((TimeSpan)dr["TIME"]).ToString(@"hh\:mm\:ss");
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
    }
}
