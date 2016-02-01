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
    public partial class EventAddWizard : BaseControlRoomPage {


        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.RequiredPermission = 4500;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Add Event");

            if(!IsPostBack) {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                Session["EID"]= string.Empty;
                lblPK.Text = Session["EID"] == null ? "" : Session["EID"].ToString();
                Page.DataBind();
            }
        }

        public string CheckDups(string Code, int EID) {
            string retVal= string.Empty;

            if(Event.GetEventCountByEventCode(EID, Code) != 0) {
                return "<font color=red><b><br/>This secret code is not unique.</b></font>";
            }


            return retVal;
        }


        public void CancelWizard() {
            string returnURL = "~/ControlRoom/Modules/Setup/EventList.aspx";
            Response.Redirect(returnURL);
        }

        public void DeleteTemporaryEvent() {
            try {
                var e = Event.GetEvent(int.Parse(lblPK.Text));
                e.Delete();
            } catch { }
        }

        public void DeleteTemporaryBadge() {
            try {
                var e = Event.GetEvent(int.Parse(lblPK.Text));
                Badge.Delete(Badge.GetBadge(e.BadgeID));
                e.BadgeID = 0;
                e.Update();
            } catch { }
        }


        public void DeleteTemporaryEventAndBadge() {
            try {
                var e = Event.GetEvent(int.Parse(lblPK.Text));
                Badge.Delete(Badge.GetBadge(e.BadgeID));
                e.Delete();
            } catch { }
        }

        protected void btnBack_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            CancelWizard();
        }

        protected void btnContinue_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            pnlEvent.Visible = false;
            pnlReward.Visible = true;
        }

        protected void btnCancel2_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            CancelWizard();
        }

        protected void btnPrevious2_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            pnlEvent.Visible = true;
            pnlReward.Visible = false;
        }

        private Event LoadEventObject() {
            var obj = new Event();

            obj.EventTitle = EventTitle.Text;
            obj.EventDate = FormatHelper.SafeToDateTime(EventDate.Text);
            obj.EventTime = EventTime.Text;
            obj.HTML = HTML.InnerHtml;
            obj.SecretCode = SecretCode.Text;
            obj.NumberPoints = NumberPoints.Text.SafeToInt();
            obj.BadgeID = BadgeID.SelectedValue.SafeToInt();
            obj.BranchID = BranchId.Text.SafeToInt();
            obj.Custom1 = Custom1.Value;
            obj.Custom2 = Custom2.Value;
            obj.Custom3 = Custom3.Value;

            obj.ShortDescription = ShortDescription.Text;
            obj.EndDate = EndDate.Text.SafeToDateTime();
            obj.EndTime = EndTime.Text;

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
            var obj = LoadEventObject();

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
                    Session["EID"] = obj.EID;
                    Response.Redirect("EventAddEdit.aspx?M=K");
                }
                if(rblBadge.SelectedIndex == 1) {
                    // Existing Badge Awarded
                    obj.BadgeID = int.Parse(BadgeID.SelectedValue);
                    obj.Insert();
                    Session["EID"] = obj.EID;
                    Response.Redirect("EventAddEdit.aspx?M=K");
                }
                if(rblBadge.SelectedIndex == 2) {
                    // Start creation of new badge
                    obj.Insert();
                    lblPK.Text = obj.EID.ToString();

                    pnlBadgeMore.Visible = true;
                    pnlReward.Visible = false;
                }
                Cache[CacheKey.EventsActive] = true;
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

        protected void SecretCode_TextChanged(object sender, EventArgs e) {
            lblDups.Text= string.Empty;
            if(SecretCode.Text == "")
                return;
            var err = CheckDups(SecretCode.Text, 0);
            if(err.Length > 0) {
                lblDups.Text = err + "<br/>";
            }
        }

        protected void btnCancel3_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            DeleteTemporaryEvent();
            CancelWizard();
        }

        protected void btnPrevious3_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            DeleteTemporaryEvent();
            pnlBadgeMore.Visible = false;
            pnlReward.Visible = true;
        }

        protected void btnContinue3_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            var obj = LoadBadgeObject();
            obj.Insert();
            Cache[CacheKey.BadgesActive] = true;
            lblBID.Text = obj.BID.ToString();
            var evt = Event.GetEvent(int.Parse(lblPK.Text));
            evt.BadgeID = obj.BID;
            evt.Update();
            FileUploadCtl.FileName = lblBID.Text;
            FileUploadCtl.ProcessRender();

            pnlLast.Visible = true;
            pnlBadgeMore.Visible = false;
        }

        protected void btnCancel4_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            DeleteTemporaryEventAndBadge();
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
                Session["EID"] = int.Parse(lblPK.Text);
                Response.Redirect("EventAddEdit.aspx?M=K");
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

        //protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        //{

        //    if (e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback")
        //    {
        //        try
        //        {
        //            var obj = new Event();
        //            //obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("GenNotificationFlag")).Checked;

        //            obj.EventTitle = ((TextBox)((DetailsView)sender).FindControl("EventTitle")).Text;
        //            obj.EventDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("EventDate")).Text);
        //            obj.EventTime = ((TextBox)((DetailsView)sender).FindControl("EventTime")).Text;
        //            obj.HTML = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML")).Text;
        //            obj.SecretCode = ((TextBox)((DetailsView)sender).FindControl("SecretCode")).Text;
        //            obj.NumberPoints =  FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumberPoints")).Text);
        //            obj.BadgeID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BadgeID")).SelectedValue);
        //            obj.BranchID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BranchID")).SelectedValue);
        //            obj.Custom1 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom1")).Value;
        //            obj.Custom2 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom2")).Value;
        //            obj.Custom3 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom3")).Value;

        //            obj.ShortDescription = ((TextBox)((DetailsView)sender).FindControl("ShortDescription")).Text;
        //            obj.EndDate = ((TextBox)((DetailsView)sender).FindControl("EndDate")).Text.SafeToDateTime();
        //            obj.EndTime = ((TextBox)((DetailsView)sender).FindControl("EndTime")).Text;

        //            obj.AddedDate = DateTime.Now;
        //            obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
        //            obj.LastModDate = obj.AddedDate;
        //            obj.LastModUser = obj.AddedUser;

        //            if (obj.IsValid(BusinessRulesValidationMode.INSERT))
        //            {
        //                obj.Insert();
        //                if (e.CommandName.ToLower() == "addandback")
        //                {
        //                    Response.Redirect(returnURL);
        //                }

        //                lblPK.Text = obj.EID.ToString();

        //                odsData.DataBind();
        //                dv.DataBind();
        //                dv.ChangeMode(DetailsViewMode.Edit);

        //                var masterPage = (IControlRoomMaster)Master;
        //                masterPage.PageMessage = SRPResources.AddedOK;
        //            }
        //            else
        //            {
        //                var masterPage = (IControlRoomMaster)Master;
        //                string message = String.Format(SRPResources.ApplicationError1, "<ul>");
        //                foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
        //                {
        //                    message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
        //                }
        //                message = string.Format("{0}</ul>", message);
        //                masterPage.PageError = message;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            var masterPage = (IControlRoomMaster)Master;
        //            masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
        //        }
        //    }
        //    if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
        //    {
        //        try
        //        {
        //            var obj = new Event();
        //            int pk = int.Parse(lblPK.Text);//int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text));
        //            obj.Fetch(pk);

        //            obj.EventTitle = ((TextBox)((DetailsView)sender).FindControl("EventTitle")).Text;
        //            obj.EventDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("EventDate")).Text);
        //            obj.EventTime = ((TextBox)((DetailsView)sender).FindControl("EventTime")).Text;
        //            obj.HTML = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML")).Text;
        //            obj.SecretCode = ((TextBox)((DetailsView)sender).FindControl("SecretCode")).Text;
        //            obj.NumberPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumberPoints")).Text);
        //            obj.BadgeID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BadgeID")).SelectedValue);
        //            obj.BranchID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BranchID")).SelectedValue);

        //            obj.Custom1 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom1")).Value;
        //            obj.Custom2 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom2")).Value;
        //            obj.Custom3 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom3")).Value;
        //            //obj.Custom2 = ((TextBox)((DetailsView)sender).FindControl("Custom2")).Text;
        //            //obj.Custom3 = ((TextBox)((DetailsView)sender).FindControl("Custom3")).Text;


        //            obj.ShortDescription = ((TextBox)((DetailsView)sender).FindControl("ShortDescription")).Text;
        //            obj.EndDate = ((TextBox)((DetailsView)sender).FindControl("EndDate")).Text.SafeToDateTime();
        //            obj.EndTime = ((TextBox)((DetailsView)sender).FindControl("EndTime")).Text;

        //            obj.LastModDate = DateTime.Now;
        //            obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

        //            if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
        //            {
        //                obj.Update();
        //                if (e.CommandName.ToLower() == "saveandback")
        //                {
        //                    Response.Redirect(returnURL);
        //                }

        //                odsData.DataBind();
        //                dv.DataBind();
        //                dv.ChangeMode(DetailsViewMode.Edit);

        //                var masterPage = (IControlRoomMaster)Master;
        //                masterPage.PageMessage = SRPResources.SaveOK;
        //            }
        //            else
        //            {
        //                var masterPage = (IControlRoomMaster)Master;
        //                string message = String.Format(SRPResources.ApplicationError1, "<ul>");
        //                foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
        //                {
        //                    message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
        //                }
        //                message = string.Format("{0}</ul>", message);
        //                masterPage.PageError = message;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            var masterPage = (IControlRoomMaster)Master;
        //            masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
        //        }
        //    }
        //}
    }
}

