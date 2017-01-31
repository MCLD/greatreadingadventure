using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;

namespace GRA.Domain.Service
{
    public class EventService : BaseUserService<EventService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILocationRepository _locationRepository;
        public EventService(ILogger<EventService> logger,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IEventRepository eventRepository,
            ILocationRepository locationRepository) : base(logger, userContextProvider)
        {
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _eventRepository = Require.IsNotNull(eventRepository, nameof(eventRepository));
            _locationRepository = Require.IsNotNull(locationRepository, nameof(locationRepository));
        }

        public async Task<DataWithCount<IEnumerable<Event>>>
            GetPaginatedListAsync(int skip,
            int take,
            Filter filter = null,
            string search = null,
            bool isMissionControl = false)
        {
            ICollection<Event> data = null;
            int count;

            if (isMissionControl)
            {
                VerifyPermission(Permission.ManageEvents);
                data = await _eventRepository.PageAsync(GetCurrentSiteId(), skip, take, filter, search, false);
                count = await _eventRepository.CountAsync(GetCurrentSiteId(), filter, search, false);
            }
            else
            {
                // paginate for public
                data = await _eventRepository.PageAsync(GetCurrentSiteId(), skip, take, filter, search, true);
                count = await _eventRepository.CountAsync(GetCurrentSiteId(), filter, search, true);
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

        public async Task<Event> GetDetails(int eventId)
        {
            var graEvent = await _eventRepository.GetByIdAsync(eventId);
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

            return graEvent;
        }

        public async Task<Event> Add(Event graEvent)
        {
            VerifyPermission(Permission.ManageEvents);
            graEvent.SiteId = GetCurrentSiteId();
            graEvent.RelatedBranchId = GetClaimId(ClaimType.BranchId);
            graEvent.RelatedSystemId = GetClaimId(ClaimType.SystemId);
            return await _eventRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), graEvent);
        }

        public async Task<Event> Edit(Event graEvent)
        {
            VerifyPermission(Permission.ManageEvents);
            graEvent.SiteId = GetCurrentSiteId();
            return await _eventRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), graEvent);
        }

        public async Task Remove(Event graEvent)
        {
            VerifyPermission(Permission.ManageEvents);
            await _eventRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), graEvent.Id);
        }

        public async Task<ICollection<Location>> GetLocations()
        {
            return await _locationRepository.GetAll(GetCurrentSiteId());
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
            await _locationRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), locationId);
        }
    }
}
