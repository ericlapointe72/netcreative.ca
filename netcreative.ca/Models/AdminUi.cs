using System;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace netcreative.ca
{
    // Small helpers shared by the admin pages.
    public static class AdminUi
    {
        // A message in the toast of toast.js (the green or red box that slides in). It used to be written with
        // Response.Write("<script>showToast('" + message + "'...), which put the script BEFORE the doctype (the page then switched to
        // quirks mode after every action) and broke as soon as the message held an apostrophe. Here the page registers the script at the
        // end of the form, and the message is written as a JSON string, so whatever it holds stays text.
        public static void Toast(Page page, string message, bool success)
        {
            string script = "showToast(" + new JavaScriptSerializer().Serialize(message ?? string.Empty) + ", '" + (success ? "success" : "error") + "');";

            // a page can show several messages in one request: each one gets its own key
            string key = "toast_" + Guid.NewGuid().ToString("N");
            page.ClientScript.RegisterStartupScript(page.GetType(), key, script, true);
        }

        // the text of a confirm box for OnClientClick (or an onclick attribute): "return confirm(...);" with the message written as a
        // JSON string, so an apostrophe or a quote in it (a user name, for example) cannot break the script. The control encodes the
        // quotes of the attribute when it renders.
        public static string ConfirmScript(string message)
        {
            return "return confirm(" + new JavaScriptSerializer().Serialize(message ?? string.Empty) + ");";
        }
    }
}
