using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom
{
    public partial class LoginRecovery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            lblMessage.Text =
                "Your password has been emailed to the address associated with the account and should arrive shortly.";

            SRPUser user = SRPUser.FetchByUsername(uxUsername.Text);
            if (user != null)
            {
                //Send Email;.....
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                var EmailBody =
                    "<h1>Dear "+user.FirstName+",</h1><br><br>This is your current account information. Please make sure you reset your password as soon as you are able to log back in.<br><br>" +
                    "Username: "+ user.Username + "<br>Password: "+user.Password+"<br><br>If you have any questions regarding your account please contact "+SRPSettings.GetSettingValue("ContactName")+
                    " at "+SRPSettings.GetSettingValue("ContactEmail")+"." +
                    "<br><br><br><a href='" + baseUrl + "'>" + baseUrl + "</a> <br> <a href='" + baseUrl + "/ControlRoom'>" + baseUrl + "/ControlRoom</a>";

                EmailService.SendEmail(user.EmailAddress, "Summer Reading Program - Control Room Password recovery", EmailBody);
            }

        }
    }

}