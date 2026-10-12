using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class privacy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Privacy_Title, Global.Privacy_Description);
        }
    }
}
