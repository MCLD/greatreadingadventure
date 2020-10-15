﻿using System;
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
using Microsoft.Extensions.Configuration;
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
        private readonly JobService _jobService;
        private readonly ReportService _reportService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly TriggerService _triggerService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public ReportingController(ILogger<ReportingController> logger,
            ServiceFacade.Controller context,
            JobService jobService,
            ReportService reportService,
            SchoolService schoolService,
            SiteService siteService,
            TriggerService triggerService,
            UserService userService,
            VendorCodeService vendorCodeService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _reportService = reportService
                ?? throw new ArgumentNullException(nameof(reportService));
            _schoolService = schoolService
                ?? throw new ArgumentNullException(nameof(schoolService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _triggerService = triggerService
                ?? throw new ArgumentNullException(nameof(triggerService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
            PageTitle = "Reporting";
        }

        [HttpGet]
        public IActionResult Index()
        {
            PageTitle = "Select a Report";
            var viewModel = new ReportIndexViewModel
            {
                Reports = _reportService.GetReportList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Configure(int id)
        {
            var report = _reportService.GetReportList().SingleOrDefault(_ => _.Id == id);
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

            var triggerList = await _triggerService.GetTriggersAwardingPrizesAsync();
            foreach (var trigger in triggerList)
            {
                trigger.AwardPrizeName += $" ({trigger.Name})";
            }

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
                VendorCodeTypeList = new SelectList(vendorCodeTypeList, "Id", "Description"),
                PrizeList = new SelectList(triggerList, "Id", "AwardPrizeName")
            });
        }

        [HttpPost]
        public async Task<IActionResult> Run(ReportCriteriaViewModel viewModel)
        {
            PageTitle = "Run the report";

            var siteId = GetCurrentSiteId();

            var criterion = new ReportCriterion
            {
                SiteId = siteId,
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

            if (viewModel.TriggerList?.Count > 0)
            {
                criterion.TriggerList = string.Join(",", viewModel.TriggerList);
            }

            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingIntAsync(siteId,
                SiteSettingKey.Users.MaximumActivityPermitted);

            if (IsSet)
            {
                criterion.MaximumAllowableActivity = SetValue;
            }

            int reportRequestId = await _reportService
                .RequestReport(criterion, viewModel.ReportId);

            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.RunReport,
                SerializedParameters = JsonConvert.SerializeObject(new JobDetailsRunReport
                {
                    ReportRequestId = reportRequestId
                })
            });

            return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
            {
                CancelUrl = Url.Action(nameof(Index)),
                JobToken = jobToken.ToString(),
                PingSeconds = 2,
                SuccessRedirectUrl = Url.Action(nameof(View), new { id = reportRequestId }),
                SuccessUrl = "",
                Title = "Loading report..."
            });
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            try
            {
                var (request, criterion) = await _reportService.GetReportResultsAsync(id);

                PageTitle = request.Name ?? "Report Results";

                var viewModel = new ReportResultsViewModel
                {
                    Title = PageTitle,
                    ReportResultId = id
                };

                if (criterion.StartDate.HasValue)
                {
                    viewModel.StartDate = criterion.StartDate;
                }
                if (criterion.EndDate.HasValue)
                {
                    viewModel.EndDate = criterion.EndDate;
                }
                if (criterion.SystemId.HasValue)
                {
                    viewModel.SystemName = (await _siteService
                        .GetSystemByIdAsync(criterion.SystemId.Value)).Name;
                }
                if (criterion.BranchId.HasValue)
                {
                    viewModel.BranchName = await _siteService
                        .GetBranchName(criterion.BranchId.Value);
                }
                if (criterion.ProgramId.HasValue)
                {
                    viewModel.ProgramName = (await _siteService
                        .GetProgramByIdAsync(criterion.ProgramId.Value)).Name;
                }
                if (criterion.GroupInfoId.HasValue)
                {
                    viewModel.GroupName = (await _userService
                        .GetGroupInfoByIdAsync(criterion.GroupInfoId.Value)).Name;
                }
                if (criterion.SchoolDistrictId.HasValue)
                {
                    viewModel.SchoolDistrictName = (await _schoolService
                        .GetDistrictByIdAsync(criterion.SchoolDistrictId.Value)).Name;
                }
                if (criterion.SchoolId.HasValue)
                {
                    viewModel.SchoolName = (await _schoolService
                        .GetByIdAsync(criterion.SchoolId.Value)).Name;
                }
                if (criterion.VendorCodeTypeId.HasValue)
                {
                    viewModel.VendorCodeName = (await _vendorCodeService
                        .GetTypeById(criterion.VendorCodeTypeId.Value)).Description;
                }

                viewModel.ReportSet = JsonConvert
                    .DeserializeObject<StoredReportSet>(request.ResultJson);

                foreach (var report in viewModel.ReportSet.Reports)
                {
                    int count = 0;
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
                criteriaDictionnary.Add("Start Date", storedReport.criterion.StartDate.Value.ToString());
            }
            if (storedReport.criterion.EndDate.HasValue)
            {
                criteriaDictionnary.Add("End Date", storedReport.criterion.EndDate.Value.ToString());
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
                workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook
                {
                    Sheets = new Sheets()
                };

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
                    if (sheets.Elements<Sheet>().Any())
                    {
                        sheetId = sheets.Elements<Sheet>()
                            .Max(_ => _.SheetId.Value) + 1;
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
                            (var cell, var length) = CreateCell(resultItem ?? string.Empty);
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
                            if (columnElements.Any())
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
                if (criteriaSheets.Elements<Sheet>().Any())
                {
                    criteriaSheetId = criteriaSheets.Elements<Sheet>()
                        .Max(_ => _.SheetId.Value) + 1;
                }

                const string criteriaSheetName = "Report Criteria";

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
                        if (columnElements.Any())
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
            var addCell = new Cell
            {
                CellValue = new CellValue(dataItem.ToString())
            };

            switch (dataItem)
            {
                case int _:
                case long _:
                    addCell.DataType = CellValues.Number;
                    break;
                case DateTime _:
                    addCell.DataType = CellValues.Date;
                    break;
                default:
                    addCell.DataType = CellValues.String;
                    break;
            }

            return (addCell, dataItem.ToString().Length);
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
