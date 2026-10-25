using System.Web;

namespace netcreative.ca
{
    public class User_IP
    {
        // The visitor's address as AdminLogin.ClientIp works it out: the machine that connected, or, behind the host's own proxy,
        // the address the proxy saw. A visitor can write anything in X-Forwarded-For, so it is never taken as it comes.
        public static string Get_UserIP()
        {
            return AdminLogin.ClientIp(HttpContext.Current);
        }
    }
}
