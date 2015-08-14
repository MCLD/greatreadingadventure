using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;


namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public partial class NotificationsAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;
            if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);

                var patron = (Patron)Session["Curr_Patron"];
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Notification", patron.FirstName, patron.LastName, patron.Username);
            }
 
            if (!IsPostBack)
            {
                lblPK.Text = Session["NID"] == null ? "" : Session["NID"].ToString();
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
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
            string returnURL = "~/ControlRoom/Modules/Patrons/PatronNotifications.aspx";
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
                    var obj = new DAL.Notifications();
                    
                    obj.PID_To = FormatHelper.SafeToInt(Session["CURR_PATRON_ID"].ToString());
                    obj.PID_From = 0;
                    obj.isQuestion = false;
                    obj.Subject = ((TextBox)((DetailsView)sender).FindControl("Subject")).Text;
                    obj.Body = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("Body")).Text;

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

                        lblPK.Text = obj.NID.ToString();

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
            //if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            //{
            //    try
            //    {
            //        var obj = new Notifications();
            //        int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
            //        obj.Fetch(pk);

            //        //obj.PID_To = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("PID_To")).SelectedValue);
            //        obj.PID_To = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PID_To")).Text);
            //        //obj.PID_From = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("PID_From")).SelectedValue);
            //        obj.PID_From = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PID_From")).Text);
            //        obj.isQuestion = ((CheckBox)((DetailsView)sender).FindControl("isQuestion")).Checked;
            //        obj.Subject = ((TextBox)((DetailsView)sender).FindControl("Subject")).Text;
            //        obj.Body = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("Body")).Text;

            //        obj.LastModDate = DateTime.Now;
            //        obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

            //        if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
            //        {
            //             obj.Update();
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

