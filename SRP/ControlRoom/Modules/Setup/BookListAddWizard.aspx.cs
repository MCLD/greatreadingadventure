using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Controls;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;

namespace GRA.SRP.ControlRoom.Modules.Setup {
    public partial class BookListAddWizard : BaseControlRoomPage {


        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.RequiredPermission = 4400;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Add Challenge");

            if(!IsPostBack) {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                Session["BLL"]= string.Empty;
                lblPK.Text = Session["BLL"] == null ? "" : Session["BLL"].ToString();
                Page.DataBind();
            }
        }

        public void CancelWizard() {
            string returnURL = "~/ControlRoom/Modules/Setup/BookListList.aspx";
            Response.Redirect(returnURL);
        }

        public void DeleteTemporaryBL() {
            try {
                var e = BookList.FetchObject(int.Parse(lblPK.Text));
                e.Delete();
            } catch { }
        }

        public void DeleteTemporaryBadge() {
            try {
                var e = BookList.FetchObject(int.Parse(lblPK.Text));
                Badge.Delete(Badge.GetBadge(e.AwardBadgeID));
                e.AwardBadgeID = 0;
                e.Update();
            } catch { }
        }


        public void DeleteTemporaryBLAndBadge() {
            try {
                var e = BookList.FetchObject(int.Parse(lblPK.Text));
                Badge.Delete(Badge.GetBadge(e.AwardBadgeID));
                e.Delete();
            } catch { }
        }

        protected void btnBack_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            CancelWizard();
        }

        protected void btnContinue_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            pnlBookList.Visible = false;
            pnlReward.Visible = true;
        }

        protected void btnCancel2_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            CancelWizard();
        }

        protected void btnPrevious2_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            pnlBookList.Visible = true;
            pnlReward.Visible = false;
        }

        private BookList LoadBookListObject() {
            var obj = new BookList();

            obj.AdminName = BLAdminName.Text;
            obj.ListName = ListName.Text;
            obj.AdminDescription = AdminDescription.Text;
            obj.Description = Description.InnerHtml;
            obj.LiteracyLevel1 = LiteracyLevel1.Text.SafeToInt();
            obj.LiteracyLevel2 = LiteracyLevel2.Text.SafeToInt();
            obj.ProgID = ProgID.SelectedValue.SafeToInt();
            obj.LibraryID = LibraryID.SelectedValue.SafeToInt();
            obj.AwardBadgeID = BadgeID.Text.SafeToInt();
            obj.AwardPoints = AwardPoints.Text.SafeToInt();

            obj.NumBooksToComplete = NumBooksToComplete.Text.SafeToInt();

            obj.AddedDate = DateTime.Now;
            obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
            obj.LastModDate = obj.AddedDate;
            obj.LastModUser = obj.AddedUser;

            return obj;
        }

        private Badge LoadBadgeObject() {
            var obj = new Badge();
            obj.AdminName = AdminName.Text;
            obj.UserName = UserName.Text;
            obj.CustomEarnedMessage = CustomEarnedMessage.InnerHtml;

            obj.IncludesPhysicalPrizeFlag = IncludesPhysicalPrizeFlag.Checked;
            obj.PhysicalPrizeName = PhysicalPrizeName.Text;

            obj.GenNotificationFlag = GenNotificationFlag.Checked;
            obj.NotificationSubject = NotificationSubject.Text;
            obj.NotificationBody = NotificationBody.InnerHtml;

            obj.AssignProgramPrizeCode = AssignProgramPrizeCode.Checked;
            obj.PCNotificationSubject = PCNotificationSubject.Text;
            obj.PCNotificationBody = PCNotificationBody.InnerHtml;

            obj.AddedDate = DateTime.Now;
            obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
            obj.LastModDate = obj.AddedDate;
            obj.LastModUser = obj.AddedUser;

            return obj;
        }

        protected void btnContinue2_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            var obj = LoadBookListObject();

            if(!obj.IsValid(BusinessRulesValidationMode.INSERT)) {
                var masterPage = (IControlRoomMaster)Master;
                string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                foreach(BusinessRulesValidationMessage m in obj.ErrorCodes) {
                    message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                }
                message = string.Format("{0}</ul>", message);
                masterPage.PageError = message;
            } else {
                if(rblBadge.SelectedIndex == 0) {
                    // No Badge Awarded

                    obj.Insert();
                    Session["BLL"] = obj.BLID;
                    Response.Redirect("BookListAddEdit.aspx?M=K");
                }
                if(rblBadge.SelectedIndex == 1) {
                    // Existing Badge Awarded
                    obj.AwardBadgeID = int.Parse(BadgeID.SelectedValue);
                    obj.Insert();
                    Session["BLL"] = obj.BLID;
                    Response.Redirect("BookListAddEdit.aspx?M=K");
                }
                if(rblBadge.SelectedIndex == 2) {
                    // Start creation of new badge
                    obj.Insert();
                    lblPK.Text = obj.BLID.ToString();

                    pnlBadgeMore.Visible = true;
                    pnlReward.Visible = false;
                }
                Cache[CacheKey.ChallengesActive] = true;

            }

        }

        protected void rblBadge_SelectedIndexChanged(object sender, EventArgs e) {
            if(rblBadge.SelectedIndex == 0) {
                BadgeID.Visible = false;
                pnlBadge.Visible = false;
                rfvAdminName.Enabled = rfvUserName.Enabled = false;
                return;
            }
            if(rblBadge.SelectedIndex == 1) {
                BadgeID.Visible = true;
                pnlBadge.Visible = false;
                rfvAdminName.Enabled = rfvUserName.Enabled = false;
                return;
            }
            if(rblBadge.SelectedIndex == 2) {
                BadgeID.Visible = false;
                pnlBadge.Visible = true;
                rfvAdminName.Enabled = rfvUserName.Enabled = true;
                return;
            }
            BadgeID.Visible = false;
            pnlBadge.Visible = false;
            rfvAdminName.Enabled = rfvUserName.Enabled = false;
        }


        protected void btnCancel3_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            DeleteTemporaryBL();
            CancelWizard();
        }

        protected void btnPrevious3_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            DeleteTemporaryBL();
            pnlBadgeMore.Visible = false;
            pnlReward.Visible = true;
        }

        protected void btnContinue3_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            var obj = LoadBadgeObject();
            obj.Insert();
            Cache[CacheKey.BadgesActive] = true;
            lblBID.Text = obj.BID.ToString();
            var bl = BookList.FetchObject(int.Parse(lblPK.Text));
            bl.AwardBadgeID = obj.BID;
            bl.Update();
            FileUploadCtl.FileName = lblBID.Text;
            FileUploadCtl.ProcessRender();

            pnlLast.Visible = true;
            pnlBadgeMore.Visible = false;
        }

        protected void btnCancel4_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            DeleteTemporaryBLAndBadge();
            CancelWizard();
        }

        protected void btnPrevious4_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            DeleteTemporaryBadge();
            pnlLast.Visible = false;
            pnlBadgeMore.Visible = true;
        }

        protected void btnContinue4_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            var obj = Badge.FetchObject(int.Parse(lblBID.Text));
            SaveBadgeExtendedAttributes(obj, gvCat, gvAge, gvBranch, gvLoc);

            if(!FileUploadCtl.FileExists()) {
                var masterPage = (IControlRoomMaster)Master;
                string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                message = string.Format(String.Format("{0}<li>{{0}}</li>", message), "The badge image is mandatory.  Please upload one.");

                message = string.Format("{0}</ul>", message);
                masterPage.PageError = message;
                masterPage.DisplayMessageOnLoad = true;
            } else {
                Session["BLL"] = int.Parse(lblPK.Text);
                Response.Redirect("BookListAddEdit.aspx?M=K");
            }
        }

        public void SaveBadgeExtendedAttributes(Badge obj, GridView gv1, GridView gv2, GridView gv3, GridView gv4) {
            var gv = gv1;
            string checkedMembers= string.Empty;
            foreach(GridViewRow row in gv.Rows) {
                if(((CheckBox)row.FindControl("isMember")).Checked) {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if(checkedMembers.Length > 0)
                checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeCategories(checkedMembers);

            gv = gv2;
            checkedMembers= string.Empty;
            foreach(GridViewRow row in gv.Rows) {
                if(((CheckBox)row.FindControl("isMember")).Checked) {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if(checkedMembers.Length > 0)
                checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeAgeGroups(checkedMembers);


            gv = gv3;
            checkedMembers= string.Empty;
            foreach(GridViewRow row in gv.Rows) {
                if(((CheckBox)row.FindControl("isMember")).Checked) {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if(checkedMembers.Length > 0)
                checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeBranches(checkedMembers);

            gv = gv4;
            checkedMembers= string.Empty;
            foreach(GridViewRow row in gv.Rows) {
                if(((CheckBox)row.FindControl("isMember")).Checked) {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if(checkedMembers.Length > 0)
                checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeLocations(checkedMembers);
        }

    }
}

