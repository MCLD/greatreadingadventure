using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Controls;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class EventAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4500;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Event Add / Edit");
            
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            
                lblPK.Text = Session["EID"] == null ? "" : Session["EID"].ToString(); //Session["EID"]= string.Empty;
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();

                if (Request["M"] == "K")
                {
                    MasterPage.DisplayMessageOnLoad = true;
                    MasterPage.PageMessage = "Event was saved successfully!";
                }
            }
        }

        public string CheckDups(string Code, int EID)
        {
            string retVal= string.Empty;

            if (Event.GetEventCountByEventCode(EID, Code) != 0)
            {
                return "<font color=red><b><br/>This secret code is not unique.</b></font>";
            }


            return retVal;
        }
        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                //var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl");
                //if (control != null) control.ProcessRender();
                var ctl = (DropDownList)dv.FindControl("BranchId");
                var lbl = (Label)dv.FindControl("BranchIdLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("BadgeID");
                lbl = (Label)dv.FindControl("BadgeIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                var control = (EvtCustFldCtl)dv.FindControl("Custom1");
                if (control != null) control.Render();
                control = (EvtCustFldCtl)dv.FindControl("Custom2");
                if (control != null) control.Render();
                control = (EvtCustFldCtl)dv.FindControl("Custom3");
                if (control != null) control.Render();
            }


        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/EventList.aspx";
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
                    var obj = new Event();
                    //obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("GenNotificationFlag")).Checked;

                    obj.EventTitle = ((TextBox)((DetailsView)sender).FindControl("EventTitle")).Text;
                    obj.EventDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("EventDate")).Text);
                    obj.EventTime = ((TextBox)((DetailsView)sender).FindControl("EventTime")).Text;
                    obj.HTML = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML")).InnerHtml;
                    obj.SecretCode = ((TextBox)((DetailsView)sender).FindControl("SecretCode")).Text;
                    obj.NumberPoints =  FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumberPoints")).Text);
                    obj.BadgeID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BadgeID")).SelectedValue);
                    obj.BranchID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BranchID")).SelectedValue);
                    obj.Custom1 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom1")).Value;
                    obj.Custom2 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom2")).Value;
                    obj.Custom3 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom3")).Value;

                    obj.ShortDescription = ((TextBox)((DetailsView)sender).FindControl("ShortDescription")).Text;
                    obj.EndDate = ((TextBox)((DetailsView)sender).FindControl("EndDate")).Text.SafeToDateTime();
                    obj.EndTime = ((TextBox)((DetailsView)sender).FindControl("EndTime")).Text;
                    
                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();
                        Cache[CacheKey.EventsActive] = true;
                        if (e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = obj.EID.ToString();

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
                    var obj = new Event();
                    int pk = int.Parse(lblPK.Text);//int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text));
                    obj.Fetch(pk);

                    obj.EventTitle = ((TextBox)((DetailsView)sender).FindControl("EventTitle")).Text;
                    obj.EventDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("EventDate")).Text);
                    obj.EventTime = ((TextBox)((DetailsView)sender).FindControl("EventTime")).Text;
                    obj.HTML = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML")).InnerHtml;
                    obj.SecretCode = ((TextBox)((DetailsView)sender).FindControl("SecretCode")).Text;
                    obj.NumberPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("NumberPoints")).Text);
                    obj.BadgeID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BadgeID")).SelectedValue);
                    obj.BranchID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BranchID")).SelectedValue);

                    obj.Custom1 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom1")).Value;
                    obj.Custom2 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom2")).Value;
                    obj.Custom3 = ((EvtCustFldCtl)((DetailsView)sender).FindControl("Custom3")).Value;
                    //obj.Custom2 = ((TextBox)((DetailsView)sender).FindControl("Custom2")).Text;
                    //obj.Custom3 = ((TextBox)((DetailsView)sender).FindControl("Custom3")).Text;


                    obj.ShortDescription = ((TextBox)((DetailsView)sender).FindControl("ShortDescription")).Text;
                    obj.EndDate = ((TextBox)((DetailsView)sender).FindControl("EndDate")).Text.SafeToDateTime();
                    obj.EndTime = ((TextBox)((DetailsView)sender).FindControl("EndTime")).Text;

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

