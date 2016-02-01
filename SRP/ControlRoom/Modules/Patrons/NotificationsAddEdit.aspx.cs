using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Web.UI.HtmlControls;
using GRA.Tools;

namespace GRA.SRP.ControlRoom.Modules.Patrons {
    public partial class NotificationsAddEdit : BaseControlRoomPage {
        public string DisplayFrom(DAL.Notifications mail) {
            if(mail.PID_From == 0) {
                var patron = DAL.Patron.FetchObject(mail.PID_To);
                if(mail.AddedUser.Equals(patron.Username, StringComparison.OrdinalIgnoreCase)) {
                    // sent from system
                    return "System";
                } else {
                    return string.Format("Administrator (<span class=\"cr-administrator\">{0}</span>)", mail.AddedUser);
                }
            } else {
                var patron = DAL.Patron.FetchObject(mail.PID_From);
                return DisplayHelper.FormatName(patron.FirstName, patron.LastName, patron.Username);
            }
        }

        public string DisplayTo(DAL.Notifications mail) {
            if(mail.PID_To == 0) {
                return "System";
            } else {
                var patron = DAL.Patron.FetchObject(mail.PID_To);
                return DisplayHelper.FormatName(patron.FirstName, patron.LastName, patron.Username);
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;
            if(Session["Curr_Patron"] == null)
                Response.Redirect("Default.aspx");

            if(!IsPostBack) {
                PatronsRibbon.GetByAppContext(this);

                var patron = (Patron)Session["Curr_Patron"];
                MasterPage.PageTitle = string.Format("Patron Notification - {0}",
                                                     DisplayHelper.FormatName(patron.FirstName, patron.LastName, patron.Username));
            }

            if(!IsPostBack) {
                lblPK.Text = Session["NID"] == null ? "" : Session["NID"].ToString();
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
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
            string returnURL = "~/ControlRoom/Modules/Patrons/PatronNotifications.aspx";
            if(e.CommandName.ToLower() == "back") {
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
            if(e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback") {
                try {
                    var obj = new DAL.Notifications();

                    obj.PID_To = FormatHelper.SafeToInt(Session["CURR_PATRON_ID"].ToString());
                    obj.PID_From = 0;
                    obj.isQuestion = false;
                    obj.Subject = ((TextBox)((DetailsView)sender).FindControl("Subject")).Text;
                    obj.Body = ((HtmlTextArea)((DetailsView)sender).FindControl("Body")).InnerHtml;

                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if(obj.IsValid(BusinessRulesValidationMode.INSERT)) {
                        obj.Insert();
                        if(e.CommandName.ToLower() == "addandback") {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = obj.NID.ToString();

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.AddedOK;
                    } else {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach(BusinessRulesValidationMessage m in obj.ErrorCodes) {
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

