using System;
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
        private readonly BadgeService _badgeService;
        private readonly EventImportService _eventImportService;
        private readonly EventService _eventService;
        private readonly ILogger<EventsController> _logger;
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
            ArgumentNullException.ThrowIfNull(badgeService);
            ArgumentNullException.ThrowIfNull(eventImportService);
            ArgumentNullException.ThrowIfNull(eventService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(spatialService);
            ArgumentNullException.ThrowIfNull(triggerService);
            ArgumentNullException.ThrowIfNull(userService);

            _badgeService = badgeService;
            _eventImportService = eventImportService;
            _eventService = eventService;
            _logger = logger;
            _siteService = siteService;
            _spatialService = spatialService;
            _triggerService = triggerService;
            _userService = userService;

            PageTitle = "Events";
        }

        public static string Name
        {
            get
            {
                return "Events";
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEditStreaming(EventsDetailViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            byte[] badgeBytes = null;

            var requireSecretCode = await GetSiteSettingBoolAsync(
                SiteSettingKey.Events.RequireBadge);

            if (model.Event.StartDate >= model.Event.StreamingAccessEnds)
            {
                ModelState.AddModelError("Event.StreamingAccessEnds",
                    "The streaming access end time cannot be before the start time");
            }

            if (!model.Editing && (model.IncludeSecretCode || requireSecretCode))
            {
                if (string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                    && model.BadgeUploadImage == null)
                {
                    ModelState.AddModelError("BadgemakerImage", "A badge is required.");
                }
                else if (model.BadgeUploadImage != null
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker))
                {
                    if (!ValidImageExtensions.Contains(
                        Path.GetExtension(model.BadgeUploadImage.FileName),
                            StringComparer.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("BadgeUploadImage",
                            $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                    }

                    try
                    {
                        await using (var ms = new MemoryStream())
                        {
                            await model.BadgeUploadImage.CopyToAsync(ms);
                            badgeBytes = ms.ToArray();
                        }
                        await _badgeService.ValidateBadgeImageAsync(badgeBytes);
                    }
                    catch (GraException gex)
                    {
                        ModelState.AddModelError("BadgeUploadImage", gex.Message);
                    }
                    if (string.IsNullOrWhiteSpace(model.BadgeAltText))
                    {
                        ModelState.AddModelError("BadgeAltText",
                            "The badge's alternative text is required.");
                    }
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
                            if (badgeBytes == null)
                            {
                                await using var ms = new MemoryStream();
                                await model.BadgeUploadImage.CopyToAsync(ms);
                                badgeBytes = ms.ToArray();
                            }
                            filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        var newBadge = new Badge
                        {
                            Filename = filename,
                            AltText = model.BadgeAltText
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

        [Authorize(Policy = Policy.ManageLocations)]
        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationsListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (!string.IsNullOrWhiteSpace(model.Location.Url))
            {
                try
                {
                    model.Location.Url = new UriBuilder(
                        model.Location.Url).Uri.AbsoluteUri;
                }
                catch (UriFormatException ufex)
                {
                    ShowAlertDanger($"Invalid URL: {ufex.Message}");
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
                            ShowAlertWarning("Unable to set location geolocation: ",
                                result.Message);
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
        public async Task<JsonResult> AddLocationReturnList(string name,
            string address,
            string url,
            string telephone)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    url = new UriBuilder(url).Uri.AbsoluteUri;
                }
                catch (UriFormatException ufex)
                {
                    return Json(new { success = false, message = $"Invalid URL: {ufex.Message}" });
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

        public async Task<IActionResult> CommunityExperiences(string search,
            int? systemId, int? branchId, bool? mine, int? programId, int page = 1)
        {
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                ShowAlertWarning($"Events will not be seen because all event requests will be <a href=\"{site.ExternalEventListUrl}\"> redirected to another site</a>.");
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
                _logger.LogError(ex,
                    "Invalid event filter by User {UserId}: {ErrorMessage}",
                    GetId(ClaimType.UserId),
                    ex.Message);
                ShowAlertDanger("Invalid filter parameters.");
                return RedirectToAction("CommunityExperiences");
            }
        }

        public async Task<IActionResult> Create(int? id,
            bool communityExperience = false,
            bool streamingEvent = false)
        {
            var requireSecretCode = await GetSiteSettingBoolAsync(
                    SiteSettingKey.Events.RequireBadge);
            var programList = await _siteService.GetProgramList();

            var (maxPointLimitSet, maxPointLimit)
                = await _siteLookupService.GetSiteSettingIntAsync(GetCurrentSiteId(),
                    SiteSettingKey.Triggers.MaxPointsPerTrigger);

            var viewModel = new EventsDetailViewModel
            {
                CanAddSecretCode = requireSecretCode
                    || UserHasPermission(Permission.ManageTriggers),
                CanManageLocations = UserHasPermission(Permission.ManageLocations),
                CanRelateChallenge = UserHasPermission(Permission.ViewAllChallenges),
                IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits),
                IncludeSecretCode = requireSecretCode,
                MaxPointLimit = maxPointLimitSet ? maxPointLimit : null,
                ProgramList = new SelectList(programList, "Id", "Name"),
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
                var siteUrl = await _siteLookupService.GetSiteLinkAsync(site.Id);
                viewModel.BadgeMakerUrl = GetBadgeMakerUrl(siteUrl.ToString(),
                    site.FromEmailAddress);
                viewModel.UseBadgeMaker = await _siteLookupService.GetSiteSettingBoolAsync(site.Id,
                    SiteSettingKey.Badges.EnableBadgeMaker);
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
                        var branch = await _siteService
                            .GetBranchByIdAsync(graEvent.AtBranchId.Value);
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
                PageTitle = "Create Community Experience";
            }
            else if (streamingEvent)
            {
                PageTitle = "Create Streaming Event";
            }
            else
            {
                PageTitle = "Create Event";
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
            ArgumentNullException.ThrowIfNull(model);

            byte[] badgeBytes = null;

            var requireSecretCode
                = await GetSiteSettingBoolAsync(SiteSettingKey.Events.RequireBadge);
            var (maxPointLimitSet, maxPointLimit)
                = await GetSiteSettingIntAsync(SiteSettingKey.Triggers.MaxPointsPerTrigger);

            model.IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits);
            model.MaxPointLimit = maxPointLimitSet ? maxPointLimit : null;

            if (model.Event.AllDay)
            {
                if (model.Event.EndDate.HasValue && model.Event.StartDate > model.Event.EndDate)
                {
                    ModelState.AddModelError("Event.EndDate",
                        "The End date cannot be before the Start date");
                }
            }
            else
            {
                if (model.Event.EndDate.HasValue && model.Event.StartDate.TimeOfDay
                    > model.Event.EndDate.Value.TimeOfDay)
                {
                    ModelState.AddModelError("Event.EndDate",
                        "The End time cannot be before the Start time");
                }
            }
            if (model.UseLocation && !model.Event.AtLocationId.HasValue)
            {
                ModelState.AddModelError("Event.AtLocationId",
                    "The At Location field is required.");
            }
            if (!model.UseLocation && !model.Event.AtBranchId.HasValue)
            {
                ModelState.AddModelError("Event.AtBranchId", "The At Branch field is required.");
            }
            if (model.IncludeSecretCode || requireSecretCode)
            {
                if (string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                    && model.BadgeUploadImage == null)
                {
                    ModelState.AddModelError("BadgemakerImage", "A badge is required.");
                }
                else if (model.BadgeUploadImage != null
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker))
                {
                    if (!ValidImageExtensions.Contains(
                        Path.GetExtension(model.BadgeUploadImage.FileName),
                            StringComparer.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("BadgeUploadImage",
                            $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                    }

                    try
                    {
                        await using var ms = new MemoryStream();
                        await model.BadgeUploadImage.CopyToAsync(ms);
                        badgeBytes = ms.ToArray();

                        await _badgeService.ValidateBadgeImageAsync(badgeBytes);
                    }
                    catch (GraException gex)
                    {
                        ModelState.AddModelError("BadgeUploadImage", gex.Message);
                    }
                }
                if (!model.IgnorePointLimits
                    && model.MaxPointLimit.HasValue
                    && model.AwardPoints > model.MaxPointLimit)
                {
                    ModelState.AddModelError("AwardPoints",
                        $"You may award up to {model.MaxPointLimit} points.");
                }
                if (string.IsNullOrWhiteSpace(model.BadgeAltText))
                {
                    ModelState.AddModelError("BadgeAltText",
                        "The badge's alternative text is required.");
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
                catch (UriFormatException ufex)
                {
                    ModelState.AddModelError("Event.ExternalLink", $"Invalid URL: {ufex.Message}");
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
                            if (badgeBytes == null)
                            {
                                await using var ms = new MemoryStream();
                                await model.BadgeUploadImage.CopyToAsync(ms);
                                badgeBytes = ms.ToArray();
                            }
                            filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        var newBadge = new Badge
                        {
                            Filename = filename,
                            AltText = model.BadgeAltText
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

            var systemList = await _siteService.GetSystemList(true);
            var branchList = await _siteService.GetBranches(model.SystemId, true);
            var locationList = await _eventService.GetLocations();
            var programList = await _siteService.GetProgramList();

            model.SystemList = new SelectList(systemList, "Id", "Name");
            model.BranchList = new SelectList(branchList, "Id", "Name");
            model.LocationList = new SelectList(locationList, "Id", "Name");
            model.ProgramList = new SelectList(programList, "Id", "Name");
            model.RequireSecretCode = requireSecretCode;

            if (model.Event.IsCommunityExperience)
            {
                PageTitle = "Create Community Experience";
                model.NewCommunityExperience = true;
                model.UseLocation = true;
            }
            else
            {
                PageTitle = "Create Event";
            }

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
                var deletedText = new StringBuilder("Event");
                var requireSecretCode
                    = await GetSiteSettingBoolAsync(SiteSettingKey.Events.RequireBadge);
                if (requireSecretCode)
                {
                    var graEvent = await _eventService.GetDetails(id);
                    await _triggerService.RemoveAsync(graEvent.RelatedTriggerId.Value);
                    deletedText.Append(" and its related trigger");
                }
                int removedFavorites = await _eventService.Remove(id);
                if (removedFavorites > 0)
                {
                    deletedText.Append(" (including ")
                        .Append(removedFavorites)
                        .Append(" user favorite");
                    if (removedFavorites != 1)
                    {
                        deletedText.Append('s');
                    }
                    deletedText.Append(')');
                }
                deletedText.Append(" were successfully deleted.");
                ShowAlertSuccess(deletedText.ToString());
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
            ArgumentNullException.ThrowIfNull(model);

            if (model.Event.AllDay)
            {
                if (model.Event.EndDate.HasValue && model.Event.StartDate > model.Event.EndDate)
                {
                    ModelState.AddModelError("Event.EndDate",
                        "The End date cannot be before the Start date");
                }
            }
            else
            {
                if (model.Event.EndDate.HasValue && model.Event.StartDate.TimeOfDay
                    > model.Event.EndDate.Value.TimeOfDay)
                {
                    ModelState.AddModelError("Event.EndDate",
                        "The End time cannot be before the Start time");
                }
            }
            if (model.UseLocation && !model.Event.AtLocationId.HasValue)
            {
                ModelState.AddModelError("Event.AtLocationId",
                    "The At Location field is required.");
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
                catch (UriFormatException ufex)
                {
                    ModelState.AddModelError("Event.ExternalLink", $"Invalid URL: {ufex.Message}");
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

        [Authorize(Policy = Policy.ManageLocations)]
        [HttpPost]
        public async Task<IActionResult> EditLocation(LocationsListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (!string.IsNullOrWhiteSpace(model.Location.Url))
            {
                try
                {
                    model.Location.Url = new UriBuilder(model.Location.Url).Uri.AbsoluteUri;
                }
                catch (UriFormatException ufex)
                {
                    ShowAlertDanger($"Invalid URL: {ufex.Message}");
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
                                ShowAlertWarning("Unable to set location geolocation: ",
                                    result.Message);
                            }
                            else
                            {
                                ShowAlertDanger("Unable to set location geolocation: ",
                                    result.Message);
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
                || !ValidFiles.CsvExtensions.Contains(Path.GetExtension(eventFileCsv.FileName),
                    StringComparer.OrdinalIgnoreCase))
            {
                AlertDanger = "You must select a .csv file.";
                ModelState.AddModelError("eventFileCsv", "You must select a .csv file.");
            }

            if (eventFileCsv != null && ModelState.ErrorCount == 0)
            {
                using var streamReader = new StreamReader(eventFileCsv.OpenReadStream());
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

            return RedirectToAction("Import");
        }

        public async Task<IActionResult> Index(string search,
            int? systemId,
            int? branchId,
            bool? mine,
            int? programId,
            int page = 1)
        {
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                ShowAlertWarning($"Events will not be seen because all event requests will be <a href=\"{site.ExternalEventListUrl}\"> redirected to another site</a>.");
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
                _logger.LogError(ex,
                    "Invalid event filter by User {UserId}: {ErrorMessage}",
                    GetId(ClaimType.UserId),
                    ex.Message);
                ShowAlertDanger("Invalid filter parameters.");
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
                CanManageLocations = UserHasPermission(Permission.ManageLocations),
                Locations = locationList.Data,
                PaginateModel = paginateModel,
            };

            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingStringAsync(
                GetCurrentSiteId(), SiteSettingKey.Events.GoogleMapsAPIKey);
            viewModel.ShowGeolocation = IsSet;
            viewModel.GoogleMapsAPIKey = SetValue;

            return View(viewModel);
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

        public async Task<IActionResult> StreamingEvents(string search,
                            int? systemId,
                            int? branchId,
                            bool? mine,
                            int? programId,
                            int page = 1)
        {
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                ShowAlertWarning($"Events will not be seen because all event requests will be <a href=\"{site.ExternalEventListUrl}\"> redirected to another site</a>.");
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
                _logger.LogError(ex,
                    "Invalid event filter by User {UserId}: {ErrorMessage}",
                    GetId(ClaimType.UserId),
                    ex.Message);
                ShowAlertDanger("Invalid filter parameters.");
                return RedirectToAction("CommunityExperiences");
            }
        }

        private async Task<EventsListViewModel> GetEventList(int? eventType,
            string search,
            int? systemId,
            int? branchId,
            bool? mine,
            int? programId,
            int page = 1)
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
                    filter.ProgramIds = new List<int?> { programId.Value };
                }
                else
                {
                    filter.ProgramIds = new List<int?> { null };
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
                    var program = await _siteService.GetProgramByIdAsync(programId.Value);
                    viewModel.ProgramName = $"Limited to {program.Name}";
                }
                else
                {
                    viewModel.ProgramName = "Not Limited to a Program";
                }
            }
            else
            {
                viewModel.ProgramName = "All Programs";
            }

            return viewModel;
        }
    }
}
