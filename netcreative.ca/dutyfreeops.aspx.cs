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
            Label_HeroEyebrow.Text = Global.DutyFreeOps_HeroEyebrow;
            H1title.InnerText = Global.DutyFreeOps_HeroTitle;
            Label_HeroText.Text = Global.DutyFreeOps_HeroText;
            Label_HeroCta.Text = Global.DutyFreeOps_HeroCta;
            Label_Intro.Text = Global.DutyFreeOps_Intro;
            Label_ModulesLabel.Text = Global.DutyFreeOps_ModulesLabel;
            Label_Module1Title.Text = Global.DutyFreeOps_Module1Title;
            Label_Module1Text.Text = Global.DutyFreeOps_Module1Text;
            Label_Module2Title.Text = Global.DutyFreeOps_Module2Title;
            Label_Module2Text.Text = Global.DutyFreeOps_Module2Text;
            Label_Module3Title.Text = Global.DutyFreeOps_Module3Title;
            Label_Module3Text.Text = Global.DutyFreeOps_Module3Text;
            Label_Module4Title.Text = Global.DutyFreeOps_Module4Title;
            Label_Module4Text.Text = Global.DutyFreeOps_Module4Text;
            Label_Module5Title.Text = Global.DutyFreeOps_Module5Title;
            Label_Module5Text.Text = Global.DutyFreeOps_Module5Text;
            Label_Module6Title.Text = Global.DutyFreeOps_Module6Title;
            Label_Module6Text.Text = Global.DutyFreeOps_Module6Text;
        }
    }
}
