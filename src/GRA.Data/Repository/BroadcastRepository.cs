using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Repository.Extensions;
using AutoMapper.QueryableExtensions;
using System;

namespace GRA.Data.Repository
{
    public class BroadcastRepository : AuditingRepository<Model.Broadcast, Broadcast>, IBroadcastRepository
    {
        public BroadcastRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<BroadcastRepository> logger) : base(repositoryFacade, logger)
        {
            
        }

        public async Task<ICollection<Broadcast>> PageAsync(BroadcastFilter filter)
        {
            var broadcasts = ApplyFilters(filter);
            if (filter.Upcoming == false)
            {
                broadcasts = broadcasts.OrderByDescending(_ => _.SendAt);
            }
            else
            {
                broadcasts = broadcasts.OrderBy(_ => _.SendAt);
            }
            return await broadcasts
                .ApplyPagination(filter)
                .ProjectTo<Broadcast>()
                .ToListAsync();
        }

        public async Task<int> CountAsync(BroadcastFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        private IQueryable<Model.Broadcast> ApplyFilters(BroadcastFilter filter)
        {
            var broadcasts = DbSet.AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (filter.Upcoming == true)
            {
                broadcasts = broadcasts.Where(_ => _.SendAt >= _dateTimeProvider.Now);
            }
            else if (filter.Upcoming == false)
            {
                broadcasts = broadcasts.Where(_ => _.SendAt < _dateTimeProvider.Now);
            }

            return broadcasts;
        }

        public async Task<IEnumerable<Broadcast>> GetNewBroadcastsAsync(int siteId, DateTime? lastBroadcast)
        {
            var broadcasts = DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.SendAt <= _dateTimeProvider.Now);

            if (lastBroadcast.HasValue)
            {
                broadcasts = broadcasts.Where(_ => _.SendAt > lastBroadcast);
            }

            return await broadcasts
                .OrderBy(_ => _.SendAt)
                .ProjectTo<Broadcast>()
                .ToListAsync();
        }
    }
}
