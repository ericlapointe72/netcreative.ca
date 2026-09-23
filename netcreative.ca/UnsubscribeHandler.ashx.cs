using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public class UnsubscribeHandler : IHttpHandler, IRequiresSessionState
    {
        private string connectionString = string.Empty;

        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            Global.SetCulture(context.Session["language"].ToString());
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            string email = context.Request.Form["email"] ?? string.Empty;

            bool success = false;
            string message = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string existsCommand = "SELECT * FROM SUBSCRIBE WHERE EMAIL=@EMAIL";
                    bool exists;

                    using (SqlCommand cmd = new SqlCommand(existsCommand, connection))
                    {
                        cmd.Parameters.AddWithValue("@EMAIL", email);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            exists = dr.HasRows;
                        }
                    }

                    if (!exists)
                    {
                        message = Global.Unsubscribe_ErrorMessage;
                    }
                    else
                    {
                        string deleteCommand = "DELETE FROM SUBSCRIBE WHERE EMAIL=@EMAIL";

                        using (SqlCommand cmd = new SqlCommand(deleteCommand, connection))
                        {
                            cmd.Parameters.AddWithValue("@EMAIL", email);
                            cmd.ExecuteNonQuery();
                        }

                        success = true;
                        message = Global.Unsubscribe_SuccessMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            context.Response.ContentType = "application/json";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(new { success, message }));
        }
    }
}
