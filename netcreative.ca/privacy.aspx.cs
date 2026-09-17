using System;
using System.Configuration;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class privacy : System.Web.UI.Page
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
            Page.Title = Global.Privacy_Title;
            H1title1.InnerText = Global.Privacy_H1Title1;
            H3subTitle1.InnerText = Global.Privacy_SubTitle1;
            Label_Paragraphe1.Text = Global.Privacy_Paragraph1;
            H3subTitle2.InnerText = Global.Privacy_SubTitle2;
            Label_Paragraphe2.Text = Global.Privacy_Paragraph2;
            Label_Bullet1.Text = Global.Privacy_Bullet1;
            Label_Bullet2.Text = Global.Privacy_Bullet2;
            Label_Bullet3.Text = Global.Privacy_Bullet3;
            Label_Bullet4.Text = Global.Privacy_Bullet4;
            Label_Bullet5.Text = Global.Privacy_Bullet5;
            Label_Bullet6.Text = Global.Privacy_Bullet6;
            Label_Bullet7.Text = Global.Privacy_Bullet7;
            Label_Paragraphe3.Text = Global.Privacy_Paragraph3;
            H3subTitle3.InnerText = Global.Privacy_SubTitle3;
            Label_Paragraphe4.Text = Global.Privacy_Paragraph4;
            Label_Bullet8.Text = Global.Privacy_Bullet8;
            Label_Bullet9.Text = Global.Privacy_Bullet9;
            H3subTitle4.InnerText = Global.Privacy_SubTitle4;
            Label_Paragraphe5.Text = Global.Privacy_Paragraph5;
            Label_Bullet10.Text = Global.Privacy_Bullet10;
            Label_Bullet11.Text = Global.Privacy_Bullet11;
            Label_Bullet12.Text = Global.Privacy_Bullet12;
            Label_Bullet13.Text = Global.Privacy_Bullet13;
            Label_Paragraphe6.Text = Global.Privacy_Paragraph6;
            H3subTitle5.InnerText = Global.Privacy_SubTitle5;
            Label_Paragraphe7.Text = Global.Privacy_Paragraph7;
        }
    }
}