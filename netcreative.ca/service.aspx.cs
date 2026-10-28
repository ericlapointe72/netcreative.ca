using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class service : System.Web.UI.Page
    {
        // the number of questions of the "Questions fréquentes" section (keys Faq_Q1/Faq_A1 ... in the resources)
        private const int FaqCount = 8;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Service_Title, Global.Service_Description);
        }

        // The same questions and answers as the page shows, as the FAQPage of schema.org (what Google reads to show them under the
        // result). "<" is written as < so that nothing in a text can close the script block.
        protected string FaqJsonLd()
        {
            var questions = new List<object>();

            for (int i = 1; i <= FaqCount; i++)
            {
                questions.Add(new Dictionary<string, object>
                {
                    { "@type", "Question" },
                    { "name", Global.Text("Faq_Q" + i) },
                    { "acceptedAnswer", new Dictionary<string, object> { { "@type", "Answer" }, { "text", Global.Text("Faq_A" + i) } } }
                });
            }

            var page = new Dictionary<string, object>
            {
                { "@context", "https://schema.org" },
                { "@type", "FAQPage" },
                { "mainEntity", questions }
            };

            return new JavaScriptSerializer().Serialize(page).Replace("<", "\\u003c");
        }
    }
}
