using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Controls;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class EventAddWizard : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4500;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Add Event");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                Session["EID"] = string.Empty;
                lblPK.Text = Session["EID"] == null ? "" : Session["EID"].ToString();
                Page.DataBind();
            }
        }

        public void CancelWizard()
        {
            string returnURL = "~/ControlRoom/Modules/Setup/EventList.aspx";
            Response.Redirect(returnURL);
        }

        public void DeleteTemporaryEvent()
        {
            try
            {
                var e = Event.GetEvent(int.Parse(lblPK.Text));
                e.Delete();
                var st = new SessionTools(Session);
                st.RemoveCache(Cache, CacheKey.EventsActive);
                st.RemoveCache(Cache, CacheKey.AllEvents);
            }
            catch { }
        }

        public void DeleteTemporaryBadge()
        {
            try
            {
                var e = Event.GetEvent(int.Parse(lblPK.Text));
                Badge.Delete(Badge.GetBadge(e.BadgeID));
                e.BadgeID = 0;
                e.Update();
            }
            catch { }
        }


        public void DeleteTemporaryEventAndBadge()
        {
            try
            {
                var e = Event.GetEvent(int.Parse(lblPK.Text));
                Badge.Delete(Badge.GetBadge(e.BadgeID));
                e.Delete();
            }
            catch { }
        }

        protected void btnBack_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            CancelWizard();
        }

        protected void btnContinue_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            pnlEvent.Visible = false;
            pnlReward.Visible = true;
            if (string.IsNullOrEmpty(AdminName.Text))
            {
                AdminName.Text = UserName.Text = string.Format("{0} Badge", EventTitle.Text);
            }
            if (BranchId.SelectedIndex > 0)
            {
                FilterBranchId.SelectedIndex = BranchId.SelectedIndex;
            }
            LoadBadgeSearch();
        }

        protected void btnCancel2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            CancelWizard();
        }

        protected void btnPrevious2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            pnlEvent.Visible = true;
            pnlReward.Visible = false;
        }

        private Event LoadEventObject()
        {
            var eventObj = new Event();

            eventObj.EventTitle = EventTitle.Text.Trim();
            eventObj.EventDate = FormatHelper.SafeToDateTime(EventDate.Text);
            //eventObj.EventTime = EventTime.Text;
            eventObj.HTML = HTML.InnerHtml.Trim();
            eventObj.SecretCode = SecretCode.Text.Trim().ToLower();
            eventObj.NumberPoints = NumberPoints.Text.SafeToInt();
            eventObj.BadgeID = BadgeID.SelectedValue.SafeToInt();
            eventObj.BranchID = BranchId.Text.SafeToInt();
            eventObj.Custom1 = Custom1.Value;
            eventObj.Custom2 = Custom2.Value;
            eventObj.Custom3 = Custom3.Value;

            //eventObj.ShortDescription = ShortDescription.Text;

            //eventObj.EndDate = EndDate.Text.SafeToDateTime();
            //eventObj.EndTime = EndTime.Text;

            eventObj.ExternalLinkToEvent = ExternalLinkToEvent.Text;
            eventObj.HiddenFromPublic = HiddenFromPublic.SelectedIndex > 0;

            eventObj.AddedDate = DateTime.Now;
            eventObj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
            eventObj.LastModDate = eventObj.AddedDate;
            eventObj.LastModUser = eventObj.AddedUser;

            return eventObj;
        }

        private Badge LoadBadgeObject()
        {
            var badge = new Badge();
            badge.AdminName = AdminName.Text.Trim();
            badge.UserName = UserName.Text.Trim();
            badge.CustomEarnedMessage = CustomEarnedMessage.InnerHtml;

            badge.IncludesPhysicalPrizeFlag = IncludesPhysicalPrizeFlag.Checked;
            badge.PhysicalPrizeName = PhysicalPrizeName.Text;

            badge.GenNotificationFlag = GenNotificationFlag.Checked;
            badge.NotificationSubject = NotificationSubject.Text.Trim();
            badge.NotificationBody = NotificationBody.InnerHtml;

            badge.AssignProgramPrizeCode = AssignProgramPrizeCode.Checked;
            badge.PCNotificationSubject = PCNotificationSubject.Text.Trim();
            badge.PCNotificationBody = PCNotificationBody.InnerHtml;

            badge.AddedDate = DateTime.Now;
            badge.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
            badge.LastModDate = badge.AddedDate;
            badge.LastModUser = badge.AddedUser;

            return badge;
        }

        protected void btnContinue2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            var eventObj = LoadEventObject();

            if (!eventObj.IsValid(BusinessRulesValidationMode.INSERT))
            {
                var masterPage = (IControlRoomMaster)Master;
                string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                foreach (BusinessRulesValidationMessage m in eventObj.ErrorCodes)
                {
                    message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                }
                message = string.Format("{0}</ul>", message);
                masterPage.PageError = message;
            }
            else
            {
                if (rblBadge.SelectedIndex == 0)
                {
                    // No Badge Awarded

                    eventObj.Insert();
                    var st = new SessionTools(Session);
                    st.RemoveCache(Cache, CacheKey.EventsActive);
                    st.RemoveCache(Cache, CacheKey.AllEvents);
                    Session["EID"] = eventObj.EID;
                    Response.Redirect("EventAddEdit.aspx?M=K");
                }
                if (rblBadge.SelectedIndex == 1)
                {
                    // Existing Badge Awarded
                    eventObj.BadgeID = int.Parse(BadgeID.SelectedValue);
                    eventObj.Insert();
                    var st = new SessionTools(Session);
                    st.RemoveCache(Cache, CacheKey.EventsActive);
                    st.RemoveCache(Cache, CacheKey.AllEvents);
                    Session["EID"] = eventObj.EID;
                    Response.Redirect("EventAddEdit.aspx?M=K");
                }
                if (rblBadge.SelectedIndex == 2)
                {
                    // Start creation of new badge
                    eventObj.Insert();
                    var st = new SessionTools(Session);
                    st.RemoveCache(Cache, CacheKey.EventsActive);
                    st.RemoveCache(Cache, CacheKey.AllEvents);
                    lblPK.Text = eventObj.EID.ToString();

                    pnlBadgeMore.Visible = true;
                    pnlReward.Visible = false;

                    foreach (GridViewRow row in gvBranch.Rows)
                    {
                        Label idLabel = row.FindControl("CID") as Label;
                        if (idLabel != null && idLabel.Text == BranchId.SelectedValue)
                        {
                            var check = row.FindControl("isMember") as CheckBox;
                            if (check != null)
                            {
                                check.Checked = true;
                            }
                        }
                    }

                    btnContinue3_Click(sender, e);
                }
                new SessionTools(Session).RemoveCache(Cache, CacheKey.EventsActive);
            }

        }

        protected void rblBadge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblBadge.SelectedIndex == 0)
            {
                ExistingBadgeSelection.Visible = false;
                pnlBadge.Visible = false;
                rfvAdminName.Enabled = rfvUserName.Enabled = false;
                return;
            }
            if (rblBadge.SelectedIndex == 1)
            {
                ExistingBadgeSelection.Visible = true;
                pnlBadge.Visible = false;
                rfvAdminName.Enabled = rfvUserName.Enabled = false;
                return;
            }
            if (rblBadge.SelectedIndex == 2)
            {
                ExistingBadgeSelection.Visible = false;
                pnlBadge.Visible = true;
                rfvAdminName.Enabled = rfvUserName.Enabled = true;
                return;
            }
            ExistingBadgeSelection.Visible = false;
            pnlBadge.Visible = false;
            rfvAdminName.Enabled = rfvUserName.Enabled = false;
        }

        protected void btnCancel3_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DeleteTemporaryEvent();
            CancelWizard();
        }

        protected void btnPrevious3_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DeleteTemporaryEvent();
            pnlBadgeMore.Visible = false;
            pnlReward.Visible = true;
        }

        protected void btnContinue3_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            var evt = Event.GetEvent(int.Parse(lblPK.Text));
            var badge = LoadBadgeObject();

            if (evt.HiddenFromPublic == true)
            {
                badge.HiddenFromPublic = true;
            }

            badge.Insert();

            try
            {
                var badgePath = string.Format(Server.MapPath("~/images/Badges/"));
                System.IO.File.Copy(string.Format("{0}no_badge.png", badgePath),
                                    string.Format("{0}{1}.png", badgePath, badge.BID));
                System.IO.File.Copy(string.Format("{0}no_badge_sm.png", badgePath),
                                    string.Format("{0}sm_{1}.png", badgePath, badge.BID));
            }
            catch (Exception ex)
            {
                this.Log().Error("Couldn't copy no_badge images into new badge: {0}",
                                 ex.Message);
            }

            new SessionTools(Session).RemoveCache(Cache, CacheKey.BadgesActive);
            lblBID.Text = badge.BID.ToString();
            evt.BadgeID = badge.BID;
            evt.Update();
            FileUploadCtl.FileName = lblBID.Text;
            FileUploadCtl.ProcessRender();
            OpenBadgesBadgeMaker.FileName = lblBID.Text;

            pnlLast.Visible = true;
            pnlBadgeMore.Visible = false;
        }

        protected void btnCancel4_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DeleteTemporaryEventAndBadge();
            CancelWizard();
        }

        protected void btnPrevious4_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DeleteTemporaryBadge();
            pnlLast.Visible = false;
            pnlBadgeMore.Visible = true;
            btnPrevious3_Click(sender, e);
        }

        protected void btnContinue4_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            var obj = Badge.FetchObject(int.Parse(lblBID.Text));
            SaveBadgeExtendedAttributes(obj, gvCat, gvAge, gvBranch, gvLoc);

            if (!FileUploadCtl.FileExists())
            {
                var masterPage = (IControlRoomMaster)Master;
                string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                message = string.Format(String.Format("{0}<li>{{0}}</li>", message), "The badge image is mandatory.  Please upload one.");

                message = string.Format("{0}</ul>", message);
                masterPage.PageError = message;
                masterPage.DisplayMessageOnLoad = true;
            }
            else
            {
                Session["EID"] = int.Parse(lblPK.Text);
                Response.Redirect("EventAddEdit.aspx?M=K");
            }
        }

        public void SaveBadgeExtendedAttributes(Badge obj, GridView gv1, GridView gv2, GridView gv3, GridView gv4)
        {
            var gv = gv1;
            string checkedMembers = string.Empty;
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if (checkedMembers.Length > 0)
                checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeCategories(checkedMembers);

            gv = gv2;
            checkedMembers = string.Empty;
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if (checkedMembers.Length > 0)
                checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeAgeGroups(checkedMembers);


            gv = gv3;
            checkedMembers = string.Empty;
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if (checkedMembers.Length > 0)
                checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeBranches(checkedMembers);

            gv = gv4;
            checkedMembers = string.Empty;
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if (checkedMembers.Length > 0)
                checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeLocations(checkedMembers);
        }

        protected void LoadBadgeSearch()
        {
            odsBadge.Select();
            BadgeID.Items.Clear();
            BadgeID.Items.Add(new ListItem("[Select a Badge]", "0"));
            BadgeID.DataBind();
        }

        protected void Search(object sender, EventArgs e)
        {
            LoadBadgeSearch();
        }
    }
}

