using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.PerformerManagement;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
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
    [Authorize(Policy = Policy.ManagePerformers)]
    public class PerformerManagementController : Base.MCController
    {
        private const int KitsPerPage = 15;

        private const int MaxUploadMB = 25;

        private const int MBSize = 1024 * 1024;

        private const int PerformersPerPage = 15;

        private const string SelectionPageTitle = "{0} ({1} selections)";
        private static readonly char[] CommaSpaceSeparator = new[] { ',', ' ' };
        private static readonly DateTime DefaultPerformerScheduleEndTime = DateTime.Parse("8:00 PM");

        private static readonly DateTime DefaultPerformerScheduleStartTime = DateTime.Parse("8:00 AM");

        private readonly ILogger<PerformerManagementController> _logger;

        private readonly PerformerSchedulingService _performerSchedulingService;

        private readonly SiteService _siteService;

        public PerformerManagementController(ILogger<PerformerManagementController> logger,
            ServiceFacade.Controller context,
            PerformerSchedulingService performerSchedulingService,
            SiteService siteService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(performerSchedulingService);
            ArgumentNullException.ThrowIfNull(siteService);

            _logger = logger;
            _performerSchedulingService = performerSchedulingService;
            _siteService = siteService;

            PageTitle = "Performer Management";
        }

        public static string Name
        { get { return "PerformerManagement"; } }

        public async Task<JsonResult> GetPerformerStatus()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            var result = await GetPerformerStatus(schedulingStage);
            return Json(result);
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var completion = new Dictionary<int, string>();
            var percent = new Dictionary<int, string>();
            var card = new Dictionary<int, string>();

            var totalBranchCount = 0;
            var totalSelectedCount = 0;

            var systems = await _performerSchedulingService
                .GetSystemListWithoutExcludedBranchesAsync();

            foreach (var system in systems)
            {
                int isSelectedCount = 0;

                foreach (var branch in system.Branches)
                {
                    branch.Selections = await _performerSchedulingService
                        .GetSelectionsByBranchIdAsync(branch.Id);

                    if (branch.Selections.Count >= settings.SelectionsPerBranch)
                    {
                        isSelectedCount++;
                    }
                }

                var branchPercent = isSelectedCount >= system.Branches.Count
                    ? 100
                    : isSelectedCount * 100 / system.Branches.Count;

                totalBranchCount += system.Branches.Count;
                totalSelectedCount += isSelectedCount;
                percent.Add(system.Id, $"{branchPercent}%");
                completion.Add(system.Id, $"({isSelectedCount}/{system.Branches.Count})");

                if (branchPercent < 50)
                {
                    card.Add(system.Id, "danger");
                }
                else if (branchPercent < 100)
                {
                    card.Add(system.Id, "warning");
                }
                else
                {
                    card.Add(system.Id, "success");
                }
            }

            int summaryPercent = totalSelectedCount >= totalBranchCount
                ? 100
                : totalSelectedCount * 100 / totalBranchCount;

            var performerStatus = await GetPerformerStatus(schedulingStage);

            var viewModel = new StatusViewModel
            {
                AgeGroups = await _performerSchedulingService.GetAgeGroupsAsync(),
                Completion = completion,
                KitCount = performerStatus.KitCount,
                Now = _dateTimeProvider.Now,
                Panel = card,
                Percent = percent,
                PerformerCount = performerStatus.PerformerCount,
                PerformerSchedulingEnabled = true,
                ProgramCount = performerStatus.ProgramCount,
                SchedulingStage = performerStatus.SchedulingStage,
                Settings = settings,
                SummaryPercent = $"{summaryPercent}%",
                Systems = systems
            };

            return View(viewModel);
        }

        private async Task<PerformerStatus> GetPerformerStatus(PsSchedulingStage schedulingStage)
        {
            return new PerformerStatus
            {
                ProgramCount = await _performerSchedulingService.GetProgramCountAsync(),
                PerformerCount = await _performerSchedulingService.GetPerformerCountAsync(),
                KitCount = await _performerSchedulingService.GetKitCountAsync(),
                SchedulingStage = Regex.Replace(schedulingStage.ToString(), "(\\B[A-Z])", " $1")
            };
        }

        private class PerformerStatus
        {
            public int KitCount { get; set; }
            public int PerformerCount { get; set; }
            public int ProgramCount { get; set; }
            public string SchedulingStage { get; set; }
        }

        #region ScheduleExport

        [HttpGet]
        public async Task<IActionResult> ScheduleExport()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();

            var storedReport = new StoredReport("Scheduled Programs", _dateTimeProvider.Now)
            {
                HeaderRow = new[]
                {
                    "Performer Name",
                    "Program Title",
                    "System",
                    "Branch",
                    "Branch Address",
                    "Branch Telephone",
                    "Start Time",
                    "Scheduled By",
                    "Scheduled By Email",
                    "Scheduled By Phone",
                    "On-site Contact Name",
                    "On-site Contact Email",
                    "On-site Contact Phone",
                    "Back-to-back Program"
                }
            };

            var stage = _performerSchedulingService.GetSchedulingStage(settings);

            var criteriaDictionary = new Dictionary<string, object>
            {
                { "As Of", storedReport.AsOf.ToString(CultureInfo.CurrentCulture) },
                { "Scheduling Stage", stage.ToString() }
            };

            var systems = await _performerSchedulingService
                .GetSystemListWithoutExcludedBranchesAsync();

            var selections = new List<PsBranchSelection>();

            foreach (var system in systems)
            {
                foreach (var branch in system.Branches)
                {
                    selections.AddRange(await _performerSchedulingService
                        .GetSelectionsByBranchIdAsync(branch.Id));
                }
            }

            storedReport.Data = selections
                .Where(_ => _.Program != null)
                .OrderBy(_ => _.Program?.PerformerName)
                .ThenBy(_ => _.Program?.Title)
                .ThenBy(_ => _.Branch.Name)
                .Select(_ => new object[]
                {
                    _.Program?.PerformerName ?? "Unknown",
                    _.Program?.Title ?? "Unknown",
                    _.Branch?.SystemName ?? "Unknown",
                    _.Branch?.Name ?? "Unknown",
                    _.Branch?.Address ?? string.Empty,
                    _.Branch?.Telephone ?? string.Empty,
                    _.ScheduleStartTime.ToString(CultureInfo.CurrentCulture),
                    _.CreatedByUser.FullName ?? string.Empty,
                    _.CreatedByUser.Email ?? string.Empty,
                    _.CreatedByUser.PhoneNumber ?? string.Empty,
                    _.OnSiteContactName ?? string.Empty,
                    _.OnSiteContactEmail ?? string.Empty,
                    _.OnSiteContactPhone ?? string.Empty,
                    _.BackToBackProgram ? "X" : string.Empty
                    });

            var memoryStream = ExcelExport.GenerateWorkbook(new[] { storedReport },
                criteriaDictionary,
                "As Of");

            var scheduleName = $"{storedReport.AsOf:yyyyMMdd}-PerformerSchedule";

            return new FileStreamResult(memoryStream, ExcelExport.ExcelMimeType)
            {
                FileDownloadName = $"{scheduleName}.{ExcelExport.ExcelFileExtension}"
            };
        }

        #endregion ScheduleExport

        #region PerformerExport

        [HttpGet]
        public async Task<IActionResult> PerformerExport()
        {
            var performerList = await _performerSchedulingService.GetAllPerformersAsync();

            var report = new StoredReport("Email Award Addresses", _dateTimeProvider.Now)
            {
                Data = performerList
                .Where(_ => _.IsApproved)
                .OrderBy(_ => _.Name)
                .Select(_ => new object[]
                {
                        _.Name,
                        _.VendorId,
                        _.BillingAddress,
                        _.Email,
                        _.Phone
                }),
                HeaderRow = new[] { "Name", "VendorID", "Address", "Email", "Phone" }
            };

            var fileName = $"{report.AsOf:yyyyMMdd}-PerformerList";

            var ms = ExcelExport.GenerateWorkbook(new List<StoredReport> { report });

            return new FileStreamResult(ms, ExcelExport.ExcelMimeType)
            {
                FileDownloadName = $"{fileName}.{ExcelExport.ExcelFileExtension}"
            };
        }

        #endregion PerformerExport

        #region Performers

        public async Task<IActionResult> Performer(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    includeBranches: true,
                    includeImages: true,
                    includePrograms: true,
                    includeSchedule: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view performer: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            if (string.IsNullOrEmpty(settings.VendorIdPrompt))
            {
                settings.VendorIdPrompt = "Vendor ID";
            }

            var viewModel = new PerformerViewModel
            {
                Approve = !performer.IsApproved,
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                BranchAvailability = performer.Branches.Select(_ => _.Id).ToList(),
                Performer = performer,
                SchedulingStage = schedulingStage,
                Settings = settings,
                Systems = await _performerSchedulingService
                    .GetSystemListWithoutExcludedBranchesAsync(),
                EnablePerformerInsuranceQuestion = await
                    GetSiteSettingBoolAsync(SiteSettingKey.Performer.EnableInsuranceQuestion)
            };

            if (performer.Images.Count > 0)
            {
                viewModel.ImagePath = _pathResolver.ResolveContentPath(
                    performer.Images[0].Filename);
            }

            if (!string.IsNullOrWhiteSpace(performer.Website)
                && Uri.TryCreate(performer.Website, UriKind.Absolute, out Uri absoluteUri))
            {
                viewModel.Uri = absoluteUri;
            }

            var performerIndexList = await _performerSchedulingService.GetPerformerIndexListAsync();
            var index = performerIndexList.IndexOf(id);
            viewModel.ReturnPage = (index / PerformersPerPage) + 1;
            if (index != 0)
            {
                viewModel.PrevPerformer = performerIndexList[index - 1];
            }
            if (performerIndexList.Count != index + 1)
            {
                viewModel.NextPerformer = performerIndexList[index + 1];
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerApprove(PerformerViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }
            else if (schedulingStage == PsSchedulingStage.RegistrationClosed)
            {
                try
                {
                    await _performerSchedulingService.SetPerformerApprovedAsync(model.Performer.Id,
                        model.Approve);
                    ShowAlertSuccess($"Performer {(model.Approve ? "Approved" : "Unapproved")}!");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger($"Unable to {(model.Approve ? "Approve" : "Unapprove")} performer: ", gex);
                    return RedirectToAction(nameof(Performers));
                }
            }

            return RedirectToAction(nameof(Performer), new { model.Performer.Id });
        }

        [HttpPost]
        public async Task<IActionResult> PerformerDelete(PerformerListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                try
                {
                    await _performerSchedulingService.RemovePerformerAsync(
                        model.PerformerToDelete.Id);
                    ShowAlertSuccess($"Performer \"{model.PerformerToDelete.Name}\" removed!.");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to remove performer: ", gex);
                }
            }
            else
            {
                ShowAlertDanger("Cannot remove performers after scheduling has opened.");
            }

            return RedirectToAction(nameof(Performers),
                new { page = model.PaginateModel.CurrentPage });
        }

        public async Task<IActionResult> PerformerDetails(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    includeBranches: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit performer: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var systems = await _performerSchedulingService
                .GetSystemListWithoutExcludedBranchesAsync();

            var viewModel = new PerformerDetailsViewModel
            {
                Performer = performer,
                Systems = systems,
                BranchCount = systems.Sum(_ => _.Branches.Count),
                VendorCodeFormat = settings.VendorCodeFormat ?? "Unspecified",
                VendorIdPrompt = settings.VendorIdPrompt ?? "Vendor ID",
                EnablePerformerInsuranceQuestion = await
                    GetSiteSettingBoolAsync(SiteSettingKey.Performer.EnableInsuranceQuestion)
            };

            if (performer.AllBranches)
            {
                viewModel.BranchAvailability = systems
                    .SelectMany(_ => _.Branches)
                    .Select(_ => _.Id)
                    .ToList();
            }
            else
            {
                viewModel.BranchAvailability = performer.Branches?.Select(_ => _.Id).ToList();
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerDetails(PerformerDetailsViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var systems = await _performerSchedulingService
                .GetSystemListWithoutExcludedBranchesAsync();
            var branchIds = systems.SelectMany(_ => _.Branches).Select(_ => _.Id);
            var branchAvailability = JsonConvert
                .DeserializeObject<List<int>>(model.BranchAvailabilityString)
                .Where(_ => branchIds.Contains(_))
                .ToList();

            if (branchAvailability.Count == 0)
            {
                ModelState.AddModelError("BranchAvailability",
                    "Please select the libraries where they are willing to perform.");
            }

            if (ModelState.IsValid)
            {
                model.Performer.AllBranches = branchAvailability.Count == branchIds.Count();

                var performer = await _performerSchedulingService.EditPerformerAsync(
                    model.Performer, branchAvailability);

                ShowAlertSuccess($"Performer {performer.Name} updated!");
                return RedirectToAction(nameof(Performer), new { id = performer.Id });
            }

            model.BranchCount = systems.Sum(_ => _.Branches.Count);
            model.BranchAvailability = branchAvailability;
            model.Systems = systems;

            return View(model);
        }

        public async Task<IActionResult> PerformerImages(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view performer images: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var viewModel = new PerformerImagesViewModel
            {
                PerformerId = performer.Id,
                PerformerName = performer.Name,
                PerformerImages = performer.Images.ToList(),
                MaxUploadMB = MaxUploadMB
            };
            viewModel.PerformerImages.ForEach(_ => _.Filename =
                _pathResolver.ResolveContentPath(_.Filename));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerImages(PerformerImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(
                    model.PerformerId, includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add performer images: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            if (model.Images == null)
            {
                ModelState.AddModelError("Images", "Please attach an image to submit.");
            }
            else if (model.Images.Count > 0)
            {
                var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                if (extensions.Any(_ => !ValidImageExtensions.Contains(_)))
                {
                    ModelState.AddModelError("Images",
                        $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                }
                else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                {
                    ModelState.AddModelError("Images",
                        $"Please limit uploads to a max of {MaxUploadMB}MB.");
                }
            }
            if (ModelState.IsValid)
            {
                foreach (var image in model.Images)
                {
                    using var fileStream = image.OpenReadStream();
                    using var ms = new MemoryStream();
                    fileStream.CopyTo(ms);
                    await _performerSchedulingService.AddPerformerImageAsync(
                        model.PerformerId, ms.ToArray(), Path.GetExtension(image.FileName));
                }
                ShowAlertSuccess("Image(s) added!");
                return RedirectToAction(nameof(PerformerImages));
            }

            var performerImages = performer.Images.ToList();
            performerImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            model.MaxUploadMB = MaxUploadMB;
            model.PerformerName = performer.Name;
            model.PerformerImages = performerImages;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerImagesDelete(PerformerImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var imageIds = Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<int>>(model.ImagesToDelete);
            if (imageIds.Count > 0)
            {
                foreach (var imageId in imageIds)
                {
                    await _performerSchedulingService.RemovePerformerImageByIdAsync(imageId);
                }
                ShowAlertSuccess("Image(s) deleted!");
            }

            return RedirectToAction(nameof(PerformerImages), new { id = model.PerformerId });
        }

        public async Task<IActionResult> Performers(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var filter = new PerformerSchedulingFilter(page, PerformersPerPage);

            var performerList = await _performerSchedulingService
                .GetPaginatedPerformerListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = performerList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }
            foreach (var performer in performerList.Data)
            {
                performer.ProgramCount = await _performerSchedulingService
                    .GetPerformerProgramCountAsync(performer.Id);

                if (schedulingStage >= PsSchedulingStage.SchedulingOpen)
                {
                    performer.SelectionsCount = await _performerSchedulingService
                        .GetPerformerSelectionCountAsync(performer.Id);
                }
            }

            var viewModel = new PerformerListViewModel
            {
                Performers = performerList.Data.ToList(),
                PaginateModel = paginateModel,
                PerformerSchedulingEnabled = true,
                RegistrationClosed = schedulingStage >= PsSchedulingStage.RegistrationClosed,
                SchedulingStage = schedulingStage,
                VendorIdPrompt = settings.VendorIdPrompt ?? "Vendor ID"
            };
            return View(viewModel);
        }

        public async Task<IActionResult> PerformerSchedule(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    includeSchedule: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to update schedule: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var viewModel = new PerformerScheduleViewModel
            {
                Performer = performer,
                StartDate = settings.ScheduleStartDate.Value,
                EndDate = settings.ScheduleEndDate.Value,
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                ScheduleDates = performer.Schedule.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerSchedule(PerformerScheduleViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var schedule = new List<PsScheduleDate>();
            try
            {
                schedule = JsonConvert.DeserializeObject<List<PsScheduleDate>>(model.JsonSchedule);

                foreach (var date in schedule)
                {
                    date.ParsedDate = DateTime.Parse($"{date.Year}-{date.Month}-{date.Date}");
                    if (Enum.TryParse(date.Availability, out PsScheduleDateStatus status))
                    {
                        date.Status = status;
                    }
                    else
                    {
                        date.Status = PsScheduleDateStatus.Available;
                    }
                }

                try
                {
                    await _performerSchedulingService.EditPerformerScheduleAsync(model.Performer.Id,
                        schedule);
                    ShowAlertSuccess($"Schedule for \"{model.Performer.Name}\" updated!");
                    return RedirectToAction(nameof(Performer), new { id = model.Performer.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Error updating schedule: ", gex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error submitting schedule for performer {Performer} by user {UserId}: {ErrorMessage}",
                    model.Performer.Id,
                    GetId(ClaimType.UserId),
                    ex.Message);
                ShowAlertDanger("An error occured while updating the schedule.");
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(
                    model.Performer.Id, includeSchedule: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to update schedule: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            model.Performer = performer;
            model.BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync();
            model.StartDate = settings.ScheduleStartDate.Value;
            model.EndDate = settings.ScheduleEndDate.Value;
            model.ScheduleDates = performer.Schedule.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerSelectionDelete
            (PerformerSelectionsViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return RedirectToAction(nameof(PerformerManagementController.PerformerSelections),
                    new { id = model.Performer.Id });
            }
            try
            {
                var branchSelection = await _performerSchedulingService
                    .GetBranchProgramSelectionByIdAsync(model.BranchSelectionId);
                await _performerSchedulingService.DeleteBranchSelectionAsync(branchSelection);
                var branch = await _siteService.GetBranchByIdAsync(branchSelection.BranchId);
                var program = await _performerSchedulingService
                    .GetProgramByIdAsync(branchSelection.ProgramId.Value);
                ShowAlertSuccess($"{branch.Name}'s selection of \"{program.Title}\" is deleted!.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete selection: ", gex);
                _logger.LogError(gex,
                    "Error deleting branch selection Id {BranchSelectionId} by user {GetActiveUserId}: {ErrorMessage}",
                    model.BranchSelectionId, GetActiveUserId(), gex.Message);
            }
            return RedirectToAction(nameof(PerformerManagementController.PerformerSelections),
                new { id = model.Performer.Id });
        }

        public async Task<IActionResult> PrepCoverSheet(int id)
        {
            PsPerformer performer;
            try
            {
                performer = await _performerSchedulingService
                    .GetPerformerByIdAsync(id, false, false, true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view performer: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var selections = await _performerSchedulingService
                .GetBranchProgramSelectionsByPerformerAsync(performer.Id);

            if (selections.Count == 0)
            {
                ShowAlertInfo("No performances found for selected performer.");
                return RedirectToAction(nameof(Performers));
            }

            var months = selections
                .OrderBy(s => s.ScheduleStartTime.Month)
                .GroupBy(s => s.ScheduleStartTime.Month).Select(_ => new SelectListItem
                {
                    Value = _.First().ScheduleStartTime.Month.ToString(CultureInfo.CurrentCulture),
                    Text = _.First().ScheduleStartTime.ToString("MMMM", CultureInfo.CurrentCulture)
                });

            var monthSelection = new SelectList(months, "Value", "Text");

            var viewModel = new PerformerCoversheetViewModel
            {
                Months = monthSelection,
                PerformerId = performer.Id,
                PerformerName = performer.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PrintCoverSheet(int id, int month, string invoiceNumber)
        {
            PsPerformer performer;
            try
            {
                performer = await _performerSchedulingService
                    .GetPerformerByIdAsync(id, false, false, true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view performer: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var allSelections = await _performerSchedulingService
                .GetBranchProgramSelectionsByPerformerAsync(performer.Id);

            var selections = allSelections
                .Where(selection => selection.ScheduleStartTime.Month == month);

            if (!selections.Any())
            {
                ShowAlertInfo("No performances found for selected performer and month.");
                return RedirectToAction(nameof(Performers));
            }

            decimal costSum = 0;
            var description = new StringBuilder();

            foreach (var selection in selections)
            {
                var program = performer.Programs.FirstOrDefault(_ => _.Id == selection.ProgramId);
                var actualCost = selection.BackToBackProgram ? program.Cost * 2 : program.Cost;
                costSum += actualCost;
                description.Append(program.Title)
                    .Append(": ")
                    .Append(selection.ScheduleStartTime.ToShortDateString())
                    .Append(' ')
                    .Append(actualCost.ToString("C", CultureInfo.CurrentCulture));

                if (selection.BackToBackProgram)
                {
                    description.Append(" (back-to-back program)");
                }
                description.AppendLine();
            }

            var settings = await _performerSchedulingService.GetSettingsAsync();

            var viewModel = new PerformerCoversheetViewModel
            {
                Description = description.ToString(),
                Cost = costSum,
                ProgramDate = selections.FirstOrDefault().ScheduleStartTime,
                VendorId = performer.VendorId,
                PayToName = performer.Name,
                PayToAddress = performer.BillingAddress,
                InvoiceNumber = invoiceNumber,
                LibraryBranch = settings.CoverSheetBranch,
                StaffContact = settings.CoverSheetContact
            };

            PageTitle = "Coversheet - "
                + performer.Name
                + " - "
                + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)
                + " "
                + DateTime.Today.Year.ToString(CultureInfo.CurrentCulture);

            return View(viewModel);
        }

        public async Task<IActionResult> Program(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(id,
                    includeAgeGroups: true,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var viewModel = new ProgramViewModel
            {
                Approve = !program.IsApproved,
                Program = program,
                SchedulingStage = schedulingStage,
                EnablePerformerLivestreamQuestions = await
                    GetSiteSettingBoolAsync(SiteSettingKey.Performer.EnableLivestreamQuestions)
            };

            if (program.Images?.Count > 0)
            {
                viewModel.Image = _pathResolver.ResolveContentPath(program.Images[0].Filename);
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ProgramAdd(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add program: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageList = new SelectList(ageGroups, "Id", "Name");
            if (ageList.Count() == 1)
            {
                ageList.First().Selected = true;
            }

            var viewModel = new ProgramDetailsViewModel
            {
                AddPerformerId = id,
                AgeList = new SelectList(ageGroups, "Id", "Name"),
                BackToBackSelection = new SelectList(new[] { GRA.Defaults.BackToBackInterval }),
                MaxUploadMB = MaxUploadMB,
                PerformerId = id,
                PerformerName = performer.Name,
                SetupSupplementalText = settings.SetupSupplementalText
            };

            var (hasIntervalString, intervalString)
                = await GetSiteSettingStringAsync(SiteSettingKey.Performer.BackToBackInterval);

            if (hasIntervalString)
            {
                var intervalOptions = intervalString.Split(CommaSpaceSeparator,
                    StringSplitOptions.RemoveEmptyEntries);

                viewModel.BackToBackSelection = new SelectList(intervalOptions);
            }

            return View(nameof(ProgramDetails), viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramApprove(ProgramViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }
            else if (schedulingStage <= PsSchedulingStage.RegistrationClosed)
            {
                try
                {
                    await _performerSchedulingService.SetProgramApprovedAsync(model.Program.Id,
                        model.Approve);
                    ShowAlertSuccess($"Program {(model.Approve ? "Approved" : "Unapproved")}!");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger($"Unable to {(model.Approve ? "Approve" : "Unapprove")} program: ", 
                        gex);
                    return RedirectToAction(nameof(Performers));
                }
            }

            return RedirectToAction(nameof(Performer), new { id = model.Program.PerformerId });
        }

        [HttpPost]
        public async Task<IActionResult> ProgramDelete(PerformerViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            try
            {
                await _performerSchedulingService.RemoveProgramAsync(model.ProgramToDelete.Id);
                ShowAlertSuccess($"Program \"{model.ProgramToDelete.Title}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove program: ", gex);
            }

            return RedirectToAction(nameof(Performer), new { id = model.Performer.Id });
        }

        public async Task<IActionResult> ProgramDetails(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(id,
                    includeAgeGroups: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit program: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();

            var viewModel = new ProgramDetailsViewModel
            {
                AddPerformerId = id,
                AgeList = new SelectList(ageGroups, "Id", "Name"),
                AgeSelection = program.AgeGroups.Select(_ => _.Id).ToList(),
                BackToBackSelection = new SelectList(new[] { GRA.Defaults.BackToBackInterval }),
                Program = program,
                SetupSupplementalText = settings.SetupSupplementalText
            };

            var (hasIntervalString, intervalString)
                = await GetSiteSettingStringAsync(SiteSettingKey.Performer.BackToBackInterval);

            if (hasIntervalString)
            {
                var intervalOptions = intervalString.Split(CommaSpaceSeparator,
                    StringSplitOptions.RemoveEmptyEntries);

                viewModel.BackToBackSelection = new SelectList(intervalOptions);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramDetails(ProgramDetailsViewModel model)
        {
            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageSelection = ageGroups
                .Where(_ => model.AgeSelection?.Contains(_.Id) == true)
                .Select(_ => _.Id)
                .ToList();
            if (ageSelection.Count == 0)
            {
                ModelState.AddModelError("AgeSelection", "Please select age groups.");
            }

            if (model.Images?.Count > 0 && model.PerformerId.HasValue)
            {
                var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                if (extensions.Any(_ => !ValidImageExtensions.Contains(_)))
                {
                    ModelState.AddModelError("Images",
                        $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                }
                else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                {
                    ModelState.AddModelError("Images",
                        $"Please limit uploads to a max of {MaxUploadMB}MB.");
                }
            }

            if (ModelState.IsValid)
            {
                var program = new PsProgram();
                try
                {
                    if (model.PerformerId.HasValue)
                    {
                        model.Program.PerformerId = model.PerformerId.Value;
                        program = await _performerSchedulingService.AddProgramAsync(model.Program,
                            ageSelection);
                        if (model.Images != null)
                        {
                            foreach (var image in model.Images)
                            {
                                using var fileStream = image.OpenReadStream();
                                using var ms = new MemoryStream();
                                fileStream.CopyTo(ms);
                                await _performerSchedulingService.AddProgramImageAsync(
                                    program.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                            }
                        }
                        ShowAlertSuccess($"Program \"{program.Title}\" added!");
                    }
                    else
                    {
                        program = await _performerSchedulingService.UpdateProgramAsync(
                            model.Program, ageSelection);

                        ShowAlertSuccess($"Program {program.Title} updated!");
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit program: ", gex);
                    return RedirectToAction(nameof(Performers));
                }
                return RedirectToAction(nameof(Performer), new { id = program.PerformerId });
            }

            model.AgeList = new SelectList(ageGroups, "Id", "Name");
            model.MaxUploadMB = MaxUploadMB;
            if (model.PerformerId.HasValue)
            {
                var performer = await _performerSchedulingService
                        .GetPerformerByIdAsync(model.PerformerId.Value);
                model.PerformerName = performer.Name;
            }

            var settings = await _performerSchedulingService.GetSettingsAsync();
            model.SetupSupplementalText = settings.SetupSupplementalText;

            return View(model);
        }

        public async Task<IActionResult> ProgramImages(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(id,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program images: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var viewModel = new ProgramImagesViewModel
            {
                ProgramId = program.Id,
                ProgramName = program.Title,
                ProgramImages = program.Images.ToList(),
                MaxUploadMB = MaxUploadMB
            };
            viewModel.ProgramImages.ForEach(_ => _.Filename = _pathResolver
                .ResolveContentPath(_.Filename));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramImages(ProgramImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            if (model.Images == null)
            {
                ModelState.AddModelError("Images", "Please attach an image to submit.");
            }
            else if (model.Images.Count > 0)
            {
                var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                if (extensions.Any(_ => !ValidImageExtensions.Contains(_)))
                {
                    ModelState.AddModelError("Images",
                        $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                }
                else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                {
                    ModelState.AddModelError("Images",
                        $"Please limit uploads to a max of {MaxUploadMB}MB.");
                }
            }
            if (ModelState.IsValid)
            {
                foreach (var image in model.Images)
                {
                    using var fileStream = image.OpenReadStream();
                    using var ms = new MemoryStream();
                    fileStream.CopyTo(ms);
                    await _performerSchedulingService.AddProgramImageAsync(
                        model.ProgramId, ms.ToArray(), Path.GetExtension(image.FileName));
                }
                ShowAlertSuccess("Image(s) added!");
                return RedirectToAction(nameof(ProgramImages), new { id = model.ProgramId });
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(model.ProgramId,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program images: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            model.ProgramName = program.Title;
            model.ProgramImages = program.Images.ToList();
            model.ProgramImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(
                _.Filename));
            model.MaxUploadMB = MaxUploadMB;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramImagesDelete(ProgramImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var imageIds = Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<int>>(model.ImagesToDelete);
            if (imageIds.Count > 0)
            {
                foreach (var imageId in imageIds)
                {
                    await _performerSchedulingService.RemoveProgramImageByIdAsync(imageId);
                }
                ShowAlertSuccess("Image(s) deleted!");
            }

            return RedirectToAction(nameof(ProgramImages), new { id = model.ProgramId });
        }

        #endregion Performers

        #region Performer Selections

        public async Task<JsonResult> CheckProgramTimeAvailability(int selectionId,
            int programId,
            DateTime dateTime,
            bool backToBack)
        {
            try
            {
                var result = await _performerSchedulingService
                    .ValidateScheduleTimeAsync(programId, dateTime, backToBack, selectionId);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    return Json(new
                    {
                        success = true,
                        message = result
                    });
                }
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            return Json(new
            {
                success = true
            });
        }

        //TODO Find a way to consolidate duplicate action in PerformerSchedulingController.
        [HttpPost]
        public async Task<JsonResult> EditBranchProgramContact([FromBody] PsBranchSelection branchSelection)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return Json(new
                {
                    success = false,
                    message = "Program selection has not yet started."
                });
            }

            try
            {
                await _performerSchedulingService.UpdateBranchProgramContactAsync(branchSelection);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            _logger.LogInformation("Contact info {BranchSelectionId} edited", branchSelection.Id);
            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditBranchProgramSelection(int branchSelectionId,
            DateTime requestedStartTime)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return Json(new
                {
                    success = false,
                    message = "Program selection has not yet started."
                });
            }

            try
            {
                await _performerSchedulingService
                    .UpdateBranchProgramSelectionAsync(branchSelectionId, requestedStartTime);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            _logger.LogInformation("Selection {BranchSelectionId} edited", branchSelectionId);

            ShowAlertSuccess("Program selection edited!");
            return Json(new
            {
                success = true
            });
        }

        public async Task<IActionResult> GetPerformerCalendar(int performerId,
            int branchSelectionId)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return RedirectToAction(nameof(Settings));
            }

            var performer = await _performerSchedulingService.GetPerformerByIdAsync(performerId,
                includeSchedule: true);

            var branchSelections = await _performerSchedulingService
                .GetPerformerBranchSelectionsAsync(performer.Id);
            var branchSelection = branchSelections.Single(_ => _.Id == branchSelectionId);

            var viewModel = new PerformerCalendarViewModel
            {
                BranchSelection = branchSelection,
                Settings = settings,
                Performer = performer,
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                BookedDates = branchSelections.Select(_ => _.RequestedStartTime)
            };

            var dayScheduleViewModel = new DayScheduleViewModel
            {
                BranchSelections = branchSelections
                    .Where(_ => _.RequestedStartTime.Date
                        == branchSelection.RequestedStartTime.Date)
                    .OrderBy(_ => _.RequestedStartTime)
                    .ToList()
            };

            var performerDaySchedule = performer.Schedule
                .SingleOrDefault(_ => _.Date == branchSelection.RequestedStartTime.Date);

            var startTime = performerDaySchedule?.StartTime ?? DefaultPerformerScheduleStartTime;
            var endTime = performerDaySchedule?.EndTime ?? DefaultPerformerScheduleEndTime;

            var earliestSelection = dayScheduleViewModel.BranchSelections
                .OrderBy(_ => _.ScheduleStartTime)
                .FirstOrDefault();
            var latestSelection = dayScheduleViewModel.BranchSelections
                .OrderByDescending(_ => _.ScheduleStartTime)
                .FirstOrDefault();

            if (earliestSelection == null
                || startTime.TimeOfDay < earliestSelection.ScheduleStartTime.TimeOfDay)
            {
                dayScheduleViewModel.StartTime = startTime.ToShortTimeString();
            }
            if (latestSelection == null
                || endTime.TimeOfDay > latestSelection.ScheduleStartTime
                    .AddMinutes(latestSelection.ScheduleDuration).TimeOfDay)
            {
                dayScheduleViewModel.EndTime = endTime.ToShortTimeString();
            }

            viewModel.DayScheduleModel = dayScheduleViewModel;

            return PartialView("_PerformerCalendarPartial", viewModel);
        }

        // disable warning that "date" is named the same as a property on the object
        public async Task<IActionResult> GetPerformerDaySchedule(int performerId, DateTime date)
        {
            var performerSchedule = await _performerSchedulingService.GetPerformerDateScheduleAsync(
                performerId, date.Date);

            var branchSelections = (await _performerSchedulingService
                .GetPerformerBranchSelectionsAsync(performerId, date)).ToList();

            var viewModel = new DayScheduleViewModel
            {
                BranchSelections = branchSelections
            };
            var startTime = performerSchedule?.StartTime ?? DefaultPerformerScheduleStartTime;
            var endTime = performerSchedule?.EndTime ?? DefaultPerformerScheduleEndTime;

            var earliestSelection = branchSelections
                .OrderBy(_ => _.ScheduleStartTime)
                .FirstOrDefault();
            var latestSelection = branchSelections
                .OrderByDescending(_ => _.ScheduleStartTime)
                .FirstOrDefault();

            if (earliestSelection == null
                || startTime.TimeOfDay < earliestSelection.ScheduleStartTime.TimeOfDay)
            {
                viewModel.StartTime = startTime.ToShortTimeString();
            }
            if (latestSelection == null
                || endTime.TimeOfDay > latestSelection.ScheduleStartTime
                    .AddMinutes(latestSelection.ScheduleDuration).TimeOfDay)
            {
                viewModel.EndTime = endTime.ToShortTimeString();
            }

            return PartialView("_DaySchedulePartial", viewModel);
        }

        public async Task<IActionResult> PerformerSelections(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return RedirectToAction(nameof(Settings));
            }

            var performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                includeSchedule: true);

            var branchSelections = await _performerSchedulingService
                .GetPerformerBranchSelectionsAsync(performer.Id);

            var branchSelectionDates = branchSelections
                .GroupBy(_ => _.RequestedStartTime.Date)
                .Select(_ => _.ToList())
                .ToList();

            var viewModel = new PerformerSelectionsViewModel
            {
                Performer = performer,
                BranchSelectionDates = branchSelectionDates,
                DefaultPerformerScheduleStartTime = DefaultPerformerScheduleStartTime,
                DefaultPerformerScheduleEndTime = DefaultPerformerScheduleEndTime
            };

            var performerIndexList = await _performerSchedulingService.GetPerformerIndexListAsync(
                true);
            var index = performerIndexList.IndexOf(id);
            viewModel.ReturnPage = (index / PerformersPerPage) + 1;
            if (index != 0)
            {
                viewModel.PrevPerformer = performerIndexList[index - 1];
            }
            if (performerIndexList.Count != index + 1)
            {
                viewModel.NextPerformer = performerIndexList[index + 1];
            }

            PageTitle = string.Format(CultureInfo.InvariantCulture,
                SelectionPageTitle,
                viewModel.Performer?.Name,
                viewModel.BranchSelectionDates?.SelectMany(_ => _).Count());

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> SetSecretCode(int id, string secretCode)
        {
            var sanitizedSecretCode = secretCode?.Trim().ToLower();

            try
            {
                await _performerSchedulingService.SetSelectionSecretCodeAsync(id,
                    sanitizedSecretCode);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    secretCode = sanitizedSecretCode,
                    message = gex.Message
                });
            }

            return Json(new
            {
                success = true,
                secretCode = sanitizedSecretCode
            });
        }

        #endregion Performer Selections

        #region Kits

        [HttpPost]
        public async Task<IActionResult> DeleteKitSelection(KitSelectionsViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(PerformerManagementController.KitSelections),
                    new { id = model.Kit.Id });
            }
            try
            {
                var branchSelection = await _performerSchedulingService
                    .GetBranchProgramSelectionByIdAsync(model.BranchSelectionId);
                await _performerSchedulingService.DeleteBranchSelectionAsync(branchSelection);
                var branch = await _siteService.GetBranchByIdAsync(branchSelection.BranchId);
                var kit = await _performerSchedulingService.GetKitByIdAsync(model.Kit.Id);
                ShowAlertSuccess($"{branch.Name}'s selection of \"{kit.Name}\" is deleted!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete selection: ", gex);
                _logger.LogError(gex,
                    "Error deleting branch selection Id {BranchSelectionId} by user {GetActiveUserId}: {ErrorMessage}",
                    model.BranchSelectionId, GetActiveUserId(), gex.Message);
            }
            return RedirectToAction(nameof(PerformerManagementController.KitSelections),
                new { id = model.Kit.Id });
        }

        public async Task<IActionResult> Kit(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id, includeAgeGroups: true,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view kit: ", gex);
                return RedirectToAction(nameof(Kits));
            }

            var viewModel = new KitViewModel
            {
                Kit = kit,
                SchedulingStage = schedulingStage
            };

            if (kit.Images.Count > 0)
            {
                viewModel.ImagePath = _pathResolver.ResolveContentPath(
                    kit.Images[0].Filename);
            }

            if (!string.IsNullOrWhiteSpace(kit.Website)
                && Uri.TryCreate(kit.Website, UriKind.Absolute, out Uri absoluteUri))
            {
                viewModel.Uri = absoluteUri;
            }

            var kitIndexList = await _performerSchedulingService.GetKitIndexListAsync();
            var index = kitIndexList.IndexOf(id);
            viewModel.ReturnPage = (index / KitsPerPage) + 1;
            if (index != 0)
            {
                viewModel.PrevKit = kitIndexList[index - 1];
            }
            if (kitIndexList.Count != index + 1)
            {
                viewModel.NextKit = kitIndexList[index + 1];
            }

            return View(viewModel);
        }

        public async Task<IActionResult> KitAdd()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();

            var viewModel = new KitDetailsViewModel
            {
                AgeList = new SelectList(ageGroups, "Id", "Name"),
                MaxUploadMB = MaxUploadMB,
                NewKit = true
            };

            return View(nameof(KitDetails), viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> KitDelete(KitListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                try
                {
                    await _performerSchedulingService.RemoveKitAsync(
                        model.KitToDelete.Id);
                    ShowAlertSuccess($"Kit \"{model.KitToDelete.Name}\" removed!.");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to remove kit: ", gex);
                }
            }
            else
            {
                ShowAlertDanger("Cannot remove kit after scheduling has opened.");
            }

            return RedirectToAction(nameof(Kits), new { page = model.PaginateModel.CurrentPage });
        }

        public async Task<IActionResult> KitDetails(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id, includeAgeGroups: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to update kit: ", gex);
                return RedirectToAction(nameof(Kits));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();

            var viewModel = new KitDetailsViewModel
            {
                AgeList = new SelectList(ageGroups, "Id", "Name"),
                AgeSelection = kit.AgeGroups.Select(_ => _.Id).ToList(),
                Kit = kit
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> KitDetails(KitDetailsViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageSelection = ageGroups
                .Where(_ => model.AgeSelection?.Contains(_.Id) == true)
                .Select(_ => _.Id)
                .ToList();
            if (ageSelection.Count == 0)
            {
                ModelState.AddModelError("AgeSelection", "Please select age groups.");
            }

            if (model.NewKit)
            {
                if (model.Images == null)
                {
                    ModelState.AddModelError("Images", "Please attach an image to submit.");
                }
                else
                {
                    var extensions = model.Images
                        .Select(_ => Path.GetExtension(_.FileName).ToLower());
                    if (extensions.Any(_ => !ValidImageExtensions.Contains(_)))
                    {
                        ModelState.AddModelError("Images",
                            $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                    }
                    else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                    {
                        ModelState.AddModelError("Images",
                            $"Please limit uploads to a max of {MaxUploadMB}MB.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var kit = new PsKit();
                    if (model.NewKit)
                    {
                        kit = await _performerSchedulingService.AddKitAsync(model.Kit,
                            ageSelection);

                        foreach (var image in model.Images)
                        {
                            using var fileStream = image.OpenReadStream();
                            using var ms = new MemoryStream();
                            fileStream.CopyTo(ms);
                            await _performerSchedulingService.AddKitImageAsync(
                                kit.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                        }

                        ShowAlertSuccess($"Kit \"{kit.Name}\" added!");
                    }
                    else
                    {
                        kit = await _performerSchedulingService.UpdateKitAsync(model.Kit,
                            ageSelection);
                        ShowAlertSuccess($"Kit \"{kit.Name}\" updated!");
                    }
                    return RedirectToAction(nameof(Kit), new { id = kit.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger($"Unable to {(model.NewKit ? "add" : "update")} kit: ", gex);
                }
            }

            model.AgeList = new SelectList(ageGroups, "Id", "Name");
            if (model.NewKit)
            {
                model.MaxUploadMB = MaxUploadMB;
            }
            return View(model);
        }

        public async Task<IActionResult> KitImages(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id, includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view kit images: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var viewModel = new KitImagesViewModel
            {
                KitId = kit.Id,
                KitName = kit.Name,
                KitImages = kit.Images.ToList(),
                MaxUploadMB = MaxUploadMB
            };
            viewModel.KitImages.ForEach(_ => _.Filename =
                _pathResolver.ResolveContentPath(_.Filename));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> KitImages(KitImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(model.KitId,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add kit images: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            if (model.Images == null)
            {
                ModelState.AddModelError("Images", "Please attach an image to submit.");
            }
            else if (model.Images.Count > 0)
            {
                var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                if (extensions.Any(_ => !ValidImageExtensions.Contains(_)))
                {
                    ModelState.AddModelError("Images",
                        $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                }
                else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                {
                    ModelState.AddModelError("Images",
                        $"Please limit uploads to a max of {MaxUploadMB}MB.");
                }
            }
            if (ModelState.IsValid)
            {
                foreach (var image in model.Images)
                {
                    using var fileStream = image.OpenReadStream();
                    using var ms = new MemoryStream();
                    fileStream.CopyTo(ms);
                    await _performerSchedulingService.AddKitImageAsync(
                        kit.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                }
                ShowAlertSuccess("Image(s) added!");
                return RedirectToAction(nameof(KitImages));
            }

            var kitImages = kit.Images.ToList();
            kitImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            model.KitName = kit.Name;
            model.KitImages = kitImages;
            model.MaxUploadMB = MaxUploadMB;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> KitImagesDelete(KitImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var imageIds = Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<int>>(model.ImagesToDelete);
            if (imageIds.Count > 0)
            {
                foreach (var imageId in imageIds)
                {
                    await _performerSchedulingService.RemoveKitImageByIdAsync(imageId);
                }
                ShowAlertSuccess("Image(s) deleted!");
            }

            return RedirectToAction(nameof(KitImages), new { id = model.KitId });
        }

        public async Task<IActionResult> Kits(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Settings));
            }

            var filter = new BaseFilter(page, KitsPerPage);

            var kitList = await _performerSchedulingService.GetPaginatedKitListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = kitList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            if (schedulingStage >= PsSchedulingStage.SchedulingOpen)
            {
                foreach (var kit in kitList.Data)
                {
                    kit.SelectionsCount = await _performerSchedulingService
                        .GetKitSelectionCountAsync(kit.Id);
                }
            }

            var viewModel = new KitListViewModel
            {
                Kits = kitList.Data.ToList(),
                PaginateModel = paginateModel,
                PerformerSchedulingEnabled = true,
                SchedulingStage = schedulingStage
            };

            return View(viewModel);
        }

        #endregion Kits

        #region Kit Selections

        public async Task<JsonResult> CheckBranchAgeGroup(int ageGroupId, int branchId,
            int? currentSelectionId = null)
        {
            var alreadySelected = await _performerSchedulingService
                .BranchAgeGroupAlreadySelectedAsync(ageGroupId, branchId, currentSelectionId);
            if (alreadySelected)
            {
                return Json(new
                {
                    success = false,
                    message = "That branch already has a selection for that age group."
                });
            }
            return Json(new
            {
                success = true,
                message = "Age group available."
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditBranchKitSelection(PsBranchSelection branchSelection)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return Json(new
                {
                    success = false,
                    message = "Kit selection has not yet started."
                });
            }

            if (branchSelection.KitId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No kit selected."
                });
            }

            try
            {
                await _performerSchedulingService.UpdateBranchKitSelectionAsync(branchSelection);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            _logger.LogInformation("Selection {BranchSelectionId} edited", branchSelection.Id);

            ShowAlertSuccess("Kit selection updated!");
            return Json(new
            {
                success = true
            });
        }

        public async Task<JsonResult> GetKitAgeGroups(int kitId)
        {
            PsKit kit;
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(kitId,
                    includeAgeGroups: true);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            return Json(new
            {
                success = true,
                data = new SelectList(kit.AgeGroups, "Id", "Name")
            });
        }

        public async Task<IActionResult> KitSelections(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return RedirectToAction(nameof(Settings));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id, includeAgeGroups: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Could not view kit selections: ", gex.Message);
                return RedirectToAction(nameof(Kits));
            }

            var kits = await _performerSchedulingService.GetAllKitsAsync();
            var branchSelections = await _performerSchedulingService
                .GetKitBranchSelectionsAsync(id);

            var viewModel = new KitSelectionsViewModel
            {
                Kit = kit,
                BranchSelections = branchSelections,
                KitList = new SelectList(kits, "Id", "Name", kit.Id),
                AgeGroupList = new SelectList(kit.AgeGroups, "Id", "Name")
            };

            var kitIndexList = await _performerSchedulingService.GetKitIndexListAsync();
            var index = kitIndexList.IndexOf(id);
            viewModel.ReturnPage = (index / KitsPerPage) + 1;
            if (index != 0)
            {
                viewModel.PrevKit = kitIndexList[index - 1];
            }
            if (kitIndexList.Count != index + 1)
            {
                viewModel.NextKit = kitIndexList[index + 1];
            }

            return View(viewModel);
        }

        #endregion Kit Selections

        #region Age Groups

        [HttpPost]
        public async Task<IActionResult> AddAgeGroup(AgeGroupsListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            if (string.IsNullOrEmpty(model.AgeGroup.Name?.Trim()))
            {
                ShowAlertDanger("You must provide a name to add Age Group.");
            }
            else
            {
                try
                {
                    var ageGroup = await _performerSchedulingService
                        .AddAgeGroupAsync(model.AgeGroup);
                    ShowAlertSuccess($"Added Age Group \"{ageGroup.Name}\"!");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to add Age Group: ", gex);
                }
            }

            return RedirectToAction(nameof(AgeGroups), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        public async Task<IActionResult> AgeGroups(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            var filter = new BaseFilter(page);

            var ageGroupList = await _performerSchedulingService.GetPaginatedAgeGroupsAsync(
                filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = ageGroupList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            foreach (var ageGroup in ageGroupList.Data)
            {
                ageGroup.BackToBackBranchIds = await _performerSchedulingService
                    .GetAgeGroupBacktoBackBranchIdsAsync(ageGroup.Id);
            }

            var viewModel = new AgeGroupsListViewModel
            {
                AgeGroups = ageGroupList.Data,
                PaginateModel = paginateModel,
                PerformerSchedulingEnabled = performerSchedulingEnabled,
                Systems = await _performerSchedulingService
                    .GetSystemListWithoutExcludedBranchesAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAgeGroup(AgeGroupsListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            try
            {
                await _performerSchedulingService.RemoveAgeGroupAsync(model.AgeGroup.Id);
                ShowAlertSuccess($"Age Group \"{model.AgeGroup.Name}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Age Group: ", gex);
            }

            return RedirectToAction(nameof(AgeGroups), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAgeGroupBackToBackBranches(
            AgeGroupsListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            var systems = await _performerSchedulingService
                .GetSystemListWithoutExcludedBranchesAsync();
            var branchIds = systems.SelectMany(_ => _.Branches).Select(_ => _.Id);
            var backToBackBranches = JsonConvert
                .DeserializeObject<List<int>>(model.BackToBackBranchesString)
                .Where(_ => branchIds.Contains(_))
                .ToList();

            try
            {
                await _performerSchedulingService.UpdateAgeGroupBackToBackBranchesAsync
                    (model.AgeGroup.Id, backToBackBranches);
                ShowAlertSuccess("Back to Back branches updated!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to update back to back branches: ", gex);
            }

            return RedirectToAction(nameof(AgeGroups), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        #endregion Age Groups

        #region Blackout Dates

        [HttpPost]
        public async Task<IActionResult> AddBlackoutDate(BlackoutDatesListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            try
            {
                var blackoutDate = await _performerSchedulingService.AddBlackoutDateAsync(
                    model.BlackoutDate);
                ShowAlertSuccess($"Added Blackout Date \"{blackoutDate.Reason}\"!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add Blackout Date: ", gex);
            }

            return RedirectToAction(nameof(BlackoutDates), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        public async Task<IActionResult> BlackoutDates(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            var filter = new BaseFilter(page);

            var blackoutDateList = await _performerSchedulingService.GetPaginatedBlackoutDatesAsync(
                filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = blackoutDateList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new BlackoutDatesListViewModel
            {
                BlackoutDates = blackoutDateList.Data,
                PaginateModel = paginateModel,
                PerformerSchedulingEnabled = performerSchedulingEnabled
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlackoutDate(BlackoutDatesListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            try
            {
                await _performerSchedulingService.RemoveBlackoutDateAsync(model.BlackoutDate.Id);
                ShowAlertSuccess($"Blackout Date \"{model.BlackoutDate.Reason}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Blackout Date: ", gex);
            }

            return RedirectToAction(nameof(BlackoutDates), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        #endregion Blackout Dates

        #region Excluded Branches

        [HttpPost]
        public async Task<IActionResult> AddBranchExclusion(ExcludedBranchListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _performerSchedulingService.AddBranchExclusionAsync(model.BranchId.Value);
                    ShowAlertSuccess("Branch added to exlcusion list!");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to add Branch Exclusion: ", gex);
                }
            }

            return RedirectToAction(nameof(ExcludedBranches), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBranchExclusion(ExcludedBranchListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _performerSchedulingService
                        .RemoveBranchExclusionAsync(model.BranchId.Value);
                    ShowAlertSuccess("Branch removed from exlcusion list!");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to remove Branch Exclusion: ", gex);
                }
            }

            return RedirectToAction(nameof(ExcludedBranches), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        public async Task<IActionResult> ExcludedBranches(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService
                .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Settings));
            }

            var filter = new BaseFilter(page);

            var excludedBranchList = await _performerSchedulingService
                .GetPaginatedExcludedBranchListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = excludedBranchList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var excludedBranchIds = await _performerSchedulingService.GetExcludedBranchIdsAsync();
            var branches = await _siteService.GetAllBranches(true);
            branches = branches.Where(_ => !excludedBranchIds.Contains(_.Id));

            var viewModel = new ExcludedBranchListViewModel
            {
                ExcludedBranches = excludedBranchList.Data,
                PaginateModel = paginateModel,
                UnexcludedBranches = new SelectList(branches, "Id", "Name"),
                PerformerSchedulingEnabled = performerSchedulingEnabled
            };

            return View(viewModel);
        }

        #endregion Excluded Branches

        #region Settings

        public async Task<IActionResult> Settings()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var viewModel = new SettingsViewModel
            {
                Settings = settings,
                SiteId = GetCurrentSiteId(),
                PerformerSchedulingEnabled = _performerSchedulingService
                    .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            if (model.Settings.RegistrationClosed < model.Settings.RegistrationOpen)
            {
                ModelState.AddModelError("Settings.RegistrationClosed",
                    "The Registration Closed date cannot be before the Registration Open date.");
            }
            if (model.Settings.SchedulingPreview < model.Settings.RegistrationClosed)
            {
                ModelState.AddModelError("Settings.SchedulingPreview",
                    "The Schedule Preview date cannot be before the Registration Closed date.");
            }
            if (model.Settings.SchedulingOpen < model.Settings.SchedulingPreview)
            {
                ModelState.AddModelError("Settings.SchedulingOpen",
                    "The Schedule Open date cannot be before the Schedule Preview date.");
            }
            if (model.Settings.SchedulingClosed < model.Settings.SchedulingOpen)
            {
                ModelState.AddModelError("Settings.SchedulingClosed",
                    "The Schedule Closed date cannot be before the Schedule Open date.");
            }
            if (model.Settings.SchedulePosted < model.Settings.SchedulingClosed)
            {
                ModelState.AddModelError("Settings.SchedulePosted",
                    "The Schedule Posted date cannot be before the Schedule Closed date.");
            }
            if (model.Settings.ScheduleEndDate < model.Settings.ScheduleStartDate)
            {
                ModelState.AddModelError("Settings.ScheduleEndDate",
                    "The Schedule End date cannot be before the Schedule Start date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _performerSchedulingService.UpdateSettingsAsync(model.Settings);
                    ShowAlertSuccess("Settings updated!");
                    return RedirectToAction(nameof(Settings));
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }

            var currentSettings = await _performerSchedulingService.GetSettingsAsync();
            var viewModel = new SettingsViewModel
            {
                Settings = model.Settings,
                SiteId = GetCurrentSiteId(),
                PerformerSchedulingEnabled = _performerSchedulingService
                    .GetSchedulingStage(currentSettings) != PsSchedulingStage.Unavailable
            };
            return View(viewModel);
        }

        #endregion Settings
    }
}
