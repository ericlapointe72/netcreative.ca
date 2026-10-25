using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using netcreative.ca.Resources;

namespace netcreative.ca.MasterPages
{
    public partial class Site : System.Web.UI.MasterPage
    {
        private const string SiteBase = "https://www.netcreative.ca";
        private const string BrandName = "Net.Créative";

        private string connectionString = string.Empty;
        private string pageDescription = string.Empty;
        private bool pageNoIndex = false;

        // the hours of the footer (loaded once per request), also given to search engines in the structured data of the home and contact pages
        private OpeningDay[] openingDays;

        protected string HtmlLang
        {
            get { return (string)Session["language"] == "en" ? "en" : "fr-CA"; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            // netcreative.ca and www.netcreative.ca both answer 200, which search engines
            // read as two identical sites. Send the bare domain to the www version for good.
            // The home page is one address too: "/" (what the visitor typed is in RawUrl; Url shows the page the server picked for "/").
            string rawPath = Request.RawUrl.Split('?')[0];
            bool askedForDefaultPage = rawPath.Equals("/default.aspx", StringComparison.OrdinalIgnoreCase);
            bool bareDomain = Request.Url.Host.Equals("netcreative.ca", StringComparison.OrdinalIgnoreCase);

            if (bareDomain || askedForDefaultPage)
            {
                string target = askedForDefaultPage || rawPath == "/" ? "/" : Request.Url.AbsolutePath;
                Response.RedirectPermanent(SiteBase + target + Request.Url.Query);
                return;
            }

            // Master.Page_Load fires *after* the content page's own Page_Load, but content
            // pages call Master.SetPageHero(...) from their Page_Load, which bakes Global.X
            // resource strings into control properties immediately (unlike the <%: %> markup
            // elsewhere, which is only evaluated at render time). So the ?lang= switch has to
            // be resolved here, in Init, which always completes before any Load runs — otherwise
            // the hero text keeps showing the culture from before the switch, one click behind.
            string queryLanguage = Request.QueryString["lang"];

            if (queryLanguage == "fr" || queryLanguage == "en")
            {
                Session["language"] = queryLanguage;
            }
            else if (string.IsNullOrEmpty((string)Session["language"]))
            {
                Session["language"] = ConfigurationManager.AppSettings["app_language"].ToString();
            }

            Global.SetCulture(Session["language"].ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            Load_Languages();
            Verify_CookieWarning();

            if (string.IsNullOrEmpty((string)Session["animation"]))
            {
                Session["animation"] = "1";
            }

            HideShow_Animation();

            // logged-in admins get sent straight to the dashboard instead of the
            // (now logout-less) login form, since there is no other way back into
            // the admin section from the public pages
            HyperLink_Login.HRef = !string.IsNullOrEmpty((string)Session["role"]) && Session["role"].Equals("admin")
                ? ResolveUrl("~/Admin/Dashboard.aspx")
                : ResolveUrl("~/Admin/login.aspx");
            HyperLink_Login.Attributes["aria-label"] = Global.Text("Master_AltLogin");
        }

        // Pages call this from their Page_Load. The <title> and meta tags are written at the
        // very start of the page's render, so setting Page.Title from the markup (inside
        // <% %>) happens too late and the master's default title always won.
        public void SetPageMeta(string title, string description = null, bool noIndex = false)
        {
            Page.Title = title.Contains(BrandName) ? title : title + " | " + BrandName;
            pageDescription = description;
            pageNoIndex = noIndex;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Add_SeoTags();
        }

        private void Add_SeoTags()
        {
            bool english = (string)Session["language"] == "en";
            string description = string.IsNullOrEmpty(pageDescription) ? Global.Master_DefaultDescription : pageDescription;
            Page.MetaDescription = description;

            if (pageNoIndex)
            {
                Page.Header.Controls.Add(new LiteralControl("\n        <meta name=\"robots\" content=\"noindex, nofollow\" />"));
                return;
            }

            // French lives at the clean URL, English at ?lang=en. Search engines never keep
            // the session cookie, so each of those URLs always renders in its own language.
            string path = Request.Url.AbsolutePath;
            if (path.Equals("/default.aspx", StringComparison.OrdinalIgnoreCase))
            {
                path = "/";
            }

            string frUrl = SiteBase + path;
            string enUrl = frUrl + "?lang=en";
            string pageUrl = english ? enUrl : frUrl;
            string imageUrl = SiteBase + "/Content/images/site/hero-accueil.jpg";
            string title = Page.Title;

            StringBuilder tags = new StringBuilder();
            tags.Append("\n        <link rel=\"canonical\" href=\"" + HttpUtility.HtmlAttributeEncode(pageUrl) + "\" />");
            tags.Append("\n        <link rel=\"alternate\" hreflang=\"fr-CA\" href=\"" + HttpUtility.HtmlAttributeEncode(frUrl) + "\" />");
            tags.Append("\n        <link rel=\"alternate\" hreflang=\"en\" href=\"" + HttpUtility.HtmlAttributeEncode(enUrl) + "\" />");
            tags.Append("\n        <link rel=\"alternate\" hreflang=\"x-default\" href=\"" + HttpUtility.HtmlAttributeEncode(frUrl) + "\" />");
            tags.Append("\n        <meta property=\"og:type\" content=\"website\" />");
            tags.Append("\n        <meta property=\"og:site_name\" content=\"" + BrandName + "\" />");
            tags.Append("\n        <meta property=\"og:locale\" content=\"" + (english ? "en_CA" : "fr_CA") + "\" />");
            tags.Append("\n        <meta property=\"og:title\" content=\"" + HttpUtility.HtmlAttributeEncode(title) + "\" />");
            tags.Append("\n        <meta property=\"og:description\" content=\"" + HttpUtility.HtmlAttributeEncode(description) + "\" />");
            tags.Append("\n        <meta property=\"og:url\" content=\"" + HttpUtility.HtmlAttributeEncode(pageUrl) + "\" />");
            tags.Append("\n        <meta property=\"og:image\" content=\"" + imageUrl + "\" />");
            tags.Append("\n        <meta name=\"twitter:card\" content=\"summary_large_image\" />");
            tags.Append("\n        <script type=\"application/ld+json\">" + StructuredData(path == "/" || path.Equals("/contact.aspx", StringComparison.OrdinalIgnoreCase)) + "</script>");

            Page.Header.Controls.Add(new LiteralControl(tags.ToString()));
        }

        // The business as a search engine reads it (schema.org): name, logo, founder, and, on the home and contact pages, the address, the
        // telephone number and the opening hours of the footer (so that the hours a search result shows are the ones of the admin page).
        private string StructuredData(bool withDetails)
        {
            var data = new Dictionary<string, object>
            {
                { "@context", "https://schema.org" },
                { "@type", withDetails ? "ProfessionalService" : "Organization" },
                { "@id", SiteBase + "/#business" },
                { "name", BrandName },
                { "url", SiteBase + "/" },
                { "logo", SiteBase + "/Content/images/site/logo.png" },
                { "founder", new Dictionary<string, object> { { "@type", "Person" }, { "name", "Éric Lapointe" } } },
                { "sameAs", new[] { "https://www.facebook.com/NetCr%C3%A9ative-101923751954399" } }
            };

            if (withDetails)
            {
                data["description"] = Global.Master_DefaultDescription;
                data["image"] = SiteBase + "/Content/images/site/hero-accueil.jpg";
                data["telephone"] = "+1-418-226-9087";
                data["email"] = "contact@netcreative.ca";
                data["address"] = new Dictionary<string, object>
                {
                    { "@type", "PostalAddress" },
                    { "streetAddress", "2845, 208e Rue" },
                    { "addressLocality", "Saint-Georges" },
                    { "addressRegion", "QC" },
                    { "postalCode", "G5Y 5B8" },
                    { "addressCountry", "CA" }
                };
                data["knowsLanguage"] = new[] { "fr-CA", "en" };

                string[] dayNames = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                var hours = new List<object>();

                for (int d = 0; openingDays != null && d < openingDays.Length; d++)
                {
                    if (!openingDays[d].Open)
                    {
                        continue;
                    }

                    hours.Add(new Dictionary<string, object>
                    {
                        { "@type", "OpeningHoursSpecification" },
                        { "dayOfWeek", dayNames[d] },
                        { "opens", string.Format("{0:00}:{1:00}", openingDays[d].OpensAt / 60, openingDays[d].OpensAt % 60) },
                        { "closes", string.Format("{0:00}:{1:00}", openingDays[d].ClosesAt / 60, openingDays[d].ClosesAt % 60) }
                    });
                }

                if (hours.Count > 0)
                {
                    data["openingHoursSpecification"] = hours;
                }
            }

            // "<" is written as \u003c: nothing in the data can end the script block it is in
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data).Replace("<", "\u003c");
        }

        public void SetPageHero(string imageUrl, string eyebrow, string title, string text = null, string ctaText = null, string ctaUrl = null, string ctaTarget = null)
        {
            PageHero.Attributes["class"] = "page-hero page-hero--photo";
            PageHero.Attributes["style"] = "background-image:url('" + imageUrl + "')";

            Span_PageHeroEyebrow.InnerText = eyebrow;
            Span_PageHeroEyebrow.Visible = true;

            H1_PageHero.InnerText = title;
            H1_PageHero.Visible = true;

            if (!string.IsNullOrEmpty(text))
            {
                Span_PageHeroText.InnerText = text;
                Span_PageHeroText.Visible = true;
            }

            if (!string.IsNullOrEmpty(ctaText) && !string.IsNullOrEmpty(ctaUrl))
            {
                HyperLink_PageHeroCta.InnerText = ctaText;
                HyperLink_PageHeroCta.HRef = ctaUrl;
                if (!string.IsNullOrEmpty(ctaTarget))
                {
                    HyperLink_PageHeroCta.Attributes["target"] = ctaTarget;
                }
                HyperLink_PageHeroCta.Visible = true;
            }
        }

        private void HideShow_Animation()
        {
            if (Session["animation"].Equals("1"))
            {
                Label_Animation.Visible = true;
                Session["animation"] = "0";
            }
            else if (Session["animation"].Equals("0"))
            {
                Label_Animation.Visible = false;
            }
        }


        private void Verify_CookieWarning()
        {
            if (Request.Cookies["netcreative_cookie_consent"] != null)
            {
                Panel_CookieWarning.Visible = false;
            }
        }

        private void Load_Languages()
        {
            try
            {
                bool english = Session["language"].ToString() != "fr";

                HyperLink_Language.HRef = Request.Url.AbsolutePath + "?lang=" + (english ? "fr" : "en");
                TextBox_Subscribe.Attributes.Add("placeholder", Global.Master_SubscribePlaceholder);
                TextBox_Subscribe.Attributes.Add("aria-label", Global.Master_SubscribePlaceholder);

                // the lines of the footer come from OpeningHours, like the preview of the admin page "Opening hours"
                OpeningDay[] days = OpeningHours.Load(connectionString);
                openingDays = days;
                Label[] labels = { Label_Monday, Label_Tuesday, Label_Wednesday, Label_Thursday, Label_Friday, Label_Saturday, Label_Sunday };

                for (int d = 0; d < labels.Length; d++)
                {
                    labels[d].Text = OpeningHours.Line(d, days[d], english);
                }
            }
            catch (Exception ex)
            {
                // the details go to the error log (App_Data\logs), never to the visitor
                ErrorLogger.Log(ex, "footer opening hours");
            }
        }

    }
}