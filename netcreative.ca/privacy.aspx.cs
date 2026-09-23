using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class privacy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Global.SetCulture(Session["language"].ToString());

            Load_Languages();
        }

        private void Load_Languages()
        {
            Page.Title = Global.Privacy_Title;
        }
    }
}