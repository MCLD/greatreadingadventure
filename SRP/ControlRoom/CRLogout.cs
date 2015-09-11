using GRA.SRP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace GRA.SRP.ControlRoom {
    public static class CRLogout {
        public static void Logout(Page page) {
            try {
                SRPUser u = (SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()];
                if(u != null) {
                    u.Logoff();
                }
            } finally {
                page.Session.Abandon();
                page.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                page.Response.Redirect("~/ControlRoom/Login.aspx", true);
            }
        }
    }
}
