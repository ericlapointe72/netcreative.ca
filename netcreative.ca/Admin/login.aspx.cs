using System;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class login : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private int counter = 0;
        private bool loginResult = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Global.SetCulture(Session["language"].ToString());

            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Button_Connection.Text = Global.Login_ConnectionButton;
            counter = Convert.ToInt32(ViewState["counter"]);
        }

        protected void Button_Connection_Click(object sender, EventArgs e)
        {
            VerifyIfAdminExist();
        }

        private void VerifyIfAdminExist()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM USERS WHERE USER_NAME=@USER_NAME";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@USER_NAME", TextBox_User.Text);

                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows && dr.Read() && PasswordHasher.Verify(TextBox_Password.Text, dr["PASSWORD"].ToString()))
                        {
                            Session["user"] = dr["USER_NAME"];
                            Session["role"] = "admin";

                            loginResult = true;
                            Write_Login();
                            Response.Redirect(ResolveUrl("~/Admin/admin.aspx"));
                        }
                        else
                        {
                            Response.Write("<script>showToast('" + Global.Login_ErrorMessage + "', 'error');</script>");
                            counter += 1;
                            ViewState["counter"] = counter;
                            loginResult = false;
                            Write_Login();

                            if (counter == 3)
                            {
                                Response.Redirect(ResolveUrl("~/default.aspx"));
                            }
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

        private void Write_Login()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand =
                        "INSERT INTO LOGIN (DATE, TIME, IP_ADDRESS, USER_NAME, ACTION) VALUES (" +
                        "@DATE, @TIME, @IP_ADDRESS, @USER_NAME, @ACTION)";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.Date);
                        cmd.Parameters.AddWithValue("@TIME", DateTime.Now.TimeOfDay);
                        cmd.Parameters.AddWithValue("@IP_ADDRESS", User_IP.Get_UserIP());
                        cmd.Parameters.AddWithValue("@USER_NAME", TextBox_User.Text);
                        cmd.Parameters.AddWithValue("@ACTION", loginResult ? "LOGIN" : "FAIL");

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch
            {

            }
        }
    }
}