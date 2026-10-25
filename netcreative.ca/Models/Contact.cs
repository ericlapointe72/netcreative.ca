using System;
using System.Text.RegularExpressions;

namespace netcreative.ca
{
    // One message of the CONTACT table, as the admin page "Contact" shows it. Everything here is text typed by a visitor: the page
    // encodes it when it writes it.
    public class Contact
    {
        // AUTO_NUMBER of the row in the CONTACT table (what the Delete button of the row names)
        public int Number { get; set; }

        // "2026-10-23 · 11:11"
        public string When { get; set; }

        public string Name { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ProjectType { get; set; }
        public string Budget { get; set; }
        public string Deadline { get; set; }
        public string Message { get; set; }

        // the number as a tel: link (digits and a leading +, nothing else)
        public string PhoneLink
        {
            get { return Regex.Replace(Phone ?? string.Empty, @"[^\d+]", string.Empty); }
        }

        public bool HasCompany
        {
            get { return !string.IsNullOrWhiteSpace(Company); }
        }

        public bool HasPhone
        {
            get { return PhoneLink.Length > 0; }
        }
    }
}
