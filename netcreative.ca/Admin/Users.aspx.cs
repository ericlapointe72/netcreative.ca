using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // The users of the admin (a table, and a form for the selected one or a new one) and, below, the signed-in user's own two-factor
    // sign-in (see Totp and LoginHandler.ashx). When two-factor sign-in is compulsory and this session did not give a code, only that
    // last part is shown: nothing else of the admin can be opened.
    public partial class Users : System.Web.UI.Page
    {
        // a password shorter than this is refused when it is set (nothing is checked on the passwords that already exist)
        private const int MinPasswordLength = 10;
        private const int NameLength = 20;
        private const string SetupKey = "totp_setup";

        private static readonly Regex NamePattern = new Regex("^[A-Za-z0-9._-]{3,20}$", RegexOptions.CultureInvariant);

        private string connectionString = string.Empty;
        private string _me;

        // one row of the table
        public sealed class UserRow
        {
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string TfaHtml { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            _me = Convert.ToString(Session["user"]);

            Button_User_Save.Text = Global.Admin_SaveButton;
            Button_User_SaveNew.Text = Global.Admin_SaveButton;
            Button_User_New.Text = Global.Admin_AddButton;
            Button_User_Delete.Text = Global.Admin_Delete;
            Button_User_Reset2fa.Text = Global.Text("Admin_UserReset2fa");

            Button_Start.Text = Global.Text("Tfa_Start_Button");
            Button_Confirm.Text = Global.Text("Tfa_ConfirmButton");
            Button_Cancel.Text = Global.Text("Tfa_CancelButton");
            Button_Regenerate.Text = Global.Text("Tfa_RegenButton");
            Button_Disable.Text = Global.Text("Tfa_DisableButton");
            Button_Disable.OnClientClick = AdminUi.ConfirmScript(Global.Text("Tfa_DisableConfirm"));

            GridView_Users.Columns[0].HeaderText = Global.Text("Tfa_ColUser");
            GridView_Users.Columns[1].HeaderText = Global.Text("Admin_UserFullNameCol");
            GridView_Users.Columns[2].HeaderText = Global.Text("Tfa_ColTfa");

            Panel_Management.Visible = !Restricted;

            try
            {
                if (!AdminLogin.TwoFactorAvailable())
                {
                    Panel_Unavailable.Visible = true;
                }

                if (!IsPostBack)
                {
                    if (!Restricted)
                    {
                        ShowEditMode();
                        BindUsers(_me);
                        User_Load();
                    }

                    if (AdminLogin.TwoFactorAvailable())
                    {
                        ShowTfaState();
                    }
                }
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        // the confirm boxes name the selected user: set after the events of the request, when the selection is final
        protected void Page_PreRender(object sender, EventArgs e)
        {
            string name = SelectedUser;
            Button_User_Delete.OnClientClick = AdminUi.ConfirmScript(Global.Format(Global.Text("Admin_UserDeleteConfirm"), name));
            Button_User_Reset2fa.OnClientClick = AdminUi.ConfirmScript(Global.Format(Global.Text("Admin_UserReset2faConfirm"), name));
        }

        // ---- who is who -------------------------------------------------------------------------------------

        // The user the form shows ("" while a new user is typed), kept between requests.
        private string SelectedUser
        {
            get { return ViewState["SelectedUser"] as string ?? string.Empty; }
            set { ViewState["SelectedUser"] = value ?? string.Empty; }
        }

        // two-factor sign-in compulsory and not given in this session: only the setup part of the page is shown (and Global.asax lets
        // nothing else of the admin open)
        private bool Restricted
        {
            get
            {
                return AdminLogin.RequireTwoFactor && AdminLogin.TwoFactorAvailable()
                    && !(Session["mfa"] is bool && (bool)Session["mfa"]);
            }
        }

        private bool IsNewMode
        {
            get { return Button_User_SaveNew.Visible; }
        }

        private bool IsSelf(string userName)
        {
            return string.Equals(userName, _me, StringComparison.OrdinalIgnoreCase);
        }

        private void ShowEditMode()
        {
            Button_User_Save.Visible = true;
            Button_User_SaveNew.Visible = false;
            Button_User_Delete.Visible = true;
            TextBox_User_Name.ReadOnly = true;
        }

        // ---- the table and the selected user ---------------------------------------------------------------------

        private void BindUsers(string select)
        {
            try
            {
                bool tfaAvailable = AdminLogin.TwoFactorAvailable();
                var rows = new List<UserRow>();

                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT USER_NAME, FIRST_NAME, LAST_NAME" + (tfaAvailable ? ", TotpEnabled" : string.Empty) + " FROM USERS ORDER BY USER_NAME", connection))
                {
                    connection.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string full = (Convert.ToString(dr["FIRST_NAME"]) + " " + Convert.ToString(dr["LAST_NAME"])).Trim();
                            bool enabled = tfaAvailable && !dr.IsDBNull(3) && Convert.ToBoolean(dr["TotpEnabled"]);

                            rows.Add(new UserRow
                            {
                                UserName = Convert.ToString(dr["USER_NAME"]),
                                FullName = full,
                                TfaHtml = !tfaAvailable ? string.Empty : enabled
                                    ? "<span class=\"tfa-pill tfa-pill--ok\">" + HttpUtility.HtmlEncode(Global.Text("Tfa_On")) + "</span>"
                                    : "<span class=\"tfa-pill tfa-pill--warn\">" + HttpUtility.HtmlEncode(Global.Text("Tfa_Off")) + "</span>"
                            });
                        }
                    }
                }

                if (select != null)
                {
                    SelectedUser = select;
                }

                GridView_Users.DataSource = rows;
                GridView_Users.DataBind();

                GridView_Users.SelectedIndex = -1;
                for (int i = 0; i < rows.Count; i++)
                {
                    if (string.Equals(rows[i].UserName, SelectedUser, StringComparison.OrdinalIgnoreCase))
                    {
                        GridView_Users.SelectedIndex = i;
                        SelectedUser = rows[i].UserName;
                    }
                }
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        private void User_Load()
        {
            TextBox_User_Password.Text = string.Empty;
            TextBox_User_Password2.Text = string.Empty;
            Literal_User_Tfa.Text = string.Empty;
            Button_User_Reset2fa.Visible = false;

            string name = SelectedUser;

            if (string.IsNullOrEmpty(name))
            {
                TextBox_User_Name.Text = string.Empty;
                TextBox_User_FirstName.Text = string.Empty;
                TextBox_User_LastName.Text = string.Empty;
                Literal_FormTitle.Text = HttpUtility.HtmlEncode(Global.Text("Admin_UserNewTitle"));
                return;
            }

            Literal_FormTitle.Text = HttpUtility.HtmlEncode(Global.Format(Global.Text("Admin_UserEditTitle"), name));

            try
            {
                bool tfaAvailable = AdminLogin.TwoFactorAvailable();

                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(
                    "SELECT USER_NAME, FIRST_NAME, LAST_NAME" + (tfaAvailable ? ", TotpEnabled" : string.Empty) + " FROM USERS WHERE USER_NAME = @USER_NAME", connection))
                {
                    cmd.Parameters.Add("@USER_NAME", SqlDbType.VarChar, NameLength).Value = name;
                    connection.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            return;
                        }

                        TextBox_User_Name.Text = Convert.ToString(dr["USER_NAME"]);
                        TextBox_User_LastName.Text = Convert.ToString(dr["LAST_NAME"]);
                        TextBox_User_FirstName.Text = Convert.ToString(dr["FIRST_NAME"]);

                        if (tfaAvailable)
                        {
                            bool enabled = !dr.IsDBNull(3) && Convert.ToBoolean(dr["TotpEnabled"]);

                            Literal_User_Tfa.Text = enabled
                                ? "<span class=\"tfa-pill tfa-pill--ok\">" + HttpUtility.HtmlEncode(Global.Text("Tfa_On")) + "</span>"
                                : "<span class=\"tfa-pill tfa-pill--warn\">" + HttpUtility.HtmlEncode(Global.Text("Tfa_Off")) + "</span>";

                            // a user can only reset the second factor of somebody else (their own is managed below)
                            Button_User_Reset2fa.Visible = enabled && !IsSelf(name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        // ---- checks -----------------------------------------------------------------------------------------

        // null when the password is acceptable, otherwise the message to show
        private string PasswordProblem(string password, string confirmation)
        {
            if (password.Length < MinPasswordLength)
            {
                return Global.Format(Global.Text("Admin_UserPasswordTooShort"), MinPasswordLength);
            }

            if (!string.Equals(password, confirmation, StringComparison.Ordinal))
            {
                return Global.Text("Admin_UserPasswordMismatch");
            }

            return null;
        }

        // ---- users: save, add, delete, reset -------------------------------------------------------------------

        private void User_Save()
        {
            string name = SelectedUser;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            string password = TextBox_User_Password.Text;
            bool changePassword = password.Length > 0;

            if (changePassword)
            {
                string problem = PasswordProblem(password, TextBox_User_Password2.Text);
                if (problem != null)
                {
                    AdminUi.Toast(this, problem, false);
                    return;
                }
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(
                    (changePassword ? "UPDATE USERS SET PASSWORD=@PASSWORD, FIRST_NAME=@FIRST_NAME, LAST_NAME=@LAST_NAME " : "UPDATE USERS SET FIRST_NAME=@FIRST_NAME, LAST_NAME=@LAST_NAME ")
                    + "WHERE USER_NAME=@USER_NAME", connection))
                {
                    cmd.Parameters.Add("@USER_NAME", SqlDbType.VarChar, NameLength).Value = name;

                    if (changePassword)
                    {
                        cmd.Parameters.Add("@PASSWORD", SqlDbType.VarChar, 100).Value = PasswordHasher.Hash(password);
                    }

                    // parameters carry the text as it is: nothing to escape (doubling the apostrophes here stored "O''Brien")
                    cmd.Parameters.Add("@LAST_NAME", SqlDbType.VarChar, NameLength).Value = AdminLogin.Clip(TextBox_User_LastName.Text, NameLength);
                    cmd.Parameters.Add("@FIRST_NAME", SqlDbType.VarChar, NameLength).Value = AdminLogin.Clip(TextBox_User_FirstName.Text, NameLength);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                if (changePassword)
                {
                    AdminAudit.Write("User password changed", name);
                }

                BindUsers(null);
                User_Load();
                AdminUi.Toast(this, Global.Format(Global.Admin_UserSavedSuccess, name), true);
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        private void User_SaveNew()
        {
            string name = (TextBox_User_Name.Text ?? string.Empty).Trim();

            if (!NamePattern.IsMatch(name))
            {
                AdminUi.Toast(this, Global.Text("Admin_UserNameInvalid"), false);
                TextBox_User_Name.Focus();
                return;
            }

            string problem = PasswordProblem(TextBox_User_Password.Text, TextBox_User_Password2.Text);
            if (problem != null)
            {
                AdminUi.Toast(this, problem, false);
                return;
            }

            try
            {
                if (UserExists(name))
                {
                    AdminUi.Toast(this, Global.Format(Global.Admin_UserAlreadyUsedError, name), false);
                    TextBox_User_Name.Focus();
                    return;
                }

                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO USERS (USER_NAME, PASSWORD, FIRST_NAME, LAST_NAME) VALUES (@USER_NAME, @PASSWORD, @FIRST_NAME, @LAST_NAME)", connection))
                {
                    cmd.Parameters.Add("@USER_NAME", SqlDbType.VarChar, NameLength).Value = name;
                    cmd.Parameters.Add("@PASSWORD", SqlDbType.VarChar, 100).Value = PasswordHasher.Hash(TextBox_User_Password.Text);
                    cmd.Parameters.Add("@LAST_NAME", SqlDbType.VarChar, NameLength).Value = AdminLogin.Clip(TextBox_User_LastName.Text, NameLength);
                    cmd.Parameters.Add("@FIRST_NAME", SqlDbType.VarChar, NameLength).Value = AdminLogin.Clip(TextBox_User_FirstName.Text, NameLength);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                AdminAudit.Write("User created", name);

                ShowEditMode();
                BindUsers(name);
                User_Load();
                AdminUi.Toast(this, Global.Format(Global.Admin_UserSavedSuccess, name), true);
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        private void User_New()
        {
            Button_User_Save.Visible = false;
            Button_User_SaveNew.Visible = true;
            Button_User_Delete.Visible = false;
            Button_User_Reset2fa.Visible = false;
            TextBox_User_Name.ReadOnly = false;

            SelectedUser = string.Empty;
            GridView_Users.SelectedIndex = -1;
            User_Load();
            TextBox_User_Name.Focus();
        }

        private bool UserExists(string name)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM USERS WHERE USER_NAME = @USER", connection))
            {
                cmd.Parameters.Add("@USER", SqlDbType.VarChar, NameLength).Value = name;
                connection.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void User_Delete()
        {
            string name = SelectedUser;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (IsSelf(name))
            {
                AdminUi.Toast(this, Global.Text("Admin_UserCannotDeleteSelf"), false);
                return;
            }

            try
            {
                int total;

                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM USERS", connection))
                {
                    connection.Open();
                    total = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (total <= 1)
                {
                    AdminUi.Toast(this, Global.Text("Admin_UserCannotDeleteLast"), false);
                    return;
                }

                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("DELETE FROM USERS WHERE USER_NAME = @USER_NAME", connection))
                {
                    cmd.Parameters.Add("@USER_NAME", SqlDbType.VarChar, NameLength).Value = name;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                // the deleted user loses access at once, not within the minute the check is kept
                AdminLogin.ForgetUser(name);
                AdminAudit.Write("User deleted", name);

                BindUsers(_me);
                User_Load();
                AdminUi.Toast(this, Global.Format(Global.Admin_UserDeletedSuccess, name), true);
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        // Somebody lost their phone and their recovery codes: another administrator puts their account back to "two-factor off".
        private void User_Reset2fa()
        {
            string name = SelectedUser;

            if (string.IsNullOrEmpty(name) || IsSelf(name) || !AdminLogin.TwoFactorAvailable())
            {
                return;
            }

            try
            {
                AdminLogin.DisableTwoFactor(name);
                AdminAudit.Write("Two-factor reset", name);

                BindUsers(null);
                User_Load();
                AdminUi.Toast(this, Global.Format(Global.Text("Admin_UserReset2faDone"), name), true);
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        // ---- my two-factor sign-in --------------------------------------------------------------------------------

        protected void Button_Start_Click(object sender, EventArgs e)
        {
            Session[SetupKey] = Totp.NewSecret();
            ShowTfaState();
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            Session.Remove(SetupKey);
            ShowTfaState();
        }

        protected void Button_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                string secret = Session[SetupKey] as string;
                if (string.IsNullOrEmpty(secret))
                {
                    ShowTfaState();
                    return;
                }

                long step = Totp.Verify(secret, TextBox_Confirm.Text, DateTime.UtcNow, 0);
                if (step < 0)
                {
                    LabelMessage.Text = Global.Text("Tfa_BadCode");
                    ShowTfaState();
                    return;
                }

                List<string> codes = Totp.NewRecoveryCodes();

                // the secret, the first used step and the recovery codes are saved in ONE statement (see EnableTwoFactor)
                AdminLogin.EnableTwoFactor(_me, Totp.Protect(secret), step, HashesOf(codes));

                Session.Remove(SetupKey);
                Session["mfa"] = true;
                Session["mfa_offer"] = false;

                AdminAudit.Write("Two-factor turned on", _me);

                ShowCodes(codes, Global.Text("Tfa_Enabled"));
                ShowTfaState();

                // the table and the form show the new state; the management part appears when the session was restricted
                Panel_Management.Visible = !Restricted;
                if (Panel_Management.Visible)
                {
                    ShowEditMode();
                    BindUsers(_me);
                    User_Load();
                }
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        protected void Button_Regenerate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentCodeIsRight())
                {
                    return;
                }

                List<string> codes = Totp.NewRecoveryCodes();

                // the old list is replaced as a whole: the old codes stop working
                AdminLogin.SaveRecovery(_me, AdminLogin.LoadTwoFactor(_me).Recovery, HashesOf(codes));
                AdminAudit.Write("Two-factor recovery codes renewed", _me);

                ShowCodes(codes, Global.Text("Tfa_NewCodes"));
                ShowTfaState();
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        protected void Button_Disable_Click(object sender, EventArgs e)
        {
            try
            {
                if (AdminLogin.RequireTwoFactor)
                {
                    LabelMessage.Text = Global.Text("Tfa_Required");
                    ShowTfaState();
                    return;
                }

                if (!CurrentCodeIsRight())
                {
                    return;
                }

                AdminLogin.DisableTwoFactor(_me);
                Session["mfa"] = false;
                Session["mfa_offer"] = true;
                AdminAudit.Write("Two-factor turned off", _me);

                LiteralNotice.Text = "<div class=\"tfa-warn\">" + HttpUtility.HtmlEncode(Global.Text("Tfa_Disabled")) + "</div>";
                ShowTfaState();

                if (Panel_Management.Visible)
                {
                    BindUsers(null);
                    User_Load();
                }
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        private string HashesOf(List<string> codes)
        {
            var hashes = new StringBuilder();

            foreach (string code in codes)
            {
                if (hashes.Length > 0)
                {
                    hashes.Append(',');
                }

                hashes.Append(Totp.HashRecovery(_me, code));
            }

            return hashes.ToString();
        }

        // the current code of the app proves the person at the keyboard has the phone; false (with the message shown) when it does not
        private bool CurrentCodeIsRight()
        {
            AdminLogin.TwoFactorState state = AdminLogin.LoadTwoFactor(_me);
            if (state == null || !state.Enabled)
            {
                ShowTfaState();
                return false;
            }

            string secret = Totp.Unprotect(state.ProtectedSecret);
            long step = Totp.Verify(secret, TextBox_Current.Text, DateTime.UtcNow, state.LastStep);

            if (step < 0 || !AdminLogin.UseStep(_me, step))
            {
                LabelMessage.Text = Global.Text("Tfa_BadCurrent");
                ShowTfaState();
                return false;
            }

            return true;
        }

        // the codes are shown ONCE: only their hashes are kept
        private void ShowCodes(List<string> codes, string title)
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"tfa-ok\"><strong>").Append(HttpUtility.HtmlEncode(title)).Append("</strong>");
            sb.Append("<p>").Append(HttpUtility.HtmlEncode(Global.Text("Tfa_CodesIntro"))).Append("</p>");

            // one code per line (a plain-text block, so a copy and paste keeps the lines apart): 8 characters, a dash in the middle
            sb.Append("<pre id=\"tfa-codes-text\" class=\"tfa-codes\">");
            foreach (string code in codes)
            {
                sb.Append(HttpUtility.HtmlEncode(code)).Append('\n');
            }

            sb.Append("</pre>");
            sb.Append("<p class=\"tfa-hint\">").Append(HttpUtility.HtmlEncode(Global.Text("Tfa_CodesFormat"))).Append("</p>");

            string copied = Global.Text("Tfa_Copied").Replace("\\", "\\\\").Replace("'", "\\'");
            sb.Append("<button type=\"button\" class=\"button button__sm\" onclick=\"var t = document.getElementById('tfa-codes-text').innerText; "
                + "if (navigator.clipboard) { navigator.clipboard.writeText(t); this.textContent = '" + HttpUtility.HtmlAttributeEncode(copied) + "'; }\">"
                + HttpUtility.HtmlEncode(Global.Text("Tfa_CopyButton")) + "</button>");
            sb.Append("</div>");
            LiteralNotice.Text = sb.ToString();
        }

        private void ShowTfaState()
        {
            AdminLogin.TwoFactorState state = AdminLogin.LoadTwoFactor(_me);
            bool enabled = state != null && state.Enabled;
            string setup = Session[SetupKey] as string;

            Panel_Manage.Visible = enabled;
            Panel_Start.Visible = !enabled && string.IsNullOrEmpty(setup);
            Panel_Confirm.Visible = !enabled && !string.IsNullOrEmpty(setup);

            if (Panel_Confirm.Visible)
            {
                Literal_Account.Text = HttpUtility.HtmlEncode(_me);
                Literal_Secret.Text = HttpUtility.HtmlEncode(Totp.Grouped(setup));
                Literal_Link.Text = "<a href=\"" + HttpUtility.HtmlAttributeEncode(Totp.OtpAuthUri(_me, setup)) + "\">" + HttpUtility.HtmlEncode(Global.Text("Tfa_LinkText")) + "</a>";
                TextBox_Confirm.Text = string.Empty;
            }

            TextBox_Current.Text = string.Empty;
        }

        // the details go to the error log (App_Data\logs), never to the browser
        private void Fail(Exception ex)
        {
            ErrorLogger.Log(ex, "users page");
            AdminUi.Toast(this, Global.Text("Admin_Error"), false);
        }

        // ---- events of the users part -------------------------------------------------------------------------------

        protected void GridView_Users_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Restricted)
            {
                return;
            }

            SelectedUser = Convert.ToString(GridView_Users.SelectedDataKey.Value);
            ShowEditMode();
            User_Load();
        }

        protected void Button_User_Save_Click(object sender, EventArgs e)
        {
            if (!Restricted)
            {
                User_Save();
            }
        }

        protected void Button_User_SaveNew_Click(object sender, EventArgs e)
        {
            if (!Restricted)
            {
                User_SaveNew();
            }
        }

        protected void Button_User_New_Click(object sender, EventArgs e)
        {
            if (!Restricted)
            {
                User_New();
            }
        }

        protected void Button_User_Delete_Click(object sender, EventArgs e)
        {
            if (!Restricted)
            {
                User_Delete();
            }
        }

        protected void Button_User_Reset2fa_Click(object sender, EventArgs e)
        {
            if (!Restricted)
            {
                User_Reset2fa();
            }
        }
    }
}
