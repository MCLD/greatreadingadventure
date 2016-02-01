using GRA.SRP.Code;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls {
    public partial class FamilyReadingLogControl : System.Web.UI.UserControl {
        protected bool ShowModal { get; set; }
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {

                var familyRelationship = new FamilyTools().ValidImpersonation(Request, Session);
                if(familyRelationship == null) {
                    Response.Redirect("~");
                }

                ViewState["SubmitAsPatronId"] = familyRelationship.PatronId;

                var patron = Patron.FetchObject(familyRelationship.PatronId);
                var program = Programs.FetchObject(patron.ProgID);

                lblAccount.Text = DisplayHelper.FormatName(patron.FirstName, 
                                                           patron.LastName, 
                                                           patron.Username);

                if(program == null || !program.IsOpen) {
                    familyReadingLogControlPanel.Visible = false;
                    return;
                }

                ViewState["ProgramGameId"] = program.PID.ToString();

                if(Request.Cookies[CookieKey.LogBookDetails] != null) {
                    enterBookDetails.Checked = true;
                }

                foreach(ActivityType activityTypeValue in Enum.GetValues(typeof(ActivityType))) {
                    int activityTypeId = (int)activityTypeValue;
                    string lookupString = string.Format("{0}.{1}.{2}",
                                                        CacheKey.PointGameConversionStub,
                                                        patron.ProgID,
                                                        activityTypeId);
                    var pgc = Cache[lookupString] as ProgramGamePointConversion;
                    if(pgc == null) {
                        this.Log().Debug("Cache miss looking up {0}", lookupString);
                        pgc = ProgramGamePointConversion.FetchObjectByActivityId(patron.ProgID,
                                                                                 activityTypeId);
                        Cache[lookupString] = pgc;
                    }

                    if(pgc != null && pgc.PointCount > 0) {
                        activityTypeSelector.Items.Add(new ListItem(activityTypeValue.ToString(),
                                                                    activityTypeId.ToString()));
                    }
                }

                if(activityTypeSelector.Items.Count == 1) {
                    var singleOption = activityTypeSelector.Items[0];

                    if(int.Parse(singleOption.Value) == (int)ActivityType.Books) {
                        countSubmittedLabel.Visible = false;
                        familyReadingActivityField.Text = "1";
                        familyReadingActivityField.Attributes.Remove("style");
                        familyReadingActivityField.Attributes.Add("style", "display: none;");
                        activityTypeSelector.Attributes.Remove("style");
                        activityTypeSelector.Attributes.Add("style", "display: none;");
                        activityTypeSingleLabel.Visible = false;
                        submitButton.Text = StringResources.getString("readinglog-read-a-book");
                    } else {
                        activityTypeSelector.Attributes.Remove("style");
                        activityTypeSelector.Attributes.Add("style", "display: none;");
                        activityTypeSingleLabel.Text = singleOption.Text;
                        activityTypeSingleLabel.Visible = true;
                    }
                } else {
                    activityTypeSingleLabel.Visible = false;
                }
            }

        }

        protected void SubmitActivity() {
            var txtCount = familyReadingActivityField.Text.Trim();
            var intCount = 0;
            if(txtCount.Length == 0 || !int.TryParse(txtCount, out intCount) || intCount < 0) {
                Session[SessionKey.PatronMessage] = "You must enter how much was read as a positive whole number.";
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                Session[SessionKey.PatronMessageGlyphicon] = "remove";
                return;
            }

            var selectedActivityType = activityTypeSelector.SelectedValue;

            // check that we aren't over the max
            int maxAmountForLogging = 0;
            switch(int.Parse(selectedActivityType)) {
                case 0:
                    maxAmountForLogging = SRPSettings.GetSettingValue("MaxBook").SafeToInt();
                    break;
                case 1:
                    maxAmountForLogging = SRPSettings.GetSettingValue("MaxPage").SafeToInt();
                    break;
                //case 2: maxAmountForLogging = SRPSettings.GetSettingValue("MaxPar").SafeToInt();
                //    break;
                case 3:
                    maxAmountForLogging = SRPSettings.GetSettingValue("MaxMin").SafeToInt();
                    break;
                default:
                    maxAmountForLogging = SRPSettings.GetSettingValue("MaxMin").SafeToInt();
                    break;
            }
            if(intCount > maxAmountForLogging) {
                Session[SessionKey.PatronMessage] = string.Format("That's an awful lot of reading! You can only submit {0} {1} at a time.",
                                                                  maxAmountForLogging,
                                                                  ((ActivityType)int.Parse(selectedActivityType)).ToString());
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
                return;
            }

            var patronId = int.Parse(ViewState["SubmitAsPatronId"].ToString());
            var programGameId = int.Parse(ViewState["ProgramGameId"].ToString());
            var patron = Patron.FetchObject(patronId);
            var patronName = DisplayHelper.FormatName(patron.FirstName, patron.LastName, patron.Username);
            var pa = new AwardPoints(patronId);
            var points = 0;

            // convert pages/minutes/etc. to points
            var pc = new ProgramGamePointConversion();
            pc.FetchByActivityId(programGameId, int.Parse(activityTypeSelector.SelectedValue));
            // round up to ensure they get at least 1 point
            decimal computedPoints = intCount * pc.PointCount / pc.ActivityCount;
            points = (int)Math.Ceiling(computedPoints);

            // ensure they aren't over teh day total
            var allPointsToday = PatronPoints.GetTotalPatronPoints(patronId, DateTime.Now);
            int maxPointsPerDayForLogging = SRPSettings.GetSettingValue("MaxPtsDay").SafeToInt();
            if(intCount + allPointsToday > maxPointsPerDayForLogging) {
                Session[SessionKey.PatronMessage] = string.Format("{0} has already reached the maximum amount of points that can be logged in a day. Keep reading and come back tomorrow!", patronName);
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
                return;
            }

            var earnedBadges = pa.AwardPointsToPatron(points: points,
                reason: PointAwardReason.Reading,
                MGID: 0,
                readingActivity: (ActivityType)pc.ActivityTypeId,
                readingAmount: intCount,
                author: authorField.Text,
                title: titleField.Text);

            // clear out the form
            var bookButton = activityTypeSelector.Items.Count == 1
                             && int.Parse(activityTypeSelector.Items[0].Value) == (int)ActivityType.Books;

            if(!bookButton) { 
                familyReadingActivityField.Text = string.Empty;
            }
            authorField.Text = string.Empty;
            titleField.Text = string.Empty;

            // set message and earned badges
            string earnedMessage = new PointCalculation().EarnedMessage(earnedBadges, points, patronName);
            if(string.IsNullOrEmpty(earnedMessage)) {
                Session[SessionKey.PatronMessage] = string.Format("Reading activity has been logged for {0}.",
                                                                  patronName);
            } else {
                Session[SessionKey.PatronMessage] = string.Format("Reading activity has been logged. <strong>{0}</strong>",
                                                                  earnedMessage);
            }
            Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Success;
            Session[SessionKey.PatronMessageGlyphicon] = "thumbs-up";
            // don't show earned badges when entering points for someone else
            //new SessionTools(Session).EarnedBadges(earnedBadges);
        }


        protected void submitButton_Click(object sender, EventArgs e) {
            if(enterBookDetails.Checked) {
                // show pop-up
                HttpCookie logDetailsCookie = new HttpCookie(CookieKey.LogBookDetails);
                logDetailsCookie.Expires = DateTime.Now.AddDays(14);
                logDetailsCookie.Value = "y";
                Response.SetCookie(logDetailsCookie);
                familyReadingLogPopup.Visible = true;
                this.ShowModal = true;
            } else {
                // log activity
                if(Request.Cookies[CookieKey.LogBookDetails] != null) {
                   Response.Cookies[CookieKey.LogBookDetails].Expires = DateTime.Now.AddDays(-1);
                }
                authorField.Text = string.Empty;
                titleField.Text = string.Empty;
                SubmitActivity();
            }
        }

        protected void cancelButton_Click(object sender, EventArgs e) {
            // hide popup
            familyReadingLogPopup.Visible = false;
            authorField.Text = string.Empty;
            titleField.Text = string.Empty;
        }
        protected void submitDetailsButton_Click(object sender, EventArgs e) {
            // log activity
            SubmitActivity();
        }
    }
}