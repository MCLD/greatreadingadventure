using Excel;
using ExportToExcel;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Modules.Settings
{
    public partial class LibraryDistrict : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4100;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = "Library and District Crosswalk";

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());

                LoadData();

            }
        }

        protected void LoadData()
        {
            var ds = LibraryCrosswalk.GetAll();
            rptrCW.DataSource = ds;
            rptrCW.DataBind();
        }

        protected void SaveData()
        {
            var rptr = rptrCW;
            int i = 0;
            bool errors = false;
            foreach (RepeaterItem item in rptr.Items)
            {

                i++;
                try
                {
                    var ID = int.Parse(((Label)item.FindControl("ID")).Text);
                    var BranchID = int.Parse(((DropDownList)item.FindControl("BranchID")).SelectedValue);
                    var DistrictID = int.Parse(((DropDownList)item.FindControl("DistrictID")).SelectedValue);
                    var City = ((TextBox)item.FindControl("City")).Text;
                    var BranchLink = ((TextBox)item.FindControl("BranchLink")).Text;
                    var BranchAddress = ((TextBox)item.FindControl("BranchAddress")).Text;
                    var BranchTelephone = ((TextBox)item.FindControl("BranchTelephone")).Text;

                    var crosswalk = new LibraryCrosswalk();
                    if (ID != 0) crosswalk.Fetch(ID);
                    crosswalk.BranchID = BranchID;
                    crosswalk.DistrictID = DistrictID;
                    crosswalk.City = City;
                    crosswalk.BranchLink = BranchLink;
                    crosswalk.BranchAddress = BranchAddress;
                    crosswalk.BranchTelephone = BranchTelephone;

                    if (ID != 0)
                    {
                        crosswalk.Update();
                    }
                    else
                    {
                        crosswalk.Insert();
                    }
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format("On Row {1}: " + SRPResources.ApplicationError1, ex.Message, i);
                    errors = true;
                }

            }

            if (!errors)
            {
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = SRPResources.SaveAllOK;
            }
        }

        protected void btnBack_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/ControlRoom/Modules/Settings/Default.aspx");
        }

        protected void btnRefresh_Click(object sender, ImageClickEventArgs e)
        {
            LoadData();
            var masterPage = (IControlRoomMaster)Master;
            masterPage.PageMessage = SRPResources.RefreshAllOK;
        }

        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            SaveData();
            LoadData();
        }

        protected void btnSaveback_Click(object sender, ImageClickEventArgs e)
        {
            SaveData();
            Response.Redirect("~/ControlRoom/Modules/Settings/Default.aspx");
        }

        protected void rptrCW_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var ctl = (DropDownList)e.Item.FindControl("BranchID");
            var txt = (TextBox)e.Item.FindControl("BranchIDTxt");
            var i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;

            ctl = (DropDownList)e.Item.FindControl("DistrictID");
            txt = (TextBox)e.Item.FindControl("DistrictIDTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;

        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            int tenantId = (int)HttpContext.Current.Session["TenantID"];
            if (ExcelFileUpload.HasFile)
            {
                using (var stream = ExcelFileUpload.PostedFile.InputStream)
                {
                    IExcelDataReader excelReader = null;
                    try
                    {
                        if (ExcelFileUpload.FileName.EndsWith(".xlsx"))
                        {
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                        }
                        else if (ExcelFileUpload.FileName.EndsWith(".xls"))
                        {
                            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }

                        this.Log().Info("Importing uploaded Excel library crosswalk: {0}",
                            ExcelFileUpload.FileName);

                        if (excelReader == null)
                        {
                            throw new Exception("Could not parse Excel file, not .xls or .xlsx");
                        }

                        excelReader.IsFirstRowAsColumnNames = true;

                        var branchMap = new Dictionary<string, int>();
                        var districtMap = new Dictionary<string, int>();

                        var codeTypes = DAL.CodeType.GetAll().Tables[0];

                        var branchCodeTypeId = (int)codeTypes
                            .Select("CodeTypeName = 'Branch'")
                            .First()["CTID"];
                        var districtCodeTypeId = (int)codeTypes
                            .Select("CodeTypeName = 'Library District'")
                            .First()["CTID"];

                        var codes = DAL.Codes.GetAll().Tables[0];

                        foreach (DataRow codeRow in codes.Rows)
                        {
                            int ctid = (int)codeRow["CTID"];
                            if (ctid == branchCodeTypeId)
                            {
                                branchMap.Add(((string)codeRow["Code"]).ToLower(),
                                    (int)codeRow["CID"]);
                            }
                            else if (ctid == districtCodeTypeId)
                            {
                                districtMap.Add(((string)codeRow["Code"]).ToLower(),
                                    (int)codeRow["CID"]);
                            }
                        }

                        int? recordCount = null;
                        var problems = new List<string>();
                        while (excelReader.Read())
                        {
                            // skip column headings
                            if (recordCount == null)
                            {
                                recordCount = 0;
                                continue;
                            }
                            try
                            {
                                string branch = excelReader.GetString(0);
                                int? branchId = null;
                                if (branchMap.ContainsKey(branch.Trim().ToLower()))
                                {
                                    branchId = branchMap[branch.Trim().ToLower()];
                                }
                                string district = excelReader.GetString(1);
                                int? districtId = null;
                                if (districtMap.ContainsKey(district.Trim().ToLower()))
                                {
                                    districtId = districtMap[district.Trim().ToLower()];
                                }
                                string branchLink = null;
                                string branchAddress = null;
                                string branchTelephone = null;
                                if (excelReader.FieldCount >= 3)
                                {
                                    branchLink = excelReader.GetString(2);
                                }
                                if (excelReader.FieldCount >= 4)
                                {
                                    branchAddress = excelReader.GetString(3);
                                }
                                if (excelReader.FieldCount >= 5)
                                {
                                    branchTelephone = excelReader.GetString(4);
                                }

                                if (branchId == null)
                                {
                                    // insert branch
                                    branchId = DAL.Codes.Insert(new DAL.Codes
                                    {
                                        CTID = branchCodeTypeId,
                                        Code = branch.Trim(),
                                        Description = branch.Trim(),
                                        TenID = tenantId
                                    });
                                    branchMap.Add(branch.Trim().ToLower(), (int)branchId);
                                }

                                if (districtId == null)
                                {
                                    // insert district
                                    districtId = DAL.Codes.Insert(new DAL.Codes
                                    {
                                        CTID = districtCodeTypeId,
                                        Code = district.Trim(),
                                        Description = district.Trim(),
                                        TenID = tenantId
                                    });
                                    districtMap.Add(district.Trim().ToLower(), (int)districtId);
                                }

                                if(!string.IsNullOrEmpty(branchLink))
                                {
                                    branchLink = branchLink.Trim();
                                }

                                if (!string.IsNullOrEmpty(branchAddress))
                                {
                                    branchAddress = branchAddress.Trim();
                                }

                                if (!string.IsNullOrEmpty(branchTelephone))
                                {
                                    branchTelephone = branchTelephone.Trim();
                                }

                                // insert crosswalk
                                DAL.LibraryCrosswalk.Insert(new DAL.LibraryCrosswalk
                                {
                                    BranchID = (int)branchId,
                                    DistrictID = (int)districtId,
                                    TenID = tenantId,
                                    BranchLink = branchLink,
                                    BranchAddress = branchAddress,
                                    BranchTelephone = branchTelephone
                                });
                                recordCount++;
                            }
                            catch (Exception ex)
                            {
                                // couldn't import this record
                                string problem = string.Format("Unable to import record {0}: {1}",
                                    recordCount,
                                    ex.Message);
                                this.Log().Info(problem);
                                problems.Add(problem);
                            }
                        }

                        var result = new StringBuilder(string.Format("Imported {0} records and encountered {1} errors in: {2}",
                            recordCount,
                            problems.Count,
                            ExcelFileUpload.FileName));
                        this.Log().Info(result.ToString());
                        foreach(var problem in problems)
                        {
                            result.Append("<br />");
                            result.AppendLine(problem);
                        }
                        PageMessage = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        string result = string.Format("Error reading Excel file for library crosswalk: {0} - {1}",
                            ex.Message,
                            ex.StackTrace);
                        this.Log().Error(result);
                        PageError = result;
                    }
                    finally
                    {
                        if (excelReader != null)
                        {
                            if (!excelReader.IsClosed)
                            {
                                excelReader.Close();
                            }
                            excelReader.Dispose();
                        }
                    }
                }
            }
            LoadData();
        }

        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            string file = string.Format("{0}-LibraryCrosswalk.xlsx",
                DateTime.Now.ToString("yyyyMMdd"));
            CreateExcelFile.CreateExcelDocument(DAL.LibraryCrosswalk.GetExport(), file, Response);
        }
    }
}