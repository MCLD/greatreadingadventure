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

namespace GRA.SRP.Classes
{
    public partial class RecoverPassword : System.Web.UI.UserControl
    {
        protected void btnEmail_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PUsername.Text.Trim()))
            {
                var patron = Patron.GetObjectByUsername(PUsername.Text.Trim());

                if (patron == null || string.IsNullOrEmpty(patron.EmailAddress))
                {
                    new SessionTools(Session).AlertPatron("Your account could not be located or is not associated with an email address. Please visit your local library branch to reset your password.", PatronMessageLevels.Warning, "exclamation-sign");
                    if (patron != null)
                    {
                        this.Log().Info("Unable to send password recovery email for patron id {0} becuase they don't have an email address configured", patron.PID);
                    }
                    return;
                }
                else {
                    try
                    {
                        string remoteAddress = Request.UserHostAddress;

                        string passwordResetToken = patron.GeneratePasswordResetToken();
                        if (string.IsNullOrEmpty(passwordResetToken))
                        {
                            new SessionTools(Session).AlertPatron("Unable to reset your password. Please visit your local library branch.", PatronMessageLevels.Warning, "exclamation-sign");
                            this.Log().Fatal("Unable to generate password reset token - critical error in password recovery");
                            return;
                        }

                        string systemName = SRPSettings.GetSettingValue("SysName");

                        var values = new
                        {
                            SystemName = systemName,
                            PasswordResetLink = string.Format("{0}{1}?token={2}",
                                                              WebTools.GetBaseUrl(Request),
                                                              "/PasswordRecovery.aspx",
                                                              passwordResetToken),
                            ContactName = SRPSettings.GetSettingValue("ContactName"),
                            ContactEmail = SRPSettings.GetSettingValue("ContactEmail"),
                            RemoteAddress = remoteAddress,
                            UserEmail = patron.EmailAddress,
                            Username = patron.Username,
                            PasswordResetSubject = string.Format("{0} password reset request", systemName)
                        };

                        StringBuilder body = new StringBuilder();
                        body.Append("<p>A password reset request was received by {SystemName} for ");
                        body.Append("your account: {Username}.</p><p>Please ");
                        body.Append("<a href=\"{PasswordResetLink}\">click here</a> ");
                        body.Append("to create a new password for your account.</p>");
                        body.Append("<p>If you did not initiate this request, take no action and your ");
                        body.Append("password will not be changed.</p>");
                        body.Append("<p>If you have any comments or questions, please contact ");
                        body.Append("{ContactName} at ");
                        body.Append("<a href=\"mailto:{ContactEmail}\">{ContactEmail}</a>.</p>");
                        body.Append("<p style=\"font-size: smaller;\"><em>This password request was ");
                        body.Append("submitted from: {RemoteAddress}.</em></p>");

                        new EmailService().SendEmail(patron.EmailAddress,
                                                     "{SystemName} - {PasswordResetSubject}".FormatWith(values),
                                                     body.ToString().FormatWith(values));
                        this.Log().Info("Sent password request email for patron id {0} to {1}",
                            patron.PID, patron.EmailAddress);

                        new SessionTools(Session).AlertPatron("Processing your password reset request, you should receive an email soon.",
                            glyphicon: "ok");
                    }
                    catch (Exception ex)
                    {
                        this.Log().Fatal("Unable to send password recovery email for patron id {0} to {1}: {2} - {3}",
                            patron.PID,
                            patron.EmailAddress,
                            ex.Message,
                            ex.StackTrace);
                        new SessionTools(Session).AlertPatron("A problem occurred resetting your password. Please visit your local library branch to reset your password.",
                            PatronMessageLevels.Warning,
                            "exclamation-sign");
                    }
                }

                new SessionTools(Session).ClearPatron();
                Response.Redirect("~");
            }
        }
    }
}