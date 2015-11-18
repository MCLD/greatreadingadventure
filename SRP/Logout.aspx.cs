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
            var navTo = "~/";
            if(IsLoggedIn) {
                var p = Programs.FetchObject((int)Session["PatronProgramID"]);
                if(p.LogoutURL.Trim().Length > 0) {
                    navTo = p.LogoutURL;
                }
            }

            new PatronSession(Session).Clear();
            Session.Abandon();
            if(Request.Cookies["ASP.NET_SessionId"] != null) {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }
            Response.Redirect(navTo);
        }
    }
}