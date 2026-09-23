using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public class LoginHandler : IHttpHandler, IRequiresSessionState
    {
        private string connectionString = string.Empty;

        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            Global.SetCulture(context.Session["language"].ToString());
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            string userName = context.Request.Form["user"] ?? string.Empty;
            string password = context.Request.Form["password"] ?? string.Empty;

            bool success = false;
            string message = string.Empty;
            string redirect = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM USERS WHERE USER_NAME=@USER_NAME";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@USER_NAME", userName);

                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows && dr.Read() && PasswordHasher.Verify(password, dr["PASSWORD"].ToString()))
                        {
                            context.Session["user"] = dr["USER_NAME"];
                            context.Session["role"] = "admin";
                            context.Session["loginAttempts"] = null;

                            success = true;
                            Write_Login(userName, true);
                            redirect = VirtualPathUtility.ToAbsolute("~/Admin/admin.aspx");
                        }
                        else
                        {
                            message = Global.Login_ErrorMessage;
                            Write_Login(userName, false);

                            int attempts = context.Session["loginAttempts"] != null ? (int)context.Session["loginAttempts"] : 0;
                            attempts += 1;
                            context.Session["loginAttempts"] = attempts;

                            if (attempts >= 3)
                            {
                                redirect = VirtualPathUtility.ToAbsolute("~/default.aspx");
                            }
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            context.Response.ContentType = "application/json";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(new { success, message, redirect }));
        }

        private void Write_Login(string userName, bool loginResult)
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
                        cmd.Parameters.AddWithValue("@USER_NAME", userName);
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
