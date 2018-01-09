using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.Filter;
using GRA.Controllers.ViewModel.MissionControl.Events;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageEvents)]
    [EventUrlFilter]
    public class EventsController : Base.MCController
    {
        private readonly ILogger<EventsController> _logger;
        private readonly BadgeService _badgeService;
        private readonly EventImportService _eventImportService;
        private readonly EventService _eventService;
        private readonly SiteService _siteService;
        private readonly TriggerService _triggerService;
        public EventsController(ILogger<EventsController> logger,
            ServiceFacade.Controller context,
            BadgeService badgeService,
            EventImportService eventImportService,
            EventService eventService,
            SiteService siteService,
            TriggerService triggerService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _badgeService = Require.IsNotNull(badgeService, nameof(badgeService));
            _eventImportService = Require.IsNotNull(eventImportService, nameof(eventImportService));
            _eventService = Require.IsNotNull(eventService, nameof(eventService));
            _siteService = Require.IsNotNull(siteService, nameof(SiteService));
            _triggerService = Require.IsNotNull(triggerService, nameof(TriggerService));
            PageTitle = "Events";
        }

        public async Task<IActionResult> Index(string search,
            int? systemId, int? branchId, bool? mine, int? programId, int page = 1)
        {
            try
            {
                var viewModel = await GetEventList(0, search, systemId, branchId, mine, programId, page);

                if (viewModel.PaginateModel.MaxPage > 0
                    && viewModel.PaginateModel.CurrentPage > viewModel.PaginateModel.MaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            page = viewModel.PaginateModel.LastPage ?? 1
                        });
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid event filter by User {GetId(ClaimType.UserId)}: {ex}");
                ShowAlertDanger("Invalid filter parameters.");
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> CommunityExperiences(string search,
            int? systemId, int? branchId, bool? mine, int? programId, int page = 1)
        {
            try
            {
                var viewModel = await GetEventList(1, search, systemId, branchId, mine, programId, page);

                if (viewModel.PaginateModel.MaxPage > 0
                    && viewModel.PaginateModel.CurrentPage > viewModel.PaginateModel.MaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            page = viewModel.PaginateModel.LastPage ?? 1
                        });
                }
                viewModel.CommunityExperience = true;
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid event filter by User {GetId(ClaimType.UserId)}: {ex}");
                ShowAlertDanger("Invalid filter parameters.");
                return RedirectToAction("CommunityExperiences");
            }
        }

        private async Task<EventsListViewModel> GetEventList(int? eventType, string search,
            int? systemId, int? branchId, bool? mine, int? programId, int page = 1)
        {
            EventFilter filter = new EventFilter(page)
            {
                EventType = eventType
            };

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

            return viewModel;
        }

        public async Task<IActionResult> Create(int? id, bool communityExperience = false)
        {
            PageTitle = "Create Event";

            var systemList = await _siteService.GetSystemList(true);
            var locationList = await _eventService.GetLocations();
            var programList = await _siteService.GetProgramList();
            EventsDetailViewModel viewModel = new EventsDetailViewModel()
            {
                CanAddSecretCode = UserHasPermission(Permission.ManageTriggers),
                CanManageLocations = UserHasPermission(Permission.ManageLocations),
                CanRelateChallenge = UserHasPermission(Permission.ViewAllChallenges),
                SystemList = new SelectList(systemList, "Id", "Name"),
                LocationList = new SelectList(locationList, "Id", "Name"),
                ProgramList = new SelectList(programList, "Id", "Name")
            };
            if (viewModel.CanAddSecretCode)
            {
                var site = await GetCurrentSiteAsync();
                var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
                viewModel.BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress);
                viewModel.UseBadgeMaker = true;
            }

            if (id.HasValue)
            {
                try
                {
                    var graEvent = await _eventService.GetDetails(id.Value,
                        viewModel.CanRelateChallenge);
                    if (!graEvent.ParentEventId.HasValue)
                    {
                        graEvent.ParentEventId = graEvent.Id;
                    }
                    viewModel.Event = graEvent;
                    viewModel.Event.RelatedTriggerId = null;
                    if (graEvent.AtBranchId.HasValue)
                    {
                        var branch = await _siteService.GetBranchByIdAsync(graEvent.AtBranchId.Value);
                        viewModel.SystemId = branch.SystemId;
                        viewModel.BranchList = new SelectList(await _siteService
                        .GetBranches(viewModel.SystemId, true), "Id", "Name");
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to copy event: ", gex);
                }
            }

            if (viewModel.BranchList == null)
            {
                viewModel.SystemId = GetId(ClaimType.SystemId);
                viewModel.BranchList = new SelectList(await _siteService
                    .GetBranches(viewModel.SystemId, true), "Id", "Name");
            }
            if (communityExperience)
            {
                viewModel.NewCommunityExperience = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventsDetailViewModel model)
        {
            if (model.Event.AllDay)
            {
                if (model.Event.EndDate.HasValue && model.Event.StartDate > model.Event.EndDate)
                {
                    ModelState.AddModelError("Event.EndDate", "The End date cannot be before the Start date");
                }
            }
            else
            {
                if (model.Event.EndDate.HasValue && model.Event.StartDate.TimeOfDay
                    > model.Event.EndDate.Value.TimeOfDay)
                {
                    ModelState.AddModelError("Event.EndDate", "The End time cannot be before the Start time");
                }
            }
            if (model.UseLocation && !model.Event.AtLocationId.HasValue)
            {
                ModelState.AddModelError("Event.AtLocationId", "The At Location field is required.");
            }
            if (!model.UseLocation && !model.Event.AtBranchId.HasValue)
            {
                ModelState.AddModelError("Event.AtBranchId", "The At Branch field is required.");
            }
            if (model.IncludeSecretCode)
            {
                if (string.IsNullOrWhiteSpace(model.BadgeMakerImage) && model.BadgeUploadImage == null)
                {
                    ModelState.AddModelError("BadgemakerImage", "A badge is required.");
                }
                else if (model.BadgeUploadImage != null
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker)
                    && (Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpg"
                        && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpeg"
                        && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".png"))
                {
                    ModelState.AddModelError("BadgeUploadImage", "Please use a .jpg or .png image.");
                }
            }
            else
            {
                ModelState.Remove(nameof(model.SecretCode));
                ModelState.Remove(nameof(model.AwardMessage));
                ModelState.Remove(nameof(model.AwardPoints));
            }

            if (!string.IsNullOrWhiteSpace(model.Event.ExternalLink))
            {
                try
                {
                    model.Event.ExternalLink = new UriBuilder(
                        model.Event.ExternalLink).Uri.AbsoluteUri;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Event.ExternalLink", "Invalid URL");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Event.ChallengeId.HasValue)
                    {
                        model.Event.ChallengeGroupId = null;
                    }
                    if (model.Event.AllDay)
                    {
                        model.Event.StartDate = model.Event.StartDate.Date;
                        if (model.Event.EndDate.HasValue)
                        {
                            if (model.Event.EndDate.Value.Date == model.Event.StartDate.Date)
                            {
                                model.Event.EndDate = null;
                            }
                            else
                            {
                                model.Event.EndDate = model.Event.EndDate.Value.Date;
                            }
                        }
                    }
                    else
                    {
                        if (model.Event.EndDate.HasValue)
                        {
                            if (model.Event.EndDate.Value.TimeOfDay == model.Event.StartDate.TimeOfDay)
                            {
                                model.Event.EndDate = null;
                            }
                            else
                            {
                                model.Event.EndDate = model.Event.StartDate.Date
                                    + model.Event.EndDate.Value.TimeOfDay;
                            }
                        }
                    }

                    int? triggerId = null;
                    if (model.IncludeSecretCode)
                    {
                        byte[] badgeBytes;
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) &&
                            (model.BadgeUploadImage == null || model.UseBadgeMaker))
                        {
                            var badgeString = model.BadgeMakerImage.Split(',').Last();
                            badgeBytes = Convert.FromBase64String(badgeString);
                            filename = "badge.png";
                        }
                        else
                        {
                            using (var fileStream = model.BadgeUploadImage.OpenReadStream())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    fileStream.CopyTo(ms);
                                    badgeBytes = ms.ToArray();
                                }
                            }
                            filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        Badge newBadge = new Badge()
                        {
                            Filename = filename
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        Trigger trigger = new Trigger
                        {
                            Name = $"Event '{model.Event.Name}' code",
                            SecretCode = model.SecretCode,
                            AwardMessage = model.AwardMessage,
                            AwardPoints = model.AwardPoints,
                            AwardBadgeId = badge.Id,
                        };
                        triggerId = (await _triggerService.AddAsync(trigger)).Id;
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

                    if (triggerId.HasValue)
                    {
                        graEvent.RelatedTriggerId = triggerId;
                    }

                    await _eventService.Add(graEvent);
                    ShowAlertSuccess($"Event '{graEvent.Name}' created.");
                    if (graEvent.IsCommunityExperience)
                    {
                        return RedirectToAction("CommunityExperiences");
                    }
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Could not create event: ", gex.Message);
                }
            }
            PageTitle = "Create Event";

            var systemList = await _siteService.GetSystemList(true);
            var branchList = await _siteService.GetBranches(model.SystemId, true);
            var locationList = await _eventService.GetLocations();
            var programList = await _siteService.GetProgramList();

            model.SystemList = new SelectList(systemList, "Id", "Name");
            model.BranchList = new SelectList(branchList, "Id", "Name");
            model.LocationList = new SelectList(locationList, "Id", "Name");
            model.ProgramList = new SelectList(programList, "Id", "Name");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            PageTitle = "Edit Event";

            try
            {
                var graEvent = await _eventService.GetDetails(id, true);
                var systemList = await _siteService.GetSystemList(true);
                var branchList = await _siteService.GetBranches(GetId(ClaimType.SystemId));
                var locationList = await _eventService.GetLocations();
                var programList = await _siteService.GetProgramList();
                EventsDetailViewModel viewModel = new EventsDetailViewModel()
                {
                    Event = graEvent,
                    UseLocation = graEvent.AtLocationId.HasValue,
                    CanAddSecretCode = UserHasPermission(Permission.ManageTriggers),
                    CanEditGroups = UserHasPermission(Permission.EditChallengeGroups),
                    CanManageLocations = UserHasPermission(Permission.ManageLocations),
                    CanRelateChallenge = UserHasPermission(Permission.ViewAllChallenges),
                    SystemList = new SelectList(systemList, "Id", "Name"),
                    BranchList = new SelectList(branchList, "Id", "Name"),
                    LocationList = new SelectList(locationList, "Id", "Name"),
                    ProgramList = new SelectList(programList, "Id", "Name")
                };

                if (graEvent.AtBranchId.HasValue)
                {
                    var branch = await _siteService.GetBranchByIdAsync(graEvent.AtBranchId.Value);
                    viewModel.SystemId = branch.SystemId;
                    viewModel.BranchList = new SelectList(await _siteService
                    .GetBranches(viewModel.SystemId, true), "Id", "Name");
                }
                else
                {
                    viewModel.SystemId = GetId(ClaimType.SystemId);
                    viewModel.BranchList = new SelectList(await _siteService
                        .GetBranches(viewModel.SystemId, true), "Id", "Name");
                }

                if (graEvent.Challenge?.BadgeId != null)
                {
                    var badge = await _badgeService.GetByIdAsync(graEvent.Challenge.BadgeId.Value);
                    graEvent.Challenge.BadgeFilename = _pathResolver
                        .ResolveContentPath(badge.Filename);
                }

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view event: ", gex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventsDetailViewModel model)
        {
            if (model.Event.AllDay)
            {
                if (model.Event.EndDate.HasValue && model.Event.StartDate > model.Event.EndDate)
                {
                    ModelState.AddModelError("Event.EndDate", "The End date cannot be before the Start date");
                }
            }
            else
            {
                if (model.Event.EndDate.HasValue && model.Event.StartDate.TimeOfDay
                    > model.Event.EndDate.Value.TimeOfDay)
                {
                    ModelState.AddModelError("Event.EndDate", "The End time cannot be before the Start time");
                }
            }
            if (model.UseLocation && !model.Event.AtLocationId.HasValue)
            {
                ModelState.AddModelError("Event.AtLocationId", "The At Location field is required.");
            }
            if (!model.UseLocation && !model.Event.AtBranchId.HasValue)
            {
                ModelState.AddModelError("Event.AtBranchId", "The At Branch field is required.");
            }

            if (!string.IsNullOrWhiteSpace(model.Event.ExternalLink))
            {
                try
                {
                    model.Event.ExternalLink = new UriBuilder(
                        model.Event.ExternalLink).Uri.AbsoluteUri;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Event.ExternalLink", "Invalid URL");
                }
            }

            ModelState.Remove(nameof(model.SecretCode));
            ModelState.Remove(nameof(model.AwardMessage));
            ModelState.Remove(nameof(model.AwardPoints));
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Event.ChallengeId.HasValue)
                    {
                        model.Event.ChallengeGroupId = null;
                    }
                    if (model.Event.AllDay)
                    {
                        model.Event.StartDate = model.Event.StartDate.Date;
                        if (model.Event.EndDate.HasValue)
                        {
                            if (model.Event.EndDate.Value.Date == model.Event.StartDate.Date)
                            {
                                model.Event.EndDate = null;
                            }
                            else
                            {
                                model.Event.EndDate = model.Event.EndDate.Value.Date;
                            }
                        }
                    }
                    else
                    {
                        if (model.Event.EndDate.HasValue)
                        {
                            if (model.Event.EndDate.Value.TimeOfDay == model.Event.StartDate.TimeOfDay)
                            {
                                model.Event.EndDate = null;
                            }
                            else
                            {
                                model.Event.EndDate = model.Event.StartDate.Date
                                    + model.Event.EndDate.Value.TimeOfDay;
                            }
                        }
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
                    if (graEvent.IsCommunityExperience)
                    {
                        return RedirectToAction("CommunityExperiences");
                    }
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Could not edit event: ", gex.Message);
                }
            }
            PageTitle = "Edit Event";

            model.Event = await _eventService.GetRelatedChallengeDetails(model.Event, true);

            var systemList = await _siteService.GetSystemList(true);
            var branchList = await _siteService.GetBranches(model.SystemId, true);
            var locationList = await _eventService.GetLocations();
            var programList = await _siteService.GetProgramList();

            model.SystemList = new SelectList(systemList, "Id", "Name");
            model.BranchList = new SelectList(branchList, "Id", "Name");
            model.LocationList = new SelectList(locationList, "Id", "Name");
            model.ProgramList = new SelectList(programList, "Id", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, bool communityExperience = false)
        {
            await _eventService.Remove(id);

            if (communityExperience)
            {
                return RedirectToAction("CommunityExperiences");
            }
            else
            {
                return RedirectToAction("Index");
            }
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
            if (!string.IsNullOrWhiteSpace(model.Location.Url))
            {
                try
                {
                    model.Location.Url = new UriBuilder(
                        model.Location.Url).Uri.AbsoluteUri;
                }
                catch (Exception)
                {
                    ShowAlertDanger("Invalid URL");
                    return RedirectToAction("Locations");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            if (!string.IsNullOrWhiteSpace(model.Location.Url))
            {
                try
                {
                    model.Location.Url = new UriBuilder(
                        model.Location.Url).Uri.AbsoluteUri;
                }
                catch (Exception)
                {
                    ShowAlertDanger("Invalid URL");
                    return RedirectToAction("Locations");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
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
                try
                {
                    url = new UriBuilder(url).Uri.AbsoluteUri;
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Invalid URL" });
                }
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
            return Json(new
            {
                success = true,
                data = new SelectList(locationList, "Id", "Name", newLocation.Id)
            });
        }

        [HttpGet]
        public IActionResult Import()
        {
            PageTitle = "Import Events";
            return View("Import");
        }

        [HttpPost]
        public async Task<IActionResult> Import(Microsoft.AspNetCore.Http.IFormFile eventFileCsv)
        {
            PageTitle = "Import Events";
            if (eventFileCsv == null
                || Path.GetExtension(eventFileCsv.FileName).ToLower() != ".csv")
            {
                AlertDanger = "You must select a .csv file.";
                ModelState.AddModelError("eventFileCsv", "You must select a .csv file.");
            }

            if (ModelState.ErrorCount == 0)
            {
                using (var streamReader = new StreamReader(eventFileCsv.OpenReadStream()))
                {
                    (ImportStatus status, string message)
                        = await _eventImportService.FromCsvAsync(streamReader);

                    switch (status)
                    {
                        case ImportStatus.Success:
                            AlertSuccess = message;
                            break;
                        case ImportStatus.Info:
                            AlertInfo = message;
                            break;
                        case ImportStatus.Warning:
                            AlertWarning = message;
                            break;
                        case ImportStatus.Danger:
                            AlertDanger = message;
                            break;
                    }
                }
            }
            return RedirectToAction("Import");
        }
    }
}
