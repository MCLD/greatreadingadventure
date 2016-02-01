using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;


//tod0 - remove <asp:ListItem Value="99" Text="Flash Game"></asp:ListItem>
namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class MinigameAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Mini Game Add");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            
                lblPK.Text = Session["PK"] == null ? "" : Session["PK"].ToString(); Session["PK"]= string.Empty;
                dv.ChangeMode(DetailsViewMode.Insert);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                //var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl");
                //if (control!=null) control.ProcessRender();
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/MinigameList.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }
            //if (e.CommandName.ToLower() == "refresh")
            //{
            //    try
            //    {
            //        odsData.DataBind();
            //        dv.DataBind();
            //        dv.ChangeMode(DetailsViewMode.Edit);

            //        var masterPage = (IControlRoomMaster)Master;
            //        if (masterPage != null) masterPage.PageMessage = SRPResources.RefreshOK;
            //    }
            //    catch (Exception ex)
            //    {
            //        var masterPage = (IControlRoomMaster)Master;
            //        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
            //    }
            //}
            if (e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback")
            {
                try
                {
                    var obj = new Minigame();
                    obj.MiniGameType = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("MiniGameType")).SelectedValue);
                    obj.MiniGameTypeName = ((DropDownList) ((DetailsView) sender).FindControl("MiniGameType")).SelectedItem.Text;
                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
                    obj.GameName = ((TextBox)((DetailsView)sender).FindControl("GameName")).Text;
                    obj.isActive = ((CheckBox)((DetailsView)sender).FindControl("isActive")).Checked;
                    obj.NumberPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumberPoints")).Text);
                    obj.AwardedBadgeID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("AwardedBadgeID")).SelectedValue);

                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();
                        Cache[CacheKey.AdventuresActive] = true;
                        if (e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        //lblPK.Text = obj.MGID.ToString();
                        Session["MGID"] = obj.MGID;
                        Response.Redirect(Minigame.GetEditPage(obj.MiniGameType));

                        //odsData.DataBind();
                        //dv.DataBind();
                        //dv.ChangeMode(DetailsViewMode.Edit);

                        //var masterPage = (IControlRoomMaster)Master;
                        //masterPage.PageMessage = SRPResources.AddedOK;
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
            //if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            //{
            //    try
            //    {
            //        var obj = new Minigame();
            //        int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
            //        obj.Fetch(pk);

            //        //obj.MiniGameType = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("MiniGameType")).SelectedValue);
            //        obj.MiniGameType = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MiniGameType")).Text);
            //        obj.MiniGameTypeName = ((TextBox)((DetailsView)sender).FindControl("MiniGameTypeName")).Text;
            //        obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
            //        obj.GameName = ((TextBox)((DetailsView)sender).FindControl("GameName")).Text;
            //        obj.isActive = ((CheckBox)((DetailsView)sender).FindControl("isActive")).Checked;
            //        //obj.NumberPoints = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("NumberPoints")).SelectedValue);
            //        obj.NumberPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumberPoints")).Text);
            //        //obj.AwardedBadgeID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("AwardedBadgeID")).SelectedValue);
            //        obj.AwardedBadgeID = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AwardedBadgeID")).Text);

            //        obj.LastModDate = DateTime.Now;
            //        obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

            //        if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
            //        {
            //            obj.Update();
            //            if (e.CommandName.ToLower() == "saveandback")
            //            {
            //                Response.Redirect(returnURL);
            //            }

            //            odsData.DataBind();
            //            dv.DataBind();
            //            dv.ChangeMode(DetailsViewMode.Edit);

            //            var masterPage = (IControlRoomMaster)Master;
            //            masterPage.PageMessage = SRPResources.SaveOK;
            //        }
            //        else
            //        {
            //            var masterPage = (IControlRoomMaster)Master;
            //            string message = String.Format(SRPResources.ApplicationError1, "<ul>");
            //            foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
            //            {
            //                message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
            //            }
            //            message = string.Format("{0}</ul>", message);
            //            masterPage.PageError = message;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        var masterPage = (IControlRoomMaster)Master;
            //        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
            //    }
            //}
        }
    }
}

