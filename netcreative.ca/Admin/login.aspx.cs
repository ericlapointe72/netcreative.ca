using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Global.SetCulture(Session["language"].ToString());

            // reset on every visit to the page (there is no postback anymore to rely
            // on IsPostBack for) so a fresh set of 3 attempts is granted each time the
            // admin lands back here, instead of staying locked out for the whole session
            Session["loginAttempts"] = null;
        }
    }
}