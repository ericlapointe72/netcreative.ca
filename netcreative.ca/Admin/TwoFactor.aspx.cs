using System;

namespace netcreative.ca
{
    // The setup of two-factor sign-in used to be a page of its own. It is now a part of the users page; this address stays so that
    // a bookmark or an old link still lands in the right place.
    public partial class TwoFactor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.RedirectPermanent("~/Admin/Users.aspx");
        }
    }
}