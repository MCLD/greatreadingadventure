using GRA.SRP.Core.Utilities;
using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace GRA.SRP.ControlRoom
{
    public static class CRLogout
    {
        public static void Logout(Page page)
        {
            try
            {
                SRPUser u = (SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()];
                if (u != null)
                {
                    u.Logoff();
                }
            }
            finally
            {
                page.Session.Abandon();
                var yesterday = DateTime.Now.AddDays(-1);

                var sessionCookie = new HttpCookie("ASP.NET_SessionId", string.Empty);
                sessionCookie.Expires = yesterday;
                page.Response.Cookies.Add(sessionCookie);

                var formsCookie = page.Request.Cookies[FormsAuthentication.FormsCookieName];
                formsCookie.Expires = yesterday;
                formsCookie.Value = string.Empty;
                page.Response.Cookies.Add(formsCookie);

                page.Response.Redirect("~/ControlRoom/Login.aspx", true);
            }
        }
    }
}
