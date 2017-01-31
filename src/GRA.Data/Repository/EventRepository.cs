using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class EventRepository
        : AuditingRepository<Model.Event, Domain.Model.Event>, IEventRepository
    {
        public EventRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<EventRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> CountAsync(int siteId,
            Filter filter = null,
            string search = null,
            bool activeOnly = true)
        {
            return await ApplyFilters(siteId, filter, search, activeOnly)
                .CountAsync();
        }

        public async Task<ICollection<Event>> PageAsync(int siteId,
            int skip,
            int take,
            Filter filter = null,
            string search = null,
            bool activeOnly = true)
        {
            var events = await ApplyFilters(siteId, filter, search, activeOnly)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Event>()
                .ToListAsync();
            await AddLocationData(events);
            return events;
        }

        public override async Task<Event> GetByIdAsync(int id)
        {
            var evt = await base.GetByIdAsync(id);
            await AddLocationData(evt);
            return evt;
        }

        private async Task AddLocationData(ICollection<Event> events)
        {
            foreach (var evt in events)
            {
                await AddLocationData(evt);
            }
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

        private IQueryable<Model.Event> ApplyFilters(int siteId,
            Filter filter = null,
            string search = null,
            bool activeOnly = true)
        {
            // site id filter
            var events = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId);

            // active-only filter
            if (activeOnly)
            {
                events = events.Where(_ => _.IsActive == true);
            }

            // apply filter

            // collect branch ids
            var branchIds = new HashSet<int?>();
            if (filter.BranchIds != null)
            {
                foreach (var branchId in filter.BranchIds)
                {
                    if (branchId != null && !branchIds.Contains(branchId))
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
                events = events.Where(_ => filter.ProgramIds.Contains(_.ProgramId));
            }

            // apply search
            if (!string.IsNullOrEmpty(search))
            {
                events = events.Where(_ => _.Name.Contains(search)
                    || _.Description.Contains(search));
            }

            return events;
        }
    }
}
