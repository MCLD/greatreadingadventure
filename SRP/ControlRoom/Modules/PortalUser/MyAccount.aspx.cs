using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;

namespace GRA.SRP.ControlRoom.Modules.PortalUser
{
    public partial class MyAccount : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.IsSecure = true;
            PageTitle = "My Account Information";

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.MyAccountRibbon());
                if (SRPUser == null)
                {
                    Response.Redirect("~/ControlRoom/", false);
                }
                else
                {
                    lblUID.Text = SRPUser.Uid.ToString();
                    dv.ChangeMode(DetailsViewMode.Edit);
                }
            }
        }

        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }
            if (e.CommandName.ToLower() == "password")
            {
                Response.Redirect("~/ControlRoom/Modules/PortalUser/PasswordReset.aspx");
            }
            if (e.CommandName.ToLower() == "refresh")
            {
                try
                {
                    odsCMSUser.DataBind();
                    dv.DataBind();
                    dv.ChangeMode(DetailsViewMode.Edit);

                    //ICMSMasterPage masterPage = (ICMSMasterPage)Master;
                    //masterPage.
                    PageMessage = SRPResources.RefreshOK;

                }
                catch (Exception ex)
                {
                    //ICMSMasterPage masterPage = (ICMSMasterPage)Master;
                    //masterPage.
                    PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }


            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    SRPUser updateSrpUser = new SRPUser();
                    int pk = (int)SRPUser.Uid;
                    updateSrpUser = SRPUser.Fetch(pk);

                    updateSrpUser.FirstName = ((TextBox)((DetailsView)sender).FindControl("FirstName")).Text;
                    updateSrpUser.LastName = ((TextBox)((DetailsView)sender).FindControl("Lastname")).Text;
                    updateSrpUser.LastName = ((TextBox)((DetailsView)sender).FindControl("Lastname")).Text;
                    updateSrpUser.EmailAddress = ((TextBox)((DetailsView)sender).FindControl("Emailaddress")).Text;
                    updateSrpUser.Title = ((TextBox)((DetailsView)sender).FindControl("Title")).Text;
                    updateSrpUser.Department = ((TextBox)((DetailsView)sender).FindControl("Department")).Text;
                    updateSrpUser.Division = ((TextBox)((DetailsView)sender).FindControl("Division")).Text;
                    updateSrpUser.LastModDate = DateTime.Now;
                    updateSrpUser.LastModUser = "N/A";  // Get from session
                    string signature = ((TextBox)((DetailsView)sender).FindControl("MailSignature")).Text;
                    if(!string.IsNullOrWhiteSpace(signature.Trim()))
                    {
                        updateSrpUser.MailSignature = signature.Trim();
                    }

                    if (updateSrpUser.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        updateSrpUser.Update();
                        SRPUser = updateSrpUser;
                        Session[SessionData.UserProfile.ToString()] = updateSrpUser;

                        if (e.CommandName.ToLower() == "saveandback")
                        {
                            Response.Redirect(returnURL);
                        }
                        odsCMSUser.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        //ICMSMasterPage masterPage = (ICMSMasterPage)Master;
                        //masterPage.
                        PageMessage = SRPResources.SaveOK;
                    }
                    else
                    {
                        //ICMSMasterPage masterPage = (ICMSMasterPage)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in updateSrpUser.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        //masterPage.
                        PageError = message;
                    }
                }
                catch (Exception ex)
                {
                    //ICMSMasterPage masterPage = (ICMSMasterPage)Master;
                    //masterPage.
                    PageError = String.Format(SRPResources.ApplicationError1, ex.Message);

                }
            }
        }
    }
}
