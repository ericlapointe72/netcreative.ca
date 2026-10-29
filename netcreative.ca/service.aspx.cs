using System;
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

        // the same questions and answers as the page shows, for Google (FAQPage)
        protected string FaqJsonLd()
        {
            return FaqSchema.Json("Faq", FaqCount);
        }
    }
}
