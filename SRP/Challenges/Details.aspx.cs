using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
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
                return 100;
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
            IsSecure = true;
            if(!IsPostBack) {
                if(Request.QueryString["blid"] != null) {
                    int blid;
                    if(int.TryParse(Request.QueryString["blid"].ToString(), out blid)) {
                        LookupChallenge(blid);
                    }

                }
                if(Request.UrlReferrer == null) {
                    challengesBackLink.NavigateUrl = "~/Challenges/";
                } else {
                    challengesBackLink.NavigateUrl = Request.UrlReferrer.AbsolutePath;
                }
                TranslateStrings(this);
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

        protected void LookupChallenge(int blid) {
            var bl = BookList.FetchObject(blid);

            if(bl == null) {
                challengeDetails.Visible = false;
                new SessionTools(Session).AlertPatron("Could not find details on that Challenge.",
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
            } else {

                challengeTitle.Text = bl.ListName;
                this.Title = string.Format("{0} Challenge", challengeTitle.Text);
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
                var ds = BookListBooks.GetForDisplay(bl.BLID, ((Patron)Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();

                this.ShowModal = true;
            }
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

            new SessionTools(Session).AlertPatron("Your progress has been saved!",
                glyphicon: "check");

            // read the entire book list!  Award points and badges 
            if((neeedCount == 0 && onlyCheckedBoxes) || (neeedCount <= readCount)) {
                new SessionTools(Session).AlertPatron("Good work, you've completed a Challenge!",
                    glyphicon: "star");

                var bl = BookList.FetchObject(selBLI);

                if(PatronPoints.HasEarnedBookList(((Patron)Session["Patron"]).PID, selBLI)) {
                    return;
                }

                if(bl.AwardBadgeID != 0 || bl.AwardPoints != 0) {
                    new SessionTools(Session).AlertPatron("Congratulations, you completed a Challenge and were awarded a badge!",
                        glyphicon: "certificate");

                    var pa = new AwardPoints(((Patron)Session["Patron"]).PID);
                    var sBadges = pa.AwardPointsToPatron(bl.AwardPoints, PointAwardReason.BookListCompletion,
                                                            bookListID: bl.BLID);
                    if(sBadges.Length > 0) {
                        new SessionTools(Session).EarnedBadges(sBadges);
                    }
                }
            }
        }

    }
}