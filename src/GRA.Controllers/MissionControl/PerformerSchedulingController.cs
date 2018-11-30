using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.PerformerScheduling;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    public class PerformerSchedulingController : Base.MCController
    {
        private static readonly int KitsPerPage = 15;
        private static readonly int PerformersPerPage = 15;
        private static readonly int ProgramsPerPage = 15;

        private static readonly DateTime DefaultPerformerScheduleStartTime = DateTime.Parse("8:00 AM");
        private static readonly DateTime DefaultPerformerScheduleEndTime = DateTime.Parse("8:00 PM");

        private readonly ILogger<PerformerSchedulingController> _logger;
        private readonly PerformerSchedulingService _performerSchedulingService;
        private readonly SiteService _siteService;
        public PerformerSchedulingController(ILogger<PerformerSchedulingController> logger,
            ServiceFacade.Controller context,
            PerformerSchedulingService performerSchedulingService,
            SiteService siteService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _performerSchedulingService = performerSchedulingService
                ?? throw new ArgumentNullException(nameof(performerSchedulingService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            PageTitle = "Performer Scheduling";
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                ShowAlertDanger("Performer scheduling is not set up.");
                return RedirectToAction("Index", "Home");
            }

            var systemId = GetId(ClaimType.SystemId);

            var viewModel = new ScheduleOverviewViewModel
            {
                Settings = settings,
                SchedulingStage = schedulingStage,

                AgeGroups = await _performerSchedulingService.GetAgeGroupsAsync()
            };

            if (schedulingStage >= PsSchedulingStage.SchedulingOpen)
            {
                var system = await _siteService.GetSystemByIdAsync(systemId);
                viewModel.SystemName = system.Name;

                var branches = await _siteService.GetBranches(systemId);
                foreach (var branch in branches)
                {
                    branch.Selections = await _performerSchedulingService
                        .GetSelectionsByBranchIdAsync(branch.Id);
                }

                viewModel.Branches = branches;
            }

            if (schedulingStage >= PsSchedulingStage.SchedulePosted)
            {
                var branches = await _siteService.GetBranches(systemId, true);
                viewModel.BranchList = new SelectList(branches, "Id", "Name");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Schedule(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulePosted)
            {
                return RedirectToAction(nameof(Index));
            }

            var branch = await _siteService.GetBranchByIdAsync(id);

            if (branch == null)
            {
                return RedirectToAction(nameof(Index));
            }

            branch.Selections = await _performerSchedulingService
                .GetSelectionsByBranchIdAsync(branch.Id);

            var viewModel = new ScheduleViewModel()
            {
                Settings = settings,
                Branch = branch
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Performers(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingPreview)
            {
                return RedirectToAction(nameof(Index));
            }

            var filter = new PerformerSchedulingFilter(page, PerformersPerPage)
            {
                IsApproved = true
            };

            var performerList = await _performerSchedulingService.GetPaginatedPerformerListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = performerList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var systemId = GetId(ClaimType.SystemId);
            foreach (var performer in performerList.Data)
            {
                performer.AvailableInSystem = await _performerSchedulingService
                    .GetPerformerSystemAvailabilityAsync(performer.Id, systemId);
                performer.ProgramCount = await _performerSchedulingService
                    .GetPerformerProgramCountAsync(performer.Id);
            }

            var viewModel = new PerformerListViewModel()
            {
                Performers = performerList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Performer(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingPreview)
            {
                return RedirectToAction(nameof(Index));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    includePrograms: true,
                    includeSchedule: true,
                    onlyApproved: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view performer: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            var system = await _siteService.GetSystemByIdAsync(GetId(ClaimType.SystemId));

            var viewModel = new PerformerViewModel()
            {
                AgeGroups = await _performerSchedulingService
                    .GetPerformerAgeGroupsAsync(performer.Id),
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                Performer = performer,
                Settings = settings,
                System = system
            };

            if (performer.AllBranches == false)
            {
                viewModel.BranchAvailability = await _performerSchedulingService
                    .GetPerformerBranchIdsAsync(performer.Id, system.Id);
            }

            if (performer.Images.Any())
            {
                viewModel.ImagePath = _pathResolver.ResolveContentPath(
                    performer.Images.First().Filename);
            }

            if (!string.IsNullOrWhiteSpace(performer.Website))
            {
                if (Uri.TryCreate(performer.Website, UriKind.Absolute, out Uri absoluteUri))
                {
                    viewModel.Uri = absoluteUri;
                }
            }

            var performerIndexList = await _performerSchedulingService
                .GetPerformerIndexListAsync(true);
            var index = performerIndexList.IndexOf(id);
            viewModel.ReturnPage = index / PerformersPerPage + 1;
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

        public async Task<IActionResult> PerformerImages(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingPreview)
            {
                return RedirectToAction(nameof(Index));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    onlyApproved: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view performer images: ", gex);
                return RedirectToAction(nameof(Performers));
            }

            performer.Images.ForEach(_ => _.Filename = _pathResolver
                .ResolveContentPath(_.Filename));

            return View(performer);
        }

        public async Task<IActionResult> Programs(int page = 1, int? ageGroup = null)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingPreview)
            {
                return RedirectToAction(nameof(Index));
            }

            var filter = new PerformerSchedulingFilter(page, ProgramsPerPage)
            {
                AgeGroupId = ageGroup,
                IsApproved = true
            };

            var programList = await _performerSchedulingService.GetPaginatedProgramListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = programList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var systemId = GetId(ClaimType.SystemId);
            foreach (var program in programList.Data)
            {
                program.AvailableInSystem = await _performerSchedulingService
                    .GetPerformerSystemAvailabilityAsync(program.PerformerId, systemId);
            }

            var viewModel = new ProgramListViewModel()
            {
                Programs = programList.Data,
                PaginateModel = paginateModel,
                AgeGroups = await _performerSchedulingService.GetAgeGroupsAsync(),
                AgeGroupId = ageGroup
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Program(int id, bool? list = null, int? ageGroup = null)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingPreview)
            {
                return RedirectToAction(nameof(Index));
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(id,
                    onlyApproved: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program: ", gex);
                return RedirectToAction(nameof(Programs));
            }

            var selectedAgeGroup = program.AgeGroups.Where(_ => _.Id == ageGroup).FirstOrDefault();

            var performer = await _performerSchedulingService.GetPerformerByIdAsync(
                program.PerformerId);
            var system = await _siteService.GetSystemByIdAsync(GetId(ClaimType.SystemId));

            var viewModel = new ProgramViewModel()
            {
                AgeGroup = selectedAgeGroup,
                AllBranches = performer.AllBranches,
                List = list == true,
                Program = program,
                SchedulingOpen = schedulingStage == PsSchedulingStage.SchedulingOpen,
                System = system
            };

            if (performer.AllBranches == false)
            {
                viewModel.BranchAvailability = await _performerSchedulingService
                    .GetPerformerBranchIdsAsync(performer.Id, system.Id);
            }

            if (program.Images?.Count > 0)
            {
                viewModel.Image = _pathResolver.ResolveContentPath(
                    program.Images.First().Filename);
            }

            if (viewModel.SchedulingOpen)
            {
                viewModel.AgeGroupList = new SelectList(program.AgeGroups, "Id", "Name");
                var branches = new List<Branch>();
                foreach (var branch in system.Branches)
                {
                    if (performer.AllBranches == false
                        && viewModel.BranchAvailability.Contains(branch.Id) == false)
                    {
                        continue;
                    }
                    else
                    {
                        var branchSelections = await _performerSchedulingService
                            .GetSelectionsByBranchIdAsync(branch.Id);

                        if (branchSelections.Count >= settings.SelectionsPerBranch)
                        {
                            continue;
                        }
                        else
                        {
                            var selectedAgeGroups = branchSelections.Select(_ => _.AgeGroupId);
                            var availableAgeGroups = program.AgeGroups
                                .Where(_ => selectedAgeGroups.Contains(_.Id) == false)
                                .Any();

                            if (availableAgeGroups == false)
                            {
                                continue;
                            }
                        }
                    }
                    branches.Add(branch);
                }
                viewModel.BranchList = new SelectList(branches, "Id", "Name");
            }

            if (viewModel.List)
            {
                var programIndexList = await _performerSchedulingService.GetProgramIndexListAsync(
                    ageGroupId: selectedAgeGroup?.Id, onlyApproved: true);

                var index = programIndexList.IndexOf(id);
                viewModel.ReturnPage = index / ProgramsPerPage + 1;
                if (index != 0)
                {
                    viewModel.PrevProgram = programIndexList[index - 1];
                }
                if (programIndexList.Count != index + 1)
                {
                    viewModel.NextProgram = programIndexList[index + 1];
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ProgramImages(int id, bool? list = null,
            int? ageGroup = null)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingPreview)
            {
                return RedirectToAction(nameof(Index));
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(id,
                onlyApproved: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program images: ", gex);
                return RedirectToAction(nameof(Programs));
            }

            program.Images.ForEach(_ => _.Filename = _pathResolver
                .ResolveContentPath(_.Filename));

            var viewModel = new ProgramImagesViewModel()
            {
                Program = program
            };

            if (list == true)
            {
                viewModel.List = list;
                viewModel.AgeGroup = ageGroup;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SelectProgram(ProgramViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.SchedulingOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(
                    model.BranchSelection.ProgramId.Value, onlyApproved: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to select program: ", gex);
                return RedirectToAction(nameof(Programs));
            }

            var ageAlreadySelected = await _performerSchedulingService
                .BranchAgeGroupAlreadySelectedAsync(model.BranchSelection.AgeGroupId,
                    model.BranchSelection.BranchId);

            if (ageAlreadySelected)
            {
                TempData[TempDataKey.AlertDanger] = $"Branch already has a selection for that age group.";
                return RedirectToAction(nameof(Program), new { id = program.Id });
            }

            var programAvailableAtBranch = await _performerSchedulingService
                .ProgramAvailableAtBranchAsync(model.BranchSelection.ProgramId.Value,
                    model.BranchSelection.BranchId);

            if (programAvailableAtBranch == false)
            {
                TempData[TempDataKey.AlertDanger] = "The performer does not performer at that branch.";
                return RedirectToAction(nameof(Program), new { id = program.Id });
            }

            var viewModel = new SelectProgramViewModel()
            {
                BranchSelection = model.BranchSelection,
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                ScheduleDates = await _performerSchedulingService.GetPerformerScheduleAsync(
                    program.PerformerId),
                BookedDates = (await _performerSchedulingService
                    .GetPerformerBranchSelectionsAsync(program.PerformerId))
                    .Select(_ => _.RequestedStartTime),
                Settings = settings
            };

            if (await _performerSchedulingService.BranchAgeGroupHasBackToBackAsync(
                model.BranchSelection.AgeGroupId, model.BranchSelection.BranchId))
            {
                viewModel.BranchSelection.BackToBackProgram = true;
            }

            viewModel.BranchSelection.Program = program;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> ScheduleProgram(PsBranchSelection branchSelection)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.SchedulingOpen)
            {
                return Json(new
                {
                    success = false,
                    message = "Program selection is not currently open."
                });
            }

            try
            {
                await _performerSchedulingService.AddBranchProgramSelectionAsync(branchSelection);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            _logger.LogInformation($"Selection {branchSelection.Id} added by user {GetId(ClaimType.UserId)}");

            TempData[TempDataKey.AlertSuccess] = "Program selection added!";
            return Json(new
            {
                success = true
            });
        }

        public async Task<JsonResult> GetProgramAvailableAgeGroupsAsync(int branchId, int programId)
        {
            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(programId,
                    onlyApproved: true);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            var branchSelections = await _performerSchedulingService
                .GetSelectionsByBranchIdAsync(branchId);
            if (branchSelections.Count >= 3)
            {
                return Json(new
                {
                    success = false,
                    message = "Branch has already made its 3 selections."
                });
            }

            var availableAgeGroups = program.AgeGroups.Except(
                branchSelections.Select(_ => _.AgeGroup)).ToList();
            if (availableAgeGroups.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Branch  already has selections for all of this programs age groups."
                });
            }

            return Json(new
            {
                success = true,
                data = new SelectList(availableAgeGroups, "Id", "Name")
            });
        }

        public async Task<IActionResult> GetPerformerDaySchedule(int performerId, DateTime date)
        {
            var performerSchedule = await _performerSchedulingService.GetPerformerDateScheduleAsync(
                performerId, date.Date);

            var branchSelections = (await _performerSchedulingService
                .GetPerformerBranchSelectionsAsync(performerId, date)).ToList();

            var viewModel = new DayScheduleViewModel()
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
            if (latestSelection == null ||
                endTime.TimeOfDay > latestSelection.ScheduleStartTime
                    .AddMinutes(latestSelection.ScheduleDuration).TimeOfDay)
            {
                viewModel.EndTime = endTime.ToShortTimeString();
            }

            return PartialView("_DaySchedulePartial", viewModel);
        }

        public async Task<JsonResult> CheckProgramTimeAvailability(int programId, DateTime date,
            bool backToBack)
        {
            try
            {
                var result = await _performerSchedulingService
                    .ValidateScheduleTimeAsync(programId, date, backToBack);

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

        public async Task<IActionResult> Kits(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingPreview)
            {
                return RedirectToAction(nameof(Index));
            }

            var filter = new BaseFilter(page, KitsPerPage);

            var kitList = await _performerSchedulingService.GetPaginatedKitListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = kitList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new KitListViewModel()
            {
                Kits = kitList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Kit(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingPreview)
            {
                return RedirectToAction(nameof(Index));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view kit: ", gex);
                return RedirectToAction(nameof(Kits));
            }

            var viewModel = new KitViewModel()
            {
                Kit = kit,
                SchedulingOpen = schedulingStage == PsSchedulingStage.SchedulingOpen
            };

            if (kit.Images.Any())
            {
                viewModel.ImagePath = _pathResolver.ResolveContentPath(
                    kit.Images.First().Filename);
            }

            if (!string.IsNullOrWhiteSpace(kit.Website))
            {
                if (Uri.TryCreate(kit.Website, UriKind.Absolute, out Uri absoluteUri))
                {
                    viewModel.Uri = absoluteUri;
                }
            }

            if (viewModel.SchedulingOpen)
            {
                viewModel.AgeGroupList = new SelectList(kit.AgeGroups, "Id", "Name");

                var system = await _siteService.GetSystemByIdAsync(GetId(ClaimType.SystemId));

                var branches = new List<Branch>();
                foreach (var branch in system.Branches)
                {
                    var branchSelections = await _performerSchedulingService
                            .GetSelectionsByBranchIdAsync(branch.Id);

                    if (branchSelections.Count >= settings.SelectionsPerBranch)
                    {
                        continue;
                    }
                    else
                    {
                        var selectedAgeGroups = branchSelections.Select(_ => _.AgeGroupId);
                        var availableAgeGroups = kit.AgeGroups
                            .Where(_ => selectedAgeGroups.Contains(_.Id) == false)
                            .Any();

                        if (availableAgeGroups == false)
                        {
                            continue;
                        }
                    }
                    branches.Add(branch);
                }
                viewModel.BranchList = new SelectList(branches, "Id", "Name");
            }

            var kitIndexList = await _performerSchedulingService.GetKitIndexListAsync();
            var index = kitIndexList.IndexOf(id);
            viewModel.ReturnPage = index / KitsPerPage + 1;
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

        public async Task<IActionResult> KitImages(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingPreview)
            {
                return RedirectToAction(nameof(Index));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view kit images: ", gex);
                return RedirectToAction(nameof(Kits));
            }

            kit.Images.ForEach(_ => _.Filename = _pathResolver
               .ResolveContentPath(_.Filename));

            return View(kit);
        }

        [HttpPost]
        public async Task<IActionResult> SelectKit(PsBranchSelection branchSelection)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.SchedulingOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                branchSelection = await _performerSchedulingService.AddBranchProgramSelectionAsync(
                    branchSelection);
                _logger.LogInformation($"Selection {branchSelection.Id} added by user {GetId(ClaimType.UserId)}");
                ShowAlertSuccess("Kit selection added!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger($"Unable to select kit: ", gex);
            }

            if (branchSelection.KitId.HasValue)
            {
                return RedirectToAction(nameof(Kit), new { id = branchSelection.KitId });
            }
            else
            {
                return RedirectToAction(nameof(Kits));
            }
        }

        public async Task<JsonResult> GetKitAvailableAgeGroupsAsync(int branchId, int kitId)
        {
            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(kitId);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            var branchSelections = await _performerSchedulingService
                .GetSelectionsByBranchIdAsync(branchId);
            if (branchSelections.Count >= 3)
            {
                return Json(new
                {
                    success = false,
                    message = "Branch has already made its 3 selections."
                });
            }

            var availableAgeGroups = kit.AgeGroups.Except(
                branchSelections.Select(_ => _.AgeGroup)).ToList();
            if (availableAgeGroups.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Branch already has selections for all of this kits age groups."
                });
            }

            return Json(new
            {
                success = true,
                data = new SelectList(availableAgeGroups, "Id", "Name")
            });
        }
    }
}
