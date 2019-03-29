using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PageRepository
        : AuditingRepository<Model.Page, Domain.Model.Page>, IPageRepository
    {
        public PageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PageRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<Page> GetByStubAsync(int siteId, 
            string pageStub, 
            int languageId)
        {
            var pageHeaderId = _context.PageHeaders
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Stub == pageStub)
                .Select(_ => _.Id);

            return await DbSet
                .AsNoTracking()
                .Where(_ => pageHeaderId.Contains(_.PageHeaderId)
                    && _.LanguageId == languageId)
                .ProjectTo<Page>()
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Page>> PageAllAsync(int siteId,
            int skip,
            int take)
        {
            return await _context.PageHeaders
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.PageName)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Page>()
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(int siteId)
        {
            return await _context.PageHeaders
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .CountAsync();
        }

        public async Task<IEnumerable<Page>> GetAreaPagesAsync(int siteId,
            bool navPages,
            int languageId)
        {
            var pageHeaders = _context.PageHeaders
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .ToList();

            var pageHeaderIds = pageHeaders.Select(_ => _.Id);

            var pages = DbSet
               .AsNoTracking()
               .Where(_ => pageHeaderIds.Contains(_.PageHeaderId)
                    && _.LanguageId == languageId
                    && _.IsPublished);

            if (navPages)
            {
                pages = pages
                    .Where(_ => !string.IsNullOrWhiteSpace(_.NavText))
                    .OrderBy(_ => _.NavText);
            }
            else
            {
                pages = pages
                    .Where(_ => !string.IsNullOrWhiteSpace(_.FooterText))
                    .OrderBy(_ => _.FooterText);
            }

            var finalPages = await pages.ProjectTo<Page>().ToListAsync();

            foreach(var page in finalPages)
            {
                page.PageStub = pageHeaders
                    .Where(_ => _.Id == page.PageHeaderId)
                    .Select(_ => _.Stub).Single();
            }

            return finalPages;
        }
    }
}