using System;
using System.Globalization;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected DashboardStats Stats;

        protected void Page_Load(object sender, EventArgs e)
        {
            Stats = DashboardStats.Load(Convert.ToString(Session["user"]));
        }

        // "30 octobre 2026" / "30 oct. 2026 14:05"
        protected static string Day(DateTime date)
        {
            return date.ToString("d MMMM yyyy", CultureInfo.CurrentCulture);
        }

        protected static string DayTime(DateTime date)
        {
            return date.ToString("d MMM yyyy HH:mm", CultureInfo.CurrentCulture);
        }

        // the card turns orange when something deserves a look: an error today, or a series of failed sign-ins
        protected string ErrorsClass
        {
            get { return Stats.ErrorsToday > 0 ? "dash-stat--warn" : "dash-stat--ok"; }
        }

        protected string FailedClass
        {
            get { return Stats.FailedSignIns24h >= 5 ? "dash-stat--warn" : "dash-stat--ok"; }
        }

        protected string MessagesSub
        {
            get
            {
                return Stats.ContactLast.HasValue
                    ? Global.Format(Global.Text("Dash_MessagesSub"), Stats.ContactTotal, Day(Stats.ContactLast.Value))
                    : Global.Text("Dash_MessagesNone");
            }
        }

        protected string NewsletterSub
        {
            get
            {
                return Stats.NewsletterDate.HasValue
                    ? Global.Format(Global.Text("Dash_NewsletterSub"), Stats.NewsletterSubject, Stats.NewsletterSent, Stats.NewsletterFailed)
                    : Global.Text("Dash_NewsletterNone");
            }
        }
    }
}
