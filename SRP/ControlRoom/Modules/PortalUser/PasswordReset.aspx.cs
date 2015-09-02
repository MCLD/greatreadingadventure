using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;

namespace GRA.SRP.ControlRoom.Modules.PortalUser {
    public partial class PasswordReset : BaseControlRoomPage {
        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = "You Must Reset Your Password!";

            if(!IsPostBack) {
                List<RibbonPanel> moduleRibbonPanels = StandardModuleRibbons.MyAccountRibbon();
                foreach(var moduleRibbonPanel in moduleRibbonPanels) {
                    MasterPage.PageRibbon.Add(moduleRibbonPanel);
                }
                MasterPage.PageRibbon.DataBind();
            }

            if(uxPassword.Text != "") {
                uxPassword.Attributes["value"] = uxPassword.Text;
            }
            if(uxReEnter.Text != "") {
                uxReEnter.Attributes["value"] = uxReEnter.Text;
            }
            if(uxCPass.Text != "") {
                uxCPass.Attributes["value"] = uxCPass.Text;
            }

        }

        protected void uvButton_Click(object sender, EventArgs e) {
            SRPUser user = new SRPUser((int)((SRPUser)Session[SessionData.UserProfile.ToString()]).Uid);
            var valid = SRPUser.VerifyPassword(user.Username, uxCPass.Text);
            if(!valid) {
                MasterPage.PageError = String.Format(SRPResources.ApplicationError1, "Your current password is invalid.");
                return;
            }
            user.LastPasswordReset = DateTime.Now;
            user.MustResetPassword = false;
            user.NewPassword = uxPassword.Text;
            try {
                user.ClearErrorCodes();
                if(user.Update()) {
                    Session[SessionData.UserProfile.ToString()] = user;
                    MasterPage.PageMessage = String.Format("Password has been changed.");
                } else {
                    string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                    foreach(BusinessRulesValidationMessage m in user.ErrorCodes) {
                        message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                    }
                    message = string.Format("{0}</ul>", message);
                    MasterPage.PageError = message;

                }
            } catch(Exception ex) {
                MasterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);

            }
        }

        protected void uvButton0_Click(object sender, EventArgs e) {
            Response.Redirect("~/ControlRoom/");
        }
    }
}
