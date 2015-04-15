using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.DAL;

namespace STG.SRP.Controls
{
    public partial class ReadingListCtl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var ds = DAL.BookList.GetForDisplay(((Patron)Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();
                if (ds.Tables[0].Rows.Count == 0) lblNoLists.Visible = true;
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var BLID = int.Parse(e.CommandArgument.ToString());

            var bl = BookList.FetchObject(BLID);

            lblTitle.Text = bl.ListName;
            lblDesc.Text = bl.Description;

            lblTitle.Visible = true;
            lblDesc.Visible = true;

            var ds = BookListBooks.GetForDisplay(bl.BLID, ((Patron)Session["Patron"]).PID);
            rptr2.DataSource = ds;
            rptr2.DataBind();

            pnlDetail.Visible = true;
            btnSave.Visible = btnPrint.Visible = true;
            lblMessage.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            var now = DateTime.Now;
            var onlyCheckedBoxes = true;
            var selBLI = 0;
            var readCount = 0;
            var neeedCount = 0;
            var BLID = -1;
            foreach (RepeaterItem item in rptr2.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    if (BLID < 0)
                    {
                        BLID = int.Parse(((Label)item.FindControl("BLID")).Text);
                        neeedCount = BookList.FetchObject(BLID).NumBooksToComplete;
                    }
                    var chkRead = (CheckBox)item.FindControl("chkRead");
                    var PBLBID = int.Parse(((Label)item.FindControl("PBLBID")).Text);
                    var BLBID = int.Parse(((Label)item.FindControl("BLBID")).Text);
                    
                    selBLI = BLID;
                    var pbl = new PatronBookLists();
                    if (PBLBID != 0)
                    {
                        pbl = PatronBookLists.FetchObject(PBLBID);
                    }
                    pbl.BLBID = BLBID;
                    pbl.BLID = BLID;
                    pbl.PID = ((Patron)Session["Patron"]).PID;
                    pbl.LastModDate = now;

                    pbl.HasReadFlag = chkRead.Checked;
                    if (!pbl.HasReadFlag)
                    {
                        onlyCheckedBoxes = false;
                    }
                    else
                    {
                        readCount++;
                    }

                    if (PBLBID != 0)
                    {
                        pbl.Update();
                    }
                    else
                    {
                        pbl.Insert();
                    }
                }
            }

            lblMessage.Visible = true;

            // read the entire book list!  Award points and badges 
            if ((neeedCount == 0 && onlyCheckedBoxes) || (neeedCount <= readCount))
            {
                var bl = BookList.FetchObject(selBLI);

                if (PatronPoints.HasEarnedBookList(((Patron)Session["Patron"]).PID, selBLI)) return;


                if (bl.AwardBadgeID != 0 || bl.AwardPoints != 0)
                {

                    var pa = new AwardPoints(((Patron)Session["Patron"]).PID);
                    var sBadges = pa.AwardPointsToPatron(bl.AwardPoints, PointAwardReason.BookListCompletion,
                                                            bookListID: bl.BLID);
                    if (sBadges.Length > 0) Response.Redirect("~/BadgeAward.aspx?b=" + sBadges);                    
                }
            }
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