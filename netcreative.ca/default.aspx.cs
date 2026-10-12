using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Home_Title, Global.Home_Description);
        }
    }
}
