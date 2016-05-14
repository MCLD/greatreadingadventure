using Excel;
using ExportToExcel;
using GRA.SRP.Core.Utilities;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class IEEvents : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5400;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Import/Export Events");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            }
        }

        protected void UploadTest_Click(object sender, EventArgs e)
        {
            HandleExcelFile(false);
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            HandleExcelFile(true);
        }
        protected void HandleExcelFile(bool doImport)
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

                        this.Log().Info("Reading uploaded Excel file of events: {0}",
                            ExcelFileUpload.FileName);

                        if (excelReader == null)
                        {
                            throw new Exception("Could not parse Excel file, not .xls or .xlsx");
                        }

                        excelReader.IsFirstRowAsColumnNames = true;

                        var branchMap = new Dictionary<string, int>();

                        var codeTypes = DAL.CodeType.GetAll().Tables[0];

                        var branchCodeTypeId = (int)codeTypes
                            .Select("CodeTypeName = 'Branch'")
                            .First()["CTID"];

                        var branches = DAL.Codes.GetAlByTypeID(branchCodeTypeId).Tables[0];

                        foreach (DataRow branchRow in branches.Rows)
                        {
                            branchMap.Add(
                                ((string)branchRow["Code"]).ToLower(),
                                (int)branchRow["CID"]
                            );
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
                                string name = null;
                                DateTime? date = null;
                                string description = null;
                                // can't be null for business rule validation
                                string secretCode = null;
                                int? pointsEarned = null;
                                string link = null;
                                bool hiddenFromPublic = false;
                                int? branchId = null;

                                try
                                {
                                    name = excelReader.GetString(0);
                                    if (string.IsNullOrWhiteSpace(name))
                                    {
                                        throw new Exception();
                                    }
                                }
                                catch (Exception)
                                {
                                    problems.Add(string.Format("Not inserting - empty event name on row {0}",
                                        recordCount + 1));
                                    continue;
                                }

                                try
                                {
                                    date = excelReader.GetDateTime(1);
                                }
                                catch (Exception)
                                {
                                    problems.Add(string.Format("Not inserting - invalid event date and time for event: {0}",
                                        name));
                                    continue;
                                }

                                try
                                {
                                    description = excelReader.GetString(2);
                                    if (string.IsNullOrWhiteSpace(description))
                                    {
                                        throw new Exception();
                                    }
                                }
                                catch (Exception)
                                {
                                    problems.Add(string.Format("Not inserting - missing description for event: {0}",
                                        name));
                                    continue;
                                }

                                if (excelReader.FieldCount >= 4)
                                {
                                    try
                                    {
                                        secretCode = excelReader.GetString(3);

                                        var lookupEvent = DAL.Event.GetEventByEventCode(secretCode.Trim());
                                        if (lookupEvent.Tables.Count != 0 && lookupEvent.Tables[0].Rows.Count != 0)
                                        {
                                            problems.Add(string.Format("Skipping code - secret code {0} provided for event {1} is already in use.",
                                                secretCode,
                                                name));
                                        }

                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                if (excelReader.FieldCount >= 5)
                                {
                                    try
                                    {
                                        var pointsEarnedString = excelReader.GetString(4);
                                        if (!string.IsNullOrWhiteSpace(pointsEarnedString))
                                        {
                                            int pointsInt;
                                            if (!int.TryParse(pointsEarnedString, out pointsInt))
                                            {
                                                problems.Add(string.Format("Skipping points earned - couldn't convert points earned to a number for: {0}",
                                                    name));
                                            }
                                            else
                                            {
                                                pointsEarned = pointsInt;
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                if (excelReader.FieldCount >= 6)
                                {

                                    try
                                    {
                                        link = excelReader.GetString(5);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                if (excelReader.FieldCount >= 7)
                                {

                                    try
                                    {
                                        var hiddenFromString = excelReader.GetString(6);
                                        string compareString = hiddenFromString.ToLower();
                                        hiddenFromPublic = compareString.Contains("true")
                                            || compareString.Contains("yes")
                                            || compareString.Contains("1");
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                if (excelReader.FieldCount >= 8)
                                {

                                    try
                                    {
                                        var branchString = excelReader.GetString(7);
                                        
                                        if (branchString != null && branchString.Length > 0)
                                        {
                                            if (branchMap.ContainsKey(branchString.ToLower()))
                                            {
                                                branchId = branchMap[branchString.ToLower()];
                                            }
                                            else
                                            {
                                                problems.Add(string.Format("Skipping branch - couldn't find branch {0} specified for event {1}",
                                                    branchString,
                                                    name));
                                            }
                                        }

                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                var newEvent = new DAL.Event
                                {
                                    EventTitle = name,
                                    EventDate = (DateTime)date,
                                    HTML = description,
                                    SecretCode = secretCode ?? string.Empty,
                                    NumberPoints = pointsEarned ?? 0,
                                    ExternalLinkToEvent = link ?? string.Empty,
                                    HiddenFromPublic = hiddenFromPublic,
                                    BranchID = branchId ?? 0
                                };

                                if (doImport)
                                {
                                    DAL.Event.Insert(newEvent);
                                }
                                else
                                {
                                    if (!newEvent.IsValid(BusinessRulesValidationMode.INSERT))
                                    {
                                        foreach (var errorCode in newEvent.ErrorCodes)
                                        {
                                            problems.Add(string.Format("Issue with event {0}: {1} {2}",
                                                name,
                                                errorCode.FieldName,
                                                errorCode.ErrorMessage));
                                        }
                                    }
                                }
                                recordCount++;
                            }
                            catch (Exception ex)
                            {
                                // couldn't import this record
                                string problem = string.Format("Unable to import row {0}: {1}",
                                    recordCount + 1,
                                    ex.Message);
                                this.Log().Info(problem);
                                problems.Add(problem);
                            }
                        }

                        var result = new StringBuilder(string.Format("Read {0} records and encountered {1} errors in: {2}",
                            recordCount,
                            problems.Count,
                            ExcelFileUpload.FileName));
                        this.Log().Info(result.ToString());
                        if (problems != null && problems.Count > 0)
                        {
                            result.Append("<p>Problems with spreadsheet:<p><ul>");
                            foreach (var problem in problems)
                            {
                                result.Append("<li>");
                                result.Append(problem);
                                result.AppendLine("</li>");
                            }
                            result.AppendLine("</ul>");
                        }
                        PageMessage = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        string result = string.Format("Error reading Excel file for event import: {0} - {1}",
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
        }

        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            string file = string.Format("{0}-Events.xlsx",
                DateTime.Now.ToString("yyyyMMdd"));
            CreateExcelFile.CreateExcelDocument(DAL.Event.GetExport(), file, Response);
        }


    }
}