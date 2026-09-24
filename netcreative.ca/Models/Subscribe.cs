using System;

namespace netcreative.ca
{
    public class Subscribe
    {
        private string number;

        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
            }
        }

        private string date;

        public string Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
            }
        }

        private string time;

        public string Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        private string ip_address;

        public string IP_Address
        {
            get
            {
                return ip_address;
            }
            set
            {
                ip_address = value;
            }
        }

        private string email;

        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }
    }
}