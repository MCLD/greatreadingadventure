using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.ControlRoom.Modules.Setup {
    public partial class BookListAddEdit : BaseControlRoomPage {


        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.RequiredPermission = 4400;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Challenge Edit");

            if(!IsPostBack) {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                lblPK.Text = Session["BLL"] == null ? "" : Session["BLL"].ToString(); //Session["BLL"]= string.Empty;
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();

                if(Request["M"] == "K") {
                    MasterPage.DisplayMessageOnLoad = true;
                    MasterPage.PageMessage = "Challenge was saved successfully! Now click the 'Tasks in this Challenge' button and start adding book titles to the list!'";
                }
            }
        }


        protected void dv_DataBound(object sender, EventArgs e) {
            if(dv.CurrentMode == DetailsViewMode.Edit) {
                var ctl = (DropDownList)dv.FindControl("LibraryID"); //this.FindControlRecursive(this, "BranchId");
                var lbl = (Label)dv.FindControl("LibraryIDLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if(i != null)
                    ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("ProgID");
                lbl = (Label)dv.FindControl("ProgIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if(i != null)
                    ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("AwardBadgeID");
                lbl = (Label)dv.FindControl("AwardBadgeIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if(i != null)
                    ctl.SelectedValue = lbl.Text;



            }
        }

        protected void dv_DataBinding(object sender, EventArgs e) {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e) {
            string returnURL = "~/ControlRoom/Modules/Setup/BookListList.aspx";
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
                    var obj = new BookList();

                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
                    obj.ListName = ((TextBox)((DetailsView)sender).FindControl("ListName")).Text;
                    obj.AdminDescription = ((TextBox)((DetailsView)sender).FindControl("AdminDescription")).Text;
                    obj.Description = ((HtmlTextArea)((DetailsView)sender).FindControl("Description")).InnerHtml;
                    obj.LiteracyLevel1 = ((TextBox)((DetailsView)sender).FindControl("LiteracyLevel1")).Text.SafeToInt();
                    obj.LiteracyLevel2 = ((TextBox)((DetailsView)sender).FindControl("LiteracyLevel2")).Text.SafeToInt();
                    obj.ProgID = ((DropDownList)((DetailsView)sender).FindControl("ProgID")).SelectedValue.SafeToInt();
                    obj.LibraryID = ((DropDownList)((DetailsView)sender).FindControl("LibraryID")).SelectedValue.SafeToInt();
                    obj.AwardBadgeID = ((DropDownList)((DetailsView)sender).FindControl("AwardBadgeID")).SelectedValue.SafeToInt();
                    obj.AwardPoints = ((TextBox)((DetailsView)sender).FindControl("AwardPoints")).Text.SafeToInt();

                    obj.NumBooksToComplete = ((TextBox)((DetailsView)sender).FindControl("NumBooksToComplete")).Text.SafeToInt();

                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if(obj.IsValid(BusinessRulesValidationMode.INSERT)) {
                        obj.Insert();
                        Cache[CacheKey.ChallengesActive] = true;

                        if(e.CommandName.ToLower() == "addandback") {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = obj.BLID.ToString();

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
            if(e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback" || e.CommandName.ToLower() == "saveandbooks") {
                try {
                    var obj = new BookList();
                    int pk = int.Parse(lblPK.Text);
                    obj.Fetch(pk);

                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
                    obj.ListName = ((TextBox)((DetailsView)sender).FindControl("ListName")).Text;
                    obj.AdminDescription = ((TextBox)((DetailsView)sender).FindControl("AdminDescription")).Text;
                    obj.Description = ((HtmlTextArea)((DetailsView)sender).FindControl("Description")).InnerHtml;
                    obj.LiteracyLevel1 = ((TextBox)((DetailsView)sender).FindControl("LiteracyLevel1")).Text.SafeToInt();
                    obj.LiteracyLevel2 = ((TextBox)((DetailsView)sender).FindControl("LiteracyLevel2")).Text.SafeToInt();
                    obj.ProgID = ((DropDownList)((DetailsView)sender).FindControl("ProgID")).SelectedValue.SafeToInt();
                    obj.LibraryID = ((DropDownList)((DetailsView)sender).FindControl("LibraryID")).SelectedValue.SafeToInt();
                    obj.AwardBadgeID = ((DropDownList)((DetailsView)sender).FindControl("AwardBadgeID")).SelectedValue.SafeToInt();
                    obj.AwardPoints = ((TextBox)((DetailsView)sender).FindControl("AwardPoints")).Text.SafeToInt();

                    obj.NumBooksToComplete = ((TextBox)((DetailsView)sender).FindControl("NumBooksToComplete")).Text.SafeToInt();

                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if(obj.IsValid(BusinessRulesValidationMode.UPDATE)) {
                        obj.Update();
                        if(e.CommandName.ToLower() == "saveandback") {
                            Response.Redirect(returnURL);
                        }

                        if(e.CommandName.ToLower() == "saveandbooks") {
                            Session["BLL"] = pk;
                            Response.Redirect("~/ControlRoom/Modules/Setup/BookListBooksList.aspx");
                            //Response.Redirect(String.Format("{0}?PK={1}", "~/ControlRoom/Modules/Setup/BookListBooksList.aspx", pk));
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
    }
}

