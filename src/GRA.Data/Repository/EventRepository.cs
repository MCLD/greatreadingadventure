using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
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
            return await ApplyFilters(filter)
                .CountAsync();
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
                                 EventLocationDistance = spatial.Distance
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
                                 EventLocationName = eventInfo.AtBranchId.HasValue ? branch.Name : location.Name,
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
                .ProjectTo<Event>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public override async Task<Event> GetByIdAsync(int id)
        {
            var evt = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .ProjectTo<Event>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (evt != null)
            {
                await AddLocationData(evt);
            }
            return evt;
        }

        public async Task<List<Event>> GetByChallengeIdAsync(int challengeId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.ChallengeId == challengeId)
                .ProjectTo<Event>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<Event>> GetByChallengeGroupIdAsync(int challengeGroupId)
        {
            return await DbSet.AsNoTracking()
                   .Where(_ => _.ChallengeGroupId == challengeGroupId)
                   .ProjectTo<Event>(_mapper.ConfigurationProvider)
                   .ToListAsync();
        }

        private async Task AddLocationData(Event evt)
        {
            if (evt.AtLocationId != null)
            {
                var location = await _context.Locations
                    .AsNoTracking()
                    .Where(_ => _.Id == evt.AtLocationId)
                    .SingleOrDefaultAsync();
                if (location != null)
                {
                    evt.EventLocationAddress = location.Address;
                    evt.EventLocationLink = location.Url;
                    evt.EventLocationName = location.Name;
                    evt.EventLocationTelephone = location.Telephone;
                }
            }
            else if (evt.AtBranchId != null)
            {
                var branch = await _context.Branches
                    .AsNoTracking()
                    .Where(_ => _.Id == evt.AtBranchId)
                    .SingleOrDefaultAsync();
                if (branch != null)
                {
                    evt.EventLocationAddress = branch.Address;
                    evt.EventLocationLink = branch.Url;
                    evt.EventLocationName = branch.Name;
                    evt.EventLocationTelephone = branch.Telephone;
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

                    case 1:
                        events = events.Where(_ => _.IsCommunityExperience);
                        break;

                    case 2:
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

            return events;
        }

        public async Task<bool> LocationInUse(int siteId, int locationId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.AtLocationId == locationId)
                .AnyAsync();
        }

        public async Task DetachRelatedTrigger(int triggerId)
        {
            var Events = await DbSet.Where(_ => _.RelatedTriggerId == triggerId).ToListAsync();
            Events.Select(_ => { _.RelatedTriggerId = null; return _; }).ToList();
            _context.UpdateRange(Events);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<Event>> GetRelatedEventsForTriggerAsync(int triggerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.RelatedTriggerId == triggerId)
                .ProjectTo<Event>(_mapper.ConfigurationProvider)
                .ToListAsync();
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

            var experienceWithUserTriggers = await communityExperiences
                .GroupJoin(_context.UserTriggers,
                    experience => experience.RelatedTriggerId.Value,
                    userTriggers => userTriggers.TriggerId,
                    (experience, userTriggers) => new { experience, userTriggers })
                .ToListAsync();

            return experienceWithUserTriggers
                .Select(_ => new DataWithCount<Event>
                {
                    Data = _mapper.Map<Event>(_.experience),
                    Count = _.userTriggers.Count()
                })
                .OrderBy(_ => _.Data.Name)
                .ToList();
        }
    }
}
