using System;
using System.Configuration;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // The form of the page "unsubscribe" (for whoever lost the link of a newsletter), called in the background by unsubscribe.js. It
    // never unsubscribes anybody by itself: it sends, to the address typed, the link that does it (if the address is on the list), and
    // answers the same thing whether the address is on the list or not, so the form cannot be used to find out who is subscribed or to
    // remove somebody else's address.
    public class UnsubscribeHandler : IHttpHandler, IReadOnlySessionState
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.ContentType = "application/json; charset=utf-8";

            bool fromSite = string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase)
                            && context.Request.Headers["X-Requested-With"] == "fetch";
            if (!fromSite)
            {
                context.Response.StatusCode = 400;
                return;
            }

            string language = context.Session == null ? null : context.Session["language"] as string;
            Global.SetCulture(string.IsNullOrEmpty(language) ? (ConfigurationManager.AppSettings["app_language"] ?? "fr") : language);

            string email = (context.Request.Form["email"] ?? string.Empty).Trim().ToLowerInvariant();

            if (email.Length == 0 || email.Length > 254 || !EmailAddressCheck.IsValid(email))
            {
                Answer(context, false, Global.Master_SubscribeErrorInvalid);
                return;
            }

            try
            {
                SubscribeService.RequestUnsubscribeLink(email, User_IP.Get_UserIP());
                Answer(context, true, Global.Text("Unsub_Sent"));
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "unsubscribe link request");
                Answer(context, false, Global.Text("Sub_Error"));
            }
        }

        private static void Answer(HttpContext context, bool success, string message)
        {
            context.Response.Write(new JavaScriptSerializer().Serialize(new { success, message }));
        }
    }
}
