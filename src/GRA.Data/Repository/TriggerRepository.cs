using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class TriggerRepository : AuditingRepository<Model.Trigger, Trigger>, ITriggerRepository
    {
        public TriggerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<TriggerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        // honors site id, skip, and take
        public async Task<int> CountAsync(Filter filter)
        {
            return await ApplySiteIdPagination(filter).CountAsync();
        }

        // honors site id, skip, and take
        public async Task<ICollection<Trigger>> PageAsync(Filter filter)
        {
            return await ApplySiteIdPagination(filter)
                .ProjectTo<Trigger>()
                .ToListAsync();
        }

        private IQueryable<Model.Trigger> ApplySiteIdPagination(Filter filter)
        {
            var filteredData = DbSet.AsNoTracking().Where(_ => _.SiteId == filter.SiteId);
            return ApplyPagination(filteredData, filter);
        }
    }
}
