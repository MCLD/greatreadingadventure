using System;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;

namespace GRA.Data.Repository
{
    public class PageRepository
        : AuditingRepository<Model.Page, Domain.Model.Page>, IPageRepository
    {
        public PageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PageRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<Page> GetByStubAsync(int siteId, string pageStub)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Stub == pageStub)
                .ProjectTo<Page>()
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Page>> PageAllAsync(int siteId,
            int skip,
            int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.Title)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Page>()
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .CountAsync();
        }

        public async Task<IEnumerable<Page>> GetFooterPagesAsync(int siteId)
        {
            return await DbSet
               .AsNoTracking()
               .Where(_ => _.SiteId == siteId
               && _.IsFooter == true
               && _.IsPublished == true)
               .OrderBy(_ => _.Title)
               .ProjectTo<Page>()
               .ToListAsync();
        }
    }
}