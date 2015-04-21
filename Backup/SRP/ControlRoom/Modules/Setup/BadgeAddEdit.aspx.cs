using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRP_DAL;
using SRPApp.Classes;
using STG.SRP.ControlRoom.Controls;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.Utilities;
using STG.SRP.DAL;

namespace STG.SRP.ControlRoom.Modules.Setup
{
    public partial class BadgeAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4700;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Badges Add/Edit");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                lblPK.Text = Session["BDD"] == null ? "" : Session["BDD"].ToString(); //Session["BDD"] = "";
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/BadgeList.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }
            if (e.CommandName.ToLower() == "refresh")
            {

                try
                {

                    odsData.DataBind();
                    //Page.DataBind(); 
                    dv.DataBind();
                    dv.ChangeMode(DetailsViewMode.Edit);


                    var masterPage = (IControlRoomMaster)Master;
                    if (masterPage != null) masterPage.PageMessage = SRPResources.RefreshOK;

                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }

            if (e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback")
            {
                try
                {
                    var obj = new Badge();
                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
                    obj.UserName = ((TextBox)((DetailsView)sender).FindControl("UserName")).Text;
                    obj.CustomEarnedMessage = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("CustomEarnedMessage")).Text;

                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();
                        if (e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = obj.BID.ToString();

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.AddedOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }

                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);

                }
            }
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    var obj = new Badge();
                    //int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
                    int pk = int.Parse(lblPK.Text);
                    obj.Fetch(pk);

                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("AdminName")).Text;
                    obj.UserName = ((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("UserName")).Text;
                    obj.CustomEarnedMessage = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("CustomEarnedMessage")).Text;
                    obj.IncludesPhysicalPrizeFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("IncludesPhysicalPrizeFlag")).Checked;
                    obj.PhysicalPrizeName = ((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("PhysicalPrizeName")).Text;

                    obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("GenNotificationFlag")).Checked;
                    obj.NotificationSubject = ((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("NotificationSubject")).Text;
                    obj.NotificationBody = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("NotificationBody")).Text;


                    obj.AssignProgramPrizeCode = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel3").FindControl("AssignProgramPrizeCode")).Checked;
                    obj.PCNotificationSubject = ((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel3").FindControl("PCNotificationSubject")).Text;
                    obj.PCNotificationBody = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel3").FindControl("PCNotificationBody")).Text;


                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();

                        SaveBadgeExtendedAttributes(obj,
                                                    (GridView)
                                                    ((DetailsView) sender).FindControl("TabContainer1").FindControl(
                                                        "TabPanel4").FindControl("gvCat"),
                                                    (GridView)
                                                    ((DetailsView) sender).FindControl("TabContainer1").FindControl(
                                                        "TabPanel4").FindControl("gvAge"),
                                                    (GridView)
                                                    ((DetailsView) sender).FindControl("TabContainer1").FindControl(
                                                        "TabPanel4").FindControl("gvBranch"),
                                                    (GridView)
                                                    ((DetailsView) sender).FindControl("TabContainer1").FindControl(
                                                        "TabPanel4").FindControl("gvLoc"));

                        /*
                        GridView gv = (GridView)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel4").FindControl("gvCat");
                        string checkedMembers = "";
                        foreach (GridViewRow row in gv.Rows)
                        {
                            if (((CheckBox)row.FindControl("isMember")).Checked)
                            {
                                checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                            }
                        }
                        if (checkedMembers.Length > 0) checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
                        obj.UpdateBadgeBranches(checkedMembers);

                        gv = (GridView)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel4").FindControl("gvAge");
                        checkedMembers = "";
                        foreach (GridViewRow row in gv.Rows)
                        {
                            if (((CheckBox)row.FindControl("isMember")).Checked)
                            {
                                checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                            }
                        }
                        if (checkedMembers.Length > 0) checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
                        obj.UpdateBadgeAgeGroups(checkedMembers);


                        gv = (GridView)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel4").FindControl("gvBranch");
                        checkedMembers = "";
                        foreach (GridViewRow row in gv.Rows)
                        {
                            if (((CheckBox)row.FindControl("isMember")).Checked)
                            {
                                checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                            }
                        }
                        if (checkedMembers.Length > 0) checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
                        obj.UpdateBadgeBranches(checkedMembers);

                        gv = (GridView)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel4").FindControl("gvLoc");
                        checkedMembers = "";
                        foreach (GridViewRow row in gv.Rows)
                        {
                            if (((CheckBox)row.FindControl("isMember")).Checked)
                            {
                                checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                            }
                        }
                        if (checkedMembers.Length > 0) checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
                        obj.UpdateBadgeLocations(checkedMembers);
                        */


                        if (e.CommandName.ToLower() == "saveandback")
                        {
                            Response.Redirect(returnURL);
                        }
                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.SaveOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }

                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);

                }

            }
        }

        public void SaveBadgeExtendedAttributes(Badge obj, GridView gv1, GridView gv2, GridView gv3, GridView gv4)
        {
            var gv = gv1;
            string checkedMembers = "";
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if (checkedMembers.Length > 0) checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeCategories(checkedMembers);

            gv = gv2;
            checkedMembers = "";
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if (checkedMembers.Length > 0) checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeAgeGroups(checkedMembers);


            gv = gv3;
            checkedMembers = "";
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if (checkedMembers.Length > 0) checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeBranches(checkedMembers);

            gv = gv4;
            checkedMembers = "";
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    checkedMembers = string.Format("{0},{1}", checkedMembers, ((Label)row.FindControl("CID")).Text);
                }
            }
            if (checkedMembers.Length > 0) checkedMembers = checkedMembers.Substring(1, checkedMembers.Length - 1);
            obj.UpdateBadgeLocations(checkedMembers);            
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {

        }

        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {

                //((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("AdminName")).Text;

                var control = (STG.SRP.Classes.FileDownloadCtl)dv.FindControl("TabContainer1").FindControl("TabPanel1").FindControl("FileUploadCtl");
                if (control != null) control.ProcessRender();

            }
        }

    }
}

