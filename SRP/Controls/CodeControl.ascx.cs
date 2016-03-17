using GRA.SRP.DAL;
using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls {
    public partial class CodeControl : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if (codeEntryField.CssClass.Contains("code-glow"))
            {
                codeEntryField.CssClass = codeEntryField.CssClass.Replace("code-glow", string.Empty);
            }

            if (!IsPostBack) {
                if(Session["Patron"] == null) {
                    Response.Redirect("~");
                }
                var patron = (Patron)Session[SessionKey.Patron];
                var program = Programs.FetchObject(patron.ProgID);

                if(program == null || !program.IsOpen) {
                    codeControlPanel.Visible = false;
                    return;
                }
                ViewState["PatronId"] = patron.PID.ToString();

                var storedCode = Session[SessionKey.SecretCode];
                if(storedCode != null && !string.IsNullOrWhiteSpace(storedCode.ToString()))
                {
                    codeEntryField.Text = storedCode.ToString();
                    if(!codeEntryField.CssClass.Contains("code-glow"))
                    {
                        codeEntryField.CssClass += " code-glow";
                    }
                
                    Session.Remove(SessionKey.SecretCode);
                    new SessionTools(Session).AlertPatron(
                        StringResources.getString("secret-code-link"),
                        PatronMessageLevels.Info,
                        "barcode");
                }
            }
        }
        protected void submitButton_Click(object sender, EventArgs e) {
            #region Event Attendance
            // Logging event attendance
            string codeValue = Logic.Code.SanitizeCode(codeEntryField.Text);
            codeEntryField.Text = codeValue;
            int patronId = ((Patron)Session[SessionKey.Patron]).PID;
            var pointsAward = new AwardPoints(patronId);

            if(codeValue.Length == 0) {
                new SessionTools(Session).AlertPatron("Please enter a code.",
                    PatronMessageLevels.Danger,
                    "remove");
                return;
            } else { 
                // verify event code was not previously redeemed
                if(PatronPoints.HasRedeemedKeywordPoints(patronId, codeValue)) {
                    new SessionTools(Session).AlertPatron("You've already redeemed this code!",
                        PatronMessageLevels.Warning,
                        "exclamation-sign");
                    return;
                }

                // get event for that code, get the # points
                var ds = Event.GetEventByEventCode(codeValue);
                if(ds.Tables[0].Rows.Count == 0) {
                    new SessionTools(Session).AlertPatron("Sorry, that's an invalid code.",
                        PatronMessageLevels.Warning,
                        "exclamation-sign");
                    return;
                }
                var EID = (int)ds.Tables[0].Rows[0]["EID"];
                var evt = Event.GetEvent(EID);
                var points = evt.NumberPoints;
                //var newPBID = 0;

                var earnedBadges = pointsAward.AwardPointsToPatron(points: points,
                                                                   reason: PointAwardReason.EventAttendance,
                                                                   eventCode: codeValue,
                                                                   eventID: EID);

                if(!string.IsNullOrWhiteSpace(earnedBadges)) {
                    new SessionTools(Session).EarnedBadges(earnedBadges);
                }

                string userMessage = null;
                // set message and earned badges
                string earnedMessage = new PointCalculation().EarnedMessage(earnedBadges, points);
                if(string.IsNullOrEmpty(earnedMessage)) {
                    userMessage = "<strong>Excellent!</strong> Your secret code has been recorded.";
                } else {
                    userMessage = string.Format("<strong>Excellent!</strong> Your secret code has been recorded. <strong>{0}</strong>",
                                                earnedMessage);
                }
                new SessionTools(Session).AlertPatron(userMessage,
                    PatronMessageLevels.Success,
                    "barcode");
                this.codeEntryField.Text = string.Empty;
            }
            #endregion

        }
    }
}
