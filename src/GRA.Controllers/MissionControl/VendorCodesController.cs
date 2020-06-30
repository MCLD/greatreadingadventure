using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageVendorCodes)]
    public class VendorCodesController : Base.MCController
    {
        private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string ExcelFileExtension = "xlsx";
        private const int ExcelStyleIndexBold = 1;
        private const int ExcelPaddingCharacters = 1;

        private readonly ILogger _logger;
        private readonly JobService _jobService;
        private readonly VendorCodeService _vendorCodeService;

        public VendorCodesController(ServiceFacade.Controller context,
            ILogger<VendorCodesController> logger,
            JobService jobService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
            PageTitle = "Vendor code management";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _vendorCodeService.GetStatusAsync());
        }

        [HttpGet]
        public async Task<IActionResult> ImportStatus()
        {
            var codeTypes = await _vendorCodeService.GetTypeAllAsync();
            var codeTypeSelectList = codeTypes.Select(_ => new SelectListItem
            {
                Value = _.Id.ToString(),
                Text = _.Description
            });

            PageTitle = "Vendor Code Import Status";

            return View(codeTypeSelectList);
        }

        [HttpPost]
        public async Task<IActionResult> ImportStatus(int vendorCodeId,
            Microsoft.AspNetCore.Http.IFormFile excelFile)
        {
            if (excelFile == null
                || !string.Equals(Path.GetExtension(excelFile.FileName), ".xls",
                    StringComparison.OrdinalIgnoreCase))
            {
                AlertDanger = "You must select an .xls file.";
                ModelState.AddModelError("excelFile", "You must select an .xls file.");
                return RedirectToAction("ImportStatus");
            }

            if (ModelState.ErrorCount == 0)
            {
                var tempFile = _pathResolver.ResolvePrivateTempFilePath();
                _logger.LogInformation("Accepted vendor id {vendorCodeId} import file {UploadFile} as {TempFile}",
                    vendorCodeId,
                    excelFile.FileName,
                    tempFile);

                using (var fileStream = new FileStream(tempFile, FileMode.Create))
                {
                    await excelFile.CopyToAsync(fileStream);
                }

                string file = WebUtility.UrlEncode(Path.GetFileName(tempFile));

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.UpdateVendorStatus,
                    SerializedParameters = JsonConvert
                        .SerializeObject(new JobDetailsVendorCodeStatus
                        {
                            Filename = file
                        })
                });

                return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                {
                    CancelUrl = Url.Action(nameof(ImportStatus)),
                    JobToken = jobToken.ToString(),
                    PingSeconds = 5,
                    SuccessRedirectUrl = "",
                    SuccessUrl = Url.Action(nameof(ImportStatus)),
                    Title = "Loading import..."
                });
            }
            else
            {
                return RedirectToAction("ImportStatus");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmailAward()
        {
            var codeTypes = await _vendorCodeService.GetTypeAllAsync();
            var codeTypeSelectList = codeTypes.Select(_ => new SelectListItem
            {
                Value = _.Id.ToString(),
                Text = _.Description
            });

            PageTitle = "Vendor Code Email Award";

            return View(codeTypeSelectList);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadUnreportedEmailAddresses(int vendorCodeTypeId)
        {
            var unreportedEmailAddresses = await _vendorCodeService
                .GetUnreportedEmailAwardCodes(vendorCodeTypeId);

            var processed = _dateTimeProvider.Now;

            try
            {
                // this will be disposed by FileStreamResult
                var ms = new MemoryStream();

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

                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    var relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    var sheet = new Sheet
                    {
                        Id = relationshipId,
                        SheetId = 1,
                        Name = "Email Award Addresses"
                    };
                    sheets.Append(sheet);

                    var maximumColumnWidth = new Dictionary<int, int>();

                    var headerColumns = new string[] {
                            "User Id",
                            "Name",
                            "Email Address"
                        };

                    var headerRow = new Row();
                    int columnNumber = 0;
                    foreach (var dataItem in headerColumns)
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

                    foreach (var emailAddress in unreportedEmailAddresses)
                    {
                        var row = new Row();
                        int rowColumnNumber = 0;

                        var rowValues = new object[]
                        {
                                emailAddress.UserId,
                                emailAddress.Name,
                                emailAddress.Email
                        };

                        foreach (var resultItem in rowValues)
                        {
                            (var cell, var length) = CreateCell(resultItem ?? string.Empty);
                            row.AppendChild(cell);
                            if (maximumColumnWidth.ContainsKey(rowColumnNumber))
                            {
                                maximumColumnWidth[rowColumnNumber]
                                    = Math.Max(maximumColumnWidth[rowColumnNumber], length);
                            }
                            else
                            {
                                maximumColumnWidth.Add(rowColumnNumber, length);
                            }
                            rowColumnNumber++;
                        }
                        sheetData.Append(row);

                        await _vendorCodeService
                            .UpdateEmailReportedAsync(GetActiveUserId(),
                            processed,
                            emailAddress.VendorCodeId);
                    }

                    await _vendorCodeService.SaveAsync();

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

                    workbook.Save();
                    workbook.Close();
                    ms.Seek(0, SeekOrigin.Begin);

                    return new FileStreamResult(ms, ExcelMimeType)
                    {
                        FileDownloadName = FileUtility
                            .EnsureValidFilename($"EmailAwards.{ExcelFileExtension}")
                    };
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error creating report of unreported email award codes for Vendor Code Id {vendorCodeTypeId}: {Message}",
                    vendorCodeTypeId,
                    ex.Message);
                ShowAlertDanger("Error creating report of unreported email award codes");

                await _vendorCodeService.UpdateEmailNotReportedAsync(GetActiveUserId(),
                    unreportedEmailAddresses);

                return RedirectToAction(nameof(EmailAward));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        [HttpPost]
        public async Task<IActionResult> EmailAwardStatus(int vendorCodeTypeId,
            Microsoft.AspNetCore.Http.IFormFile excelFile)
        {
            if (excelFile == null
                || !string.Equals(Path.GetExtension(excelFile.FileName), ".xls",
                    StringComparison.OrdinalIgnoreCase))
            {
                AlertDanger = "You must select an .xls file.";
                ModelState.AddModelError("excelFile", "You must select an .xls file.");
                return RedirectToAction(nameof(EmailAward));
            }

            if (ModelState.ErrorCount == 0)
            {
                var tempFile = _pathResolver.ResolvePrivateTempFilePath();
                _logger.LogInformation("Accepted vendor code type {vendorCodeTypeId} email award file {UploadFile} as {TempFile}",
                    vendorCodeTypeId,
                    excelFile.FileName,
                    tempFile);

                using (var fileStream = new FileStream(tempFile, FileMode.Create))
                {
                    await excelFile.CopyToAsync(fileStream);
                }

                string file = WebUtility.UrlEncode(Path.GetFileName(tempFile));

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.UpdateEmailAwardStatus,
                    SerializedParameters = JsonConvert
                        .SerializeObject(new JobDetailsVendorCodeStatus
                        {
                            Filename = file
                        })
                });

                return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                {
                    CancelUrl = Url.Action(nameof(EmailAward)),
                    JobToken = jobToken.ToString(),
                    PingSeconds = 5,
                    SuccessRedirectUrl = "",
                    SuccessUrl = Url.Action(nameof(EmailAward)),
                    Title = "Loading email award status..."
                });
            }
            else
            {
                return RedirectToAction(nameof(EmailAward));
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
