using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class ProgramGameLevelAddEdit : BaseControlRoomPage
    {

        //public string ShowBoardWGrid(int PGID, int Board = 0)
        //{
        //    var tbl= string.Empty;
        //    var pg = ProgramGame.FetchObject(PGID);
        //    var rows= string.Empty;
        //    var size = 800;
        //    var cellSize = (int)(800/pg.BoardWidth);
        //    size = cellSize*pg.BoardWidth;
        //    var img = string.Format("<img src='/images/Games/Board/{0}{1}.png' style=' width: {2}px; height: {2}px;' width='{2}px' height='{2}px' />",
        //        (Board == 0 ? "" : "bonus_"), PGID, size);
        //    for (int i = 0; i < pg.BoardWidth; i++)
        //    {
        //        for (int j = 0; j < pg.BoardWidth; j++)
        //        {
        //            rows = string.Format("{0}<td style='font-size: 8px; overflow: hidden;' align='center' valign='middle' width='{1}px' height='{2}px' nowrap>X = {3}<br>Y = {4}</td>", rows, cellSize, cellSize - 1, j + 1, i + 1);
        //        }
        //        rows = string.Format("<tr>{0}</tr>", rows);
        //    }
        //    tbl = string.Format("{1}<table border=1 style='border-collapse: collapse; position: relative; top:-{2}px; width: {2}px; height: {2}px;' width='{2}px' height='{2}px'>{0}</table>", rows, img, size);
            
        //    return tbl;
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Game Level Add / Edit");
            
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            
                lblPK.Text = Request["PK"];
                PGID.Text = Request["PK2"];
                if (lblPK.Text.Length == 0 && PGID.Text=="") Response.Redirect("ProgramGameLevelList.aspx");
                if (PGID.Text != "")
                {
                    var o = ProgramGame.FetchObject(int.Parse(PGID.Text));
                    lblGameName.Text = o.GameName;
                }
                if (lblPK.Text.Length == 0)
                {
                    dv.ChangeMode(DetailsViewMode.Insert);
                }
                else
                {
                    dv.ChangeMode(DetailsViewMode.Edit);
                    var o = ProgramGame.FetchObject(ProgramGameLevel.FetchObject(int.Parse(lblPK.Text)).PGID);
                    PGID.Text = o.PGID.ToString();
                    lblGameName.Text = o.GameName;
                }
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var ctl = (DropDownList)dv.FindControl("AwardBadgeID"); 
                var lbl = (Label)dv.FindControl("AwardBadgeIDLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("Minigame1ID");
                lbl = (Label)dv.FindControl("Minigame1IDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("Minigame2ID");
                lbl = (Label)dv.FindControl("Minigame2IDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList) dv.FindControl("AwardBadgeIDBonus");
                lbl = (Label)dv.FindControl("AwardBadgeIDBonusLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("Minigame1IDBonus");
                lbl = (Label)dv.FindControl("Minigame1IDBonusLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("Minigame2IDBonus");
                lbl = (Label)dv.FindControl("Minigame2IDBonusLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/ProgramGameLevelList.aspx?PGID=" + PGID.Text;
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
                    var obj = new ProgramGameLevel();
                    obj.PGID = FormatHelper.SafeToInt(PGID.Text);

                    obj.LevelNumber = -1;// FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LevelNumber")).Text);
                    obj.LocationX = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LocationX")).Text);
                    obj.LocationY = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LocationY")).Text);
                    obj.PointNumber = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PointNumber")).Text);

                    obj.Minigame1ID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("Minigame1ID")).SelectedValue);
                    obj.Minigame2ID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("Minigame2ID")).SelectedValue);
                    obj.AwardBadgeID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("AwardBadgeID")).SelectedValue);

                    obj.LocationXBonus = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LocationXBonus")).Text);
                    obj.LocationYBonus = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LocationYBonus")).Text);
                    obj.Minigame1IDBonus = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("Minigame1IDBonus")).SelectedValue);
                    obj.Minigame2IDBonus = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("Minigame2IDBonus")).SelectedValue);
                    obj.AwardBadgeIDBonus = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("AwardBadgeIDBonus")).SelectedValue);
                    
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

                        lblPK.Text = obj.PGLID.ToString();

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
                    var obj = new ProgramGameLevel();
                    int pk = int.Parse(lblPK.Text);
                    obj.Fetch(pk);

                    //obj.PGID = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PGID")).Text);
                    //obj.LevelNumber = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LevelNumber")).Text);
                    obj.LocationX = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LocationX")).Text);
                    obj.LocationY = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LocationY")).Text);
                    obj.PointNumber = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PointNumber")).Text);
                    
                    obj.Minigame1ID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("Minigame1ID")).SelectedValue);
                    obj.Minigame2ID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("Minigame2ID")).SelectedValue);
                    obj.AwardBadgeID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("AwardBadgeID")).SelectedValue);

                    obj.LocationXBonus = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LocationXBonus")).Text);
                    obj.LocationYBonus = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LocationYBonus")).Text);
                    obj.Minigame1IDBonus = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("Minigame1IDBonus")).SelectedValue);
                    obj.Minigame2IDBonus = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("Minigame2IDBonus")).SelectedValue);
                    obj.AwardBadgeIDBonus = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("AwardBadgeIDBonus")).SelectedValue);

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

