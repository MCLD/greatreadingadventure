using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.ControlRoom {
    public partial class PasswordReset : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(Session[SessionData.IsLoggedIn.ToString()] == null || !(bool)Session[SessionData.IsLoggedIn.ToString()])
                Response.Redirect("~/ControlRoom/Logoff.aspx");

        }
        protected void Button1_Click(object sender, EventArgs e) {
            SRPUser user = (SRPUser)Session[SessionData.UserProfile.ToString()];
            user.NewPassword = uxNewPasswordField.Text;
            user.MustResetPassword = false;
            user.LastPasswordReset = DateTime.Now;
            user.Update();
            Session[SessionData.UserProfile.ToString()] = user;
            FormsAuthentication.RedirectFromLoginPage(user.Username, false);
        }

    }
}