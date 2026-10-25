using System;
using System.Globalization;

namespace netcreative.ca
{
    // A signed "the form was shown at this time" stamp, put in a form when the page is drawn and sent back with it. A person needs a few
    // seconds to fill a form; a script that posts it at once, or that never loaded the page, has no valid stamp. The stamp cannot be
    // forged (SignedToken) and expires after a week.
    public static class FormTimer
    {
        public enum Verdict
        {
            Ok,
            // sent less than the minimum number of seconds after the form was shown: a robot
            TooFast,
            // missing, altered or too old: the visitor is asked to reload the page
            Invalid
        }

        private const string Purpose = "form timer";
        private const int MaxAgeDays = 7;

        public static string Issue()
        {
            return SignedToken.Protect(ToUnixSeconds(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), Purpose);
        }

        public static Verdict Check(string token, int minimumSeconds)
        {
            string stamp = SignedToken.Unprotect(token, Purpose);
            long seconds;

            if (stamp == null || !long.TryParse(stamp, NumberStyles.None, CultureInfo.InvariantCulture, out seconds))
            {
                return Verdict.Invalid;
            }

            double age = ToUnixSeconds(DateTime.UtcNow) - seconds;

            if (age > MaxAgeDays * 86400.0 || age < -300)
            {
                return Verdict.Invalid;
            }

            return age < minimumSeconds ? Verdict.TooFast : Verdict.Ok;
        }

        private static long ToUnixSeconds(DateTime utc)
        {
            return (long)(utc.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}
