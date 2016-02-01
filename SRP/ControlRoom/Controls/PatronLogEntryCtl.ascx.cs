using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Controls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using Minigame = GRA.SRP.DAL.Minigame;

namespace GRA.SRP.ControlRoom.Controls
{
    public partial class PatronLogEntryCtl : System.Web.UI.UserControl
    {

        public string PatronID { get { return PID.Text; } set { PID.Text = value; } }
        public string PatronPointsID { get { return PPID.Text; } set { PPID.Text = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void LoadControl()
        {
            if (PatronPointsID != "")
            {
                // Update:
                var o = PatronPoints.FetchObject(int.Parse(PatronPointsID));

                AwardReasonCd.SelectedValue = o.AwardReasonCd.ToString();
                AwardDate.Text = FormatHelper.ToNormalDate(o.AwardDate);
                NumPoints.Text = FormatHelper.ToInt(o.NumPoints);

                if (o.BadgeAwardedFlag)
                {
                    pnlEditOnly.Visible = true;
                    BadgeAwarded.Text = DAL.Badge.GetBadge(o.BadgeID).AdminName;
                }
                EventCode.Text = o.EventCode;

                if (o.isBookList) pnlBook.Visible = true;
                if (o.isEvent) pnlEvent.Visible = true;
                if (o.isGame)
                {
                    pnlGame.Visible = true;
                    
                }
                if (o.isGameLevelActivity)
                {
                    pnlMini.Visible = true;
                    var mg = Minigame.FetchObject(o.GameLevelActivityID);
                    if (mg!=null) lblMGame.Text = mg.AdminName;
                }

            }
        }

        protected void AwardReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlBook.Visible = false;
            pnlEvent.Visible = false;
            pnlGame.Visible = false;
            pnlMini.Visible = false;

            switch (AwardReasonCd.SelectedValue)
            {
                case "0" :
                    break;
                case "1": pnlEvent.Visible = true; break;
                case "2": pnlBook.Visible = true; break;
                case "3": pnlGame.Visible = true; break;
                case "4": pnlMini.Visible = true; break;
            }


        }

        protected void btnBack_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("PatronLog.aspx");
        }

        protected void btnRefresh_Click(object sender, ImageClickEventArgs e)
        {
            LoadControl();
            var masterPage = (IControlRoomMaster)((BaseControlRoomPage)Page).Master; 
            if (masterPage != null) masterPage.PageMessage = SRPResources.RefreshOK;
        }

        public bool IsAdd()
        {
            return (PatronPointsID == "");
        }

        protected void ImageButton1_Command(object sender, CommandEventArgs e)
        {
            var masterPage = (IControlRoomMaster)((BaseControlRoomPage)Page).Master;
            lblError.Text= string.Empty;
            Page.Validate();
            if (Page.IsValid)
            {
                var patron = Patron.FetchObject(int.Parse(PID.Text));
                var pgm = Programs.FetchObject(patron.ProgID);

                
                if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
                {
                    //var p = new Patron();
                    var o = new PatronPoints();
                    var sBadges= string.Empty;
                    var pa = new AwardPoints(patron.PID);
                    var points = int.Parse(NumPoints.Text);

                    if (!IsAdd())
                    {
                        //p = Patron.FetchObject(int.Parse(PID.Text));
                        o = PatronPoints.FetchObject(int.Parse(PatronPointsID));
                        o.Delete();

                        var pl = PatronReadingLog.FetchObject(o.LogID);
                        if (pl != null && pl.HasReview)
                        {
                            var r = PatronReview.FetchObjectByLogId(o.LogID);
                            if (r != null) r.Delete();     
                        }
                        if (pl != null) pl.Delete();
                    }

                    //o.PID = patron.PID;
                    //o.NumPoints = int.Parse(NumPoints.Text);
                    //o.AwardDate = DateTime.Parse(AwardDate.Text);
                    //o.AwardReasonCd = int.Parse(AwardReasonCd.SelectedValue);
                    //o.AwardReason = PatronPoints.PointAwardReasonCdToDescription((PointAwardReason)o.AwardReasonCd);
                    //o.isReading = o.isBookList = o.isEvent = o.isGame = o.isGameLevelActivity = false;
                    if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.Reading)
                    {
                        var pc = new ProgramGamePointConversion();
                        pc.FetchByActivityId(pgm.PID, 1);//ActivityType.Pages);
                        var pages = 1; 
                        try { Convert.ToInt32(pc.ActivityCount * points / pc.PointCount);} catch{}
                        sBadges = pa.AwardPointsToPatron(points, PointAwardReason.Reading,
                                                            0,
                                                            ActivityType.Pages, pages, "", "", "", forceDate: DateTime.Parse(AwardDate.Text));
                    }


                    if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.EventAttendance)
                    {
                        var ds = Event.GetEventByEventCode(pa.pgm.StartDate.ToShortDateString(),
                                                   DateTime.Now.ToShortDateString(), EventCode.Text);
                        var EID = 0;
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            EID = 0;
                        }
                        else
                        {
                            EID = (int)ds.Tables[0].Rows[0]["EID"];
                        }
                        sBadges = pa.AwardPointsToPatron(points, PointAwardReason.EventAttendance,
                                                     eventCode: EventCode.Text, eventID: EID, forceDate: DateTime.Parse(AwardDate.Text));
                    }
                    if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.BookListCompletion)
                    {
                        var BLID = 0;
                        int.TryParse(lblBookList.Text, out BLID);
                        sBadges = pa.AwardPointsToPatron(points, PointAwardReason.BookListCompletion,
                                                            bookListID: BLID, forceDate: DateTime.Parse(AwardDate.Text));
                        
                    }
                    if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.GameCompletion)
                    {
                        var GID = 0;
                        int.TryParse(lblGame.Text, out GID);
                        sBadges = pa.AwardPointsToPatron(points, PointAwardReason.GameCompletion, GID, forceDate: DateTime.Parse(AwardDate.Text));                        
                    }
                    if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.MiniGameCompletion)
                    {
                        var MGID = 0;
                        int.TryParse(lblMGame.Text, out MGID);
                        sBadges = pa.AwardPointsToPatron(points, PointAwardReason.MiniGameCompletion, MGID, forceDate: DateTime.Parse(AwardDate.Text));
                    }

                    PatronPointsID = PatronPoints.GetLastPatronEntryID(int.Parse(PatronID)).ToString();
                    
                    LoadControl();
                    masterPage.PageMessage = SRPResources.AddedOK;

                    if (e.CommandName.ToLower() == "saveandback")
                    {
                        Response.Redirect("PatronLog.aspx");
                    }

                }

            }
        }


        //protected void ImageButton1_Command(object sender, CommandEventArgs e)
        //{
        //    var masterPage = (IControlRoomMaster)((BaseControlRoomPage)Page).Master; 
        //    lblError.Text= string.Empty;
        //    Page.Validate();
        //    if (Page.IsValid)
        //    {
        //        var patron = Patron.FetchObject(int.Parse(PID.Text));
        //        var pgm = Programs.FetchObject(patron.ProgID);
        //        var StartingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);

        //        if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
        //        {
        //            //var p = new Patron();
        //            var o = new PatronPoints();
        //            var oo = new PatronPoints();
        //            if (!IsAdd())
        //            {
        //                //p = Patron.FetchObject(int.Parse(PID.Text));
        //                o = PatronPoints.FetchObject(int.Parse(PatronPointsID));
        //                oo = PatronPoints.FetchObject(int.Parse(PatronPointsID));
        //            }

        //            o.PID = patron.PID;
        //            o.NumPoints = int.Parse(NumPoints.Text);
        //            o.AwardDate = DateTime.Parse(AwardDate.Text);
        //            o.AwardReasonCd = int.Parse(AwardReasonCd.SelectedValue);
        //            o.AwardReason = PatronPoints.PointAwardReasonCdToDescription((PointAwardReason)o.AwardReasonCd);
        //            o.isReading = o.isBookList = o.isEvent = o.isGame = o.isGameLevelActivity = false;
        //            if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.Reading) o.isReading = true;
        //            if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.BookListCompletion) o.isBookList = true;
        //            if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.EventAttendance) o.isEvent = true;
        //            if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.GameCompletion) o.isGame = true;
        //            if ((PointAwardReason)o.AwardReasonCd == PointAwardReason.MiniGameCompletion) o.isGameLevelActivity = true;
        //            o.EventCode = EventCode.Text.Trim();

        //            if (o.AwardReasonCd != oo.AwardReasonCd && !IsAdd())
        //            {
        //                // we chnaged the type of entry ... so clear the associated entries.
        //                if (oo.isReading)
        //                {
        //                    //o.LogID = 0;

        //                    var pl = PatronReadingLog.FetchObject(oo.LogID);
        //                    if (pl != null && pl.HasReview)
        //                    {
        //                        var r = PatronReview.FetchObjectByLogId(oo.LogID);
        //                        if (r!=null) r.Delete();
        //                        pl.Delete();
        //                    }
                            
        //                }
        //                if (oo.isEvent) o.EventID = 0;
        //                if (oo.isBookList) o.BookListID = 0;
        //                if (oo.isGame) o.GameID = 0; 
        //                if (oo.isGameLevelActivity) o.GameLevelActivityID = 0;
        //            }

        //            if (IsAdd())
        //            {
        //                if (o.IsValid(BusinessRulesValidationMode.INSERT))
        //                {
        //                    o.Insert();
        //                    PPID.Text = o.PPID.ToString();
        //                    LoadControl();
        //                    masterPage.PageMessage = SRPResources.AddedOK;
                            
        //                }
        //                else
        //                {
        //                    string message = String.Format(SRPResources.ApplicationError1, "<ul>");
        //                    foreach (BusinessRulesValidationMessage m in o.ErrorCodes)
        //                    {
        //                        message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
        //                    }
        //                    message = string.Format("{0}</ul>", message);
        //                    masterPage.PageError = message;
        //                }
        //            }
        //            else
        //            {
        //                if (o.IsValid(BusinessRulesValidationMode.UPDATE))
        //                {
        //                    o.Update();
        //                    masterPage.PageMessage = SRPResources.SaveOK;                          

        //                }
        //                else
        //                {
        //                    string message = String.Format(SRPResources.ApplicationError1, "<ul>");
        //                    foreach (BusinessRulesValidationMessage m in o.ErrorCodes)
        //                    {
        //                        message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
        //                    }
        //                    message = string.Format("{0}</ul>", message);
        //                    masterPage.PageError = message;
        //                }
        //            }


        //            var EndingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);

        //            if (EndingPoints > StartingPoints)
        //            {
        //                // we got more points then when we started ... so maybe we earned a new badge.
        //                Badge EarnedBadge = null;
        //                var EarnedBadges = new List<Badge>();
        //                var now = DateTime.Now;

        //                EarnedBadge = TallyPoints(patron, pgm, StartingPoints, EndingPoints, ref EarnedBadges);
        //                if (EarnedBadge != null)
        //                {
        //                    var list = new List<Badge>();
        //                    AwardPoints.AwardBadgeToPatron(EarnedBadge.BID, patron, ref EarnedBadges);

        //                    //// Award the badge
        //                    //var newPBID = 0;
        //                    //var pb = new PatronBadges { BadgeID = EarnedBadge.BID, DateEarned = now, PID = patron.PID };
        //                    //pb.Insert();
        //                    //newPBID = pb.PBID;

        //                    ////if badge generates notification, then generate the notification
        //                    //if (EarnedBadge.GenNotificationFlag)
        //                    //{
        //                    //    var not = new Notifications
        //                    //    {
        //                    //        PID_To = patron.PID,
        //                    //        PID_From = 0,  //0 == System Notification
        //                    //        Subject = EarnedBadge.NotificationSubject,
        //                    //        Body = EarnedBadge.NotificationBody,
        //                    //        isQuestion = false,
        //                    //        AddedDate = now,
        //                    //        LastModDate = now,
        //                    //        AddedUser = patron.Username,
        //                    //        LastModUser = "N/A"
        //                    //    };
        //                    //    not.Insert();
        //                    //}
        //                }

        //            }

        //            if (e.CommandName.ToLower() == "saveandback")
        //            {
        //                Response.Redirect("PatronLog.aspx");
        //            }

        //        }

        //    }
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
    }
}