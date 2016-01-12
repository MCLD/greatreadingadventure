using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.ControlRoom.Modules.Tenant
{
    public partial class MyTenantAccount : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 8000;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "My Organization Info");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SubTenantRibbon());

                lblPK.Text = CRTenantID.ToString();
                dv.ChangeMode(DetailsViewMode.Edit);
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
            string returnURL = "~/ControlRoom/Default.aspx";
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
            
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    var obj = new GRA.SRP.Core.Utilities.Tenant();
                    int pk = int.Parse(lblPK.Text);
                    obj.Fetch(pk);

                    obj.Name = ((TextBox)((DetailsView)sender).FindControl("Name")).Text;
                    obj.LandingName = ((TextBox)((DetailsView)sender).FindControl("LandingName")).Text;
                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
                    obj.isActiveFlag = ((CheckBox)((DetailsView)sender).FindControl("isActiveFlag")).Checked;
                    obj.isMasterFlag = ((CheckBox)((DetailsView)sender).FindControl("isMasterFlag")).Checked;
                    obj.Description = ((HtmlTextArea)((DetailsView)sender).FindControl("Description")).InnerHtml;
                    obj.DomainName = ((TextBox)((DetailsView)sender).FindControl("DomainName")).Text;

                    try
                    {
                        obj.showNotifications = ((CheckBox)((DetailsView)sender).FindControl("showNotifications")).Checked;
                        obj.showOffers = ((CheckBox)((DetailsView)sender).FindControl("showOffers")).Checked;
                        obj.showBadges = ((CheckBox)((DetailsView)sender).FindControl("showBadges")).Checked;
                        obj.showEvents = ((CheckBox)((DetailsView)sender).FindControl("showEvents")).Checked;
                        obj.NotificationsMenuText = ((TextBox)((DetailsView)sender).FindControl("NotificationsMenuText")).Text;
                        obj.OffersMenuText = ((TextBox)((DetailsView)sender).FindControl("OffersMenuText")).Text;
                        obj.BadgesMenuText = ((TextBox)((DetailsView)sender).FindControl("BadgesMenuText")).Text;
                        obj.EventsMenuText = ((TextBox)((DetailsView)sender).FindControl("EventsMenuText")).Text;

                        obj.FldInt1 = ((TextBox)((DetailsView)sender).FindControl("FldInt1")).Text.SafeToInt();                        
                    }
                    catch(Exception exc) {
                        this.Log().Error("Error in my tenant account save: {0}", exc.Message);
                    }

                    /*

                    obj.FldInt2 = ((TextBox)((DetailsView)sender).FindControl("FldInt2")).Text.SafeToInt();
                    obj.FldInt3 = ((TextBox)((DetailsView)sender).FindControl("FldInt3")).Text.SafeToInt();
                    obj.FldBit1 = ((CheckBox)((DetailsView)sender).FindControl("FldBit1")).Checked;
                    obj.FldBit2 = ((CheckBox)((DetailsView)sender).FindControl("FldBit2")).Checked;
                    obj.FldBit3 = ((CheckBox)((DetailsView)sender).FindControl("FldBit3")).Checked;
                    obj.FldText1 = ((HtmlTextArea)((DetailsView)sender).FindControl("FldText1")).Text;
                    obj.FldText2 = ((HtmlTextArea)((DetailsView)sender).FindControl("FldText2")).Text;
                    obj.FldText3 = ((HtmlTextArea)((DetailsView)sender).FindControl("FldText3")).Text;
                    */

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

