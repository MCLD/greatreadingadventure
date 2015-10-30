using GRA.Communications;
using GRA.SRP.DAL;
using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Text;
using System.Web.UI;

namespace GRA.SRP {
    public partial class PasswordRecovery : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            uxNewPasswordStrengthValidator.ValidationExpression = STGOnlyUtilities.PasswordStrengthRE();
            uxNewPasswordStrengthValidator.ErrorMessage = STGOnlyUtilities.PasswordStrengthError();

            if(!Page.IsPostBack) {
                string token = Request.QueryString["token"];
                this.ViewState["token"] = token;
                if(string.IsNullOrEmpty(token)) {
                    Response.Redirect("~/Recover.aspx");
                } else {
                    Patron p = Patron.GetUserByToken(token);
                    if(Patron.GetUserByToken(token) == null) {
                        pnlfields.Visible = false;
                        lblError.Visible = true;
                        lblError.Text = "The provided password token is invalid. Please <a href=\"Recovery.aspx\">generate a new one</a> if you wish to change your password.";
                    } else {
                        Session["ProgramID"] = p.ProgID;
                    }
                }
            }
            TranslateStrings(this);
        }

        protected void btnLogin_Click(object sender, EventArgs e) {
            if(Page.IsValid) {
                object tokenObject = this.ViewState["token"];
                if(tokenObject == null) {
                    pnlfields.Visible = false;
                    lblError.Visible = true;
                    lblError.Text = "The provided password token is invalid. Please <a href=\"Recovery.aspx\">generate a new one</a> if you wish to change your password.";
                    return;
                }

                if(NPassword.Text.Trim() != NPasswordR.Text.Trim()) {
                    lblError.Visible = true;
                    lblError.Text =
                        "The new password and new password re-entry do not match.";
                    NPassword.Attributes.Add("Value", NPassword.Text);
                    NPasswordR.Attributes.Add("Value", NPasswordR.Text);
                    return;
                }

                var user = Patron.UpdatePasswordByToken(tokenObject.ToString(),
                                                        NPassword.Text);

                if(user == null) {
                    pnlfields.Visible = false;
                    lblError.Visible = true;
                    lblError.Text = "The provided password token is invalid. Please <a href=\"Recovery.aspx\">generate a new one</a> if you wish to change your password.";
                    return;
                }

                // user requested a password for an email address that is not in the database
                // if account doesn't exist, send an email saying so
                var values = new {
                    SystemName = SRPSettings.GetSettingValue("SysName"),
                    ContactName = SRPSettings.GetSettingValue("ContactName"),
                    ContactEmail = SRPSettings.GetSettingValue("ContactEmail"),
                    RemoteAddress = Request.UserHostAddress,
                    UserEmail = user.EmailAddress,
                    Username = user.Username,
                    LoginLink = string.Format("{0}{1}",
                                              WebTools.GetBaseUrl(Request),
                                              "/Login.aspx"),
                    PasswordResetSuccessSubject = "Your password has been reset!"
                };

                this.Log().Info("Password reset process for {0} ({1}) complete from {2}",
                                values.Username,
                                values.UserEmail,
                                values.RemoteAddress);

                // TODO email - move this template out to the database
                StringBuilder body = new StringBuilder();
                body.Append("<p>The password reset for your {SystemName} account {Username} is now");
                body.Append("complete.</p>");
                body.Append("<p>You may now <a href=\"{LoginLink}\">log in</a> using your new ");
                body.Append("password.</p>");
                body.Append("<p>If you have any comments or questions, please contact ");
                body.Append("{ContactName} at <a href=\"mailto:{ContactEmail}\">{ContactEmail}");
                body.Append("</a>.</p>");
                body.Append("<p style=\"font-size: smaller;\"><em>This password request was ");
                body.Append("completed from: {RemoteAddress}.</em></p>");

                EmailService.SendEmail(user.EmailAddress,
                                       "{SystemName} - {PasswordResetSuccessSubject}".FormatWith(values),
                                       body.ToString().FormatWith(values));


                new PatronSession(Session).Establish(user);

                Session[SessionKey.PatronMessageGlyphicon] = "ok";
                Session[SessionKey.PatronMessage] = "Your password has been reset!";
                Response.Redirect("~/Dashboard.aspx");
            }
        }
    }
}
