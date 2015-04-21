using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;

namespace STG.SRP.Classes
{
    public partial class RecoverPassword : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnEmail_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PUsername.Text.Trim()))
            {
                var patron = Patron.GetObjectByUsername(PUsername.Text.Trim());
                // Show message no matter what, even if we can't do it, because of hacking concerns

                if (patron == null || patron.EmailAddress =="")
                {
                    lbMessage.Text = "Your account does not have an email address associated with it or you provided an incorrect email address, so we were unable to email you your password. <br><br> Please visit your local library branch to reset your password.";
                }
                else
                {
                    lbMessage.Text = "Your password has been emailed to the email address associated with your account and should be arriving shortly. <br><br>Please check your email.";

                    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                    var EmailBody =
                        "<h1>Dear " + patron.FirstName + ",</h1><br><br>This is your current account information. Please make sure you reset your password as soon as you are able to log back in.<br><br>" +
                        "Username: " + patron.Username + "<br>Password: " + patron.Password + "<br><br>If you have any questions regarding your account please contact " + SRPSettings.GetSettingValue("ContactName") +
                        " at " + SRPSettings.GetSettingValue("ContactEmail") + "." +
                        "<br><br><br><a href='" + baseUrl + "'>" + baseUrl + "</a> <br> ";

                    EmailService.SendEmail(patron.EmailAddress, "Summer Reading Program - Password recovery", EmailBody);

                }

                Session["PatronLoggedIn"] = false;
                Session["Patron"] = null;
            }
        }
    }
}