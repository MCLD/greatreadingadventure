using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;


namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class BoardGameAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Board Game Add / Edit");
            
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            
                lblPK.Text = Session["BGID"] == null ? "" : Session["BGID"].ToString();
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtlMap");
                if (control != null) control.ProcessRender();

                control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtlBonus");
                if (control != null) control.ProcessRender();

                control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtlStamp");
                if (control != null) control.ProcessRender();

                var ctl = (DropDownList)dv.FindControl("Minigame1ID");
                var lbl = (Label)dv.FindControl("Minigame1IDLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("Minigame2ID");
                lbl = (Label)dv.FindControl("Minigame2IDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/BoardGameList.aspx";
            string levelURL = "~/ControlRoom/Modules/Setup/ProgramGameLevelList.aspx";
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
                    var obj = new ProgramGame();

                    obj.GameName = ((TextBox)((DetailsView)sender).FindControl("GameName")).Text;
                    //obj.MapImage = ((TextBox)((DetailsView)sender).FindControl("MapImage")).Text;
                    //obj.BonusMapImage = ((TextBox)((DetailsView)sender).FindControl("BonusMapImage")).Text;
                    obj.BoardWidth = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("BoardWidth")).Text);
                    //obj.BoardHeight = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("BoardHeight")).Text);
                    obj.BonusLevelPointMultiplier = FormatHelper.SafeToDecimal(((TextBox)((DetailsView)sender).FindControl("BonusLevelPointMultiplier")).Text); 
                    //obj.LevelCompleteImage = ((TextBox)((DetailsView)sender).FindControl("LevelCompleteImage")).Text;

                    obj.Minigame1ID = ((DropDownList)((DetailsView)sender).FindControl("Minigame1ID")).SelectedValue.SafeToInt();
                    obj.Minigame2ID = ((DropDownList)((DetailsView)sender).FindControl("Minigame2ID")).SelectedValue.SafeToInt();

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

                        lblPK.Text = obj.PGID.ToString();

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
            if (e.CommandName.ToLower() == "levels")
            {
                Response.Redirect(levelURL + "?PGID=" + lblPK.Text);
            }
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    var obj = new ProgramGame();
                    int pk = int.Parse(lblPK.Text);
                    obj.Fetch(pk);

                    obj.GameName = ((TextBox)((DetailsView)sender).FindControl("GameName")).Text;
                    //obj.MapImage = ((TextBox)((DetailsView)sender).FindControl("MapImage")).Text;
                    //obj.BonusMapImage = ((TextBox)((DetailsView)sender).FindControl("BonusMapImage")).Text;
                    obj.BoardWidth = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("BoardWidth")).Text);
                    //obj.BoardHeight = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("BoardHeight")).Text);
                    obj.BonusLevelPointMultiplier = FormatHelper.SafeToDecimal(((TextBox)((DetailsView)sender).FindControl("BonusLevelPointMultiplier")).Text);
                    //obj.LevelCompleteImage = ((TextBox)((DetailsView)sender).FindControl("LevelCompleteImage")).Text;

                    obj.Minigame1ID = ((DropDownList)((DetailsView)sender).FindControl("Minigame1ID")).SelectedValue.SafeToInt();
                    obj.Minigame2ID = ((DropDownList)((DetailsView)sender).FindControl("Minigame2ID")).SelectedValue.SafeToInt();

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

