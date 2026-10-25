using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public enum SubscribeResult
    {
        // a confirmation e-mail was sent (or one was already sent less than a day ago): the address is NOT subscribed yet
        Pending,
        // the address is already subscribed and confirmed
        Already,
        TooMany
    }

    // The newsletter list (table SUBSCRIBE), with double confirmation like beaucedutyfree.com: an address only receives the newsletter after
    // its owner clicked the link of the e-mail the site sent to it (CONFIRMED is set then). Nobody can subscribe somebody else's address.
    // Unsubscribing needs a link signed by the site (UnsubscribeLink), which every newsletter carries; whoever lost it asks for a new one.
    public static class SubscribeService
    {
        // sign-ups accepted from one IP address per hour, and from everybody together: nobody signs up many people in an hour,
        // a script that fills the list does
        private const int MaxPerHourPerIp = 10;
        private const int MaxPerHourAll = 40;

        // an address gets one confirmation e-mail per day at most (nobody can flood a mailbox through the form)
        private const int ResendAfterHours = 24;

        // how long the link of the confirmation e-mail stays valid
        private const int LinkValidDays = 7;

        // links of "unsubscribe" asked for from one IP address per hour, and for one address per day
        private const int MaxLinkRequestsPerHourPerIp = 5;
        private const int MaxLinkRequestsPerDayPerAddress = 3;

        private static readonly Regex TokenPattern = new Regex("^[0-9a-f]{32}$", RegexOptions.Compiled);

        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString; }
        }

        // ---- the sign-up ----

        public static SubscribeResult Subscribe(string email, string ip)
        {
            string token;
            SubscribeResult result = Register(email, ip, out token);

            if (result == SubscribeResult.Pending && token != null)
            {
                SendConfirmation(email, token);
            }

            return result;
        }

        // The database part. token is set only when a confirmation e-mail has to be sent now.
        private static SubscribeResult Register(string email, string ip, out string token)
        {
            token = null;
            email = (email ?? string.Empty).Trim().ToLowerInvariant();

            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                PurgeUnconfirmedOncePerDay(cn);

                DateTime now = DateTime.Now;
                DateTime nowUtc = DateTime.UtcNow;

                // 1) limits (a sign-up and a new confirmation e-mail both count)
                using (var cmd = new SqlCommand(@"
SELECT
    (SELECT COUNT(*) FROM SUBSCRIBE WHERE IP_ADDRESS = @Ip AND (CAST([DATE] AS datetime) + CAST([TIME] AS datetime) >= @Since OR CONFIRM_SENT >= @SinceUtc)),
    (SELECT COUNT(*) FROM SUBSCRIBE WHERE CAST([DATE] AS datetime) + CAST([TIME] AS datetime) >= @Since OR CONFIRM_SENT >= @SinceUtc);", cn))
                {
                    cmd.Parameters.Add("@Ip", SqlDbType.VarChar, 50).Value = (object)ip ?? DBNull.Value;
                    cmd.Parameters.Add("@Since", SqlDbType.DateTime).Value = now.AddHours(-1);
                    cmd.Parameters.Add("@SinceUtc", SqlDbType.DateTime2).Value = nowUtc.AddHours(-1);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        r.Read();
                        if ((!string.IsNullOrEmpty(ip) && r.GetInt32(0) >= MaxPerHourPerIp) || r.GetInt32(1) >= MaxPerHourAll)
                        {
                            return SubscribeResult.TooMany;
                        }
                    }
                }

                // 2) is this address already known?
                int id = 0;
                bool exists = false;
                bool active = false;
                bool waiting = false;

                using (var cmd = new SqlCommand(
                    "SELECT TOP 1 AUTO_NUMBER, CONFIRMED, CONFIRM_TOKEN, CONFIRM_SENT FROM SUBSCRIBE WHERE EMAIL = @Email ORDER BY AUTO_NUMBER", cn))
                {
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 254).Value = email;

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            exists = true;
                            id = r.GetInt32(0);
                            active = !r.IsDBNull(1);
                            waiting = !r.IsDBNull(2) && !r.IsDBNull(3) && r.GetDateTime(3) >= nowUtc.AddHours(-ResendAfterHours);
                        }
                    }
                }

                if (exists && active)
                {
                    return SubscribeResult.Already;
                }

                if (exists && waiting)
                {
                    // a confirmation e-mail left less than a day ago: the answer is the same, but nothing is sent again
                    return SubscribeResult.Pending;
                }

                string newToken = NewToken();

                // 3) a new row, or a new link for the row that waits for its confirmation
                if (exists)
                {
                    using (var cmd = new SqlCommand("UPDATE SUBSCRIBE SET CONFIRM_TOKEN = @Token, CONFIRM_SENT = @NowUtc, IP_ADDRESS = @Ip WHERE AUTO_NUMBER = @Id", cn))
                    {
                        cmd.Parameters.Add("@Token", SqlDbType.Char, 32).Value = newToken;
                        cmd.Parameters.Add("@NowUtc", SqlDbType.DateTime2).Value = nowUtc;
                        cmd.Parameters.Add("@Ip", SqlDbType.VarChar, 50).Value = (object)ip ?? DBNull.Value;
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(
                        "INSERT INTO SUBSCRIBE (DATE, TIME, IP_ADDRESS, EMAIL, CONFIRM_TOKEN, CONFIRM_SENT) VALUES (@Date, @Time, @Ip, @Email, @Token, @NowUtc)", cn))
                    {
                        cmd.Parameters.Add("@Date", SqlDbType.Date).Value = now.Date;
                        cmd.Parameters.Add("@Time", SqlDbType.Time).Value = now.TimeOfDay;
                        cmd.Parameters.Add("@Ip", SqlDbType.VarChar, 50).Value = (object)ip ?? DBNull.Value;
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 254).Value = email;
                        cmd.Parameters.Add("@Token", SqlDbType.Char, 32).Value = newToken;
                        cmd.Parameters.Add("@NowUtc", SqlDbType.DateTime2).Value = nowUtc;
                        cmd.ExecuteNonQuery();
                    }
                }

                token = newToken;
                return SubscribeResult.Pending;
            }
        }

        // Once a day (at the first sign-up after the application starts, at most): the addresses that never confirmed are erased once their
        // link is a month old. They were typed by somebody, maybe not by their owner; nothing needs to keep them.
        private static void PurgeUnconfirmedOncePerDay(SqlConnection cn)
        {
            if (HttpRuntime.Cache["nc_subscribe_purged"] != null)
            {
                return;
            }

            HttpRuntime.Cache.Insert("nc_subscribe_purged", true, null, DateTime.UtcNow.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);

            try
            {
                using (var cmd = new SqlCommand("DELETE FROM SUBSCRIBE WHERE CONFIRMED IS NULL AND CONFIRM_SENT < DATEADD(day, -30, SYSUTCDATETIME())", cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "unconfirmed sign-ups purge");
            }
        }

        // ---- the confirmation (the page of the link) ----

        // The address that was just confirmed, or null when the link is not valid (already used, older than a week, or invented).
        public static string Confirm(string token)
        {
            token = (token ?? string.Empty).Trim().ToLowerInvariant();
            if (!TokenPattern.IsMatch(token))
            {
                return null;
            }

            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(@"
UPDATE SUBSCRIBE
SET CONFIRMED = SYSUTCDATETIME(), CONFIRM_TOKEN = NULL
OUTPUT INSERTED.EMAIL
WHERE CONFIRM_TOKEN = @Token AND CONFIRMED IS NULL AND CONFIRM_SENT >= DATEADD(day, -" + LinkValidDays + @", SYSUTCDATETIME());", cn))
            {
                cmd.Parameters.Add("@Token", SqlDbType.Char, 32).Value = token;
                cn.Open();

                object email = cmd.ExecuteScalar();
                return email == null ? null : Convert.ToString(email);
            }
        }

        // an FYI for the owner, once the address is confirmed (not at every attempt: a robot cannot make the site mail the owner)
        public static void NotifyOwner(string email)
        {
            // the texts are looked up here, on the visitor's thread (the thread below has no culture of its own)
            string subject = Global.Master_MailSubjectNewSubscriber;
            string body = Global.Master_MailBodyNewSubscriber + Environment.NewLine + email;

            // on its own thread: the visitor does not wait for the mail server
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    Mailer.Send(Mailer.FromAddress, subject, body, false);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex, "new subscriber notification");
                }
            });
        }

        // ---- unsubscribing ----

        // removes the address from the list (every row of it, confirmed or not); true when there was one
        public static bool Unsubscribe(string email)
        {
            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("DELETE FROM SUBSCRIBE WHERE EMAIL = @Email", cn))
            {
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 254).Value = (email ?? string.Empty).Trim().ToLowerInvariant();
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // The page of the form: whoever types an address gets, if it is on the list, an e-mail with the link that unsubscribes it. The
        // answer is the same whether the address is on the list or not (the form cannot be used to find out who is subscribed).
        public static void RequestUnsubscribeLink(string email, string ip)
        {
            email = (email ?? string.Empty).Trim().ToLowerInvariant();

            if (!Allow("unsub-ip:" + ip, MaxLinkRequestsPerHourPerIp, TimeSpan.FromHours(1))
                || !Allow("unsub-mail:" + email, MaxLinkRequestsPerDayPerAddress, TimeSpan.FromDays(1)))
            {
                return;
            }

            bool onTheList;

            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SUBSCRIBE WHERE EMAIL = @Email", cn))
            {
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 254).Value = email;
                cn.Open();
                onTheList = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }

            if (onTheList)
            {
                SendUnsubscribeLink(email);
            }
        }

        // a counter per key that forgets itself after the window (the first hit starts it)
        private static bool Allow(string key, int max, TimeSpan window)
        {
            var cache = HttpRuntime.Cache;
            string cacheKey = "nc_limit_" + key;
            int[] counter = cache[cacheKey] as int[];

            if (counter == null)
            {
                counter = new[] { 0 };
                cache.Insert(cacheKey, counter, null, DateTime.UtcNow.Add(window), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            return System.Threading.Interlocked.Increment(ref counter[0]) <= max;
        }

        // ---- the e-mails ----

        private static string Button(string url, string text)
        {
            return "<p style=\"margin:22px 0\"><a href=\"" + HttpUtility.HtmlAttributeEncode(url)
                + "\" style=\"background:#4473c5;color:#ffffff;text-decoration:none;padding:12px 22px;border-radius:6px;font-weight:bold\">"
                + HttpUtility.HtmlEncode(text) + "</a></p>";
        }

        private static string Wrap(string intro, string url, string buttonText, string ignore)
        {
            return "<div style=\"font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:1.5;color:#222222;max-width:520px\">"
                + "<p>" + HttpUtility.HtmlEncode(intro) + "</p>" + Button(url, buttonText)
                + "<p style=\"color:#666666;font-size:13px\">" + HttpUtility.HtmlEncode(ignore) + "</p></div>";
        }

        private static void SendConfirmation(string email, string token)
        {
            string url = Mailer.SiteBase + "/confirm.aspx?t=" + token + "&lang=" + LanguageCode();

            try
            {
                Mailer.Send(email, Global.Text("Sub_Mail_Subject"),
                    Wrap(Global.Text("Sub_Mail_Intro"), url, Global.Text("Sub_Mail_Button"), Global.Text("Sub_Mail_Ignore")), true);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "subscription confirmation e-mail");
            }
        }

        private static void SendUnsubscribeLink(string email)
        {
            try
            {
                Mailer.Send(email, Global.Text("Unsub_Mail_Subject"),
                    Wrap(Global.Text("Unsub_Mail_Intro"), UnsubscribeLink.Url(email), Global.Text("Unsub_Mail_Button"), Global.Text("Unsub_Mail_Ignore")), true);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "unsubscribe link e-mail");
            }
        }

        private static string LanguageCode()
        {
            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr" ? "fr" : "en";
        }

        // 128 random bits as 32 hexadecimal characters
        private static string NewToken()
        {
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var sb = new StringBuilder(32);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }
    }
}
