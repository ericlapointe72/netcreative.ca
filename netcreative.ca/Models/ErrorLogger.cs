using System;
using System.IO;
using System.Text;
using System.Web;

namespace netcreative.ca
{
    // A trace of the errors that end as a "500", kept in App_Data\logs (a folder the web server never serves), one file per day.
    // What the visitor sees is a plain page without technical details; the details are here, for the owner.
    public static class ErrorLogger
    {
        private const long MaxFileBytes = 2 * 1024 * 1024;
        private const int KeepDays = 30;

        private static readonly object Sync = new object();
        private static DateTime lastCleanup = DateTime.MinValue;

        public static void Log(Exception error, string where)
        {
            try
            {
                if (error == null)
                {
                    return;
                }

                string folder = Folder();
                if (folder == null)
                {
                    return;
                }

                // an error wrapped by ASP.NET (HttpUnhandledException): the useful one is inside
                Exception inner = error;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }

                var sb = new StringBuilder();
                sb.Append(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")).Append("Z | ").Append(where ?? string.Empty);

                HttpContext context = HttpContext.Current;
                if (context != null && context.Request != null)
                {
                    // the path only, never the query string or the form: they can hold what a visitor typed
                    sb.Append(" | ").Append(context.Request.Path).Append(" | ").Append(AdminLogin.ClientIp(context));
                }

                sb.AppendLine();
                sb.Append("  ").Append(inner.GetType().FullName).Append(": ").AppendLine(AdminLogin.Clip(inner.Message, 500));

                string stack = inner.StackTrace ?? string.Empty;
                if (stack.Length > 1500)
                {
                    stack = stack.Substring(0, 1500);
                }

                sb.AppendLine(stack);
                sb.AppendLine();

                string path = Path.Combine(folder, "errors-" + DateTime.UtcNow.ToString("yyyyMMdd") + ".log");

                lock (Sync)
                {
                    if (File.Exists(path) && new FileInfo(path).Length > MaxFileBytes)
                    {
                        return;
                    }

                    File.AppendAllText(path, sb.ToString(), Encoding.UTF8);
                    Cleanup(folder);
                }
            }
            catch (Exception)
            {
                // logging an error must never cause another one
            }
        }

        private static string Folder()
        {
            string root = MapAppData();
            if (root == null)
            {
                return null;
            }

            string folder = Path.Combine(root, "logs");
            Directory.CreateDirectory(folder);
            return folder;
        }

        private static string MapAppData()
        {
            try
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
            }
            catch (Exception)
            {
                return null;
            }
        }

        // old files go away (once an hour at most)
        private static void Cleanup(string folder)
        {
            if (DateTime.UtcNow - lastCleanup < TimeSpan.FromHours(1))
            {
                return;
            }

            lastCleanup = DateTime.UtcNow;

            foreach (string file in Directory.GetFiles(folder, "errors-*.log"))
            {
                if (File.GetLastWriteTimeUtc(file) < DateTime.UtcNow.AddDays(-KeepDays))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
