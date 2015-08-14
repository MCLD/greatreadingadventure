using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronReviewEdit : BaseControlRoomPage
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
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Review", patron.FirstName, patron.LastName, patron.Username);
            }


            if (!IsPostBack)
            {
                lblPK.Text = Session["PRID"] == null ? "" : Session["PRID"].ToString(); 
                if (lblPK.Text.Length == 0)
                {
                    Response.Redirect("PatronReviews.aspx");
                }
                else
                {
                    dv.ChangeMode(DetailsViewMode.Edit);
                }
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
            string returnURL = "~/ControlRoom/Modules/Patrons/PatronReviews.aspx";
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
            //if (e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback")
            //{
            //    try
            //    {
            //        var obj = new PatronReview();
            //        //obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("GenNotificationFlag")).Checked;

            //        //obj.PID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("PID")).SelectedValue);
            //        obj.PID = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PID")).Text);
            //        //obj.PRLID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("PRLID")).SelectedValue);
            //        obj.PRLID = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PRLID")).Text);
            //        obj.Author = ((TextBox)((DetailsView)sender).FindControl("Author")).Text;
            //        obj.Title = ((TextBox)((DetailsView)sender).FindControl("Title")).Text;
            //        obj.Review = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("Review")).Text;
            //        obj.isApproved = ((CheckBox)((DetailsView)sender).FindControl("isApproved")).Checked;
            //        obj.ReviewDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("ReviewDate")).Text);
            //        obj.ApprovalDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("ApprovalDate")).Text);
            //        obj.ApprovedBy = ((TextBox)((DetailsView)sender).FindControl("ApprovedBy")).Text;

            //        obj.AddedDate = DateTime.Now;
            //        obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
            //        obj.LastModDate = obj.AddedDate;
            //        obj.LastModUser = obj.AddedUser;

            //        if (obj.IsValid(BusinessRulesValidationMode.INSERT))
            //        {
            //            obj.Insert();
            //            if (e.CommandName.ToLower() == "addandback")
            //            {
            //                Response.Redirect(returnURL);
            //            }

            //            lblPK.Text = obj.PRID.ToString();

            //            odsData.DataBind();
            //            dv.DataBind();
            //            dv.ChangeMode(DetailsViewMode.Edit);

            //            var masterPage = (IControlRoomMaster)Master;
            //            masterPage.PageMessage = SRPResources.AddedOK;
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
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    var obj = new PatronReview();
                    int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
                    obj.Fetch(pk);

                    obj.isApproved = ((CheckBox)((DetailsView)sender).FindControl("isApproved")).Checked;
                    obj.ApprovalDate = DateTime.Now;
                    obj.ApprovedBy = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;

                   
                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();
                        // Always and back
                        Response.Redirect(returnURL);
                        //if (e.CommandName.ToLower() == "saveandback")
                        //{
                        //    Response.Redirect(returnURL);
                        //}

                        //odsData.DataBind();
                        //dv.DataBind();
                        //dv.ChangeMode(DetailsViewMode.Edit);

                        //var masterPage = (IControlRoomMaster)Master;
                        //masterPage.PageMessage = SRPResources.SaveOK;
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

