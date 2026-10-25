using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Global.SetCulture(Session["language"].ToString());
            Master.SetPageMeta(Global.Login_Title, null, true);

            // the limit on failed attempts is kept by the server per address and per user name (see AdminLogin),
            // not in the session: nothing to reset here
        }
    }
}
