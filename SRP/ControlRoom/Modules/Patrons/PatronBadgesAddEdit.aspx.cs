using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Controls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;


namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronBadgesAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;
            if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");
            var patron = (Patron)Session["Curr_Patron"];
            MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Add Patron Badge", patron.FirstName, patron.LastName, patron.Username);

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);
            }

            if (!IsPostBack)
            {
                lblPK.Text= string.Empty;
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
            string returnURL = "~/ControlRoom/Modules/Patrons/PatronBadges.aspx";
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
                    var badgeID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BadgeID")).SelectedValue);
                    

                    if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");
                    var patron = (Patron)Session["Curr_Patron"];

                    var earnedBadges = new List<Badge>();
                    AwardPoints.AwardBadgeToPatron(badgeID, patron, ref earnedBadges);
                    Response.Redirect(returnURL);

                    //var obj = new DAL.PatronBadges();
                    //obj.PID = patron.PID;
                    //obj.BadgeID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("BadgeID")).SelectedValue);
                    //var dt = ((TextBox) ((DetailsView) sender).FindControl("DateEarned")).Text;
                    //obj.DateEarned = (dt == "" ? DateTime.Now : FormatHelper.SafeToDateTime(dt));

                    
                    //if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    //{
                        //obj.Insert();
                        //var earnedBadge = Badge.GetBadge(obj.BadgeID);
                        




                        //if (earnedBadge.GenNotificationFlag)
                        //{
                        //    var notif = new DAL.Notifications
                        //    {
                        //        PID_To = patron.PID,
                        //        PID_From = 0,  //0 == System Notification
                        //        Subject = earnedBadge.NotificationSubject,
                        //        Body = earnedBadge.NotificationBody,
                        //        isQuestion = false,
                        //        AddedDate = obj.DateEarned,
                        //        LastModDate = obj.DateEarned,
                        //        AddedUser = patron.Username,
                        //        LastModUser = "N/A"
                        //    };
                        //    notif.Insert();
                        //}

                        // Always go back
                        //if (e.CommandName.ToLower() == "addandback")
                        //{
                            //Response.Redirect(returnURL);
                        //}

                        //lblPK.Text = obj.PBID.ToString();

                        //odsData.DataBind();
                        //dv.DataBind();
                        //dv.ChangeMode(DetailsViewMode.Edit);

                        //var masterPage = (IControlRoomMaster)Master;
                        //masterPage.PageMessage = SRPResources.AddedOK;
                    //}
                    //else
                    //{
                    //    var masterPage = (IControlRoomMaster)Master;
                    //    string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                    //    foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                    //    {
                    //        message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                    //    }
                    //    message = string.Format("{0}</ul>", message);
                    //    masterPage.PageError = message;
                    //}
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
            //        var obj = new PatronBadges();
            //        int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
            //        obj.Fetch(pk);

            //        //obj.PID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("PID")).SelectedValue);
            //        obj.PID = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PID")).Text);
            //        //obj.BadgeID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("BadgeID")).SelectedValue);
            //        obj.BadgeID = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("BadgeID")).Text);
            //        obj.DateEarned = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("DateEarned")).Text);

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

