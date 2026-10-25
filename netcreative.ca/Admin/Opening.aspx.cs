using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Opening : System.Web.UI.Page
    {
        // what the page gives the Repeater for one day (public properties, so that Eval can read them)
        private class DayRow
        {
            public string Name { get; set; }
            public bool Open { get; set; }
            public string From { get; set; }
            public string To { get; set; }
        }

        // 8, 8:30, 08:30, 8h30, 8 h 30, 08:30:00 (what the time box of a browser sends, or what is typed in one that has no time picker)
        private static readonly Regex TimePattern = new Regex(@"^(\d{1,2})\s*(?:[:hH]\s*(\d{1,2})?)?(?::\d{1,2})?$", RegexOptions.Compiled);

        private string connectionString = string.Empty;

        // the preview follows the language of the admin, like the footer of the site follows the visitor's
        protected bool PreviewEnglish
        {
            get { return (Session["language"] as string) == "en"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Button_Opening_Save.Text = Global.Admin_SaveButton;

            if (!IsPostBack)
            {
                Opening_Load();
            }
        }

        private void Opening_Load()
        {
            try
            {
                OpeningDay[] days = OpeningHours.Load(connectionString);

                // a closed day suggests the hours of the first open day (9:00 to 17:00 when none is open), so that ticking "Open"
                // does not start with empty boxes
                int suggestedFrom = 9 * 60;
                int suggestedTo = 17 * 60;

                foreach (OpeningDay day in days)
                {
                    if (day.Open)
                    {
                        suggestedFrom = day.OpensAt;
                        suggestedTo = day.ClosesAt;
                        break;
                    }
                }

                var rows = new List<DayRow>();

                for (int d = 0; d < days.Length; d++)
                {
                    rows.Add(new DayRow
                    {
                        Name = OpeningHours.DayName(d),
                        Open = days[d].Open,
                        From = TimeBox(days[d].Open ? days[d].OpensAt : suggestedFrom),
                        To = TimeBox(days[d].Open ? days[d].ClosesAt : suggestedTo)
                    });
                }

                Repeater_Days.DataSource = rows;
                Repeater_Days.DataBind();

                ShowPreview(days);
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        // the lines of the footer as the visitors read them now (the page's script keeps them up to date while the boxes change)
        private void ShowPreview(OpeningDay[] days)
        {
            var html = new StringBuilder();

            for (int d = 0; d < days.Length; d++)
            {
                html.Append("<div class=\"opening-preview__line\">")
                    .Append(HttpUtility.HtmlEncode(OpeningHours.Line(d, days[d], PreviewEnglish)))
                    .Append("</div>");
            }

            Literal_Preview.Text = html.ToString();
        }

        // 510 -> "08:30", the value of a time box
        private static string TimeBox(int minutes)
        {
            return (minutes / 60).ToString("00") + ":" + (minutes % 60).ToString("00");
        }

        // the text of a time box -> minutes after midnight, or -1 when it is not a time
        private static int ParseTime(string text)
        {
            Match match = TimePattern.Match((text ?? string.Empty).Trim());
            if (!match.Success)
            {
                return -1;
            }

            int hour = int.Parse(match.Groups[1].Value);
            int minute = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;

            return hour > 23 || minute > 59 ? -1 : hour * 60 + minute;
        }

        // Every open day is checked first, then all seven are saved together. A closed day keeps no hour of its own.
        // The shop cannot open at 0:xx: in the table an opening hour of 0 is what says "closed".
        private void Opening_Save()
        {
            if (Repeater_Days.Items.Count != OpeningHours.DayCount)
            {
                AdminUi.Toast(this, Global.Text("Admin_Error"), false);
                return;
            }

            var days = new OpeningDay[OpeningHours.DayCount];

            foreach (RepeaterItem item in Repeater_Days.Items)
            {
                int d = item.ItemIndex;
                string name = OpeningHours.DayName(d);

                days[d] = new OpeningDay { Open = ((CheckBox)item.FindControl("CheckBox_Open")).Checked };

                if (!days[d].Open)
                {
                    continue;
                }

                days[d].OpensAt = ParseTime(((TextBox)item.FindControl("TextBox_From")).Text);
                days[d].ClosesAt = ParseTime(((TextBox)item.FindControl("TextBox_To")).Text);

                if (days[d].OpensAt < 0 || days[d].ClosesAt < 0)
                {
                    AdminUi.Toast(this, Global.Format(Global.Text("Admin_OpeningInvalid"), name), false);
                    return;
                }

                if (days[d].OpensAt < 60)
                {
                    AdminUi.Toast(this, Global.Format(Global.Text("Opening_EarlyOpen"), name), false);
                    return;
                }

                if (days[d].ClosesAt <= days[d].OpensAt)
                {
                    AdminUi.Toast(this, Global.Format(Global.Text("Admin_OpeningOrder"), name), false);
                    return;
                }
            }

            try
            {
                OpeningHours.Save(connectionString, days);

                var detail = new StringBuilder();
                for (int d = 0; d < days.Length; d++)
                {
                    detail.Append(d == 0 ? string.Empty : ", ").Append(d + 1).Append("=")
                        .Append(days[d].Open ? TimeBox(days[d].OpensAt) + "-" + TimeBox(days[d].ClosesAt) : "closed");
                }

                AdminAudit.Write("Opening hours changed", "days 1 (Monday) to 7: " + detail);
                Opening_Load();
                AdminUi.Toast(this, Global.Admin_OpeningSavedSuccess, true);
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        // the details go to the error log (App_Data\logs), never to the browser
        private void Fail(Exception ex)
        {
            ErrorLogger.Log(ex, "opening hours page");
            AdminUi.Toast(this, Global.Text("Admin_Error"), false);
        }

        protected void Button_Opening_Save_Click(object sender, EventArgs e)
        {
            Opening_Save();
        }
    }
}
