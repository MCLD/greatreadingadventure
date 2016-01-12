using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class OfferAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4600;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Offer Add / Edit");
            
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            
                lblPK.Text = Session["OFF"] == null ? "" : Session["OFF"].ToString(); //Session["OFF"]= string.Empty;
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl");
                if (control != null) control.ProcessRender();
            
                var ctl = (DropDownList)dv.FindControl("BranchId");
                var lbl = (Label)dv.FindControl("BranchIdLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("ProgramId");
                lbl = (Label)dv.FindControl("ProgramIdLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;
            }

        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/OfferList.aspx";
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
                    var obj = new Offer();
                    obj.isEnabled = ((CheckBox)((DetailsView)sender).FindControl("isEnabled")).Checked;
                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
                    obj.Title = ((TextBox)((DetailsView)sender).FindControl("Title")).Text;
                    obj.ExternalRedirectFlag = ((CheckBox)((DetailsView)sender).FindControl("ExternalRedirectFlag")).Checked;
                    obj.RedirectURL = ((TextBox)((DetailsView)sender).FindControl("RedirectURL")).Text;
                    obj.MaxImpressions = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MaxImpressions")).Text);
                    //obj.TotalImpressions = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("TotalImpressions")).Text);
                    obj.SerialPrefix = ((TextBox)((DetailsView)sender).FindControl("SerialPrefix")).Text;
                    obj.ZipCode = ((TextBox)((DetailsView)sender).FindControl("ZipCode")).Text;
                    obj.AgeStart = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AgeStart")).Text);
                    obj.AgeEnd = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AgeEnd")).Text);
                    obj.ProgramId = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("ProgramId")).SelectedValue);
                    obj.BranchId = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BranchId")).SelectedValue);

                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();
                        Cache[CacheKey.OffersActive] = true;
                        if (e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = obj.OID.ToString();

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
                    var obj = new Offer();
                    int pk = int.Parse(lblPK.Text);
                    obj.Fetch(pk);

                    obj.isEnabled = ((CheckBox)((DetailsView)sender).FindControl("isEnabled")).Checked;
                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
                    obj.Title = ((TextBox)((DetailsView)sender).FindControl("Title")).Text;
                    obj.ExternalRedirectFlag = ((CheckBox)((DetailsView)sender).FindControl("ExternalRedirectFlag")).Checked;
                    obj.RedirectURL = ((TextBox)((DetailsView)sender).FindControl("RedirectURL")).Text;
                    obj.MaxImpressions = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MaxImpressions")).Text);
                    //obj.TotalImpressions = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("TotalImpressions")).Text);
                    obj.SerialPrefix = ((TextBox)((DetailsView)sender).FindControl("SerialPrefix")).Text;
                    obj.ZipCode = ((TextBox)((DetailsView)sender).FindControl("ZipCode")).Text;
                    obj.AgeStart = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AgeStart")).Text);
                    obj.AgeEnd = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AgeEnd")).Text);
                    obj.ProgramId = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("ProgramId")).SelectedValue);
                    obj.BranchId = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BranchId")).SelectedValue);

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

