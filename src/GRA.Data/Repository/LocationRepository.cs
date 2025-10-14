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
    public class LocationRepository
        : AuditingRepository<Model.Location, Domain.Model.Location>, ILocationRepository
    {
        public LocationRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<LocationRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<Location>> GetAll(int siteId,
            bool requireGeolocation = false)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId
                    && (!requireGeolocation || !string.IsNullOrWhiteSpace(_.Geolocation)))
                .OrderBy(_ => _.Name)
                .ProjectToType<Location>()
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
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .Select(_ => new Location
                {
                    Id = _.Id,
                    Address = _.Address,
                    CreatedAt = _.CreatedAt,
                    CreatedBy = _.CreatedBy,
                    Geolocation = _.Geolocation,
                    Name = _.Name,
                    SiteId = _.SiteId,
                    Telephone = _.Telephone,
                    Url = _.Url,
                    EventCount = _context.Events.Where(e => e.AtLocationId == _.Id).Count()
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
