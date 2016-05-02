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

namespace GRA.SRP.Controls
{
    public partial class ChallengesCtl : System.Web.UI.UserControl
    {
        private const string CompletedCommand = "ShowCompleted";
        private const string DisplayCommand = "Show";

        private bool ProgramOpen { get; set; }
        private bool CompletedChallenge { get; set; }
        private bool Filtered { get; set; }
        private readonly string BadgeLink = VirtualPathUtility.ToAbsolute("~/Badges/Details.aspx");
        private const string BadgeLinkAndImage = "<a href=\"{0}?BadgeId={1}\" runat=\"server\" OnClick=\"return ShowBadgeInfo({1});\" class=\"thumbnail pull-left\"><img src=\"/images/badges/sm_{1}.png\" /></a>";
        public bool ShowModal { get; set; }
        public Patron CurrentPatron { get; set; }

        protected string ShowBadge(object badgeIdObject)
        {
            int? badgeId = badgeIdObject as int?;
            if (badgeId == null || badgeId == 0)
            {
                return "No badge.";
            }
            else
            {
                var badge = DAL.Badge.FetchObject((int)badgeId);
                if (badge == null)
                {
                    return "No badge.";
                }
                if (badge.HiddenFromPublic != true)
                {
                    return string.Format(BadgeLinkAndImage, BadgeLink, badgeId);
                }
                else
                {
                    return "Unknown <small>(it's a secret)</small>.";
                }
            }
        }

        protected string PopupCommand(object amountObject, object totalObject)
        {
            int amount = amountObject as int? ?? 0;
            int total = totalObject as int? ?? 0;

            if (total == 0 || amount == 0)
            {
                return DisplayCommand;
            }
            if (amount >= total)
            {
                return CompletedCommand;
            }
            return DisplayCommand;
        }

        protected string ProgressDisplay(object amountObject, object totalObject)
        {
            int amount = amountObject as int? ?? 0;
            int total = totalObject as int? ?? 0;

            if (total == 0 || amount == 0)
            {
                return string.Empty;
            }
            if (amount >= total)
            {
                return "Challenge complete!";
            }

            return string.Format("{0} of {1}", amountObject, totalObject);
        }

        protected int ComputePercent(object amountObject, object totalObject)
        {
            int amount = amountObject as int? ?? 0;
            int total = totalObject as int? ?? 0;

            if (total == 0)
            {
                return 0;
            }
            else
            {
                if (amount > total)
                {
                    return 100;
                }
                return (int)(amount * 100.0 / total);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Programs pgm = null;
            this.CurrentPatron = Session[SessionKey.Patron] as Patron;

            if (this.CurrentPatron != null)
            {
                pgm = DAL.Programs.FetchObject(this.CurrentPatron.ProgID);
                if (pgm == null)
                {
                    pgm = Programs.FetchObject(
                        Programs.GetDefaultProgramForAgeAndGrade(this.CurrentPatron.Age,
                            this.CurrentPatron.SchoolGrade.SafeToInt()));
                }
            }

            if (pgm == null || !pgm.IsOpen)
            {
                this.ProgramOpen = false;
                btnSave.Visible = false;
            }
            else
            {
                this.ProgramOpen = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Count > 0)
                {
                    var querySearch = Request.QueryString["Search"];
                    if (!string.IsNullOrWhiteSpace(querySearch))
                    {
                        if (querySearch.Length > 255)
                        {
                            SearchText.Text = querySearch.Substring(0, 255);
                        }
                        else
                        {
                            SearchText.Text = querySearch;
                        }
                    }
                }
                PopulateChallengeList();
                this.ShowModal = false;
            }
        }

        protected void PopulateChallengeList()
        {
            int patronId = -1;
            if (this.CurrentPatron != null)
            {
                patronId = this.CurrentPatron.PID;
            }
            Filtered = !string.IsNullOrWhiteSpace(SearchText.Text);
            var ds = DAL.BookList.GetForDisplay(patronId, SearchText.Text);
            rptr.DataSource = ds;
            rptr.DataBind();
            var wt = new WebTools();
            if (ds.Tables[0].Rows.Count == 0)
            {
                ChallengesContainer.Visible = false;
                if (Filtered)
                {
                    WhatsShowing.Text
                        = WhatsShowingPrint.Text
                        = StringResources.getString("challenges-no-match");
                    WhatsShowingPanel.Visible = true;
                    WhatsShowingPanel.CssClass = wt.CssRemoveClass(WhatsShowingPanel.CssClass,
                        "alert-success");
                    WhatsShowingPanel.CssClass = wt.CssEnsureClass(WhatsShowingPanel.CssClass,
                        "alert-warning");
                }
                else
                {
                    WhatsShowing.Text
                        = WhatsShowingPrint.Text
                        = StringResources.getString("challenges-no-challenges");
                    WhatsShowingPanel.Visible = true;
                    WhatsShowingPanel.CssClass = wt.CssRemoveClass(WhatsShowingPanel.CssClass,
                        "alert-success");
                    WhatsShowingPanel.CssClass = wt.CssEnsureClass(WhatsShowingPanel.CssClass,
                        "alert-warning");
                }
            }
            else
            {
                ChallengesContainer.Visible = true;
                if (Filtered)
                {
                    WhatsShowing.Text
                        = WhatsShowingPrint.Text
                        = string.Format("You searched for: <strong>{0}</strong>", SearchText.Text);
                    WhatsShowingPanel.Visible = true;
                    WhatsShowingPanel.CssClass
                        = wt.CssRemoveClass(WhatsShowingPanel.CssClass, "alert-warning");
                    WhatsShowingPanel.CssClass
                        = wt.CssEnsureClass(WhatsShowingPanel.CssClass, "alert-success");
                }
                else
                {
                    WhatsShowing.Text
                        = WhatsShowingPrint.Text
                        = string.Empty;
                    WhatsShowingPanel.Visible = false;
                }
            }
            if (Filtered)
            {
                SearchText.CssClass
                    = wt.CssEnsureClass(SearchText.CssClass, "gra-search-active");
            }
            else
            {
                SearchText.CssClass
                    = wt.CssRemoveClass(SearchText.CssClass, "gra-search-active");
            }
        }

        protected void rptr_ItemDataBound(object source, RepeaterItemEventArgs e)
        {
            if (!this.ProgramOpen
                || this.CompletedChallenge)
            {
                if (e.Item.ItemType == ListItemType.Item
                   || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    var checkbox = e.Item.FindControl("chkRead") as CheckBox;
                    if (checkbox != null)
                    {
                        checkbox.Enabled = false;
                    }

                }
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var BLID = int.Parse(e.CommandArgument.ToString());

            this.CompletedChallenge = e.CommandName.Equals(CompletedCommand,
                StringComparison.OrdinalIgnoreCase);

            var bl = BookList.FetchObject(BLID);

            lblTitle.Text = bl.ListName;
            lblDesc.Text = Server.HtmlDecode(bl.Description);

            string award = null;

            if (this.CompletedChallenge)
            {
                //completed 
                award = string.Format("<span class=\"text-success lead\">Congratulations, you completed this challenge!</span>");
            }
            else
            {
                // not yet completed
                if (bl.AwardPoints > 0)
                {
                    award = string.Format("Completing <strong>{0} task{1}</strong> will earn: <strong>{2} point{3}</strong>",
                        bl.NumBooksToComplete,
                        bl.NumBooksToComplete > 1 ? "s" : string.Empty,
                        bl.AwardPoints,
                        bl.AwardPoints > 1 ? "s" : string.Empty);
                }

                if (bl.AwardBadgeID > 0)
                {
                    var badge = DAL.Badge.FetchObject(bl.AwardBadgeID);
                    if (badge != null)
                    {
                        if (badge.HiddenFromPublic != true)
                        {
                            if (string.IsNullOrWhiteSpace(award))
                            {
                                award = string.Format("Completing {0} task{1} will earn: <strong>a badge</strong>.",
                                    bl.NumBooksToComplete,
                                    bl.NumBooksToComplete > 1 ? "s" : string.Empty);
                            }
                            else
                            {
                                award += " and <strong>a badge</strong>.";
                            }

                            BadgeImage.Text = string.Format("<img class=\"thumbnail disabled\" src=\"/images/badges/sm_{0}.png\" />", bl.AwardBadgeID);
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(award))
                            {
                                award = string.Format("Completing {0} task{1} will earn: <strong>a secret badge</strong>.",
                                    bl.NumBooksToComplete,
                                    bl.NumBooksToComplete > 1 ? "s" : string.Empty);
                            }
                            else
                            {
                                award += " and <strong>a secret badge</strong>.";
                            }
                            BadgeImage.Text = string.Empty;
                        }
                    }
                }
            }

            BadgeImage.Visible = !string.IsNullOrEmpty(BadgeImage.Text);

            if (!string.IsNullOrWhiteSpace(award))
            {
                lblPoints.Text = award;
                lblPoints.Visible = true;
            }

            lblTitle.Visible = true;
            lblDesc.Visible = true;

            int patronId = -1;
            if (this.CurrentPatron != null)
            {
                patronId = this.CurrentPatron.PID;
            }

            var ds = BookListBooks.GetForDisplay(bl.BLID, patronId);
            rptr2.DataSource = ds;
            rptr2.DataBind();
            printLink.NavigateUrl = string.Format("~/Challenges/Details.aspx?ChallengeId={0}&print=1",
                bl.BLID);
            detailsLink.NavigateUrl = string.Format("~/Challenges/Details.aspx?ChallengeId={0}",
                bl.BLID);
            pnlDetail.Visible = true;

            if (this.CompletedChallenge
                || this.CurrentPatron == null
                || this.ProgramOpen == false)
            {
                btnSave.Visible = false;
            }
            else
            {
                btnSave.Visible = true;
            }

            this.ShowModal = true;
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            pnlDetail.Visible = false;
            this.ShowModal = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ProgramOpen)
            {
                return;
            }

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
                    pbl.PID = this.CurrentPatron.PID;
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

            string success = StringResources.getString("challenges-progress-saved");
            new SessionTools(Session).AlertPatron(success,
                glyphicon: "check");

            // read the entire book list!  Award points and badges 
            if ((neeedCount == 0 && onlyCheckedBoxes) || (neeedCount <= readCount))
            {
                success = StringResources.getString("challenges-completed");

                new SessionTools(Session).AlertPatron(success, glyphicon: "star");

                var bl = BookList.FetchObject(selBLI);

                if (PatronPoints.HasEarnedBookList(this.CurrentPatron.PID, selBLI))
                {
                    PopulateChallengeList();
                    return;
                }

                if (bl.AwardBadgeID != 0 || bl.AwardPoints != 0)
                {
                    success = StringResources.getString("challenges-completed-badge");
                    new SessionTools(Session).AlertPatron(success,
                        glyphicon: "certificate");

                    var pa = new AwardPoints(this.CurrentPatron.PID);
                    var sBadges = pa.AwardPointsToPatron(bl.AwardPoints, PointAwardReason.BookListCompletion,
                                                            bookListID: bl.BLID);
                    if (sBadges.Length > 0)
                    {
                        new SessionTools(Session).EarnedBadges(sBadges);
                    }
                }
            }
            PopulateChallengeList();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            PopulateChallengeList();
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            SearchText.Text = string.Empty;
            PopulateChallengeList();
        }

    }
}