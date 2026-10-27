using System.Web;
using System.Web.SessionState;

namespace netcreative.ca
{
    // The preview of the newsletter page: the message and the closing line typed so far, as the HTML of the e-mail (the same
    // NewsletterFormat that makes the e-mail). It changes nothing and is under /Admin/, so only a signed-in administrator gets an answer.
    public class NewsletterPreview : IHttpHandler, IReadOnlySessionState
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            if (context.Request.HttpMethod != "POST")
            {
                context.Response.StatusCode = 405;
                return;
            }

            // what is typed may hold "<" (the page itself accepts it too: it is only ever written as text); the answer is made of
            // encoded text and the marks of NewsletterFormat
            string message = context.Request.Unvalidated.Form["message"];
            string closing = context.Request.Unvalidated.Form["closing"];

            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.Write(NewsletterFormat.Html(message, closing));
        }
    }
}
