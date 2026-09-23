using System;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class AdminMaster : System.Web.UI.MasterPage
    {
        private string connectionString = string.Empty;

        protected void Page_Init(object sender, EventArgs e)
        {
            // content pages' own Page_Load fires *before* the master page's Page_Load
            // (see the same note in Site.Master.cs), so the access check has to happen
            // here in Init - which always completes before any Load runs - otherwise a
            // content page's Page_Load would already have queried the database before
            // this redirect had a chance to run
            if (string.IsNullOrEmpty((string)Session["role"]) || Session["role"].ToString() != "admin")
            {
                Response.Redirect(ResolveUrl("~/Admin/login.aspx"));
                return;
            }

            if (string.IsNullOrEmpty((string)Session["language"]))
            {
                Session["language"] = ConfigurationManager.AppSettings["app_language"].ToString();
            }

            Global.SetCulture(Session["language"].ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Button_Logout.Text = Global.Admin_LogoutButton;
            Label_CurrentUser.Text = Session["user"].ToString();

            if (!IsPostBack)
            {
                HighlightCurrentNav();
            }
        }

        private void HighlightCurrentNav()
        {
            string path = Request.AppRelativeCurrentExecutionFilePath;

            var links = new[]
            {
                Link_Dashboard, Link_Contact, Link_Login, Link_Newsletter, Link_Opening, Link_Subscribers, Link_Users, Link_Sql
            };

            foreach (var link in links)
            {
                if (string.Equals(link.NavigateUrl, path, StringComparison.OrdinalIgnoreCase))
                {
                    link.CssClass += " admin-nav__link--active";
                }
            }
        }

        protected void Button_Logout_Click(object sender, EventArgs e)
        {
            Logout();
            Response.Redirect(ResolveUrl("~/default.aspx"));
        }

        private void Logout()
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
                        cmd.Parameters.AddWithValue("@USER_NAME", Session["user"].ToString());
                        cmd.Parameters.AddWithValue("@ACTION", "LOGOUT");

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        Session["user"] = string.Empty;
                        Session["role"] = string.Empty;
                    }
                }
            }
            catch
            {
            }
        }
    }
}
