using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using ExportToExcel;
using Excel;
using System.Data;
using System.Text;

namespace GRA.SRP.ControlRoom.Modules.Settings
{
    public partial class SchoolDistrict : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4100;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "School and District Crosswalk");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());

                //LoadData();

            }

            _mStrSortExp = String.Empty;
            if (!IsPostBack)
            {
                _mStrSortExp = String.Empty;
            }
            else
            {
                if (null != ViewState["_SortExp_"])
                {
                    _mStrSortExp = ViewState["_SortExp_"] as String;
                }

                if (null != ViewState["_Direction_"])
                {
                    _mSortDirection = (SortDirection)ViewState["_Direction_"];
                }
            }

        }

        protected void GvSorting(object sender, GridViewSortEventArgs e)
        {
            if (String.Empty != _mStrSortExp)
            {
                if (String.Compare(e.SortExpression, _mStrSortExp, true) == 0)
                {
                    _mSortDirection =
                        (_mSortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
                }
            }
            ViewState["_Direction_"] = _mSortDirection;
            ViewState["_SortExp_"] = _mStrSortExp = e.SortExpression;
        }

        protected void GvRowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (String.Empty != _mStrSortExp)
                {
                    GlobalUtilities.AddSortImage(e.Row, (GridView)sender, _mStrSortExp, _mSortDirection);
                }
            }

        }

        protected void GvSelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //protected void LoadData()
        //{
        //    var ds = SchoolCrosswalk.GetAll();
        //    rptrCW.DataSource = ds;
        //    rptrCW.DataBind();
        //}

        protected bool SaveData(bool autoSaveMessage = false)
        {
            //var rptr = rptrCW;
            var rptr = gv;
            int i = 0;
            bool errors = false;
            foreach (GridViewRow item in rptr.Rows)
            {

                i++;
                try
                {
                    var ID = int.Parse(((Label)item.FindControl("ID")).Text);
                    //var SchoolID = int.Parse(((DropDownList)item.FindControl("SchoolID")).SelectedValue);
                    var SchoolID = int.Parse(((TextBox)item.FindControl("SchoolID")).Text);
                    var SchTypeID = int.Parse(((DropDownList)item.FindControl("SchTypeID")).SelectedValue);
                    var DistrictID = int.Parse(((DropDownList)item.FindControl("DistrictID")).SelectedValue);
                    var City = ((TextBox)item.FindControl("City")).Text;

                    var MinGrade = ((TextBox)item.FindControl("MinGrade")).Text.SafeToInt();
                    var MaxGrade = ((TextBox)item.FindControl("MaxGrade")).Text.SafeToInt();
                    var MinAge = ((TextBox)item.FindControl("MinAge")).Text.SafeToInt();
                    var MaxAge = ((TextBox)item.FindControl("MaxAge")).Text.SafeToInt();


                    var obj = new SchoolCrosswalk();
                    if (ID != 0) obj.Fetch(ID);
                    obj.SchoolID = SchoolID;
                    obj.SchTypeID = SchTypeID;
                    obj.DistrictID = DistrictID;
                    obj.City = City;
                    obj.MinGrade = MinGrade;
                    obj.MaxGrade = MaxGrade;
                    obj.MinAge = MinAge;
                    obj.MaxAge = MaxAge;

                    if (ID != 0)
                    {
                        obj.Update();
                    }
                    else
                    {
                        obj.Insert();
                    }
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format("On Row {1}: " + SRPResources.ApplicationError1, ex.Message, i);
                    errors = true;
                }

            }

            if (!errors && !autoSaveMessage)
            {
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = SRPResources.SaveAllOK;
            }

            if (!errors && autoSaveMessage)
            {
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = "Changes on this page have been auto-saved.";
            }

            return (!errors);
        }

        protected void btnBack_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/ControlRoom/Modules/Settings/Default.aspx");
        }

        protected void btnRefresh_Click(object sender, ImageClickEventArgs e)
        {
            //LoadData();
            odsData.DataBind();
            gv.DataSourceID = "odsData";
            gv.DataBind();
            var masterPage = (IControlRoomMaster)Master;
            masterPage.PageMessage = SRPResources.RefreshAllOK;
        }

        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            SaveData();
            Response.Redirect("~/ControlRoom/Modules/Settings/SchoolDistrict.aspx");
            //LoadData();
        }

        protected void btnSaveback_Click(object sender, ImageClickEventArgs e)
        {
            SaveData();
            Response.Redirect("~/ControlRoom/Modules/Settings/Default.aspx");
        }


        protected void gv_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Validate();
            if (IsValid)
            {
                var success = SaveData(true);
                e.Cancel = !success;
            }
            else
            {
                e.Cancel = true;
            }

        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ctl = (DropDownList)e.Row.FindControl("SchTypeID");
                var txt = (TextBox)e.Row.FindControl("SchTypeIDTxt");
                var i = ctl.Items.FindByValue(txt.Text);
                if (i != null) ctl.SelectedValue = txt.Text;

                ctl = (DropDownList)e.Row.FindControl("DistrictID");
                txt = (TextBox)e.Row.FindControl("DistrictIDTxt");
                i = ctl.Items.FindByValue(txt.Text);
                if (i != null) ctl.SelectedValue = txt.Text;
            }
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

                        this.Log().Info("Importing uploaded Excel school crosswalk: {0}",
                            ExcelFileUpload.FileName);

                        if (excelReader == null)
                        {
                            throw new Exception("Could not parse Excel file, not .xls or .xlsx");
                        }

                        excelReader.IsFirstRowAsColumnNames = true;


                        var schoolMap = new Dictionary<string, int>();
                        var schoolTypeMap = new Dictionary<string, int>();
                        var districtMap = new Dictionary<string, int>();

                        var codeTypes = DAL.CodeType.GetAll().Tables[0];

                        var schoolCodeTypeId = (int)codeTypes
                            .Select("CodeTypeName = 'School'")
                            .First()["CTID"];
                        var schoolTypeCodeTypeId = (int)codeTypes
                            .Select("CodeTypeName = 'School Type'")
                            .First()["CTID"];
                        var districtCodeTypeId = (int)codeTypes
                            .Select("CodeTypeName = 'School District'")
                            .First()["CTID"];

                        var codes = DAL.Codes.GetAll().Tables[0];

                        foreach (DataRow codeRow in codes.Rows)
                        {
                            int ctid = (int)codeRow["CTID"];
                            if (ctid == schoolCodeTypeId)
                            {
                                schoolMap.Add(((string)codeRow["Code"]).ToLower(),
                                    (int)codeRow["CID"]);
                            }
                            else if (ctid == schoolTypeCodeTypeId)
                            {
                                schoolTypeMap.Add(((string)codeRow["Code"]).ToLower(),
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
                                string schoolName = excelReader.GetString(0);
                                int? schoolId = null;
                                if (schoolMap.ContainsKey(schoolName.Trim().ToLower()))
                                {
                                    schoolId = schoolMap[schoolName.Trim().ToLower()];
                                }
                                string schoolTypeName = excelReader.GetString(1);
                                int? schoolTypeId = null;
                                if (schoolTypeMap.ContainsKey(schoolTypeName.Trim().ToLower()))
                                {
                                    schoolTypeId = schoolTypeMap[schoolTypeName.Trim().ToLower()];
                                }
                                string district = excelReader.GetString(2);
                                int? districtId = null;
                                if (districtMap.ContainsKey(district.Trim().ToLower()))
                                {
                                    districtId = districtMap[district.Trim().ToLower()];
                                }
                                int? minGrade = null;
                                int? maxGrade = null;
                                int? minAge = null;
                                int? maxAge = null;
                                if (excelReader.FieldCount >= 4)
                                {
                                    // minGrade
                                    int convertedValue;
                                    if (int.TryParse(excelReader.GetString(3), out convertedValue))
                                    {
                                        maxGrade = convertedValue;
                                    }
                                }
                                if (excelReader.FieldCount >= 5)
                                {
                                    // maxGrade
                                    int convertedValue;
                                    if (int.TryParse(excelReader.GetString(4), out convertedValue))
                                    {
                                        maxGrade = convertedValue;
                                    }
                                }
                                if (excelReader.FieldCount >= 6)
                                {
                                    // minAge
                                    int convertedValue;
                                    if (int.TryParse(excelReader.GetString(5), out convertedValue))
                                    {
                                        maxGrade = convertedValue;
                                    }
                                }
                                if (excelReader.FieldCount >= 7)
                                {
                                    // maxAge
                                    int convertedValue;
                                    if (int.TryParse(excelReader.GetString(6), out convertedValue))
                                    {
                                        maxAge = convertedValue;
                                    }
                                }

                                if (schoolId == null)
                                {
                                    // insert branch
                                    schoolId = DAL.Codes.Insert(new DAL.Codes
                                    {
                                        CTID = schoolCodeTypeId,
                                        Code = schoolName.Trim(),
                                        Description = schoolName.Trim(),
                                        TenID = tenantId
                                    });
                                    schoolMap.Add(schoolName.Trim().ToLower(), (int)schoolId);
                                }

                                if (schoolTypeId == null)
                                {
                                    // insert branch
                                    schoolTypeId = DAL.Codes.Insert(new DAL.Codes
                                    {
                                        CTID = schoolTypeCodeTypeId,
                                        Code = schoolTypeName.Trim(),
                                        Description = schoolTypeName.Trim(),
                                        TenID = tenantId
                                    });
                                    schoolTypeMap.Add(schoolTypeName.Trim().ToLower(), (int)schoolTypeId);
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

                                // insert crosswalk
                                var crosswalk = new DAL.SchoolCrosswalk
                                {
                                    SchoolID = (int)schoolId,
                                    SchTypeID = (int)schoolTypeId,
                                    DistrictID = (int)districtId,
                                    City = string.Empty,
                                    MinGrade = minGrade ?? 0,
                                    MaxGrade = maxGrade ?? 0,
                                    MinAge = minAge ?? 0,
                                    MaxAge = maxAge ?? 0
                                };

                                DAL.SchoolCrosswalk.Insert(crosswalk);
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
                        foreach (var problem in problems)
                        {
                            result.Append("<br />");
                            result.AppendLine(problem);
                        }
                        PageMessage = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        string result = string.Format("Error reading Excel file for school crosswalk: {0} - {1}",
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
            odsData.DataBind();
            gv.DataSourceID = "odsData";
            gv.DataBind();
        }

        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            string file = string.Format("{0}-SchoolCrosswalk.xlsx",
                DateTime.Now.ToString("yyyyMMdd"));
            CreateExcelFile.CreateExcelDocument(DAL.SchoolCrosswalk.GetExport(), file, Response);
        }
    }
}