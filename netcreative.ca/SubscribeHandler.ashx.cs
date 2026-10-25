using System;
using System.Configuration;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // The newsletter box of the footer, called in the background by subscribe.js. It answers { success, message } in the language of the
    // visitor. Like beaucedutyfree.com: a double confirmation (the address only receives the newsletter after its owner clicked the link
    // of the e-mail the site sends to it), a hidden field and a stamp that only a robot gets wrong, and limits per hour.
    public class SubscribeHandler : IHttpHandler, IReadOnlySessionState
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.ContentType = "application/json; charset=utf-8";

            // only the site's own script calls this (a custom header cannot be added by another site)
            bool fromSite = string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase)
                            && context.Request.Headers["X-Requested-With"] == "fetch";
            if (!fromSite)
            {
                context.Response.StatusCode = 400;
                return;
            }

            string language = context.Session == null ? null : context.Session["language"] as string;
            Global.SetCulture(string.IsNullOrEmpty(language) ? (ConfigurationManager.AppSettings["app_language"] ?? "fr") : language);

            // the hidden field a person never fills: a robot does. It gets the same answer as a success.
            if (!string.IsNullOrWhiteSpace(context.Request.Form["subscribe_website"]))
            {
                Answer(context, true, Global.Text("Sub_Pending"));
                return;
            }

            string email = (context.Request.Form["email"] ?? string.Empty).Trim().ToLowerInvariant();

            if (email.Length == 0 || email.Length > 254 || !EmailAddressCheck.IsValid(email))
            {
                Answer(context, false, Global.Master_SubscribeErrorInvalid);
                return;
            }

            // the stamp the form carries (see FormTimer): a robot that posts at once, or that never loaded the page, fails here
            FormTimer.Verdict timing = FormTimer.Check(context.Request.Form["ts"], 3);
            if (timing == FormTimer.Verdict.Invalid)
            {
                Answer(context, false, Global.Text("Sub_Reload"));
                return;
            }

            // a robot's address or a robot's speed: the same answer as a success, and nothing is kept or sent
            if (timing == FormTimer.Verdict.TooFast || EmailAddressCheck.LooksGenerated(email))
            {
                Answer(context, true, Global.Text("Sub_Pending"));
                return;
            }

            try
            {
                switch (SubscribeService.Subscribe(email, User_IP.Get_UserIP()))
                {
                    case SubscribeResult.Already:
                        Answer(context, false, Global.Master_SubscribeErrorExists);
                        break;
                    case SubscribeResult.TooMany:
                        Answer(context, false, Global.Text("Sub_TooMany"));
                        break;
                    default:
                        Answer(context, true, Global.Text("Sub_Pending"));
                        break;
                }
            }
            catch (Exception ex)
            {
                // the details go to the error log (App_Data\logs), never to the visitor
                ErrorLogger.Log(ex, "newsletter sign-up");
                Answer(context, false, Global.Text("Sub_Error"));
            }
        }

        private static void Answer(HttpContext context, bool success, string message)
        {
            context.Response.Write(new JavaScriptSerializer().Serialize(new { success, message }));
        }
    }
}
