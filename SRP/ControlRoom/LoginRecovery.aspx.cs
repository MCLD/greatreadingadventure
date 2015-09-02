using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.Communications;
using System.Text;
using SRPApp.Classes;

namespace GRA.SRP.ControlRoom {
    public partial class LoginRecovery : BaseControlRoomPage {
        protected void Page_Load(object sender, EventArgs e) {

        }
        protected void Button1_Click(object sender, EventArgs e) {
            string userId = new SRPUser().GetUsernameByEmail(uxEmailaddress.Text);
            string senderAddress = Request.UserHostAddress;
            string baseUrl = string.Format("{0}://{1}{2}",
                                           Request.Url.Scheme,
                                           Request.Url.Authority,
                                           Request.ApplicationPath.TrimEnd('/'));

            if(string.IsNullOrEmpty(userId)) {
                // user requested a password for an email address that is not in the database
                // if account doesn't exist, send an email saying so

                var values = new {
                    SystemName = SRPSettings.GetSettingValue("SysName"),
                    ControlRoomLink = string.Format("{0}{1}",
                                                    baseUrl,
                                                    "/ControlRoom/LoginRecovery.aspx"),
                    ContactName = SRPSettings.GetSettingValue("ContactName"),
                    ContactEmail = SRPSettings.GetSettingValue("ContactEmail"),
                    RemoteAddress = senderAddress,
                    UserEmail = uxEmailaddress.Text,
                    PasswordResetSubject = SRPResources.PasswordEmailSubject
                };

                this.Log().Info(() => "User at {RemoteAddress} requested password reset for nonexistent email {UserEmail}"
                                      .FormatWith(values));

                // TODO move this template out to the database
                StringBuilder body = new StringBuilder();
                body.Append("<p>A password reset request was received by {SystemName} for your ");
                body.Append("address. Unfortunately no account could be found associated with ");
                body.Append("this email address.</p>");
                body.Append("<p>If you initiated this request, feel free to ");
                body.Append("<a href=\"{ControlRoomLink}\">try requesting the password</a> ");
                body.Append("for any other email address you might have used.</p>");
                body.Append("<p>If you have any comments or questions, please contact ");
                body.Append("{ContactName} at <a href=\"mailto:{ContactEmail}\">{ContactEmail}");
                body.Append("</a>.</p>");
                body.Append("<p style=\"font-size: smaller;\"><em>This password request was ");
                body.Append("submitted from: {RemoteAddress}.</em></p>");

                EmailService.SendEmail(uxEmailaddress.Text,
                                       "{SystemName} - {PasswordResetSubject}".FormatWith(values),
                                       body.ToString().FormatWith(values));

            } else {
                SRPUser lookupUser = SRPUser.FetchByUsername(userId);
                string passwordResetToken = lookupUser.GeneratePasswordResetToken();
                if(string.IsNullOrEmpty(passwordResetToken)) {
                    lblMessage.Text = "Unable to initiate password reset process.";
                    return;
                }

                var values = new {
                    SystemName = SRPSettings.GetSettingValue("SysName"),
                    PasswordResetLink = string.Format("{0}{1}?token={2}",
                                                      baseUrl,
                                                      "/ControlRoom/PasswordRecovery.aspx",
                                                      passwordResetToken),
                    ContactName = SRPSettings.GetSettingValue("ContactName"),
                    ContactEmail = SRPSettings.GetSettingValue("ContactEmail"),
                    RemoteAddress = senderAddress,
                    UserEmail = uxEmailaddress.Text,
                    PasswordResetSubject = SRPResources.PasswordEmailSubject,
                };

                this.Log().Info(() => "User at {RemoteAddress} requested password reset for email {UserEmail}"
                                      .FormatWith(values));
                // TODO move this template out to the database
                StringBuilder body = new StringBuilder();
                body.Append("<p>A password reset request was received by {SystemName} for your ");
                body.Append("address.</p>");
                body.Append("<p>Please <a href=\"{PasswordResetLink}\">click here</a> to create ");
                body.Append("a new password for your account.</p>");
                body.Append("<p>If you did not initiate this request, take no action and your ");
                body.Append("password will not be changed.</p>");
                body.Append("<p>If you have any comments or questions, please contact ");
                body.Append("{ContactName} at <a href=\"mailto:{ContactEmail}\">{ContactEmail}");
                body.Append("</a>.</p>");
                body.Append("<p style=\"font-size: smaller;\"><em>This password request was ");
                body.Append("submitted from: {RemoteAddress}.</em></p>");

                EmailService.SendEmail(uxEmailaddress.Text,
                                       "{SystemName} - {PasswordResetSubject}".FormatWith(values),
                                       body.ToString().FormatWith(values));
            }

            lblMessage.Text = "Processing your password reset request, you should receive an email soon.";
        }
    }

}