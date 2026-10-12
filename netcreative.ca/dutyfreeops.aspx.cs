using System;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class dutyfreeops : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.DutyFreeOps_Title, Global.DutyFreeOps_Description);
        }
    }
}
