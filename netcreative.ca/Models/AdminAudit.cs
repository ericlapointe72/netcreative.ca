using System;
using System.IO;
using System.Text;
using System.Web;

namespace netcreative.ca
{
    // A trace of what an administrator did that matters for security (a user created or deleted, a password changed, a two-factor reset,
    // an SQL command, a newsletter sent, a log purged): who, when, from which address, what. One text file per month in App_Data\logs
    // (a folder the web server never serves), kept 24 months. It is for the owner, who can download it like the error logs.
    public static class AdminAudit
    {
        private const long MaxFileBytes = 5 * 1024 * 1024;
        private const int KeepMonths = 24;

        private static readonly object Sync = new object();
        private static DateTime lastCleanup = DateTime.MinValue;

        // 'detail' is cut and stripped of control characters: it can hold what an administrator typed (an SQL command)
        public static void Write(string action, string detail)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                string user = context != null && context.Session != null ? Convert.ToString(context.Session["user"]) : string.Empty;
                string ip = context != null ? AdminLogin.ClientIp(context) : string.Empty;

                string folder = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/logs");
                if (folder == null)
                {
                    return;
                }

                Directory.CreateDirectory(folder);

                string line = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "Z | " + AdminLogin.Clip(user, 20) + " | " + ip + " | " + AdminLogin.Clip(action, 60)
                    + " | " + AdminLogin.Clip(detail, 1500) + Environment.NewLine;

                string path = Path.Combine(folder, "audit-" + DateTime.UtcNow.ToString("yyyyMM") + ".log");

                lock (Sync)
                {
                    if (File.Exists(path) && new FileInfo(path).Length > MaxFileBytes)
                    {
                        return;
                    }

                    File.AppendAllText(path, line, Encoding.UTF8);
                    Cleanup(folder);
                }
            }
            catch (Exception)
            {
                // the trace of an action must never stop the action
            }
        }

        // old months go away (once an hour at most)
        private static void Cleanup(string folder)
        {
            if (DateTime.UtcNow - lastCleanup < TimeSpan.FromHours(1))
            {
                return;
            }

            lastCleanup = DateTime.UtcNow;

            foreach (string file in Directory.GetFiles(folder, "audit-*.log"))
            {
                if (File.GetLastWriteTimeUtc(file) < DateTime.UtcNow.AddMonths(-KeepMonths))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
