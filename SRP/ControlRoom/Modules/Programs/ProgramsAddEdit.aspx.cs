﻿using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;
using ExportToExcel;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using GRA.Tools;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.ControlRoom.Modules.Programs
{
    public partial class ProgramsAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 2200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Program Add / Edit");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ProgramRibbon());

                lblPK.Text = Session["PGM"] == null ? "" : Session["PGM"].ToString();
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();

                string codeName = StringResources.getStringOrNull("myaccount-program-reward-code");
                if (!string.IsNullOrEmpty(codeName))
                {
                    if (!codeName.EndsWith("s"))
                    {
                        codeName += "s";
                    }
                    var tabContainer = dv.FindControl("tc1"); // only visible on edit screen
                    if (tabContainer != null)
                    {
                        var codePanel = tabContainer.FindControl("tp6") as AjaxControlToolkit.TabPanel;
                        if (codePanel != null)
                        {
                            codePanel.HeaderText = codeName;
                            var generateBtn = codePanel.FindControl("GenerateButtonText") as Label;
                            if (generateBtn != null)
                            {
                                generateBtn.Text = string.Format("Generate {0}", codeName);
                            }
                        }
                    }
                }
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("tc1").FindControl("tp1").FindControl("FileUploadCtl");
                if (control != null)
                    control.ProcessRender();


                var ctl = (DropDownList)dv.FindControl("tc1").FindControl("tp2").FindControl("RegistrationBadgeID"); //this.FindControlRecursive(this, "BranchId");
                var lbl = (Label)dv.FindControl("tc1").FindControl("tp2").FindControl("RegistrationBadgeIDLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null)
                    ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("tc1").FindControl("tp2").FindControl("ProgramGameID");
                lbl = (Label)dv.FindControl("tc1").FindControl("tp2").FindControl("ProgramGameIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null)
                    ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("tc1").FindControl("tp5").FindControl("PreTestID");
                lbl = (Label)dv.FindControl("tc1").FindControl("tp5").FindControl("PreTestIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null)
                    ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("tc1").FindControl("tp5").FindControl("PostTestID");
                lbl = (Label)dv.FindControl("tc1").FindControl("tp5").FindControl("PostTestIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null)
                    ctl.SelectedValue = lbl.Text;



                int PK = int.Parse(lblPK.Text);
                var ds = ProgramCodes.GetProgramStats(PK);

                lbl = (Label)dv.FindControl("tc1").FindControl("tp6").FindControl("lblTotalCodes");
                lbl.Text = FormatHelper.ToInt((int)ds.Tables[0].Rows[0]["TotalCodes"]);
                lbl = (Label)dv.FindControl("tc1").FindControl("tp6").FindControl("lblUsedCodes");
                lbl.Text = FormatHelper.ToInt((int)ds.Tables[0].Rows[0]["UsedCodes"]);
                lbl = (Label)dv.FindControl("tc1").FindControl("tp6").FindControl("lblRemainingCodes");
                lbl.Text = FormatHelper.ToInt((int)ds.Tables[0].Rows[0]["RemainingCodes"]);
                lbl = (Label)dv.FindControl("tc1").FindControl("tp6").FindControl("lblLastCode");
                lbl.Text = (string)ds.Tables[0].Rows[0]["LastUsedCode"];

            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Programs/ProgramList.aspx";
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
                    if (masterPage != null)
                        masterPage.PageMessage = SRPResources.RefreshOK;
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
                    var program = new DAL.Programs();
                    //obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("tc1").FindControl("tp2").FindControl("GenNotificationFlag")).Checked;

                    program.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
                    program.Title = ((TextBox)((DetailsView)sender).FindControl("Title")).Text;
                    program.TabName = ((TextBox)((DetailsView)sender).FindControl("TabName")).Text;
                    //obj.POrder = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("POrder")).Text);
                    program.IsActive = ((CheckBox)((DetailsView)sender).FindControl("IsActive")).Checked;
                    program.IsHidden = ((CheckBox)((DetailsView)sender).FindControl("IsHidden")).Checked;
                    program.StartDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("StartDate")).Text);
                    program.EndDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("EndDate")).Text);
                    //obj.MaxAge = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MaxAge")).Text);
                    //obj.MaxGrade = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MaxGrade")).Text);
                    program.LoggingStart = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("LoggingStart")).Text);
                    program.LoggingEnd = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("LoggingEnd")).Text);
                    program.CompletionPoints = 0;
                    //obj.ParentalConsentFlag = ((CheckBox)((DetailsView)sender).FindControl("ParentalConsentFlag")).Checked;
                    //obj.ParentalConsentText = ((HtmlTextArea)((DetailsView)sender).FindControl("ParentalConsentText")).Text;
                    //obj.PatronReviewFlag = ((CheckBox)((DetailsView)sender).FindControl("PatronReviewFlag")).Checked;
                    //obj.LogoutURL = ((TextBox)((DetailsView)sender).FindControl("LogoutURL")).Text;
                    //obj.ProgramGameID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("ProgramGameID")).SelectedValue);
                    //obj.HTML1 = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML1")).Text;
                    //obj.HTML2 = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML2")).Text;
                    //obj.HTML3 = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML3")).Text;
                    //obj.HTML4 = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML4")).Text;
                    //obj.HTML5 = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML5")).Text;
                    //obj.HTML6 = ((HtmlTextArea)((DetailsView)sender).FindControl("HTML6")).Text;
                    //obj.BannerImage = ((TextBox)((DetailsView)sender).FindControl("BannerImage")).Text;
                    //obj.RegistrationBadgeID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("RegistrationBadgeID")).SelectedValue);

                    program.AddedDate = DateTime.Now;
                    program.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    program.LastModDate = program.AddedDate;
                    program.LastModUser = program.AddedUser;

                    if (program.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        program.Insert();

                        File.Copy(Server.MapPath("~/css/program/default.css"), Server.MapPath("~/css/program/" + program.PID.ToString() + ".css"), true);
                        File.Copy(Server.MapPath("~/images/meadow.jpg"), Server.MapPath("~/images/banners/" + program.PID.ToString() + ".jpg"), true);
                        File.Copy(Server.MapPath("~/images/meadow@2x.jpg"), Server.MapPath("~/images/banners/" + program.PID.ToString() + ".jpg"), true);
                        File.Copy(Server.MapPath("~/resources/program.default.en-US.txt"), Server.MapPath("~/resources/program." + program.PID.ToString() + ".en-US.txt"), true);
                        //CompileResourceFile(Server.MapPath("~/resources/program." + obj.PID.ToString() + ".en-US.txt"));

                        foreach (ActivityType val in Enum.GetValues(typeof(ActivityType)))
                        {
                            var o = new ProgramGamePointConversion();
                            o.PGID = program.PID;
                            o.ActivityTypeId = (int)val;
                            o.ActivityCount = 1;
                            o.PointCount = 0;
                            o.AddedDate = program.AddedDate;
                            o.AddedUser = program.AddedUser;
                            o.LastModDate = o.AddedDate;
                            o.LastModUser = o.AddedUser;

                            o.Insert();

                            string cacheValue = string.Format("{0}.{1}.{2}",
                                  CacheKey.PointGameConversionStub,
                                  o.PGID,
                                  o.ActivityTypeId);
                            new SessionTools(Session).RemoveCache(Cache, cacheValue);
                        }

                        if (e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = program.PID.ToString();

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
                        foreach (BusinessRulesValidationMessage m in program.ErrorCodes)
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
                    var program = new DAL.Programs();
                    int pk = int.Parse(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("PID")).Text);
                    program.Fetch(pk);

                    program.AdminName = ((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("AdminName")).Text;
                    program.Title = ((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("Title")).Text;
                    program.TabName = ((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("TabName")).Text;
                    //obj.POrder = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("POrder")).Text);
                    program.IsActive = ((CheckBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("IsActive")).Checked;
                    program.IsHidden = ((CheckBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("IsHidden")).Checked;
                    program.StartDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("StartDate")).Text);
                    program.EndDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("EndDate")).Text);
                    program.MaxAge = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("MaxAge")).Text);
                    program.MaxGrade = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("MaxGrade")).Text);
                    program.LoggingStart = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("LoggingStart")).Text);
                    program.LoggingEnd = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("LoggingEnd")).Text);
                    program.ParentalConsentFlag = ((CheckBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("ParentalConsentFlag")).Checked;
                    program.ParentalConsentText = ((HtmlTextArea)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("ParentalConsentText")).InnerHtml;
                    program.PatronReviewFlag = ((CheckBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("PatronReviewFlag")).Checked;
                    program.RequireBookDetails = ((CheckBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("RequireBookDetails")).Checked;
                    program.LogoutURL = ((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("LogoutURL")).Text;

                    program.CompletionPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp2").FindControl("CompletionPoints")).Text);
                    program.ProgramGameID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("tc1").FindControl("tp2").FindControl("ProgramGameID")).Text);
                    //obj.ProgramGameID = 0;
                    program.RegistrationBadgeID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("tc1").FindControl("tp2").FindControl("RegistrationBadgeID")).Text);
                    //obj.RegistrationBadgeID = 0;
                    program.HTML1 = ((HtmlTextArea)((DetailsView)sender).FindControl("tc1").FindControl("tp2").FindControl("HTML1")).InnerHtml;
                    program.HTML2 = ((HtmlTextArea)((DetailsView)sender).FindControl("tc1").FindControl("tp2").FindControl("HTML2")).InnerHtml;

                    program.HTML3 = ((HtmlTextArea)((DetailsView)sender).FindControl("tc1").FindControl("tp3").FindControl("HTML3")).InnerHtml;
                    program.HTML4 = ((HtmlTextArea)((DetailsView)sender).FindControl("tc1").FindControl("tp3").FindControl("HTML4")).InnerHtml;

                    program.HTML5 = ((HtmlTextArea)((DetailsView)sender).FindControl("tc1").FindControl("tp4").FindControl("HTML5")).InnerHtml;
                    program.HTML6 = ((HtmlTextArea)((DetailsView)sender).FindControl("tc1").FindControl("tp4").FindControl("HTML6")).InnerHtml;

                    program.BannerImage = string.Empty;//((TextBox)((DetailsView)sender).FindControl("BannerImage")).Text;

                    program.PreTestEndDate = ((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp5").FindControl("PreTestEndDate")).Text.SafeToDateTime();
                    program.PostTestStartDate = ((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp5").FindControl("PostTestStartDate")).Text.SafeToDateTime();
                    program.PreTestID = ((DropDownList)((DetailsView)sender).FindControl("tc1").FindControl("tp5").FindControl("PreTestID")).SelectedValue.SafeToInt();
                    program.PostTestID = ((DropDownList)((DetailsView)sender).FindControl("tc1").FindControl("tp5").FindControl("PostTestID")).SelectedValue.SafeToInt();
                    program.PreTestMandatory = ((CheckBox)((DetailsView)sender).FindControl("tc1").FindControl("tp5").FindControl("PreTestMandatory")).Checked;

                    program.GoalDefault = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("GoalDefault")).Text);
                    program.GoalMin = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("GoalMin")).Text);
                    program.GoalMax = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("GoalMax")).Text);
                    program.GoalIntervalId = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("GoalIntervalId")).SelectedValue);

                    program.HideSchoolInRegistration = ((CheckBox)((DetailsView)sender).FindControl("tc1").FindControl("tp1").FindControl("HideSchoolInRegistration")).Checked;

                    program.LastModDate = DateTime.Now;
                    program.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if (program.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        program.Update();

                        var rptr = ((Repeater)((DetailsView)sender).FindControl("tc1").FindControl("tp5").FindControl("rptr"));
                        if (rptr != null)
                        {
                            foreach (RepeaterItem item in rptr.Items)
                            {
                                var PGCID = int.Parse(((TextBox)item.FindControl("PGCID")).Text);
                                var ActivityCountTxt = ((TextBox)item.FindControl("ActivityCount")).Text;
                                var PointCountTxt = ((TextBox)item.FindControl("PointCount")).Text;
                                int ActivityCount = 1, PointCount = 0;
                                int.TryParse(ActivityCountTxt, out ActivityCount);
                                int.TryParse(PointCountTxt, out PointCount);

                                var o = ProgramGamePointConversion.FetchObject(PGCID);
                                if (o == null)
                                    continue;
                                o.ActivityCount = ActivityCount;
                                o.PointCount = PointCount;
                                o.LastModDate = program.LastModDate;
                                o.LastModUser = program.LastModUser;
                                o.Update();

                                string cacheValue = string.Format("{0}.{1}.{2}",
                                                                  CacheKey.PointGameConversionStub,
                                                                  o.PGID,
                                                                  o.ActivityTypeId);
                                new SessionTools(Session).RemoveCache(Cache, cacheValue);
                            }

                        }


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
                        foreach (BusinessRulesValidationMessage m in program.ErrorCodes)
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
            if (e.CommandName.ToLower() == "gen")
            {

                int PK = int.Parse(lblPK.Text);
                var ds = ProgramCodes.GetProgramStats(PK);

                int TotalCodes = (int)ds.Tables[0].Rows[0]["TotalCodes"];
                var txt = (TextBox)dv.FindControl("tc1").FindControl("tp6").FindControl("txtGen");
                int addl = int.Parse(txt.Text);

                string result = ProgramCodes.Generate(TotalCodes + 1, TotalCodes + addl, PK);

                odsData.DataBind();
                dv.DataBind();
                dv.ChangeMode(DetailsViewMode.Edit);

                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = result;

            }
            if (e.CommandName.ToLower() == "exp")
            {

                int PK = int.Parse(lblPK.Text);

                var ds = ProgramCodes.GetExportList(PK);

                string excelFilename = Server.MapPath("~/ProgramRewardCodes.xlsx");
                CreateExcelFile.CreateExcelDocument(ds, excelFilename);

                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=ProgramRewardCodes.xlsx");
                EnableViewState = false;
                Response.BinaryWrite(File.ReadAllBytes(excelFilename));
                File.Delete(excelFilename);
                Response.End();


                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = "Codes Exported!";

            }
        }

        //protected void CompileResourceFile(string resourceFile)
        //{
        //    Process objProcess = new Process();
        //    //objProcess.StartInfo.FileName = @"C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin\x64\resgen.exe";
        //    objProcess.StartInfo.FileName = @"C:\temp\resgen.exe";
        //    objProcess.StartInfo.FileName = (Server.MapPath("~/Resources/") + "\\resgen.exe").Replace("\\\\", "\\");
        //    objProcess.StartInfo.UseShellExecute = false;
        //    objProcess.StartInfo.CreateNoWindow = false;
        //    objProcess.StartInfo.WorkingDirectory = Server.MapPath("~/Resources/"); ;
        //    objProcess.StartInfo.Arguments = resourceFile;// @"\\remote_server_name -w c:\physical_path perl c:\physical_path\test.pl ""parameter1""";
        //    objProcess.Start();
        //}
    }
}
