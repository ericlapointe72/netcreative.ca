using System;
using System.Web;
using System.Web.SessionState;

namespace netcreative.ca
{
    // Second half of signing in. LoginHandler.ashx checked the password, threw the old session away and sent the browser here with a
    // one-use ticket; this handler starts the signed-in session, which is a NEW one (a session identifier that existed before signing
    // in is never the one that carries the sign-in).
    public class LoginComplete : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            AdminLogin.Ticket ticket = AdminLogin.RedeemTicket(context.Request.QueryString["t"]);

            if (ticket == null)
            {
                context.Response.Redirect("~/Admin/login.aspx", false);
                context.ApplicationInstance.CompleteRequest();
                return;
            }

            context.Session["language"] = ticket.Language == "en" ? "en" : "fr";
            context.Session["user"] = ticket.UserName;
            context.Session["role"] = "admin";

            // "mfa": this sign-in used a second factor; "mfa_offer": the user could turn it on and has not (the admin shows a reminder)
            context.Session["mfa"] = ticket.TwoFactorDone;
            context.Session["mfa_offer"] = ticket.TwoFactorOffered;

            // with two-factor compulsory, a user who has not set it up goes straight to the setup page
            string target = AdminLogin.RequireTwoFactor && ticket.TwoFactorOffered ? "~/Admin/Users.aspx" : "~/Admin/Dashboard.aspx";

            context.Response.Redirect(target, false);
            context.ApplicationInstance.CompleteRequest();
        }
    }
}
