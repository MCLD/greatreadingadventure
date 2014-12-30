﻿using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;
using ExportToExcel;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;


namespace STG.SRP.ControlRoom.Modules.Programs
{
    public partial class ProgramsAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ProgramRibbon());
            }

            MasterPage.RequiredPermission = 2200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Program Add / Edit");

            if (!IsPostBack)
            {
                lblPK.Text = Request["PK"];
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var control = (STG.SRP.Classes.FileDownloadCtl)dv.FindControl("TabContainer1").FindControl("TabPanel1").FindControl("FileUploadCtl");
                if (control!=null) control.ProcessRender();


                var ctl = (DropDownList)dv.FindControl("TabContainer1").FindControl("TabPanel2").FindControl("RegistrationBadgeID"); //this.FindControlRecursive(this, "BranchId");
                var lbl = (Label)dv.FindControl("TabContainer1").FindControl("TabPanel2").FindControl("RegistrationBadgeIDLbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("TabContainer1").FindControl("TabPanel2").FindControl("ProgramGameID");
                lbl = (Label)dv.FindControl("TabContainer1").FindControl("TabPanel2").FindControl("ProgramGameIDLbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;


                int PK = int.Parse(lblPK.Text);
                var ds = ProgramCodes.GetProgramStats(PK);

                lbl = (Label)dv.FindControl("TabContainer1").FindControl("TabPanel6").FindControl("lblTotalCodes");
                lbl.Text = FormatHelper.ToInt((int)ds.Tables[0].Rows[0]["TotalCodes"]);
                lbl = (Label)dv.FindControl("TabContainer1").FindControl("TabPanel6").FindControl("lblUsedCodes");
                lbl.Text = FormatHelper.ToInt((int)ds.Tables[0].Rows[0]["UsedCodes"]);
                lbl = (Label)dv.FindControl("TabContainer1").FindControl("TabPanel6").FindControl("lblRemainingCodes");
                lbl.Text = FormatHelper.ToInt((int)ds.Tables[0].Rows[0]["RemainingCodes"]);
                lbl = (Label)dv.FindControl("TabContainer1").FindControl("TabPanel6").FindControl("lblLastCode");
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
                    var obj = new DAL.Programs();
                    //obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("GenNotificationFlag")).Checked;

                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("AdminName")).Text;
                    obj.Title = ((TextBox)((DetailsView)sender).FindControl("Title")).Text;
                    obj.TabName = ((TextBox)((DetailsView)sender).FindControl("TabName")).Text;
                    //obj.POrder = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("POrder")).Text);
                    obj.IsActive = ((CheckBox)((DetailsView)sender).FindControl("IsActive")).Checked;
                    obj.IsHidden = ((CheckBox)((DetailsView)sender).FindControl("IsHidden")).Checked;
                    obj.StartDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("StartDate")).Text);
                    obj.EndDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("EndDate")).Text);
                    //obj.MaxAge = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MaxAge")).Text);
                    //obj.MaxGrade = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("MaxGrade")).Text);
                    obj.LoggingStart = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("LoggingStart")).Text);
                    obj.LoggingEnd = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("LoggingEnd")).Text);
                    obj.CompletionPoints = 0;
                    //obj.ParentalConsentFlag = ((CheckBox)((DetailsView)sender).FindControl("ParentalConsentFlag")).Checked;
                    //obj.ParentalConsentText = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("ParentalConsentText")).Text;
                    //obj.PatronReviewFlag = ((CheckBox)((DetailsView)sender).FindControl("PatronReviewFlag")).Checked;
                    //obj.LogoutURL = ((TextBox)((DetailsView)sender).FindControl("LogoutURL")).Text;
                    //obj.ProgramGameID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("ProgramGameID")).SelectedValue);
                    //obj.HTML1 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("HTML1")).Text;
                    //obj.HTML2 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("HTML2")).Text;
                    //obj.HTML3 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("HTML3")).Text;
                    //obj.HTML4 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("HTML4")).Text;
                    //obj.HTML5 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("HTML5")).Text;
                    //obj.HTML6 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("HTML6")).Text;
                    //obj.BannerImage = ((TextBox)((DetailsView)sender).FindControl("BannerImage")).Text;
                    //obj.RegistrationBadgeID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("RegistrationBadgeID")).SelectedValue);
                    
                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();

                        File.Copy(Server.MapPath("~/css/program/default.css"), Server.MapPath("~/css/program/" + obj.PID.ToString() + ".css"),true);
                        File.Copy(Server.MapPath("~/images/banners/default.png"), Server.MapPath("~/images/banners/" + obj.PID.ToString() + ".png"), true);
                        File.Copy(Server.MapPath("~/resources/program.default.en-US.txt"), Server.MapPath("~/resources/program." + obj.PID.ToString() + ".en-US.txt"), true);
                        //CompileResourceFile(Server.MapPath("~/resources/program." + obj.PID.ToString() + ".en-US.txt"));

                        foreach (ActivityType val in Enum.GetValues(typeof(ActivityType)))
                        {
                            var o = new ProgramGamePointConversion();
                            o.PGID = obj.PID;
                            o.ActivityTypeId = (int)val;
                            o.ActivityCount = 1;
                            o.PointCount = 0;
                            o.AddedDate = obj.AddedDate;
                            o.AddedUser = obj.AddedUser;
                            o.LastModDate = o.AddedDate;
                            o.LastModUser = o.AddedUser;

                            o.Insert();
                        }

                        if (e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = obj.PID.ToString();

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
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    var obj = new DAL.Programs();
                    int pk = int.Parse(((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("PID")).Text);
                    obj.Fetch(pk);

                    obj.AdminName = ((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("AdminName")).Text;
                    obj.Title = ((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("Title")).Text;
                    obj.TabName = ((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("TabName")).Text;
                    //obj.POrder = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("POrder")).Text);
                    obj.IsActive = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("IsActive")).Checked;
                    obj.IsHidden = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("IsHidden")).Checked;
                    obj.StartDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("StartDate")).Text);
                    obj.EndDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("EndDate")).Text);
                    obj.MaxAge = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("MaxAge")).Text);
                    obj.MaxGrade = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("MaxGrade")).Text);
                    obj.LoggingStart = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("LoggingStart")).Text);
                    obj.LoggingEnd = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("LoggingEnd")).Text);
                    obj.ParentalConsentFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("ParentalConsentFlag")).Checked;
                    obj.ParentalConsentText = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("ParentalConsentText")).Text; 
                    obj.PatronReviewFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("PatronReviewFlag")).Checked;
                    obj.LogoutURL = ((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("LogoutURL")).Text;

                    obj.CompletionPoints = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("CompletionPoints")).Text);
                    obj.ProgramGameID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("ProgramGameID")).Text); 
                    //obj.ProgramGameID = 0;
                    obj.RegistrationBadgeID = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("RegistrationBadgeID")).Text); 
                    //obj.RegistrationBadgeID = 0;
                    obj.HTML1 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("HTML1")).Text;
                    obj.HTML2 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("HTML2")).Text;

                    obj.HTML3 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel3").FindControl("HTML3")).Text;
                    obj.HTML4 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel3").FindControl("HTML4")).Text;

                    obj.HTML5 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel4").FindControl("HTML5")).Text;
                    obj.HTML6 = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel4").FindControl("HTML6")).Text; 

                    obj.BannerImage = "";//((TextBox)((DetailsView)sender).FindControl("BannerImage")).Text;


                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();

                        var rptr = ((Repeater)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel5").FindControl("rptr")); 
                        if (rptr!=null)
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
                                if (o == null) continue;
                                o.ActivityCount = ActivityCount;
                                o.PointCount = PointCount;
                                o.LastModDate = obj.LastModDate;
                                o.LastModUser = obj.LastModUser;
                                o.Update();
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
            if (e.CommandName.ToLower() == "gen")
            {

                int PK = int.Parse(lblPK.Text);
                var ds = ProgramCodes.GetProgramStats(PK);

                int TotalCodes = (int)ds.Tables[0].Rows[0]["TotalCodes"];
                var txt = (TextBox)dv.FindControl("TabContainer1").FindControl("TabPanel6").FindControl("txtGen");
                int addl = int.Parse(txt.Text);

                ProgramCodes.Generate(TotalCodes + 1, TotalCodes + addl, PK);

                odsData.DataBind();
                dv.DataBind();
                dv.ChangeMode(DetailsViewMode.Edit);

                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = "Additional codes generated!";

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

