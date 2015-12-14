using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace GRA.SRP {
    public partial class Logout : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            IsSecure = false;
            var navTo = "~";
            if(IsLoggedIn) {
                Patron patron = Session["Patron"] as Patron;
                if(patron != null) {
                    var p = Programs.FetchObject(patron.ProgID);
                    if(p.LogoutURL.Trim().Length > 0) {
                        navTo = p.LogoutURL;
                    }
                }
            }

            new SessionTools(Session).ClearPatron();
            Session.Abandon();
            if(Request.Cookies["ASP.NET_SessionId"] != null) {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }
            Response.Redirect(navTo);
        }
    }
}