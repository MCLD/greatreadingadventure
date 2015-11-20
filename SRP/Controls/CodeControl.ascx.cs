using GRA.SRP.DAL;
using GRA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls {
    public partial class CodeControl : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
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

            }
        }
        protected void submitButton_Click(object sender, EventArgs e) {

            #region Event Attendance
            // Logging event attendance
            string codeValue = codeEntryField.Text;
            int patronId = ((Patron)Session[SessionKey.Patron]).PID;
            var pointsAward = new AwardPoints(patronId);

            if(codeValue.Length == 0) {
                Session[SessionKey.PatronMessage] = "Please enter a code.";
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                Session[SessionKey.PatronMessageGlyphicon] = "remove";
                return;
            } else { 
                // verify event code was not previously redeemed
                if(PatronPoints.HasRedeemedKeywordPoints(patronId, codeValue)) {
                    Session[SessionKey.PatronMessage] = "You've already redeemed this code!";
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                    Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
                    return;
                }

                // get event for that code, get the # points
                var ds = Event.GetEventByEventCode(pointsAward.pgm.StartDate.ToShortDateString(),
                                                   DateTime.Now.ToShortDateString(), codeValue);
                if(ds.Tables[0].Rows.Count == 0) {
                    Session[SessionKey.PatronMessage] = "Sorry, that's an invalid code.";
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                    Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
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

                // set message and earned badges
                string earnedMessage = new PointCalculation().EarnedMessage(earnedBadges, points);
                if(string.IsNullOrEmpty(earnedMessage)) {
                    Session[SessionKey.PatronMessage] = "<strong>Excellent!</strong> Your secret code has been recorded.";
                } else {
                    Session[SessionKey.PatronMessage] = string.Format("<strong>Excellent!</strong> Your secret code has been recorded. <strong>{0}</strong>",
                                                                      earnedMessage);
                }
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Success;
                Session[SessionKey.PatronMessageGlyphicon] = "barcode";
                this.codeEntryField.Text = string.Empty;
            }
            #endregion

        }
    }
}
