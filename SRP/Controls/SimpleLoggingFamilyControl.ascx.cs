using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.Controls
{
    public partial class SimpleLoggingFamilyControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {



                if (string.IsNullOrEmpty(Request["SA"]) && (Session["SA"] == null || Session["SA"].ToString() == ""))
                {
                    Response.Redirect("~/FamilyAccountList.aspx");
                }
                if (!string.IsNullOrEmpty(Request["SA"]))
                {
                    lblPID.Text = Request["SA"];
                    Session["SA"] = lblPID.Text;
                }
                else
                {
                    lblPID.Text = Session["SA"].ToString();
                }

                //var parent = (Patron)Session["Patron"];           
                var parent = Patron.FetchObject((int)Session["MasterAcctPID"]);

                lblParentPID.Text = parent.PID.ToString();

                // now validate user can change manage log for SA Sub Account
                if (!parent.IsMasterAccount)
                {
                    // kick them out
                    Response.Redirect("~/Logout.aspx");
                }

                if (!Patron.CanManageSubAccount(parent.PID, int.Parse(lblPID.Text)))
                {
                    // kick them out
                    Response.Redirect("~/Logout.aspx");
                }


                var patron = Patron.FetchObject(int.Parse(lblPID.Text));
                var prog = Programs.FetchObject(patron.ProgID);
                if (prog == null)
                {
                    var progID = Programs.GetDefaultProgramForAgeAndGrade(patron.Age, patron.SchoolGrade.SafeToInt());
                    prog = Programs.FetchObject(progID);
                    patron.ProgID = progID;
                    patron.Update();
                }

                lblPGID.Text = prog.PID.ToString();
                pnlReview.Visible = prog.PatronReviewFlag;

                lblAccount.Text = (patron.FirstName + " " + patron.LastName).Trim();
                if (lblAccount.Text.Length == 0) lblAccount.Text = patron.Username;


                // Load the Acticity Types to log
                foreach (ActivityType val in Enum.GetValues(typeof(ActivityType)))
                {
                    var pgc = ProgramGamePointConversion.FetchObjectByActivityId(prog.PID, (int) val);
                    if (pgc != null && pgc.PointCount > 0)
                    {
                        rbActivityType.Items.Add(new ListItem(val.ToString(), ((int) val).ToString()));
                    }
                }
                rbActivityType.SelectedIndex = 0;
            
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            var txtCount = txtCountSubmitted.Text.Trim();
            var txtCode = txtProgramCode.Text.Trim();
            // ---------------------------------------------------------------------------------------------------
            if (txtCount.Length > 0 && txtCode.Length > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please enter either how much you have read OR a code, but not both.<br><br>";
                return;
            }

            if (txtCount.Length == 0 && txtCode.Length == 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please enter either how much you have read OR a code.<br><br>";
                return;
            }
            // ---------------------------------------------------------------------------------------------------

            int PID = int.Parse(lblPID.Text);
            int PGID = int.Parse(lblPGID.Text);
            var StartingPoints = PatronPoints.GetTotalPatronPoints(PID);


            var pa = new AwardPoints(PID);
            var sBadges = "";
            
            #region Reading
            // ---------------------------------------------------------------------------------------------------
            // Logging reading ...
            
            //Badge EarnedBadge;
            if (txtCount.Length > 0)
            {
                var intCount = 0;
                if (!int.TryParse(txtCount, out intCount))
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "How much was read must be a number.";
                    return;
                }

                if (intCount < 0)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "Hmmm, you must enter a positive number...<br><br>";
                    return;
                }

                int maxAmountForLogging = 0;
                int maxPointsPerDayForLogging = SRPSettings.GetSettingValue("MaxPtsDay").SafeToInt();
                switch (int.Parse(rbActivityType.SelectedValue))
                {
                    case 0: maxAmountForLogging = SRPSettings.GetSettingValue("MaxBook").SafeToInt();
                        break;
                    case 1: maxAmountForLogging = SRPSettings.GetSettingValue("MaxPage").SafeToInt();
                        break;
                    //case 2: maxAmountForLogging = SRPSettings.GetSettingValue("MaxPar").SafeToInt();
                    //    break;
                    case 3: maxAmountForLogging = SRPSettings.GetSettingValue("MaxMin").SafeToInt();
                        break;
                    default: maxAmountForLogging = SRPSettings.GetSettingValue("MaxMin").SafeToInt();
                        break;
                }

                if (intCount > maxAmountForLogging)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = string.Format("That is an awful lot of reading... unfortunately the maximum you can submit at one time is {0} {1}.<br><br>",
                        maxAmountForLogging, ((ActivityType)int.Parse(rbActivityType.SelectedValue)).ToString());
                    return;
                }

                // convert pages/minutes/etc. to points
                var pc = new ProgramGamePointConversion();
                pc.FetchByActivityId(PGID, int.Parse(rbActivityType.SelectedValue));
                var points = Convert.ToInt32(intCount * pc.PointCount / pc.ActivityCount);

                var allPointsToday = PatronPoints.GetTotalPatronPoints(PID, DateTime.Now);
                if (intCount + allPointsToday > maxPointsPerDayForLogging)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = string.Format("We are sorry, you have reached the maximum amount of points you are allowed to log in a single day, regardless of how the points were earned. Please come back and and log them tomorrow.<br><br>");
                    return;
                }


                //// convert pages/minutes/etc. to points
                //var pc = new ProgramGamePointConversion();
                //pc.FetchByActivityId(PGID, int.Parse(rbActivityType.SelectedValue));
                //var points = Convert.ToInt32(intCount * pc.PointCount / pc.ActivityCount);

                sBadges = pa.AwardPointsToPatron(points, PointAwardReason.Reading,
                     0,
                    (ActivityType)pc.ActivityTypeId, intCount, txtAuthor.Text.Trim(), txtTitle.Text.Trim(), Review.Text.Trim());
            }
            #endregion

            #region Event Attendance
            // Logging event attendance
            if (txtCode.Length > 0)
            {
                // verify event code was not previously redeemed
                if (PatronPoints.HasRedeemedKeywordPoints(PID, txtCode))
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "This code has already been redeemend for this account.";
                    return;
                }

                // get event for that code, get the # points
                var ds = Event.GetEventByEventCode(pa.pgm.StartDate.ToShortDateString(),
                                                   DateTime.Now.ToShortDateString(), txtCode);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "This code is not valid.";
                    return;
                }
                var EID = (int) ds.Tables[0].Rows[0]["EID"];
                var evt = Event.GetEvent(EID);
                var points = evt.NumberPoints;
                //var newPBID = 0;

                sBadges = pa.AwardPointsToPatron(points, PointAwardReason.EventAttendance, eventCode: txtCode, eventID: EID);
                //if (evt.BadgeID != 0)
                //{
                //    sBadges = pa.AwardPointsToPatron(points, PointAwardReason.EventAttendance,
                //                                     eventCode: txtCode, eventID: EID);
                //}
            }
            #endregion

            var EndingPoints = PatronPoints.GetTotalPatronPoints(PID);
            
            // No need to announcve the badge award
            
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = (EndingPoints - StartingPoints).ToInt() + @" points have been added to the account!";

            txtAuthor.Text = txtTitle.Text = txtCountSubmitted.Text = Review.Text = txtProgramCode.Text = "";
            btnSubmit.Visible = false;
            btnReSubmit.Visible = true;
            EntryTable.Visible = false;
        }


        protected void btnReSubmit_Click(object sender, EventArgs e)
        {
            btnSubmit.Visible = true;
            btnReSubmit.Visible = false;
            EntryTable.Visible = true;
            lblMessage.Text = "";
        }

        //protected void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    int PID = int.Parse(lblPID.Text);
        //    int PGID = int.Parse(lblPGID.Text);

        //    var txtCount = txtCountSubmitted.Text.Trim();
        //    var txtCode = txtProgramCode.Text.Trim();

        //    var EarnedBadges = new List<Badge>();
        //    var patron = Patron.FetchObject(PID);
        //    var pgm = Programs.FetchObject(PGID);
        //    var StartingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);
        //    var EndingPoints = StartingPoints;
        //    // ---------------------------------------------------------------------------------------------------
        //    if (txtCount.Length > 0 && txtCode.Length > 0)
        //    {
        //        lblMessage.ForeColor = System.Drawing.Color.Red;
        //        lblMessage.Text = "Please enter either how much was read OR a code, but not both.";
        //        return;
        //    }

        //    if (txtCount.Length == 0 && txtCode.Length == 0)
        //    {
        //        lblMessage.ForeColor = System.Drawing.Color.Red;
        //        lblMessage.Text = "Please enter either how much was read OR a code.";
        //        return;
        //    }

        //    var now = DateTime.Now;

        //    #region Reading
        //    // ---------------------------------------------------------------------------------------------------
        //    // Logging reading ...
        //    Badge EarnedBadge;
        //    if (txtCount.Length > 0)
        //    {
        //        var intCount = 0;
        //        if (!int.TryParse(txtCount, out intCount))
        //        {
        //            lblMessage.ForeColor = System.Drawing.Color.Red;
        //            lblMessage.Text = "How much was read must be a number.";
        //            return;
        //        }

        //        // convert pages/minutes/etc. to points
        //        var pc = new ProgramGamePointConversion();
        //        pc.FetchByActivityId(PGID, int.Parse(rbActivityType.SelectedValue));
        //        var points = Convert.ToInt32(intCount*pc.PointCount/pc.ActivityCount);

        //        // record in the LOG
        //        var rl = new PatronReadingLog();
        //        rl.PID = PID;
        //        rl.ReadingType = pc.ActivityTypeId;
        //        rl.ReadingTypeLabel = ((ActivityType) pc.ActivityTypeId).ToString();
        //        rl.ReadingAmount = intCount;
        //        rl.ReadingPoints = points;
        //        rl.LoggingDate = FormatHelper.ToNormalDate(now);
        //        rl.Author = txtAuthor.Text.Trim();
        //        rl.Title = txtAuthor.Text.Trim();
        //        rl.HasReview = (pnlReview.Visible && Review.Text.Trim().Length > 0);
        //        rl.ReviewID = 0;
        //        rl.Insert();

        //        // If there is a review, record the review
        //        if (rl.HasReview)
        //        {
        //            var r = new PatronReview
        //                        {
        //                            PID = PID,
        //                            PRLID = rl.PRLID,
        //                            Author = rl.Author,
        //                            Title = rl.Title,
        //                            Review = Review.Text.Trim(),
        //                            isApproved = false
        //                        };
        //            r.Insert();

        //            rl.ReviewID = r.PRID;
        //            rl.Update();
        //        }

        //        // Also cummulate the earned points to the points ledger
        //        var pp = new PatronPoints
        //                     {
        //                         PID = PID,
        //                         NumPoints = points,
        //                         AwardDate = now,
        //                         AwardReasonCd = (int) PointAwardReason.Reading,
        //                         AwardReason = PatronPoints.PointAwardReasonCdToDescription(PointAwardReason.Reading),
        //                         BadgeAwardedFlag = false,
        //                         isBookList = false,
        //                         isEvent = false,
        //                         isGame = false,
        //                         isGameLevelActivity = false,
        //                         isReading = true,
        //                         LogID = rl.PRLID
        //                     };
        //        pp.Insert();

        //        // based on the new total points, figure out if they earned a badge
        //        EndingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);
        //        EarnedBadge = TallyPoints(patron, pgm, StartingPoints, EndingPoints, ref EarnedBadges);
        //        if (EarnedBadge != null)
        //        {
        //            // Award the badge
        //            var newPBID = 0;
        //            var pb = new PatronBadges { BadgeID = EarnedBadge.BID, DateEarned = now, PID = PID };
        //            pb.Insert();
        //            newPBID = pb.PBID;

        //            // update the pointe ledger with the earned badge
        //            pp.BadgeAwardedFlag = true;
        //            pp.BadgeID = EarnedBadge.BID;
        //            pp.Update();

        //            //if badge generates notification, then generate the notification
        //            if (EarnedBadge.GenNotificationFlag)
        //            {
        //                var not = new Notifications
        //                {
        //                    PID_To = PID,
        //                    PID_From = 0,  //0 == System Notification
        //                    Subject = EarnedBadge.NotificationSubject,
        //                    Body = EarnedBadge.NotificationBody,
        //                    isQuestion = false,
        //                    AddedDate = now,
        //                    LastModDate = now,
        //                    AddedUser = patron.Username,
        //                    LastModUser = "N/A"
        //                };
        //                not.Insert();
        //            }

        //        }
        //    }
        //    #endregion

        //    #region Event Attendance
        //    // Logging event attendance
        //    if (txtCode.Length > 0)
        //    {
        //        // verify event code was not previously redeemed
        //        if (PatronPoints.HasRedeemedKeywordPoints(PID, txtCode))
        //        {
        //            lblMessage.ForeColor = System.Drawing.Color.Red;
        //            lblMessage.Text = "This code has already been redeemend for this account.";
        //            return;                    
        //        }

        //        // get event for that code, get the # points
        //        var ds = Event.GetEventByEventCode(pgm.StartDate.ToShortDateString(), now.ToShortDateString(), txtCode);
        //        if (ds.Tables[0].Rows.Count == 0)
        //        {
        //            lblMessage.ForeColor = System.Drawing.Color.Red;
        //            lblMessage.Text = "This code is not valid.";
        //            return;                     
        //        }
        //        var EID = (int)ds.Tables[0].Rows[0]["EID"];
        //        var evt = Event.GetEvent(EID);
        //        var points = evt.NumberPoints;
        //        var newPBID = 0;
        //        if (evt.BadgeID != 0)
        //        {
        //            // if the event awards a badge, then record the badge to the patron badge list
        //            var pb = new PatronBadges { BadgeID = evt.BadgeID, DateEarned = now, PID = PID };
        //            pb.Insert();
        //            newPBID = pb.PBID;

        //            // remember the awarded badge as to notify later
        //            EarnedBadge = null;
        //            EarnedBadge = Badge.GetBadge(evt.BadgeID);
        //            EarnedBadges.Add(EarnedBadge);

        //            //if badge generates notification, then generate the notification
        //            if (EarnedBadge.GenNotificationFlag)
        //            {
        //                var not = new Notifications
        //                              {
        //                                  PID_To = PID,
        //                                  PID_From = 0,  //0 == System Notification
        //                                  Subject = EarnedBadge.NotificationSubject,
        //                                  Body = EarnedBadge.NotificationBody,
        //                                  isQuestion = false,
        //                                  AddedDate = now,
        //                                  LastModDate = now,
        //                                  AddedUser = patron.Username,
        //                                  LastModUser = "N/A"
        //                              };
        //                not.Insert();
        //            }
        //        }

        //        // Record the points to the points ledger
        //        var pp = new PatronPoints
        //        {
        //            PID = PID,
        //            NumPoints = points,
        //            AwardDate = now,
        //            AwardReasonCd = (int)PointAwardReason.EventAttendance,
        //            AwardReason = PatronPoints.PointAwardReasonCdToDescription(PointAwardReason.EventAttendance),
                    
        //            isBookList = false,
        //            isGame = false,
        //            isGameLevelActivity = false,
        //            isReading = false,
        //            isEvent = true,
        //            EventCode = txtCode,
        //            EventID = evt.EID,
        //            BadgeAwardedFlag = (evt.BadgeID != 0),
        //            BadgeID = evt.BadgeID,
        //            PBID = newPBID
        //        };
        //        pp.Insert();

        //    }
        //    #endregion

        //    EndingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);
        //    EarnedBadge = null;
        //    EarnedBadge = TallyPoints(patron, pgm, StartingPoints, EndingPoints, ref EarnedBadges);
        //    if (EarnedBadge != null)
        //    {
        //        // Award the badge
        //        var newPBID = 0;
        //        var pb = new PatronBadges { BadgeID = EarnedBadge.BID, DateEarned = now, PID = PID };
        //        pb.Insert();
        //        newPBID = pb.PBID;
                
        //        //if badge generates notification, then generate the notification
        //        if (EarnedBadge.GenNotificationFlag)
        //        {
        //            var not = new Notifications
        //            {
        //                PID_To = PID,
        //                PID_From = 0,  //0 == System Notification
        //                Subject = EarnedBadge.NotificationSubject,
        //                Body = EarnedBadge.NotificationBody,
        //                isQuestion = false,
        //                AddedDate = now,
        //                LastModDate = now,
        //                AddedUser = patron.Username,
        //                LastModUser = "N/A"
        //            };
        //            not.Insert();
        //        }

        //    }
            
        //    // No need to announcve the badge award
        //    //if (EarnedBadges.Count > 0)
        //    //{
        //    //    //Display Badges Awards messages
        //    //    var badges = EarnedBadges.Count.ToString();
        //    //    //foreach(Badge b in EarnedBadges)
        //    //    //{
        //    //    //    badges = badges + "|" + b.BID.ToString();
        //    //    //}
        //    //    badges = EarnedBadges.Aggregate(badges, (current, b) => current + "|" + b.BID.ToString());
        //    //    //Server.Transfer("~/BadgeAward.aspx?b=" + badges);
        //    //    Response.Redirect("~/BadgeAward.aspx?b=" + badges);

        //    //}

        //    lblMessage.ForeColor = System.Drawing.Color.Green;
        //    lblMessage.Text = FormatHelper.ToInt(EndingPoints - StartingPoints) + " points have been added to the account!";
        //}

        //public Badge TallyPoints(Patron patron, Programs pgm, int StartingPoints, int EndingPoints, ref List<Badge> EarnedBadges)
        //{
        //    Badge b = null;
        //    //Tally up the points and figure out if we need to award a badge.  
        //    if (pgm.ProgramGameID > 0)
        //    {
        //        // only if we have a game we can earn badges by reading ....
        //        var gm = ProgramGame.FetchObject(pgm.ProgramGameID);
        //        var ds = ProgramGameLevel.GetAll(gm.PGID);

        //        var normalLevelTotalPoints = GetGameCompletionPoints(ds);
        //        var bonusLevelTotalPoints = GetGameCompletionBonusPoints(ds, gm.BonusLevelPointMultiplier);

        //        var bonus = (StartingPoints > normalLevelTotalPoints);
        //        int BeforeLevel = 0, AfterLevel = 0;

        //        // loop thru the levels to see where we are at ... before awarding the new points
        //        var rp = StartingPoints;   //remaining points
        //        if (bonus)
        //        {
        //            // if we are on the bonus, eliminate the "fully completed boards/levels) and then see what the remainder of the points is.
        //            rp = rp - normalLevelTotalPoints;
        //            rp = rp % bonusLevelTotalPoints;
        //        }

        //        for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            var multiplier = (bonus ? gm.BonusLevelPointMultiplier : 1.00m);
        //            var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
        //            rp = rp - levelPoints;
        //            if (rp < 0)
        //            {
        //                BeforeLevel = i;
        //                break;
        //            }
        //        }


        //        // loop thru the levels to see where we are at ... AFTER awarding the new points
        //        rp = EndingPoints;   //remaining points
        //        if (bonus)
        //        {
        //            // if we are on the bonus, eliminate the "fully completed boards/levels) and then see what the remainder of the points is.
        //            rp = rp - normalLevelTotalPoints;
        //            rp = rp % bonusLevelTotalPoints;
        //        }
        //        for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            var multiplier = (bonus ? gm.BonusLevelPointMultiplier : 1.00m);
        //            var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
        //            rp = rp - levelPoints;
        //            if (rp < 0)
        //            {
        //                AfterLevel = i;
        //                break;
        //            }
        //        }

        //        if (BeforeLevel != AfterLevel)
        //        {
        //            // completed the "beforeLevel" and moved up to the "AfterLevel" , so check if we need to award a badge

        //            var BadgeToAward = Convert.ToInt32(ds.Tables[0].Rows[BeforeLevel]["AwardBadgeID"]);
        //            if (BadgeToAward > 0)
        //            {
        //                b = Badge.GetBadge(BadgeToAward);
        //                EarnedBadges.Add(b);
        //            }
        //        }
        //    }
        //    return b;
        //}

        //public int GetGameCompletionPoints(DataSet ds)
        //{
        //    var ret = 0;
        //    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        ret = ret + Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]));
        //    }
        //    return ret;
        //}

        //public int GetGameCompletionBonusPoints(DataSet ds, decimal bonusLevelPointMultiplier)
        //{
        //    var ret = 0;
        //    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        var multiplier = bonusLevelPointMultiplier;
        //        var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
        //        ret = ret + levelPoints;
        //    }
        //    return ret;
        //}

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/FamilyAccountList.aspx");
        }

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            Session["ActHistPID"] = lblPID.Text;
            Response.Redirect("~/Account/ActivityHistory.aspx");
        }

    }
}