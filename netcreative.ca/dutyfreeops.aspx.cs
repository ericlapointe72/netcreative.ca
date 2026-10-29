using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class dutyfreeops : System.Web.UI.Page
    {
        // the number of questions of the "Questions fréquentes" section (keys DfoFaq_Q1/DfoFaq_A1 ... in the resources)
        protected const int FaqCount = 9;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.DutyFreeOps_Title, Global.DutyFreeOps_Description);
        }

        // the same questions and answers as the page shows, for Google (FAQPage)
        protected string FaqJsonLd()
        {
            return FaqSchema.Json("DfoFaq", FaqCount);
        }
    }
}
