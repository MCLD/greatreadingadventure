using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.Tools;

namespace GRA.SRP.Classes {
    public partial class ChangeFamilyPassword : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                if(string.IsNullOrEmpty(Request["SA"]) && (Session["SA"] == null || Session["SA"].ToString() == "")) {
                    Response.Redirect("~/FamilyAccountList.aspx");
                }
                if(!string.IsNullOrEmpty(Request["SA"])) {
                    SA.Text = Request["SA"];
                    Session["SA"] = SA.Text;
                } else {
                    SA.Text = Session["SA"].ToString();
                }

                // now validate user can change password for SA Sub Account

                var patron = (Patron)Session["Patron"];
                //if (!patron.IsMasterAccount)
                if(Session[SessionKey.IsMasterAccount] == null || !(bool)Session[SessionKey.IsMasterAccount]) {
                    // kick them out
                    Response.Redirect("~");
                }

                if(!Patron.CanManageSubAccount((int)Session["MasterAcctPID"], int.Parse(SA.Text))) {
                    // kick them out
                    Response.Redirect("~");
                }
                var sa = Patron.FetchObject(int.Parse(SA.Text));

                lblAccount.Text = DisplayHelper.FormatName(sa.FirstName, sa.LastName, sa.Username);
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e) {
            if(Page.IsValid) {
                if(!(string.IsNullOrEmpty(NPassword.Text.Trim()))) {
                    var patron = Patron.FetchObject((int)Session["MasterAcctPID"]);//(Patron)Session["Patron"];)
                    if(!Patron.VerifyPassword(patron.Username, CPass.Text.Trim())) {
                        new SessionTools(Session).AlertPatron("The password entered for the head of the family (you) is not correct, please try entering your current password again.",
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
                    var sa = Patron.FetchObject(int.Parse(SA.Text));
                    sa.NewPassword = NPassword.Text.Trim();
                    sa.Update();

                    new SessionTools(Session).AlertPatron(string.Format("The password for {0} has been updated!", 
                        DisplayHelper.FormatName(sa.FirstName, sa.LastName, sa.Username)),
                        glyphicon: "check");
                    Response.Redirect("~/Account/");
                }
            }


        }

        protected void btnCancel_Click(object sender, EventArgs e) {
            Response.Redirect("~/FamilyAccountList.aspx");
        }

    }
}