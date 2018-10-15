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
            var dates = await _performerSchedulingService.GetDatesAsync();
            var performerSchedulingEnabled = _performerSchedulingService.GetSchedulingStage(dates)
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
            var dates = await _performerSchedulingService.GetDatesAsync();
            var viewModel = new ScheduleViewModel
            {
                Dates = dates,
                PerformerSchedulingEnbabled = _performerSchedulingService.GetSchedulingStage(dates) 
                    != PsSchedulingStage.Unavailable
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Schedule(PsDates dates)
        {
            if (dates.RegistrationClosed < dates.RegistrationOpen)
            {
                ModelState.AddModelError("Dates.RegistrationClosed", "The Registration Closed date cannot be before the Registration Open date.");
            }
            if (dates.SchedulingPreview < dates.RegistrationClosed)
            {
                ModelState.AddModelError("Dates.SchedulingPreview", "The Schedule Preview date cannot be before the Registration Closed date.");
            }
            if (dates.SchedulingOpen < dates.SchedulingPreview)
            {
                ModelState.AddModelError("Dates.SchedulingOpen", "The Schedule Open date cannot be before the Schedule Preview date.");
            }
            if (dates.SchedulingClosed < dates.SchedulingOpen)
            {
                ModelState.AddModelError("Dates.SchedulingClosed", "The Schedule Closed date cannot be before the Schedule Open date.");
            }
            if (dates.SchedulePosted < dates.SchedulingClosed)
            {
                ModelState.AddModelError("Dates.SchedulePosted", "The Schedule Posted date cannot be before the Schedule Closed date.");
            }

            if (dates.ScheduleEndDate < dates.ScheduleStartDate)
            {
                ModelState.AddModelError("Dates.ScheduleEndDate", "The Schedule End date cannot be before the Schedule Start date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _performerSchedulingService.UpdateDatesAsync(dates);
                    ShowAlertSuccess($"Schedule successfully updated!");
                    return RedirectToAction(nameof(Schedule));
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }

            var viewModel = new ScheduleViewModel
            {
                Dates = dates,
                PerformerSchedulingEnbabled = _performerSchedulingService.GetSchedulingStage(dates)
                    != PsSchedulingStage.Unavailable
            };
            return View(viewModel);
        }

        public async Task<IActionResult> BlackoutDates(int page = 1)
        {
            var dates = await _performerSchedulingService.GetDatesAsync();
            var performerSchedulingEnabled = _performerSchedulingService.GetSchedulingStage(dates)
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

            var viewModel = new BlackoutDatesViewModel
            {
                BlackoutDates = blackoutDateList.Data,
                PaginateModel = paginateModel,
                PerformerSchedulingEnbabled = performerSchedulingEnabled
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddBlackoutDate(BlackoutDatesViewModel model)
        {
            try
            {
                await _performerSchedulingService.AddBlackoutDateAsync(model.BlackoutDate);
                ShowAlertSuccess($"Added Blackout Date!");
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
        public async Task<IActionResult> DeleteBlackoutDate(BlackoutDatesViewModel model)
        {
            try
            {
                await _performerSchedulingService.RemoveBlackoutDateAsync(model.BlackoutDate.Id);
                ShowAlertSuccess("Blackout Date removed!");
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
    }
}
