using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class ReportRequestRepository
        : AuditingRepository<Model.ReportRequest, ReportRequest>, IReportRequestRepository
    {
        public ReportRequestRepository(ServiceFacade.Repository facade,
            ILogger<ReportRequestRepository> logger) : base(facade, logger)
        {
        }

        public async Task<int> CountAsync(ReportRequestFilter filter)
        {
            return await ApplyFilters(filter).CountAsync();
        }

        public async Task<ICollection<ReportRequestSummary>> PageAsync(ReportRequestFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderByDescending(_ => _.CreatedAt)
                .ApplyPagination(filter)
                .Select(_ => new ReportRequestSummary
                {
                    Id = _.Id,
                    ReportId = _.ReportId,
                    Name = _.Name,
                    CreatedAt = _.CreatedAt,
                    RequestedByUserId = _.CreatedBy,
                    Success = _.Success,
                    Message = _.Message,
                    Started = _.Started,
                    Finished = _.Finished
                })
                .ToListAsync();
        }

        private IQueryable<Model.ReportRequest> ApplyFilters(ReportRequestFilter filter)
        {
            var query =
                from rr in DbSet.AsNoTracking()
                join rc in _context.Set<Model.ReportCriterion>() on rr.ReportCriteriaId equals rc.Id
                where (!filter.SiteId.HasValue || rc.SiteId == filter.SiteId.Value)
                select rr;

            if (filter.ReportId.HasValue)
                query = query.Where(_ => _.ReportId == filter.ReportId.Value);

            if (filter.RequestedByUserId.HasValue)
                query = query.Where(_ => _.CreatedBy == filter.RequestedByUserId.Value);

            if (filter.StartDate.HasValue)
                query = query.Where(_ => _.CreatedAt >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(_ => _.CreatedAt <= filter.EndDate.Value);

            return query;
        }
    }
}
