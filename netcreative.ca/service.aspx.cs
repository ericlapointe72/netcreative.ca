using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class service : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Service_Title, Global.Service_Description);
        }
    }
}
