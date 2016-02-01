using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;


// --> MODULENAME 
// --> XXXXXRibbon 
// --> PERMISSIONID 
namespace GRA.SRP.ControlRoom.Modules.Setup {
    public partial class AwardAddEdit : BaseControlRoomPage {


        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.RequiredPermission = 4900;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Award Add / Edit");

            if(!IsPostBack) {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                //lblPK.Text = Request["PK"];
                lblPK.Text = Session["AWD"] == null ? "" : Session["AWD"].ToString(); //Session["AWD"]= string.Empty;
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();

                if(Request["M"] == "K") {
                    MasterPage.DisplayMessageOnLoad = true;
                    MasterPage.PageMessage = "Award definition was saved successfully! As patrons meet the Award Triggers, they will receive the award badge.";
                }

            }
        }


        protected void dv_DataBound(object sender, EventArgs e) {
            if(dv.CurrentMode == DetailsViewMode.Edit) {
                var ctl = (DropDownList)dv.FindControl("BranchID");
                var lbl = (Label)dv.FindControl("BranchIDLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if(i != null)
                    ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("BadgeID");
                lbl = (Label)dv.FindControl("BadgeIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if(i != null)
                    ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("ProgramID");
                lbl = (Label)dv.FindControl("ProgramIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if(i != null)
                    ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("District");
                lbl = (Label)dv.FindControl("DistrictLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if(i != null)
                    ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("SchoolName");
                lbl = (Label)dv.FindControl("SchoolNameLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if(i != null)
                    ctl.SelectedValue = lbl.Text;

            }
        }

        protected void dv_DataBinding(object sender, EventArgs e) {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e) {
            string returnURL = "~/ControlRoom/Modules/Setup/AwardList.aspx";
            if(e.CommandName.ToLower() == "back") {
                Response.Redirect(returnURL);
            }
            if(e.CommandName.ToLower() == "refresh") {
                try {
                    odsData.DataBind();
                    dv.DataBind();
                    dv.ChangeMode(DetailsViewMode.Edit);

                    var masterPage = (IControlRoomMaster)Master;
                    if(masterPage != null)
                        masterPage.PageMessage = SRPResources.RefreshOK;
                } catch(Exception ex) {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
            if(e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback") {
                try {
                    var obj = new Award();

                    obj.AwardName = ((TextBox)((DetailsView)sender).FindControl("AwardName")).Text;
                    obj.BadgeID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BadgeID")).SelectedValue);
                    obj.NumPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumPoints")).Text);
                    obj.BranchID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BranchID")).SelectedValue);
                    obj.ProgramID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("ProgramID")).SelectedValue);

                    obj.District = ((DropDownList)((DetailsView)sender).FindControl("District")).SelectedValue;
                    obj.SchoolName = ((DropDownList)((DetailsView)sender).FindControl("SchoolName")).SelectedValue;

                    obj.BadgeList = CoalesceBadges((DetailsView)sender);// ((TextBox)((DetailsView)sender).FindControl("BadgeList")).Text;

                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if(obj.IsValid(BusinessRulesValidationMode.INSERT)) {
                        obj.Insert();
                        if(e.CommandName.ToLower() == "addandback") {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = obj.AID.ToString();

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.AddedOK;
                    } else {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach(BusinessRulesValidationMessage m in obj.ErrorCodes) {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }
                } catch(Exception ex) {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
            if(e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback") {
                try {
                    var obj = new Award();
                    int pk = int.Parse(lblPK.Text);
                    obj.Fetch(pk);

                    obj.AwardName = ((TextBox)((DetailsView)sender).FindControl("AwardName")).Text;
                    obj.BadgeID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BadgeID")).SelectedValue);
                    obj.NumPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumPoints")).Text);
                    obj.BranchID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BranchID")).SelectedValue);
                    obj.ProgramID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("ProgramID")).SelectedValue);

                    obj.District = ((DropDownList)((DetailsView)sender).FindControl("District")).SelectedValue;
                    obj.SchoolName = ((DropDownList)((DetailsView)sender).FindControl("SchoolName")).SelectedValue;

                    obj.BadgeList = CoalesceBadges((DetailsView)sender);// ((TextBox)((DetailsView)sender).FindControl("BadgeList")).Text;

                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if(obj.IsValid(BusinessRulesValidationMode.UPDATE)) {
                        obj.Update();
                        if(e.CommandName.ToLower() == "saveandback") {
                            Response.Redirect(returnURL);
                        }

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.SaveOK;
                    } else {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach(BusinessRulesValidationMessage m in obj.ErrorCodes) {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }
                } catch(Exception ex) {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
        }

        protected string CoalesceBadges(DetailsView dv) {
            var gv = (GridView)dv.FindControl("gvBadgeMembership");
            string badgesList= string.Empty;
            foreach(GridViewRow row in gv.Rows) {
                if(((CheckBox)row.FindControl("isMember")).Checked) {
                    badgesList = string.Format("{0},{1}", badgesList, ((Label)row.FindControl("BID")).Text);
                }
            }
            if(badgesList.Length > 0)
                badgesList = badgesList.Substring(1, badgesList.Length - 1);

            return badgesList;
        }
    }
}

