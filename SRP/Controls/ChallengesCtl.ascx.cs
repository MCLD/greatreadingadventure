using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using GRA.Tools;
using GRA.SRP.Utilities.CoreClasses;
using SRPApp.Classes;

namespace GRA.SRP.Controls {
    public partial class ChallengesCtl : System.Web.UI.UserControl {
        private bool ProgramOpen { get; set; }
        private const string BadgeLinkAndImage = "<a href=\"~/Badges/Details.aspx?BadgeId={0}\" runat=\"server\" OnClick=\"return ShowBadgeInfo({0});\" class=\"thumbnail pull-left\"><img src=\"/images/badges/sm_{0}.png\" /></a>";
        public bool ShowModal { get; set; }

        protected string ShowBadge(object badgeIdObject) {
            int? badgeId = badgeIdObject as int?;
            if(badgeId == null || badgeId == 0) {
                return "No badge.";
            } else {
                return string.Format(BadgeLinkAndImage, badgeId);
            }
        }

        protected string ProgressDisplay(object amountObject, object totalObject) {
            int amount = amountObject as int? ?? 0;
            int total = totalObject as int? ?? 0;

            if(total == 0 || amount == 0) {
                return string.Empty;
            }
            if(amount == total) {
                return "Challenge complete!";
            }

            return string.Format("{0} of {1}", amountObject, totalObject);
        }

        protected int ComputePercent(object amountObject, object totalObject) {
            int amount = amountObject as int? ?? 0;
            int total = totalObject as int? ?? 0;

            if(total == 0) {
                return 0;
            } else {
                return (int)(amount * 100.0 / total);
            }
        }

        protected void Page_Init(object sender, EventArgs e) {
            var patron = Session["Patron"] as Patron;
            if(patron == null) {
                Response.Redirect("~");
            }

            var pgm = DAL.Programs.FetchObject(patron.ProgID);
            if(pgm == null) {
                pgm = Programs.FetchObject(
                    Programs.GetDefaultProgramForAgeAndGrade(patron.Age,
                                                             patron.SchoolGrade.SafeToInt()));
            }

            if(pgm == null || !pgm.IsOpen) {
                this.ProgramOpen = false;
                btnSave.Visible = false;
            } else {
                this.ProgramOpen = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                PopulateChallengeList();
                this.ShowModal = false;
            }
        }

        protected void PopulateChallengeList() {
            var ds = DAL.BookList.GetForDisplay(((Patron)Session["Patron"]).PID);
            rptr.DataSource = ds;
            rptr.DataBind();
            if(ds.Tables[0].Rows.Count == 0) {
                lblNoLists.Visible = true;
            }
        }

        protected void rptr_ItemDataBound(object source, RepeaterItemEventArgs e) {
            if(!this.ProgramOpen) {
                if(e.Item.ItemType == ListItemType.Item
                   || e.Item.ItemType == ListItemType.AlternatingItem) {
                    var checkbox = e.Item.FindControl("chkRead") as CheckBox;
                    if(checkbox != null) {
                        checkbox.Enabled = false;
                    }

                }
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e) {
            var BLID = int.Parse(e.CommandArgument.ToString());

            var bl = BookList.FetchObject(BLID);

            lblTitle.Text = bl.ListName;
            lblDesc.Text = Server.HtmlDecode(bl.Description);

            string award = null;

            if(bl.AwardPoints > 0) {
                award = string.Format("Completing this challenge will earn: <strong>{0} point{1}</strong>",
                                      bl.AwardPoints,
                                      bl.AwardPoints > 1 ? "s" : string.Empty);
            }

            if(bl.AwardBadgeID > 0) {
                if(string.IsNullOrWhiteSpace(award)) {
                    award = "Completing this challenge will earn: <strong>a badge</strong>.";
                } else {
                    award += " and <strong>a badge</strong>.";
                }

                BadgeImage.Text = string.Format("<img class=\"thumbnail disabled\" src=\"/images/badges/sm_{0}.png\" />", bl.AwardBadgeID);
            } else {
                BadgeImage.Text = string.Empty;
                award += ".";
            }

            BadgeImage.Visible = !string.IsNullOrEmpty(BadgeImage.Text);

            if(!string.IsNullOrWhiteSpace(award)) {
                lblPoints.Text = award;
                lblPoints.Visible = true;
            }

            lblTitle.Visible = true;
            lblDesc.Visible = true;

            var ds = BookListBooks.GetForDisplay(bl.BLID, ((Patron)Session["Patron"]).PID);
            rptr2.DataSource = ds;
            rptr2.DataBind();
            printLink.NavigateUrl = string.Format("~/Challenges/Details.aspx?blid={0}&print=1",
                                                  bl.BLID);
            pnlDetail.Visible = true;
            this.ShowModal = true;
        }

        protected void btnClose_Click(object sender, EventArgs e) {
            pnlDetail.Visible = false;
            this.ShowModal = false;
        }

        protected void btnSave_Click(object sender, EventArgs e) {
            if(!this.ProgramOpen) {
                return;
            }

            var now = DateTime.Now;
            var onlyCheckedBoxes = true;
            var selBLI = 0;
            var readCount = 0;
            var neeedCount = 0;
            var BLID = -1;
            foreach(RepeaterItem item in rptr2.Items) {
                if(item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem) {
                    if(BLID < 0) {
                        BLID = int.Parse(((Label)item.FindControl("BLID")).Text);
                        neeedCount = BookList.FetchObject(BLID).NumBooksToComplete;
                    }
                    var chkRead = (CheckBox)item.FindControl("chkRead");
                    var PBLBID = int.Parse(((Label)item.FindControl("PBLBID")).Text);
                    var BLBID = int.Parse(((Label)item.FindControl("BLBID")).Text);

                    selBLI = BLID;
                    var pbl = new PatronBookLists();
                    if(PBLBID != 0) {
                        pbl = PatronBookLists.FetchObject(PBLBID);
                    }
                    pbl.BLBID = BLBID;
                    pbl.BLID = BLID;
                    pbl.PID = ((Patron)Session["Patron"]).PID;
                    pbl.LastModDate = now;

                    pbl.HasReadFlag = chkRead.Checked;
                    if(!pbl.HasReadFlag) {
                        onlyCheckedBoxes = false;
                    } else {
                        readCount++;
                    }

                    if(PBLBID != 0) {
                        pbl.Update();
                    } else {
                        pbl.Insert();
                    }
                }
            }

            string success = StringResources.getString("challenges-progress-saved");
            new SessionTools(Session).AlertPatron(success,
                glyphicon: "check");

            // read the entire book list!  Award points and badges 
            if((neeedCount == 0 && onlyCheckedBoxes) || (neeedCount <= readCount)) {
                success = StringResources.getString("challenges-completed");

                new SessionTools(Session).AlertPatron(success, glyphicon: "star");

                var bl = BookList.FetchObject(selBLI);

                if(PatronPoints.HasEarnedBookList(((Patron)Session["Patron"]).PID, selBLI)) {
                    PopulateChallengeList();
                    return;
                }

                if(bl.AwardBadgeID != 0 || bl.AwardPoints != 0) {
                    success = StringResources.getString("challenges-completed-badge");
                    new SessionTools(Session).AlertPatron(success,
                        glyphicon: "certificate");

                    var pa = new AwardPoints(((Patron)Session["Patron"]).PID);
                    var sBadges = pa.AwardPointsToPatron(bl.AwardPoints, PointAwardReason.BookListCompletion,
                                                            bookListID: bl.BLID);
                    if(sBadges.Length > 0) {
                        new SessionTools(Session).EarnedBadges(sBadges);
                    }
                }
            }
            PopulateChallengeList();
        }



        //protected void btnSave_Click(object sender, EventArgs e)
        //{

        //    var now = DateTime.Now;
        //    var onlyCheckedBoxes = true;
        //    var selBLI = 0;
        //    foreach (RepeaterItem item in rptr2.Items)
        //    {
        //        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
        //        {
        //            var chkRead = (CheckBox)item.FindControl("chkRead");
        //            var PBLBID = int.Parse(((Label)item.FindControl("PBLBID")).Text);
        //            var BLBID = int.Parse(((Label)item.FindControl("BLBID")).Text);
        //            var BLID = int.Parse(((Label)item.FindControl("BLID")).Text);
        //            selBLI = BLID;
        //            var pbl = new PatronBookLists();
        //            if (PBLBID !=0)
        //            {
        //                pbl = PatronBookLists.FetchObject(PBLBID);
        //            }
        //            pbl.BLBID = BLBID;
        //            pbl.BLID = BLID;
        //            pbl.PID = ((Patron)Session["Patron"]).PID;
        //            pbl.LastModDate = now;

        //            pbl.HasReadFlag = chkRead.Checked;
        //            if (!pbl.HasReadFlag) onlyCheckedBoxes = false;

        //            if (PBLBID != 0)
        //            {
        //                pbl.Update();
        //            }
        //            else
        //            {
        //                pbl.Insert();
        //            }
        //        }
        //    }

        //    lblMessage.Visible = true;

        //    // read the entire book list!  Award points and badges 
        //    if (onlyCheckedBoxes)
        //    {
        //        var bl = BookList.FetchObject(selBLI);

        //        if (PatronPoints.HasEarnedBookList(((Patron)Session["Patron"]).PID, selBLI)) return;


        //        if (bl.AwardBadgeID != 0 || bl.AwardPoints != 0)
        //        {


        //            //check first if they earned this already
        //            var EarnedBadges = new List<Badge>();
        //            var patron = (Patron)Session["Patron"];
        //            var pgm = Programs.FetchObject(patron.ProgID);
        //            var StartingPoints = PatronPoints.GetTotalPatronPoints(patron.PID);
        //            var EndingPoints = StartingPoints;

        //            Badge EarnedBadge;

        //            var pp = new PatronPoints();
        //            if (bl.AwardPoints != 0)
        //            {
        //                // 1 - log points to patron activity and no badge yet
        //                pp = new PatronPoints
        //                             {
        //                                 PID = patron.PID,
        //                                 NumPoints = bl.AwardPoints,
        //                                 AwardDate = now,
        //                                 AwardReasonCd = (int) PointAwardReason.BookListCompletion,
        //                                 AwardReason =
        //                                     PatronPoints.PointAwardReasonCdToDescription(
        //                                         PointAwardReason.BookListCompletion),
        //                                 BadgeAwardedFlag = false,
        //                                 isBookList = true,
        //                                 BookListID = bl.BLID,
        //                                 isEvent = false,
        //                                 isGame = false,
        //                                 isGameLevelActivity = false,
        //                                 isReading = false,
        //                                 LogID = 0
        //                             };
        //                pp.Insert();                        
        //            }
        //            // if we also need to award badge ...
        //            if (bl.AwardBadgeID != 0)
        //            {
        //                var pbds = PatronBadges.GetAll(patron.PID);
        //                var a = pbds.Tables[0].AsEnumerable().Where(r => r.Field<int>("BadgeID") == bl.AwardBadgeID);

        //                var newTable = new DataTable();
        //                try {newTable = a.CopyToDataTable();}catch{}
        //                //DataTable newTable = a.CopyToDataTable();

        //                if (newTable.Rows.Count == 0)
        //                {
        //                    var pb = new PatronBadges { BadgeID = bl.AwardBadgeID, DateEarned = now, PID = patron.PID };
        //                    pb.Insert();

        //                    EarnedBadge = Badge.GetBadge(bl.AwardBadgeID);
        //                    EarnedBadges.Add(EarnedBadge);

        //                    //if badge generates notification, then generate the notification
        //                    if (EarnedBadge.GenNotificationFlag)
        //                    {
        //                        var not = new Notifications
        //                        {
        //                            PID_To = patron.PID,
        //                            PID_From = 0,  //0 == System Notification
        //                            Subject = EarnedBadge.NotificationSubject,
        //                            Body = EarnedBadge.NotificationBody,
        //                            isQuestion = false,
        //                            AddedDate = now,
        //                            LastModDate = now,
        //                            AddedUser = patron.Username,
        //                            LastModUser = "N/A"
        //                        };
        //                        not.Insert();
        //                    }

        //                    // If we awarded points as well, the update with the badge...
        //                    if (pp.PPID != 0)
        //                    {
        //                        pp.BadgeAwardedFlag = true;
        //                        pp.BadgeID = bl.AwardBadgeID;
        //                        pp.Update();
        //                    }
        //                }
        //            }

        //            if (EarnedBadges.Count > 0)
        //            {
        //                //Display Badges Awards messages
        //                var badges = EarnedBadges.Count.ToString();
        //                //foreach(Badge b in EarnedBadges)
        //                //{
        //                //    badges = badges + "|" + b.BID.ToString();
        //                //}
        //                badges = EarnedBadges.Aggregate(badges, (current, b) => current + "|" + b.BID.ToString());
        //                //Server.Transfer("~/BadgeAward.aspx?b=" + badges);
        //                Response.Redirect("~/BadgeAward.aspx?b=" + badges);

        //            }
        //        }




        //    }


        //}

    }
}