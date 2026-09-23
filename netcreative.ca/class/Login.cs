using System;

namespace netcreative.ca
{
    public class Login
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

        private string user;

        public string User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
            }
        }

        private string action;

        public string Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }
    }
}