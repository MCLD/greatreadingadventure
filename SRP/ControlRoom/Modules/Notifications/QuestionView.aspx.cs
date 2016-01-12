using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Modules.Patrons;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.ControlRoom.Modules.Notifications {
    public partial class QuestionView : BaseControlRoomPage {


        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.RequiredPermission = 5000;
            MasterPage.IsSecure = true;
            if(Session["Curr_Patron"] == null)
                Response.Redirect("Default.aspx");

            if(!IsPostBack) {
                SetPageRibbon(StandardModuleRibbons.NotificationsRibbon());
                PatronsRibbon.GetByAppContext(this);

                var patron = (Patron)Session["Curr_Patron"];
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Notification", patron.FirstName, patron.LastName, patron.Username);
            }

            if(!IsPostBack) {
                lblPK.Text = Request["PK"];
                if(lblPK.Text.Length == 0) {
                    dv.ChangeMode(DetailsViewMode.Insert);
                    Session["Curr_Notification_ID"] = lblPK.Text;
                } else {
                    dv.ChangeMode(DetailsViewMode.Edit);
                }
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e) {
            if(dv.CurrentMode == DetailsViewMode.Edit) {
                //var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl");
                //if (control!=null) control.ProcessRender();
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e) {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e) {
            string returnURL = "~/ControlRoom/Modules/Notifications/NotificationList.aspx";
            if(e.CommandName.ToLower() == "back") {
                Session["Curr_Notification_ID"]= string.Empty;
                Response.Redirect(returnURL);
            }
            if(e.CommandName.ToLower() == "refresh") {
                try {
                    odsData.DataBind();
                    dv.DataBind();
                    dv.ChangeMode(DetailsViewMode.Edit);

                    var masterPage = (IControlRoomMaster)Master;
                    if(masterPage != null)
                        masterPage.PageMessage = SRPResources.RefreshOK;
                } catch(Exception ex) {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
            if(e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback") {
                try {
                    var reply = new DAL.Notifications();
                    var originalMessage = new DAL.Notifications();
                    int pk = int.Parse(lblPK.Text);
                    originalMessage.Fetch(pk);

                    reply.PID_To = FormatHelper.SafeToInt(Session["CURR_PATRON_ID"].ToString());
                    reply.PID_From = 0;
                    reply.isQuestion = false;
                    reply.Subject = "RE: " + originalMessage.Subject;
                    reply.Body = ((HtmlTextArea)((DetailsView)sender).FindControl("Reply")).InnerHtml;

                    reply.AddedDate = DateTime.Now;
                    reply.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    reply.LastModDate = reply.AddedDate;
                    reply.LastModUser = reply.AddedUser;

                    if(reply.IsValid(BusinessRulesValidationMode.INSERT)) {
                        reply.Insert();
                        if(e.CommandName.ToLower() == "saveandback") {

                            //objO.Delete();
                            originalMessage.isUnread = false;
                            originalMessage.Update();

                            Session["CURR_PATRON_ID"]= string.Empty;
                            Session["CURR_PATRON"] = null;
                            Session["CURR_PATRON_MODE"]= string.Empty;
                            Session["Curr_Notification_ID"]= string.Empty;

                            Response.Redirect(returnURL);
                        }

                        //lblPK.Text = obj.NID.ToString();

                        //odsData.DataBind();
                        //dv.DataBind();
                        //dv.ChangeMode(DetailsViewMode.Edit);

                        //var masterPage = (IControlRoomMaster)Master;
                        //masterPage.PageMessage = SRPResources.AddedOK;
                    } else {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach(BusinessRulesValidationMessage m in reply.ErrorCodes) {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }
                } catch(Exception ex) {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
        }
    }
}

