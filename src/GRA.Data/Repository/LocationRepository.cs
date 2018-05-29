using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class LocationRepository
        : AuditingRepository<Model.Location, Domain.Model.Location>, ILocationRepository
    {
        public LocationRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<LocationRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<Location>> GetAll(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.Name)
                .ProjectTo<Location>()
                .ToListAsync();
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<Location>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .ApplyPagination(filter)
                .GroupJoin(_context.Events, l => l.Id, e => e.AtLocationId, (l, e) => new { l, e })
                .Select(_ => new Location {
                    Id = _.l.Id,
                    Address = _.l.Address,
                    CreatedAt = _.l.CreatedAt,
                    CreatedBy = _.l.CreatedBy,
                    Name = _.l.Name,
                    SiteId = _.l.SiteId,
                    Telephone = _.l.Telephone,
                    Url = _.l.Url,
                    EventCount = _.e.Count()
                })
                .ToListAsync();
        }

        private IQueryable<Model.Location> ApplyFilters(BaseFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);
        }

        public async Task<bool> ValidateAsync(int locationId, int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == locationId && _.SiteId == siteId)
                .AnyAsync();
        }
    }
}
