using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class boutique : System.Web.UI.Page
    {
        // the number of questions of the "Questions fréquentes" section (keys BtqFaq_Q1/BtqFaq_A1 ... in the resources)
        protected const int FaqCount = 7;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Boutique_Title, Global.Boutique_Description);
        }

        // the same questions and answers as the page shows, for Google (FAQPage)
        protected string FaqJsonLd()
        {
            return FaqSchema.Json("BtqFaq", FaqCount);
        }
    }
}
