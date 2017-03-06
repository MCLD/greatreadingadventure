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
                .ProjectTo<Location>()
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
