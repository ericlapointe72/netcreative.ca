using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

namespace netcreative.ca
{
    // What the owner can take out of the site: the subscribers and the contact messages as a CSV file (opens in Excel), and the
    // log files of App_Data\logs as they are. Under /Admin/, so Global.asax lets in a signed-in administrator only; every download is
    // written to the audit log.
    public class Export : IHttpHandler, IReadOnlySessionState
    {
        // "errors-20261026.log", "audit-202610.log": the only names that can be asked for (nothing else of the folder, no path)
        private static readonly Regex LogName = new Regex(@"^(errors-\d{8}|audit-\d{6})\.log$", RegexOptions.Compiled);

        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            try
            {
                switch (context.Request.QueryString["what"])
                {
                    case "subscribers":
                        Subscribers(context);
                        break;
                    case "contact":
                        Contacts(context);
                        break;
                    case "log":
                        Log(context);
                        break;
                    default:
                        context.Response.StatusCode = 404;
                        break;
                }
            }
            catch (Exception ex)
            {
                // a file that stopped half way is not worth keeping: the visitor of the page gets a plain error
                ErrorLogger.Log(ex, "export");
                context.Response.Clear();
                context.Response.StatusCode = 500;
            }
        }

        // ---- subscribers ---------------------------------------------------------------------------------------

        private static void Subscribers(HttpContext context)
        {
            bool english = IsEnglish(context);
            string[] header = english
                ? new[] { "Number", "Date", "Time", "IP address", "E-mail", "Confirmed (UTC)" }
                : new[] { "Numéro", "Date", "Heure", "Adresse IP", "Courriel", "Confirmé (UTC)" };

            StartCsv(context, "subscribers", header);

            int rows = 0;

            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            using (var cmd = new SqlCommand("SELECT AUTO_NUMBER, DATE, TIME, IP_ADDRESS, EMAIL, CONFIRMED FROM SUBSCRIBE ORDER BY AUTO_NUMBER", cn))
            {
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        WriteRow(context,
                            Convert.ToString(dr["AUTO_NUMBER"]),
                            ((DateTime)dr["DATE"]).ToString("yyyy-MM-dd"),
                            ((TimeSpan)dr["TIME"]).ToString(@"hh\:mm\:ss"),
                            Convert.ToString(dr["IP_ADDRESS"]),
                            Convert.ToString(dr["EMAIL"]),
                            dr["CONFIRMED"] is DateTime ? ((DateTime)dr["CONFIRMED"]).ToString("yyyy-MM-dd HH:mm") : string.Empty);
                        rows++;
                    }
                }
            }

            AdminAudit.Write("Export", "subscribers, " + rows + " row(s)");
        }

        // ---- contact messages ----------------------------------------------------------------------------------

        private static void Contacts(HttpContext context)
        {
            bool english = IsEnglish(context);
            string[] header = english
                ? new[] { "Number", "Date", "Time", "First name", "Last name", "Company", "E-mail", "Phone", "Project", "Budget", "Deadline", "Message" }
                : new[] { "Numéro", "Date", "Heure", "Prénom", "Nom", "Entreprise", "Courriel", "Téléphone", "Projet", "Budget", "Échéancier", "Message" };

            StartCsv(context, "contact", header);

            int rows = 0;

            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            using (var cmd = new SqlCommand("SELECT * FROM CONTACT ORDER BY AUTO_NUMBER", cn))
            {
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        WriteRow(context,
                            Convert.ToString(dr["AUTO_NUMBER"]),
                            ((DateTime)dr["DATE"]).ToString("yyyy-MM-dd"),
                            ((TimeSpan)dr["TIME"]).ToString(@"hh\:mm"),
                            Convert.ToString(dr["FIRST_NAME"]),
                            Convert.ToString(dr["LAST_NAME"]),
                            Convert.ToString(dr["COMPANY"]),
                            Convert.ToString(dr["EMAIL"]),
                            Convert.ToString(dr["PHONE"]),
                            Convert.ToString(dr["PROJECT_TYPE"]),
                            Convert.ToString(dr["PROJECT_BUDGET"]),
                            Convert.ToString(dr["PROJECT_DEADLINE"]),
                            Convert.ToString(dr["DESCRIPTION"]));
                        rows++;
                    }
                }
            }

            AdminAudit.Write("Export", "contact messages, " + rows + " row(s)");
        }

        // ---- a log file ----------------------------------------------------------------------------------------

        private static void Log(HttpContext context)
        {
            string name = context.Request.QueryString["file"] ?? string.Empty;

            if (!LogName.IsMatch(name))
            {
                context.Response.StatusCode = 404;
                return;
            }

            string folder = context.Server.MapPath("~/App_Data/logs");
            string path = Path.Combine(folder, name);

            if (!File.Exists(path))
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + name + "\"");

            // the logger may be writing to the file at this very moment
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                stream.CopyTo(context.Response.OutputStream);
            }

            AdminAudit.Write("Export", "log file " + name);
        }

        // ---- CSV -----------------------------------------------------------------------------------------------

        private static bool IsEnglish(HttpContext context)
        {
            return string.Equals(context.Session["language"] as string, "en", StringComparison.Ordinal);
        }

        // UTF-8 with a signature (Excel then reads the accents), fields between quotes, ";" between them (the separator of Excel in French)
        private static void StartCsv(HttpContext context, string baseName, string[] header)
        {
            context.Response.ContentType = "text/csv; charset=utf-8";
            context.Response.ContentEncoding = new UTF8Encoding(false);
            context.Response.AppendHeader("Content-Disposition", "attachment; filename=\"netcreative-" + baseName + "-" + DateTime.Now.ToString("yyyyMMdd") + ".csv\"");
            context.Response.BufferOutput = true;
            context.Response.Write("﻿");

            WriteRow(context, header);
        }

        private static void WriteRow(HttpContext context, params string[] cells)
        {
            var line = new StringBuilder();

            for (int i = 0; i < cells.Length; i++)
            {
                if (i > 0)
                {
                    line.Append(';');
                }

                line.Append('"').Append(Cell(cells[i]).Replace("\"", "\"\"")).Append('"');
            }

            line.Append("\r\n");
            context.Response.Write(line.ToString());
        }

        // What a visitor typed ends up in a spreadsheet: a text that starts with = + - @ (or a tab, a return) would be run as a formula
        // when the file is opened. It gets a quote in front, so it stays text. Phone numbers (+1 418 ...) are left as they are.
        private static string Cell(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            char first = text[0];

            if ((first == '=' || first == '@' || first == '\t' || first == '\r')
                || ((first == '+' || first == '-') && !Regex.IsMatch(text, @"^[+-]?[\d\s().-]+$")))
            {
                return "'" + text;
            }

            return text;
        }
    }
}
