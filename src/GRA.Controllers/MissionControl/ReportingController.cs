using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GRA.Controllers.ViewModel.MissionControl.Reporting;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewAllReporting)]
    public class ReportingController : Base.MCController
    {
        private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string ExcelFileExtension = "xlsx";
        private const int ExcelStyleIndexBold = 1;
        private const int ExcelPaddingCharacters = 1;

        private readonly ILogger<ReportingController> _logger;
        private readonly ReportService _reportService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public ReportingController(ILogger<ReportingController> logger,
            ServiceFacade.Controller context,
            ReportService reportService,
            SchoolService schoolService,
            SiteService siteService,
            UserService userService,
            VendorCodeService vendorCodeService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _reportService = Require.IsNotNull(reportService, nameof(reportService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
            PageTitle = "Reporting";
        }

        [HttpGet]
        public IActionResult Index()
        {
            PageTitle = "Select a Report";
            return View(_reportService.GetReportList());
        }

        [HttpGet]
        public async Task<IActionResult> Configure(int id)
        {
            var report = _reportService.GetReportList().Where(_ => _.Id == id).SingleOrDefault();
            if (report == null)
            {
                AlertDanger = $"Could not find report of type {id}.";
                return RedirectToAction("Index");
            }
            PageTitle = $"Configure {report.Name}";

            string viewName = report.Name.Replace(" ", string.Empty);
            if (viewName.EndsWith("Report"))
            {
                viewName = viewName.Substring(0, viewName.Length - 6);
            }

            var systemList = await _siteService.GetSystemList(true);
            var branchList = await _siteService.GetAllBranches(true);
            var programList = await _siteService.GetProgramList();
            var schoolDistrictList = await _schoolService.GetDistrictsAsync();
            var schoolList = await _schoolService.GetSchoolsAsync(schoolDistrictList.FirstOrDefault()?.Id);
            var groupInfoList = await _userService.GetGroupInfosAsync();
            var vendorCodeTypeList = await _vendorCodeService.GetTypeAllAsync();
            var site = await GetCurrentSiteAsync();

            return View($"{viewName}Criteria", new ReportCriteriaViewModel
            {
                ReportId = id,
                ProgramStartDate = site.ProgramStarts ?? new DateTime(2018, 01, 01),
                SystemList = new SelectList(systemList, "Id", "Name"),
                BranchList = new SelectList(branchList, "Id", "Name"),
                ProgramList = new SelectList(programList, "Id", "Name"),
                SchoolDistrictList = new SelectList(schoolDistrictList, "Id", "Name"),
                SchoolList = new SelectList(schoolList, "Id", "Name"),
                GroupInfosList = new SelectList(groupInfoList, "Id", "Name"),
                VendorCodeTypeList = new SelectList(vendorCodeTypeList, "Id", "Description")
            });
        }

        [HttpPost]
        public async Task<IActionResult> Run(ReportCriteriaViewModel viewModel)
        {
            PageTitle = "Run the report";

            var criterion = new ReportCriterion
            {
                SiteId = GetCurrentSiteId(),
                EndDate = viewModel.EndDate,
                StartDate = viewModel.StartDate,
                SystemId = viewModel.SystemId,
                BranchId = viewModel.BranchId,
                ProgramId = viewModel.ProgramId,
                SchoolDistrictId = viewModel.SchoolDistrictId,
                SchoolId = viewModel.SchoolId,
                GroupInfoId = viewModel.GroupInfoId,
                VendorCodeTypeId = viewModel.VendorCodeTypeId,
                BadgeRequiredList = viewModel.BadgeRequiredList,
                ChallengeRequiredList = viewModel.ChallengeRequiredList
            };

            var reportRequestId = await _reportService
                .RequestReport(criterion, viewModel.ReportId);

            var wsUrl = await _siteService.GetWsUrl(Request.Scheme, Request.Host.Value);

            return View("Run", new RunReportViewModel
            {
                Id = reportRequestId,
                RunReportUrl = $"{wsUrl}/MissionControl/runreport/{reportRequestId}"
            });
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            try
            {
                var storedReport = await _reportService.GetReportResultsAsync(id);

                PageTitle = storedReport.request.Name ?? "Report Results";

                var viewModel = new ReportResultsViewModel
                {
                    Title = PageTitle,
                    ReportResultId = id
                };

                if (storedReport.criterion.StartDate.HasValue)
                {
                    viewModel.StartDate = storedReport.criterion.StartDate;
                }
                if (storedReport.criterion.EndDate.HasValue)
                {
                    viewModel.EndDate = storedReport.criterion.EndDate;
                }
                if (storedReport.criterion.SystemId.HasValue)
                {
                    viewModel.SystemName = (await _siteService
                        .GetSystemByIdAsync(storedReport.criterion.SystemId.Value)).Name;
                }
                if (storedReport.criterion.BranchId.HasValue)
                {
                    viewModel.BranchName = await _siteService
                        .GetBranchName(storedReport.criterion.BranchId.Value);
                }
                if (storedReport.criterion.ProgramId.HasValue)
                {
                    viewModel.ProgramName = (await _siteService
                        .GetProgramByIdAsync(storedReport.criterion.ProgramId.Value)).Name;
                }
                if (storedReport.criterion.GroupInfoId.HasValue)
                {
                    viewModel.GroupName = (await _userService
                        .GetGroupInfoByIdAsync(storedReport.criterion.GroupInfoId.Value)).Name;
                }
                if (storedReport.criterion.SchoolDistrictId.HasValue)
                {
                    viewModel.SchoolDistrictName = (await _schoolService
                        .GetDistrictByIdAsync(storedReport.criterion.SchoolDistrictId.Value)).Name;
                }
                if (storedReport.criterion.SchoolId.HasValue)
                {
                    viewModel.SchoolName = (await _schoolService
                        .GetByIdAsync(storedReport.criterion.SchoolId.Value)).Name;
                }
                if (storedReport.criterion.VendorCodeTypeId.HasValue)
                {
                    viewModel.VendorCodeName = (await _vendorCodeService
                        .GetTypeById(storedReport.criterion.VendorCodeTypeId.Value)).Description;
                }

                viewModel.ReportSet = JsonConvert
                    .DeserializeObject<StoredReportSet>(storedReport.request.ResultJson);

                foreach (var report in viewModel.ReportSet.Reports)
                {
                    int count = 0;
                    int totalRows = report.Data.Count();
                    var displayRows = new List<List<string>>();

                    if (report.HeaderRow != null)
                    {
                        var display = new List<string>();
                        foreach (var dataItem in report.HeaderRow)
                        {
                            display.Add(FormatDataItem(dataItem));
                        }
                        report.HeaderRow = display;
                    }

                    foreach (var resultRow in report.Data)
                    {
                        var displayRow = new List<string>();

                        foreach (var resultItem in resultRow)
                        {
                            displayRow.Add(FormatDataItem(resultItem));
                        }
                        displayRows.Add(displayRow);
                        count++;
                    }
                    report.Data = displayRows;

                    if (report.FooterRow != null)
                    {
                        var display = new List<string>();
                        foreach (var dataItem in report.FooterRow)
                        {
                            display.Add(FormatDataItem(dataItem));
                        }
                        report.FooterRow = display;
                    }
                }

                return View(viewModel);
            }
            catch (GraException gex)
            {
                AlertDanger = gex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<FileStreamResult> Download(int id)
        {
            var storedReport = await _reportService.GetReportResultsAsync(id);

            PageTitle = storedReport.request.Name ?? "Report Results";

            var viewModel = new ReportResultsViewModel
            {
                Title = PageTitle
            };

            var criteriaDictionnary = new Dictionary<string, object>();
            if (storedReport.criterion.StartDate.HasValue)
            {
                criteriaDictionnary.Add("Start Date", storedReport.criterion.StartDate.Value);
            }
            if (storedReport.criterion.EndDate.HasValue)
            {
                criteriaDictionnary.Add("End Date", storedReport.criterion.StartDate.Value);
            }
            if (storedReport.criterion.SystemId.HasValue)
            {
                criteriaDictionnary.Add("System", (await _siteService
                    .GetSystemByIdAsync(storedReport.criterion.SystemId.Value)).Name);
            }
            if (storedReport.criterion.BranchId.HasValue)
            {
                criteriaDictionnary.Add("Branch", await _siteService
                    .GetBranchName(storedReport.criterion.BranchId.Value));
            }
            if (storedReport.criterion.ProgramId.HasValue)
            {
                criteriaDictionnary.Add("Program", (await _siteService
                    .GetProgramByIdAsync(storedReport.criterion.ProgramId.Value)).Name);
            }
            if (storedReport.criterion.GroupInfoId.HasValue)
            {
                criteriaDictionnary.Add("Group", (await _userService
                    .GetGroupInfoByIdAsync(storedReport.criterion.GroupInfoId.Value)).Name);
            }
            if (storedReport.criterion.SchoolDistrictId.HasValue)
            {
                criteriaDictionnary.Add("School District", (await _schoolService
                    .GetDistrictByIdAsync(storedReport.criterion.SchoolDistrictId.Value)).Name);
            }
            if (storedReport.criterion.SchoolId.HasValue)
            {
                criteriaDictionnary.Add("Program", (await _schoolService
                    .GetByIdAsync(storedReport.criterion.SchoolId.Value)).Name);
            }
            if (storedReport.criterion.VendorCodeTypeId.HasValue)
            {
                criteriaDictionnary.Add("Program", (await _vendorCodeService
                    .GetTypeById(storedReport.criterion.VendorCodeTypeId.Value)).Description);
            }

            viewModel.ReportSet = JsonConvert
                .DeserializeObject<StoredReportSet>(storedReport.request.ResultJson);

            var ms = new System.IO.MemoryStream();

            using (var workbook = SpreadsheetDocument.Create(ms,
                DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook();
                workbook.WorkbookPart.Workbook.Sheets = new Sheets();

                var stylesPart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = GetStylesheet();
                stylesPart.Stylesheet.Save();

                foreach (var report in viewModel.ReportSet.Reports)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    var relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<Sheet>()
                            .Select(_ => _.SheetId.Value).Max() + 1;
                    }

                    string sheetName = report.Title ?? PageTitle ?? "Report Results";
                    if (sheetName.Length > 31)
                    {
                        sheetName = sheetName.Substring(0, 31);
                    }

                    var sheet = new Sheet
                    {
                        Id = relationshipId,
                        SheetId = sheetId,
                        Name = sheetName
                    };
                    sheets.Append(sheet);

                    var maximumColumnWidth = new Dictionary<int, int>();

                    if (report.HeaderRow != null)
                    {
                        var headerRow = new Row();
                        int columnNumber = 0;
                        foreach (var dataItem in report.HeaderRow)
                        {
                            (var cell, var length) = CreateCell(dataItem);
                            cell.StyleIndex = ExcelStyleIndexBold;
                            headerRow.AppendChild(cell);
                            if (maximumColumnWidth.ContainsKey(columnNumber))
                            {
                                maximumColumnWidth[columnNumber]
                                    = Math.Max(maximumColumnWidth[columnNumber], length);
                            }
                            else
                            {
                                maximumColumnWidth.Add(columnNumber, length);
                            }
                            columnNumber++;
                        }
                        sheetData.Append(headerRow);
                    }

                    foreach (var resultRow in report.Data)
                    {
                        var row = new Row();
                        int columnNumber = 0;
                        foreach (var resultItem in resultRow)
                        {
                            (var cell, var length) = CreateCell(resultItem);
                            row.AppendChild(cell);
                            if (maximumColumnWidth.ContainsKey(columnNumber))
                            {
                                maximumColumnWidth[columnNumber]
                                    = Math.Max(maximumColumnWidth[columnNumber], length);
                            }
                            else
                            {
                                maximumColumnWidth.Add(columnNumber, length);
                            }
                            columnNumber++;
                        }
                        sheetData.Append(row);
                    }

                    if (report.FooterRow != null)
                    {
                        var footerRow = new Row();
                        int columnNumber = 0;
                        foreach (var dataItem in report.FooterRow)
                        {
                            (var cell, var length) = CreateCell(dataItem);
                            cell.StyleIndex = ExcelStyleIndexBold;
                            footerRow.AppendChild(cell);
                            if (maximumColumnWidth.ContainsKey(columnNumber))
                            {
                                maximumColumnWidth[columnNumber]
                                    = Math.Max(maximumColumnWidth[columnNumber], length);
                            }
                            else
                            {
                                maximumColumnWidth.Add(columnNumber, length);
                            }
                            columnNumber++;
                        }
                        sheetData.Append(footerRow);
                    }

                    if (report.FooterText != null)
                    {
                        foreach (var dataItem in report.FooterText)
                        {
                            var footerTextRow = new Row();
                            (var cell, var length) = CreateCell(dataItem);
                            footerTextRow.AppendChild(cell);
                            sheetData.Append(footerTextRow);
                        }
                    }

                    foreach (var value in maximumColumnWidth.Keys.OrderByDescending(_ => _))
                    {
                        var columnId = value + 1;
                        var width = maximumColumnWidth[value] + ExcelPaddingCharacters;
                        Columns cs = sheet.GetFirstChild<Columns>();
                        if (cs != null)
                        {
                            var columnElements = cs.Elements<Column>()
                                .Where(_ => _.Min == columnId && _.Max == columnId);
                            if (columnElements.Count() > 0)
                            {
                                var column = columnElements.First();
                                column.Width = width;
                                column.CustomWidth = true;
                            }
                            else
                            {
                                var column = new Column
                                {
                                    Min = (uint)columnId,
                                    Max = (uint)columnId,
                                    Width = width,
                                    CustomWidth = true
                                };
                                cs.Append(column);
                            }
                        }
                        else
                        {
                            cs = new Columns();
                            cs.Append(new Column
                            {
                                Min = (uint)columnId,
                                Max = (uint)columnId,
                                Width = width,
                                CustomWidth = true
                            });
                            sheetPart.Worksheet.InsertAfter(cs,
                                sheetPart.Worksheet.GetFirstChild<SheetFormatProperties>());
                        }
                    }
                }

                var criteriaSheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var criteriaSheetData = new SheetData();
                criteriaSheetPart.Worksheet = new Worksheet(criteriaSheetData);

                var criteriaSheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                var criteriaRelationshipId = workbook.WorkbookPart.GetIdOfPart(criteriaSheetPart);

                uint criteriaSheetId = 1;
                if (criteriaSheets.Elements<Sheet>().Count() > 0)
                {
                    criteriaSheetId = criteriaSheets.Elements<Sheet>()
                        .Select(_ => _.SheetId.Value).Max() + 1;
                }

                string criteriaSheetName = "Report Criteria";

                var criteriaSheet = new Sheet
                {
                    Id = criteriaRelationshipId,
                    SheetId = criteriaSheetId,
                    Name = criteriaSheetName
                };
                criteriaSheets.Append(criteriaSheet);

                var criteriaMaximumColumnWidth = new Dictionary<int, int>();

                foreach (var criterion in criteriaDictionnary)
                {
                    var row = new Row();

                    (var nameCell, var nameLength) = CreateCell(criterion.Key);
                    row.AppendChild(nameCell);
                    if (criteriaMaximumColumnWidth.ContainsKey(0))
                    {
                        criteriaMaximumColumnWidth[0]
                            = Math.Max(criteriaMaximumColumnWidth[0], nameLength);
                    }
                    else
                    {
                        criteriaMaximumColumnWidth.Add(0, nameLength);
                    }

                    (var dataCell, var dataLength) = CreateCell(criterion.Value);
                    row.AppendChild(dataCell);
                    if (criteriaMaximumColumnWidth.ContainsKey(1))
                    {
                        criteriaMaximumColumnWidth[1]
                            = Math.Max(criteriaMaximumColumnWidth[1], dataLength);
                    }
                    else
                    {
                        criteriaMaximumColumnWidth.Add(1, dataLength);
                    }

                    criteriaSheetData.Append(row);
                }

                foreach (var value in criteriaMaximumColumnWidth.Keys.OrderByDescending(_ => _))
                {
                    var columnId = value + 1;
                    var width = criteriaMaximumColumnWidth[value] + ExcelPaddingCharacters;
                    Columns cs = criteriaSheet.GetFirstChild<Columns>();
                    if (cs != null)
                    {
                        var columnElements = cs.Elements<Column>()
                            .Where(_ => _.Min == columnId && _.Max == columnId);
                        if (columnElements.Count() > 0)
                        {
                            var column = columnElements.First();
                            column.Width = width;
                            column.CustomWidth = true;
                        }
                        else
                        {
                            var column = new Column
                            {
                                Min = (uint)columnId,
                                Max = (uint)columnId,
                                Width = width,
                                CustomWidth = true
                            };
                            cs.Append(column);
                        }
                    }
                    else
                    {
                        cs = new Columns();
                        cs.Append(new Column
                        {
                            Min = (uint)columnId,
                            Max = (uint)columnId,
                            Width = width,
                            CustomWidth = true
                        });
                        criteriaSheetPart.Worksheet.InsertAfter(cs,
                            criteriaSheetPart.Worksheet.GetFirstChild<SheetFormatProperties>());
                    }
                }

                workbook.Save();
                workbook.Close();
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                var fileOutput = new FileStreamResult(ms, ExcelMimeType)
                {
                    FileDownloadName = $"{PageTitle}.{ExcelFileExtension}"
                };
                return fileOutput;
            }
        }
        private string FormatDataItem(object dataItem)
        {
            switch (dataItem)
            {
                case int i:
                    return i.ToString("N0");
                case long l:
                    return l.ToString("N0");
                default:
                    return WebUtility.HtmlEncode(dataItem.ToString());
                case null:
                    return string.Empty;
            }
        }

        private (Cell cell, int length) CreateCell(object dataItem)
        {
            var cell = new Cell
            {
                CellValue = new CellValue(dataItem.ToString())
            };

            switch (dataItem)
            {
                case int i:
                case long l:
                    cell.DataType = CellValues.Number;
                    break;
                case DateTime d:
                    cell.DataType = CellValues.Date;
                    break;
                case null:
                default:
                    cell.DataType = CellValues.String;
                    break;
            }

            return (cell, dataItem.ToString().Length);
        }

        private Stylesheet GetStylesheet()
        {
            var stylesheet = new Stylesheet();

            var font = new Font();
            var boldFont = new Font();
            boldFont.Append(new Bold());

            var fonts = new Fonts();
            fonts.Append(font);
            fonts.Append(boldFont);

            var fill = new Fill();
            var fills = new Fills();
            fills.Append(fill);

            var border = new Border();
            var borders = new Borders();
            borders.Append(border);

            var regularFormat = new CellFormat
            {
                FontId = 0
            };
            var boldFormat = new CellFormat
            {
                FontId = 1
            };
            var cellFormats = new CellFormats();
            cellFormats.Append(regularFormat);
            cellFormats.Append(boldFormat);

            stylesheet.Append(fonts);
            stylesheet.Append(fills);
            stylesheet.Append(borders);
            stylesheet.Append(cellFormats);

            return stylesheet;
        }
    }
}
