using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Reporting;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewAllReporting)]
    public class ReportingController : Base.MCController
    {
        private readonly BadgeService _badgeService;
        private readonly ChallengeService _challengeService;
        private readonly JobService _jobService;
        private readonly ILogger<ReportingController> _logger;
        private readonly ReportService _reportService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly TriggerService _triggerService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public ReportingController(ILogger<ReportingController> logger,
            ServiceFacade.Controller context,
            BadgeService badgeService,
            ChallengeService challengeService,
            JobService jobService,
            ReportService reportService,
            SchoolService schoolService,
            SiteService siteService,
            TriggerService triggerService,
            UserService userService,
            VendorCodeService vendorCodeService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(badgeService);
            ArgumentNullException.ThrowIfNull(challengeService);
            ArgumentNullException.ThrowIfNull(jobService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(reportService);
            ArgumentNullException.ThrowIfNull(schoolService);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(triggerService);
            ArgumentNullException.ThrowIfNull(userService);
            ArgumentNullException.ThrowIfNull(vendorCodeService);

            _badgeService = badgeService;
            _challengeService = challengeService;
            _jobService = jobService;
            _logger = logger;
            _reportService = reportService;
            _schoolService = schoolService;
            _siteService = siteService;
            _triggerService = triggerService;
            _userService = userService;
            _vendorCodeService = vendorCodeService;

            PageTitle = "Reporting";
        }

        public static string Name
        {
            get
            {
                return "Reporting";
            }
        }

        [HttpGet]
        public async Task<IActionResult> Configure(int id)
        {
            var report = _reportService.GetReportList().SingleOrDefault(_ => _.Id == id);
            if (report == null)
            {
                ShowAlertDanger($"Could not find report of type {id}.");
                return RedirectToAction(nameof(Index));
            }
            PageTitle = $"Configure report: {report.Name}";

            string viewName = report.Name.Replace(" ",
                string.Empty,
                StringComparison.OrdinalIgnoreCase);
            if (viewName.EndsWith("Report"))
            {
                viewName = viewName[..^6];
            }

            var branchList = await _siteService.GetAllBranches(true);
            var groupInfoList = await _userService.GetGroupInfosAsync();
            var programList = await _siteService.GetProgramList();
            var schoolDistrictList = await _schoolService.GetDistrictsAsync();
            var schoolList = await _schoolService
                .GetSchoolsAsync(schoolDistrictList.FirstOrDefault()?.Id);
            var site = await GetCurrentSiteAsync();
            var systemList = await _siteService.GetSystemList(true);
            var vendorCodeTypeList = await _vendorCodeService.GetTypeAllAsync();

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

        [HttpGet]
        public async Task<FileStreamResult> Download(int id)
        {
            var (request, criterion) = await _reportService.GetReportResultsAsync(id);

            PageTitle = request.Name ?? "Report Results";

            var viewModel = new ReportResultsViewModel
            {
                Title = PageTitle
            };

            var criteriaDictionary = new Dictionary<string, object>();

            if (criterion.StartDate.HasValue)
            {
                criteriaDictionary.Add("Start Date",
                    criterion.StartDate.Value.ToString(CultureInfo.CurrentCulture));
            }
            if (criterion.EndDate.HasValue)
            {
                criteriaDictionary.Add("End Date",
                    criterion.EndDate.Value.ToString(CultureInfo.CurrentCulture));
            }
            if (criterion.SystemId.HasValue)
            {
                criteriaDictionary.Add("System", (await _siteService
                    .GetSystemByIdAsync(criterion.SystemId.Value)).Name);
            }
            if (criterion.BranchId.HasValue)
            {
                criteriaDictionary.Add("Branch", await _siteService
                    .GetBranchName(criterion.BranchId.Value));
            }
            if (criterion.ProgramId.HasValue)
            {
                criteriaDictionary.Add("Program", (await _siteService
                    .GetProgramByIdAsync(criterion.ProgramId.Value)).Name);
            }
            if (criterion.GroupInfoId.HasValue)
            {
                criteriaDictionary.Add("Group", (await _userService
                    .GetGroupInfoByIdAsync(criterion.GroupInfoId.Value)).Name);
            }
            if (criterion.SchoolDistrictId.HasValue)
            {
                criteriaDictionary.Add("School District", (await _schoolService
                    .GetDistrictByIdAsync(criterion.SchoolDistrictId.Value)).Name);
            }
            if (criterion.SchoolId.HasValue)
            {
                criteriaDictionary.Add("Program", (await _schoolService
                    .GetByIdAsync(criterion.SchoolId.Value)).Name);
            }
            if (criterion.VendorCodeTypeId.HasValue)
            {
                criteriaDictionary.Add("Program", (await _vendorCodeService
                    .GetTypeById(criterion.VendorCodeTypeId.Value)).Description);
            }

            if (!string.IsNullOrEmpty(criterion.BadgeRequiredList))
            {
                var badgeIds = criterion.BadgeRequiredList.Split(',')?.Select(int.Parse);
                var badgeNames = await _badgeService.GetNamesAsync(badgeIds);
                if (badgeNames.Count() == 1)
                {
                    criteriaDictionary.Add("Badge", badgeNames.First());
                }
                else
                {
                    int count = 1;
                    foreach (var name in badgeNames)
                    {
                        criteriaDictionary.Add($"Badge {count++}", name);
                    }
                }
            }

            if (!string.IsNullOrEmpty(criterion.ChallengeRequiredList))
            {
                var challengeIds = criterion.ChallengeRequiredList.Split(',')?.Select(int.Parse);
                var challengeNames = await _challengeService.GetNamesAsync(challengeIds);
                if (challengeNames.Count() == 1)
                {
                    criteriaDictionary.Add("Challenge", challengeNames.First());
                }
                else
                {
                    int count = 1;
                    foreach (var name in challengeNames)
                    {
                        criteriaDictionary.Add($"Challenge {count++}", name);
                    }
                }
            }

            if (!string.IsNullOrEmpty(criterion.TriggerList))
            {
                var triggerIds = criterion.TriggerList.Split(',')?.Select(int.Parse);
                var triggerNames = await _triggerService.GetNamesAsync(triggerIds);
                if (triggerNames.Count() == 1)
                {
                    criteriaDictionary.Add("Trigger", triggerNames.First());
                }
                else
                {
                    int count = 1;
                    foreach (var name in triggerNames)
                    {
                        criteriaDictionary.Add($"Trigger {count++}", name);
                    }
                }
            }

            viewModel.ReportSet = JsonSerializer.Deserialize<StoredReportSet>(request.ResultJson);

            if (viewModel.ReportSet.Reports?.FirstOrDefault()?.AsOf != null)
            {
                criteriaDictionary.Add("Report Run At",
                    viewModel.ReportSet.Reports.First().AsOf.ToString(CultureInfo.CurrentCulture));
            }

            var ms = ExcelExport.GenerateWorkbook(viewModel.ReportSet.Reports,
                criteriaDictionary,
                "Report Criteria");

            return new FileStreamResult(ms, ExcelExport.ExcelMimeType)
            {
                FileDownloadName = $"{PageTitle}.{ExcelExport.ExcelFileExtension}"
            };
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

        [HttpPost]
        public async Task<IActionResult> Run(ReportCriteriaViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            try
            {
                _reportService.GetReportDetails(viewModel.ReportId);
            }
            catch (GraException gex)
            {
                ShowAlertDanger(gex.Message);
                return RedirectToAction(nameof(Index));
            }

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

            var serializedParameters = new JobDetailsRunReport
            {
                ReportRequestId = reportRequestId
            };

            serializedParameters.Properties.Add(JobDetailsPropertyName.PackingSlipLink,
                Url.Action(nameof(VendorCodesController.ViewPackingSlip),
                        VendorCodesController.Name,
                        new { area = nameof(MissionControl), id = 0 }).TrimEnd('0'));

            serializedParameters.Properties.Add(JobDetailsPropertyName.ProfileLink,
                Url.Action(nameof(ParticipantsController.Detail),
                        ParticipantsController.Name,
                        new { area = nameof(MissionControl), id = 0 }).TrimEnd('0'));

            serializedParameters.Properties.Add(JobDetailsPropertyName.VendorCodeLink,
                Url.Action(nameof(ParticipantsController.VendorCodes),
                        ParticipantsController.Name,
                        new { area = nameof(MissionControl), id = 0 }).TrimEnd('0'));

            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.RunReport,
                SerializedParameters = JsonSerializer.Serialize(serializedParameters)
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
                    ReportResultId = id,
                    Title = PageTitle,
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

                if (!string.IsNullOrEmpty(request.ResultJson))
                {
                    viewModel.ReportSet = JsonSerializer
                        .Deserialize<StoredReportSet>(request.ResultJson);

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
                }

                var message = new StringBuilder();

                if (request.Success == true)
                {
                    message.Append("This report ran successfully");
                }
                if (request.Success == false)
                {
                    message.Append("This report did not run successfully");
                }
                if (string.IsNullOrEmpty(request.Message))
                {
                    message.Append('.');
                }
                else
                {
                    if (message.Length > 0)
                    {
                        message.Append(": ");
                    }
                    message.Append("<strong>").Append(request.Message).Append("</strong>");
                }

                viewModel.Message = message.ToString();

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertDanger(gex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        private static string FormatDataItem(object dataItem)
        {
            return dataItem switch
            {
                int i => i.ToString("N0", CultureInfo.InvariantCulture),
                long l => l.ToString("N0", CultureInfo.InvariantCulture),
                null => string.Empty,
                _ => WebUtility.HtmlEncode(dataItem.ToString()),
            };
        }
    }
}
