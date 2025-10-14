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
    public class SiteRepository
        : AuditingRepository<Model.Site, Site>, ISiteRepository
    {
        public SiteRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SiteRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Site>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .ProjectToType<Site>()
                .ToListAsync();
        }

        public async Task<DataWithCount<IEnumerable<Site>>> PageAsync(BaseFilter filter)
        {
            var sites = DbSet.AsNoTracking();
            var count = await sites.CountAsync();
            var data = await sites
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectToType<Site>()
                .ToListAsync();

            return new DataWithCount<IEnumerable<Site>>
            {
                Data = data,
                Count = count
            };
        }
    }
}
