using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;

namespace GRA.SRP.Controls {
    public partial class SimpleLoggingControl : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {

                if(Session["Patron"] == null)
                    Response.Redirect("/");
                var patron = (Patron)Session["Patron"];
                lblPID.Text = patron.PID.ToString();
                var prog = Programs.FetchObject(patron.ProgID);
                if(prog == null || !prog.IsOpen) {
                    simpleLoggingControlPanel.Visible = false;
                    return;
                }
                lblPGID.Text = prog.PID.ToString();
                pnlReview.Visible = prog.PatronReviewFlag;

                // Load the Activity Types to log

                foreach(ActivityType val in Enum.GetValues(typeof(ActivityType))) {
                    var pgc = ProgramGamePointConversion.FetchObjectByActivityId(prog.PID, (int)val);
                    if(pgc != null && pgc.PointCount > 0) {
                        rbActivityType.Items.Add(new ListItem(val.ToString(), ((int)val).ToString()));
                    }
                }
                rbActivityType.SelectedIndex = 0;



            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e) {
            var txtCount = txtCountSubmitted.Text.Trim();
            var txtCode = txtProgramCode.Text.Trim();
            // ---------------------------------------------------------------------------------------------------
            if(txtCount.Length > 0 && txtCode.Length > 0) {
                Session[SessionKey.PatronMessage] = "Please enter how much you've read or a code but not both.";
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                Session[SessionKey.PatronMessageGlyphicon] = "remove";
                return;
            }

            if(txtCount.Length == 0 && txtCode.Length == 0) {
                Session[SessionKey.PatronMessage] = "Please enter how much you've read or a code.";
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                Session[SessionKey.PatronMessageGlyphicon] = "remove";
                return;
            }
            // ---------------------------------------------------------------------------------------------------

            int PID = int.Parse(lblPID.Text);
            int PGID = int.Parse(lblPGID.Text);

            var pa = new AwardPoints(PID);
            var sBadges = "";
            var points = 0;
            #region Reading
            // ---------------------------------------------------------------------------------------------------
            // Logging reading ...
            //Badge EarnedBadge;
            if(txtCount.Length > 0) {
                var intCount = 0;
                if(!int.TryParse(txtCount, out intCount) || intCount < 0) {
                    Session[SessionKey.PatronMessage] = "You must enter how much you've read as a positive whole number.";
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                    Session[SessionKey.PatronMessageGlyphicon] = "remove";
                    return;
                }

                int maxAmountForLogging = 0;
                int maxPointsPerDayForLogging = SRPSettings.GetSettingValue("MaxPtsDay").SafeToInt();
                switch(int.Parse(rbActivityType.SelectedValue)) {
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
                                                                      ((ActivityType)int.Parse(rbActivityType.SelectedValue)).ToString());
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                    Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
                    return;
                }

                // convert pages/minutes/etc. to points
                var pc = new ProgramGamePointConversion();
                pc.FetchByActivityId(PGID, int.Parse(rbActivityType.SelectedValue));
                // round up to ensure they get at least 1 point
                decimal computedPoints = intCount * pc.PointCount / pc.ActivityCount;
                points = (int)Math.Ceiling(computedPoints);

                var allPointsToday = PatronPoints.GetTotalPatronPoints(PID, DateTime.Now);
                if(intCount + allPointsToday > maxPointsPerDayForLogging) {
                    Session[SessionKey.PatronMessage] = "Sorry but you have already reached the maximum amount of points that you can log in a day. Keep reading and come back tomorrow!";
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                    Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
                    return;
                }



                sBadges = pa.AwardPointsToPatron(points, PointAwardReason.Reading,
                     0,
                    (ActivityType)pc.ActivityTypeId, intCount, txtAuthor.Text.Trim(), txtTitle.Text.Trim(), Review.Text.Trim());
            }
            #endregion

            #region Event Attendance
            // Logging event attendance
            if(txtCode.Length > 0) {
                // verify event code was not previously redeemed
                if(PatronPoints.HasRedeemedKeywordPoints(PID, txtCode)) {
                    Session[SessionKey.PatronMessage] = "You've already redeemed this code!";
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                    Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
                    return;
                }

                // get event for that code, get the # points
                var ds = Event.GetEventByEventCode(pa.pgm.StartDate.ToShortDateString(),
                                                   DateTime.Now.ToShortDateString(), txtCode);
                if(ds.Tables[0].Rows.Count == 0) {
                    Session[SessionKey.PatronMessage] = "Sorry but that is not a valid code.";
                    Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                    Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";
                    return;
                }
                var EID = (int)ds.Tables[0].Rows[0]["EID"];
                var evt = Event.GetEvent(EID);
                points = evt.NumberPoints;
                //var newPBID = 0;

                sBadges = pa.AwardPointsToPatron(points, PointAwardReason.EventAttendance, eventCode: txtCode, eventID: EID);
                //if (evt.BadgeID != 0)
                //{
                //    sBadges = pa.AwardPointsToPatron(points, PointAwardReason.EventAttendance,
                //                                     eventCode: txtCode, eventID: EID);
                //}
            }
            #endregion

            if(sBadges != "") {
                Session["GoToUrl"] = GoToUrl;
                new SessionTools(Session).EarnedBadges(sBadges);
            }

            Session[SessionKey.PatronMessage] = string.Format("<strong>Good job!</strong> Your reading activity has been logged. <strong>You earned {0} point{1}!</strong>",
                                                              points,
                                                              points > 1 ? "s" : string.Empty);
            Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Success;
            Session[SessionKey.PatronMessageGlyphicon] = "thumbs-up";

            txtAuthor.Text = txtTitle.Text = txtCountSubmitted.Text = Review.Text = txtProgramCode.Text = "";

            if(!StayOnPage) {
                Response.Redirect(GoToUrl);
            }
        }

        protected void btnHistory_Click(object sender, EventArgs e) {
            Session["ActHistPID"] = lblPID.Text;
            Response.Redirect("~/ActivityHistory.aspx");
        }

        public string GoToUrl {
            get {
                if(ViewState["gotourl"] == null || ViewState["gotourl"].ToString().Length == 0) {
                    ViewState["gotourl"] = "~/Dashboard.aspx";
                }
                return ViewState["gotourl"].ToString();
            }
            set { ViewState["gotourl"] = value; }
        }

        public bool StayOnPage {
            get {
                if(ViewState["StayOnPage"] == null) {
                    ViewState["StayOnPage"] = true;// false;
                }
                return (bool)ViewState["StayOnPage"];
            }
            set { ViewState["StayOnPage"] = value; }
        }
    }
}