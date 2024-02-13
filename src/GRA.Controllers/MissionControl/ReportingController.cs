using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Reporting;
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
    [Authorize(Policy = Policy.ViewAllReporting)]
    public class ReportingController : Base.MCController
    {
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
            JobService jobService,
            ReportService reportService,
            SchoolService schoolService,
            SiteService siteService,
            TriggerService triggerService,
            UserService userService,
            VendorCodeService vendorCodeService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(jobService);
            ArgumentNullException.ThrowIfNull(reportService);
            ArgumentNullException.ThrowIfNull(schoolService);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(triggerService);
            ArgumentNullException.ThrowIfNull(userService);
            ArgumentNullException.ThrowIfNull(vendorCodeService);

            _logger = logger;
            _jobService = jobService;
            _reportService = reportService;
            _schoolService = schoolService;
            _siteService = siteService;
            _triggerService = triggerService;
            _userService = userService;
            _vendorCodeService = vendorCodeService;
            PageTitle = "Reporting";
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

            string viewName = report.Name.Replace(" ",
                string.Empty,
                StringComparison.OrdinalIgnoreCase);
            if (viewName.EndsWith("Report"))
            {
                viewName = viewName[..^6];
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

        [HttpGet]
        public async Task<FileStreamResult> Download(int id)
        {
            var (request, criterion) = await _reportService.GetReportResultsAsync(id);

            PageTitle = request.Name ?? "Report Results";

            var viewModel = new ReportResultsViewModel
            {
                Title = PageTitle
            };

            var criteriaDictionnary = new Dictionary<string, object>();

            if (criterion.StartDate.HasValue)
            {
                criteriaDictionnary.Add("Start Date",
                    criterion.StartDate.Value.ToString(CultureInfo.CurrentCulture));
            }
            if (criterion.EndDate.HasValue)
            {
                criteriaDictionnary.Add("End Date",
                    criterion.EndDate.Value.ToString(CultureInfo.CurrentCulture));
            }
            if (criterion.SystemId.HasValue)
            {
                criteriaDictionnary.Add("System", (await _siteService
                    .GetSystemByIdAsync(criterion.SystemId.Value)).Name);
            }
            if (criterion.BranchId.HasValue)
            {
                criteriaDictionnary.Add("Branch", await _siteService
                    .GetBranchName(criterion.BranchId.Value));
            }
            if (criterion.ProgramId.HasValue)
            {
                criteriaDictionnary.Add("Program", (await _siteService
                    .GetProgramByIdAsync(criterion.ProgramId.Value)).Name);
            }
            if (criterion.GroupInfoId.HasValue)
            {
                criteriaDictionnary.Add("Group", (await _userService
                    .GetGroupInfoByIdAsync(criterion.GroupInfoId.Value)).Name);
            }
            if (criterion.SchoolDistrictId.HasValue)
            {
                criteriaDictionnary.Add("School District", (await _schoolService
                    .GetDistrictByIdAsync(criterion.SchoolDistrictId.Value)).Name);
            }
            if (criterion.SchoolId.HasValue)
            {
                criteriaDictionnary.Add("Program", (await _schoolService
                    .GetByIdAsync(criterion.SchoolId.Value)).Name);
            }
            if (criterion.VendorCodeTypeId.HasValue)
            {
                criteriaDictionnary.Add("Program", (await _vendorCodeService
                    .GetTypeById(criterion.VendorCodeTypeId.Value)).Description);
            }

            viewModel.ReportSet = JsonConvert
                .DeserializeObject<StoredReportSet>(request.ResultJson);

            if (viewModel.ReportSet.Reports?.FirstOrDefault()?.AsOf != null)
            {
                criteriaDictionnary.Add("Report Run At",
                    viewModel.ReportSet.Reports.First().AsOf.ToString(CultureInfo.CurrentCulture));
            }

            var ms = ExcelExport.GenerateWorkbook(viewModel.ReportSet.Reports,
                criteriaDictionnary,
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
