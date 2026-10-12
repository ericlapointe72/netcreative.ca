using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class unsubscribe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Unsubscribe_Title, null, true);
        }
    }
}
