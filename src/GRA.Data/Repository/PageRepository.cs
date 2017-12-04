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
            var page = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Stub == pageStub);

            return await page
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

        public async Task<IEnumerable<Page>> GetAreaPagesAsync(int siteId, bool navPages)
        {
            var pages = DbSet
               .AsNoTracking()
               .Where(_ => _.SiteId == siteId && _.IsPublished == true);

            if (navPages)
            {
                pages = pages
                    .Where(_ => string.IsNullOrWhiteSpace(_.NavText) == false)
                    .OrderBy(_ => _.NavText);
            }
            else
            {
                pages = pages
                    .Where(_ => string.IsNullOrWhiteSpace(_.FooterText) == false)
                    .OrderBy(_ => _.FooterText);
            }

            return await pages.ProjectTo<Page>().ToListAsync();
        }
    }
}