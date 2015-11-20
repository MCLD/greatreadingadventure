using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.Communications;
using GRA.Tools;
using System.Text;

namespace GRA.SRP.Classes {
    public partial class RecoverPassword : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {

        }

        protected void btnEmail_Click(object sender, EventArgs e) {
            if(!string.IsNullOrEmpty(PUsername.Text.Trim())) {
                var patron = Patron.GetObjectByUsername(PUsername.Text.Trim());
                // Show message no matter what, even if we can't do it, because of hacking concerns

                if(patron == null || patron.EmailAddress == "") {
                    lbMessage.Text = "Your account does not have an email address associated with it or your email address is invalid.<br><br>Please visit your local library branch to reset your password.";
                } else {
                    string remoteAddress = Request.UserHostAddress;

                    string passwordResetToken = patron.GeneratePasswordResetToken();
                    if(string.IsNullOrEmpty(passwordResetToken)) {
                        lbMessage.Text = "Unable to initiate password reset process.";
                        return;
                    }

                    var values = new {
                        SystemName = SRPSettings.GetSettingValue("SysName"),
                        PasswordResetLink = string.Format("{0}{1}?token={2}",
                                                          WebTools.GetBaseUrl(Request),
                                                          "/PasswordRecovery.aspx",
                                                          passwordResetToken),
                        ContactName = SRPSettings.GetSettingValue("ContactName"),
                        ContactEmail = SRPSettings.GetSettingValue("ContactEmail"),
                        RemoteAddress = remoteAddress,
                        UserEmail = patron.EmailAddress,
                        Username = patron.Username,
                        PasswordResetSubject = "Password reset request"
                    };

                    StringBuilder body = new StringBuilder();
                    body.Append("<p>A password reset request was received by {SystemName} for ");
                    body.Append("your account: {Username}.</p><p>Please ");
                    body.Append("<a href=\"{PasswordResetLink}\">click here</a> in the next hour ");
                    body.Append("to create a new password for your account.</p>");
                    body.Append("<p>If you did not initiate this request, take no action and your ");
                    body.Append("password will not be changed.</p>");
                    body.Append("<p>If you have any comments or questions, please contact ");
                    body.Append("{ContactName} at <a href=\"mailto:{ContactEmail}\">{ContactEmail}");
                    body.Append("</a>.</p>");
                    body.Append("<p style=\"font-size: smaller;\"><em>This password request was ");
                    body.Append("submitted from: {RemoteAddress}.</em></p>");

                    EmailService.SendEmail(patron.EmailAddress,
                                           "{SystemName} - {PasswordResetSubject}".FormatWith(values),
                                           body.ToString().FormatWith(values));
                    lbMessage.Text = "Processing your password reset request, you should receive an email soon.";
                }

                new SessionTools(Session).ClearPatron();
            }
        }
    }
}