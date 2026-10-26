using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // Signing in, called by login.js, in one or two steps:
    //   1. user name and password. When they are right and the user has turned on two-factor sign-in, the answer is "needCode" with a
    //      token that stands for this half-finished sign-in; otherwise the sign-in is complete.
    //   2. the token and the code of the authenticator app (or a recovery code).
    // A complete sign-in does NOT sign anyone in here: it throws the current session away and sends the browser to LoginComplete.ashx
    // with a one-use ticket, which starts the signed-in session as a NEW one.
    public class LoginHandler : IHttpHandler, IRequiresSessionState
    {
        private const int UserNameLength = 20;
        private const int PasswordMaxLength = 200;
        private const int CodeMaxLength = 20;

        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 405;
                return;
            }

            // a request that arrives without a session language (the sign-in page was never opened in this browser) uses the default one
            string language = context.Session == null ? null : context.Session["language"] as string;
            if (string.IsNullOrEmpty(language))
            {
                language = ConfigurationManager.AppSettings["app_language"] ?? "fr";
            }

            Global.SetCulture(language);

            var answer = new Answer();

            try
            {
                string pending = context.Request.Form["pending"];

                if (string.IsNullOrEmpty(pending))
                {
                    StepOne(context, language, answer);
                }
                else
                {
                    StepTwo(context, language, pending, answer);
                }
            }
            catch (Exception ex)
            {
                // the details go to the error log, never to the browser
                ErrorLogger.Log(ex, "sign-in");
                answer = new Answer { message = Global.Login_NetworkError };
            }

            if (answer.blocked)
            {
                context.Response.StatusCode = 429;
            }

            context.Response.ContentType = "application/json";
            context.Response.Write(new JavaScriptSerializer().Serialize(new { answer.success, answer.message, answer.redirect, answer.needCode, answer.pending, answer.restart }));
        }

        private sealed class Answer
        {
            public bool success;
            public string message = string.Empty;
            public string redirect;
            public bool needCode;
            public string pending;
            public bool restart;
            public bool blocked;
        }

        // ---- step 1: user name and password -------------------------------------------------------------------

        private static void StepOne(HttpContext context, string language, Answer answer)
        {
            string ip = AdminLogin.ClientIp(context);
            string userName = AdminLogin.Clip(context.Request.Form["user"], UserNameLength).Trim();
            string password = context.Request.Form["password"] ?? string.Empty;
            if (password.Length > PasswordMaxLength)
            {
                password = password.Substring(0, PasswordMaxLength);
            }

            if (AdminLogin.Blocked(ip, userName))
            {
                // too many failures from this address (or on this user name): not even the password is checked
                answer.blocked = true;
                answer.message = Global.Login_BlockedMessage;
                return;
            }

            string canonicalName;

            if (!CheckPassword(userName, password, out canonicalName))
            {
                AdminLogin.RecordFailure(ip, userName);
                AdminLogin.LogEvent(userName, "FAIL");
                answer.message = Global.Login_ErrorMessage;
                return;
            }

            bool offered = false;

            if (AdminLogin.TwoFactorAvailable())
            {
                AdminLogin.TwoFactorState state = AdminLogin.LoadTwoFactor(canonicalName);

                if (state != null && state.Enabled)
                {
                    // the password is right, the code is still missing: nothing is signed in, nothing is logged as a success yet
                    answer.needCode = true;
                    answer.pending = AdminLogin.CreatePending(canonicalName, language, ip);
                    return;
                }

                offered = true;
            }

            Complete(context, ip, userName, canonicalName, language, false, offered, answer);
        }

        // ---- step 2: the code ------------------------------------------------------------------------------------

        private static void StepTwo(HttpContext context, string language, string token, Answer answer)
        {
            string ip = AdminLogin.ClientIp(context);
            string typed = AdminLogin.Clip(context.Request.Form["code"], CodeMaxLength).Trim();

            AdminLogin.PendingTwoFactor pending = AdminLogin.GetPending(token, ip);

            if (pending == null)
            {
                // expired, dropped after too many wrong codes, or presented from another address: start again
                answer.restart = true;
                answer.message = Global.Login_CodeExpired;
                return;
            }

            if (AdminLogin.Blocked(ip, pending.UserName))
            {
                answer.blocked = true;
                answer.message = Global.Login_BlockedMessage;
                return;
            }

            AdminLogin.TwoFactorState state = AdminLogin.LoadTwoFactor(pending.UserName);

            if (state == null || !state.Enabled)
            {
                AdminLogin.RemovePending(token);
                answer.restart = true;
                answer.message = Global.Login_CodeExpired;
                return;
            }

            bool ok = false;

            if (Totp.LooksLikeRecovery(typed))
            {
                // a recovery code works once: the list without it is saved, and only the request that saved it is let in
                string remaining = Totp.ConsumeRecovery(pending.UserName, state.Recovery, typed);
                ok = remaining != null && AdminLogin.SaveRecovery(pending.UserName, state.Recovery, remaining);
            }
            else
            {
                string secret = Totp.Unprotect(state.ProtectedSecret);
                long step = Totp.Verify(secret, typed, DateTime.UtcNow, state.LastStep);
                ok = step >= 0 && AdminLogin.UseStep(pending.UserName, step);
            }

            if (!ok)
            {
                AdminLogin.RecordFailure(ip, pending.UserName);
                AdminLogin.PendingFailed(token);
                AdminLogin.LogEvent(pending.UserName, "FAIL");
                answer.message = Global.Login_CodeError;
                answer.pending = token;
                answer.needCode = true;
                return;
            }

            AdminLogin.RemovePending(token);
            Complete(context, ip, pending.UserName, pending.UserName, pending.Language, true, false, answer);
        }

        // ---- the sign-in is complete -------------------------------------------------------------------------------

        private static void Complete(HttpContext context, string ip, string typedName, string canonicalName, string language, bool twoFactorDone, bool offered, Answer answer)
        {
            AdminLogin.ClearFailures(ip, typedName);
            AdminLogin.ClearFailures(ip, canonicalName);
            AdminLogin.LogEvent(canonicalName, "LOGIN");
            AdminLogin.PurgeOldLogOncePerDay();
            DataRetention.PurgeOldContactsOncePerDay();

            string ticket = AdminLogin.CreateTicket(canonicalName, language == "en" ? "en" : "fr", twoFactorDone, offered);

            // the session that existed before signing in is thrown away: the signed-in one is created by LoginComplete.ashx
            context.Session.Abandon();
            context.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty) { Expires = DateTime.UtcNow.AddYears(-1), HttpOnly = true, Path = "/" });

            answer.success = true;
            answer.redirect = VirtualPathUtility.ToAbsolute("~/Admin/LoginComplete.ashx") + "?t=" + HttpUtility.UrlEncode(ticket);
        }

        // true when the user exists and the password is right. A user name that does not exist costs the same time as one that does
        // (a password is hashed either way), so the time taken does not tell which user names exist.
        private static bool CheckPassword(string userName, string password, out string canonicalName)
        {
            canonicalName = null;
            string stored = null;

            if (userName.Length > 0)
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                using (var cmd = new SqlCommand("SELECT USER_NAME, PASSWORD FROM USERS WHERE USER_NAME = @USER_NAME", connection))
                {
                    cmd.Parameters.Add("@USER_NAME", SqlDbType.VarChar, UserNameLength).Value = userName;
                    connection.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            canonicalName = Convert.ToString(dr["USER_NAME"]);
                            stored = Convert.ToString(dr["PASSWORD"]);
                        }
                    }
                }
            }

            if (stored == null)
            {
                PasswordHasher.VerifyDummy(password);
                return false;
            }

            return PasswordHasher.Verify(password, stored);
        }
    }
}
