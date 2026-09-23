using System;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Users : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private bool user_exist = false;
        private string user_message_use = string.Empty;
        private string user_message_save = string.Empty;
        private string user_message_delete = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Button_User_Save.Text = Global.Admin_SaveButton;
            Button_User_SaveNew.Text = Global.Admin_SaveButton;
            Button_User_New.Text = Global.Admin_AddButton;
            Button_User_Delete.Text = Global.Admin_Delete;
            user_message_use = Global.Format(Global.Admin_UserAlreadyUsedError, TextBox_User_Name.Text);
            user_message_save = Global.Format(Global.Admin_UserSavedSuccess, TextBox_User_Name.Text);
            user_message_delete = Global.Format(Global.Admin_UserDeletedSuccess, TextBox_User_Name.Text);

            if (!IsPostBack)
            {
                Button_User_SaveNew.Visible = false;
                User_LoadList();
                User_Load();
            }
        }

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
    }
}
