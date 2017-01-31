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
    }
}
