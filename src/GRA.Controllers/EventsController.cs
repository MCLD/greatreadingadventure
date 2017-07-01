using GRA.Controllers.ViewModel.Events;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace GRA.Controllers
{
    public class EventsController : Base.UserController
    {
        private readonly ILogger<EventsController> _logger;
        private readonly EventService _eventService;
        private readonly SiteService _siteService;
        public EventsController(ILogger<EventsController> logger,
            ServiceFacade.Controller context,
            EventService eventService,
            SiteService siteService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _eventService = Require.IsNotNull(eventService, nameof(eventService));
            _siteService = Require.IsNotNull(siteService, nameof(SiteService));
            PageTitle = "Events";
        }
        public async Task<IActionResult> CommunityExperiences(int page = 1,
            string search = null,
            int? system = null,
            int? branch = null,
            int? location = null,
            int? program = null,
            string StartDate = null,
            string EndDate = null)
        {
            return await Index(page, search, system, branch, location, program, StartDate, EndDate, true);
        }

        public async Task<IActionResult> Index(int page = 1,
            string search = null,
            int? system = null,
            int? branch = null,
            int? location = null,
            int? program = null,
            string StartDate = null,
            string EndDate = null,
            bool CommunityExperiences = false)
        {
            ModelState.Clear();
            EventFilter filter = new EventFilter(page)
            {
                Search = search,
                EventType = CommunityExperiences ? 1 : 0
            };

            // ignore location if branch has value
            if (branch.HasValue)
            {
                filter.BranchIds = new List<int>() { branch.Value };
            }
            else if (system.HasValue)
            {
                filter.SystemIds = new List<int>() { system.Value };
            }
            else if (location.HasValue)
            {
                filter.LocationIds = new List<int?>() { location.Value };
            }

            if (program.HasValue)
            {
                filter.ProgramIds = new List<int?>() { program.Value };
            }

            if (!String.Equals(StartDate, "False", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    DateTime startDate;
                    if (DateTime.TryParse(StartDate, out startDate))
                    {
                        filter.StartDate = startDate.Date;
                    }
                }
                else
                {
                    filter.StartDate = _dateTimeProvider.Now.Date;
                }
            }

            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                DateTime endDate;
                if (DateTime.TryParse(EndDate, out endDate))
                {
                    filter.EndDate = endDate.Date;
                }
            }

            var eventList = await _eventService.GetPaginatedListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = eventList.Count,
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

            EventsListViewModel viewModel = new EventsListViewModel()
            {
                Events = eventList.Data,
                PaginateModel = paginateModel,
                Search = search,
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                ProgramId = program,
                SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name"),
                LocationList = new SelectList((await _eventService.GetLocations()), "Id", "Name"),
                ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name"),
                CommunityExperiences = CommunityExperiences
            };

            if (branch.HasValue)
            {
                var selectedBranch = await _siteService.GetBranchByIdAsync(branch.Value);
                viewModel.SystemId = selectedBranch.SystemId;
                viewModel.BranchList = new SelectList(
                    (await _siteService.GetBranches(selectedBranch.SystemId)),
                    "Id", "Name", branch.Value);
            }
            else if (system.HasValue)
            {
                viewModel.SystemId = system;
                viewModel.BranchList = new SelectList(
                    (await _siteService.GetBranches(system.Value)), "Id", "Name");
            }
            else
            {
                viewModel.BranchList = new SelectList((await _siteService.GetAllBranches()),
                    "Id", "Name");
            }
            if (location.HasValue && !branch.HasValue)
            {
                viewModel.LocationId = location.Value;
                viewModel.UseLocation = true;
            }

            return View("Index", viewModel);
        }

        [HttpPost]
        public IActionResult Index(EventsListViewModel model)
        {
            string startDate = "False";
            string endDate = null;
            bool? isCommunityExperience = null;

            if (model.UseLocation == true)
            {
                model.SystemId = null;
                model.BranchId = null;
            }
            else
            {
                model.LocationId = null;
            }

            if (model.BranchId.HasValue)
            {
                model.SystemId = null;
            }

            if (model.StartDate.HasValue)
            {
                startDate = model.StartDate.Value.ToString("MM-dd-yyyy");
            }
            if (model.EndDate.HasValue
                && (!model.StartDate.HasValue || model.EndDate >= model.StartDate))
            {
                endDate = model.EndDate.Value.ToString("MM-dd-yyyy");
            }
            if (model.CommunityExperiences)
            {
                isCommunityExperience = true;
            }

            return RedirectToAction("Index", new { Search = model.Search, System = model.SystemId, Branch = model.BranchId, Location = model.LocationId, Program = model.ProgramId, StartDate = startDate, EndDate = endDate, CommunityExperiences = isCommunityExperience });
        }

        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                EventsDetailViewModel viewModel = new EventsDetailViewModel()
                {
                    Event = await _eventService.GetDetails(id)
                };

                if (viewModel.Event.ProgramId.HasValue)
                {
                    var program = await _siteService.GetProgramByIdAsync(viewModel.Event.ProgramId.Value);
                    viewModel.ProgramString = $"This event is limited to the {program.Name} program.";
                }
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view event: ", gex);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> GetDetails(int eventId)
        {
            try
            {
                EventsDetailViewModel viewModel = new EventsDetailViewModel()
                {
                    Event = await _eventService.GetDetails(eventId)
                };
                if (viewModel.Event.ProgramId.HasValue)
                {
                    var program = await _siteService.GetProgramByIdAsync(viewModel.Event.ProgramId.Value);
                    viewModel.ProgramString = $"This event is limited to the {program.Name} program.";
                }

                return PartialView("_DetailPartial", viewModel);
            }
            catch (GraException gex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Content($"Unable to view event: {gex.Message}");
            }
        }
    }
}