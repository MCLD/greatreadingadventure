using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Drawings
{
    public partial class PrizeDrawingAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4000;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Manage Prize Drawing");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.DrawingsRibbon());
            }

            if (!IsPostBack)
            {
                lblPK.Text = Session["DID"] == null ? "" : Session["DID"].ToString(); //Session["DID"]= string.Empty;
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var ctl = (DropDownList)dv.FindControl("TID"); //this.FindControlRecursive(this, "BranchId");
                var lbl = (Label)dv.FindControl("TIDLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Drawings/Default.aspx";
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
                    var obj = new PrizeDrawing();
                    obj.PrizeName = ((TextBox)((DetailsView)sender).FindControl("PrizeName")).Text;
                    obj.TID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("TID")).SelectedValue);
                    obj.DrawingDateTime = FormatHelper.SafeToDateTime("");
                    obj.NumWinners = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumWinners")).Text);

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

                        lblPK.Text = obj.PDID.ToString();

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
                    var obj = new PrizeDrawing();
                    int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
                    obj.Fetch(pk);

                    obj.PrizeName = ((TextBox)((DetailsView)sender).FindControl("PrizeName")).Text;
                    obj.TID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("TID")).SelectedValue);
                   //obj.DrawingDateTime = FormatHelper.SafeToDateTime("");
                    obj.NumWinners = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumWinners")).Text);

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

            if (e.CommandName.ToLower() == "draw")
            {
                try
                {
                    var obj = new PrizeDrawing();
                    int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
                    obj.Fetch(pk);

                    obj.DrawingDateTime = DateTime.Now;
                    
                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();          

                        var num = PrizeDrawing.DrawWinners(pk, obj.NumWinners);

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = string.Format("{0} Winners have been drawn!", num);
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


            if (e.CommandName.ToLower() == "drawadd")
            {
                try
                {
                    var obj = new PrizeDrawing();
                    int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
                    obj.Fetch(pk);

                    obj.DrawingDateTime = DateTime.Now;

                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session


                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();

                        var addl = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("addl")).Text);

                        if (addl==0)
                        {
                            var mp = (IControlRoomMaster)Master;
                            mp.PageMessage = string.Format("Must enter a value greater than 0 for the Additional # To Draw!", addl);
                            return;
                        }
                        var num = PrizeDrawing.DrawWinners(pk, addl, 1);

                        odsData.DataBind();
                        dv.DataBind();
                        odsWinners.DataBind();
                        var gv2 = (GridView)((DetailsView)sender).FindControl("gv2");
                        gv2.DataBind(); 
                        dv.ChangeMode(DetailsViewMode.Edit);


                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = string.Format("{0} Winners have been drawn!", num);
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



            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var obj = PrizeDrawingWinners.FetchObject(key);
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {

                        try{DAL.Notifications.Delete(DAL.Notifications.FetchObject(obj.NotificationID));}catch{}
                        try { PatronPrizes.Delete(PatronPrizes.FetchObjectByDrawing(key)); }catch { }
                        obj.Delete();

                        odsWinners.DataBind();
                        var gv2 = (GridView)((DetailsView)sender).FindControl("gv2");
                        gv2.DataBind();

                        var masterPage = (IControlRoomMaster)Master;
                        if (masterPage != null) masterPage.PageMessage = SRPResources.DeleteOK;
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




            if (e.CommandName.ToLower() == "pickup")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var obj = PrizeDrawingWinners.FetchObject(key);
                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {

                        try { DAL.Notifications.Delete(DAL.Notifications.FetchObject(obj.NotificationID)); }
                        catch { }
                        try { var pp = PatronPrizes.FetchObjectByDrawing(key);
                            pp.RedeemedFlag = true;
                            pp.Update();
                        }
                        catch { }
                        obj.PrizePickedUpFlag = true;
                        obj.Update();

                        odsWinners.DataBind();
                        var gv2 = (GridView)((DetailsView)sender).FindControl("gv2");
                        gv2.DataBind();

                        var masterPage = (IControlRoomMaster)Master;
                        if (masterPage != null) masterPage.PageMessage = "Prize has been marked a picked up!";
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

