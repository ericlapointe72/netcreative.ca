using System;
using System.Configuration;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class boutique : System.Web.UI.Page
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
            Page.Title = Global.Boutique_Title;
            Master.SetPageHero("Imgs_Site/hero-boutique.jpg", Global.Boutique_HeroEyebrow, Global.Boutique_HeroTitle, Global.Boutique_HeroText, Global.Boutique_HeroCta, "https://www.beaucedutyfree.com", "_blank");
        }
    }
}
