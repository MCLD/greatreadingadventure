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
    public class PageHeaderRepository
        : AuditingRepository<Model.PageHeader, Domain.Model.PageHeader>, IPageHeaderRepository
    {
        public PageHeaderRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PageHeaderRepository> logger)
            : base(repositoryFacade, logger)
        {
        }

        public async Task<DataWithCount<IEnumerable<PageHeader>>> PageAsync(BaseFilter filter)
        {
            var pages = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            var count = await pages.CountAsync();

            var data = await pages
                .OrderBy(_ => _.PageName)
                .ApplyPagination(filter)
                .ProjectToType<PageHeader>()
                .ToListAsync();

            return new DataWithCount<IEnumerable<PageHeader>>
            {
                Data = data,
                Count = count
            };
        }

        public async Task<bool> StubExistsAsync(int siteId, string stub)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Stub == stub)
                .AnyAsync();
        }
    }
}
