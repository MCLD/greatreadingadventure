using GRA.Communications;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

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
            //  check if token is valid (exists, hasn't expired) - if not, advise to start over
            //  allow password to be changed
            //  delete reset token
            //  email user confirmation of change
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

            string baseUrl = string.Format("{0}://{1}{2}",
                                           Request.Url.Scheme,
                                           Request.Url.Authority,
                                           Request.ApplicationPath.TrimEnd('/'));

            var values = new {
                SystemName = SRPSettings.GetSettingValue("SysName"),
                ContactName = SRPSettings.GetSettingValue("ContactName"),
                ContactEmail = SRPSettings.GetSettingValue("ContactEmail"),
                RemoteAddress = Request.UserHostAddress,
                UserEmail = user.EmailAddress,
                ControlRoomLink = string.Format("{0}{1}",
                                                baseUrl,
                                                "/ControlRoom/"),
                PasswordResetSuccessSubject = SRPResources.PasswordEmailSuccessSubject
            };

            this.Log().Info(() => "Password reset process for {UserEmail} complete from {RemoteAddress}"
                                  .FormatWith(values));

            // TODO move this template out to the database
            StringBuilder body = new StringBuilder();
            body.Append("<p>The password reset for your {SystemName} account is now complete.</p>");
            body.Append("<p>You may now <a href=\"{ControlRoomLink}\">log in</a> using your new ");
            body.Append("password.</p>");
            body.Append("<p>If you have any comments or questions, please contact ");
            body.Append("{ContactName} at <a href=\"mailto:{ContactEmail}\">{ContactEmail}");
            body.Append("</a>.</p>");
            body.Append("<p style=\"font-size: smaller;\"><em>This password request was ");
            body.Append("completed from: {RemoteAddress}.</em></p>");

            EmailService.SendEmail(user.EmailAddress,
                                   "{SystemName} - {PasswordResetSuccessSubject}".FormatWith(values),
                                   body.ToString().FormatWith(values));

            Response.Redirect("Login.aspx");
        }
    }
}