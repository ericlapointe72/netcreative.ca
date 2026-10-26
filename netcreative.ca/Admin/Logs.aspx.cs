using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // The log files the site writes to App_Data\logs (errors: one file per day, kept 30 days; audit: one file per month, kept 24 months),
    // read here instead of with an FTP client. Only files with those two names are listed, and what they hold is shown as text.
    public partial class Logs : System.Web.UI.Page
    {
        private static readonly Regex LogName = new Regex(@"^(errors-\d{8}|audit-\d{6})\.log$", RegexOptions.Compiled);

        // how much of the end of a file is read, and how many entries are shown (the newest first)
        private const int MaxBytes = 256 * 1024;
        private const int MaxEntries = 200;

        protected void Page_Load(object sender, EventArgs e)
        {
            Label_Logs_File.Text = Global.Text("Logs_File");
            HyperLink_Logs_Download.Text = Global.Text("Logs_Download");

            if (!IsPostBack)
            {
                FillList();
                Show();
            }
        }

        protected void DropDownList_Logs_File_SelectedIndexChanged(object sender, EventArgs e)
        {
            Show();
        }

        private static string Folder()
        {
            return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/logs");
        }

        // the newest file first
        private void FillList()
        {
            string folder = Folder();

            var files = Directory.Exists(folder)
                ? new DirectoryInfo(folder).GetFiles("*.log").Where(f => LogName.IsMatch(f.Name)).OrderByDescending(f => f.LastWriteTimeUtc).ToList()
                : new System.Collections.Generic.List<FileInfo>();

            foreach (FileInfo file in files)
            {
                DropDownList_Logs_File.Items.Add(new ListItem(file.Name + " (" + Math.Max(1, file.Length / 1024) + " KB)", file.Name));
            }

            bool any = files.Count > 0;
            PlaceHolder_Picker.Visible = any;
            Label_Logs_Empty.Visible = !any;
            Label_Logs_Empty.Text = Global.Text("Logs_Empty");
        }

        private void Show()
        {
            string name = DropDownList_Logs_File.SelectedValue;

            // what comes back from the browser is checked again: only the names of the two kinds of log are accepted
            if (!LogName.IsMatch(name ?? string.Empty))
            {
                return;
            }

            string path = Path.Combine(Folder(), name);

            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var info = new FileInfo(path);
                bool cut;
                string text = ReadEnd(path, out cut);

                // an error entry spans several lines and ends with an empty line; an audit entry is one line
                bool errors = name.StartsWith("errors-", StringComparison.Ordinal);
                string[] entries = errors
                    ? Regex.Split(text.Trim(), @"\r?\n[ \t]*\r?\n").Where(s => s.Trim().Length > 0).ToArray()
                    : text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                var newest = entries.Reverse().Take(MaxEntries).ToArray();

                Literal_Logs_Text.Text = string.Join(errors ? "\r\n\r\n" : "\r\n", newest);
                Label_Logs_Info.Text = Global.Format(Global.Text("Logs_Info"), newest.Length, entries.Length + (cut ? "+" : string.Empty),
                    info.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                HyperLink_Logs_Download.NavigateUrl = "~/Admin/Export.ashx?what=log&file=" + Uri.EscapeDataString(name);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "logs page");
                AdminUi.Toast(this, Global.Text("Admin_Error"), false);
            }
        }

        // the end of the file (the logger may be writing to it: it is opened for sharing). When the file is longer than what is read,
        // the first line is dropped, because it is cut.
        private static string ReadEnd(string path, out bool cut)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                cut = stream.Length > MaxBytes;
                stream.Seek(cut ? stream.Length - MaxBytes : 0, SeekOrigin.Begin);

                var buffer = new byte[Math.Min(stream.Length, MaxBytes)];
                int read = 0;

                while (read < buffer.Length)
                {
                    int n = stream.Read(buffer, read, buffer.Length - read);
                    if (n <= 0)
                    {
                        break;
                    }

                    read += n;
                }

                string text = Encoding.UTF8.GetString(buffer, 0, read);

                if (cut)
                {
                    int newline = text.IndexOf('\n');
                    text = newline >= 0 ? text.Substring(newline + 1) : string.Empty;
                }

                return text;
            }
        }
    }
}
