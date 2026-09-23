using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class LoginLog : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private bool login_result = false;
        private string login_message_exist = string.Empty;
        private string login_message_delete = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Button_Login_Delete.Text = Global.Admin_Delete;
            login_message_exist = Global.Format(Global.Admin_LoginExistsError, TextBox_Login_Delete.Text);
            login_message_delete = Global.Format(Global.Admin_LoginDeletedSuccess, TextBox_Login_Delete.Text);

            if (!IsPostBack)
            {
                Login_Load();
            }
        }

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
    }
}
