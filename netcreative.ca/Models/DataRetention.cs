using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;

namespace netcreative.ca
{
    // How long the site keeps what visitors send it. The privacy policy promises it, so the site does it by itself:
    //   - sign-in log: 12 months (AdminLogin.PurgeOldLogOncePerDay)
    //   - e-mail address never confirmed: 30 days (SubscribeService.PurgeUnconfirmedOncePerDay)
    //   - messages of the contact form: 24 months (here)
    public static class DataRetention
    {
        public const int ContactMonths = 24;

        // Once a day (at the first sign-in after the application starts, at most): the messages of the contact form received more than
        // 24 months ago are erased. A single DELETE: there is nothing to undo half way.
        public static void PurgeOldContactsOncePerDay()
        {
            if (HttpRuntime.Cache["nc_contact_purged"] != null)
            {
                return;
            }

            HttpRuntime.Cache.Insert("nc_contact_purged", true, null, DateTime.UtcNow.AddHours(24), Cache.NoSlidingExpiration);

            try
            {
                int erased;

                using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                using (var cmd = new SqlCommand("DELETE FROM CONTACT WHERE DATE < DATEADD(month, -@MONTHS, CAST(GETDATE() AS date));", cn))
                {
                    cmd.Parameters.AddWithValue("@MONTHS", ContactMonths);
                    cn.Open();
                    erased = cmd.ExecuteNonQuery();
                }

                if (erased > 0)
                {
                    AdminAudit.Write("Contact messages purged", erased + " message(s) older than " + ContactMonths + " months (automatic)");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "contact purge");
            }
        }
    }
}
