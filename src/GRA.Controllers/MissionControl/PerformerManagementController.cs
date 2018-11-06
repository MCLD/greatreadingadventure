using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.PerformerManagement;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManagePerformers)]
    public class PerformerManagementController : Base.MCController
    {
        private readonly ILogger<PerformerManagementController> _logger;
        private readonly PerformerSchedulingService _performerSchedulingService;
        public PerformerManagementController(ILogger<PerformerManagementController> logger,
            ServiceFacade.Controller context,
            PerformerSchedulingService performerSchedulingService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _performerSchedulingService = performerSchedulingService
                ?? throw new ArgumentNullException(nameof(performerSchedulingService));
            PageTitle = "Performer Management";
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService.GetSchedulingStage(settings)
                    != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var viewModel = new PerformerListViewModel
            {
                PerformerSchedulingEnbabled = performerSchedulingEnabled
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Schedule()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var viewModel = new ScheduleDetailViewModel
            {
                Settings = settings,
                PerformerSchedulingEnbabled = _performerSchedulingService
                    .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Schedule(PsSettings settings)
        {
            if (settings.RegistrationClosed < settings.RegistrationOpen)
            {
                ModelState.AddModelError("Settings.RegistrationClosed", "The Registration Closed date cannot be before the Registration Open date.");
            }
            if (settings.SchedulingPreview < settings.RegistrationClosed)
            {
                ModelState.AddModelError("Settings.SchedulingPreview", "The Schedule Preview date cannot be before the Registration Closed date.");
            }
            if (settings.SchedulingOpen < settings.SchedulingPreview)
            {
                ModelState.AddModelError("Settings.SchedulingOpen", "The Schedule Open date cannot be before the Schedule Preview date.");
            }
            if (settings.SchedulingClosed < settings.SchedulingOpen)
            {
                ModelState.AddModelError("Settings.SchedulingClosed", "The Schedule Closed date cannot be before the Schedule Open date.");
            }
            if (settings.SchedulePosted < settings.SchedulingClosed)
            {
                ModelState.AddModelError("Settings.SchedulePosted", "The Schedule Posted date cannot be before the Schedule Closed date.");
            }

            if (settings.ScheduleEndDate < settings.ScheduleStartDate)
            {
                ModelState.AddModelError("Settings.ScheduleEndDate", "The Schedule End date cannot be before the Schedule Start date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _performerSchedulingService.UpdateSettingsAsync(settings);
                    ShowAlertSuccess($"Schedule successfully updated!");
                    return RedirectToAction(nameof(Schedule));
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }

            var viewModel = new ScheduleDetailViewModel
            {
                Settings = settings,
                PerformerSchedulingEnbabled = _performerSchedulingService.GetSchedulingStage(settings)
                    != PsSchedulingStage.Unavailable
            };
            return View(viewModel);
        }

        public async Task<IActionResult> BlackoutDates(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService.GetSchedulingStage(settings)
                    != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var filter = new BaseFilter(page);

            var blackoutDateList = await _performerSchedulingService.GetPaginatedBlackoutDatesAsync(
                filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = blackoutDateList.Count,
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

            var viewModel = new BlackoutDatesListViewModel
            {
                BlackoutDates = blackoutDateList.Data,
                PaginateModel = paginateModel,
                PerformerSchedulingEnbabled = performerSchedulingEnabled
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddBlackoutDate(BlackoutDatesListViewModel model)
        {
            try
            {
                var blackoutDate = await _performerSchedulingService.AddBlackoutDateAsync(
                    model.BlackoutDate);
                ShowAlertSuccess($"Added Blackout Date \"{blackoutDate.Reason}\"!");
            }
            catch(GraException gex)
            {
                ShowAlertDanger("Unable to add Blackout Date: ", gex);
            }

            return RedirectToAction(nameof(BlackoutDates), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost] 
        public async Task<IActionResult> DeleteBlackoutDate(BlackoutDatesListViewModel model)
        {
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

        public async Task<IActionResult> AgeGroups(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService.GetSchedulingStage(settings)
                    != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Schedule));
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
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new AgeGroupsListViewModel
            {
                AgeGroups = ageGroupList.Data,
                PaginateModel = paginateModel,
                PerformerSchedulingEnbabled = performerSchedulingEnabled
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddAgeGroup(AgeGroupsListViewModel model)
        {
            try
            {
                var ageGroup = await _performerSchedulingService.AddAgeGroupAsync(model.AgeGroup);
                ShowAlertSuccess($"Added Age Group \"{ageGroup.Name}\"!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add Age Group: ", gex);
            }

            return RedirectToAction(nameof(AgeGroups), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAgeGroup(AgeGroupsListViewModel model)
        {
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
    }
}
