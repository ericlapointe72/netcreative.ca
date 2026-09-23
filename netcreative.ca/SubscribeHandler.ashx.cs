using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public class SubscribeHandler : IHttpHandler, IRequiresSessionState
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

            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.CultureInvariant | RegexOptions.Singleline);

            if (!regex.IsMatch(email))
            {
                message = Global.Master_SubscribeErrorInvalid;
            }
            else
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string existsCommand = "SELECT * FROM SUBSCRIBE WHERE EMAIL=@EMAIL";

                        using (SqlCommand cmd = new SqlCommand(existsCommand, connection))
                        {
                            cmd.Parameters.AddWithValue("@EMAIL", email);

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                if (dr.HasRows)
                                {
                                    message = Global.Master_SubscribeErrorExists;
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(message))
                        {
                            string insertCommand = "INSERT INTO SUBSCRIBE (DATE, TIME, IP_ADDRESS, EMAIL) VALUES (@DATE, @TIME, @IP_ADDRESS, @EMAIL)";

                            using (SqlCommand cmd = new SqlCommand(insertCommand, connection))
                            {
                                cmd.Parameters.AddWithValue("@DATE", DateTime.Now.Date);
                                cmd.Parameters.AddWithValue("@TIME", DateTime.Now.TimeOfDay);
                                cmd.Parameters.AddWithValue("@IP_ADDRESS", User_IP.Get_UserIP());
                                cmd.Parameters.AddWithValue("@EMAIL", email);

                                cmd.ExecuteNonQuery();
                            }

                            success = true;
                            message = Global.Master_SubscribeDone;

                            // resolved on this thread, where Global.SetCulture() above has
                            // already set the right culture - the background thread below
                            // has no culture context of its own, so this can't be looked up
                            // lazily inside SendNotificationEmail() or it silently falls back
                            // to the server's default culture instead of the visitor's
                            string mailSubject = Global.Master_MailSubjectNewSubscriber;
                            string mailBody = Global.Master_MailBodyNewSubscriber;

                            // fire-and-forget: this is just an FYI notification to the site
                            // owner, the subscription itself is already safely stored above
                            System.Threading.Tasks.Task.Run(() => SendNotificationEmail(mailSubject, mailBody));
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }

            context.Response.ContentType = "application/json";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(new { success, message }));
        }

        private void SendNotificationEmail(string mailSubject, string mailBody)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["smtp_host"], Convert.ToInt32(ConfigurationManager.AppSettings["smtp_port"]));
                smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtp_user"], ConfigurationManager.AppSettings["smtp_password"]);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["smtp_user"], "Net.Créative.ca");
                mail.To.Add(new MailAddress(ConfigurationManager.AppSettings["smtp_user"]));
                mail.Subject = mailSubject;
                mail.Body = mailBody;

                smtpClient.Send(mail);
            }
            catch
            {
            }
        }
    }
}
