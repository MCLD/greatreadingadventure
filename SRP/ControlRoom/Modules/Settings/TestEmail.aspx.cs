using GRA.Communications;
using GRA.SRP.Core.Utilities;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Modules.Settings
{
    public partial class TestEmail : BaseControlRoomPage
    {
        private string GetSettingValue(string settingName)
        {
            return DAL.SRPSettings.GetSettingValue(settingName);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            AlertPanel.Visible = false;

            if (!IsPostBack)
            {
                MasterPage.IsSecure = true;
                MasterPage.PageTitle = "Test Email";
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());

                this.EmailSubject.Text = string.Format("Test email from {0}",
                    GetSettingValue("SysName"));
                this.EmailTo.Text = GetSettingValue("FromEmailAddress");
                if (string.IsNullOrEmpty(this.EmailTo.Text))
                {
                    this.EmailTo.Text = ConfigurationManager.AppSettings[AppSettingKeys.DefaultEmailFrom.ToString()];
                }
            }
        }

        protected void SendTestEmail_Click(object sender, EventArgs e)
        {
            string body = string.Format("<p>This is a test email from {0} sent at {1}.</p>",
                GetSettingValue("SysName"),
                DateTime.Now.ToString());

            var emailService = new EmailService();
            emailService.SendEmail(this.EmailTo.Text, this.EmailSubject.Text, body);
            if (emailService.ErrorException == null)
            {
                AlertPanel.Visible = true;
                AlertPanel.CssClass = "alert alert-success";
                AlertGlyphicon.Attributes.Add("class", "glyphicon glyphicon-ok");
                AlertMessage.Text = string.Format("Message successfully sent to {0}.",
                    this.EmailTo.Text);
            }
            else
            {
                AlertPanel.Visible = true;
                AlertPanel.CssClass = "alert alert-danger";
                AlertGlyphicon.Attributes.Add("class", "glyphicon glyphicon-remove");
                var sb = new StringBuilder("An error occurred sending the test email: <strong>");
                sb.Append(emailService.ErrorException.Message);
                sb.Append("</strong><p>Detailed error:</p><pre>");
                var ex = emailService.ErrorException;
                while (ex != null)
                {
                    sb.Append(ex.StackTrace);
                    ex = ex.InnerException;
                }
                sb.Append("</pre>");
                AlertMessage.Text = sb.ToString();
            }
        }
    }
}