﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;

namespace STG.SRP.Controls
{
    public class AwardPoints
    {
        private Patron patron = null;
        public Programs pgm = null;
        private int StartingPoints = 0;
        private int EndingPoints = 0;
        public List<Badge> EarnedBadges = null;

        Badge EarnedBadge;
        DateTime now = DateTime.Now;

        public AwardPoints(int PID)
        {
            patron = Patron.FetchObject(PID);
            pgm = Programs.FetchObject(patron.ProgID);
            EarnedBadges = new List<Badge>();

            StartingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);
            EndingPoints = StartingPoints;
        }

        // points = 100
        // reason = PointAwardReason.MiniGameCompletion
        // MGID = Mini Game ID, if the reason is MiniGameCompletion, else 0
        //
        //
        // returns: a string based list of the badges they earned, or an empty string
        public string AwardPointsToPatron(int points, PointAwardReason reason, 
                // Minigame
                int MGID = 0,
                // reading
                ActivityType readingActivity = ActivityType.Pages, int readingAmount = 0, string author = "", string title = "", string review = "",
                // event
                string eventCode = "", int eventID = 0,
                // book List
                int bookListID = 0,

                DateTime? forceDate = null
            )
        {
            if (forceDate != null) now = (DateTime)forceDate;

            string retValue = "";

        #region Reading - Log to PatronReadingLog
            PatronReadingLog rl = null;
            if (reason == PointAwardReason.Reading)
            {
                rl = new PatronReadingLog
                         {
                             PID = patron.PID,
                             ReadingType = (int) readingActivity,
                             ReadingTypeLabel = (readingActivity).ToString(),
                             ReadingAmount = readingAmount,
                             ReadingPoints = points,
                             LoggingDate = FormatHelper.ToNormalDate(now),
                             Author = author.Trim(),
                             Title = title.Trim(),
                             HasReview = (review.Trim().Length > 0),
                             ReviewID = 0
                         };
                rl.Insert();
            
                // If there is a review, record the review
                if (review.Trim().Length > 0)
                {
                    var r = new PatronReview
                    {
                        PID = patron.PID,
                        PRLID = rl.PRLID,
                        Author = rl.Author,
                        Title = rl.Title,
                        Review = review.Trim(),
                        isApproved = false
                    };
                    r.Insert();

                    rl.ReviewID = r.PRID;
                    rl.Update();
                }

            }
        #endregion

        #region Award PatronPoints

            var pp = new PatronPoints
            {
                PID = patron.PID,
                NumPoints = points,
                AwardDate = now,
                AwardReasonCd = (int)reason,
                AwardReason = PatronPoints.PointAwardReasonCdToDescription(reason),
                BadgeAwardedFlag = false,
                isReading = (reason == PointAwardReason.Reading),
                isEvent = (reason == PointAwardReason.EventAttendance),
                isGameLevelActivity = (reason == PointAwardReason.MiniGameCompletion),
                isBookList = (reason == PointAwardReason.BookListCompletion),
                isGame = false,
                GameLevelActivityID = MGID,
                EventCode = eventCode,
                EventID = eventID,
                BookListID = bookListID,
                LogID = (rl == null ? 0 : rl.PRLID)
            };
            pp.Insert();

            var badgeAwarded = false;
            int badgeToAward = 0;               // <===========

            DAL.Minigame mg = null; 
            if (reason == PointAwardReason.MiniGameCompletion)
            {
                mg = DAL.Minigame.FetchObject(MGID);
                badgeAwarded = mg.AwardedBadgeID > 0;
                badgeToAward = mg.AwardedBadgeID;
            }
            if (reason == PointAwardReason.EventAttendance)
            {
                var evt = Event.GetEvent(eventID);
                badgeAwarded = evt.BadgeID > 0;
                badgeToAward = evt.BadgeID;
            }
            if (reason == PointAwardReason.BookListCompletion)
            {
                var bl = BookList.FetchObject(bookListID);;
                badgeAwarded = (bl.AwardBadgeID > 0);
                badgeToAward = bl.AwardBadgeID;
            }
            
            DataSet pbds = null;
            //DataTable newTable = null;
            if (badgeAwarded)
            {
                if (AwardBadgeToPatron(badgeToAward, patron, ref EarnedBadges))
                {
                    if (pp.PPID != 0)
                    {
                        pp.BadgeAwardedFlag = true;
                        pp.BadgeID = badgeToAward;
                        pp.Update();
                    }
                }
                #region AwardBadgeToPatron code
                //// check if badge was already earned...
                //pbds = PatronBadges.GetAll(patron.PID);
                //var a = pbds.Tables[0].AsEnumerable().Where(r => r.Field<int>("BadgeID") == badgeToAward);

                //newTable = new DataTable();
                //try { newTable = a.CopyToDataTable(); } catch { }

                //// badge not found, award it!
                //if (newTable.Rows.Count == 0)
                //{
                //    var pb = new PatronBadges { BadgeID = badgeToAward, DateEarned = now, PID = patron.PID };
                //    pb.Insert();

                //    EarnedBadge = Badge.GetBadge(badgeToAward);
                //    EarnedBadges.Add(EarnedBadge);

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

                //    //if badge generates prize, then generate the prize
                //    if (EarnedBadge.IncludesPhysicalPrizeFlag)
                //    {
                //        var ppp = new DAL.PatronPrizes
                //                      {
                //                          PID = patron.PID,
                //                          PrizeSource = 1,
                //                          PrizeName = EarnedBadge.PhysicalPrizeName,
                //                          RedeemedFlag = false,
                //                          AddedUser = patron.Username,
                //                          LastModUser = "N/A",
                //                          AddedDate = now,
                //                          LastModDate = now
                //                      };

                //        ppp.Insert();
                //    }



                //    // if badge generates award code, then generate the code
                //    if (EarnedBadge.AssignProgramPrizeCode)
                //    {
                //        var RewardCode = "";
                //        // get the Code value
                //        // save the code value for the patron
                //        RewardCode = ProgramCodes.AssignCodeForPatron(patron.ProgID, patron.ProgID);

                //        // generate the notification
                //        var not = new Notifications
                //        {
                //            PID_To = patron.PID,
                //            PID_From = 0,  //0 == System Notification
                //            Subject = EarnedBadge.PCNotificationSubject,
                //            Body = EarnedBadge.PCNotificationBody.Replace("{ProgramRewardCode}", RewardCode),
                //            isQuestion = false,
                //            AddedDate = now,
                //            LastModDate = now,
                //            AddedUser = patron.Username,
                //            LastModUser = "N/A"
                //        };
                //        not.Insert();
                //    }

                //    if (pp.PPID != 0)
                //    {
                //        pp.BadgeAwardedFlag = true;
                //        pp.BadgeID = badgeToAward;
                //        pp.Update();
                //    }
                //}
                #endregion

            }

        #endregion

        #region If jumped level, award another badge(s)

            // since thay just earned points, check if they also advanced a level in the board game (if there is a board game for the program)
            EndingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);
            EarnedBadge = null;

            var earnedBadges2 = new List<Badge>();

            EarnedBadge = TallyPoints(patron, pgm, StartingPoints, EndingPoints, ref earnedBadges2);
            pbds = PatronBadges.GetAll(patron.PID);

            foreach (var badge in earnedBadges2)
            {
                EarnedBadge = badge;  

                if (EarnedBadge!=null)
                {
                    AwardBadgeToPatron(EarnedBadge.BID, patron, ref EarnedBadges);
                }

                #region AwardBadgeToPatron call2
                //// check if badge was already earned...
                //if (EarnedBadge != null)
                //{
                //    var aa = pbds.Tables[0].AsEnumerable().Where(r => r.Field<int>("BadgeID") == badgeToAward);

                //    newTable = new DataTable();
                //    try { newTable = aa.CopyToDataTable(); }
                //    catch { }

                //    // badge not found, award it!
                //}
                //if (EarnedBadge != null && newTable != null && newTable.Rows.Count == 0)
                //{
                //    // Award the badge

                //    EarnedBadges.Add(badge);  // Add badge tot he list used to display to user ...

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

                //    //if badge generates prize, then generate the prize
                //    if (EarnedBadge.IncludesPhysicalPrizeFlag)
                //    {
                //        var ppp = new DAL.PatronPrizes
                //        {
                //            PID = patron.PID,
                //            PrizeSource = 1,
                //            PrizeName = EarnedBadge.PhysicalPrizeName,
                //            RedeemedFlag = false,
                //            AddedUser = patron.Username,
                //            LastModUser = "N/A",
                //            AddedDate = now,
                //            LastModDate = now
                //        };

                //        ppp.Insert();
                //    }

                //    // if badge generates award code, then generate the code
                //    if (EarnedBadge.AssignProgramPrizeCode)
                //    {
                //        var RewardCode = "";
                //        // get the Code value
                //        // save the code value for the patron
                //        RewardCode = ProgramCodes.AssignCodeForPatron(patron.ProgID, patron.ProgID);

                //        // generate the notification
                //        var not = new Notifications
                //        {
                //            PID_To = patron.PID,
                //            PID_From = 0,  //0 == System Notification
                //            Subject = EarnedBadge.PCNotificationSubject,
                //            Body = EarnedBadge.PCNotificationBody.Replace("{ProgramRewardCode}", RewardCode),
                //            isQuestion = false,
                //            AddedDate = now,
                //            LastModDate = now,
                //            AddedUser = patron.Username,
                //            LastModUser = "N/A"
                //        };
                //        not.Insert();
                //    }
                //}
                #endregion
            }

            #region deprecated - replaced by abbility to earn multiple badges above
            //EarnedBadge = TallyPoints(patron, pgm, StartingPoints, EndingPoints, ref EarnedBadges);
 
            //// check if badge was already earned...
            //if (EarnedBadge != null) {
            //    pbds = PatronBadges.GetAll(patron.PID);
            //    var aa = pbds.Tables[0].AsEnumerable().Where(r => r.Field<int>("BadgeID") == badgeToAward);

            //    newTable = new DataTable();
            //    try { newTable = aa.CopyToDataTable(); } catch { }

            //    // badge not found, award it!
            //}
            //if (EarnedBadge != null && newTable!= null && newTable.Rows.Count == 0)
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

            //    //if badge generates prize, then generate the prize
            //    if (EarnedBadge.IncludesPhysicalPrizeFlag)
            //    {
            //        var ppp = new DAL.PatronPrizes
            //        {
            //            PID = patron.PID,
            //            PrizeSource = 1,
            //            PrizeName = EarnedBadge.PhysicalPrizeName,
            //            RedeemedFlag = false,
            //            AddedUser = patron.Username,
            //            LastModUser = "N/A",
            //            AddedDate = now,
            //            LastModDate = now
            //        };

            //        ppp.Insert();
            //    }

            //    // if badge generates award code, then generate the code
            //    if (EarnedBadge.AssignProgramPrizeCode)
            //    {
            //        var RewardCode = "";
            //        // get the Code value
            //        // save the code value for the patron
            //        RewardCode = ProgramCodes.AssignCodeForPatron(patron.ProgID, patron.ProgID);

            //        // generate the notification
            //        var not = new Notifications
            //        {
            //            PID_To = patron.PID,
            //            PID_From = 0,  //0 == System Notification
            //            Subject = EarnedBadge.PCNotificationSubject,
            //            Body = EarnedBadge.PCNotificationBody.Replace("{ProgramRewardCode}", RewardCode),
            //            isQuestion = false,
            //            AddedDate = now,
            //            LastModDate = now,
            //            AddedUser = patron.Username,
            //            LastModUser = "N/A"
            //        };
            //        not.Insert();
            //    }
            #endregion

            
        #endregion

        #region Check and give awards if any

            AwardBadgeToPatronViaMatchingAwards(patron, ref EarnedBadges);

        #endregion

         #region Prepare return code
            // did they earn one or more badges?
            if (EarnedBadges.Count > 0)
            {
                var badges = EarnedBadges.Count.ToString();
                //foreach(Badge b in EarnedBadges)
                //{
                //    badges = badges + "|" + b.BID.ToString();
                //}
                badges = EarnedBadges.Aggregate(badges, (current, b) => current + "|" + b.BID.ToString());
                //Response.Redirect("~/BadgeAward.aspx?b=" + badges);
                retValue = badges;
            }
        #endregion

            return retValue;
        }

        public Badge TallyPoints(Patron patron, Programs pgm, int StartingPoints, int EndingPoints, ref List<Badge> EarnedBadges)
        {
            Badge b = null;
            //Tally up the points and figure out if we need to award a badge.  
            if (pgm.ProgramGameID > 0)
            {
                // only if we have a game we can earn badges by reading ....
                var gm = ProgramGame.FetchObject(pgm.ProgramGameID);
                var ds = ProgramGameLevel.GetAll(gm.PGID);

                var normalLevelTotalPoints = GetGameCompletionPoints(ds);
                var bonusLevelTotalPoints = GetGameCompletionBonusPoints(ds, gm.BonusLevelPointMultiplier);

                var bonus = (StartingPoints > normalLevelTotalPoints);
                int BeforeLevel = 0, AfterLevel = 0;

                // loop thru the levels to see where we are at ... before awarding the new points
                var rp = StartingPoints;   //remaining points
                if (bonus)
                {
                    // if we are on the bonus, eliminate the "fully completed boards/levels) and then see what the remainder of the points is.
                    rp = rp - normalLevelTotalPoints;
                    rp = rp % bonusLevelTotalPoints;
                }

                for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var multiplier = (bonus ? gm.BonusLevelPointMultiplier : 1.00m);
                    var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
                    rp = rp - levelPoints;
                    if (rp < 0)
                    {
                        BeforeLevel = i;
                        break;
                    }
                }


                // loop thru the levels to see where we are at ... AFTER awarding the new points
                rp = EndingPoints;   //remaining points
                if (bonus)
                {
                    // if we are on the bonus, eliminate the "fully completed boards/levels) and then see what the remainder of the points is.
                    rp = rp - normalLevelTotalPoints;
                    rp = rp % bonusLevelTotalPoints;
                }
                for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var multiplier = (bonus ? gm.BonusLevelPointMultiplier : 1.00m);
                    var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
                    rp = rp - levelPoints;
                    AfterLevel = i;
                    if (rp < 0)
                    {
                        break;
                    }
                    else
                    {
                        if (!((i+1) < ds.Tables[0].Rows.Count))
                        {
                            AfterLevel = (i+1);
                        }
                    }
                }

                if (BeforeLevel != AfterLevel)
                {
                    // completed the "beforeLevel" and moved up to the "AfterLevel" , so check if we need to award a badge
                    for (var i = BeforeLevel ; i < AfterLevel ; i++)
                    {
                        var badgeToAward = Convert.ToInt32(ds.Tables[0].Rows[i]["AwardBadgeID"]);
                        if (badgeToAward > 0)
                        {
                            b = Badge.GetBadge(badgeToAward);
                            EarnedBadges.Add(b);
                        }
                    }
                    //var BadgeToAward = Convert.ToInt32(ds.Tables[0].Rows[BeforeLevel]["AwardBadgeID"]);
                    //if (BadgeToAward > 0)
                    //{
                    //    b = Badge.GetBadge(BadgeToAward);
                    //    EarnedBadges.Add(b);
                    //}
                }
            }
            return b;
        }

        public int GetGameCompletionPoints(DataSet ds)
        {
            var ret = 0;
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ret = ret + Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]));
            }
            return ret;
        }

        public int GetGameCompletionBonusPoints(DataSet ds, decimal bonusLevelPointMultiplier)
        {
            var ret = 0;
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var multiplier = bonusLevelPointMultiplier;
                var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
                ret = ret + levelPoints;
            }
            return ret;
        }

        public static bool AwardBadgeToPatron(int badgeToAward, Patron patron, ref List<Badge> earnedBadges)
        {
            var now = DateTime.Now;

            // check if badge was already earned...
            var pbds = PatronBadges.GetAll(patron.PID);
            var a = pbds.Tables[0].AsEnumerable().Where(r => r.Field<int>("BadgeID") == badgeToAward);

            var newTable = new DataTable();
            try { newTable = a.CopyToDataTable(); }
            catch { }

            // badge not found, award it!
            if (newTable.Rows.Count == 0)
            {
                var pb = new PatronBadges { BadgeID = badgeToAward, DateEarned = now, PID = patron.PID };
                pb.Insert();

                var earnedBadge = Badge.GetBadge(badgeToAward);
                earnedBadges.Add(earnedBadge);

                //if badge generates notification, then generate the notification
                if (earnedBadge.GenNotificationFlag)
                {
                    var not = new Notifications
                    {
                        PID_To = patron.PID,
                        PID_From = 0,  //0 == System Notification
                        Subject = earnedBadge.NotificationSubject,
                        Body = earnedBadge.NotificationBody,
                        isQuestion = false,
                        AddedDate = now,
                        LastModDate = now,
                        AddedUser = patron.Username,
                        LastModUser = "N/A"
                    };
                    not.Insert();
                }

                //if badge generates prize, then generate the prize
                if (earnedBadge.IncludesPhysicalPrizeFlag)
                {
                    var ppp = new DAL.PatronPrizes
                    {
                        PID = patron.PID,
                        PrizeSource = 1,
                        BadgeID = badgeToAward,
                        PrizeName = earnedBadge.PhysicalPrizeName,
                        RedeemedFlag = false,
                        AddedUser = patron.Username,
                        LastModUser = "N/A",
                        AddedDate = now,
                        LastModDate = now
                    };

                    ppp.Insert();
                }



                // if badge generates award code, then generate the code
                if (earnedBadge.AssignProgramPrizeCode)
                {
                    var rewardCode = "";
                    // get the Code value
                    // save the code value for the patron
                    rewardCode = ProgramCodes.AssignCodeForPatron(patron.ProgID, patron.ProgID);

                    // generate the notification
                    var not = new Notifications
                    {
                        PID_To = patron.PID,
                        PID_From = 0,  //0 == System Notification
                        Subject = earnedBadge.PCNotificationSubject,
                        Body = earnedBadge.PCNotificationBody.Replace("{ProgramRewardCode}", rewardCode),
                        isQuestion = false,
                        AddedDate = now,
                        LastModDate = now,
                        AddedUser = patron.Username,
                        LastModUser = "N/A"
                    };
                    not.Insert();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AwardBadgeToPatronViaMatchingAwards(Patron patron, ref List<Badge> earnedBadges)
        {
            var retcode = false;
            var ds = Award.GetMatchingAwards(patron.PID);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var bid = Convert.ToInt32(row["BadgeID"]);
                    var ret = AwardBadgeToPatron(bid, patron, ref earnedBadges);

                    retcode = ret || retcode;
                } 
            }

            return retcode;
        }
    }
}