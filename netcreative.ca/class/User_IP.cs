using System.Web;

namespace netcreative.ca
{
    public class User_IP
    {
        public static string Get_UserIP()
        {
            string user_ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (user_ip == null)
            {
                user_ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                user_ip = user_ip.Split(',')[0];
            }

            return user_ip;
        }
    }
}