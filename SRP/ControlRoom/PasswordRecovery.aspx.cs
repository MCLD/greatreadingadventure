using GRA.Communications;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using SRPApp.Classes;
using System;
using System.Text;
using System.Web.UI;

namespace GRA.SRP.ControlRoom {
    public partial class PasswordRecovery : BaseControlRoomPage {
        protected void Page_Load(object sender, EventArgs e) {
            if(!Page.IsPostBack) {
                string token = Request.QueryString["token"];
                this.ViewState["token"] = token;
                if(string.IsNullOrEmpty(token)) {
                    Response.Redirect("~/ControlRoom/PasswordReset.aspx");
                } else {
                    if(SRPUser.GetUserByToken(token) == null) {
                        passwordUpdate.Visible = false;
                        invalidToken.Visible = true;
                    }
                }
            }
        }
        protected void Button1_Click(object sender, EventArgs e) {
            object tokenObject = this.ViewState["token"];
            if(tokenObject == null) {
                passwordUpdate.Visible = false;
                invalidToken.Visible = true;
                return;
            }

            var user = SRPUser.UpdatePasswordByToken(tokenObject.ToString(),
                                                     uxNewPasswordField.Text);

            if(user == null) {
                passwordUpdate.Visible = false;
                invalidToken.Visible = true;
                return;
            }

            // user requested a password for an email address that is not in the database
            // if account doesn't exist, send an email saying so
            var values = new {
                SystemName = SRPSettings.GetSettingValue("SysName", user.TenID),
                ContactName = SRPSettings.GetSettingValue("ContactName", user.TenID),
                ContactEmail = SRPSettings.GetSettingValue("ContactEmail", user.TenID),
                RemoteAddress = Request.UserHostAddress,
                UserEmail = user.EmailAddress,
                ControlRoomLink = string.Format("{0}{1}",
                                                BaseUrl,
                                                "/ControlRoom/"),
                PasswordResetSuccessSubject = SRPResources.PasswordEmailSuccessSubject
            };

            this.Log().Info("Password reset process for {0} complete from {1}",
                            values.UserEmail,
                            values.RemoteAddress);

            // TODO email - move this template out to the database
            StringBuilder body = new StringBuilder();
            body.Append("<p>The password reset for your {SystemName} account is now complete.</p>");
            body.Append("<p>You may now <a href=\"{ControlRoomLink}\">log in</a> using your new ");
            body.Append("password.</p>");
            body.Append("<p>If you have any comments or questions, please contact ");
            body.Append("{ContactName} at <a href=\"mailto:{ContactEmail}\">{ContactEmail}");
            body.Append("</a>.</p>");
            body.Append("<p style=\"font-size: smaller;\"><em>This password request was ");
            body.Append("completed from: {RemoteAddress}.</em></p>");

            new EmailService().SendEmail(user.EmailAddress,
                                         "{SystemName} - {PasswordResetSuccessSubject}".FormatWith(values),
                                         body.ToString().FormatWith(values));
        
            Response.Redirect("Login.aspx");
        }
    }
}