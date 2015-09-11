using GRA.SRP.Core.Utilities;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Modules.Security {
    public partial class ChangePassword : BaseControlRoomPage {
        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.IsSecure = false;
            MasterPage.PageTitle = "Summer Reading Program - Add / Edit User";

            ControlRoomAccessPermission.CheckControlRoomAccessPermission(1000); // Change Appropriately;

            if(!IsPostBack) {
                List<RibbonPanel> moduleRibbonPanels = StandardModuleRibbons.SecurityRibbon();
                foreach(var moduleRibbonPanel in moduleRibbonPanels) {
                    MasterPage.PageRibbon.Add(moduleRibbonPanel);
                }
                MasterPage.PageRibbon.DataBind();

                object userIdObject = Session["UID"];

                int userId = 0;
                if(userIdObject == null
                   || !int.TryParse(userIdObject.ToString(), out userId)
                   || userId == 0) {
                    Response.Redirect("Default.aspx");
                }
                SRPUser user = SRPUser.Fetch(userId);
                this.Username.Text = user.Username;
            }

            Error.Visible = !string.IsNullOrEmpty(Error.Text);
        }

        protected void ResetPassword_Click(object sender, EventArgs e) {
            Error.Text = string.Empty;
            object userIdObject = Session["UID"];

            int userId = 0;
            if(userIdObject == null
               || !int.TryParse(userIdObject.ToString(), out userId)
               || userId == 0) {
                Response.Redirect("Default.aspx");
            }

            SRPUser user = SRPUser.Fetch(userId);
            user.NewPassword = uxPassword.Text;
            try {
                user.Update();
                SRPUser currentUser = Session[SessionData.UserProfile.ToString()] as SRPUser;
                var changeInfo = new {
                    Changer = currentUser == null
                              ? "unknown"
                              : currentUser.Username,
                    User = user.Username
                };
                this.Log().Info("Admin user {0} changed password for user {1}",
                                changeInfo.Changer,
                                changeInfo.User);
            } catch (Exception ex) {
                this.Log().Error("Admin user unable to change password for user {0}: {1}",
                                 userId,
                                 ex.Message);
                Error.Text = string.Format("An error occurred: {0}", ex.Message);
            }
            Response.Redirect("UserAddEdit.aspx");
        }

        protected void Cancel_Click(object sender, EventArgs e) {
            Response.Redirect("UserAddEdit.aspx");
        }
    }
}