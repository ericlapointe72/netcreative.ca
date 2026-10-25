using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace netcreative.ca
{
    // The checks made on an e-mail address typed in a form (the same as on beaucedutyfree.com).
    public static class EmailAddressCheck
    {
        // a real domain ending in at least two letters ("x@live.c" or "x@live" are refused; ".quebec" or ".photography" are accepted)
        private static readonly Regex Syntax = new Regex(
            @"^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@([A-Za-z0-9]([A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+([A-Za-z]{2,}|xn--[A-Za-z0-9-]{2,})$",
            RegexOptions.Compiled);

        public static bool IsValid(string email)
        {
            email = (email ?? string.Empty).Trim();

            if (email.Length == 0 || email.Length > 254)
            {
                return false;
            }

            int at = email.LastIndexOf('@');
            if (at < 1 || at > 64)
            {
                return false;
            }

            return Syntax.IsMatch(email);
        }

        // A Gmail address built by a robot: Gmail ignores the dots, so a script writes a mailbox with dots everywhere and numbers
        // ("o.hi.mihi.j.o.26@gmail.com"). Three dots or more, at least two very short pieces, and a digit. A person's address with
        // initials ("marie.j.p.tremblay@gmail.com") has no digit and is not caught. Such an address is not even stored.
        public static bool LooksGenerated(string email)
        {
            email = (email ?? string.Empty).Trim().ToLowerInvariant();
            int at = email.LastIndexOf('@');
            if (at < 1)
            {
                return false;
            }

            string domain = email.Substring(at + 1);
            if (domain != "gmail.com" && domain != "googlemail.com")
            {
                return false;
            }

            string local = email.Substring(0, at);
            int plus = local.IndexOf('+');
            if (plus > 0)
            {
                local = local.Substring(0, plus);
            }

            string[] pieces = local.Split('.');
            if (pieces.Length - 1 < 3)
            {
                return false;
            }

            return pieces.Count(p => p.Length <= 2) >= 2 && local.Any(char.IsDigit);
        }
    }
}
