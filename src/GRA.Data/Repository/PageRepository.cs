using System;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

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
    }
}