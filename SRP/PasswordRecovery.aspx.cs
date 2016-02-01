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
            if(!Page.IsPostBack) {
                string token = Request.QueryString["token"];
                this.ViewState["token"] = token;
                if(string.IsNullOrEmpty(token)) {
                    Response.Redirect("~/Recover.aspx");
                } else {
                    Patron p = Patron.GetUserByToken(token);
                    if(Patron.GetUserByToken(token) == null) {
                        new SessionTools(Session).AlertPatron(GetResourceString("password-recovery-expired"),
                                                              PatronMessageLevels.Warning,
                                                              "exclamation-sign");
                        Response.Redirect("~/Recover.aspx");
                        return;
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
                    new SessionTools(Session).AlertPatron(GetResourceString("password-recovery-expired"),
                                                          PatronMessageLevels.Warning,
                                                          "exclamation-sign");
                    Response.Redirect("~/Recover.aspx");
                    return;
                }

                var user = Patron.UpdatePasswordByToken(tokenObject.ToString(),
                                                        NPassword.Text);

                if(user == null) {
                    new SessionTools(Session).AlertPatron(GetResourceString("password-recovery-expired"),
                                                          PatronMessageLevels.Warning,
                                                          "exclamation-sign");
                    Response.Redirect("~/Recovery.aspx");
                    return;
                }

                var values = new {
                    SystemName = SRPSettings.GetSettingValue("SysName", user.TenID),
                    ContactName = SRPSettings.GetSettingValue("ContactName", user.TenID),
                    ContactEmail = SRPSettings.GetSettingValue("ContactEmail", user.TenID),
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
                body.Append("<p>The password change has been successful for the {SystemName} account: {Username}.</p>");
                body.Append("<p>You may now <a href=\"{LoginLink}\">log in</a> using your new password.</p>");
                body.Append("<p>If you have any comments or questions, please contact ");
                body.Append("{ContactName} at <a href=\"mailto:{ContactEmail}\">{ContactEmail}</a>.</p>");
                body.Append("<p style=\"font-size: smaller;\"><em>This password request was ");
                body.Append("completed from: {RemoteAddress}.</em></p>");

                new EmailService().SendEmail(user.EmailAddress,
                                             "{SystemName} - {PasswordResetSuccessSubject}".FormatWith(values),
                                             body.ToString().FormatWith(values));


                var st = new SessionTools(Session);
                st.EstablishPatron(user);
                st.AlertPatron(GetResourceString("Your password has been reset!"),
                                                 glyphicon: "ok");
                Response.Redirect("~");
            }
        }
    }
}
