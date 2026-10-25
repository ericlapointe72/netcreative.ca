using System;
using System.Configuration;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class AdminMaster : System.Web.UI.MasterPage
    {
        // the user could use two-factor sign-in and has not turned it on (set at sign-in by LoginComplete.ashx): a reminder on every page but the users page, where it is set up
        protected bool ShowTwoFactorReminder
        {
            get
            {
                return Session["mfa_offer"] is bool && (bool)Session["mfa_offer"]
                    && !string.Equals(Request.AppRelativeCurrentExecutionFilePath, "~/Admin/Users.aspx", StringComparison.OrdinalIgnoreCase);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            // content pages' own Page_Load fires *before* the master page's Page_Load
            // (see the same note in Site.Master.cs), so the access check has to happen
            // here in Init - which always completes before any Load runs - otherwise a
            // content page's Page_Load would already have queried the database before
            // this redirect had a chance to run. (Global.asax checks the same thing even
            // earlier, before any page runs; this is the second lock on the same door.)
            if (string.IsNullOrEmpty((string)Session["role"]) || Session["role"].ToString() != "admin")
            {
                Response.Redirect(ResolveUrl("~/Admin/login.aspx"));
                return;
            }

            string language = Session["language"] as string;
            Global.SetCulture(string.IsNullOrEmpty(language) ? (ConfigurationManager.AppSettings["app_language"] ?? "fr") : language);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Button_Logout.Text = Global.Admin_LogoutButton;
            Label_CurrentUser.Text = Convert.ToString(Session["user"]);

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
            Response.Redirect(ResolveUrl("~/"));
        }

        // The session ends whatever happens to the log line: a sign-out that failed to write its trace used to leave the session open.
        private void Logout()
        {
            AdminLogin.LogEvent(Convert.ToString(Session["user"]), "LOGOUT");

            Session.Clear();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty) { Expires = DateTime.UtcNow.AddYears(-1), HttpOnly = true, Path = "/" });
        }
    }
}
