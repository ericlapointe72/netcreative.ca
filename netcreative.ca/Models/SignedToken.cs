using System;
using System.Text;
using System.Web.Security;

namespace netcreative.ca
{
    // A short text that only this site can have written and nobody can read or change: it is encrypted and signed with the site's
    // machine key (App_Data\machineKey.config), for one purpose (a token made for one use is refused by another). Used for the
    // unsubscribe links and the "form was shown at this time" stamp, so no extra secret has to be kept in the settings.
    public static class SignedToken
    {
        private const int MaxLength = 1200;

        public static string Protect(string text, string purpose)
        {
            byte[] protectedBytes = MachineKey.Protect(Encoding.UTF8.GetBytes(text ?? string.Empty), "netcreative.ca", purpose);
            return Convert.ToBase64String(protectedBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        // the text, or null when the token is not one this site made for this purpose (altered, from another purpose, cut short)
        public static string Unprotect(string token, string purpose)
        {
            if (string.IsNullOrEmpty(token) || token.Length > MaxLength)
            {
                return null;
            }

            try
            {
                string base64 = token.Replace('-', '+').Replace('_', '/');
                base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');

                byte[] bytes = MachineKey.Unprotect(Convert.FromBase64String(base64), "netcreative.ca", purpose);
                return bytes == null ? null : Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
