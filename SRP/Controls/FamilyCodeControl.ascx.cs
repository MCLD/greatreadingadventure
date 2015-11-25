using GRA.SRP.Code;
using GRA.SRP.DAL;
using GRA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls {
    public partial class FamilyCodeControl : System.Web.UI.UserControl {
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
                    familyCodeControlPanel.Visible = false;
                    return;
                }
                ViewState["SubmitAsPatronId"] = familyRelationship.PatronId;
            }
        }
        protected void submitButton_Click(object sender, EventArgs e) {

            #region Event Attendance
            // Logging event attendance
            string codeValue = codeEntryField.Text;
            int patronId = (int)ViewState["SubmitAsPatronId"];
            var pointsAward = new AwardPoints(patronId);
            var patron = Patron.FetchObject(patronId);
            var patronName = DisplayHelper.FormatName(patron.FirstName, patron.LastName, patron.Username);

            if(codeValue.Length == 0) {
                Session[SessionKey.PatronMessage] = "Please enter a code.";
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                Session[SessionKey.PatronMessageGlyphicon] = "remove";
                return;
            } else { 
                // verify event code was not previously redeemed
                if(PatronPoints.HasRedeemedKeywordPoints(patronId, codeValue)) {
                    Session[SessionKey.PatronMessage] = string.Format("{0} has already redeemed this code!", patronName);
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

                // don't show badge earnings to the parent
                //if(!string.IsNullOrWhiteSpace(earnedBadges)) {
                //    new SessionTools(Session).EarnedBadges(earnedBadges);
                //}

                // set message and earned badges
                string earnedMessage = new PointCalculation().EarnedMessage(earnedBadges, points, patronName);
                if(string.IsNullOrEmpty(earnedMessage)) {
                    Session[SessionKey.PatronMessage] = string.Format("<strong>Excellent!</strong> Secret code recorded for {0}.", patronName);
                } else {
                    Session[SessionKey.PatronMessage] = string.Format("<strong>Excellent!</strong> Secret code recorded. <strong>{0}</strong>",
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
