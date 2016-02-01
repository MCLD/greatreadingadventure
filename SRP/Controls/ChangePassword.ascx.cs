using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;

namespace GRA.SRP.Classes {
    public partial class ChangePassword : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
        }

        protected void btnLogin_Click(object sender, EventArgs e) {
            if(Page.IsValid) {
                if(!(string.IsNullOrEmpty(NPassword.Text.Trim()))) {
                    var patron = (Patron)Session["Patron"];
                    if(!Patron.VerifyPassword(patron.Username, CPass.Text.Trim())) {
                        new SessionTools(Session).AlertPatron("That's not your current password, please try entering your current password again.",
                            PatronMessageLevels.Danger, "remove");

                        CPass.Attributes.Add("Value", CPass.Text);
                        NPassword.Attributes.Add("Value", NPassword.Text);
                        NPasswordR.Attributes.Add("Value", NPasswordR.Text);
                        return;
                    }

                    if(NPassword.Text.Trim() != NPasswordR.Text.Trim()) {
                        new SessionTools(Session).AlertPatron("New password and new password validation do not match.",
                            PatronMessageLevels.Danger, "remove");
                        CPass.Attributes.Add("Value", CPass.Text);
                        NPassword.Attributes.Add("Value", NPassword.Text);
                        NPasswordR.Attributes.Add("Value", NPasswordR.Text);
                        return;
                    }
                    patron.NewPassword = NPassword.Text.Trim();
                    patron.Update();

                    var st = new SessionTools(Session);

                    st.EstablishPatron(patron);
                    //Session["Patron"] = patron;
                    //Session["ProgramID"] = patron.ProgID;

                    st.AlertPatron("Your password has been updated!", glyphicon: "check");
                    Response.Redirect("~/Account/");
                }
            }
        }
    }
}