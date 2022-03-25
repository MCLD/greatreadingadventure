using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class VendorCodesController : Base.MCController
    {
        private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string ExcelFileExtension = "xlsx";
        private const int ExcelStyleIndexBold = 1;
        private const int ExcelPaddingCharacters = 1;

        private const string NoAccess = "You do not have access to vendor codes.";

        private readonly ILogger _logger;
        private readonly EmailManagementService _emailManagementService;
        private readonly JobService _jobService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public static string Name { get { return "VendorCodes"; } }

        public VendorCodesController(ServiceFacade.Controller context,
            ILogger<VendorCodesController> logger,
            EmailManagementService emailManagementService,
            JobService jobService,
            UserService userService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailManagementService = emailManagementService
                ?? throw new ArgumentNullException(nameof(emailManagementService));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = "Vendor code management";
        }

        /// <summary>
        /// Drop-down list options (true/false) for ship date
        /// </summary>
        public static IEnumerable<SelectListItem> ShipDateOptions
        {
            get
            {
                return new[]
                {
                    new SelectListItem("Don't award a prize when item marked shipped from an import", "False"),
                    new SelectListItem("Award a prize when item is marked shipped from an import", "True")
                };
            }
        }

        /// <summary>
        /// Drop-down list options (true/false) for packing slip
        /// </summary>
        public static IEnumerable<SelectListItem> PackingSlipOptions
        {
            get
            {
                return new[]
                {
                    new SelectListItem("Don't award a prize when item is received via packing slip entry", "False"),
                    new SelectListItem("Award a prize when item is received via packing slip entry", "True")

                };
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (UserHasPermission(Permission.ManageVendorCodes))
            {
                var vendorCodeStatus = await _vendorCodeService.GetStatusAsync();
                if (vendorCodeStatus.IsConfigured)
                {
                    return View(vendorCodeStatus);
                }
                else
                {
                    var vendorCodeType = await _vendorCodeService.GetTypeAllAsync();
                    if (vendorCodeType?.Count == 0)
                    {
                        return RedirectToAction(nameof(Configure));
                    }
                    else
                    {
                        return RedirectToAction(nameof(GenerateCodes));
                    }
                }
            }

            return RedirectToAction(nameof(ViewPackingSlip));
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> ImportStatus()
        {
            var codeTypes = await _vendorCodeService.GetTypeAllAsync();
            var codeTypeSelectList = codeTypes.Select(_ => new SelectListItem
            {
                Value = _.Id.ToString(CultureInfo.InvariantCulture),
                Text = _.Description
            });

            PageTitle = "Vendor Code Import Status";

            return View(codeTypeSelectList);
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> ImportStatus(int vendorCodeTypeId,
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
                    vendorCodeTypeId,
                    excelFile.FileName,
                    tempFile);

                using (var fileStream = new FileStream(tempFile, FileMode.Create))
                {
                    await excelFile.CopyToAsync(fileStream);
                }

                string file = WebUtility.UrlEncode(Path.GetFileName(tempFile));

                var site = await GetCurrentSiteAsync();

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.UpdateVendorStatus,
                    SerializedParameters = JsonConvert
                        .SerializeObject(new JobDetailsVendorCodeStatus
                        {
                            VendorCodeTypeId = vendorCodeTypeId,
                            Filename = file,
                            SiteName = site.Name
                        })
                }); ;

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
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> EmailAward()
        {
            var codeTypes = await _vendorCodeService.GetTypeAllAsync();
            var codeTypeSelectList = codeTypes.Select(_ => new SelectListItem
            {
                Value = _.Id.ToString(CultureInfo.InvariantCulture),
                Text = _.Description
            });

            PageTitle = "Vendor Code Email Award";

            return View(codeTypeSelectList);
        }

#pragma warning disable S3220 // Method calls should not resolve ambiguously to overloads with "params"
        [HttpGet]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> DownloadUnreportedEmailAddresses(int vendorCodeTypeId)
        {
            var unreportedEmailAddresses = await _vendorCodeService
                .GetUnreportedEmailAwardCodes(vendorCodeTypeId);

            var processed = _dateTimeProvider.Now;

            try
            {
                // this will be disposed by FileStreamResult
                var ms = new MemoryStream();

                using var workbook = SpreadsheetDocument.Create(ms,
                    DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);
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
#pragma warning restore S3220 // Method calls should not resolve ambiguously to overloads with "params"

        [HttpPost]
        [Authorize(Policy = Policy.ManageVendorCodes)]
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

                var site = await GetCurrentSiteAsync();

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.UpdateEmailAwardStatus,
                    SerializedParameters = JsonConvert
                        .SerializeObject(new JobDetailsVendorCodeStatus
                        {
                            VendorCodeTypeId = vendorCodeTypeId,
                            Filename = file,
                            SiteName = site.Name
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

        [HttpGet]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> Configure()
        {
            var vendorCodeType = await _vendorCodeService.GetTypeAllAsync();

            var viewModel = vendorCodeType?.FirstOrDefault() ?? new VendorCodeType
            {
                SiteId = GetCurrentSiteId()
            };

            viewModel.DirectEmailTemplates = await _emailManagementService.GetUserTemplatesAsync();

            return View("Configure", viewModel);
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> UpdateConfiguration(VendorCodeType vendorCodeType)
        {
            if (vendorCodeType == null)
            {
                AlertDanger = "Could not create empty vendor code type.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                vendorCodeType.DirectEmailTemplates = await _emailManagementService
                    .GetUserTemplatesAsync();

                return View("Configure", vendorCodeType);
            }

            try
            {
                var existingVendorCodeType = await _vendorCodeService.GetTypeAllAsync();
                if (existingVendorCodeType?.Count > 0)
                {
                    vendorCodeType.Id = existingVendorCodeType.First().Id;
                    await _vendorCodeService.UpdateTypeAsync(vendorCodeType);
                }
                else
                {
                    await _vendorCodeService.AddTypeAsync(vendorCodeType);
                }
            }
            catch (GraFieldValidationException gex)
            {
                foreach (var validationError in gex.FieldValidationErrors)
                {
                    foreach (var errorMessage in validationError)
                    {
                        ModelState.AddModelError(validationError.Key, errorMessage);
                    }
                }
                return View("Configure", vendorCodeType);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> GenerateCodes()
        {
            var vendorCodeType = await _vendorCodeService.GetTypeAllAsync();
            if (vendorCodeType?.FirstOrDefault() == null)
            {
                AlertDanger = "You must create a vendor code type before you can generate codes.";
                return RedirectToAction(nameof(Index));
            }
            return View("GenerateCodes", vendorCodeType.First().Description);
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> GenerateCodes(int numberOfCodes)
        {
            var existingVendorCodeType = await _vendorCodeService.GetTypeAllAsync();
            if (existingVendorCodeType?.Count != 1)
            {
                AlertDanger = "Could not generate codes, unable to determine vendor code type";
                return RedirectToAction(nameof(Index));
            }

            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.GenerateVendorCodes,
                SerializedParameters = JsonConvert.SerializeObject(
                new JobDetailsGenerateVendorCodes
                {
                    NumberOfCodes = numberOfCodes,
                    VendorCodeTypeId = existingVendorCodeType.First().Id,
                    CodeLength = 15
                })
            });

            return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
            {
                CancelUrl = Url.Action(nameof(Index)),
                JobToken = jobToken.ToString(),
                PingSeconds = 5,
                SuccessRedirectUrl = "",
                SuccessUrl = Url.Action(nameof(Index)),
                Title = "Generating vendor codes..."
            });
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> ExportCodes()
        {
            var vendorCodeType = await _vendorCodeService.GetTypeAllAsync();
            if (vendorCodeType?.FirstOrDefault() == null)
            {
                AlertDanger = "You must create a vendor code type before you can export codes.";
                return RedirectToAction(nameof(Index));
            }

            string date = _dateTimeProvider.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string codeFileName = vendorCodeType
                .First()
                .Description
                .Replace(" ", "", StringComparison.OrdinalIgnoreCase);

            return File(await _vendorCodeService.ExportVendorCodesAsync(vendorCodeType.First().Id),
                "text/plain",
                FileUtility.EnsureValidFilename($"{date}-{codeFileName}.txt"));
        }

        [HttpPost]
        public IActionResult LookupPackingSlip(long id)
        {
            return RedirectToAction(nameof(ViewPackingSlip), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> ViewPackingSlip(long id)
        {
            if (id == 0)
            {
                return View("EnterPackingSlip");
            }

            var summary = await _vendorCodeService.VerifyPackingSlipAsync(id);
            summary.CanViewDetails = UserHasPermission(Permission.ViewParticipantDetails);

            if (summary.VendorCodes.Count > 0 || summary.VendorCodePackingSlip != null)
            {
                if (summary.VendorCodePackingSlip != null)
                {
                    if (summary.CanViewDetails)
                    {
                        var enteredBy = await _userService
                            .GetDetailsByPermission(summary.VendorCodePackingSlip.CreatedBy);
                        summary.ReceivedBy = enteredBy.FullName;
                    }
                }
                else
                {
                    var vendorCodeType = await _vendorCodeService
                        .GetTypeById(summary.VendorCodes.First().VendorCodeTypeId);
                    summary.CanBeReceived = UserHasPermission(Permission.ReceivePackingSlips)
                        || UserHasPermission(Permission.ManageVendorCodes);
                    summary.SubmitText = vendorCodeType.AwardPrizeOnPackingSlip
                        ? "Mark as received and award prizes"
                        : "Mark as received";
                }

                var tracking = new HashSet<string>();
                foreach (var tns in summary.VendorCodes.Select(_ => _.TrackingNumber).Distinct())
                {
                    foreach (var tn in tns.Split(','))
                    {
                        tracking.Add(tn.Trim());
                    }
                }

                if (tracking.Count > 0)
                {
                    summary.TrackingNumbers = tracking;
                }

                return View("ViewPackingSlip", summary);
            }

            ShowAlertDanger($"Could not find packing slip number {id}, please contact your administrator.");
            return View("EnterPackingSlip", id);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPackingSlip(PackingSlipSummary summary)
        {
            if (!UserHasPermission(Permission.ManageVendorCodes)
               && !UserHasPermission(Permission.ReceivePackingSlips))
            {
                return RedirectNotAuthorized(NoAccess);
            }

            if (summary.PackingSlipNumber == 0)
            {
                AlertWarning = "Please enter a valid packing slip number.";
                return View("EnterPackingSlip", summary.PackingSlipNumber);
            }

            var damagedItems = summary.DamagedItems;
            var missingItems = summary.MissingItems;

            summary = await _vendorCodeService.VerifyPackingSlipAsync(summary.PackingSlipNumber);

            if (summary.VendorCodes.Count > 0 || summary.VendorCodePackingSlip != null)
            {
                if (summary.VendorCodePackingSlip != null)
                {
                    var enteredBy = await _userService
                        .GetDetails(summary.VendorCodePackingSlip.CreatedBy);
                    ShowAlertWarning($"This packing slip was already received on {summary.VendorCodePackingSlip.CreatedAt} by {enteredBy.FullName}.");

                    summary.CanViewDetails = UserHasPermission(Permission.ViewParticipantDetails);

                    return View("ViewPackingSlip", summary);
                }
                else
                {
                    var currentSite = await GetCurrentSiteAsync();
                    var jobToken = await _jobService.CreateJobAsync(new Job
                    {
                        JobType = JobType.ReceivePackingSlip,
                        SerializedParameters = JsonConvert.SerializeObject(
                        new JobDetailsReceivePackingSlip
                        {
                            SiteName = currentSite.Name,
                            PackingSlipNumber = summary.PackingSlipNumber,
                            DamagedItems = damagedItems,
                            MissingItems = missingItems
                        })
                    });

                    return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                    {
                        CancelUrl = Url.Action(nameof(Index)),
                        JobToken = jobToken.ToString(),
                        PingSeconds = 5,
                        SuccessRedirectUrl = "",
                        SuccessUrl = Url.Action(nameof(Index)),
                        Title = "Receiving packing slip..."
                    });
                }
            }

            ShowAlertDanger($"Could not find packing slip number {summary.PackingSlipNumber}, please contact your administrator.");
            return View("EnterPackingSlip", summary.PackingSlipNumber);
        }

        #region Spreadsheet utility methods
        private static (Cell cell, int length) CreateCell(object dataItem)
        {
            var addCell = new Cell
            {
                CellValue = new CellValue(dataItem.ToString())
            };

            addCell.DataType = dataItem switch
            {
                int _ or long _ => CellValues.Number,
                DateTime _ => CellValues.Date,
                _ => CellValues.String,
            };
            return (addCell, dataItem.ToString().Length);
        }

#pragma warning disable S3220 // Method calls should not resolve ambiguously to overloads with "params"
        private static Stylesheet GetStylesheet()
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
#pragma warning restore S3220 // Method calls should not resolve ambiguously to overloads with "params"
        #endregion Spreadsheet utility methods
    }
}
