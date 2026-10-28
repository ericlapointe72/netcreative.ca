using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // Runs any SQL typed by the administrator: the site has no other way into its database, so a small fix (deleting a test message,
    // looking at a table, adding a column) is done here. Everything runs in ONE transaction: either all of it is kept, or (simulation,
    // the default) all of it is rolled back after showing what the query returned. A run that really changes data asks for the password
    // again, and every run is written to the audit log (App_Data\logs).
    public partial class Sql : System.Web.UI.Page
    {
        private const int MaxRows = 500;
        private const int MaxCellChars = 300;
        private const int MaxLoggedChars = 1200;

        protected sealed class Template
        {
            public string Label;
            public string Sql;
        }

        // the ready-made queries of the buttons above the box (texts in the resources, in the language of the page)
        protected static List<Template> Templates
        {
            get
            {
                var list = new List<Template>();

                for (int i = 1; i <= 10; i++)
                {
                    list.Add(new Template { Label = Global.Text("Sql_T" + i + "_L"), Sql = Global.Text("Sql_T" + i + "_Q") });
                }

                return list;
            }
        }

        // the SQL as the value of an attribute: encoded, and its line breaks kept
        protected static string AttributeText(string sql)
        {
            return HttpUtility.HtmlEncode(sql ?? string.Empty).Replace("\r\n", "&#10;").Replace("\n", "&#10;");
        }

        protected string ConfirmTextJson
        {
            get { return new JavaScriptSerializer().Serialize(Global.Text("Sql_ConfirmReal")); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ButtonRun.Text = Global.Text("Sql_Run");

            if (!IsPostBack)
            {
                CheckSimulation.Checked = true;
            }
        }

        protected void ButtonRun_Click(object sender, EventArgs e)
        {
            string query = (TextBoxSql.Text ?? string.Empty).Trim();

            if (query.Length == 0)
            {
                Show(Global.Text("Sql_Empty"), false);
                return;
            }

            // "GO" is a command of the SQL Server tools, not of the database
            if (Regex.IsMatch(query, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
            {
                Show(Global.Text("Sql_NoGo"), false);
                return;
            }

            bool simulate = CheckSimulation.Checked;

            // The whole run is ONE transaction that this page ends (rolled back in simulation, kept otherwise). A query that ends it
            // itself (COMMIT, ROLLBACK, a new BEGIN TRAN) would keep what it changed even in simulation: those words are refused.
            if (EndsTransaction(query))
            {
                Audit(query, simulate, "REFUSED: transaction control");
                Show(Global.Text("Sql_NoTransaction"), false);
                return;
            }

            // changing real data asks for the password again: a stolen session alone is not enough
            if (!simulate && !PasswordConfirmed())
            {
                Audit(query, simulate, "REFUSED: password not confirmed");
                Show(Global.Text("Sql_BadPassword"), false);
                return;
            }

            var timer = Stopwatch.StartNew();

            try
            {
                string tables;
                int sets;
                int affected;
                bool endedByQuery;

                Run(query, simulate, out tables, out sets, out affected, out endedByQuery);
                timer.Stop();

                // the query found a way to end the transaction before the page did (dynamic SQL, for example): what it changed may be kept
                if (endedByQuery)
                {
                    LiteralResult.Text = tables;
                    Show(Global.Text("Sql_EndedByQuery"), false);
                    Audit(query, simulate, "WARNING the query ended the transaction itself");
                    return;
                }

                var summary = new StringBuilder();

                if (affected >= 0)
                {
                    summary.Append(Global.Format(Global.Text("Sql_SummaryRows"), affected));
                }

                if (sets > 0)
                {
                    summary.Append(Global.Format(Global.Text("Sql_SummarySets"), sets));
                }

                // a command that changes the structure (ALTER, CREATE, DROP...) counts no rows and shows nothing: say that it ran
                if (affected < 0 && sets == 0)
                {
                    summary.Append(Global.Text("Sql_SummaryDone"));
                }

                summary.Append(Global.Format(Global.Text("Sql_SummaryTime"), timer.ElapsedMilliseconds));
                summary.Append(Global.Text(simulate ? "Sql_SimNote" : "Sql_RealNote"));

                LiteralResult.Text = tables;
                Show(summary.ToString(), true);
                Audit(query, simulate, "OK " + summary);
            }
            catch (Exception ex)
            {
                timer.Stop();
                LiteralResult.Text = string.Empty;

                // an administrator who runs a query needs the database's own message (a column that does not exist, a syntax error...)
                Show(Global.Format(Global.Text("Sql_Error"), ex.Message), false);
                Audit(query, simulate, "ERROR " + ex.Message);
            }
        }

        // is the password typed here the one of the signed-in user? Wrong ones count in the same limits as the sign-in page.
        private bool PasswordConfirmed()
        {
            string typed = TextBoxPassword.Text ?? string.Empty;
            TextBoxPassword.Text = string.Empty;

            string user = Convert.ToString(Session["user"]);
            string ip = AdminLogin.ClientIp(Context);

            if (typed.Length == 0 || AdminLogin.Blocked(ip, user))
            {
                return false;
            }

            string hash = null;

            try
            {
                using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                using (var cmd = new SqlCommand("SELECT TOP 1 PASSWORD FROM USERS WHERE USER_NAME = @u;", cn))
                {
                    cmd.Parameters.Add("@u", SqlDbType.VarChar, 20).Value = AdminLogin.Clip(user, 20);
                    cn.Open();
                    hash = cmd.ExecuteScalar() as string;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "sql page password");
                return false;
            }

            bool ok = !string.IsNullOrEmpty(hash) ? PasswordHasher.Verify(typed, hash) : false;

            if (!ok)
            {
                AdminLogin.RecordFailure(ip, user);
                AdminLogin.LogEvent(user, "FAIL");
            }

            return ok;
        }

        // the words of the query that would end the transaction (the comments and the texts between quotes are not looked at)
        private static bool EndsTransaction(string query)
        {
            string code = Regex.Replace(query, @"/\*.*?\*/", " ", RegexOptions.Singleline);
            code = Regex.Replace(code, @"--[^\r\n]*", " ");
            code = Regex.Replace(code, @"'(?:[^']|'')*'", "''");

            return Regex.IsMatch(code, @"\b(COMMIT|ROLLBACK|BEGIN\s+TRAN(SACTION)?|SAVE\s+TRAN(SACTION)?|SET\s+IMPLICIT_TRANSACTIONS)\b", RegexOptions.IgnoreCase);
        }

        private static void Run(string query, bool simulate, out string tables, out int sets, out int affected, out bool endedByQuery)
        {
            var html = new StringBuilder();
            sets = 0;
            affected = -1;
            endedByQuery = false;

            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            {
                cn.Open();

                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(query, cn, tx))
                        {
                            cmd.CommandTimeout = 60;

                            SqlDataReader reader = cmd.ExecuteReader();

                            try
                            {
                                do
                                {
                                    if (reader.FieldCount > 0)
                                    {
                                        sets++;
                                        html.Append(RenderSet(reader, sets));
                                    }
                                }
                                while (reader.NextResult());
                            }
                            finally
                            {
                                reader.Close();
                            }

                            // only known once the reader is closed
                            affected = reader.RecordsAffected;
                        }

                        // "this transaction has already ended": the query ended it itself
                        try
                        {
                            if (simulate)
                            {
                                tx.Rollback();
                            }
                            else
                            {
                                tx.Commit();
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            endedByQuery = true;
                        }
                    }
                    catch
                    {
                        try
                        {
                            tx.Rollback();
                        }
                        catch (InvalidOperationException)
                        {
                            // the server already ended the transaction
                        }

                        throw;
                    }
                }
            }

            tables = html.ToString();
        }

        private static string RenderSet(SqlDataReader reader, int number)
        {
            var html = new StringBuilder();
            html.Append("<div class=\"sql-set\"><div class=\"sql-note\">").Append(HttpUtility.HtmlEncode(Global.Format(Global.Text("Sql_ResultN"), number))).Append("</div>");
            html.Append("<div class=\"sql-scroll\"><table class=\"grid\"><thead><tr>");

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                html.Append("<th>").Append(HttpUtility.HtmlEncode(string.IsNullOrEmpty(name) ? Global.Text("Sql_NoName") : name)).Append("</th>");
            }

            html.Append("</tr></thead><tbody>");

            int rows = 0;
            bool more = false;

            while (reader.Read())
            {
                if (rows >= MaxRows)
                {
                    more = true;
                    break;
                }

                rows++;
                html.Append("<tr>");

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    html.Append("<td>").Append(Cell(reader.GetValue(i))).Append("</td>");
                }

                html.Append("</tr>");
            }

            html.Append("</tbody></table></div>");
            html.Append("<div class=\"sql-note\">").Append(HttpUtility.HtmlEncode(Global.Format(Global.Text("Sql_RowsCount"), rows)));

            if (more)
            {
                html.Append(HttpUtility.HtmlEncode(Global.Text("Sql_RowsMore")));
            }

            return html.Append("</div></div>").ToString();
        }

        private static string Cell(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "<em class=\"sql-null\">NULL</em>";
            }

            string text;

            if (value is DateTime)
            {
                text = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else if (value is byte[])
            {
                var bytes = (byte[])value;
                text = "0x" + BitConverter.ToString(bytes, 0, Math.Min(bytes.Length, 16)).Replace("-", "") + (bytes.Length > 16 ? "..." : string.Empty);
            }
            else
            {
                text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
            }

            if (text.Length > MaxCellChars)
            {
                text = text.Substring(0, MaxCellChars) + "...";
            }

            return HttpUtility.HtmlEncode(text);
        }

        private void Show(string message, bool ok)
        {
            LabelStatus.Text = HttpUtility.HtmlEncode(message);
            LabelStatus.CssClass = ok ? "sql-status sql-ok" : "sql-status sql-error";
        }

        // one line of the audit log per run: simulation or real, what happened, and the query itself (on one line)
        private static void Audit(string query, bool simulate, string outcome)
        {
            string oneLine = Regex.Replace(query, @"\s+", " ");
            if (oneLine.Length > MaxLoggedChars)
            {
                oneLine = oneLine.Substring(0, MaxLoggedChars) + " [...]";
            }

            AdminAudit.Write(simulate ? "SQL simulation" : "SQL REAL", outcome + " | " + oneLine);
        }
    }
}
