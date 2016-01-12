using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;
using ExportToExcel;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;


namespace GRA.SRP.ControlRoom.Modules.Reports
{
    public partial class ReportAddEdit : BaseControlRoomPage
    {

        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Ad-Hoc Report");
            
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ReportsRibbon());
                if (Request["RID"] == "new") Session["RID"] = null;

                lblPK.Text = Session["RID"] == null ? "" : Session["RID"].ToString();
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {

                var ctl = (DropDownList)dv.FindControl("ProgID");
                var lbl = (Label)dv.FindControl("ProgIDLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("RTID");
                lbl = (Label)dv.FindControl("RTIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("ReportFormat");
                lbl = (Label)dv.FindControl("ReportFormatLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("Gender");
                lbl = (Label)dv.FindControl("GenderLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("PrimaryLibrary");
                lbl = (Label)dv.FindControl("PrimaryLibraryLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("District");
                lbl = (Label)dv.FindControl("DistrictLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("SDistrict");
                lbl = (Label)dv.FindControl("SDistrictLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("SchoolName");
                lbl = (Label)dv.FindControl("SchoolNameLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;


                ctl = (DropDownList)dv.FindControl("SchoolType");
                lbl = (Label)dv.FindControl("SchoolTypeLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("EarnedBadge");
                lbl = (Label)dv.FindControl("EarnedBadgeLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Reports/ReportList.aspx";
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
#region Save report 
            if (e.CommandName.ToLower() == "savereport")
            {
                if (lblPK.Text == "")
                {
                    try
                    {
                        //ADD 
                        var obj = new SRPReport();

                        obj.RTID =
                            FormatHelper.SafeToInt(
                                ((DropDownList)((DetailsView)sender).FindControl("RTID")).SelectedValue);
                        obj.ProgId =
                            FormatHelper.SafeToInt(
                                ((DropDownList)((DetailsView)sender).FindControl("ProgId")).SelectedValue);
                        obj.ReportName = ((TextBox)((DetailsView)sender).FindControl("ReportName")).Text;
                        obj.DisplayFilters = ((CheckBox)((DetailsView)sender).FindControl("DisplayFilters")).Checked;
                        obj.ReportFormat =
                            FormatHelper.SafeToInt(
                                ((DropDownList)((DetailsView)sender).FindControl("ReportFormat")).SelectedValue);

                        obj.DOBFrom =
                            FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("DOBFrom")).Text);
                        obj.DOBTo =
                            FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("DOBTo")).Text);
                        obj.AgeFrom =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AgeFrom")).Text);
                        obj.AgeTo = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AgeTo")).Text);
                        obj.SchoolGrade = ((TextBox)((DetailsView)sender).FindControl("SchoolGrade")).Text;
                        obj.FirstName = ((TextBox)((DetailsView)sender).FindControl("FirstName")).Text;
                        obj.LastName = ((TextBox)((DetailsView)sender).FindControl("LastName")).Text;
                        obj.Gender = ((DropDownList)((DetailsView)sender).FindControl("Gender")).SelectedValue;
                        obj.EmailAddress = ((TextBox)((DetailsView)sender).FindControl("EmailAddress")).Text;
                        obj.PhoneNumber = ((TextBox)((DetailsView)sender).FindControl("PhoneNumber")).Text;
                        obj.City = ((TextBox)((DetailsView)sender).FindControl("City")).Text;
                        obj.State = ((TextBox)((DetailsView)sender).FindControl("State")).Text;
                        obj.ZipCode = ((TextBox)((DetailsView)sender).FindControl("ZipCode")).Text;
                        obj.County = ((TextBox)((DetailsView)sender).FindControl("County")).Text;
                        obj.PrimaryLibrary =
                            FormatHelper.SafeToInt(
                                ((DropDownList)((DetailsView)sender).FindControl("PrimaryLibrary")).SelectedValue);
                        obj.SchoolName = ((DropDownList)((DetailsView)sender).FindControl("SchoolName")).SelectedValue;
                        obj.District = ((DropDownList)((DetailsView)sender).FindControl("District")).SelectedValue;
                        obj.SDistrict = ((DropDownList)((DetailsView)sender).FindControl("SDistrict")).SelectedValue;

                        obj.Teacher = ((TextBox)((DetailsView)sender).FindControl("Teacher")).Text;
                        obj.GroupTeamName = ((TextBox)((DetailsView)sender).FindControl("GroupTeamName")).Text;
                        obj.SchoolType =
                            FormatHelper.SafeToInt(
                                ((DropDownList)((DetailsView)sender).FindControl("SchoolType")).SelectedValue);

                        obj.LiteracyLevel1 =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LiteracyLevel1")).Text);
                        obj.LiteracyLevel2 =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LiteracyLevel2")).Text);

                        obj.Custom1 = ((TextBox)((DetailsView)sender).FindControl("Custom1")).Text;
                        obj.Custom2 = ((TextBox)((DetailsView)sender).FindControl("Custom2")).Text;
                        obj.Custom3 = ((TextBox)((DetailsView)sender).FindControl("Custom3")).Text;
                        obj.Custom4 = ((TextBox)((DetailsView)sender).FindControl("Custom4")).Text;
                        obj.Custom5 = ((TextBox)((DetailsView)sender).FindControl("Custom5")).Text;

                        obj.RegistrationDateStart =
                            FormatHelper.SafeToDateTime(
                                ((TextBox)((DetailsView)sender).FindControl("RegistrationDateStart")).Text);
                        obj.RegistrationDateEnd =
                            FormatHelper.SafeToDateTime(
                                ((TextBox)((DetailsView)sender).FindControl("RegistrationDateEnd")).Text);

                        obj.PointsMin =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PointsMin")).Text);
                        obj.PointsMax =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PointsMax")).Text);
                        obj.PointsStart =
                            FormatHelper.SafeToDateTime(
                                ((TextBox)((DetailsView)sender).FindControl("PointsStart")).Text);
                        obj.PointsEnd =
                            FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("PointsEnd")).Text);
                        obj.EventCode = ((TextBox)((DetailsView)sender).FindControl("EventCode")).Text;
                        obj.EarnedBadge =
                            FormatHelper.SafeToInt(
                                ((DropDownList)((DetailsView)sender).FindControl("EarnedBadge")).SelectedValue);
                        obj.PhysicalPrizeEarned =
                            ((TextBox)((DetailsView)sender).FindControl("PhysicalPrizeEarned")).Text;
                        obj.PhysicalPrizeRedeemed =
                            ((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeRedeemed")).Checked;
                        obj.PhysicalPrizeStartDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox)((DetailsView)sender).FindControl("PhysicalPrizeStartDate")).Text);
                        obj.PhysicalPrizeEndDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox)((DetailsView)sender).FindControl("PhysicalPrizeEndDate")).Text);

                        obj.ReviewsMin =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("ReviewsMin")).Text);
                        obj.ReviewsMax =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("ReviewsMax")).Text);
                        obj.ReviewTitle = ((TextBox)((DetailsView)sender).FindControl("ReviewTitle")).Text;
                        obj.ReviewAuthor = ((TextBox)((DetailsView)sender).FindControl("ReviewAuthor")).Text;
                        obj.ReviewStartDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox)((DetailsView)sender).FindControl("ReviewStartDate")).Text);
                        obj.ReviewEndDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox)((DetailsView)sender).FindControl("ReviewEndDate")).Text);
                        obj.RandomDrawingName = ((TextBox)((DetailsView)sender).FindControl("RandomDrawingName")).Text;
                        obj.RandomDrawingNum =
                            FormatHelper.SafeToInt(
                                ((TextBox)((DetailsView)sender).FindControl("RandomDrawingNum")).Text);
                        obj.RandomDrawingStartDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox)((DetailsView)sender).FindControl("RandomDrawingStartDate")).Text);
                        obj.RandomDrawingEndDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox)((DetailsView)sender).FindControl("RandomDrawingEndDate")).Text);
                        obj.HasBeenDrawn = ((CheckBox)((DetailsView)sender).FindControl("HasBeenDrawn")).Checked;
                        obj.HasRedeemend = ((CheckBox)((DetailsView)sender).FindControl("HasRedeemend")).Checked;

                        obj.PIDInc = ((CheckBox)((DetailsView)sender).FindControl("PIDInc")).Checked;
                        obj.UsernameInc = ((CheckBox)((DetailsView)sender).FindControl("UsernameInc")).Checked;
                        obj.DOBInc = ((CheckBox)((DetailsView)sender).FindControl("DOBInc")).Checked;
                        obj.AgeInc = ((CheckBox)((DetailsView)sender).FindControl("AgeInc")).Checked;
                        obj.SchoolGradeInc = ((CheckBox)((DetailsView)sender).FindControl("SchoolGradeInc")).Checked;
                        obj.FirstNameInc = ((CheckBox)((DetailsView)sender).FindControl("FirstNameInc")).Checked;
                        obj.LastNameInc = ((CheckBox)((DetailsView)sender).FindControl("LastNameInc")).Checked;
                        obj.GenderInc = ((CheckBox)((DetailsView)sender).FindControl("GenderInc")).Checked;
                        obj.EmailAddressInc = ((CheckBox)((DetailsView)sender).FindControl("EmailAddressInc")).Checked;
                        obj.PhoneNumberInc = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumberInc")).Checked;
                        obj.CityInc = ((CheckBox)((DetailsView)sender).FindControl("CityInc")).Checked;
                        obj.StateInc = ((CheckBox)((DetailsView)sender).FindControl("StateInc")).Checked;
                        obj.ZipCodeInc = ((CheckBox)((DetailsView)sender).FindControl("ZipCodeInc")).Checked;
                        obj.CountyInc = ((CheckBox)((DetailsView)sender).FindControl("CountyInc")).Checked;
                        obj.PrimaryLibraryInc =
                            ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibraryInc")).Checked;
                        obj.SchoolNameInc = ((CheckBox)((DetailsView)sender).FindControl("SchoolNameInc")).Checked;
                        obj.DistrictInc = ((CheckBox)((DetailsView)sender).FindControl("DistrictInc")).Checked;
                        obj.TeacherInc = ((CheckBox)((DetailsView)sender).FindControl("TeacherInc")).Checked;
                        obj.GroupTeamNameInc =
                            ((CheckBox)((DetailsView)sender).FindControl("GroupTeamNameInc")).Checked;
                        obj.SchoolTypeInc = ((CheckBox)((DetailsView)sender).FindControl("SchoolTypeInc")).Checked;
                        obj.LiteracyLevel1Inc =
                            ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1Inc")).Checked;
                        obj.LiteracyLevel2Inc =
                            ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2Inc")).Checked;
                        obj.Custom1Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom1Inc")).Checked;
                        obj.Custom2Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom2Inc")).Checked;
                        obj.Custom3Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom3Inc")).Checked;
                        obj.Custom4Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom4Inc")).Checked;
                        obj.Custom5Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom5Inc")).Checked;
                        obj.RegistrationDateInc =
                            ((CheckBox)((DetailsView)sender).FindControl("RegistrationDateInc")).Checked;
                        obj.PointsInc = ((CheckBox)((DetailsView)sender).FindControl("PointsInc")).Checked;
                        obj.EarnedBadgeInc = ((CheckBox)((DetailsView)sender).FindControl("EarnedBadgeInc")).Checked;
                        obj.PhysicalPrizeNameInc =
                            ((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeNameInc")).Checked;
                        obj.PhysicalPrizeDateInc =
                            ((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeDateInc")).Checked;
                        obj.NumReviewsInc = ((CheckBox)((DetailsView)sender).FindControl("NumReviewsInc")).Checked;
                        obj.ReviewAuthorInc = ((CheckBox)((DetailsView)sender).FindControl("ReviewAuthorInc")).Checked;
                        obj.ReviewTitleInc = ((CheckBox)((DetailsView)sender).FindControl("ReviewTitleInc")).Checked;
                        obj.ReviewDateInc = ((CheckBox)((DetailsView)sender).FindControl("ReviewDateInc")).Checked;
                        obj.RandomDrawingNameInc =
                            ((CheckBox)((DetailsView)sender).FindControl("RandomDrawingNameInc")).Checked;
                        obj.RandomDrawingNumInc =
                            ((CheckBox)((DetailsView)sender).FindControl("RandomDrawingNumInc")).Checked;
                        obj.RandomDrawingDateInc =
                            ((CheckBox)((DetailsView)sender).FindControl("RandomDrawingDateInc")).Checked;
                        obj.HasBeenDrawnInc = ((CheckBox)((DetailsView)sender).FindControl("HasBeenDrawnInc")).Checked;
                        obj.HasRedeemendInc = ((CheckBox)((DetailsView)sender).FindControl("HasRedeemendInc")).Checked;


                        obj.Score1From =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1From")).Text);
                        obj.Score1To =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1To")).Text);
                        obj.Score2From =
                            FormatHelper.SafeToInt(
                                ((TextBox)((DetailsView)sender).FindControl("Score2From")).Text);
                        obj.Score2To =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score2To")).Text);

                        obj.Score1PctFrom =
                                FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1PctFrom")).Text);
                        obj.Score1PctTo =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1PctTo")).Text);
                        obj.Score2PctFrom =
                            FormatHelper.SafeToInt(
                                ((TextBox)((DetailsView)sender).FindControl("Score2PctFrom")).Text);
                        obj.Score2PctTo =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score2PctTo")).Text);

                        obj.Score1Inc = ((CheckBox)((DetailsView)sender).FindControl("Score1Inc")).Checked;
                        obj.Score2Inc = ((CheckBox)((DetailsView)sender).FindControl("Score2Inc")).Checked;
                        obj.Score1PctInc = ((CheckBox)((DetailsView)sender).FindControl("Score1PctInc")).Checked;
                        obj.Score2PctInc = ((CheckBox)((DetailsView)sender).FindControl("Score2PctInc")).Checked;

                        obj.AddedDate = DateTime.Now;
                        obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;
                        //"N/A";  // Get from session
                        obj.LastModDate = obj.AddedDate;
                        obj.LastModUser = obj.AddedUser;

                        if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                        {
                            obj.Insert();

                            lblPK.Text = obj.RID.ToString();

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
                else
                {

                    try
                    {
                        var obj = new SRPReport();
                        int pk = int.Parse(lblPK.Text);
                        obj.Fetch(pk);

                        obj.RTID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("RTID")).SelectedValue);
                        obj.ProgId = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("ProgId")).SelectedValue);
                        obj.ReportName = ((TextBox) ((DetailsView) sender).FindControl("ReportName")).Text;
                        obj.DisplayFilters = ((CheckBox) ((DetailsView) sender).FindControl("DisplayFilters")).Checked;
                        obj.ReportFormat = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("ReportFormat")).SelectedValue);
                        obj.DOBFrom =
                            FormatHelper.SafeToDateTime(((TextBox) ((DetailsView) sender).FindControl("DOBFrom")).Text);
                        obj.DOBTo =
                            FormatHelper.SafeToDateTime(((TextBox) ((DetailsView) sender).FindControl("DOBTo")).Text);
                        obj.AgeFrom =
                            FormatHelper.SafeToInt(((TextBox) ((DetailsView) sender).FindControl("AgeFrom")).Text);
                        obj.AgeTo = FormatHelper.SafeToInt(((TextBox) ((DetailsView) sender).FindControl("AgeTo")).Text);
                        obj.SchoolGrade = ((TextBox) ((DetailsView) sender).FindControl("SchoolGrade")).Text;
                        obj.FirstName = ((TextBox) ((DetailsView) sender).FindControl("FirstName")).Text;
                        obj.LastName = ((TextBox) ((DetailsView) sender).FindControl("LastName")).Text;
                        obj.Gender = ((DropDownList) ((DetailsView) sender).FindControl("Gender")).SelectedValue;
                        obj.EmailAddress = ((TextBox) ((DetailsView) sender).FindControl("EmailAddress")).Text;
                        obj.PhoneNumber = ((TextBox) ((DetailsView) sender).FindControl("PhoneNumber")).Text;
                        obj.City = ((TextBox) ((DetailsView) sender).FindControl("City")).Text;
                        obj.State = ((TextBox) ((DetailsView) sender).FindControl("State")).Text;
                        obj.ZipCode = ((TextBox) ((DetailsView) sender).FindControl("ZipCode")).Text;
                        obj.County = ((TextBox) ((DetailsView) sender).FindControl("County")).Text;

                        obj.PrimaryLibrary =    FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("PrimaryLibrary")).SelectedValue);
                        obj.SchoolName = ((DropDownList)((DetailsView)sender).FindControl("SchoolName")).SelectedValue;
                        obj.District = ((DropDownList)((DetailsView)sender).FindControl("District")).SelectedValue;
                        obj.SDistrict = ((DropDownList)((DetailsView)sender).FindControl("SDistrict")).SelectedValue; 

                        obj.Teacher = ((TextBox) ((DetailsView) sender).FindControl("Teacher")).Text;
                        obj.GroupTeamName = ((TextBox) ((DetailsView) sender).FindControl("GroupTeamName")).Text;
                        obj.SchoolType = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("SchoolType")).SelectedValue);
                        obj.LiteracyLevel1 =
                            FormatHelper.SafeToInt(((TextBox) ((DetailsView) sender).FindControl("LiteracyLevel1")).Text);
                        obj.LiteracyLevel2 =
                            FormatHelper.SafeToInt(((TextBox) ((DetailsView) sender).FindControl("LiteracyLevel2")).Text);
                        obj.Custom1 = ((TextBox) ((DetailsView) sender).FindControl("Custom1")).Text;
                        obj.Custom2 = ((TextBox) ((DetailsView) sender).FindControl("Custom2")).Text;
                        obj.Custom3 = ((TextBox) ((DetailsView) sender).FindControl("Custom3")).Text;
                        obj.Custom4 = ((TextBox) ((DetailsView) sender).FindControl("Custom4")).Text;
                        obj.Custom5 = ((TextBox) ((DetailsView) sender).FindControl("Custom5")).Text;
                        obj.RegistrationDateStart =
                            FormatHelper.SafeToDateTime(
                                ((TextBox) ((DetailsView) sender).FindControl("RegistrationDateStart")).Text);
                        obj.RegistrationDateEnd =
                            FormatHelper.SafeToDateTime(
                                ((TextBox) ((DetailsView) sender).FindControl("RegistrationDateEnd")).Text);
                        obj.PointsMin =
                            FormatHelper.SafeToInt(((TextBox) ((DetailsView) sender).FindControl("PointsMin")).Text);
                        obj.PointsMax =
                            FormatHelper.SafeToInt(((TextBox) ((DetailsView) sender).FindControl("PointsMax")).Text);
                        obj.PointsStart =
                            FormatHelper.SafeToDateTime(
                                ((TextBox) ((DetailsView) sender).FindControl("PointsStart")).Text);
                        obj.PointsEnd =
                            FormatHelper.SafeToDateTime(((TextBox) ((DetailsView) sender).FindControl("PointsEnd")).Text);
                        obj.EventCode = ((TextBox) ((DetailsView) sender).FindControl("EventCode")).Text;
                        obj.EarnedBadge = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("EarnedBadge")).SelectedValue);
                        obj.PhysicalPrizeEarned =
                            ((TextBox) ((DetailsView) sender).FindControl("PhysicalPrizeEarned")).Text;
                        obj.PhysicalPrizeRedeemed =
                            ((CheckBox) ((DetailsView) sender).FindControl("PhysicalPrizeRedeemed")).Checked;
                        obj.PhysicalPrizeStartDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox) ((DetailsView) sender).FindControl("PhysicalPrizeStartDate")).Text);
                        obj.PhysicalPrizeEndDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox) ((DetailsView) sender).FindControl("PhysicalPrizeEndDate")).Text);
                        obj.ReviewsMin =
                            FormatHelper.SafeToInt(((TextBox) ((DetailsView) sender).FindControl("ReviewsMin")).Text);
                        obj.ReviewsMax =
                            FormatHelper.SafeToInt(((TextBox) ((DetailsView) sender).FindControl("ReviewsMax")).Text);
                        obj.ReviewTitle = ((TextBox) ((DetailsView) sender).FindControl("ReviewTitle")).Text;
                        obj.ReviewAuthor = ((TextBox) ((DetailsView) sender).FindControl("ReviewAuthor")).Text;
                        obj.ReviewStartDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox) ((DetailsView) sender).FindControl("ReviewStartDate")).Text);
                        obj.ReviewEndDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox) ((DetailsView) sender).FindControl("ReviewEndDate")).Text);
                        obj.RandomDrawingName = ((TextBox) ((DetailsView) sender).FindControl("RandomDrawingName")).Text;
                        obj.RandomDrawingNum =
                            FormatHelper.SafeToInt(
                                ((TextBox) ((DetailsView) sender).FindControl("RandomDrawingNum")).Text);
                        obj.RandomDrawingStartDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox) ((DetailsView) sender).FindControl("RandomDrawingStartDate")).Text);
                        obj.RandomDrawingEndDate =
                            FormatHelper.SafeToDateTime(
                                ((TextBox) ((DetailsView) sender).FindControl("RandomDrawingEndDate")).Text);
                        obj.HasBeenDrawn = ((CheckBox) ((DetailsView) sender).FindControl("HasBeenDrawn")).Checked;
                        obj.HasRedeemend = ((CheckBox) ((DetailsView) sender).FindControl("HasRedeemend")).Checked;
                        obj.PIDInc = ((CheckBox) ((DetailsView) sender).FindControl("PIDInc")).Checked;
                        obj.UsernameInc = ((CheckBox) ((DetailsView) sender).FindControl("UsernameInc")).Checked;
                        obj.DOBInc = ((CheckBox) ((DetailsView) sender).FindControl("DOBInc")).Checked;
                        obj.AgeInc = ((CheckBox) ((DetailsView) sender).FindControl("AgeInc")).Checked;
                        obj.SchoolGradeInc = ((CheckBox) ((DetailsView) sender).FindControl("SchoolGradeInc")).Checked;
                        obj.FirstNameInc = ((CheckBox) ((DetailsView) sender).FindControl("FirstNameInc")).Checked;
                        obj.LastNameInc = ((CheckBox) ((DetailsView) sender).FindControl("LastNameInc")).Checked;
                        obj.GenderInc = ((CheckBox) ((DetailsView) sender).FindControl("GenderInc")).Checked;
                        obj.EmailAddressInc = ((CheckBox) ((DetailsView) sender).FindControl("EmailAddressInc")).Checked;
                        obj.PhoneNumberInc = ((CheckBox) ((DetailsView) sender).FindControl("PhoneNumberInc")).Checked;
                        obj.CityInc = ((CheckBox) ((DetailsView) sender).FindControl("CityInc")).Checked;
                        obj.StateInc = ((CheckBox) ((DetailsView) sender).FindControl("StateInc")).Checked;
                        obj.ZipCodeInc = ((CheckBox) ((DetailsView) sender).FindControl("ZipCodeInc")).Checked;
                        obj.CountyInc = ((CheckBox) ((DetailsView) sender).FindControl("CountyInc")).Checked;
                        obj.PrimaryLibraryInc =
                            ((CheckBox) ((DetailsView) sender).FindControl("PrimaryLibraryInc")).Checked;
                        obj.SchoolNameInc = ((CheckBox) ((DetailsView) sender).FindControl("SchoolNameInc")).Checked;
                        obj.DistrictInc = ((CheckBox) ((DetailsView) sender).FindControl("DistrictInc")).Checked;
                        obj.TeacherInc = ((CheckBox) ((DetailsView) sender).FindControl("TeacherInc")).Checked;
                        obj.GroupTeamNameInc =
                            ((CheckBox) ((DetailsView) sender).FindControl("GroupTeamNameInc")).Checked;
                        obj.SchoolTypeInc = ((CheckBox) ((DetailsView) sender).FindControl("SchoolTypeInc")).Checked;
                        obj.LiteracyLevel1Inc =
                            ((CheckBox) ((DetailsView) sender).FindControl("LiteracyLevel1Inc")).Checked;
                        obj.LiteracyLevel2Inc =
                            ((CheckBox) ((DetailsView) sender).FindControl("LiteracyLevel2Inc")).Checked;
                        obj.Custom1Inc = ((CheckBox) ((DetailsView) sender).FindControl("Custom1Inc")).Checked;
                        obj.Custom2Inc = ((CheckBox) ((DetailsView) sender).FindControl("Custom2Inc")).Checked;
                        obj.Custom3Inc = ((CheckBox) ((DetailsView) sender).FindControl("Custom3Inc")).Checked;
                        obj.Custom4Inc = ((CheckBox) ((DetailsView) sender).FindControl("Custom4Inc")).Checked;
                        obj.Custom5Inc = ((CheckBox) ((DetailsView) sender).FindControl("Custom5Inc")).Checked;
                        obj.RegistrationDateInc =
                            ((CheckBox) ((DetailsView) sender).FindControl("RegistrationDateInc")).Checked;
                        obj.PointsInc = ((CheckBox) ((DetailsView) sender).FindControl("PointsInc")).Checked;
                        obj.EarnedBadgeInc = ((CheckBox) ((DetailsView) sender).FindControl("EarnedBadgeInc")).Checked;
                        obj.PhysicalPrizeNameInc =
                            ((CheckBox) ((DetailsView) sender).FindControl("PhysicalPrizeNameInc")).Checked;
                        obj.PhysicalPrizeDateInc =
                            ((CheckBox) ((DetailsView) sender).FindControl("PhysicalPrizeDateInc")).Checked;
                        obj.NumReviewsInc = ((CheckBox) ((DetailsView) sender).FindControl("NumReviewsInc")).Checked;
                        obj.ReviewAuthorInc = ((CheckBox) ((DetailsView) sender).FindControl("ReviewAuthorInc")).Checked;
                        obj.ReviewTitleInc = ((CheckBox) ((DetailsView) sender).FindControl("ReviewTitleInc")).Checked;
                        obj.ReviewDateInc = ((CheckBox) ((DetailsView) sender).FindControl("ReviewDateInc")).Checked;
                        obj.RandomDrawingNameInc =
                            ((CheckBox) ((DetailsView) sender).FindControl("RandomDrawingNameInc")).Checked;
                        obj.RandomDrawingNumInc =
                            ((CheckBox) ((DetailsView) sender).FindControl("RandomDrawingNumInc")).Checked;
                        obj.RandomDrawingDateInc =
                            ((CheckBox) ((DetailsView) sender).FindControl("RandomDrawingDateInc")).Checked;
                        obj.HasBeenDrawnInc = ((CheckBox) ((DetailsView) sender).FindControl("HasBeenDrawnInc")).Checked;
                        obj.HasRedeemendInc = ((CheckBox) ((DetailsView) sender).FindControl("HasRedeemendInc")).Checked;

                        obj.Score1From =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1From")).Text);
                        obj.Score1To =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1To")).Text);
                        obj.Score2From =
                            FormatHelper.SafeToInt(
                                ((TextBox)((DetailsView)sender).FindControl("Score2From")).Text);
                        obj.Score2To =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score2To")).Text);

                        obj.Score1PctFrom =
                                FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1PctFrom")).Text);
                        obj.Score1PctTo =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1PctTo")).Text);
                        obj.Score2PctFrom =
                            FormatHelper.SafeToInt(
                                ((TextBox)((DetailsView)sender).FindControl("Score2PctFrom")).Text);
                        obj.Score2PctTo =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score2PctTo")).Text);

                        obj.Score1Inc = ((CheckBox)((DetailsView)sender).FindControl("Score1Inc")).Checked;
                        obj.Score2Inc = ((CheckBox)((DetailsView)sender).FindControl("Score2Inc")).Checked;
                        obj.Score1PctInc = ((CheckBox)((DetailsView)sender).FindControl("Score1PctInc")).Checked;
                        obj.Score2PctInc = ((CheckBox)((DetailsView)sender).FindControl("Score2PctInc")).Checked;

                        obj.LastModDate = DateTime.Now;
                        obj.LastModUser = ((SRPUser) Session[SessionData.UserProfile.ToString()]).Username;
                            //"N/A";  // Get from session

                        if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                        {
                            obj.Update();
                           
                            odsData.DataBind();
                            dv.DataBind();
                            dv.ChangeMode(DetailsViewMode.Edit);

                            var masterPage = (IControlRoomMaster) Master;
                            masterPage.PageMessage = SRPResources.SaveOK;
                        }
                        else
                        {
                            var masterPage = (IControlRoomMaster) Master;
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
                        var masterPage = (IControlRoomMaster) Master;
                        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                    }
                }
            }
#endregion

#region save as template
            if (e.CommandName.ToLower() == "savetemplate")
            {
                var templateName = ((TextBox)((DetailsView)sender).FindControl("TemplateName")).Text.Trim();
                if (templateName=="")
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format("To create a report template from the current report format, you need to enter a Template Name.");
                    ((TextBox) ((DetailsView) sender).FindControl("TemplateName")).Focus();
                    return;
                }
                try
                {
                    //ADD 
                    var obj = new ReportTemplate();

                    obj.ProgId =
                        FormatHelper.SafeToInt(
                            ((DropDownList)((DetailsView)sender).FindControl("ProgId")).SelectedValue);
                    obj.ReportName = ((TextBox)((DetailsView)sender).FindControl("TemplateName")).Text;
                    obj.DisplayFilters = ((CheckBox)((DetailsView)sender).FindControl("DisplayFilters")).Checked;
                    
                    obj.DOBFrom =
                        FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("DOBFrom")).Text);
                    obj.DOBTo =
                        FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("DOBTo")).Text);
                    obj.AgeFrom =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AgeFrom")).Text);
                    obj.AgeTo = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("AgeTo")).Text);
                    obj.SchoolGrade = ((TextBox)((DetailsView)sender).FindControl("SchoolGrade")).Text;
                    obj.FirstName = ((TextBox)((DetailsView)sender).FindControl("FirstName")).Text;
                    obj.LastName = ((TextBox)((DetailsView)sender).FindControl("LastName")).Text;
                    obj.Gender = ((DropDownList)((DetailsView)sender).FindControl("Gender")).SelectedValue;
                    obj.EmailAddress = ((TextBox)((DetailsView)sender).FindControl("EmailAddress")).Text;
                    obj.PhoneNumber = ((TextBox)((DetailsView)sender).FindControl("PhoneNumber")).Text;
                    obj.City = ((TextBox)((DetailsView)sender).FindControl("City")).Text;
                    obj.State = ((TextBox)((DetailsView)sender).FindControl("State")).Text;
                    obj.ZipCode = ((TextBox)((DetailsView)sender).FindControl("ZipCode")).Text;
                    obj.County = ((TextBox)((DetailsView)sender).FindControl("County")).Text;
                    
                    obj.PrimaryLibrary =
                        FormatHelper.SafeToInt(
                            ((DropDownList)((DetailsView)sender).FindControl("PrimaryLibrary")).SelectedValue);
                    obj.SchoolName = ((DropDownList)((DetailsView)sender).FindControl("SchoolName")).SelectedValue;
                    obj.District = ((DropDownList)((DetailsView)sender).FindControl("District")).SelectedValue;
                    obj.SDistrict = ((DropDownList)((DetailsView)sender).FindControl("SDistrict")).SelectedValue;
                    
                    obj.Teacher = ((TextBox)((DetailsView)sender).FindControl("Teacher")).Text;
                    obj.GroupTeamName = ((TextBox)((DetailsView)sender).FindControl("GroupTeamName")).Text;
                    obj.SchoolType =
                        FormatHelper.SafeToInt(
                            ((DropDownList)((DetailsView)sender).FindControl("SchoolType")).SelectedValue);

                    obj.LiteracyLevel1 =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LiteracyLevel1")).Text);
                    obj.LiteracyLevel2 =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("LiteracyLevel2")).Text);

                    obj.Custom1 = ((TextBox)((DetailsView)sender).FindControl("Custom1")).Text;
                    obj.Custom2 = ((TextBox)((DetailsView)sender).FindControl("Custom2")).Text;
                    obj.Custom3 = ((TextBox)((DetailsView)sender).FindControl("Custom3")).Text;
                    obj.Custom4 = ((TextBox)((DetailsView)sender).FindControl("Custom4")).Text;
                    obj.Custom5 = ((TextBox)((DetailsView)sender).FindControl("Custom5")).Text;

                    obj.RegistrationDateStart =
                        FormatHelper.SafeToDateTime(
                            ((TextBox)((DetailsView)sender).FindControl("RegistrationDateStart")).Text);
                    obj.RegistrationDateEnd =
                        FormatHelper.SafeToDateTime(
                            ((TextBox)((DetailsView)sender).FindControl("RegistrationDateEnd")).Text);

                    obj.PointsMin =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PointsMin")).Text);
                    obj.PointsMax =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PointsMax")).Text);
                    obj.PointsStart =
                        FormatHelper.SafeToDateTime(
                            ((TextBox)((DetailsView)sender).FindControl("PointsStart")).Text);
                    obj.PointsEnd =
                        FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("PointsEnd")).Text);
                    obj.EventCode = ((TextBox)((DetailsView)sender).FindControl("EventCode")).Text;
                    obj.EarnedBadge =
                        FormatHelper.SafeToInt(
                            ((DropDownList)((DetailsView)sender).FindControl("EarnedBadge")).SelectedValue);
                    obj.PhysicalPrizeEarned =
                        ((TextBox)((DetailsView)sender).FindControl("PhysicalPrizeEarned")).Text;
                    obj.PhysicalPrizeRedeemed =
                        ((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeRedeemed")).Checked;
                    obj.PhysicalPrizeStartDate =
                        FormatHelper.SafeToDateTime(
                            ((TextBox)((DetailsView)sender).FindControl("PhysicalPrizeStartDate")).Text);
                    obj.PhysicalPrizeEndDate =
                        FormatHelper.SafeToDateTime(
                            ((TextBox)((DetailsView)sender).FindControl("PhysicalPrizeEndDate")).Text);

                    obj.ReviewsMin =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("ReviewsMin")).Text);
                    obj.ReviewsMax =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("ReviewsMax")).Text);
                    obj.ReviewTitle = ((TextBox)((DetailsView)sender).FindControl("ReviewTitle")).Text;
                    obj.ReviewAuthor = ((TextBox)((DetailsView)sender).FindControl("ReviewAuthor")).Text;
                    obj.ReviewStartDate =
                        FormatHelper.SafeToDateTime(
                            ((TextBox)((DetailsView)sender).FindControl("ReviewStartDate")).Text);
                    obj.ReviewEndDate =
                        FormatHelper.SafeToDateTime(
                            ((TextBox)((DetailsView)sender).FindControl("ReviewEndDate")).Text);
                    obj.RandomDrawingName = ((TextBox)((DetailsView)sender).FindControl("RandomDrawingName")).Text;
                    obj.RandomDrawingNum =
                        FormatHelper.SafeToInt(
                            ((TextBox)((DetailsView)sender).FindControl("RandomDrawingNum")).Text);
                    obj.RandomDrawingStartDate =
                        FormatHelper.SafeToDateTime(
                            ((TextBox)((DetailsView)sender).FindControl("RandomDrawingStartDate")).Text);
                    obj.RandomDrawingEndDate =
                        FormatHelper.SafeToDateTime(
                            ((TextBox)((DetailsView)sender).FindControl("RandomDrawingEndDate")).Text);
                    obj.HasBeenDrawn = ((CheckBox)((DetailsView)sender).FindControl("HasBeenDrawn")).Checked;
                    obj.HasRedeemend = ((CheckBox)((DetailsView)sender).FindControl("HasRedeemend")).Checked;

                    
                    
                    obj.Score1From =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1From")).Text);
                    obj.Score1To =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1To")).Text);
                    obj.Score2From =
                        FormatHelper.SafeToInt(
                            ((TextBox)((DetailsView)sender).FindControl("Score2From")).Text);
                    obj.Score2To =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score2To")).Text);

                    obj.Score1PctFrom =
                            FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1PctFrom")).Text);
                    obj.Score1PctTo =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score1PctTo")).Text);
                    obj.Score2PctFrom =
                        FormatHelper.SafeToInt(
                            ((TextBox)((DetailsView)sender).FindControl("Score2PctFrom")).Text);
                    obj.Score2PctTo =
                        FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("Score2PctTo")).Text);

                    obj.Score1Inc = ((CheckBox)((DetailsView)sender).FindControl("Score1Inc")).Checked;
                    obj.Score2Inc = ((CheckBox)((DetailsView)sender).FindControl("Score2Inc")).Checked;
                    obj.Score2PctInc = ((CheckBox)((DetailsView)sender).FindControl("Score2PctInc")).Checked;
                    obj.Score2PctInc = ((CheckBox)((DetailsView)sender).FindControl("Score2PctInc")).Checked;


                    obj.PIDInc = ((CheckBox)((DetailsView)sender).FindControl("PIDInc")).Checked;
                    obj.UsernameInc = ((CheckBox)((DetailsView)sender).FindControl("UsernameInc")).Checked;
                    obj.DOBInc = ((CheckBox)((DetailsView)sender).FindControl("DOBInc")).Checked;
                    obj.AgeInc = ((CheckBox)((DetailsView)sender).FindControl("AgeInc")).Checked;
                    obj.SchoolGradeInc = ((CheckBox)((DetailsView)sender).FindControl("SchoolGradeInc")).Checked;
                    obj.FirstNameInc = ((CheckBox)((DetailsView)sender).FindControl("FirstNameInc")).Checked;
                    obj.LastNameInc = ((CheckBox)((DetailsView)sender).FindControl("LastNameInc")).Checked;
                    obj.GenderInc = ((CheckBox)((DetailsView)sender).FindControl("GenderInc")).Checked;
                    obj.EmailAddressInc = ((CheckBox)((DetailsView)sender).FindControl("EmailAddressInc")).Checked;
                    obj.PhoneNumberInc = ((CheckBox)((DetailsView)sender).FindControl("PhoneNumberInc")).Checked;
                    obj.CityInc = ((CheckBox)((DetailsView)sender).FindControl("CityInc")).Checked;
                    obj.StateInc = ((CheckBox)((DetailsView)sender).FindControl("StateInc")).Checked;
                    obj.ZipCodeInc = ((CheckBox)((DetailsView)sender).FindControl("ZipCodeInc")).Checked;
                    obj.CountyInc = ((CheckBox)((DetailsView)sender).FindControl("CountyInc")).Checked;
                    obj.PrimaryLibraryInc =
                        ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibraryInc")).Checked;
                    obj.SchoolNameInc = ((CheckBox)((DetailsView)sender).FindControl("SchoolNameInc")).Checked;
                    obj.DistrictInc = ((CheckBox)((DetailsView)sender).FindControl("DistrictInc")).Checked;
                    obj.TeacherInc = ((CheckBox)((DetailsView)sender).FindControl("TeacherInc")).Checked;
                    obj.GroupTeamNameInc =
                        ((CheckBox)((DetailsView)sender).FindControl("GroupTeamNameInc")).Checked;
                    obj.SchoolTypeInc = ((CheckBox)((DetailsView)sender).FindControl("SchoolTypeInc")).Checked;
                    obj.LiteracyLevel1Inc =
                        ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1Inc")).Checked;
                    obj.LiteracyLevel2Inc =
                        ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2Inc")).Checked;
                    obj.Custom1Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom1Inc")).Checked;
                    obj.Custom2Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom2Inc")).Checked;
                    obj.Custom3Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom3Inc")).Checked;
                    obj.Custom4Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom4Inc")).Checked;
                    obj.Custom5Inc = ((CheckBox)((DetailsView)sender).FindControl("Custom5Inc")).Checked;
                    obj.RegistrationDateInc =
                        ((CheckBox)((DetailsView)sender).FindControl("RegistrationDateInc")).Checked;
                    obj.PointsInc = ((CheckBox)((DetailsView)sender).FindControl("PointsInc")).Checked;
                    obj.EarnedBadgeInc = ((CheckBox)((DetailsView)sender).FindControl("EarnedBadgeInc")).Checked;
                    obj.PhysicalPrizeNameInc =
                        ((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeNameInc")).Checked;
                    obj.PhysicalPrizeDateInc =
                        ((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeDateInc")).Checked;
                    obj.NumReviewsInc = ((CheckBox)((DetailsView)sender).FindControl("NumReviewsInc")).Checked;
                    obj.ReviewAuthorInc = ((CheckBox)((DetailsView)sender).FindControl("ReviewAuthorInc")).Checked;
                    obj.ReviewTitleInc = ((CheckBox)((DetailsView)sender).FindControl("ReviewTitleInc")).Checked;
                    obj.ReviewDateInc = ((CheckBox)((DetailsView)sender).FindControl("ReviewDateInc")).Checked;
                    obj.RandomDrawingNameInc =
                        ((CheckBox)((DetailsView)sender).FindControl("RandomDrawingNameInc")).Checked;
                    obj.RandomDrawingNumInc =
                        ((CheckBox)((DetailsView)sender).FindControl("RandomDrawingNumInc")).Checked;
                    obj.RandomDrawingDateInc =
                        ((CheckBox)((DetailsView)sender).FindControl("RandomDrawingDateInc")).Checked;
                    obj.HasBeenDrawnInc = ((CheckBox)((DetailsView)sender).FindControl("HasBeenDrawnInc")).Checked;
                    obj.HasRedeemendInc = ((CheckBox)((DetailsView)sender).FindControl("HasRedeemendInc")).Checked;

                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;
                    //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = "Report Template has been successfully created!";
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
            
                

#endregion

#region load from template
            if (e.CommandName.ToLower() == "loadtemplate")
            {
                try
                {
                    var RTID =
                            FormatHelper.SafeToInt(
                                ((DropDownList)((DetailsView)sender).FindControl("RTID")).SelectedValue);

                    if (RTID == 0) return;

                    var obj = ReportTemplate.FetchObject(RTID);

                    var dd = ((DropDownList) ((DetailsView) sender).FindControl("ProgId"));
                    var i = dd.Items.FindByValue(obj.ProgId.ToString());
                    if (i != null) dd.SelectedValue = (obj.ProgId.ToString());

                    dd = ((DropDownList)((DetailsView)sender).FindControl("Gender"));
                    i = dd.Items.FindByValue(obj.Gender);
                    if (i != null) dd.SelectedValue = (obj.Gender);

                    dd = ((DropDownList)((DetailsView)sender).FindControl("PrimaryLibrary"));
                    i = dd.Items.FindByValue(obj.PrimaryLibrary.ToString());
                    if (i != null) dd.SelectedValue = (obj.PrimaryLibrary.ToString());

                    dd = ((DropDownList)((DetailsView)sender).FindControl("SchoolName"));
                    i = dd.Items.FindByValue(obj.SchoolName.ToString());
                    if (i != null) dd.SelectedValue = (obj.SchoolName.ToString());

                    dd = ((DropDownList)((DetailsView)sender).FindControl("District"));
                    i = dd.Items.FindByValue(obj.District.ToString());
                    if (i != null) dd.SelectedValue = (obj.District.ToString());

                    dd = ((DropDownList)((DetailsView)sender).FindControl("SDistrict"));
                    i = dd.Items.FindByValue(obj.SDistrict.ToString());
                    if (i != null) dd.SelectedValue = (obj.SDistrict.ToString());


                    dd = ((DropDownList)((DetailsView)sender).FindControl("SchoolType"));
                    i = dd.Items.FindByValue(obj.SchoolType.ToString());
                    if (i != null) dd.SelectedValue = (obj.SchoolType.ToString());

                    dd = ((DropDownList)((DetailsView)sender).FindControl("EarnedBadge"));
                    i = dd.Items.FindByValue(obj.EarnedBadge.ToString());
                    if (i != null) dd.SelectedValue = (obj.EarnedBadge.ToString());


                    //((TextBox)((DetailsView)sender).FindControl("ReportName")).Text = obj.ReportName;
                    ((TextBox)((DetailsView)sender).FindControl("SchoolGrade")).Text = obj.SchoolGrade;
                    ((TextBox)((DetailsView)sender).FindControl("FirstName")).Text = obj.FirstName;
                    ((TextBox)((DetailsView)sender).FindControl("LastName")).Text = obj.LastName;
                    ((TextBox)((DetailsView)sender).FindControl("PhoneNumber")).Text = obj.PhoneNumber;
                    ((TextBox)((DetailsView)sender).FindControl("City")).Text = obj.City;
                    ((TextBox)((DetailsView)sender).FindControl("State")).Text = obj.State;
                    ((TextBox)((DetailsView)sender).FindControl("ZipCode")).Text = obj.ZipCode;
                    ((TextBox)((DetailsView)sender).FindControl("County")).Text = obj.County;
                    //((TextBox)((DetailsView)sender).FindControl("SchoolName")).Text = obj.SchoolName;
                    //((TextBox)((DetailsView)sender).FindControl("District")).Text = obj.District;
                    ((TextBox)((DetailsView)sender).FindControl("Teacher")).Text = obj.Teacher;
                    ((TextBox)((DetailsView)sender).FindControl("GroupTeamName")).Text = obj.GroupTeamName;
                    ((TextBox)((DetailsView)sender).FindControl("Custom1")).Text = obj.Custom1;
                    ((TextBox)((DetailsView)sender).FindControl("Custom2")).Text = obj.Custom2;
                    ((TextBox)((DetailsView)sender).FindControl("Custom3")).Text = obj.Custom3;
                    ((TextBox)((DetailsView)sender).FindControl("Custom4")).Text = obj.Custom4;
                    ((TextBox)((DetailsView)sender).FindControl("Custom5")).Text = obj.Custom5;
                    ((TextBox)((DetailsView)sender).FindControl("EventCode")).Text = obj.EventCode;
                    ((TextBox)((DetailsView)sender).FindControl("PhysicalPrizeEarned")).Text = obj.PhysicalPrizeEarned;
                    ((TextBox)((DetailsView)sender).FindControl("ReviewTitle")).Text = obj.ReviewTitle;
                    ((TextBox)((DetailsView)sender).FindControl("ReviewAuthor")).Text = obj.ReviewAuthor;
                    ((TextBox)((DetailsView)sender).FindControl("RandomDrawingName")).Text = obj.RandomDrawingName;


                    ((TextBox)((DetailsView)sender).FindControl("DOBFrom")).Text = FormatHelper.ToWidgetDisplayDate(obj.DOBFrom);
                    ((TextBox)((DetailsView)sender).FindControl("DOBTo")).Text = FormatHelper.ToWidgetDisplayDate(obj.DOBTo);
                    ((TextBox)((DetailsView)sender).FindControl("RegistrationDateStart")).Text = FormatHelper.ToWidgetDisplayDate(obj.RegistrationDateStart);
                    ((TextBox)((DetailsView)sender).FindControl("RegistrationDateEnd")).Text = FormatHelper.ToWidgetDisplayDate(obj.RegistrationDateEnd);
                    ((TextBox)((DetailsView)sender).FindControl("PointsStart")).Text = FormatHelper.ToWidgetDisplayDate(obj.PointsStart);
                    ((TextBox)((DetailsView)sender).FindControl("PointsEnd")).Text = FormatHelper.ToWidgetDisplayDate(obj.PointsEnd);
                    ((TextBox)((DetailsView)sender).FindControl("PhysicalPrizeStartDate")).Text = FormatHelper.ToWidgetDisplayDate(obj.PhysicalPrizeStartDate);
                    ((TextBox)((DetailsView)sender).FindControl("PhysicalPrizeEndDate")).Text = FormatHelper.ToWidgetDisplayDate(obj.PhysicalPrizeEndDate);
                    ((TextBox)((DetailsView)sender).FindControl("ReviewStartDate")).Text = FormatHelper.ToWidgetDisplayDate(obj.ReviewStartDate);
                    ((TextBox)((DetailsView)sender).FindControl("ReviewEndDate")).Text = FormatHelper.ToWidgetDisplayDate(obj.ReviewEndDate);
                    ((TextBox)((DetailsView)sender).FindControl("RandomDrawingStartDate")).Text = FormatHelper.ToWidgetDisplayDate(obj.RandomDrawingStartDate);
                    ((TextBox)((DetailsView)sender).FindControl("RandomDrawingEndDate")).Text = FormatHelper.ToWidgetDisplayDate(obj.RandomDrawingEndDate);


                    ((TextBox)((DetailsView)sender).FindControl("AgeFrom")).Text = FormatHelper.ToWidgetDisplayInt(obj.AgeFrom);
                    ((TextBox)((DetailsView)sender).FindControl("AgeTo")).Text = FormatHelper.ToWidgetDisplayInt(obj.AgeTo);
                    ((TextBox)((DetailsView)sender).FindControl("LiteracyLevel1")).Text = FormatHelper.ToWidgetDisplayInt(obj.LiteracyLevel1);
                    ((TextBox)((DetailsView)sender).FindControl("LiteracyLevel2")).Text = FormatHelper.ToWidgetDisplayInt(obj.LiteracyLevel2);
                    ((TextBox)((DetailsView)sender).FindControl("PointsMin")).Text = FormatHelper.ToWidgetDisplayInt(obj.PointsMin);
                    ((TextBox)((DetailsView)sender).FindControl("PointsMax")).Text = FormatHelper.ToWidgetDisplayInt(obj.PointsMax);
                    ((TextBox)((DetailsView)sender).FindControl("ReviewsMin")).Text = FormatHelper.ToWidgetDisplayInt(obj.ReviewsMin);
                    ((TextBox)((DetailsView)sender).FindControl("ReviewsMax")).Text = FormatHelper.ToWidgetDisplayInt(obj.ReviewsMax);
                    ((TextBox)((DetailsView)sender).FindControl("RandomDrawingNum")).Text = FormatHelper.ToWidgetDisplayInt(obj.RandomDrawingNum);


                    ((CheckBox)((DetailsView)sender).FindControl("DisplayFilters")).Checked = obj.DisplayFilters;
                    ((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeRedeemed")).Checked = obj.PhysicalPrizeRedeemed;
                    ((CheckBox)((DetailsView)sender).FindControl("HasBeenDrawn")).Checked = obj.HasBeenDrawn;
                    ((CheckBox)((DetailsView)sender).FindControl("HasRedeemend")).Checked = obj.HasRedeemend;
                    ((CheckBox)((DetailsView)sender).FindControl("PIDInc")).Checked = obj.PIDInc;
                    ((CheckBox)((DetailsView)sender).FindControl("UsernameInc")).Checked = obj.UsernameInc;
                    ((CheckBox)((DetailsView)sender).FindControl("AgeInc")).Checked = obj.AgeInc;
                    ((CheckBox)((DetailsView)sender).FindControl("SchoolGradeInc")).Checked = obj.SchoolGradeInc;
                    ((CheckBox)((DetailsView)sender).FindControl("FirstNameInc")).Checked = obj.FirstNameInc;
                    ((CheckBox)((DetailsView)sender).FindControl("LastNameInc")).Checked = obj.LastNameInc;
                    ((CheckBox)((DetailsView)sender).FindControl("GenderInc")).Checked = obj.GenderInc;
                    ((CheckBox)((DetailsView)sender).FindControl("EmailAddressInc")).Checked = obj.EmailAddressInc;
                    ((CheckBox)((DetailsView)sender).FindControl("PhoneNumberInc")).Checked = obj.PhoneNumberInc;
                    ((CheckBox)((DetailsView)sender).FindControl("CityInc")).Checked = obj.CityInc;
                    ((CheckBox)((DetailsView)sender).FindControl("StateInc")).Checked = obj.StateInc;
                    ((CheckBox)((DetailsView)sender).FindControl("ZipCodeInc")).Checked = obj.ZipCodeInc;
                    ((CheckBox)((DetailsView)sender).FindControl("CountyInc")).Checked = obj.CountyInc;
                    ((CheckBox)((DetailsView)sender).FindControl("PrimaryLibraryInc")).Checked = obj.PrimaryLibraryInc;
                    ((CheckBox)((DetailsView)sender).FindControl("SchoolNameInc")).Checked = obj.SchoolNameInc;
                    ((CheckBox)((DetailsView)sender).FindControl("TeacherInc")).Checked = obj.TeacherInc;
                    ((CheckBox)((DetailsView)sender).FindControl("GroupTeamNameInc")).Checked = obj.GroupTeamNameInc;
                    ((CheckBox)((DetailsView)sender).FindControl("SchoolTypeInc")).Checked = obj.SchoolTypeInc;
                    ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1Inc")).Checked = obj.LiteracyLevel1Inc;
                    ((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2Inc")).Checked = obj.LiteracyLevel2Inc;
                    ((CheckBox)((DetailsView)sender).FindControl("Custom1Inc")).Checked = obj.Custom1Inc;
                    ((CheckBox)((DetailsView)sender).FindControl("Custom2Inc")).Checked = obj.Custom2Inc;
                    ((CheckBox)((DetailsView)sender).FindControl("Custom3Inc")).Checked = obj.Custom3Inc;
                    ((CheckBox)((DetailsView)sender).FindControl("Custom4Inc")).Checked = obj.Custom4Inc;
                    ((CheckBox)((DetailsView)sender).FindControl("Custom5Inc")).Checked = obj.Custom5Inc;
                    ((CheckBox)((DetailsView)sender).FindControl("RegistrationDateInc")).Checked = obj.RegistrationDateInc;
                    ((CheckBox)((DetailsView)sender).FindControl("PointsInc")).Checked = obj.PointsInc;
                    ((CheckBox)((DetailsView)sender).FindControl("EarnedBadgeInc")).Checked = obj.EarnedBadgeInc;
                    ((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeNameInc")).Checked = obj.PhysicalPrizeNameInc;
                    ((CheckBox)((DetailsView)sender).FindControl("NumReviewsInc")).Checked = obj.NumReviewsInc;
                    ((CheckBox)((DetailsView)sender).FindControl("ReviewAuthorInc")).Checked = obj.ReviewAuthorInc;
                    ((CheckBox)((DetailsView)sender).FindControl("ReviewTitleInc")).Checked = obj.ReviewTitleInc;
                    ((CheckBox)((DetailsView)sender).FindControl("ReviewDateInc")).Checked = obj.ReviewDateInc;
                    ((CheckBox)((DetailsView)sender).FindControl("RandomDrawingNameInc")).Checked = obj.RandomDrawingNameInc;
                    ((CheckBox)((DetailsView)sender).FindControl("RandomDrawingNumInc")).Checked = obj.RandomDrawingNumInc;
                    ((CheckBox)((DetailsView)sender).FindControl("RandomDrawingDateInc")).Checked = obj.RandomDrawingDateInc;
                    ((CheckBox)((DetailsView)sender).FindControl("HasBeenDrawnInc")).Checked = obj.HasBeenDrawnInc;
                    ((CheckBox)((DetailsView)sender).FindControl("HasRedeemendInc")).Checked = obj.HasRedeemendInc;

                    ((TextBox)((DetailsView)sender).FindControl("Score1From")).Text = FormatHelper.ToWidgetDisplayInt(obj.Score1From);
                    ((TextBox)((DetailsView)sender).FindControl("Score2From")).Text = FormatHelper.ToWidgetDisplayInt(obj.Score2From);
                    ((TextBox)((DetailsView)sender).FindControl("Score1PctFrom")).Text = FormatHelper.ToWidgetDisplayInt(obj.Score1PctFrom);
                    ((TextBox)((DetailsView)sender).FindControl("Score2PctFrom")).Text = FormatHelper.ToWidgetDisplayInt(obj.Score2PctFrom);

                    ((TextBox)((DetailsView)sender).FindControl("Score1To")).Text = FormatHelper.ToWidgetDisplayInt(obj.Score1To);
                    ((TextBox)((DetailsView)sender).FindControl("Score2To")).Text = FormatHelper.ToWidgetDisplayInt(obj.Score2To);
                    ((TextBox)((DetailsView)sender).FindControl("Score1PctTo")).Text = FormatHelper.ToWidgetDisplayInt(obj.Score1PctTo);
                    ((TextBox)((DetailsView)sender).FindControl("Score2PctTo")).Text = FormatHelper.ToWidgetDisplayInt(obj.Score2PctTo);

                    ((CheckBox)((DetailsView)sender).FindControl("Score1Inc")).Checked = obj.Score1Inc;
                    ((CheckBox)((DetailsView)sender).FindControl("Score2Inc")).Checked = obj.Score2Inc;
                    ((CheckBox)((DetailsView)sender).FindControl("Score1PctInc")).Checked = obj.Score1PctInc;
                    ((CheckBox)((DetailsView)sender).FindControl("Score2PctInc")).Checked = obj.Score2PctInc;

                    var masterPage = (IControlRoomMaster)Master;
                    if (masterPage != null) masterPage.PageMessage = "Report template loaded.";
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
#endregion


#region RUN REPORT
            if (e.CommandName.ToLower() == "runreport")
            {
                try
                {
                    SqlParameter[] arrParams = null;
                    var retColumns= string.Empty;
                    var whereClause= string.Empty;
                    var fromClause = "Patron p ";

                    var filterStr= string.Empty;
                    //var tmpFilter= string.Empty;
                    var minorSep = "~";
                    var majorSep = "|";

                    if (((CheckBox)((DetailsView)sender).FindControl("PIDInc")).Checked)
                        retColumns = Coalesce(retColumns, "p.PID as PatronID", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("UsernameInc")).Checked)
                        retColumns = Coalesce(retColumns, "p.Username", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("DOBInc")).Checked)
                        retColumns = Coalesce(retColumns, "convert(varchar, DOB, 101) as DOB", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("AgeInc")).Checked)
                        retColumns = Coalesce(retColumns, "case when Age > 0 then Age else null end as Age", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("SchoolGradeInc")).Checked)
                        retColumns = Coalesce(retColumns, "SchoolGrade", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("FirstNameInc")).Checked)
                        retColumns = Coalesce(retColumns, "FirstName", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("LastNameInc")).Checked)
                        retColumns = Coalesce(retColumns, "LastName", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("GenderInc")).Checked)
                        retColumns = Coalesce(retColumns, "Gender", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("EmailAddressInc")).Checked)
                        retColumns = Coalesce(retColumns, "EmailAddress", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("PhoneNumberInc")).Checked)
                        retColumns = Coalesce(retColumns, "PhoneNumber", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("CityInc")).Checked)
                        retColumns = Coalesce(retColumns, "City", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("StateInc")).Checked)
                        retColumns = Coalesce(retColumns, "State", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("ZipCodeInc")).Checked)
                        retColumns = Coalesce(retColumns, "ZipCode", ",");

                    if (((CheckBox)((DetailsView)sender).FindControl("Score1Inc")).Checked)
                        retColumns = Coalesce(retColumns, "Score1", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("Score2Inc")).Checked)
                        retColumns = Coalesce(retColumns, "Score2", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("Score1PctInc")).Checked)
                        retColumns = Coalesce(retColumns, "Score1Pct", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("Score2PctInc")).Checked)
                        retColumns = Coalesce(retColumns, "Score2Pct", ",");


                    if (((CheckBox)((DetailsView)sender).FindControl("DistrictInc")).Checked)
                    {
                        // CODE Translate
                        fromClause = Coalesce(fromClause, "left outer join Code C4 on C4.CID = p.District", " ");
                        retColumns = Coalesce(retColumns, "C4.Code as District", ",");
                    }
                    
                    if (((CheckBox)((DetailsView)sender).FindControl("PrimaryLibraryInc")).Checked)
                    {
                        // CODE Translate
                        fromClause = Coalesce(fromClause, "left outer join Code C1 on C1.CID = p.PrimaryLibrary", " ");
                        retColumns = Coalesce(retColumns, "C1.Code as PrimaryLibrary", ",");
                    }

                    if (((CheckBox)((DetailsView)sender).FindControl("SDistrictInc")).Checked)
                    {
                        // CODE Translate
                        fromClause = Coalesce(fromClause, "left outer join Code C5 on C5.CID = p.SDistrict", " ");
                        retColumns = Coalesce(retColumns, "C5.Code as SDistrict", ",");
                    }

                    if (((CheckBox)((DetailsView)sender).FindControl("SchoolNameInc")).Checked)
                    {
                        // CODE Translate
                        fromClause = Coalesce(fromClause, "left outer join Code C3 on C3.CID = p.SchoolName", " ");
                        retColumns = Coalesce(retColumns, "C3.Code as SchoolName", ",");
                    }


                    //if (((CheckBox)((DetailsView)sender).FindControl("SchoolNameInc")).Checked)
                    //    retColumns = Coalesce(retColumns, "SchoolName", ",");
                    //if (((CheckBox)((DetailsView)sender).FindControl("DistrictInc")).Checked)
                    //    retColumns = Coalesce(retColumns, "District", ",");

                    if (((CheckBox)((DetailsView)sender).FindControl("TeacherInc")).Checked)
                        retColumns = Coalesce(retColumns, "Teacher", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("GroupTeamNameInc")).Checked)
                        retColumns = Coalesce(retColumns, "GroupTeamName", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("SchoolTypeInc")).Checked)
                    {
                        //CODE Translate1
                        fromClause = Coalesce(fromClause, "left outer join Code C2 on C2.CID = p.SchoolType", " ");
                        retColumns = Coalesce(retColumns, "C2.Code as SchoolType", ","); 
                    }             
                        
                    if (((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel1Inc")).Checked)
                        retColumns = Coalesce(retColumns, "case when LiteracyLevel1 > 0 then LiteracyLevel1 else null end as LiteracyLevel1", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("LiteracyLevel2Inc")).Checked)
                        retColumns = Coalesce(retColumns, "case when LiteracyLevel2 > 0 then LiteracyLevel2 else null end as LiteracyLevel2", ",");
                    
                    if (((CheckBox)((DetailsView)sender).FindControl("Custom1Inc")).Checked)                
                        retColumns = Coalesce(retColumns, "Custom1", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("Custom2Inc")).Checked)
                        retColumns = Coalesce(retColumns, "Custom2", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("Custom3Inc")).Checked)
                        retColumns = Coalesce(retColumns, "Custom3", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("Custom4Inc")).Checked)
                        retColumns = Coalesce(retColumns, "Custom4", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("Custom5Inc")).Checked)
                        retColumns = Coalesce(retColumns, "Custom5", ",");
                    if (((CheckBox)((DetailsView)sender).FindControl("RegistrationDateInc")).Checked)
                        retColumns = Coalesce(retColumns, "convert(varchar, RegistrationDate, 101) as RegistrationDate", ",");


                    ProcessSimpleStringFilter((DetailsView)sender, "FirstName", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "LastName", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "SchoolGrade", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "EmailAddress", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "PhoneNumber", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "City", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "State", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "ZipCode", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "County", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "Teacher", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "GroupTeamName", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);

                    //ProcessSimpleStringFilter((DetailsView)sender, "District", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    //ProcessSimpleStringFilter((DetailsView)sender, "SchoolName", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);


                    string txt;
                    string parmField;
                    int _int;
                    DateTime _dt;
                    string filterField;

                    //ProgID
                    fromClause = Coalesce(fromClause, "left outer join Programs PGM on PGM.PID = p.ProgId", " ");
                    retColumns = Coalesce(retColumns, "PGM.AdminName as Program", ",");
                    parmField = "ProgID";
                    txt = ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedValue;
                    _int = int.Parse(txt);
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + parmField, _int));
                        whereClause = Coalesce(whereClause, string.Format("(p.{0} = @{1})", parmField, parmField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", "Program", minorSep, ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedItem.Text), majorSep);
                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                     
                    txt = ((DropDownList)((DetailsView)sender).FindControl("Gender")).SelectedValue.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + "Gender", txt));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{0})", "Gender"), " AND ");
                        //retColumns = Coalesce(retColumns, filterField, ",");
                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", "Gender", minorSep, ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedItem.Text), majorSep);
                    }

                    parmField = "PrimaryLibrary";
                    txt = ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedValue;
                    _int = txt.SafeToInt();
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + parmField, _int));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{1})", parmField, parmField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", parmField, minorSep, ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedItem.Text), majorSep);
                    }

                    parmField = "District";
                    txt = ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedValue;
                    _int = txt.SafeToInt();
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + parmField, _int));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{1})", parmField, parmField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", "Library District", minorSep, ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedItem.Text), majorSep);
                    }

                    parmField = "SDistrict";
                    txt = ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedValue;
                    _int = txt.SafeToInt();
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + parmField, _int));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{1})", parmField, parmField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", "School District", minorSep, ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedItem.Text), majorSep);
                    }

                    parmField = "SchoolName";
                    txt = ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedValue;
                    _int = txt.SafeToInt();
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + parmField, _int));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{1})", parmField, parmField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", "School Name", minorSep, ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedItem.Text), majorSep);
                    }


                    parmField = "SchoolType";
                    txt = ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedValue;
                    _int = txt.SafeToInt();
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + parmField, _int));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{1})", parmField, parmField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", parmField, minorSep, ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedItem.Text), majorSep);
                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    parmField = "LiteracyLevel1";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(parmField))).Text.Trim();
                    _int = txt.SafeToInt();
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + parmField, _int));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{1})", parmField, parmField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", parmField, minorSep, txt), majorSep);
                    }

                    parmField = "LiteracyLevel2";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(parmField))).Text.Trim();
                    _int = txt.SafeToInt();
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + parmField, _int));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{1})", parmField, parmField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", parmField, minorSep, txt), majorSep);
                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    ProcessSimpleStringFilter((DetailsView)sender, "Custom1", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "Custom2", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "Custom3", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "Custom4", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);
                    ProcessSimpleStringFilter((DetailsView)sender, "Custom5", ref whereClause, ref retColumns, ref arrParams, ref filterStr, minorSep, majorSep);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    filterField = "Score1From";
                    parmField = "Score1";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} >= {2}", parmField, minorSep, txt), majorSep);
                    }

                    filterField = "Score2From";
                    parmField = "Score2";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} >= {2}", parmField, minorSep, txt), majorSep);
                    }

                    filterField = "Score1To";
                    parmField = "Score1";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} <= {2}", parmField, minorSep, txt), majorSep);
                    }

                    filterField = "Score2To";
                    parmField = "Score2";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} <= {2}", parmField, minorSep, txt), majorSep);
                    }


                    filterField = "Score1PctFrom";
                    parmField = "Score1Pct";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} >= {2}", parmField, minorSep, txt), majorSep);
                    }

                    filterField = "Score2PctFrom";
                    parmField = "Score2Pct";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} >= {2}", parmField, minorSep, txt), majorSep);
                    }

                    filterField = "Score1PctTo";
                    parmField = "Score1Pct";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} <= {2}", parmField, minorSep, txt), majorSep);
                    }

                    filterField = "Score2PctTo";
                    parmField = "Score2Pct";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} <= {2}", parmField, minorSep, txt), majorSep);
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    filterField = "AgeFrom";
                    parmField = "Age";
                    txt = ((TextBox) (((DetailsView) sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, txt), majorSep);
                    }

                    filterField = "AgeTo";
                    parmField = "Age";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, txt), majorSep);
                    }

                    filterField = "DOBFrom";
                    parmField = "DOB";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, txt), majorSep);
                    }

                    filterField = "DOBTo";
                    parmField = "DOB";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, txt), majorSep);
                    }


                    filterField = "RegistrationDateStart";
                    parmField = "RegistrationDate";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, txt), majorSep);
                    }

                    filterField = "RegistrationDateEnd";
                    parmField = "RegistrationDate";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, txt), majorSep);
                    }

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    var needPointsDate = false;
                    if (((CheckBox)((DetailsView)sender).FindControl("PointsInc")).Checked)
                    {
                        retColumns = Coalesce(retColumns, "(select SUM(convert(BIGINT, NumPoints)) from PatronPoints pp where pp.PID = p.PID AND (@PointsStart is null or pp.AwardDate >= @PointsStart) and (@PointsEnd is null or pp.AwardDate <= @PointsEnd)) as NumPoints", ",");
                        needPointsDate = true;
                    }

                    filterField = "PointsMin";
                    parmField = "(select SUM(convert(BIGINT, NumPoints)) from PatronPoints pp where pp.PID = p.PID AND (@PointsStart is null or pp.AwardDate >= @PointsStart) and (@PointsEnd is null or pp.AwardDate <= @PointsEnd))";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1} or @{1} is null)", parmField, filterField), " AND ");
                        needPointsDate = true;

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, txt), majorSep);
                    }

                    filterField = "PointsMax";
                    parmField = "(select SUM(convert(BIGINT, NumPoints)) from PatronPoints pp where pp.PID = p.PID AND (@PointsStart is null or pp.AwardDate >= @PointsStart) and (@PointsEnd is null or pp.AwardDate <= @PointsEnd))";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1} or @{1} is null)", parmField, filterField), " AND ");
                        needPointsDate = true;

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, txt), majorSep);
                    }


                    filterField = "PointsStart";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim()), majorSep);
                    }
                    else if (needPointsDate)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, DBNull.Value));
                    }

                    filterField = "PointsEnd";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim()), majorSep);
                    }
                    else if (needPointsDate)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, DBNull.Value));
                    }


                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

                    filterField = "EventCode";
                    txt = ((TextBox) (((DetailsView) sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        retColumns = Coalesce(retColumns, "pp1.EventCode", ",");
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, txt));
                        whereClause = Coalesce(whereClause, string.Format("(pp1.{0} like '%' + @{1} + '%')", filterField, filterField), " AND ");
                        fromClause = Coalesce(fromClause, "left join PatronPoints pp1 on p.PID = pp1.PID", " ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} like {2}", filterField, minorSep, txt), majorSep);
                    }




                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

                    var needBadges = false;
                    if (((CheckBox)((DetailsView)sender).FindControl("EarnedBadgeInc")).Checked)
                    {
                        needBadges = true;                      
                        retColumns = Coalesce(retColumns, "b.AdminName as BadgeName", ",");
                    }

                    filterField = "EarnedBadge";
                    txt = ((DropDownList)(((DetailsView)sender).FindControl(filterField))).SelectedValue;
                    _int = int.Parse(txt);
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _int));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{1})", "pb.BadgeId", filterField), " AND ");
                        needBadges = true;

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, ((DropDownList)(((DetailsView)sender).FindControl(filterField))).SelectedItem.Text), majorSep);
                    }

                    if (needBadges)
                    {
                        fromClause = Coalesce(fromClause, "left join PatronBadges pb on pb.PID = p.PID left outer join Badge b on pb.BadgeID = b.BID", " ");
                    }
                    
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

                    var needPrize = false;
                    if (((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeNameInc")).Checked)
                    {
                        needPrize = true;
                        retColumns = Coalesce(retColumns, "pr.PrizeName", ",");
                        retColumns = Coalesce(retColumns, "pr.RedeemedFlag", ",");
                    }

                    filterField = "PhysicalPrizeEarned";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        needPrize = true;

                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, txt));
                        whereClause = Coalesce(whereClause, string.Format("({0} like '%' + @{1} + '%')", "pr.PrizeName", filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", "PrizeName", minorSep, txt), majorSep);

                        filterField = "PhysicalPrizeRedeemed";
                        var chk = ((CheckBox)(((DetailsView)sender).FindControl(filterField))).Checked;

                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, (chk ? 1 : 0)));
                        whereClause = Coalesce(whereClause, string.Format("(pr.RedeemedFlag = @{0})", filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", "Redeemed", minorSep, (chk ? 1 : 0)), majorSep);
                    }

                    if (((CheckBox)((DetailsView)sender).FindControl("PhysicalPrizeDateInc")).Checked)
                    {
                        needPrize = true;
                        retColumns = Coalesce(retColumns, "pr.AddedDate as PrizeAwardDate", ",");
                    }

                    filterField = "PhysicalPrizeStartDate";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        needPrize = true;
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("(pr.AddedDate >= @{0} or @{0} is null)", filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim()), majorSep);
                    }
                    
                    filterField = "PhysicalPrizeEndDate";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        needPrize = true;
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("(pr.AddedDate <= @{0} or @{0} is null)", filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim()), majorSep);

                    }

                    if (needPrize)
                    {
                        fromClause = Coalesce(fromClause, "left join PatronPrizes pr on p.PID = pr.PID", " ");
                    }

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

                    var needReview = false;
                    var needReviewDates = false;
                    if (((CheckBox)((DetailsView)sender).FindControl("NumReviewsInc")).Checked)
                    {
                        needReview = true;
                        retColumns = Coalesce(retColumns, "(select Count(*) from PatronReview pr where pr.PID = p.PID AND (@ReviewStartDate is null or pr.ReviewDate >= @ReviewStartDate) and (@ReviewEndDate is null or pr.ReviewDate <= @ReviewEndDate)) as NumReviews", ",");
                        needReviewDates = true;

                    }

                    filterField = "ReviewsMin";
                    parmField = "(select Count(*) from PatronReview pr where pr.PID = p.PID AND (@ReviewStartDate is null or pr.ReviewDate >= @ReviewStartDate) and (@ReviewEndDate is null or pr.ReviewDate <= @ReviewEndDate))";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1} or @{1} is null)", parmField, filterField), " AND ");
                        needReviewDates = true;

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, txt), majorSep);
                    }

                    filterField = "ReviewsMax";
                    parmField = "(select Count(*) from PatronReview pr where pr.PID = p.PID AND (@ReviewStartDate is null or pr.ReviewDate >= @ReviewStartDate) and (@ReviewEndDate is null or pr.ReviewDate <= @ReviewEndDate))";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1} or @{1} is null)", parmField, filterField), " AND ");
                        needReviewDates = true;

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, txt), majorSep);
                    }

                    if (((CheckBox)((DetailsView)sender).FindControl("ReviewTitleInc")).Checked)
                    {
                        needReview = true;
                        retColumns = Coalesce(retColumns, "patr.Title", ",");
                    }

                    filterField = "ReviewTitle";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        needReview = true;
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, txt));
                        whereClause = Coalesce(whereClause, string.Format("(patr.Title like '%' + @{0} + '%')", filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} like {2}", filterField, minorSep, txt), majorSep);
                    }

                    if (((CheckBox)((DetailsView)sender).FindControl("ReviewAuthorInc")).Checked)
                    {
                        needReview = true;
                        retColumns = Coalesce(retColumns, "patr.Author", ",");
                    }

                    filterField = "ReviewAuthor";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        needReview = true;
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, txt));
                        whereClause = Coalesce(whereClause, string.Format("(patr.Author like '%' + @{0} + '%')", filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} like {2}", filterField, minorSep, txt), majorSep);
                    }


                    if (((CheckBox)((DetailsView)sender).FindControl("ReviewDateInc")).Checked)
                    {
                        needReview = true;
                        retColumns = Coalesce(retColumns, "convert(varchar, patr.ReviewDate, 101) as ReviewDate", ",");
                    }

                    filterField = "ReviewStartDate";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("(@{0} is null or patr.ReviewDate >= @{0})", filterField), " AND ");
                        needReview = true;

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim()), majorSep);
                    }
                    else if (needReviewDates)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, DBNull.Value));
                        needReview = true;
                    }

                    filterField = "ReviewEndDate";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("(@{0} is null or patr.ReviewDate <= @{0})", filterField), " AND ");
                        needReview = true;

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim()), majorSep);
                    }
                    else if (needReviewDates)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, DBNull.Value));
                        needReview = true;
                    }


                    if (needReview)
                    {
                        fromClause = Coalesce(fromClause, "left join PatronReview patr on patr.PID = p.PID", " ");
                    }

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

                    var needDrawing = false;
                    //var needDrawingDates = false;
                    if (((CheckBox)((DetailsView)sender).FindControl("RandomDrawingNameInc")).Checked)
                    {
                        needDrawing = true;
                        retColumns = Coalesce(retColumns, "pdr.PrizeName as RandomDrawingName", ",");
                    }
                    filterField = "RandomDrawingName";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim();
                    if (txt.Length > 0)
                    {
                        needReview = true;
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, txt));
                        whereClause = Coalesce(whereClause, string.Format("(pdr.PrizeName like '%' + @{0} + '%')", filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, txt), majorSep);
                    }

                    if (((CheckBox)((DetailsView)sender).FindControl("RandomDrawingNumInc")).Checked)
                    {
                        needDrawing = true;
                        retColumns = Coalesce(retColumns, "pdr.PDID as RandomDrawingNum", ",");
                    }
                    parmField = "RandomDrawingNum";
                    txt = ((TextBox)(((DetailsView)sender).FindControl(parmField))).Text.Trim();
                    _int = txt.SafeToInt();
                    if (_int > 0)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + parmField, _int));
                        whereClause = Coalesce(whereClause, string.Format("({0} = @{1})", "pdr.PDID", parmField), " AND ");
                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, txt), majorSep);
                    }

                    if (((CheckBox)((DetailsView)sender).FindControl("HasRedeemendInc")).Checked)
                    {
                        needDrawing = true;
                        retColumns = Coalesce(retColumns, "pdw.PrizePickedUpFlag as UserRedeemedPrize", ",");

                        filterField = "HasBeenDrawn";
                        var chk = ((CheckBox)(((DetailsView)sender).FindControl(filterField))).Checked;

                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, (chk ? 1 : 0)));
                        whereClause = Coalesce(whereClause, string.Format("(pdw.PrizePickedUpFlag = @{0})", filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, (chk ? 1 : 0)), majorSep);
                    }


                    if (((CheckBox)((DetailsView)sender).FindControl("HasBeenDrawnInc")).Checked)
                    {
                        needDrawing = true;
                        retColumns = Coalesce(retColumns, "'Yes' as UserHasBeenDrawn", ",");
                    }

                    if (((CheckBox)((DetailsView)sender).FindControl("RandomDrawingDateInc")).Checked)
                    {
                        needDrawing = true;
                        retColumns = Coalesce(retColumns, "convert(varchar, pdr.DrawingDateTime, 101) as RandomDrawingDate", ",");
                    }

                    filterField = "RandomDrawingStartDate";
                    parmField = "pdr.DrawingDateTime";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("({0} >= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim()), majorSep);
                    }

                    filterField = "RandomDrawingEndDate";
                    parmField = "pdr.DrawingDateTime";
                    _dt = FormatHelper.SafeToDateTime(((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim());
                    if (_dt != DateTime.MinValue)
                    {
                        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, _dt));
                        whereClause = Coalesce(whereClause, string.Format("({0} <= @{1})", parmField, filterField), " AND ");

                        filterStr = Coalesce(filterStr, string.Format("{0}{1} = {2}", filterField, minorSep, ((TextBox)(((DetailsView)sender).FindControl(filterField))).Text.Trim()), majorSep);
                    }

                    if (needDrawing)
                    {
                        fromClause = Coalesce(fromClause, "left outer join PrizeDrawingWinners pdw on pdw.PatronId = p.PID left outer join PrizeDrawing pdr on pdw.PDID = pdr.PDID", " ");
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (retColumns.Length == 0)
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageError = String.Format("You must select the columns to display.");
                        return;
                    }

                    AddSqlParameter(ref arrParams, new SqlParameter("@TenID", CRTenantID == null ? -1 : CRTenantID));
                    whereClause = Coalesce(whereClause, "(p.TenID = @TenID)", " AND ");

                    // Should this be DISTINCT ?????????
                    var SQL = "SELECT DISTINCT " + retColumns + " FROM " + fromClause;
                    if (whereClause.Length > 0) SQL = SQL + " WHERE " + whereClause;






                    
                    //lblSQl.Text = SQL;
                    var ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, SQL, arrParams);

                    Session["rptSql"] = ds;
                    

                    //gv.DataSource = ds;
                    //gv.DataBind();

                    parmField = "ReportFormat";
                    txt = ((DropDownList)(((DetailsView)sender).FindControl(parmField))).SelectedValue;
                    _int = int.Parse(txt);
                    if (_int == 0)
                    {
                        string path = "HTMLReportResults.aspx";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "var win = window.open('" + path + "','',''); if (win == null || typeof(win)=='undefined') {alert('Turn off your pop-up blocker!');}", true);
                        //Response.Redirect("HTMLReportResults.aspx");

                        Session["rptFilter"] = filterStr.Replace(minorSep, " ").Replace(majorSep, "<br/>");
                    }
                    if (_int == 2)
                    {
                        string excelFilename = Server.MapPath("~/Adhocreport.xlsx");
                        CreateExcelFile.CreateExcelDocument(ds, excelFilename, filterStr.Replace(minorSep, " ").Replace(majorSep, "\n"));

                        Response.ContentType = "application/vnd.ms-excel";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=Adhocreport.xlsx");
                        EnableViewState = false;
                        Response.BinaryWrite(File.ReadAllBytes(excelFilename));
                        File.Delete(excelFilename);
                        Response.End();
                    }

                    

                    


                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }

            
            

            #endregion
        }

        private void ProcessSimpleStringFilter(DetailsView sender, string filterField, ref string whereClause, ref string retColumns, ref SqlParameter[] arrParams, ref string filterStr, string minorSep, string majorSep)
        {
            var txt = ((TextBox)(sender).FindControl(filterField)).Text.Trim();
            if (txt.Length > 0)
            {
                AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, txt));
                whereClause = Coalesce(whereClause, string.Format("({0} like '%' + @{0} + '%')", filterField), " AND ");

                filterStr = Coalesce(filterStr, string.Format("{0}{1}like {2}", filterField, minorSep, txt), majorSep);
                //retColumns = Coalesce(retColumns, filterField, ",");
            }
        }

        private void ProcessSimpleIntFilter(DetailsView sender, string filterField, ref string whereClause, ref string retColumns, ref SqlParameter[] arrParams)
        {
            var txt = ((TextBox)(sender).FindControl(filterField)).Text.Trim();
            if (txt.Length > 0)
            {
                AddSqlParameter(ref arrParams, new SqlParameter("@" + filterField, int.Parse(txt)));
                whereClause = Coalesce(whereClause, string.Format("({0} like '%' + @{0} + '%')", filterField), " AND ");
                //retColumns = Coalesce(retColumns, filterField, ",");
            }
        }

        //private void ProcessRangeIntFilter(DetailsView sender, string filterField, string filterFieldParm, ref string whereClause, ref string retColumns, ref SqlParameter[] arrParams)
        //{
        //    var txt = ((TextBox)(sender).FindControl(filterField)).Text.Trim();
        //    if (txt.Length > 0)
        //    {
        //        AddSqlParameter(ref arrParams, new SqlParameter("@" + filterFieldMin, int.Parse(txt)));
        //        whereClause = Coalesce(whereClause, string.Format("({0} like '%' + @{0} + '%')", filterFieldMin), " AND ");
        //        //retColumns = Coalesce(retColumns, filterField, ",");
        //    }
        //}

        private string Coalesce (string startingString, string additionalString, string separator= "")
        {
            if (startingString.Length == 0) return additionalString;
            return string.Format("{0} {1} {2}", startingString, separator, additionalString);
        }

        private void AddSqlParameter(ref SqlParameter[] arr, SqlParameter x)
        {
            if (arr == null)
            {
                arr = new SqlParameter[1];
                arr[0] = x;
            }
            else
            {
                int ubound = arr.GetUpperBound(0) + 1;
                Array.Resize<SqlParameter>(ref arr, ubound + 1);
                arr[ubound] = x;
            }
        }
    }
}

