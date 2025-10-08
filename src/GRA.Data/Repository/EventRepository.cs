using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class EventRepository
        : AuditingRepository<Model.Event, Domain.Model.Event>, IEventRepository
    {
        public EventRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<EventRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> CountAsync(EventFilter filter)
        {
            return await ApplyFilters(filter).CountAsync();
        }

        public async Task DetachRelatedChallenge(int userId, int challengeId)
        {
            var events = await DbSet.Where(_ => _.ChallengeId == challengeId).ToListAsync();

            foreach (var graEvent in events)
            {
                graEvent.ChallengeId = null;
                await base.UpdateAsync(userId, graEvent, null);
            }
        }

        public async Task DetachRelatedChallengeGroup(int userId, int challengeGroupId)
        {
            var events = await DbSet.Where(_ => _.ChallengeGroupId == challengeGroupId)
                .ToListAsync();

            foreach (var graEvent in events)
            {
                graEvent.ChallengeGroupId = null;
                await base.UpdateAsync(userId, graEvent, null);
            }
        }

        public async Task DetachRelatedTrigger(int triggerId)
        {
            var Events = await DbSet.Where(_ => _.RelatedTriggerId == triggerId).ToListAsync();
            Events.Select(_ => { _.RelatedTriggerId = null; return _; }).ToList();
            _context.UpdateRange(Events);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Event>> GetByChallengeGroupIdAsync(int challengeGroupId)
        {
            return await DbSet.AsNoTracking()
                   .Where(_ => _.ChallengeGroupId == challengeGroupId)
                   .ProjectToType<Event>()
                   .ToListAsync();
        }

        public async Task<List<Event>> GetByChallengeIdAsync(int challengeId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.ChallengeId == challengeId)
                .ProjectToType<Event>()
                .ToListAsync();
        }

        public override async Task<Event> GetByIdAsync(int id)
        {
            var currentEvent = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .ProjectToType<Event>()
                .SingleOrDefaultAsync();

            if (currentEvent != null)
            {
                await AddLocationData(currentEvent);
            }
            return currentEvent;
        }

        public async Task<ICollection<DataWithCount<Event>>>
            GetCommunityExperienceAttendanceAsync(ReportCriterion criterion)
        {
            var communityExperiences = DbSet
                .AsNoTracking()
                .Where(_ => _.IsCommunityExperience && _.RelatedTriggerId.HasValue);

            if (criterion.BranchId.HasValue)
            {
                communityExperiences = communityExperiences
                    .Where(_ => _.RelatedBranchId == criterion.BranchId);
            }
            else if (criterion.SystemId.HasValue)
            {
                communityExperiences = communityExperiences
                    .Where(_ => _.RelatedSystemId == criterion.SystemId);
            }

            var communityExperienceList = (await communityExperiences
                .ToListAsync())
                .Select(_ => new DataWithCount<Event>
                {
                    Data = _mapper.Map<Event>(_)
                })
                .OrderBy(_ => _.Data.Name)
                .ToList();

            var triggerCounts = await _context.UserTriggers
                .AsNoTracking()
                .Where(_ => communityExperiences
                    .Select(e => e.RelatedTriggerId)
                    .Contains(_.TriggerId))
                .GroupBy(_ => _.TriggerId)
                .Select(_ => new { _.Key, Count = _.Count() })
                .ToListAsync();

            foreach (var experience in communityExperienceList)
            {
                experience.Count = triggerCounts
                    .Where(_ => _.Key == experience.Data.RelatedTriggerId)
                    .Select(_ => _.Count)
                    .FirstOrDefault();
            }

            return communityExperienceList;
        }

        public async Task<ICollection<Event>> GetEventListAsync(EventFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return await ApplyFilters(filter).Select(_ => new Event
            {
                Id = _.Id,
                Name = _.Name,
                StartDate = _.StartDate,
                StreamingAccessEnds = _.StreamingAccessEnds,
                EndDate = _.EndDate
            })
            .ToListAsync();
        }

        public async Task<ICollection<Event>> GetRelatedEventsForTriggerAsync(int triggerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.RelatedTriggerId == triggerId)
                .ProjectToType<Event>()
                .ToListAsync();
        }

        public async Task<string> GetSecretCodeForStreamingEventAsync(int eventId)
        {
            return await DbSet
                .Where(_ => _.IsStreaming && _.RelatedTriggerId.HasValue && _.Id == eventId)
                .Join(_context.Triggers,
                    graEvent => graEvent.RelatedTriggerId,
                    trigger => trigger.Id,
                    (_, trigger) => trigger)
                .AsNoTracking()
                .Select(_ => _.SecretCode)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<int>> GetUserFavoriteEvents(int userId,
            IEnumerable<int> eventIds = null)
        {
            var favoriteEvents = _context.UserFavoriteEvents
                .AsNoTracking()
                .Where(_ => _.UserId == userId);

            if (eventIds?.Count() > 0)
            {
                favoriteEvents = favoriteEvents
                    .Where(_ => eventIds.Contains(_.EventId));
            }

            return await favoriteEvents
                .Select(_ => _.EventId)
                .ToListAsync();
        }

        public async Task<bool> LocationInUse(int siteId, int locationId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.AtLocationId == locationId)
                .AnyAsync();
        }

        public async Task<ICollection<Event>> PageAsync(EventFilter filter)
        {
            var eventQuery = ApplyFilters(filter);

            if (filter.SpatialDistanceHeaderId.HasValue)
            {
                var spatialDetails = _context.SpatialDistanceDetails
                    .AsNoTracking()
                    .Where(_ => _.SpatialDistanceHeaderId == filter.SpatialDistanceHeaderId);

                // tables joined with Cross Apply
                eventQuery = from eventInfo in eventQuery
                             from spatial in spatialDetails
                                 .Where(_ => (_.BranchId.HasValue
                                         && _.BranchId == eventInfo.AtBranchId)
                                     || (_.LocationId.HasValue
                                         && _.LocationId == eventInfo.AtLocationId))
                                .DefaultIfEmpty()
                             select new Model.Event
                             {
                                 Id = eventInfo.Id,
                                 Name = eventInfo.Name,
                                 StartDate = eventInfo.StartDate,
                                 EventLocationName = spatial.BranchId.HasValue ? spatial.Branch.Name : spatial.Location.Name,
                                 EventLocationDistance = spatial.Distance,
                                 AllDay = eventInfo.AllDay,
                                 EndDate = eventInfo.EndDate
                             };
            }
            else
            {
                // tables joined with Left Outer Join
                eventQuery = from eventInfo in eventQuery
                             join branches in _context.Branches on eventInfo.AtBranchId equals branches.Id into b
                             from branch in b.DefaultIfEmpty()
                             join locations in _context.Locations on eventInfo.AtLocationId equals locations.Id into l
                             from location in l.DefaultIfEmpty()
                             select new Model.Event
                             {
                                 Id = eventInfo.Id,
                                 Name = eventInfo.Name,
                                 StartDate = eventInfo.StartDate,
                                 StreamingAccessEnds = eventInfo.StreamingAccessEnds,
                                 EventLocationName = eventInfo.AtBranchId.HasValue ? branch.Name : location.Name,
                                 AllDay = eventInfo.AllDay,
                                 EndDate = eventInfo.EndDate
                             };
            }

            if (filter.SortBy == SortEventsBy.StartDate)
            {
                eventQuery = eventQuery
                    .OrderBy(_ => _.StartDate)
                    .ThenBy(_ => _.Name);
            }
            else
            {
                eventQuery = eventQuery
                        .OrderBy(_ => _.EventLocationDistance)
                        .ThenBy(_ => _.StartDate)
                        .ThenBy(_ => _.Name);
            }

            return await eventQuery.ApplyPagination(filter)
                .ProjectToType<Event>()
                .ToListAsync();
        }

        public async Task<int> RemoveFavoritesAsync(int eventId)
        {
            var favorites = await _context
                .UserFavoriteEvents
                .AsNoTracking()
                .Where(_ => _.EventId == eventId)
                .ToListAsync();

            int favoriteCount = favorites.Count;

            _context.UserFavoriteEvents.RemoveRange(favorites);
            await _context.SaveChangesAsync();

            return favoriteCount;
        }

        public async Task UpdateUserFavoritesAsync(int authUserId, int userId,
            IEnumerable<int> favoritesToAdd, IEnumerable<int> favoritesToRemove)
        {
            if (favoritesToAdd.Any())
            {
                var time = _dateTimeProvider.Now;
                var userFavoriteList = new List<Model.UserFavoriteEvent>();
                foreach (var eventId in favoritesToAdd)
                {
                    userFavoriteList.Add(new Model.UserFavoriteEvent
                    {
                        UserId = userId,
                        EventId = eventId,
                        CreatedAt = time,
                        CreatedBy = authUserId
                    });
                }
                await _context.UserFavoriteEvents.AddRangeAsync(userFavoriteList);
            }
            if (favoritesToRemove.Any())
            {
                var removeList = _context.UserFavoriteEvents
                    .Where(_ => _.UserId == userId && favoritesToRemove
                    .Contains(_.EventId));
                _context.UserFavoriteEvents.RemoveRange(removeList);
            }
            await SaveAsync();
        }

        public async Task<IEnumerable<int>> ValidateEventIdsAsync(int siteId,
            IEnumerable<int> eventIds)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && eventIds.Contains(_.Id))
                .Select(_ => _.Id)
                .ToListAsync();
        }

        private async Task AddLocationData(Event currentEvent)
        {
            if (currentEvent.AtLocationId != null)
            {
                var location = await _context.Locations
                    .AsNoTracking()
                    .SingleOrDefaultAsync(_ => _.Id == currentEvent.AtLocationId);
                if (location != null)
                {
                    currentEvent.EventLocationAddress = location.Address;
                    currentEvent.EventLocationLink = location.Url;
                    currentEvent.EventLocationName = location.Name;
                    currentEvent.EventLocationTelephone = location.Telephone;
                }
            }
            else if (currentEvent.AtBranchId != null)
            {
                var branch = await _context.Branches
                    .AsNoTracking()
                    .SingleOrDefaultAsync(_ => _.Id == currentEvent.AtBranchId);
                if (branch != null)
                {
                    currentEvent.EventLocationAddress = branch.Address;
                    currentEvent.EventLocationLink = branch.Url;
                    currentEvent.EventLocationName = branch.Name;
                    currentEvent.EventLocationTelephone = branch.Telephone;
                }
            }
        }

        private IQueryable<Model.Event> ApplyFilters(EventFilter filter)
        {
            // site id filter
            var events = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            // active-only filter
            if (filter.IsActive.HasValue)
            {
                events = events.Where(_ => _.IsActive == filter.IsActive);
            }

            if (filter.EventType.HasValue)
            {
                switch (filter.EventType.Value)
                {
                    // case 0
                    default:
                        events = events.Where(_ => !_.IsCommunityExperience && !_.IsStreaming);
                        break;

                    case (int)EventType.CommunityExperience:
                        events = events.Where(_ => _.IsCommunityExperience);
                        break;

                    case (int)EventType.StreamingEvent:
                        events = events.Where(_ => _.IsStreaming);
                        break;
                }
            }

            // apply filter
            // collect branch ids
            var branchIds = new HashSet<int?>();
            if (filter.BranchIds != null)
            {
                foreach (var branchId in filter.BranchIds)
                {
                    if (!branchIds.Contains(branchId))
                    {
                        branchIds.Add(branchId);
                    }
                }
            }

            if (filter.SystemIds != null)
            {
                var branchIdsFromSystem = _context.Branches
                    .AsNoTracking()
                    .Where(_ => filter.SystemIds.Contains(_.SystemId))
                    .Select(_ => _.Id);

                foreach (var branchId in branchIdsFromSystem)
                {
                    if (!branchIds.Contains(branchId))
                    {
                        branchIds.Add(branchId);
                    }
                }
            }

            // filter by branch ids
            if (branchIds.Count > 0)
            {
                events = events.Where(_ => branchIds.Contains(_.AtBranchId));
            }

            // filter by location ids
            if (filter.LocationIds != null)
            {
                events = events.Where(_ => filter.LocationIds.Contains(_.AtLocationId));
            }

            // filter by program ids
            if (filter.ProgramIds != null)
            {
                events = events.Where(_ => filter.ProgramIds.Any(p => p == _.ProgramId));
            }

            // filter by user ids
            if (filter.UserIds != null)
            {
                events = events.Where(_ => filter.UserIds.Contains(_.CreatedBy));
            }

            // filter by dates
            if (filter.EventType == (int)EventType.StreamingEvent)
            {
                if (filter.StartDate != null)
                {
                    if (filter.IsStreamingNow == true)
                    {
                        events = events.Where(_ => _.StartDate <= filter.StartDate
                                && filter.StartDate <= _.StreamingAccessEnds);
                    }
                    else
                    {
                        events = events.Where(_ => _.StartDate >= filter.StartDate
                            || (_.StartDate <= filter.StartDate
                                && filter.StartDate <= _.StreamingAccessEnds));
                    }
                }
                if (filter.EndDate != null)
                {
                    events = events.Where(_ => filter.EndDate <= _.StreamingAccessEnds);
                }
            }
            else
            {
                if (filter.StartDate != null)
                {
                    events = events.Where(_ =>
                        ((!_.AllDay || !_.EndDate.HasValue) && _.StartDate.Date >= filter.StartDate.Value.Date)
                        || _.EndDate.Value.Date >= filter.StartDate.Value.Date);
                }
                if (filter.EndDate != null)
                {
                    events = events.Where(_ =>
                        ((!_.AllDay || !_.EndDate.HasValue) && _.StartDate.Date <= filter.EndDate.Value.Date)
                        || _.StartDate.Date <= filter.EndDate.Value.Date);
                }
            }

            // filter for favorites
            if (filter.Favorites == true && filter.CurrentUserId.HasValue)
            {
                var userFavoriteEvents = _context.UserFavoriteEvents
                    .AsNoTracking()
                    .Where(_ => _.UserId == filter.CurrentUserId);

                events = events.Join(userFavoriteEvents,
                    graEvent => graEvent.Id,
                    favoritedEvent => favoritedEvent.EventId,
                    (graEvent, favoritedEvent) => graEvent);
            }

            // filter for attended
            if (filter.IsAttended.HasValue && filter.CurrentUserId.HasValue)
            {
                var userAttendedEvents = _context.UserLogs
                    .AsNoTracking()
                    .Where(_ => _.UserId == filter.CurrentUserId
                        && _.EventId.HasValue
                        && !_.IsDeleted)
                    .Select(_ => _.EventId);

                events = events
                    .Where(_ => userAttendedEvents.Contains(_.Id) == filter.IsAttended.Value);
            }

            // apply search
            if (!string.IsNullOrEmpty(filter.Search))
            {
                events = events.Where(_ => _.Name.Contains(filter.Search)
                    || _.Description.Contains(filter.Search));
            }

            // for spatial searches make sure events have a corresponding geolocation
            if (filter.SpatialDistanceHeaderId.HasValue)
            {
                var spatialDetails = _context.SpatialDistanceDetails
                    .AsNoTracking()
                    .Where(_ => _.SpatialDistanceHeaderId == filter.SpatialDistanceHeaderId);

                var spatialBranchIds = spatialDetails
                    .Where(_ => _.BranchId.HasValue)
                    .Select(_ => _.BranchId);

                var spatialLocationIds = spatialDetails
                    .Where(_ => _.LocationId.HasValue)
                    .Select(_ => _.LocationId);

                events = events.Where(_ =>
                    (_.AtBranchId.HasValue && spatialBranchIds.Contains(_.AtBranchId.Value))
                    || (_.AtLocationId.HasValue
                        && spatialLocationIds.Contains(_.AtLocationId.Value)));
            }

            // apply ordering if requested
            if (filter.ByStreamingStartDesc)
            {
                events = events.OrderByDescending(_ => _.StartDate)
                    .ThenBy(_ => _.Name);
            }

            return events;
        }
    }
}
