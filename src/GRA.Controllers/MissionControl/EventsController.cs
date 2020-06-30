﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Events;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Domain.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageEvents)]
    public class EventsController : Base.MCController
    {
        private readonly ILogger<EventsController> _logger;
        private readonly BadgeService _badgeService;
        private readonly EventImportService _eventImportService;
        private readonly EventService _eventService;
        private readonly SiteService _siteService;
        private readonly SpatialService _spatialService;
        private readonly TriggerService _triggerService;
        private readonly UserService _userService;

        public EventsController(ILogger<EventsController> logger,
            ServiceFacade.Controller context,
            BadgeService badgeService,
            EventImportService eventImportService,
            EventService eventService,
            SiteService siteService,
            TriggerService triggerService,
            SpatialService spatialService,
            UserService userService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            _eventImportService = eventImportService
                ?? throw new ArgumentNullException(nameof(eventImportService));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _spatialService = spatialService
                ?? throw new ArgumentNullException(nameof(spatialService));
            _triggerService = triggerService
                ?? throw new ArgumentNullException(nameof(triggerService));
            _userService = userService
                ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = "Events";
        }

        public async Task<IActionResult> Index(string search,
            int? systemId, int? branchId, bool? mine, int? programId, int page = 1)
        {
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                ShowAlertWarning($"Events will not be seen becuase all event requests will be <a href=\"{site.ExternalEventListUrl}\"> redirected to another site</a>.");
            }

            try
            {
                var viewModel = await GetEventList(0, search, systemId, branchId, mine, programId, page);

                if (viewModel.PaginateModel.PastMaxPage)
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
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                ShowAlertWarning($"Events will not be seen becuase all event requests will be <a href=\"{site.ExternalEventListUrl}\"> redirected to another site</a>.");
            }

            try
            {
                var viewModel = await GetEventList(1, search, systemId, branchId, mine, programId, page);

                if (viewModel.PaginateModel.PastMaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            page = viewModel.PaginateModel.LastPage ?? 1
                        });
                }

                viewModel.CommunityExperience = true;
                PageTitle = "Community Experiences";
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid event filter by User {GetId(ClaimType.UserId)}: {ex}");
                ShowAlertDanger("Invalid filter parameters.");
                return RedirectToAction("CommunityExperiences");
            }
        }

        public async Task<IActionResult> StreamingEvents(string search,
            int? systemId, int? branchId, bool? mine, int? programId, int page = 1)
        {
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                ShowAlertWarning($"Events will not be seen becuase all event requests will be <a href=\"{site.ExternalEventListUrl}\"> redirected to another site</a>.");
            }

            try
            {
                var viewModel
                    = await GetEventList(2, search, systemId, branchId, mine, programId, page);

                if (viewModel.PaginateModel.PastMaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            page = viewModel.PaginateModel.LastPage ?? 1
                        });
                }

                viewModel.Streaming = true;
                PageTitle = "Streaming Events";
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
            var filter = new EventFilter(page)
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

            var paginateModel = new PaginateViewModel
            {
                ItemCount = eventList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);

            var viewModel = new EventsListViewModel
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
                CanManageLocations = UserHasPermission(Permission.ManageLocations),
                RequireSecretCode = await GetSiteSettingBoolAsync(
                    SiteSettingKey.Events.RequireBadge)
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
                    .SingleOrDefault(_ => _.Id == branch.SystemId)?.Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (systemId.HasValue)
            {
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == systemId.Value)?.Name;
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

        public async Task<IActionResult> Create(int? id,
            bool communityExperience = false,
            bool streamingEvent = false)
        {
            PageTitle = "Create Event";

            if (communityExperience)
            {
                PageTitle = "Create Community Experience";
            }
            else if (streamingEvent)
            {
                PageTitle = "Create Streaming Event";
            }

            var requireSecretCode = await GetSiteSettingBoolAsync(
                    SiteSettingKey.Events.RequireBadge);
            var programList = await _siteService.GetProgramList();

            var viewModel = new EventsDetailViewModel
            {
                CanAddSecretCode = requireSecretCode
                    || UserHasPermission(Permission.ManageTriggers),
                CanManageLocations = UserHasPermission(Permission.ManageLocations),
                CanRelateChallenge = UserHasPermission(Permission.ViewAllChallenges),
                ProgramList = new SelectList(programList, "Id", "Name"),
                IncludeSecretCode = requireSecretCode,
                RequireSecretCode = requireSecretCode
            };

            if (!streamingEvent)
            {
                viewModel.SystemList
                    = new SelectList(await _siteService.GetSystemList(true), "Id", "Name");
                viewModel.LocationList
                    = new SelectList(await _eventService.GetLocations(), "Id", "Name");
            }

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

            if (viewModel.BranchList == null && !streamingEvent)
            {
                viewModel.SystemId = GetId(ClaimType.SystemId);
                viewModel.BranchList = new SelectList(await _siteService
                    .GetBranches(viewModel.SystemId, true), "Id", "Name");
            }
            if (communityExperience)
            {
                viewModel.NewCommunityExperience = true;
                viewModel.UseLocation = true;
            }

            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingStringAsync(
                GetCurrentSiteId(), SiteSettingKey.Events.GoogleMapsAPIKey);
            viewModel.ShowGeolocation = IsSet;
            viewModel.GoogleMapsAPIKey = SetValue;

            return streamingEvent
                ? View("AddEditStreaming", viewModel)
                : View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventsDetailViewModel model)
        {
            var requireSecretCode = await GetSiteSettingBoolAsync(
                SiteSettingKey.Events.RequireBadge);

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
            if (model.IncludeSecretCode || requireSecretCode)
            {
                if (string.IsNullOrWhiteSpace(model.BadgeMakerImage) && model.BadgeUploadImage == null)
                {
                    ModelState.AddModelError("BadgemakerImage", "A badge is required.");
                }
                else if (model.BadgeUploadImage != null
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker)
                    && (!ValidImageExtensions.Contains(Path.GetExtension(model.BadgeUploadImage.FileName).ToLower())))
                {
                    ModelState.AddModelError("BadgeUploadImage", $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
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
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                            && (model.BadgeUploadImage == null || model.UseBadgeMaker))
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
                        var newBadge = new Badge
                        {
                            Filename = filename
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        var trigger = new Trigger
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
            model.RequireSecretCode = requireSecretCode;

            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingStringAsync(
                GetCurrentSiteId(), SiteSettingKey.Events.GoogleMapsAPIKey);
            model.ShowGeolocation = IsSet;
            model.GoogleMapsAPIKey = SetValue;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditStreaming(EventsDetailViewModel model)
        {
            var requireSecretCode = await GetSiteSettingBoolAsync(
                SiteSettingKey.Events.RequireBadge);

            if (model.Event.StartDate >= model.Event.StreamingAccessEnds)
            {
                ModelState.AddModelError("Event.StreamingAccessEnds", "The streaming access end time cannot be before the start time");
            }

            if (!model.Editing && (model.IncludeSecretCode || requireSecretCode))
            {
                if (string.IsNullOrWhiteSpace(model.BadgeMakerImage) && model.BadgeUploadImage == null)
                {
                    ModelState.AddModelError("BadgemakerImage", "A badge is required.");
                }
                else if (model.BadgeUploadImage != null
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker)
                    && (!ValidImageExtensions.Contains(Path.GetExtension(model.BadgeUploadImage.FileName).ToLower())))
                {
                    ModelState.AddModelError("BadgeUploadImage", $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                }
            }
            else
            {
                ModelState.Remove(nameof(model.SecretCode));
                ModelState.Remove(nameof(model.AwardMessage));
                ModelState.Remove(nameof(model.AwardPoints));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Event.ChallengeId.HasValue)
                    {
                        model.Event.ChallengeGroupId = null;
                    }

                    int? triggerId = null;
                    if (model.IncludeSecretCode && !model.Editing)
                    {
                        byte[] badgeBytes;
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                            && (model.BadgeUploadImage == null || model.UseBadgeMaker))
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
                        var newBadge = new Badge
                        {
                            Filename = filename
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        var trigger = new Trigger
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
                    graEvent.IsActive = true;
                    graEvent.IsValid = true;

                    // this will make streaming events play nice with the rest of the event code
                    // in future we will differentiate between event end and streaming access ends
                    graEvent.EndDate = graEvent.StreamingAccessEnds;

                    if (triggerId.HasValue)
                    {
                        graEvent.RelatedTriggerId = triggerId;
                    }

                    if (model.Editing)
                    {
                        await _eventService.Edit(graEvent);
                        ShowAlertSuccess($"Streaming event '{graEvent.Name}' edited.");
                    }
                    else
                    {
                        await _eventService.Add(graEvent);
                        ShowAlertSuccess($"Streaming event '{graEvent.Name}' created.");
                    }

                    return RedirectToAction("StreamingEvents");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Could not create streaming event: ", gex.Message);
                }
            }

            var programList = await _siteService.GetProgramList();

            model.ProgramList = new SelectList(programList, "Id", "Name");
            model.RequireSecretCode = requireSecretCode;

            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingStringAsync(
                GetCurrentSiteId(), SiteSettingKey.Events.GoogleMapsAPIKey);
            model.ShowGeolocation = IsSet;
            model.GoogleMapsAPIKey = SetValue;

            if (model.Editing)
            {
                PageTitle = "Edit Streaming Event";
                if (model.Event.CreatedBy != default)
                {
                    model.CreatedByName
                        = await _userService.GetUsersNameByIdAsync(model.Event.CreatedBy);
                }
                model.Event = await _eventService.GetRelatedChallengeDetails(model.Event, true);
            }
            else
            {
                PageTitle = "Create Streaming Event";
            }

            return View(model);
        }

        public async Task<IActionResult> EditStreaming(int id)
        {
            try
            {
                var graEvent = await _eventService.GetDetails(id, true);

                if (!graEvent.IsStreaming)
                {
                    return RedirectToAction("Edit", new { id });
                }

                PageTitle = "Edit Streaming Event";

                var programList = await _siteService.GetProgramList();
                var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingStringAsync(
                    GetCurrentSiteId(), SiteSettingKey.Events.GoogleMapsAPIKey);

                var model = new EventsDetailViewModel
                {
                    Editing = true,
                    Event = graEvent,
                    CreatedByName = await _userService.GetUsersNameByIdAsync(graEvent.CreatedBy),
                    CanViewParticipants = UserHasPermission(Permission.ViewParticipantDetails),
                    CanAddSecretCode = UserHasPermission(Permission.ManageTriggers),
                    CanEditGroups = UserHasPermission(Permission.EditChallengeGroups),
                    CanManageLocations = UserHasPermission(Permission.ManageLocations),
                    CanRelateChallenge = UserHasPermission(Permission.ViewAllChallenges),
                    ProgramList = new SelectList(programList, "Id", "Name"),
                    RequireSecretCode = await GetSiteSettingBoolAsync(
                        SiteSettingKey.Events.RequireBadge),

                    ShowGeolocation = IsSet,
                    GoogleMapsAPIKey = SetValue,
                };

                model.Event = await _eventService.GetRelatedChallengeDetails(model.Event, true);

                if (graEvent.Challenge?.BadgeId != null)
                {
                    var badge = await _badgeService.GetByIdAsync(graEvent.Challenge.BadgeId.Value);
                    graEvent.Challenge.BadgeFilename = _pathResolver
                        .ResolveContentPath(badge.Filename);
                }

                return View("AddEditStreaming", model);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view event/community experience: ", gex);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var graEvent = await _eventService.GetDetails(id, true);

                if (graEvent.IsStreaming)
                {
                    return RedirectToAction("EditStreaming", new { id });
                }

                PageTitle = graEvent.IsCommunityExperience
                    ? "Edit Community Experience"
                    : "Edit Event";

                var systemList = await _siteService.GetSystemList(true);
                var branchList = await _siteService.GetBranches(GetId(ClaimType.SystemId));
                var locationList = await _eventService.GetLocations();
                var programList = await _siteService.GetProgramList();
                var viewModel = new EventsDetailViewModel
                {
                    Event = graEvent,
                    CreatedByName = await _userService.GetUsersNameByIdAsync(graEvent.CreatedBy),
                    CanViewParticipants = UserHasPermission(Permission.ViewParticipantDetails),
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

                var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingStringAsync(
                    GetCurrentSiteId(), SiteSettingKey.Events.GoogleMapsAPIKey);
                viewModel.ShowGeolocation = IsSet;
                viewModel.GoogleMapsAPIKey = SetValue;

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view event/community experience: ", gex);
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

            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingStringAsync(
                GetCurrentSiteId(), SiteSettingKey.Events.GoogleMapsAPIKey);
            model.ShowGeolocation = IsSet;
            model.GoogleMapsAPIKey = SetValue;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, bool communityExperience = false)
        {
            try
            {
                var deletedText = "Event";
                var requireSecretCode = await GetSiteSettingBoolAsync(
                SiteSettingKey.Events.RequireBadge);
                if (requireSecretCode)
                {
                    var graEvent = await _eventService.GetDetails(id);
                    await _triggerService.RemoveAsync(graEvent.RelatedTriggerId.Value);
                    deletedText += " and its related trigger";
                }
                deletedText += " were successfully deleted!";
                ShowAlertSuccess(deletedText);
                await _eventService.Remove(id);
            }
            catch (GraException gex)
            {
                if (gex.Data?.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (DictionaryEntry trigger in gex.Data)
                    {
                        sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                            "<a href=\"{0}\" target=\"_blank\">{1}</a>, ",
                            Url.Action(nameof(TriggersController.Edit),
                                new { controller = TriggersController.Name, id = trigger.Key }),
                            trigger.Value);
                    }
                    ShowAlertWarning("Unable to delete event due to these trigger(s): ",
                        sb.ToString().Trim(' ').Trim(','));
                }
                else
                {
                    ShowAlertWarning("Unable to delete event: ", gex.Message);
                }
            }

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
            var filter = new BaseFilter(page);

            var locationList = await _eventService.GetPaginatedLocationsListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = locationList.Count,
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

            var viewModel = new LocationsListViewModel
            {
                Locations = locationList.Data,
                PaginateModel = paginateModel,
            };

            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingStringAsync(
                GetCurrentSiteId(), SiteSettingKey.Events.GoogleMapsAPIKey);
            viewModel.ShowGeolocation = IsSet;
            viewModel.GoogleMapsAPIKey = SetValue;

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
                    model.Location.Geolocation = null;
                    var location = await _eventService.AddLocationAsync(model.Location);
                    ShowAlertSuccess($"Added Location '{model.Location.Name}'");

                    if (await _siteLookupService.IsSiteSettingSetAsync(GetCurrentSiteId(),
                    SiteSettingKey.Events.GoogleMapsAPIKey))
                    {
                        var result = await _spatialService
                            .GetGeocodedAddressAsync(location.Address);
                        if (result.Status == ServiceResultStatus.Success)
                        {
                            location.Geolocation = result.Data;
                            await _eventService.UpdateLocationAsync(location);
                        }
                        else if (result.Status == ServiceResultStatus.Warning)
                        {
                            ShowAlertWarning("Unable to set location geolocation: ", result.Message);
                        }
                        else
                        {
                            ShowAlertDanger("Unable to set location geolocation: ", result.Message);
                        }
                    }
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
                    model.Location.Url = new UriBuilder(model.Location.Url).Uri.AbsoluteUri;
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
                    if (await _siteLookupService.IsSiteSettingSetAsync(GetCurrentSiteId(),
                    SiteSettingKey.Events.GoogleMapsAPIKey))
                    {
                        model.Location.Geolocation = null;
                        var newAddress = model.Location.Address?.Trim();
                        var currentLocation = await _eventService
                            .GetLocationByIdAsync(model.Location.Id);
                        if (string.IsNullOrWhiteSpace(currentLocation.Geolocation)
                            || !string.Equals(currentLocation.Address, newAddress,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            var result = await _spatialService
                                .GetGeocodedAddressAsync(newAddress);
                            if (result.Status == ServiceResultStatus.Success)
                            {
                                model.Location.Geolocation = result.Data;
                            }
                            else if (result.Status == ServiceResultStatus.Warning)
                            {
                                ShowAlertWarning("Unable to set location geolocation: ", result.Message);
                            }
                            else
                            {
                                ShowAlertDanger("Unable to set location geolocation: ", result.Message);
                            }
                        }
                    }

                    await _eventService.UpdateLocationAsync(model.Location);
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
                await _eventService.RemoveLocationAsync(id);
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
            var location = new Location
            {
                Name = name,
                Address = address,
                Url = url,
                Telephone = telephone
            };
            location = await _eventService.AddLocationAsync(location);

            string message = null;
            if (await _siteLookupService.IsSiteSettingSetAsync(GetCurrentSiteId(),
                    SiteSettingKey.Events.GoogleMapsAPIKey))
            {
                var result = await _spatialService
                        .GetGeocodedAddressAsync(location.Address);
                if (result.Status == ServiceResultStatus.Success)
                {
                    location.Geolocation = result.Data;
                    await _eventService.UpdateLocationAsync(location);
                }
                else
                {
                    message = result.Message;
                }
            }

            var locationList = await _eventService.GetLocations();
            return Json(new
            {
                success = true,
                data = new SelectList(locationList, "Id", "Name", location.Id),
                message
            });
        }

        [HttpPost]
        public async Task<JsonResult> SetLocationGeolocation(int id)
        {
            var success = false;
            var message = string.Empty;

            if (await _siteLookupService.IsSiteSettingSetAsync(GetCurrentSiteId(),
                    SiteSettingKey.Events.GoogleMapsAPIKey))
            {
                var location = await _eventService.GetLocationByIdAsync(id);
                if (string.IsNullOrEmpty(location.Geolocation))
                {
                    var result = await _spatialService
                            .GetGeocodedAddressAsync(location.Address);
                    if (result.Status == ServiceResultStatus.Success)
                    {
                        location.Geolocation = result.Data;
                        await _eventService.UpdateLocationAsync(location);

                        success = true;
                    }
                    else
                    {
                        message = result.Message;
                    }
                }
                else
                {
                    message = "Geolocation is already set.";
                }
            }
            else
            {
                message = "Geolocation is not set up.";
            }

            return Json(new
            {
                success,
                message
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
                || !ValidCsvExtensions.Contains(Path.GetExtension(eventFileCsv.FileName).ToLower()))
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
                        case ImportStatus.Warning:
                            AlertWarning = message;
                            break;
                        case ImportStatus.Danger:
                            AlertDanger = message;
                            break;
                        default:
                            AlertInfo = message;
                            break;
                    }
                }
            }

            return RedirectToAction("Import");
        }
    }
}
