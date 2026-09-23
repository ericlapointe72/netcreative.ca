using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class ContactLog : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private bool contact_result = false;
        private string contact_message_exist = string.Empty;
        private string contact_message_delete = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Button_Contact_Delete.Text = Global.Admin_Delete;
            contact_message_exist = Global.Format(Global.Admin_ContactExistsError, TextBox_Contact_Delete.Text);
            contact_message_delete = Global.Format(Global.Admin_ContactDeletedSuccess, TextBox_Contact_Delete.Text);

            if (!IsPostBack)
            {
                Contact_Load();
            }
        }

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
    }
}
