using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Service
{
    public class EventService : BaseUserService<EventService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeGroupRepository _challengeGroupRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProgramRepository _programRepository;
        public EventService(ILogger<EventService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IChallengeRepository challengeRepository,
            IChallengeGroupRepository challengeGroupRepository,
            IEventRepository eventRepository,
            ILocationRepository locationRepository,
            IProgramRepository programRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _challengeRepository = Require.IsNotNull(
                challengeRepository, nameof(challengeRepository));
            _challengeGroupRepository = Require.IsNotNull(
                challengeGroupRepository, nameof(challengeGroupRepository));
            _eventRepository = Require.IsNotNull(eventRepository, nameof(eventRepository));
            _locationRepository = Require.IsNotNull(locationRepository, nameof(locationRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
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
                data = await _eventRepository.PageAsync(filter);
                count = await _eventRepository.CountAsync(filter);
            }

            foreach (var item in data)
            {
                if (item.AtBranchId != null)
                {
                    var branch = await _branchRepository.GetByIdAsync((int)item.AtBranchId);
                    item.EventLocationName = branch.Name;
                }
                if (item.AtLocationId != null)
                {
                    var location = await _locationRepository.GetByIdAsync((int)item.AtLocationId);
                    item.EventLocationName = location.Name;
                }
            }

            return new DataWithCount<IEnumerable<Event>>
            {
                Data = data,
                Count = count
            };
        }

        public async Task<Event> GetDetails(int eventId, bool showInactiveChallenge = false)
        {
            var graEvent = await _eventRepository.GetByIdAsync(eventId);
            if (graEvent == null)
            {
                throw new GraException("The requested event could not be accessed or does not exist.");
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

            graEvent = await GetRelatedChallengeDetails(graEvent, showInactiveChallenge);

            return graEvent;
        }

        public async Task<Event> Add(Event graEvent)
        {
            VerifyPermission(Permission.ManageEvents);
            graEvent.SiteId = GetCurrentSiteId();
            graEvent.RelatedBranchId = GetClaimId(ClaimType.BranchId);
            graEvent.RelatedSystemId = GetClaimId(ClaimType.SystemId);
            if (HasPermission(Permission.ViewAllChallenges) == false)
            {
                graEvent.ChallengeId = null;
                graEvent.ChallengeGroupId = null;
            }
            await ValidateEvent(graEvent);
            return await _eventRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), graEvent);
        }

        public async Task<Event> Edit(Event graEvent)
        {
            VerifyPermission(Permission.ManageEvents);
            var currentEvent = await _eventRepository.GetByIdAsync(graEvent.Id);
            graEvent.SiteId = currentEvent.SiteId;
            if (HasPermission(Permission.ViewAllChallenges) == false)
            {
                graEvent.ChallengeId = currentEvent.ChallengeId;
                graEvent.ChallengeGroupId = currentEvent.ChallengeGroupId;
            }
            await ValidateEvent(graEvent);
            return await _eventRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), graEvent);
        }

        public async Task Remove(int eventId)
        {
            VerifyPermission(Permission.ManageEvents);
            await _eventRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), eventId);
        }

        public async Task<ICollection<Location>> GetLocations()
        {
            return await _locationRepository.GetAll(GetCurrentSiteId());
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

        public async Task<Location> AddLocation(Location location)
        {
            VerifyPermission(Permission.ManageLocations);
            location.SiteId = GetCurrentSiteId();
            return await _locationRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), location);
        }

        public async Task<Location> EditLocation(Location location)
        {
            VerifyPermission(Permission.ManageLocations);
            location.SiteId = GetCurrentSiteId();
            return await _locationRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), location);
        }

        public async Task RemoveLocation(int locationId)
        {
            VerifyPermission(Permission.ManageLocations);
            if (await _eventRepository.LocationInUse(GetCurrentSiteId(), locationId))
            {
                throw new GraException("The location is being used by events.");
            }
            await _locationRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), locationId);
        }

        public async Task<ICollection<Event>> GetRelatedEventsForTriggerAsync(int triggerId)
        {
            VerifyPermission(Permission.ManageEvents);
            return await _eventRepository.GetRelatedEventsForTriggerAsync(triggerId);
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

        private async Task ValidateEvent(Event graEvent)
        {
            if (graEvent.AtBranchId.HasValue)
            {
                if (!(await _branchRepository.ValidateBySiteAsync(
                    graEvent.AtBranchId.Value, graEvent.SiteId)))
                {
                    throw new GraException("Invalid Branch selection.");
                }
            }
            if (graEvent.AtLocationId.HasValue)
            {
                if (!(await _locationRepository.ValidateAsync(
                    graEvent.AtLocationId.Value, graEvent.SiteId)))
                {
                    throw new GraException("Invalid Location selection.");
                }
            }
            if (graEvent.ProgramId.HasValue)
            {
                if (!(await _programRepository.ValidateAsync(
                    graEvent.ProgramId.Value, graEvent.SiteId)))
                {
                    throw new GraException("Invalid Program selection.");
                }
            }
        }

        public async Task<List<Event>> GetByChallengeIdAsync(int challengeId)
        {
            return await _eventRepository.GetByChallengeIdAsync(challengeId);
        }

        public async Task<List<Event>> GetByChallengeGroupIdAsync(int challengeGroupId)
        {
            return await _eventRepository.GetByChallengeGroupIdAsync(challengeGroupId);
        }
    }
}
