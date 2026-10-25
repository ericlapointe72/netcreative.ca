using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;

namespace netcreative.ca
{
    // The pieces around signing in to the admin:
    //  - a limit of failed sign-ins per network address and per user name (kept in memory: a visitor cannot reset it by
    //    dropping their session cookie or by reloading the sign-in page, which the old counter in the session allowed)
    //  - a "ticket" that gives the visitor a NEW session when they sign in (the session that existed before signing in is
    //    thrown away, so nobody can plant a session identifier on a browser and use it afterwards)
    //  - a check that the signed-in user still exists (a user deleted in "Users" loses access within a minute)
    //  - the real network address of the visitor, and the line written to the LOGIN table
    public static class AdminLogin
    {
        private const int MaxFailuresPerAddress = 10;
        private const int MaxFailuresPerUser = 20;
        private const int FailureWindowMinutes = 15;
        private const int TicketSeconds = 60;
        private const int ActiveCheckSeconds = 60;

        // the size of the columns of the LOGIN table that hold what a visitor typed or sent
        private const int UserNameLength = 20;
        private const int IpLength = 45;

        private static readonly object Sync = new object();

        // ---- failed attempts ----------------------------------------------------------------------------

        public static bool Blocked(string ip, string userName)
        {
            lock (Sync)
            {
                return RecentFailures("ip_" + ip).Count >= MaxFailuresPerAddress
                    || RecentFailures("user_" + NormalizeUser(userName)).Count >= MaxFailuresPerUser;
            }
        }

        public static void RecordFailure(string ip, string userName)
        {
            lock (Sync)
            {
                Add("ip_" + ip);
                Add("user_" + NormalizeUser(userName));
            }
        }

        public static void ClearFailures(string ip, string userName)
        {
            lock (Sync)
            {
                HttpRuntime.Cache.Remove("nc_login_fail_ip_" + ip);
                HttpRuntime.Cache.Remove("nc_login_fail_user_" + NormalizeUser(userName));
            }
        }

        private static void Add(string key)
        {
            var list = RecentFailures(key);
            list.Add(DateTime.UtcNow);
            HttpRuntime.Cache.Insert("nc_login_fail_" + key, list, null, DateTime.UtcNow.AddMinutes(FailureWindowMinutes), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        private static List<DateTime> RecentFailures(string key)
        {
            var list = HttpRuntime.Cache["nc_login_fail_" + key] as List<DateTime>;
            if (list == null)
            {
                return new List<DateTime>();
            }

            DateTime limit = DateTime.UtcNow.AddMinutes(-FailureWindowMinutes);
            return list.Where(t => t > limit).ToList();
        }

        private static string NormalizeUser(string userName)
        {
            return (userName ?? string.Empty).Trim().ToLowerInvariant();
        }

        // ---- ticket for the new session -------------------------------------------------------------------

        public sealed class Ticket
        {
            public string UserName;
            public string Language;

            // the second factor (a code from the authenticator app) was given in this sign-in
            public bool TwoFactorDone;

            // the user can use two-factor sign-in but has not turned it on: the admin shows a reminder
            public bool TwoFactorOffered;
        }

        public static string CreateTicket(string userName, string language, bool twoFactorDone, bool twoFactorOffered)
        {
            string token = RandomToken();

            HttpRuntime.Cache.Insert("nc_login_ticket_" + token,
                new Ticket { UserName = userName, Language = language, TwoFactorDone = twoFactorDone, TwoFactorOffered = twoFactorOffered },
                null, DateTime.UtcNow.AddSeconds(TicketSeconds), System.Web.Caching.Cache.NoSlidingExpiration);

            return token;
        }

        // one use only: the ticket is removed as it is read
        public static Ticket RedeemTicket(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || token.Length > 64)
            {
                return null;
            }

            string key = "nc_login_ticket_" + token;

            lock (Sync)
            {
                var ticket = HttpRuntime.Cache[key] as Ticket;
                HttpRuntime.Cache.Remove(key);
                return ticket;
            }
        }

        public static string RandomToken()
        {
            var bytes = new byte[24];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        // ---- is the signed-in user still allowed? --------------------------------------------------------------

        public static bool IsUserStillActive(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            string key = "nc_user_active_" + NormalizeUser(userName);
            var cached = HttpRuntime.Cache[key] as bool?;
            if (cached.HasValue)
            {
                return cached.Value;
            }

            bool active;

            try
            {
                using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM USERS WHERE USER_NAME = @u;", cn))
                {
                    cmd.Parameters.Add("@u", SqlDbType.VarChar, UserNameLength).Value = Clip(userName, UserNameLength);
                    cn.Open();
                    active = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            catch (Exception)
            {
                // if the database cannot be read right now, the visitor is not thrown out for that
                return true;
            }

            HttpRuntime.Cache.Insert(key, active, null, DateTime.UtcNow.AddSeconds(ActiveCheckSeconds), System.Web.Caching.Cache.NoSlidingExpiration);
            return active;
        }

        // a change made in "Users" takes effect at once
        public static void ForgetUser(string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                HttpRuntime.Cache.Remove("nc_user_active_" + NormalizeUser(userName));
            }
        }

        // ---- second factor (see Totp) -----------------------------------------------------------------------------

        private const int PendingMinutes = 5;
        private const int MaxCodeAttempts = 5;

        // The sign-in waits for the code of the authenticator app: the password was right, nothing is signed in yet.
        public sealed class PendingTwoFactor
        {
            public string UserName;
            public string Language;
            public string Ip;
            public int Attempts;
        }

        // true once the four columns of two-factor sign-in exist in USERS (the script sql\2026-10-22_users_two_factor.sql adds them).
        // Until then nobody is asked for a code: the sign-in works as before, so the code can be deployed first.
        public static bool TwoFactorAvailable()
        {
            var cached = HttpRuntime.Cache["nc_2fa_schema"] as bool?;
            if (cached.HasValue)
            {
                return cached.Value;
            }

            bool available;

            try
            {
                using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                using (var cmd = new SqlCommand(
                    "SELECT CASE WHEN COL_LENGTH('dbo.USERS', 'TotpSecret') IS NOT NULL AND COL_LENGTH('dbo.USERS', 'TotpEnabled') IS NOT NULL"
                    + " AND COL_LENGTH('dbo.USERS', 'TotpLastStep') IS NOT NULL AND COL_LENGTH('dbo.USERS', 'TotpRecovery') IS NOT NULL THEN 1 ELSE 0 END;", cn))
                {
                    cn.Open();
                    available = Convert.ToInt32(cmd.ExecuteScalar()) == 1;
                }
            }
            catch (Exception)
            {
                return false;
            }

            HttpRuntime.Cache.Insert("nc_2fa_schema", available, null, DateTime.UtcNow.AddMinutes(available ? 60 : 2), System.Web.Caching.Cache.NoSlidingExpiration);
            return available;
        }

        // Optional: <add key="AdminRequireTwoFactor" value="true" /> in the secrets file makes the code compulsory (a user who has not
        // set it up yet is taken to the setup page and cannot open anything else in the admin).
        public static bool RequireTwoFactor
        {
            get { return string.Equals((ConfigurationManager.AppSettings["AdminRequireTwoFactor"] ?? string.Empty).Trim(), "true", StringComparison.OrdinalIgnoreCase); }
        }

        public sealed class TwoFactorState
        {
            public bool Enabled;
            public string ProtectedSecret;
            public long LastStep;
            public string Recovery;
        }

        // null when the user does not exist
        public static TwoFactorState LoadTwoFactor(string userName)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            using (var cmd = new SqlCommand("SELECT TotpEnabled, TotpSecret, ISNULL(TotpLastStep, 0), TotpRecovery FROM dbo.USERS WHERE USER_NAME = @U;", cn))
            {
                cmd.Parameters.Add("@U", SqlDbType.VarChar, UserNameLength).Value = Clip(userName, UserNameLength);
                cn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                    {
                        return null;
                    }

                    return new TwoFactorState
                    {
                        Enabled = !r.IsDBNull(0) && r.GetBoolean(0),
                        ProtectedSecret = r.IsDBNull(1) ? null : r.GetString(1),
                        LastStep = r.GetInt64(2),
                        Recovery = r.IsDBNull(3) ? null : r.GetString(3)
                    };
                }
            }
        }

        // A time step can be used once: false when it was already used (two requests with the same code at the same time, or a replay).
        public static bool UseStep(string userName, long step)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            using (var cmd = new SqlCommand("UPDATE dbo.USERS SET TotpLastStep = @Step WHERE USER_NAME = @U AND (TotpLastStep IS NULL OR TotpLastStep < @Step);", cn))
            {
                cmd.Parameters.Add("@Step", SqlDbType.BigInt).Value = step;
                cmd.Parameters.Add("@U", SqlDbType.VarChar, UserNameLength).Value = Clip(userName, UserNameLength);
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        // A recovery code is used once: false when the list changed meanwhile (the same code sent twice at the same time).
        public static bool SaveRecovery(string userName, string expectedOld, string recovery)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            using (var cmd = new SqlCommand("UPDATE dbo.USERS SET TotpRecovery = @New WHERE USER_NAME = @U AND ISNULL(TotpRecovery, N'') = ISNULL(@Old, N'');", cn))
            {
                cmd.Parameters.Add("@New", SqlDbType.NVarChar, 600).Value = (object)recovery ?? DBNull.Value;
                cmd.Parameters.Add("@Old", SqlDbType.NVarChar, 600).Value = (object)expectedOld ?? DBNull.Value;
                cmd.Parameters.Add("@U", SqlDbType.VarChar, UserNameLength).Value = Clip(userName, UserNameLength);
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public static void EnableTwoFactor(string userName, string protectedSecret, long usedStep, string recovery)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            using (var cmd = new SqlCommand(
                "UPDATE dbo.USERS SET TotpSecret = @Secret, TotpEnabled = 1, TotpLastStep = @Step, TotpRecovery = @Recovery WHERE USER_NAME = @U;", cn))
            {
                cmd.Parameters.Add("@Secret", SqlDbType.NVarChar, 400).Value = protectedSecret;
                cmd.Parameters.Add("@Step", SqlDbType.BigInt).Value = usedStep;
                cmd.Parameters.Add("@Recovery", SqlDbType.NVarChar, 600).Value = recovery;
                cmd.Parameters.Add("@U", SqlDbType.VarChar, UserNameLength).Value = Clip(userName, UserNameLength);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DisableTwoFactor(string userName)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            using (var cmd = new SqlCommand("UPDATE dbo.USERS SET TotpSecret = NULL, TotpEnabled = 0, TotpLastStep = NULL, TotpRecovery = NULL WHERE USER_NAME = @U;", cn))
            {
                cmd.Parameters.Add("@U", SqlDbType.VarChar, UserNameLength).Value = Clip(userName, UserNameLength);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // the sign-in that waits for the code: a random token (kept by the page), valid 5 minutes, only from the same address
        public static string CreatePending(string userName, string language, string ip)
        {
            string token = RandomToken();

            HttpRuntime.Cache.Insert("nc_2fa_pending_" + token,
                new PendingTwoFactor { UserName = userName, Language = language, Ip = ip },
                null, DateTime.UtcNow.AddMinutes(PendingMinutes), System.Web.Caching.Cache.NoSlidingExpiration);

            return token;
        }

        public static PendingTwoFactor GetPending(string token, string ip)
        {
            if (string.IsNullOrWhiteSpace(token) || token.Length > 64)
            {
                return null;
            }

            var pending = HttpRuntime.Cache["nc_2fa_pending_" + token] as PendingTwoFactor;
            return pending != null && pending.Ip == ip ? pending : null;
        }

        // a wrong code: after 5 of them the sign-in is dropped and the password has to be typed again
        public static void PendingFailed(string token)
        {
            lock (Sync)
            {
                var pending = HttpRuntime.Cache["nc_2fa_pending_" + token] as PendingTwoFactor;
                if (pending == null)
                {
                    return;
                }

                pending.Attempts++;
                if (pending.Attempts >= MaxCodeAttempts)
                {
                    HttpRuntime.Cache.Remove("nc_2fa_pending_" + token);
                }
            }
        }

        public static void RemovePending(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                HttpRuntime.Cache.Remove("nc_2fa_pending_" + token);
            }
        }

        // ---- the visitor's address ------------------------------------------------------------------------------

        // The address of the machine that connected. Behind a proxy of the host (a private address) the proxy adds the visitor's address at
        // the END of X-Forwarded-For; what comes before it was written by the visitor and is never trusted (it could name any address).
        public static string ClientIp(HttpContext context)
        {
            string remote = context.Request.ServerVariables["REMOTE_ADDR"] ?? string.Empty;

            if (IsPrivate(remote))
            {
                string forwarded = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrWhiteSpace(forwarded))
                {
                    string[] parts = forwarded.Split(',');
                    string last = parts[parts.Length - 1].Trim();
                    IPAddress parsed;

                    if (IPAddress.TryParse(last, out parsed))
                    {
                        return Clip(parsed.ToString(), IpLength);
                    }
                }
            }

            return Clip(remote, IpLength);
        }

        private static bool IsPrivate(string address)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(address, out ip))
            {
                return false;
            }

            if (IPAddress.IsLoopback(ip))
            {
                return true;
            }

            byte[] b = ip.GetAddressBytes();

            if (b.Length == 4)
            {
                return b[0] == 10 || (b[0] == 172 && b[1] >= 16 && b[1] <= 31) || (b[0] == 192 && b[1] == 168) || (b[0] == 169 && b[1] == 254);
            }

            // IPv6: unique local (fc00::/7) and link local (fe80::/10)
            return (b[0] & 0xFE) == 0xFC || (b[0] == 0xFE && (b[1] & 0xC0) == 0x80);
        }

        // ---- the LOGIN table --------------------------------------------------------------------------------------

        // One line for the sign-in log. What a visitor typed is cut to the size of the column (a longer text made the insert fail and the
        // failed attempt was not logged at all) and stripped of control characters. Never throws: a log that cannot be written must not
        // stop a sign-in or a sign-out.
        public static void LogEvent(string userName, string action)
        {
            try
            {
                using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO LOGIN (DATE, TIME, IP_ADDRESS, USER_NAME, ACTION) VALUES (@DATE, @TIME, @IP_ADDRESS, @USER_NAME, @ACTION)", cn))
                {
                    cmd.Parameters.AddWithValue("@DATE", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@TIME", DateTime.Now.TimeOfDay);
                    cmd.Parameters.AddWithValue("@IP_ADDRESS", ClientIp(HttpContext.Current));
                    cmd.Parameters.AddWithValue("@USER_NAME", Clip(userName, UserNameLength));
                    cmd.Parameters.AddWithValue("@ACTION", action);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "sign-in log");
            }
        }

        // Once a day (at the first sign-in after the application starts, at most): the rows of the sign-in log older than 12 months are
        // erased. They hold network addresses, which are personal information; nothing needs them for longer than a year.
        public static void PurgeOldLogOncePerDay()
        {
            if (HttpRuntime.Cache["nc_login_purged"] != null)
            {
                return;
            }

            HttpRuntime.Cache.Insert("nc_login_purged", true, null, DateTime.UtcNow.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);

            try
            {
                using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                using (var cmd = new SqlCommand("DELETE FROM LOGIN WHERE DATE < DATEADD(month, -12, CAST(GETDATE() AS date));", cn))
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "sign-in log purge");
            }
        }

        // what a visitor typed, as text that is safe to keep: no control characters, at most 'max' characters
        public static string Clip(string text, int max)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var chars = text.Where(c => !char.IsControl(c)).Take(max).ToArray();
            return new string(chars);
        }
    }
}
