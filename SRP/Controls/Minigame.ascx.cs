using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using GRA.Tools;

namespace GRA.SRP.Controls
{
    public partial class Minigame : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["MGID"]) && (Session["MGID"] == null || Session["MGID"].ToString() == ""))
                {
                    Response.Redirect("~/Account/");
                }
                if (!string.IsNullOrEmpty(Request["MGID"]))
                {
                    MGID.Text = Request["MGID"];
                    Session["MGID"] = MGID.Text;
                }
                else
                {
                    MGID.Text = Session["MGID"].ToString();
                }

                if (Session["GoToUrl"] != null && Session["GoToUrl"].ToString() == "")
                {
                    GoToUrl = Session["GoToUrl"].ToString();
                    Session["GoToUrl"] = "";
                }



                var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text));
                MGName.Text = mg.GameName;
                Acknowledgements.Text = mg.Acknowledgements; 
                if (Acknowledgements.Text.Length==0)
                {
                    Acknowledgements.Visible = false;
                }
                else
                {
                    Acknowledgements.Text = "<br><br>" + Acknowledgements.Text;
                }
                SetIntructions(mg.MiniGameType);
                LoadMinigame(mg.MGID, mg.MiniGameType);
                
                var gs = new DAL.GamePlayStats();
                var patron = (Patron) Session["Patron"];
                PID.Text = patron.PID.ToString();
                gs.Started = DateTime.Now;
                gs.MGID = mg.MGID;
                gs.MGType = mg.MiniGameType;
                gs.Difficulty = "Easy";
                gs.PID = patron.PID;
                gs.Insert();
                GPSID.Text = gs.GPSID.ToString();
            }
        }

        private void SetIntructions(int minigame)
        {
            var lbl = FindControl("lbl_mg" + minigame.ToString()) as Label;
            if (lbl != null) lbl.Visible = true;
        }

        protected override void OnInit(EventArgs e)
        {
            // Turn Off AJAX so we can use the JQuery for game boad manipulation on Hidden Picture AND Matching Game
            if (string.IsNullOrEmpty(Request["MGID"]) && (Session["MGID"] == null || Session["MGID"].ToString() == ""))
            {
                Response.Redirect("~/Account/");
            }
            if (!string.IsNullOrEmpty(Request["MGID"]))
            {
                MGID.Text = Request["MGID"];
                Session["MGID"] = MGID.Text;
            }
            else
            {
                MGID.Text = Session["MGID"].ToString();
            }
            var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text));
            if (mg.MiniGameType == 5 || mg.MiniGameType == 6) ScriptManager1.EnablePartialRendering = false;
            base.OnInit(e);
        }

        private void LoadMinigame(int mgid, int minigame)
        {
            if (minigame ==1)
            {
                var mg = DAL.MGOnlineBook.FetchObjectByParent(mgid);
                MGBook1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false; 
                    pnlGame.Visible = true;
                }
                MGBook1.Visible = true;
                
            }

            if (minigame == 2)
            {
                var mg = DAL.MGMixAndMatch.FetchObjectByParent(mgid);
                MGMixMatchPlay1.LoadGame(mgid,1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGMixMatchPlay1.Visible = true;
            }

            if (minigame == 3)
            {
                //var mg = DAL.MGCodeBreaker.FetchObjectByParent(mgid);
                var mg = DAL.MGCodeBreaker.FetchObjectByParent(mgid);
                MGCodeBreaker1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGCodeBreaker1.Visible = true;
            }

            if (minigame == 4)
            {
                var mg = DAL.MGWordMatch.FetchObjectByParent(mgid);
                MGWordMatch1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGWordMatch1.Visible = true;
            }

            if (minigame == 5)
            {
                var mg = DAL.MGMatchingGame.FetchObjectByParent(mgid);
                MGMatchingGamePlay1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGMatchingGamePlay1.Visible = true;
            }

            if (minigame == 6)
            {
                
                var mg = DAL.MGHiddenPic.FetchObjectByParent(mgid);
                MGHiddenPicPlay1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGHiddenPicPlay1.Visible = true;
            }

            if (minigame == 7)
            {
                var mg = DAL.MGChooseAdv.FetchObjectByParent(mgid);
                MGChooseAdvPlay1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGChooseAdvPlay1.Visible = true;
            }

            


        }

        protected void BtnEasy_Click(object sender, EventArgs e)
        {
            pnlDifficulty.Visible = false;
            pnlGame.Visible = true;

        }

        protected void BtnMedium_Click(object sender, EventArgs e)
        {
            pnlDifficulty.Visible = false;
            pnlGame.Visible = true;
            var gs = GamePlayStats.FetchObject(int.Parse(GPSID.Text));
            gs.Difficulty = "Medium";
            gs.Update();

            //ReloadGame(Medium)
            var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text));
            if (mg.MiniGameType ==1)
            {
                MGBook1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 2)
            {
                MGMixMatchPlay1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 3)
            {
                MGCodeBreaker1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 4)
            {
                MGWordMatch1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 5)
            {
                MGMatchingGamePlay1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 6)
            {
                MGHiddenPicPlay1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 7)
            {
                MGChooseAdvPlay1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }          
        }

        protected void BtnHard_Click(object sender, EventArgs e)
        {
            pnlDifficulty.Visible = false;
            pnlGame.Visible = true;
            var gs = GamePlayStats.FetchObject(int.Parse(GPSID.Text));
            gs.Difficulty = "Hard";
            gs.Update();

            //ReloadGame(Hard)
            var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text)); 
            if (mg.MiniGameType == 1)
            {
                MGBook1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 2)
            {
                MGMixMatchPlay1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 3)
            {
                MGCodeBreaker1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 4)
            {
                MGWordMatch1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 5)
            {
                MGMatchingGamePlay1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 6)
            {
                MGHiddenPicPlay1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 7)
            {
                MGChooseAdvPlay1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }  
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            CompleteGamePlay();
        }

        public void CompleteGamePlay()
        {
            var gs = GamePlayStats.FetchObject(int.Parse(GPSID.Text));
            gs.Completed = DateTime.Now;
            gs.CompletedPlay = true;
            gs.Update();

            pnlGame.Visible = false;
            pnlWinner.Visible = true;

            //* actually award points and what not ... but only if they have not reaceived points for this minigame already
            if (!PatronPoints.HasEarnedMinigamePoints(int.Parse(PID.Text), int.Parse(MGID.Text))) ProcessTheWin();

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect(GoToUrl);
        }

        public void ProcessTheWin()
        {
            var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text));
            var pa = new AwardPoints(int.Parse(PID.Text));
            var sBadges = pa.AwardPointsToPatron(mg.NumberPoints, PointAwardReason.MiniGameCompletion, mg.MGID);
            if (sBadges.Length > 0) {
                Session[SessionKey.EarnedBadges] = sBadges;
            }
            

            //var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text));
            //var patron = (Patron)Patron.FetchObject(int.Parse(PID.Text));
            //var pgm = Programs.FetchObject(patron.ProgID);

            //var StartingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);
            //var EndingPoints = StartingPoints;
            //var EarnedBadges = new List<Badge>();
            //Badge EarnedBadge;

            //// 1 - log points to patron activity (mg.NumberPoints)
            //var now = DateTime.Now;
            //var pp = new PatronPoints
            //{
            //    PID = patron.PID,
            //    NumPoints = mg.NumberPoints,
            //    AwardDate = now,
            //    AwardReasonCd = (int)PointAwardReason.MiniGameCompletion,
            //    AwardReason = PatronPoints.PointAwardReasonCdToDescription(PointAwardReason.MiniGameCompletion),
            //    BadgeAwardedFlag = false,
            //    isBookList = false,
            //    isEvent = false,
            //    isGame = false,
            //    isGameLevelActivity = true,
            //    GameLevelActivityID = mg.MGID,
            //    isReading = false,
            //    LogID = 0
            //};
            //pp.Insert();

            //if (mg.AwardedBadgeID > 0)
            //{
            //    var pbds = PatronBadges.GetAll(patron.PID);
            //    var a = pbds.Tables[0].AsEnumerable().Where(r => r.Field<int>("BadgeID") == mg.AwardedBadgeID);

            //    var newTable = new DataTable();
            //    try { newTable = a.CopyToDataTable(); }
            //    catch { }
            //    //DataTable newTable = a.CopyToDataTable();

            //    if (newTable.Rows.Count == 0)
            //    {
            //        var pb = new PatronBadges { BadgeID = mg.AwardedBadgeID, DateEarned = now, PID = patron.PID };
            //        pb.Insert();

            //        EarnedBadge = Badge.GetBadge(mg.AwardedBadgeID);
            //        EarnedBadges.Add(EarnedBadge);

            //        //if badge generates notification, then generate the notification
            //        if (EarnedBadge.GenNotificationFlag)
            //        {
            //            var not = new Notifications
            //            {
            //                PID_To = patron.PID,
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

            //        pp.BadgeAwardedFlag = true;
            //        pp.BadgeID = mg.AwardedBadgeID;
            //        pp.Update();
            //    }


            //}
            //EndingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);
            //EarnedBadge = null;
            //EarnedBadge = TallyPoints(patron, pgm, StartingPoints, EndingPoints, ref EarnedBadges);
            //if (EarnedBadge != null)
            //{
            //    // Award the badge
            //    var newPBID = 0;
            //    var pb = new PatronBadges { BadgeID = EarnedBadge.BID, DateEarned = now, PID = patron.PID };
            //    pb.Insert();
            //    newPBID = pb.PBID;

            //    //if badge generates notification, then generate the notification
            //    if (EarnedBadge.GenNotificationFlag)
            //    {
            //        var not = new Notifications
            //        {
            //            PID_To = patron.PID,
            //            PID_From = 0,  //0 == System Notification
            //            Subject = EarnedBadge.NotificationSubject,
            //            Body = EarnedBadge.NotificationBody,
            //            isQuestion = false,
            //            AddedDate = now,
            //            LastModDate = now,
            //            AddedUser = patron.Username,
            //            LastModUser = "N/A"
            //        };
            //        not.Insert();
            //    }

            //}

            //if (EarnedBadges.Count > 0)
            //{
            //    //Display Badges Awards messages
            //    var badges = EarnedBadges.Count.ToString();
            //    //foreach(Badge b in EarnedBadges)
            //    //{
            //    //    badges = badges + "|" + b.BID.ToString();
            //    //}
            //    badges = EarnedBadges.Aggregate(badges, (current, b) => current + "|" + b.BID.ToString());
            //    //Server.Transfer("~/BadgeAward.aspx?b=" + badges);
            //    Response.Redirect("~/BadgeAward.aspx?b=" + badges);

            //}

        }

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

        public string GoToUrl
        {
            get
            {
                if (ViewState["gotourl"] == null || ViewState["gotourl"].ToString().Length == 0)
                {
                    ViewState["gotourl"] = "~/Adventures/";
                }
                return ViewState["gotourl"].ToString();
            }
            set { ViewState["gotourl"] = value; }
        }

    }
}