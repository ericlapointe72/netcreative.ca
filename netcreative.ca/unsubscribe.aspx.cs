using System;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // The page of the "unsubscribe" link every newsletter carries (?t=, the address encrypted and signed by the site), and the form for whoever
    // lost the link. Opening the link only shows the address and a button: the address is removed when the person clicks it (a mail
    // program that opens every link to check it must not unsubscribe anybody). The "Unsubscribe" button of Gmail, Outlook and Apple Mail
    // does not open this page: it posts "List-Unsubscribe=One-Click" to the same link, which unsubscribes at once.
    public partial class unsubscribe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageMeta(Global.Unsubscribe_Title, null, true);

            Button_Confirm.Text = Global.Unsubscribe_Button;
            Panel_Form.Attributes["data-network-error"] = Global.Unsubscribe_NetworkError;

            bool oneClick = Request.HttpMethod == "POST"
                            && string.Equals(Request.Form["List-Unsubscribe"], "One-Click", StringComparison.OrdinalIgnoreCase);

            if (IsPostBack && !oneClick)
            {
                return;
            }

            string token = Request.QueryString["t"];

            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            string email = UnsubscribeLink.Email(token);

            if (email == null)
            {
                ShowMessage(Global.Text("Unsub_Invalid"));
                return;
            }

            if (oneClick)
            {
                Unsubscribe(email);
                return;
            }

            HiddenField_Token.Value = token;
            Literal_ConfirmText.Text = HttpUtility.HtmlEncode(Global.Format(Global.Text("Unsub_Confirm"), Mask(email)));
            Panel_Confirm.Visible = true;
            Panel_Form.Visible = false;
        }

        protected void Button_Confirm_Click(object sender, EventArgs e)
        {
            string email = UnsubscribeLink.Email(HiddenField_Token.Value);

            if (email == null)
            {
                Panel_Confirm.Visible = false;
                ShowMessage(Global.Text("Unsub_Invalid"));
                return;
            }

            Unsubscribe(email);
        }

        private void Unsubscribe(string email)
        {
            try
            {
                // the same answer whether the address was still on the list or not
                SubscribeService.Unsubscribe(email);
                Panel_Confirm.Visible = false;
                Panel_Form.Visible = false;
                ShowMessage(Global.Text("Unsub_Done"));
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "unsubscribe page");
                ShowMessage(Global.Text("Sub_Error"));
            }
        }

        private void ShowMessage(string text)
        {
            Literal_Message.Text = HttpUtility.HtmlEncode(text);
            Panel_Message.Visible = true;
        }

        // "j***@live.ca": enough for the owner of the address to recognise it, not enough for anybody else to read it
        private static string Mask(string email)
        {
            int at = email.IndexOf('@');
            return at < 2 ? email : email.Substring(0, 1) + "***" + email.Substring(at);
        }
    }
}
