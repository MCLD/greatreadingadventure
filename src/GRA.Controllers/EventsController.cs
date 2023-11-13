using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.Events;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Domain.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class EventsController : Base.UserController
    {
        public const string VisitedAll = "All";
        public const string VisitedNo = "No";
        public const string VisitedYes = "Yes";

        private readonly ActivityService _activityService;
        private readonly EventService _eventService;
        private readonly ILogger<EventsController> _logger;
        private readonly SiteService _siteService;
        private readonly SpatialService _spatialService;
        private readonly UserService _userService;

        public EventsController(ILogger<EventsController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            EventService eventService,
            SiteService siteService,
            SpatialService spatialService,
            UserService userService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activityService = activityService
                ?? throw new ArgumentNullException(nameof(activityService));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _spatialService = spatialService
                ?? throw new ArgumentNullException(nameof(spatialService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = _sharedLocalizer[Annotations.Title.Events];
        }

        public static string Name
        { get { return "Events"; } }

        public async Task<IActionResult> CommunityExperiences(int page = 1,
            string sort = null,
            string search = null,
            string near = null,
            int? system = null,
            int? branch = null,
            int? location = null,
            int? program = null,
            string StartDate = null,
            string EndDate = null,
            bool Favorites = false,
            string Status = null)
        {
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                return new RedirectResult(site.ExternalEventListUrl);
            }

            PageTitle = _sharedLocalizer[Annotations.Title.CommunityExperiences];
            return await Index(page,
                sort,
                search,
                near,
                system,
                branch,
                location,
                program,
                StartDate,
                EndDate,
                Favorites,
                Status,
                EventType.CommunityExperience);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                return new RedirectResult(site.ExternalEventListUrl);
            }

            try
            {
                var viewModel = new EventsDetailViewModel
                {
                    IsAuthenticated = AuthUser.Identity.IsAuthenticated,
                    Event = await _eventService.GetDetails(id)
                };

                PageTitle = _sharedLocalizer[Annotations.Interface.DateAtTime,
                    viewModel.Event.Name,
                    viewModel.Event.EventLocationName];

                if (!string.IsNullOrEmpty(viewModel.Event.EventLocationName)
                    && !string.IsNullOrEmpty(viewModel.Event.EventLocationAddress))
                {
                    viewModel.ShowStructuredData = true;
                    if (viewModel.Event.AllDay)
                    {
                        viewModel.EventStart = viewModel.Event.StartDate.ToString("yyyy-MM-dd",
                            CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        viewModel.EventStart = viewModel.Event.StartDate.ToString("s",
                            CultureInfo.InvariantCulture);
                        if (viewModel.Event.EndDate != null)
                        {
                            var endDate = (DateTime)viewModel.Event.EndDate;
                            viewModel.EventEnd = endDate.ToString("s",
                                CultureInfo.InvariantCulture);
                        }
                    }
                }

                if (viewModel.Event.ProgramId.HasValue)
                {
                    var program
                        = await _siteService.GetProgramByIdAsync(viewModel.Event.ProgramId.Value);
                    viewModel.ProgramString
                        = _sharedLocalizer[Annotations.Info.EventLimitedToProgram, program.Name];
                }
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning(gex.Message);
                return await Index(httpStatus: HttpStatusCode.NotFound);
            }
        }

        public async Task<IActionResult> GetDetails(int eventId)
        {
            try
            {
                var viewModel = new EventsDetailViewModel
                {
                    IsAuthenticated = AuthUser.Identity.IsAuthenticated,
                    Event = await _eventService.GetDetails(eventId)
                };

                if (viewModel.Event.ProgramId.HasValue)
                {
                    var program
                        = await _siteService.GetProgramByIdAsync(viewModel.Event.ProgramId.Value);
                    viewModel.ProgramString
                        = _sharedLocalizer[Annotations.Info.EventLimitedToProgram, program.Name];
                }

                return PartialView("_DetailPartial", viewModel);
            }
            catch (GraException gex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Content(gex.Message);
            }
        }

        public async Task<IActionResult> Index(int page = 1,
            string sort = null,
            string search = null,
            string near = null,
            int? system = null,
            int? branch = null,
            int? location = null,
            int? program = null,
            string StartDate = null,
            string EndDate = null,
            bool Favorites = false,
            string Visited = null,
            EventType eventType = EventType.Event,
            HttpStatusCode httpStatus = HttpStatusCode.OK)
        {
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                return new RedirectResult(site.ExternalEventListUrl);
            }

            ModelState.Clear();
            var filter = new EventFilter(page)
            {
                Search = search,
                EventType = (int)eventType
            };

            var nearSearchEnabled = await _siteLookupService
                .IsSiteSettingSetAsync(site.Id, SiteSettingKey.Events.GoogleMapsAPIKey);

            if (!string.IsNullOrWhiteSpace(sort) && Enum.IsDefined(typeof(SortEventsBy), sort))
            {
                filter.SortBy = (SortEventsBy)Enum.Parse(typeof(SortEventsBy), sort);
            }
            else
            {
                if (nearSearchEnabled && !string.IsNullOrWhiteSpace(near))
                {
                    filter.SortBy = SortEventsBy.Distance;
                }
            }
            if (AuthUser.Identity.IsAuthenticated)
            {
                filter.Favorites = Favorites;
                if (string.IsNullOrWhiteSpace(Visited)
                    || string.Equals(Visited, VisitedNo, StringComparison.OrdinalIgnoreCase))
                {
                    filter.IsAttended = false;
                }
                else if (string.Equals(Visited, VisitedYes, StringComparison.OrdinalIgnoreCase))
                {
                    filter.IsAttended = true;
                }
            }
            if (nearSearchEnabled)
            {
                if (!string.IsNullOrWhiteSpace(near))
                {
                    var geocodeResult = await _spatialService.GetGeocodedAddressAsync(near);
                    if (geocodeResult.Status == ServiceResultStatus.Success)
                    {
                        filter.SpatialDistanceHeaderId = await _spatialService
                            .GetSpatialDistanceIdForGeolocationAsync(geocodeResult.Data);
                    }
                    else
                    {
                        ShowAlertWarning("Not able to find that location.");
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            else
            {
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
            }

            if (program.HasValue)
            {
                filter.ProgramIds = new List<int?>() { program.Value };
            }

            if (!string.Equals(StartDate, "False", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    if (DateTime.TryParse(StartDate, out var startDate))
                    {
                        filter.StartDate = startDate.Date;
                    }
                }
                else
                {
                    filter.StartDate = _dateTimeProvider.Now.Date;
                }
            }

            if (!string.IsNullOrWhiteSpace(EndDate) && DateTime.TryParse(EndDate, out var endDate))
            {
                filter.EndDate = endDate.Date;
            }
            var eventList = await _eventService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = eventList.Count,
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

            var viewModel = new EventsListViewModel
            {
                IsAuthenticated = AuthUser.Identity.IsAuthenticated,
                Events = eventList.Data.ToList(),
                PaginateModel = paginateModel,
                Sort = filter.SortBy.ToString(),
                Search = search,
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                Favorites = Favorites,
                IsLoggedIn = AuthUser.Identity.IsAuthenticated,
                ProgramId = program,
                ProgramList = new SelectList(await _siteService.GetProgramList(), "Id", "Name"),
                EventType = eventType,
                ShowNearSearch = nearSearchEnabled,
            };

            if (nearSearchEnabled)
            {
                viewModel.Near = near?.Trim();

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var user = await _userService.GetDetails(GetActiveUserId());
                    if (!string.IsNullOrWhiteSpace(user.PostalCode))
                    {
                        viewModel.UserZipCode = user.PostalCode;
                    }
                }
            }
            else
            {
                viewModel.SystemList = new SelectList(
                    await _siteService.GetSystemList(), "Id", "Name");
                viewModel.LocationList = new SelectList(
                    await _eventService.GetLocations(), "Id", "Name");

                if (branch.HasValue)
                {
                    var selectedBranch = await _siteService.GetBranchByIdAsync(branch.Value);
                    viewModel.SystemId = selectedBranch.SystemId;
                    viewModel.BranchList = new SelectList(
                        await _siteService.GetBranches(selectedBranch.SystemId),
                        "Id", "Name", branch.Value);
                }
                else if (system.HasValue)
                {
                    viewModel.SystemId = system;
                    viewModel.BranchList = new SelectList(
                        await _siteService.GetBranches(system.Value), "Id", "Name");
                }
                else
                {
                    viewModel.BranchList = new SelectList(await _siteService.GetAllBranches(),
                        "Id", "Name");
                }

                if (location.HasValue && !branch.HasValue)
                {
                    viewModel.LocationId = location.Value;
                    viewModel.UseLocation = true;
                }
            }

            var (descriptionTextSet, communityExperienceDescription) = await _siteLookupService
                .GetSiteSettingStringAsync(site.Id,
                    SiteSettingKey.Events.CommunityExperienceDescription);

            if (descriptionTextSet)
            {
                viewModel.CommunityExperienceDescription = communityExperienceDescription;
            }

            if (eventType == EventType.StreamingEvent)
            {
                viewModel.Viewed = Visited;
            }
            else
            {
                viewModel.Visited = Visited;
            }

            if (httpStatus != HttpStatusCode.OK)
            {
                Response.StatusCode = (int)httpStatus;
            }

            return View(nameof(Index), viewModel);
        }

        [HttpPost]
        public IActionResult Index(EventsListViewModel model, bool keepPage = false)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            string startDate = "False";
            string endDate = null;
            string visited = null;

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

            if (model.EventType == EventType.StreamingEvent)
            {
                visited = model.Viewed;
            }
            else
            {
                visited = model.Visited;
            }

            int? page = null;
            if (keepPage && model.PaginateModel?.CurrentPage > 1)
            {
                page = model.PaginateModel.CurrentPage;
            }

            return RedirectToAction(nameof(Index), new
            {
                page,
                model.Sort,
                model.Search,
                model.Near,
                System = model.SystemId,
                Branch = model.BranchId,
                Location = model.LocationId,
                Program = model.ProgramId,
                StartDate = startDate,
                EndDate = endDate,
                model.Favorites,
                visited,
                model.EventType
            });
        }

        public async Task<IActionResult> Stream(int id)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                AlertInfo = _sharedLocalizer[Annotations.Interface.SignInForStreams];
                return RedirectToSignIn();
            }

            Event graEvent = null;
            try
            {
                graEvent = await _eventService.GetDetails(id);
            }
            catch (GraException) { }

            if (graEvent == null
                && Request.Query.ContainsKey("eventId")
                && int.TryParse(Request.Query["eventId"], out id))
            {
                try
                {
                    graEvent = await _eventService.GetDetails(id);
                }
                catch (GraException) { }
            }

            if (graEvent == null)
            {
                AlertWarning = _sharedLocalizer[Annotations.Interface.EventNotFound];
                return RedirectToAction(nameof(StreamingEvents));
            }

            if (!graEvent.IsStreaming)
            {
                return RedirectToAction(nameof(Detail), new { id });
            }
            else
            {
                if (_dateTimeProvider.Now < graEvent.StartDate)
                {
                    AlertWarning = _sharedLocalizer[Annotations.Interface.EventNotStarted];
                    return RedirectToAction(nameof(Detail), new { id });
                }
                else if (_dateTimeProvider.Now > graEvent.StreamingAccessEnds)
                {
                    AlertWarning = _sharedLocalizer[Annotations.Interface.EventHasEnded];
                    return RedirectToAction(nameof(Detail), new { id });
                }
                else
                {
                    if (graEvent.IsStreamingEmbed)
                    {
                        var viewModel = new StreamViewModel
                        {
                            EventName = graEvent.Name,
                            Embed = graEvent.StreamingLinkData,
                            EndDate = graEvent.EndDate
                        };

                        if (graEvent.RelatedTriggerId.HasValue
                            && await GetSiteSettingBoolAsync(SiteSettingKey.Events.StreamingShowCode))
                        {
                            viewModel.SecretCode = await _eventService
                                .GetSecretCodeForStreamingEventAsync(graEvent.Id);
                        }

                        return View(viewModel);
                    }
                    else
                    {
                        return Redirect(graEvent.StreamingLinkData);
                    }
                }
            }
        }

        public async Task<IActionResult> StreamingEvents(int page = 1,
                                                    string sort = null,
                    string search = null,
                    string near = null,
                    int? system = null,
                    int? branch = null,
                    int? location = null,
                    int? program = null,
                    string StartDate = null,
                    string EndDate = null,
                    bool Favorites = false,
                    string Status = null)
        {
            var site = await GetCurrentSiteAsync();
            if (!string.IsNullOrEmpty(site.ExternalEventListUrl))
            {
                return new RedirectResult(site.ExternalEventListUrl);
            }

            PageTitle = _sharedLocalizer[Annotations.Title.StreamingEvents];
            return await Index(page,
                sort,
                search,
                near,
                system,
                branch,
                location,
                program,
                StartDate,
                EndDate,
                Favorites,
                Status,
                EventType.StreamingEvent);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateFavorites(EventsListViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var serviceResult = await _activityService.UpdateFavoriteEvents(model.Events);
            if (serviceResult.Status == ServiceResultStatus.Warning
                        && !string.IsNullOrWhiteSpace(serviceResult.Message))
            {
                ShowAlertWarning(serviceResult.Message);
            }

            return Index(model, true);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateSingleFavorite(int eventId, bool favorite)
        {
            var serviceResult = new ServiceResult();
            var currEvent = await _eventService.GetDetails(eventId, true);
            try
            {
                var eventList = new List<Event>
                {
                    new Event
                    {
                        Id = eventId,
                        IsFavorited = favorite
                    }
                };
                serviceResult = await _activityService.UpdateFavoriteEvents(eventList);
            }
            catch (Exception ex)
            {
                if (currEvent.IsCommunityExperience)
                {
                    _logger.LogError(ex,
                        "Error updating user favorite community experience: {ErrorMessage}",
                        ex.Message);
                    serviceResult.Status = ServiceResultStatus.Error;
                    serviceResult.Message = "An error occured while trying to update the community experience.";
                }
                else
                {
                    _logger.LogError(ex,
                        "Error updating user favorite events: {ErrorMessage}",
                        ex.Message);
                    serviceResult.Status = ServiceResultStatus.Error;
                    serviceResult.Message = "An error occured while trying to update the event.";
                }
            }
            return Json(new
            {
                success = serviceResult.Status == ServiceResultStatus.Success,
                message = serviceResult.Message,
                favorite
            });
        }
    }
}
