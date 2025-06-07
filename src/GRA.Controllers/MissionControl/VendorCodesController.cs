using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.VendorCodes;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class VendorCodesController : Base.MCController
    {
        private const string NoAccess = "You do not have access to vendor codes.";

        private readonly EmailManagementService _emailManagementService;
        private readonly JobService _jobService;
        private readonly LanguageService _languageService;
        private readonly ILogger _logger;
        private readonly MessageTemplateService _messageTemplateService;
        private readonly SegmentService _segmentService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public VendorCodesController(ServiceFacade.Controller context,
            ILogger<VendorCodesController> logger,
            EmailManagementService emailManagementService,
            JobService jobService,
            LanguageService languageService,
            MessageTemplateService messageTemplateService,
            SegmentService segmentService,
            UserService userService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(emailManagementService);
            ArgumentNullException.ThrowIfNull(jobService);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(messageTemplateService);
            ArgumentNullException.ThrowIfNull(segmentService);
            ArgumentNullException.ThrowIfNull(userService);
            ArgumentNullException.ThrowIfNull(vendorCodeService);

            _emailManagementService = emailManagementService;
            _jobService = jobService;
            _languageService = languageService;
            _logger = logger;
            _messageTemplateService = messageTemplateService;
            _segmentService = segmentService;
            _userService = userService;
            _vendorCodeService = vendorCodeService;

            PageTitle = "Vendor code management";
        }

        public static string Name
        { get { return "VendorCodes"; } }

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

        [HttpGet]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public IActionResult BulkCodeReassignment()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> BulkCodeReassignment(string reason, IFormFile textFile)
        {
            var issues = new List<string>();
            if (string.IsNullOrEmpty(reason) || reason.Length > 255)
            {
                issues.Add("Please supply a reason between 1 and 255 characters.");
                ModelState.AddModelError(nameof(reason), "You must supply a reason between 1-255 characters.");
            }

            if (textFile?.FileName == null
                || (!string.Equals(Path.GetExtension(textFile.FileName), ".txt",
                    StringComparison.OrdinalIgnoreCase)))
            {
                issues.Add("You must select a .txt file.");
                ModelState.AddModelError(nameof(textFile), "You must select a .txt file.");
            }

            if (ModelState.ErrorCount == 0)
            {
                var tempFile = _pathResolver.ResolvePrivateTempFilePath();
                _logger.LogInformation("Accepted reassignment import file {UploadFile} as {TempFile}",
                    textFile.FileName,
                    tempFile);

                await using var fileStream = new FileStream(tempFile, FileMode.Create);
                await textFile.CopyToAsync(fileStream);

                string file = WebUtility.UrlEncode(Path.GetFileName(tempFile));

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.BulkReassignCodes,
                    SerializedParameters = JsonConvert
                        .SerializeObject(new JobDetailsVendorCodeBulkReassignment
                        {
                            Filename = file,
                            Reason = reason
                        })
                });

                return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                {
                    CancelUrl = Url.Action(nameof(BulkCodeReassignment)),
                    CompleteButton = "Return to Vendor Code Management",
                    JobToken = jobToken.ToString(),
                    PingSeconds = 5,
                    SuccessRedirectUrl = "",
                    SuccessUrl = Url.Action(nameof(BulkCodeReassignment)),
                    Title = "Loading import..."
                });
            }
            else
            {
                AlertDanger = string.Join(' ', issues)?.Trim();
                return RedirectToAction(nameof(BulkCodeReassignment));
            }
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> Configure()
        {
            var vendorCodeTypes = await _vendorCodeService.GetTypeAllAsync();
            var vendorCodeType = vendorCodeTypes?.FirstOrDefault() ?? new VendorCodeType
            {
                SiteId = GetCurrentSiteId(),
                MessageTemplateId = await _messageTemplateService
                        .GetMessageIdAsync("Vendor Code Award"),
            };

            return await ShowConfigurationAsync(new ConfigureViewModel
            {
                VendorCodeType = vendorCodeType
            });
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> DownloadUnreportedEmailAddresses(int vendorCodeTypeId)
        {
            ICollection<VendorCodeEmailAward> unreportedEmailAddresses = null;
            try
            {
                unreportedEmailAddresses = await _vendorCodeService
                    .GetUnreportedEmailAwardCodes(vendorCodeTypeId);

                var report = new StoredReport("Email Award Addresses", _dateTimeProvider.Now)
                {
                    Data = unreportedEmailAddresses
                        .Select(_ => new object[]
                        {
                            _.UserId,
                            _.Name,
                            _.Email
                        }),
                    HeaderRow = new[] { "User Id", "Name", "Email Address" },
                };

                var ms = ExcelExport.GenerateWorkbook(new List<StoredReport> { report });

                return new FileStreamResult(ms, ExcelExport.ExcelMimeType)
                {
                    FileDownloadName = FileUtility
                        .EnsureValidFilename($"EmailAwards.{ExcelExport.ExcelFileExtension}")
                };
            }
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

                await using (var fileStream = new FileStream(tempFile, FileMode.Create))
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
                            Filename = file,
                            OriginalFilename = excelFile.FileName,
                            SiteName = site.Name,
                            VendorCodeTypeId = vendorCodeTypeId
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
                CompleteButton = "Back to Vendor Code Management",
                JobToken = jobToken.ToString(),
                PingSeconds = 5,
                SuccessRedirectUrl = "",
                SuccessUrl = Url.Action(nameof(Index)),
                Title = "Generating vendor codes..."
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageTemplate(int vendorCodeTypeId,
            int languageId,
            string item)
        {
            int? id;
            var vendorCodeType = await _vendorCodeService.GetTypeById(vendorCodeTypeId);

            try
            {
                id = GetMessageTemplateId(vendorCodeType, item);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    Success = false,
                    gex.Message
                });
            }

            var messageTemplateText = id.HasValue
                 ? await _messageTemplateService.GetMessageTextAsync(id.Value, languageId)
                 : default;

            return Json(new
            {
                Success = true,
                messageTemplateText?.Subject,
                messageTemplateText?.Body,
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetSegment(int vendorCodeTypeId,
            int languageId,
            string item)
        {
            int? id;
            var vendorCodeType = await _vendorCodeService.GetTypeById(vendorCodeTypeId);

            try
            {
                id = GetSegmentTextId(vendorCodeType, item);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    Success = false,
                    gex.Message
                });
            }

            var segmentText = id.HasValue
                 ? await _segmentService.GetDbTextAsync(id.Value, languageId)
                 : default;

            return Json(new
            {
                Success = true,
                Text = segmentText
            });
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
                || (!string.Equals(Path.GetExtension(excelFile.FileName), ".xls",
                    StringComparison.OrdinalIgnoreCase)
                && !string.Equals(Path.GetExtension(excelFile.FileName), ".xlsx",
                    StringComparison.OrdinalIgnoreCase)))
            {
                AlertDanger = "You must select an .xls or .xlsx file.";
                ModelState.AddModelError("excelFile", "You must select an .xls or .xlsx file.");
                return RedirectToAction("ImportStatus");
            }

            if (ModelState.ErrorCount == 0)
            {
                var tempFile = _pathResolver.ResolvePrivateTempFilePath();
                _logger.LogInformation("Accepted vendor id {vendorCodeId} import file {UploadFile} as {TempFile}",
                    vendorCodeTypeId,
                    excelFile.FileName,
                    tempFile);

                await using (var fileStream = new FileStream(tempFile, FileMode.Create))
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
                            Filename = file,
                            OriginalFilename = excelFile.FileName,
                            SiteName = site.Name,
                            VendorCodeTypeId = vendorCodeTypeId
                        })
                });

                return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                {
                    CancelUrl = Url.Action(nameof(ImportStatus)),
                    CompleteButton = "Back to Vendor Code Management",
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
        public async Task<IActionResult> Index()
        {
            var vendorCodeTypes = await _vendorCodeService.GetTypeAllAsync();
            var vendorCodeType = vendorCodeTypes?.FirstOrDefault();

            if (vendorCodeType == null)
            {
                return RedirectToAction(nameof(Configure));
            }

            if (UserHasPermission(Permission.ManageVendorCodes))
            {
                var vendorCodeStatus = await _vendorCodeService.GetStatusAsync(vendorCodeType.Id);
                if (vendorCodeStatus.IsConfigured)
                {
                    return View(vendorCodeStatus);
                }
                else
                {
                    return RedirectToAction(nameof(GenerateCodes));
                }
            }

            return RedirectToAction(nameof(ViewPackingSlip));
        }

        [HttpPost]
        public IActionResult LookupPackingSlip(string id)
        {
            return RedirectToAction(nameof(ViewPackingSlip), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPackingSlip(PackingSlipSummary summary)
        {
            ArgumentNullException.ThrowIfNull(summary);

            if (!UserHasPermission(Permission.ManageVendorCodes)
               && !UserHasPermission(Permission.ReceivePackingSlips))
            {
                return RedirectNotAuthorized(NoAccess);
            }

            if (string.IsNullOrEmpty(summary.PackingSlipNumber))
            {
                AlertWarning = "Please enter a valid packing slip number.";
                return View("EnterPackingSlip", new EnterPackingSlipViewModel
                {
                    PackingSlipNumber = summary.PackingSlipNumber,
                    ViewedPackingSlips = await _userService
                        .GetViewedPackingSlipsAsync(GetActiveUserId())
                });
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
            return View("EnterPackingSlip", new EnterPackingSlipViewModel
            {
                PackingSlipNumber = summary.PackingSlipNumber,
                ViewedPackingSlips = await _userService
                    .GetViewedPackingSlipsAsync(GetActiveUserId())
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetMessageTemplate(int vendorCodeTypeId,
            int languageId,
            string item,
            string subject,
            string body)
        {
            try
            {
                if (string.IsNullOrEmpty(item))
                {
                    throw new GraException("Message to change not specified.");
                }

                var vendorCode = await _vendorCodeService.GetTypeById(vendorCodeTypeId)
                    ?? throw new GraException("Could not find the requested vendor code type.");

                var messageTemplateId = GetMessageTemplateId(vendorCode, item);

                if (messageTemplateId.HasValue)
                {
                    if ((string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
                        && item == nameof(vendorCode.MessageTemplateId))
                    {
                        throw new GraException("You cannot remove the achievement message.");
                    }

                    await _messageTemplateService.UpdateTextAsync(messageTemplateId.Value,
                        languageId,
                        subject?.Trim(),
                        body?.Trim());

                    if (string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(body))
                    {
                        var languageCheck = await _messageTemplateService
                            .GetLanguageStatusAsync(new[] {
                                GetMessageTemplateId(vendorCode, item)
                            });

                        if (languageCheck.First().Value.Length == 0)
                        {
                            switch (item)
                            {
                                case nameof(vendorCode.DonationMessageTemplateId):
                                    vendorCode.DonationMessageTemplateId = null;
                                    break;

                                case nameof(vendorCode.EmailAwardMessageTemplateId):
                                    vendorCode.EmailAwardMessageTemplateId = null;
                                    break;

                                case nameof(vendorCode.OptionMessageTemplateId):
                                    vendorCode.OptionMessageTemplateId = null;
                                    break;

                                default:
                                    throw new GraException("Unable to determine which message template to set.");
                            }
                            await _vendorCodeService.UpdateTypeAsync(vendorCode);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(body))
                    {
                        throw new GraException("Unable to add empty message.");
                    }

                    // messageTemplate needs to be inserted
                    var text = await _messageTemplateService.AddTextAsync(languageId,
                        subject,
                        body,
                        item);

                    switch (item)
                    {
                        case nameof(vendorCode.DonationMessageTemplateId):
                            vendorCode.DonationMessageTemplateId = text.MessageTemplateId;
                            break;

                        case nameof(vendorCode.EmailAwardMessageTemplateId):
                            vendorCode.EmailAwardMessageTemplateId = text.MessageTemplateId;
                            break;

                        case nameof(vendorCode.MessageTemplateId):
                            vendorCode.MessageTemplateId = text.MessageTemplateId;
                            break;

                        case nameof(vendorCode.OptionMessageTemplateId):
                            vendorCode.OptionMessageTemplateId = text.MessageTemplateId;
                            break;

                        default:
                            throw new GraException("Unable to determine which message template to set.");
                    }

                    await _vendorCodeService.UpdateTypeAsync(vendorCode);
                }

                return Json(new
                {
                    Success = true
                });
            }
            catch (GraFieldValidationException gfvex)
            {
                var builder = new StringBuilder("Could not save vendor code type changes:")
                    .AppendLine();
                foreach (var validationError in gfvex.FieldValidationErrors)
                {
                    foreach (var errorMessage in validationError)
                    {
                        builder.Append("- ").AppendLine(errorMessage);
                    }
                }
                return Json(new
                {
                    Success = false,
                    Message = builder.ToString()
                });
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    Success = false,
                    gex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetSegment(int vendorCodeTypeId,
            int languageId,
            string item,
            string text)
        {
            try
            {
                _logger.LogInformation("Message for code {Code} language {LanguageId} item {Item}: {Text}",
                    vendorCodeTypeId,
                    languageId,
                    item,
                    text);

                if (string.IsNullOrEmpty(item))
                {
                    throw new GraException("Pop-up message to change not specified.");
                }

                var vendorCode = await _vendorCodeService.GetTypeById(vendorCodeTypeId)
                    ?? throw new GraException("Could not find the requested vendor code type.");

                var segmentTextId = GetSegmentTextId(vendorCode, item);

                if (segmentTextId.HasValue)
                {
                    await _segmentService.UpdateTextAsync(segmentTextId.Value,
                        languageId,
                        text?.Trim());

                    if (string.IsNullOrEmpty(text))
                    {
                        var languageCheck = await _segmentService
                                .GetLanguageStatusAsync(new[] {
                                    GetSegmentTextId(vendorCode, item)
                            });

                        if (languageCheck.First().Value.Length == 0)
                        {
                            switch (item)
                            {
                                case nameof(vendorCode.DonationSegmentId):
                                    vendorCode.DonationSegmentId = null;
                                    break;

                                case nameof(vendorCode.EmailAwardSegmentId):
                                    vendorCode.EmailAwardSegmentId = null;
                                    break;

                                default:
                                    throw new GraException("Unable to determine which message to set.");
                            }
                            await _vendorCodeService.UpdateTypeAsync(vendorCode);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        throw new GraException("Unable to add empty message.");
                    }

                    var addedText = await _segmentService.AddTextAsync(GetActiveUserId(),
                        languageId,
                        SegmentType.VendorCode,
                        text,
                        item);

                    if (item == nameof(vendorCode.DonationSegmentId))
                    {
                        vendorCode.DonationSegmentId = addedText.SegmentId;
                    }
                    else if (item == nameof(vendorCode.EmailAwardSegmentId))
                    {
                        vendorCode.EmailAwardSegmentId = addedText.SegmentId;
                    }
                    else
                    {
                        throw new GraException("Unable to determine which message to set.");
                    }

                    await _vendorCodeService.UpdateTypeAsync(vendorCode);
                }

                return Json(new
                {
                    Success = true
                });
            }
            catch (GraFieldValidationException gfvex)
            {
                var builder = new StringBuilder("Could not save vendor code type changes:")
                    .AppendLine();
                foreach (var validationError in gfvex.FieldValidationErrors)
                {
                    foreach (var errorMessage in validationError)
                    {
                        builder.Append("- ").AppendLine(errorMessage);
                    }
                }
                return Json(new
                {
                    Success = false,
                    Message = builder.ToString()
                });
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    Success = false,
                    gex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> UpdateConfiguration(ConfigureViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            if (viewModel.VendorCodeType == null)
            {
                ShowAlertDanger("Could not create empty vendor code type.");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                ShowAlertWarning("There were issues with the vendor code.");
                return await ShowConfigurationAsync(viewModel);
            }

            try
            {
                var existingVendorCodeType = await _vendorCodeService.GetTypeAllAsync();
                if (existingVendorCodeType?.Count > 0)
                {
                    viewModel.VendorCodeType.Id = existingVendorCodeType.First().Id;
                    await _vendorCodeService.UpdateTypeAsync(viewModel.VendorCodeType);
                }
                else
                {
                    await _vendorCodeService.AddTypeAsync(viewModel.VendorCodeType);
                }
            }
            catch (GraFieldValidationException gex)
            {
                foreach (var validationError in gex.FieldValidationErrors)
                {
                    foreach (var errorMessage in validationError)
                    {
                        ModelState.AddModelError(nameof(viewModel.VendorCodeType)
                                + '.'
                                + validationError.Key,
                            errorMessage);
                    }
                }
                ShowAlertWarning("There were issues updating the vendor code.");
                return await ShowConfigurationAsync(viewModel);
            }

            ShowAlertSuccess("Vendor code updated!");

            return RedirectToAction(nameof(Configure));
        }

        [HttpGet]
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> ViewHoldSlips(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("EnterPackingSlip", new EnterPackingSlipViewModel
                {
                    ViewedPackingSlips = await _userService
                        .GetViewedPackingSlipsAsync(GetActiveUserId())
                });
            }

            var holdSlipSummary = await _vendorCodeService.GetHoldSlipsAsync(id);

            if (holdSlipSummary.VendorCodes.Count > 0
                || holdSlipSummary.VendorCodePackingSlip != null)
            {
                return View("HoldSlips", holdSlipSummary);
            }

            ShowAlertDanger($"Could not find packing slip number {id}, please contact your administrator.");
            return View("EnterPackingSlip", new EnterPackingSlipViewModel
            {
                PackingSlipNumber = id,
                ViewedPackingSlips = await _userService
                    .GetViewedPackingSlipsAsync(GetActiveUserId())
            });
        }

        [HttpGet]
        public async Task<IActionResult> ViewPackingSlip(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("EnterPackingSlip", new EnterPackingSlipViewModel
                {
                    ViewedPackingSlips = await _userService
                        .GetViewedPackingSlipsAsync(GetActiveUserId())
                });
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
                foreach (var tns in summary.VendorCodes
                    .Where(_ => !string.IsNullOrEmpty(_.TrackingNumber))
                    .Select(_ => _.TrackingNumber).Distinct())
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

                await _userService.ViewPackingSlipAsync(GetActiveUserId(), id);

                return View("ViewPackingSlip", summary);
            }

            ShowAlertDanger($"Could not find packing slip number {id}, please contact your administrator.");
            return View("EnterPackingSlip", new EnterPackingSlipViewModel
            {
                PackingSlipNumber = id,
                ViewedPackingSlips = await _userService
                    .GetViewedPackingSlipsAsync(GetActiveUserId())
            });
        }

        private static int? GetMessageTemplateId(VendorCodeType vendorCodeType, string item)
            => item switch
            {
                nameof(vendorCodeType.DonationMessageTemplateId)
                    => vendorCodeType.DonationMessageTemplateId,
                nameof(vendorCodeType.EmailAwardMessageTemplateId)
                    => vendorCodeType.EmailAwardMessageTemplateId,
                nameof(vendorCodeType.MessageTemplateId) => vendorCodeType.MessageTemplateId,
                nameof(vendorCodeType.OptionMessageTemplateId)
                => vendorCodeType.OptionMessageTemplateId,
                _ => throw new GraException("Unknown message template type")
            };

        private static int? GetSegmentTextId(VendorCodeType vendorCodeType, string item)
            => item switch
            {
                nameof(vendorCodeType.DonationSegmentId) => vendorCodeType.DonationSegmentId,
                nameof(vendorCodeType.EmailAwardSegmentId) => vendorCodeType.EmailAwardSegmentId,
                _ => throw new GraException("Unknown segment type")
            };

        private async Task<IActionResult> ShowConfigurationAsync(ConfigureViewModel viewModel)
        {
            viewModel.DirectEmailTemplates = await _emailManagementService.GetUserTemplatesAsync();
            viewModel.MessageTemplateLanguageIds = await _messageTemplateService
                .GetLanguageStatusAsync(new int?[] {
                    viewModel.VendorCodeType.MessageTemplateId,
                    viewModel.VendorCodeType.DonationMessageTemplateId,
                    viewModel.VendorCodeType.EmailAwardMessageTemplateId,
                    viewModel.VendorCodeType.OptionMessageTemplateId
                });
            viewModel.SegmentLanguageIds = await _segmentService
                .GetLanguageStatusAsync(new int?[]
                {
                    viewModel.VendorCodeType.DonationSegmentId,
                    viewModel.VendorCodeType.EmailAwardSegmentId
                });
            viewModel.Languages = (await _languageService.GetActiveAsync())
                .OrderByDescending(_ => _.IsDefault)
                .ThenBy(_ => _.Description)
                .ToDictionary(k => k.Description, v => v.Id);

            return View("Configure", viewModel);
        }
    }
}
