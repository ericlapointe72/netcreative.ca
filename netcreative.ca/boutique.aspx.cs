using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class boutique : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Boutique_Title, Global.Boutique_Description);
        }
    }
}
