using System;
using System.Configuration;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class dutyfreeops : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((string)Session["language"]))
            {
                Session["language"] = ConfigurationManager.AppSettings["app_language"].ToString();
            }

            Global.SetCulture(Session["language"].ToString());

            Load_Languages();
        }

        private void Load_Languages()
        {
            Page.Title = Global.DutyFreeOps_Title;
            Master.SetPageHero("Imgs_Site/hero-dutyfreeops.jpg", Global.DutyFreeOps_HeroEyebrow, Global.DutyFreeOps_HeroTitle, Global.DutyFreeOps_HeroText, Global.DutyFreeOps_HeroCta, "contact.aspx");
        }
    }
}
