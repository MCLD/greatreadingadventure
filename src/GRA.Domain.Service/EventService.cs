using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class EventService : BaseUserService<EventService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IDistributedCache _cache;
        private readonly IChallengeGroupRepository _challengeGroupRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISpatialDistanceRepository _spatialDistanceRepository;

        public EventService(ILogger<EventService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IDistributedCache cache,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IChallengeRepository challengeRepository,
            IChallengeGroupRepository challengeGroupRepository,
            IEventRepository eventRepository,
            ILocationRepository locationRepository,
            IProgramRepository programRepository,
            ISpatialDistanceRepository spatialDistanceRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _challengeRepository =
                challengeRepository
                ?? throw new ArgumentNullException(nameof(challengeRepository));
            _challengeGroupRepository = challengeGroupRepository
                ?? throw new ArgumentNullException(nameof(challengeGroupRepository));
            _eventRepository = eventRepository
                ?? throw new ArgumentNullException(nameof(eventRepository));
            _locationRepository = locationRepository
                ?? throw new ArgumentNullException(nameof(locationRepository));
            _programRepository = programRepository
                ?? throw new ArgumentNullException(nameof(programRepository));
            _spatialDistanceRepository = spatialDistanceRepository
                ?? throw new ArgumentNullException(nameof(spatialDistanceRepository));
        }

        public async Task<Event> Add(Event graEvent)
        {
            VerifyPermission(Permission.ManageEvents);
            graEvent.SiteId = GetCurrentSiteId();
            graEvent.RelatedBranchId = GetClaimId(ClaimType.BranchId);
            graEvent.RelatedSystemId = GetClaimId(ClaimType.SystemId);
            if (!HasPermission(Permission.ViewAllChallenges))
            {
                graEvent.ChallengeId = null;
                graEvent.ChallengeGroupId = null;
            }
            await ValidateEvent(graEvent);
            if (graEvent.IsStreaming)
            {
                await _cache.RemoveAsync(CacheKey.StreamingEvents);
            }
            return await _eventRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), graEvent);
        }

        public async Task<Location> AddLocationAsync(Location location)
        {
            VerifyPermission(Permission.ManageLocations);

            location.Address = location.Address.Trim();
            location.Name = location.Name.Trim();
            location.SiteId = GetCurrentSiteId();
            location.Telephone = location.Telephone?.Trim();
            location.Url = location.Url?.Trim();

            return await _locationRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), location);
        }

        public async Task<Event> Edit(Event graEvent)
        {
            VerifyPermission(Permission.ManageEvents);
            var currentEvent = await _eventRepository.GetByIdAsync(graEvent.Id);
            graEvent.SiteId = currentEvent.SiteId;
            if (!HasPermission(Permission.ViewAllChallenges))
            {
                graEvent.ChallengeId = currentEvent.ChallengeId;
                graEvent.ChallengeGroupId = currentEvent.ChallengeGroupId;
            }
            await ValidateEvent(graEvent);
            if (graEvent.IsStreaming)
            {
                await _cache.RemoveAsync(CacheKey.StreamingEvents);
            }
            return await _eventRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), graEvent);
        }

        public async Task<List<Event>> GetByChallengeGroupIdAsync(int challengeGroupId)
        {
            return await _eventRepository.GetByChallengeGroupIdAsync(challengeGroupId);
        }

        public async Task<List<Event>> GetByChallengeIdAsync(int challengeId)
        {
            return await _eventRepository.GetByChallengeIdAsync(challengeId);
        }

        public async Task<Event> GetDetails(int eventId, bool showInactiveChallenge = false)
        {
            var graEvent = await _eventRepository.GetByIdAsync(eventId);
            if (graEvent == null)
            {
                throw new GraException("Event not found.");
            }

            if (graEvent.AtBranchId != null)
            {
                var branch = await _branchRepository.GetByIdAsync((int)graEvent.AtBranchId);
                graEvent.EventLocationName = branch.Name;
                graEvent.EventLocationAddress = branch.Address;
                graEvent.EventLocationTelephone = branch.Telephone;
                graEvent.EventLocationLink = branch.Url;
            }
            if (graEvent.AtLocationId != null)
            {
                var location = await _locationRepository.GetByIdAsync((int)graEvent.AtLocationId);
                graEvent.EventLocationName = location.Name;
                graEvent.EventLocationAddress = location.Address;
                graEvent.EventLocationTelephone = location.Telephone;
                graEvent.EventLocationLink = location.Url;
            }

            return await GetRelatedChallengeDetails(graEvent, showInactiveChallenge);
        }

        public async Task<Location> GetLocationByIdAsync(int id)
        {
            return await _locationRepository.GetByIdAsync(id);
        }

        public async Task<ICollection<Location>> GetLocations()
        {
            return await _locationRepository.GetAll(GetCurrentSiteId());
        }

        public async Task<DataWithCount<IEnumerable<Event>>>
            GetPaginatedListAsync(EventFilter filter,
            bool isMissionControl = false)
        {
            ICollection<Event> data = null;
            int count;

            filter.SiteId = GetCurrentSiteId();

            if (isMissionControl)
            {
                VerifyPermission(Permission.ManageEvents);
                data = await _eventRepository.PageAsync(filter);
                count = await _eventRepository.CountAsync(filter);
            }
            else
            {
                // paginate for public
                filter.IsActive = true;

                if (GetAuthUser().Identity.IsAuthenticated)
                {
                    filter.CurrentUserId = GetActiveUserId();
                }

                data = await _eventRepository.PageAsync(filter);
                count = await _eventRepository.CountAsync(filter);

                if (GetAuthUser().Identity.IsAuthenticated)
                {
                    if (filter.Favorites == true)
                    {
                        data = data.Select(_ => { _.IsFavorited = true; return _; }).ToList();
                    }
                    else
                    {
                        var eventIds = data.Select(_ => _.Id);
                        var favoritedEventIds = await _eventRepository.GetUserFavoriteEvents(
                            filter.CurrentUserId.Value, eventIds);

                        foreach (var graEvent in data)
                        {
                            if (favoritedEventIds.Contains(graEvent.Id))
                            {
                                graEvent.IsFavorited = true;
                            }
                        }
                    }
                }
            }
            return new DataWithCount<IEnumerable<Event>>
            {
                Data = data,
                Count = count
            };
        }

        public async Task<DataWithCount<ICollection<Location>>> GetPaginatedLocationsListAsync(
            BaseFilter filter)
        {
            VerifyPermission(Permission.ManageLocations);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Location>>
            {
                Data = await _locationRepository.PageAsync(filter),
                Count = await _locationRepository.CountAsync(filter)
            };
        }

        public async Task<Event> GetRelatedChallengeDetails(Event graEvent,
            bool showInactiveChallenge)
        {
            if (graEvent.ChallengeId.HasValue)
            {
                if (showInactiveChallenge)
                {
                    graEvent.Challenge = await _challengeRepository
                        .GetByIdAsync(graEvent.ChallengeId.Value);
                }
                else
                {
                    graEvent.Challenge = await _challengeRepository
                        .GetActiveByIdAsync(graEvent.ChallengeId.Value);
                }
            }
            else if (graEvent.ChallengeGroupId.HasValue)
            {
                if (showInactiveChallenge)
                {
                    graEvent.ChallengeGroup = await _challengeGroupRepository
                        .GetByIdAsync(graEvent.ChallengeGroupId.Value);
                }
                else
                {
                    graEvent.ChallengeGroup = await _challengeGroupRepository
                        .GetActiveByIdAsync(graEvent.ChallengeGroupId.Value);
                }
            }
            return graEvent;
        }

        public async Task<ICollection<Event>> GetRelatedEventsForTriggerAsync(int triggerId)
        {
            VerifyPermission(Permission.ManageEvents);
            return await _eventRepository.GetRelatedEventsForTriggerAsync(triggerId);
        }

        public async Task<ICollection<Event>> GetUpcomingStreamListAsync()
        {
            return await GetUpcomingStreamListAsync(null);
        }

        public async Task<ICollection<Event>> GetUpcomingStreamListAsync(int count)
        {
            if (count == 0)
            {
                return null;
            }
            return await GetUpcomingStreamListAsync(count);
        }

        public async Task Remove(int eventId)
        {
            VerifyPermission(Permission.ManageEvents);
            await _cache.RemoveAsync(CacheKey.StreamingEvents);
            await _eventRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), eventId);
        }

        public async Task RemoveLocationAsync(int locationId)
        {
            VerifyPermission(Permission.ManageLocations);
            if (await _eventRepository.LocationInUse(GetCurrentSiteId(), locationId))
            {
                throw new GraException("The location is being used by events.");
            }

            var location = await _locationRepository.GetByIdAsync(locationId);

            await _locationRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), locationId);

            if (!string.IsNullOrWhiteSpace(location.Geolocation))
            {
                await _spatialDistanceRepository.InvalidateHeadersAsync(GetCurrentSiteId());
            }
        }

        public async Task<Location> UpdateLocationAsync(Location location)
        {
            VerifyPermission(Permission.ManageLocations);

            var currentLocation = await _locationRepository.GetByIdAsync(location.Id);

            var invalidateSpatialHeaders = false;
            if (currentLocation.Geolocation != location.Geolocation)
            {
                invalidateSpatialHeaders = true;
            }

            currentLocation.Address = location.Address.Trim();
            currentLocation.Geolocation = location.Geolocation;
            currentLocation.Name = location.Name.Trim();
            currentLocation.Telephone = location.Telephone?.Trim();
            currentLocation.Url = location.Url?.Trim();

            currentLocation = await _locationRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), currentLocation);

            if (invalidateSpatialHeaders)
            {
                await _spatialDistanceRepository.InvalidateHeadersAsync(GetCurrentSiteId());
            }

            return currentLocation;
        }

        private async Task<ICollection<Event>> GetUpcomingStreamListAsync(int? count)
        {
            ICollection<Event> events = null;

            var cachedEvents = await _cache.GetStringAsync(CacheKey.StreamingEvents);

            if (!string.IsNullOrEmpty(cachedEvents))
            {
                events = JsonConvert.DeserializeObject<ICollection<Event>>(cachedEvents);
            }
            else
            {
                var filter = new EventFilter
                {
                    ByStreamingStartDesc = true,
                    EventType = (int)EventType.StreamingEvent,
                    IsActive = true,
                    IsStreamingNow = true,
                    SiteId = GetCurrentSiteId(),
                    StartDate = _dateTimeProvider.Now
                };

                if (count.HasValue)
                {
                    filter.Take = count.Value;
                }

                events = await _eventRepository.GetEventListAsync(filter);

                // expire cache at the earlier of: StreamingAccessEnds or one hour from now
                var expiration = events.OrderBy(_ => _.StreamingAccessEnds)
                    .Select(_ => _.StreamingAccessEnds)
                    .FirstOrDefault();

                if (expiration == null || expiration > _dateTimeProvider.Now.AddMinutes(15))
                {
                    expiration = _dateTimeProvider.Now.AddMinutes(15);
                }
                else
                {
                    _logger.LogDebug("Expiring dashboard streaming events early because an event stops streaming at {Expiration}",
                        expiration);
                }

                await _cache.SetStringAsync(CacheKey.StreamingEvents,
                    JsonConvert.SerializeObject(events),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = expiration
                    });
            }

            return events;
        }

        private async Task ValidateEvent(Event graEvent)
        {
            if (graEvent.AtBranchId.HasValue && !(await _branchRepository.ValidateBySiteAsync(
                    graEvent.AtBranchId.Value, graEvent.SiteId)))
            {
                throw new GraException("Invalid Branch selection.");
            }
            if (graEvent.AtLocationId.HasValue && !(await _locationRepository.ValidateAsync(
                    graEvent.AtLocationId.Value, graEvent.SiteId)))
            {
                throw new GraException("Invalid Location selection.");
            }
            if (graEvent.ProgramId.HasValue && !(await _programRepository.ValidateAsync(
                    graEvent.ProgramId.Value, graEvent.SiteId)))
            {
                throw new GraException("Invalid Program selection.");
            }
        }
    }
}
