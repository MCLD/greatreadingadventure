using GRA.Controllers.ViewModel.MissionControl.Events;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageEvents)]
    public class EventsController : Base.MCController
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

        public async Task<IActionResult> Index(string search,
            int? systemId, int? branchId, bool? mine, int? programId, int page = 1)
        {
            BaseFilter filter = new BaseFilter(page);

            if (!string.IsNullOrWhiteSpace(search))
            {
                filter.Search = search;
            }

            if (mine == true)
            {
                filter.UserIds = new List<int>() { GetId(ClaimType.UserId) };
            }
            else if (branchId.HasValue)
            {
                filter.BranchIds = new List<int>() { branchId.Value };
            }
            else if (systemId.HasValue)
            {
                filter.SystemIds = new List<int>() { systemId.Value };
            }

            if (programId.HasValue)
            {
                if (programId.Value > 0)
                {
                    filter.ProgramIds = new List<int?>() { programId.Value };
                }
                else
                {
                    filter.ProgramIds = new List<int?>() { null };
                }
            }

            var eventList = await _eventService.GetPaginatedListAsync(filter, true);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = eventList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(new { page = paginateModel.LastPage ?? 1 });
            }

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);

            EventsListViewModel viewModel = new EventsListViewModel()
            {
                Events = eventList.Data,
                PaginateModel = paginateModel,
                Search = search,
                SystemId = systemId,
                BranchId = branchId,
                ProgramId = programId,
                Mine = mine,
                SystemList = systemList,
                ProgramList = await _siteService.GetProgramList(),
                CanManageLocations = UserHasPermission(Permission.ManageLocations)
            };

            if (mine == true)
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Mine";
            }
            else if (branchId.HasValue)
            {
                var branch = await _siteService.GetBranchByIdAsync(branchId.Value);
                viewModel.BranchName = branch.Name;
                viewModel.SystemName = systemList
                    .Where(_ => _.Id == branch.SystemId).SingleOrDefault().Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (systemId.HasValue)
            {
                viewModel.SystemName = systemList
                    .Where(_ => _.Id == systemId.Value).SingleOrDefault().Name;
                viewModel.BranchList = (await _siteService.GetBranches(systemId.Value))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "System";
            }
            else
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "All";
            }
            if (programId.HasValue)
            {
                if (programId.Value > 0)
                {
                    viewModel.ProgramName =
                        (await _siteService.GetProgramByIdAsync(programId.Value)).Name;
                }
                else
                {
                    viewModel.ProgramName = "Not Limited";
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Create(int? id)
        {
            PageTitle = "Create Event";

            var branchList = await _siteService.GetBranches(GetId(ClaimType.SystemId));
            var locationList = await _eventService.GetLocations();
            var programList = await _siteService.GetProgramList();
            EventsDetailViewModel viewModel = new EventsDetailViewModel()
            {
                CanManageLocations = UserHasPermission(Permission.ManageLocations),
                BranchList = new SelectList(branchList, "Id", "Name"),
                LocationList = new SelectList(locationList, "Id", "Name"),
                ProgramList = new SelectList(programList, "Id", "Name")
            };

            if (id.HasValue)
            {
                var graEvent = await _eventService.GetDetails(id.Value);
                if (!graEvent.ParentEventId.HasValue)
                {
                    graEvent.ParentEventId = graEvent.Id;
                }
                viewModel.Event = graEvent;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventsDetailViewModel model)
        {
            if (model.UseLocation && !model.Event.AtLocationId.HasValue)
            {
                ModelState.AddModelError("Event.AtLocationId", "The At Location field is required.");
            }
            if (!model.UseLocation && !model.Event.AtBranchId.HasValue)
            {
                ModelState.AddModelError("Event.AtBranchId", "The At Branch field is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(model.Event.ExternalLink))
                    {
                        model.Event.ExternalLink = new UriBuilder(
                            model.Event.ExternalLink).Uri.AbsoluteUri;
                    }
                    var graEvent = model.Event;
                    if (model.UseLocation)
                    {
                        graEvent.AtBranchId = null;
                    }
                    else
                    {
                        graEvent.AtLocationId = null;
                    }
                    graEvent.IsActive = true;
                    graEvent.IsValid = true;

                    await _eventService.Add(graEvent);
                    ShowAlertSuccess($"Event '{graEvent.Name}' created.");
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Could not create event: ", gex.Message);
                }
            }
            PageTitle = "Create Event";

            var branchList = await _siteService.GetBranches(GetId(ClaimType.SystemId));
            var locationList = await _eventService.GetLocations();
            var programList = await _siteService.GetProgramList();

            model.BranchList = new SelectList(branchList, "Id", "Name");
            model.LocationList = new SelectList(locationList, "Id", "Name");
            model.ProgramList = new SelectList(programList, "Id", "Name");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            PageTitle = "Edit Event";

            var graEvent = await _eventService.GetDetails(id);
            var branchList = await _siteService.GetBranches(GetId(ClaimType.SystemId));
            var locationList = await _eventService.GetLocations();
            var programList = await _siteService.GetProgramList();
            EventsDetailViewModel viewModel = new EventsDetailViewModel()
            {
                Event = graEvent,
                UseLocation = graEvent.AtLocationId.HasValue,
                CanManageLocations = UserHasPermission(Permission.ManageLocations),
                BranchList = new SelectList(branchList, "Id", "Name"),
                LocationList = new SelectList(locationList, "Id", "Name"),
                ProgramList = new SelectList(programList, "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventsDetailViewModel model)
        {
            if (model.UseLocation && !model.Event.AtLocationId.HasValue)
            {
                ModelState.AddModelError("Event.AtLocationId", "The At Location field is required.");
            }
            if (!model.UseLocation && !model.Event.AtBranchId.HasValue)
            {
                ModelState.AddModelError("Event.AtBranchId", "The At Branch field is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(model.Event.ExternalLink))
                    {
                        model.Event.ExternalLink = new UriBuilder(
                            model.Event.ExternalLink).Uri.AbsoluteUri;
                    }
                    var graEvent = model.Event;
                    if (model.UseLocation)
                    {
                        graEvent.AtBranchId = null;
                    }
                    else
                    {
                        graEvent.AtLocationId = null;
                    }

                    await _eventService.Edit(graEvent);
                    ShowAlertSuccess($"Event '{graEvent.Name}' edited.");
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Could not create event: ", gex.Message);
                }
            }
            PageTitle = "Edit Event";

            var branchList = await _siteService.GetBranches(GetId(ClaimType.SystemId));
            var locationList = await _eventService.GetLocations();
            var programList = await _siteService.GetProgramList();

            model.BranchList = new SelectList(branchList, "Id", "Name");
            model.LocationList = new SelectList(locationList, "Id", "Name");
            model.ProgramList = new SelectList(programList, "Id", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _eventService.Remove(id);
            ShowAlertSuccess("Event deleted.");
            return RedirectToAction("Index");
        }

        [Authorize(Policy = Policy.ManageLocations)]
        public async Task<IActionResult> Locations(int page = 1)
        {
            BaseFilter filter = new BaseFilter(page);

            var locationList = await _eventService.GetPaginatedLocationsListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = locationList.Count,
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

            LocationsListViewModel viewModel = new LocationsListViewModel()
            {
                Locations = locationList.Data,
                PaginateModel = paginateModel,
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.ManageLocations)]
        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationsListViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(model.Location.Url))
                    {
                        model.Location.Url = new UriBuilder(model.Location.Url).Uri.AbsoluteUri;
                    }
                    await _eventService.AddLocation(model.Location);
                    ShowAlertSuccess($"Added Location '{model.Location.Name}'");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to add Location: ", gex);
                }
            }
            return RedirectToAction("Locations");
        }

        [Authorize(Policy = Policy.ManageLocations)]
        [HttpPost]
        public async Task<IActionResult> EditLocation(LocationsListViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(model.Location.Url))
                    {
                        model.Location.Url = new UriBuilder(model.Location.Url).Uri.AbsoluteUri;
                    }
                    await _eventService.EditLocation(model.Location);
                    ShowAlertSuccess($"Location '{model.Location.Name}' updated");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit Location: ", gex);
                }
            }
            return RedirectToAction("Locations");
        }

        [Authorize(Policy = Policy.ManageLocations)]
        [HttpPost]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                await _eventService.RemoveLocation(id);
                ShowAlertSuccess("Location removed");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete Location: ", gex);
            }
            return RedirectToAction("Locations");
        }

        [Authorize(Policy = Policy.ManageLocations)]
        public async Task<JsonResult> AddLocationReturnList(string name,
            string address, string url, string telephone)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                url = new UriBuilder(url).Uri.AbsoluteUri;
            }
            Location location = new Location()
            {
                Name = name,
                Address = address,
                Url = url,
                Telephone = telephone
            };
            var newLocation = await _eventService.AddLocation(location);
            var locationList = await _eventService.GetLocations();
            return Json(new SelectList(locationList, "Id", "Name", newLocation.Id));
        }
    }
}
