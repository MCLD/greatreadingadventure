using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Challenges {
    public partial class Details : BaseSRPPage {
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
                return 100;
            } else {
                return (int)(amount * 100.0 / total);
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            IsSecure = true;
            if(!IsPostBack) {
                if(Request.QueryString["blid"] != null) {
                    int blid;
                    if(int.TryParse(Request.QueryString["blid"].ToString(), out blid)) {
                        LookupChallenge(blid);
                    }

                }
                TranslateStrings(this);
            }
        }

        protected void LookupChallenge(int blid) {
            var bl = BookList.FetchObject(blid);

            if(bl == null) {
                challengeDetails.Visible = false;
                Session[SessionKey.PatronMessage] = "Could not find details on that Challenge.";
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                Session[SessionKey.PatronMessageGlyphicon] = "remove";
            } else {

                challengeTitle.Text = bl.ListName;
                lblDesc.Text = bl.Description;

                string award = null;

                if(bl.AwardPoints > 0) {
                    award = string.Format("Completing this challenge will earn: <strong>{0} points</strong>", bl.AwardPoints);
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
                var ds = BookListBooks.GetForDisplay(bl.BLID, ((Patron)Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();

                this.ShowModal = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e) {

            var now = DateTime.Now;
            var onlyCheckedBoxes = true;
            var selBLI = 0;
            var readCount = 0;
            var neeedCount = 0;
            var BLID = -1;
            foreach(RepeaterItem item in rptr.Items) {
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

            Session[SessionKey.PatronMessage] = "Your progress has been saved!";
            Session[SessionKey.PatronMessageGlyphicon] = "check";

            // read the entire book list!  Award points and badges 
            if((neeedCount == 0 && onlyCheckedBoxes) || (neeedCount <= readCount)) {
                Session[SessionKey.PatronMessage] = "Good work, you've completed a Challenge!";
                Session[SessionKey.PatronMessageGlyphicon] = "star";

                var bl = BookList.FetchObject(selBLI);

                if(PatronPoints.HasEarnedBookList(((Patron)Session["Patron"]).PID, selBLI)) {
                    return;
                }

                if(bl.AwardBadgeID != 0 || bl.AwardPoints != 0) {
                    Session[SessionKey.PatronMessage] = "Congratulations, you completed a Challenge and were awarded a badge!";
                    Session[SessionKey.PatronMessageGlyphicon] = "certificate";


                    var pa = new AwardPoints(((Patron)Session["Patron"]).PID);
                    var sBadges = pa.AwardPointsToPatron(bl.AwardPoints, PointAwardReason.BookListCompletion,
                                                            bookListID: bl.BLID);
                    if(sBadges.Length > 0) {
                        Session[SessionKey.EarnedBadges] = sBadges;
                    }
                }
            }
        }

    }
}