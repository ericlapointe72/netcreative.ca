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
            Label_HeroEyebrow.Text = Global.Boutique_HeroEyebrow;
            H1title.InnerText = Global.Boutique_HeroTitle;
            Label_HeroText.Text = Global.Boutique_HeroText;
            Label_HeroCta.Text = Global.Boutique_HeroCta;
            Label_Intro.Text = Global.Boutique_Intro;
            Label_CategoriesLabel.Text = Global.Boutique_CategoriesLabel;
            Label_Category1Title.Text = Global.Boutique_Category1Title;
            Label_Category1Text.Text = Global.Boutique_Category1Text;
            Label_Category2Title.Text = Global.Boutique_Category2Title;
            Label_Category2Text.Text = Global.Boutique_Category2Text;
            Label_Category3Title.Text = Global.Boutique_Category3Title;
            Label_Category3Text.Text = Global.Boutique_Category3Text;
        }
    }
}
