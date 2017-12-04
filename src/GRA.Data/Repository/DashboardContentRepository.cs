using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DashboardContentRepository
        : AuditingRepository<Model.DashboardContent, Domain.Model.DashboardContent>,
        IDashboardContentRepository
    {
        public DashboardContentRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DashboardContentRepository> logger): base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<DashboardContent>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.StartTime)
                .ApplyPagination(filter)
                .ProjectTo<DashboardContent>()
                .ToListAsync();
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        private IQueryable<Model.DashboardContent> ApplyFilters(BaseFilter filter)
        {
            var dashboardContents = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (filter.IsActive == true)
            {
                var currentContentId = dashboardContents
                    .Where(_ => _.StartTime <= _dateTimeProvider.Now)
                    .OrderByDescending(_ => _.StartTime)
                    .Select(_ => _.Id)
                    .FirstOrDefault();

                dashboardContents = dashboardContents
                    .Where(_ => _.StartTime > _dateTimeProvider.Now || _.Id == currentContentId);
            }
            else if (filter.IsActive == false)
            {
                dashboardContents = dashboardContents
                    .Where(_ => _.StartTime < _dateTimeProvider.Now)
                    .OrderByDescending(_ => _.StartTime)
                    .Skip(1);
            }

            return dashboardContents;
        }

        public async Task<DashboardContent> GetCurrentAsync(int siteId)
        {
            return await DbSet
                    .AsNoTracking()
                    .Where(_ => _.StartTime <= _dateTimeProvider.Now)
                    .OrderByDescending(_ => _.StartTime)
                    .Take(1)
                    .ProjectTo<DashboardContent>()
                    .SingleOrDefaultAsync();
        }
    }
}
