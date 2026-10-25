using System;
using System.Text.RegularExpressions;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // The page of the link in the confirmation e-mail of the newsletter. Opening the link only shows a button; the address is added to
    // the list (CONFIRMED) when the person clicks it, so a program that opens the links of a message to check them confirms nobody.
    public partial class confirm : System.Web.UI.Page
    {
        private static readonly Regex TokenPattern = new Regex("^[0-9a-f]{32}$", RegexOptions.Compiled);

        protected void Page_Load(object sender, EventArgs e)
        {
            // nothing here is for search engines
            Master.SetPageMeta(Global.Text("Confirm_H1Title"), null, true);
            Button_Confirm.Text = Global.Text("Confirm_Button");

            if (IsPostBack)
            {
                return;
            }

            string token = (Request.QueryString["t"] ?? string.Empty).Trim().ToLowerInvariant();

            if (!TokenPattern.IsMatch(token))
            {
                ShowMessage(Global.Text("Confirm_Invalid"));
                return;
            }

            HiddenField_Token.Value = token;
            Panel_Confirm.Visible = true;
        }

        protected void Button_Confirm_Click(object sender, EventArgs e)
        {
            Panel_Confirm.Visible = false;

            try
            {
                string email = SubscribeService.Confirm(HiddenField_Token.Value);

                if (email == null)
                {
                    ShowMessage(Global.Text("Confirm_Invalid"));
                    return;
                }

                ShowMessage(Global.Text("Confirm_Done"));

                SubscribeService.NotifyOwner(email);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "subscription confirmation page");
                ShowMessage(Global.Text("Sub_Error"));
            }
        }

        private void ShowMessage(string text)
        {
            Literal_Message.Text = HttpUtility.HtmlEncode(text);
            Panel_Message.Visible = true;
        }
    }
}
