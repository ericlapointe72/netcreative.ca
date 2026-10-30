using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace netcreative.ca
{
    // The figures of the admin home page. All of them are read from what the site already keeps (the SUBSCRIBE, CONTACT and LOGIN tables and
    // the log files of App_Data\logs): nothing is written, and a figure that cannot be read leaves its card empty instead of stopping the page.
    public class DashboardStats
    {
        public class RecentMessage
        {
            public DateTime Date;
            public string Name = string.Empty;
            public string ProjectType = string.Empty;
        }

        public int SubscribersConfirmed;
        public int SubscribersPending;
        public int SubscribersNew30;

        public int ContactTotal;
        public int ContactMonth;
        public DateTime? ContactLast;
        public List<RecentMessage> Recent = new List<RecentMessage>();

        public int FailedSignIns24h;
        public int FailedSignIns7d;
        public DateTime? PreviousSignIn;
        public string PreviousSignInIp = string.Empty;

        public int ErrorsToday;
        public int Errors7d;

        public DateTime? NewsletterDate;
        public string NewsletterSubject = string.Empty;
        public int NewsletterSent;
        public int NewsletterFailed;

        // one of the figures could not be read (the detail is in the error log)
        public bool Incomplete;

        // an error entry starts with its time in UTC, the lines of its stack trace do not
        private static readonly Regex EntryStart = new Regex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}Z \| ", RegexOptions.Compiled);
        private static readonly Regex ErrorFile = new Regex(@"^errors-(\d{8})\.log$", RegexOptions.Compiled);
        private static readonly Regex AuditFile = new Regex(@"^audit-\d{6}\.log$", RegexOptions.Compiled);
        private static readonly Regex NewsletterLine = new Regex(
            @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})Z \|[^|]*\|[^|]*\| Newsletter sent \| ""(.*)"" sent=(\d+) failed=(\d+)\s*$", RegexOptions.Compiled);

        public static DashboardStats Load(string user)
        {
            var stats = new DashboardStats();
            DateTime now = DateTime.Now;

            stats.Guard(() => stats.LoadDatabase(user, now));
            stats.Guard(() => stats.LoadErrors());
            stats.Guard(() => stats.LoadNewsletter());

            return stats;
        }

        private void Guard(Action read)
        {
            try
            {
                read();
            }
            catch (Exception ex)
            {
                Incomplete = true;
                ErrorLogger.Log(ex, "dashboard");
            }
        }

        private void LoadDatabase(string user, DateTime now)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            {
                connection.Open();

                // subscribers: CONFIRMED is the UTC time at which the owner of the address clicked the link of the e-mail
                using (var cmd = new SqlCommand("SELECT COUNT(*), COUNT(CONFIRMED), COUNT(CASE WHEN CONFIRMED >= @Since THEN 1 END) FROM SUBSCRIBE", connection))
                {
                    cmd.Parameters.Add("@Since", SqlDbType.DateTime2).Value = DateTime.UtcNow.AddDays(-30);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();
                        SubscribersConfirmed = dr.GetInt32(1);
                        SubscribersPending = dr.GetInt32(0) - SubscribersConfirmed;
                        SubscribersNew30 = dr.GetInt32(2);
                    }
                }

                // contact messages
                using (var cmd = new SqlCommand("SELECT COUNT(*), COUNT(CASE WHEN [DATE] >= @MonthStart THEN 1 END), MAX([DATE]) FROM CONTACT", connection))
                {
                    cmd.Parameters.Add("@MonthStart", SqlDbType.Date).Value = new DateTime(now.Year, now.Month, 1);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();
                        ContactTotal = dr.GetInt32(0);
                        ContactMonth = dr.GetInt32(1);
                        ContactLast = dr.IsDBNull(2) ? (DateTime?)null : Convert.ToDateTime(dr[2]);
                    }
                }

                using (var cmd = new SqlCommand("SELECT TOP 5 [DATE], FIRST_NAME, LAST_NAME, PROJECT_TYPE FROM CONTACT ORDER BY AUTO_NUMBER DESC", connection))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Recent.Add(new RecentMessage
                        {
                            Date = Convert.ToDateTime(dr[0]),
                            Name = (Convert.ToString(dr[1]) + " " + Convert.ToString(dr[2])).Trim(),
                            ProjectType = Convert.ToString(dr[3])
                        });
                    }
                }

                // failed sign-ins: the table holds the server's local date and time (what AdminLogin.LogEvent writes)
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(CASE WHEN CAST([DATE] AS datetime) + CAST([TIME] AS datetime) >= @Since24 THEN 1 END), COUNT(*) FROM LOGIN WHERE ACTION = 'FAIL' AND [DATE] >= @Since7", connection))
                {
                    cmd.Parameters.Add("@Since24", SqlDbType.DateTime).Value = now.AddHours(-24);
                    cmd.Parameters.Add("@Since7", SqlDbType.Date).Value = now.AddDays(-7).Date;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();
                        FailedSignIns24h = dr.GetInt32(0);
                        FailedSignIns7d = dr.GetInt32(1);
                    }
                }

                // the sign-in before the one of this session: somebody else using the account would show here
                if (!string.IsNullOrEmpty(user))
                {
                    using (var cmd = new SqlCommand(
                        "SELECT [DATE], [TIME], IP_ADDRESS FROM LOGIN WHERE ACTION = 'LOGIN' AND USER_NAME = @User ORDER BY AUTO_NUMBER DESC OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY", connection))
                    {
                        cmd.Parameters.Add("@User", SqlDbType.VarChar, 20).Value = user.Length > 20 ? user.Substring(0, 20) : user;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                PreviousSignIn = Convert.ToDateTime(dr[0]).Date + (TimeSpan)dr[1];
                                PreviousSignInIp = Convert.ToString(dr[2]);
                            }
                        }
                    }
                }
            }
        }

        private static string LogFolder()
        {
            return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/logs");
        }

        // entries of the error files of the last seven days (the files are named after the UTC day); the logger may be writing: shared reading
        private void LoadErrors()
        {
            string folder = LogFolder();

            if (folder == null || !Directory.Exists(folder))
            {
                return;
            }

            string today = DateTime.UtcNow.ToString("yyyyMMdd");
            string oldest = DateTime.UtcNow.AddDays(-6).ToString("yyyyMMdd");

            foreach (string path in Directory.GetFiles(folder, "errors-*.log"))
            {
                Match name = ErrorFile.Match(Path.GetFileName(path));

                if (!name.Success || string.CompareOrdinal(name.Groups[1].Value, oldest) < 0)
                {
                    continue;
                }

                int entries = CountEntries(path);
                Errors7d += entries;

                if (name.Groups[1].Value == today)
                {
                    ErrorsToday += entries;
                }
            }
        }

        private static int CountEntries(string path)
        {
            int count = 0;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (EntryStart.IsMatch(line))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        // the last mailing to the subscribers in the audit files (a single test send is not audited; a subject that starts with "[Test] " is
        // skipped anyway); only the two newest months are looked at
        private void LoadNewsletter()
        {
            string folder = LogFolder();

            if (folder == null || !Directory.Exists(folder))
            {
                return;
            }

            var files = new List<string>();

            foreach (string path in Directory.GetFiles(folder, "audit-*.log"))
            {
                if (AuditFile.IsMatch(Path.GetFileName(path)))
                {
                    files.Add(path);
                }
            }

            files.Sort(StringComparer.OrdinalIgnoreCase);
            files.Reverse();

            for (int i = 0; i < files.Count && i < 2; i++)
            {
                string[] lines;

                using (var stream = new FileStream(files[i], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    lines = reader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }

                for (int n = lines.Length - 1; n >= 0; n--)
                {
                    Match m = NewsletterLine.Match(lines[n]);

                    if (!m.Success || m.Groups[2].Value.StartsWith("[Test] ", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    DateTime utc = DateTime.SpecifyKind(DateTime.ParseExact(m.Groups[1].Value, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture), DateTimeKind.Utc);

                    NewsletterDate = utc.ToLocalTime();
                    NewsletterSubject = m.Groups[2].Value;
                    NewsletterSent = int.Parse(m.Groups[3].Value);
                    NewsletterFailed = int.Parse(m.Groups[4].Value);
                    return;
                }
            }
        }
    }
}
