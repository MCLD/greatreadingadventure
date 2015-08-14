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
                lblUID.Text = SRPUser.Uid.ToString();
                dv.ChangeMode(DetailsViewMode.Edit);
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
                        SRPUser obj = new SRPUser();
                        int pk = (int)SRPUser.Uid;
                        obj = SRPUser.Fetch(pk);

                        obj.FirstName = ((TextBox)((DetailsView)sender).FindControl("FirstName")).Text;
                        obj.LastName = ((TextBox)((DetailsView)sender).FindControl("Lastname")).Text;
                        obj.LastName = ((TextBox)((DetailsView)sender).FindControl("Lastname")).Text;
                        obj.EmailAddress = ((TextBox)((DetailsView)sender).FindControl("Emailaddress")).Text;
                        obj.Title = ((TextBox)((DetailsView)sender).FindControl("Title")).Text;
                        obj.Department = ((TextBox)((DetailsView)sender).FindControl("Department")).Text;
                        obj.Division = ((TextBox)((DetailsView)sender).FindControl("Division")).Text;
                        obj.LastModDate = DateTime.Now;
                        obj.LastModUser = "N/A";  // Get from session

                        if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                        {
                            obj.Update();
                            SRPUser = obj;
                            Session[SessionData.UserProfile.ToString()] = obj;

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
                            foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
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
