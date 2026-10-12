using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class confirmation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Confirmation_Title, null, true);
        }
    }
}
