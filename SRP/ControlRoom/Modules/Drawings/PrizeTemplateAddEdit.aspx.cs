using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.ControlRoom.Modules.Drawings
{
    public partial class PrizeTemplateAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4000;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Prize Templates Add / Edit");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.DrawingsRibbon());
            }
 
            if (!IsPostBack)
            {
                lblPK.Text = Session["DTD"] == null ? "" : Session["DTD"].ToString(); //Session["DTD"]= string.Empty;
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var ctl = (DropDownList)dv.FindControl("PrimaryLibrary"); //this.FindControlRecursive(this, "BranchId");
                var lbl = (Label)dv.FindControl("PrimaryLibraryLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("ProgID");
                lbl = (Label)dv.FindControl("ProgIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("SchoolName");
                lbl = (Label)dv.FindControl("SchoolNameLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Drawings/TemplateList.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }
            if (e.CommandName.ToLower() == "refresh")
            {
                try
                {          
                    odsData.DataBind();
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
                    var obj = new PrizeTemplate();
                    //obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("GenNotificationFlag")).Checked;

                    obj.TName = ((TextBox)((DetailsView)sender).FindControl("TName")).Text;

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

                        lblPK.Text = obj.TID.ToString();

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
                    var obj = new PrizeTemplate();
                    int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
                    obj.Fetch(pk);

                    obj.TName = ((TextBox)((DetailsView)sender).FindControl("TName")).Text;
                    obj.NumPrizes = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumPrizes")).Text);
                    obj.IncPrevWinnersFlag = ((CheckBox)((DetailsView)sender).FindControl("IncPrevWinnersFlag")).Checked;
                    obj.SendNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("SendNotificationFlag")).Checked;
                    obj.NotificationSubject = ((TextBox)((DetailsView)sender).FindControl("NotificationSubject")).Text;
                    obj.NotificationMessage = ((HtmlTextArea)((DetailsView)sender).FindControl("NotificationMessage")).InnerHtml;
                    obj.ProgID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("ProgID")).SelectedValue);
                    obj.Gender = ((DropDownList)((DetailsView)sender).FindControl("Gender")).SelectedValue;
                    obj.PrimaryLibrary = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("PrimaryLibrary")).SelectedValue);
                    obj.MinPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MinPoints")).Text);
                    obj.MaxPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MaxPoints")).Text);
                    obj.LogDateStart = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("LogDateStart")).Text);
                    obj.LogDateEnd = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("LogDateEnd")).Text);
                    obj.MinReviews = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MinReviews")).Text);
                    obj.MaxReviews = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MaxReviews")).Text);
                    obj.ReviewDateStart = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("ReviewDateStart")).Text);
                    obj.ReviewDateEnd = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("ReviewDateEnd")).Text);

                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                         obj.Update();
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
    }
}


