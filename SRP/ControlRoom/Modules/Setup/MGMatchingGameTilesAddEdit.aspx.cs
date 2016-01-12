using System;
using System.Linq;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class MGMatchingGameTilesAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Matching Game Tiles Add / Edit");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            
                if (Request["MAGID"] != null)
                {
                    lblMGID.Text = Session["MGID"].ToString();
                    lblMAGID.Text = Request["MAGID"];

                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName;

                    lblPK.Text= string.Empty;
                    dv.ChangeMode(DetailsViewMode.Insert);

                }
                else
                {
                    lblPK.Text = Request["PK"];

                    var o1 = MGMatchingGameTiles.FetchObject(int.Parse(lblPK.Text));
                    lblMGID.Text = o1.MGID.ToString();
                    lblMAGID.Text = o1.MAGID.ToString();

                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName;

                    dv.ChangeMode(DetailsViewMode.Edit);
                }
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var control = (Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl_1");
                if (control != null) control.ProcessRender();
                
                control = (Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl_2");
                if (control != null) control.ProcessRender();
                
                control = (Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl_3");
                if (control != null) control.ProcessRender();
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            var returnUrl = "~/ControlRoom/Modules/Setup/MGMatchingGameTilesList.aspx?MGID=" + lblMGID.Text;
            
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnUrl);
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
                    if (masterPage != null)
                        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
            if (e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback")
            {
                try
                {
                    var obj = new MGMatchingGameTiles();

                    obj.MAGID = FormatHelper.SafeToInt(lblMAGID.Text);
                    obj.MGID = FormatHelper.SafeToInt(lblMGID.Text);

                    obj.Tile1UseMedium = ((CheckBox)((DetailsView)sender).FindControl("Tile1UseMedium")).Checked;
                    obj.Tile1UseHard = ((CheckBox)((DetailsView)sender).FindControl("Tile1UseHard")).Checked;
                    obj.Tile2UseMedium = ((CheckBox)((DetailsView)sender).FindControl("Tile2UseMedium")).Checked;
                    obj.Tile2UseHard = ((CheckBox)((DetailsView)sender).FindControl("Tile2UseHard")).Checked;
                    obj.Tile3UseMedium = ((CheckBox)((DetailsView)sender).FindControl("Tile3UseMedium")).Checked;
                    obj.Tile3UseHard = ((CheckBox)((DetailsView)sender).FindControl("Tile3UseHard")).Checked;

                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();
                        if (e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnUrl);
                        }

                        lblPK.Text = obj.MAGTID.ToString();

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        if (masterPage != null) masterPage.PageMessage = SRPResources.AddedOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        var message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        message = obj.ErrorCodes.Aggregate(message, (current, m) => string.Format(String.Format("{0}<li>{{0}}</li>", current), m.ErrorMessage));
                        message = string.Format("{0}</ul>", message);
                        if (masterPage != null) masterPage.PageError = message;
                    }
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    if (masterPage != null)
                        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    var obj = new MGMatchingGameTiles();
                    var pk = int.Parse(((TextBox)((DetailsView)sender).FindControl("MAGTID")).Text);
                    obj.Fetch(pk);

                    obj.MAGID = FormatHelper.SafeToInt(lblMAGID.Text);
                    obj.MGID = FormatHelper.SafeToInt(lblMGID.Text);

                    obj.Tile1UseMedium = ((CheckBox)((DetailsView)sender).FindControl("Tile1UseMedium")).Checked;
                    obj.Tile1UseHard = ((CheckBox)((DetailsView)sender).FindControl("Tile1UseHard")).Checked;
                    obj.Tile2UseMedium = ((CheckBox)((DetailsView)sender).FindControl("Tile2UseMedium")).Checked;
                    obj.Tile2UseHard = ((CheckBox)((DetailsView)sender).FindControl("Tile2UseHard")).Checked;
                    obj.Tile3UseMedium = ((CheckBox)((DetailsView)sender).FindControl("Tile3UseMedium")).Checked;
                    obj.Tile3UseHard = ((CheckBox)((DetailsView)sender).FindControl("Tile3UseHard")).Checked;

                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();
                        if (e.CommandName.ToLower() == "saveandback")
                        {
                            Response.Redirect(returnUrl);
                        }

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        if (masterPage != null) masterPage.PageMessage = SRPResources.SaveOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        var message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        message = obj.ErrorCodes.Aggregate(message, (current, m) => string.Format(String.Format("{0}<li>{{0}}</li>", current), m.ErrorMessage));
                        message = string.Format("{0}</ul>", message);
                        if (masterPage != null) masterPage.PageError = message;
                    }
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    if (masterPage != null)
                        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
        }
    }
}

