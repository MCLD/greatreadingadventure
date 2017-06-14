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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public ReportingController(ILogger<ReportingController> logger,
            ServiceFacade.Controller context,
            ReportService reportService,
            SchoolService schoolService,
            SiteService siteService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _reportService = Require.IsNotNull(reportService, nameof(reportService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
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
            PageTitle = "Configure the Report";
            var report = _reportService.GetReportList().Where(_ => _.Id == id).SingleOrDefault();
            if (report == null)
            {
                AlertDanger = $"Could not find report of type {id}.";
                return RedirectToAction("Index");
            }
            string viewName = report.Name.Replace(" ", string.Empty);
            if (viewName.EndsWith("Report"))
            {
                viewName = viewName.Substring(0, viewName.Length - 6);
            }

            var systemList = await _siteService.GetSystemList();
            var branchList = await _siteService.GetAllBranches(true);
            var schoolDistrictList = await _schoolService.GetDistrictsAsync();

            return View($"{viewName}Criteria", new ReportCriteriaViewModel
            {
                ReportId = id,
                SystemList = new SelectList(systemList, "Id", "Name"),
                BranchList = new SelectList(branchList, "Id", "Name"),
                SchoolDistrictList = new SelectList(schoolDistrictList, "Id", "Name")
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
                    return dataItem.ToString();
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
