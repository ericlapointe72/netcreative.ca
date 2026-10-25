using System;
using System.Web;
using System.Web.UI;

namespace netcreative.ca
{
    // Named SiteApplication, not Global: the resource wrapper netcreative.ca.Resources.Global is used everywhere as "Global.X".
    public class SiteApplication : HttpApplication
    {
        // At the start of every request: headers that tell the browser how to treat the site. Nothing here changes how a page looks.
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpResponse response = HttpContext.Current.Response;

            // the site never needs the camera, the microphone, the position or the payment API (so nothing injected could use them)
            response.AppendHeader("Permissions-Policy", "camera=(), microphone=(), geolocation=(), payment=(), usb=()");

            // what a page serves is never reinterpreted as another type; the pages are not shown inside another site's frames;
            // the address of a page is only given to other sites without its path and query
            response.AppendHeader("X-Content-Type-Options", "nosniff");
            response.AppendHeader("X-Frame-Options", "SAMEORIGIN");
            response.AppendHeader("Referrer-Policy", "strict-origin-when-cross-origin");

            // What a page may do with what it is given, without forbidding anything the pages do today: no <base> that redirects the
            // relative addresses, no plug-in objects, no frame of another site around the pages, and forms that only post to this site.
            // (Scripts and styles are not restricted: several pages write small scripts and styles of their own in the page.)
            response.AppendHeader("Content-Security-Policy", "base-uri 'self'; object-src 'none'; frame-ancestors 'self'; form-action 'self'");
        }

        // Everything under /Admin/ needs a signed-in administrator, checked here BEFORE any page code runs (a page that does not use
        // Admin.Master, or whose own code runs before the master's check, can no longer be reached without signing in).
        // Open on purpose: the sign-in page, the two handlers of the sign-in. Admin pages also tell the browser not to keep a copy
        // (they show the visitors' names, e-mails and messages), so the Back button after signing out shows nothing.
        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            if (!IsProtectedAdminPath(context))
            {
                return;
            }

            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            // no session available for this request: not an administrator either
            if (context.Session == null)
            {
                context.Response.StatusCode = 403;
                context.ApplicationInstance.CompleteRequest();
                return;
            }

            string role = context.Session["role"] as string;
            string user = context.Session["user"] as string;

            if (string.IsNullOrEmpty(user) || role != "admin")
            {
                context.Response.Redirect("~/Admin/login.aspx", false);
                context.ApplicationInstance.CompleteRequest();
                return;
            }

            // a user deleted in "Users" loses access within a minute, not at the end of the session
            if (!AdminLogin.IsUserStillActive(user))
            {
                context.Session.Clear();
                context.Session.Abandon();
                context.Response.Redirect("~/Admin/login.aspx", false);
                context.ApplicationInstance.CompleteRequest();
                return;
            }

            // Two-factor sign-in compulsory (appSetting AdminRequireTwoFactor): a session that did not give a code can only open the
            // page where the code is set up, nothing else of the admin
            if (AdminLogin.RequireTwoFactor && AdminLogin.TwoFactorAvailable()
                && !(context.Session["mfa"] is bool && (bool)context.Session["mfa"])
                && !string.Equals(context.Request.AppRelativeCurrentExecutionFilePath, "~/Admin/Users.aspx", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Redirect("~/Admin/Users.aspx", false);
                context.ApplicationInstance.CompleteRequest();
            }
        }

        // An admin page's view state is tied to the signed-in session: a form prepared on another session (or by an attacker's page)
        // is refused when it is posted, so an administrator cannot be tricked into submitting it.
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            var page = context.Handler as Page;
            if (page == null || context.Session == null || !IsProtectedAdminPath(context))
            {
                return;
            }

            page.ViewStateUserKey = context.Session.SessionID;
        }

        // true for every admin address except the three open on purpose: the sign-in page and the two handlers of the sign-in
        private static bool IsProtectedAdminPath(HttpContext context)
        {
            string path = (context.Request.AppRelativeCurrentExecutionFilePath ?? string.Empty).ToLowerInvariant();

            if (!path.StartsWith("~/admin/") && path != "~/admin")
            {
                return false;
            }

            return path != "~/admin/login.aspx" && path != "~/admin/loginhandler.ashx" && path != "~/admin/logincomplete.ashx";
        }

        // Keeps a trace of every error that ends as a "500" (the visitor sees a plain page, without the technical details ASP.NET would
        // show). A request refused by ASP.NET's own check of what was typed (a "<script>" in a form, for example) is not a fault of the
        // site: a short 400, nothing logged.
        protected void Application_Error(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                return;
            }

            Exception error = context.Server.GetLastError();

            var http = error as HttpException;

            // A request ASP.NET itself refuses (a "<script>" in a form, or an address that holds a character it does not allow, such as the
            // "javascript:__doPostBack(...)" a search engine makes of an old link) is not a fault of the site: a short 400, nothing logged.
            if (error is HttpRequestValidationException || (http != null && http.GetHttpCode() == 400))
            {
                ShowPlainPage(context, 400);
                return;
            }

            bool notFound = http != null && http.GetHttpCode() == 404;

            if (!notFound)
            {
                ErrorLogger.Log(error, "unhandled");
            }

            if (!context.Response.HeadersWritten)
            {
                ShowPlainPage(context, notFound ? 404 : 500);
            }
        }

        private static void ShowPlainPage(HttpContext context, int status)
        {
            string language = null;
            try
            {
                language = context.Session == null ? null : context.Session["language"] as string;
            }
            catch (Exception)
            {
            }

            bool fr = language != "en";

            string title;
            string message;
            string back = fr ? "Retour à l’accueil" : "Back to the home page";

            switch (status)
            {
                case 400:
                    title = fr ? "Requête invalide" : "Invalid request";
                    message = fr ? "Cette demande n’a pas pu être traitée." : "This request could not be processed.";
                    break;

                case 404:
                    title = fr ? "Page introuvable" : "Page not found";
                    message = fr ? "La page demandée n’existe pas ou a été déplacée." : "The page you asked for does not exist or has moved.";
                    break;

                default:
                    title = fr ? "Un problème est survenu" : "Something went wrong";
                    message = fr ? "Une erreur est survenue de notre côté. Réessayez dans un instant." : "An error occurred on our side. Please try again in a moment.";
                    break;
            }

            context.Server.ClearError();
            context.Response.Clear();
            context.Response.TrySkipIisCustomErrors = true;
            context.Response.StatusCode = status;
            context.Response.ContentType = "text/html";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            string home = VirtualPathUtility.ToAbsolute("~/");

            context.Response.Write("<!DOCTYPE html><html lang=\"" + (fr ? "fr" : "en") + "\"><head><meta charset=\"utf-8\">"
                + "<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><meta name=\"robots\" content=\"noindex\">"
                + "<title>" + title + "</title>"
                + "<style>body{font-family:Arial,Helvetica,sans-serif;margin:0;background:#f6f6f7;color:#222}"
                + "main{max-width:34rem;margin:15vh auto 0;padding:0 1.25rem;text-align:center}"
                + "h1{font-size:1.5rem;margin:0 0 .75rem}p{line-height:1.5}"
                + "a{display:inline-block;margin-top:1rem;padding:.6rem 1.1rem;border-radius:999px;background:#222;color:#fff;text-decoration:none;font-weight:bold}"
                + "</style></head><body><main><h1>" + title + "</h1><p>" + message + "</p><a href=\"" + home + "\">" + back + "</a></main></body></html>");

            context.ApplicationInstance.CompleteRequest();
        }

        // The session cookie only travels over HTTPS (it already cannot be read by scripts); the site is HTTPS only. The same goes for
        // every other cookie of the site: scripts never read them (only the server does), they are only sent over HTTPS, and not with
        // requests started by another site.
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            if (!context.Request.IsSecureConnection)
            {
                return;
            }

            foreach (string name in context.Response.Cookies.AllKeys)
            {
                HttpCookie cookie = context.Response.Cookies[name];
                if (cookie == null)
                {
                    continue;
                }

                cookie.Secure = true;
                cookie.HttpOnly = true;
                cookie.SameSite = SameSiteMode.Lax;
            }
        }
    }
}
