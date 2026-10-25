using System;

namespace netcreative.ca
{
    // The link that unsubscribes one address: the address itself, encrypted and signed by the site (SignedToken). Every newsletter carries
    // the link of its own recipient; nobody can make one for another address, and it never expires (a link in an old e-mail must work).
    public static class UnsubscribeLink
    {
        private const string Purpose = "unsubscribe";

        public static string Url(string email)
        {
            return Mailer.SiteBase + "/unsubscribe.aspx?t=" + Token(email);
        }

        public static string Token(string email)
        {
            return SignedToken.Protect((email ?? string.Empty).Trim().ToLowerInvariant(), Purpose);
        }

        // the address the link was made for, or null when the link is not one of the site's
        public static string Email(string token)
        {
            return SignedToken.Unprotect(token, Purpose);
        }
    }
}
