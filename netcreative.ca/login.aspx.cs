using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class login : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private string message_error = string.Empty;
        private string message_greeting = string.Empty;
        int counter = 0;
        bool loginResult = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((string)Session["language"]))
            {
                Session["language"] = ConfigurationManager.AppSettings["app_language"].ToString();
            }

            Global.SetCulture(Session["language"].ToString());

            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Load_Languages();
            counter = Convert.ToInt32(ViewState["counter"]);
        }

        protected void Button_Connection_Click(object sender, EventArgs e)
        {
            VerifyIfAdminExist();
        }

        private void Load_Languages()
        {
            Page.Title = Global.Login_Title;
            Label_Title.Text = Global.Login_Welcome;
            Label_User.Text = Global.Login_User;
            Label_Password.Text = Global.Login_Password;
            Button_Connection.Text = Global.Login_ConnectionButton;
            message_error = Global.Login_ErrorMessage;
            message_greeting = Global.Login_Greeting;
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
                            Session["name"] = message_greeting + " " + dr["FIRST_NAME"];
                            Session["role"] = "admin";

                            loginResult = true;
                            Write_Login();
                            Response.Redirect("default.aspx");
                        }
                        else
                        {
                            Response.Write("<script>alert('" + message_error + "');</script>");
                            counter += 1;
                            ViewState["counter"] = counter;
                            loginResult = false;
                            Write_Login();

                            if (counter == 3)
                            {
                                Response.Redirect("default.aspx");
                            }
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

        private void Write_Login()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand =
                        "INSERT INTO LOGIN (DATE, TIME, IP_ADDRESS, USER_NAME, PASSWORD, ACTION) VALUES (" +
                        "@DATE, @TIME, @IP_ADDRESS, @USER_NAME, @PASSWORD, @ACTION)";

                    string user_ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (user_ip == null)
                    {
                        user_ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    else
                    {
                        user_ip = user_ip.Split(',')[0];
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@DATE", DateTime.Now.ToString("yyyy/MM/dd"));
                        cmd.Parameters.AddWithValue("@TIME", DateTime.Now.ToLongTimeString());
                        cmd.Parameters.AddWithValue("@IP_ADDRESS", user_ip);
                        cmd.Parameters.AddWithValue("@USER_NAME", TextBox_User.Text);

                        cmd.Parameters.AddWithValue("@PASSWORD", string.Empty);

                        if (loginResult)
                        {
                            ConfigurationManager.AppSettings["app_user"] = TextBox_User.Text;
                            cmd.Parameters.AddWithValue("@ACTION", "LOGIN");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ACTION", "FAIL");
                        }

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