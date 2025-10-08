using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class SystemRepository
        : AuditingRepository<Model.System, Domain.Model.System>, ISystemRepository
    {
        public SystemRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SystemRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Domain.Model.System>> GetAllAsync(int siteId)
        {
            return await _context.Systems
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.Name)
                .ProjectToType<Domain.Model.System>()
                .ToListAsync();
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<Domain.Model.System>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectToType<Domain.Model.System>()
                .ToListAsync();
        }

        private IQueryable<Data.Model.System> ApplyFilters(BaseFilter filter)
        {
            var systemList = DbSet
                 .AsNoTracking()
                 .Where(_ => _.SiteId == filter.SiteId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                systemList = systemList.Where(_ => _.Name.Contains(filter.Search));
            }

            return systemList;
        }

        public async Task<bool> IsInUseAsync(int systemId)
        {
            return await _context.Branches.AsNoTracking().AnyAsync(_ => _.SystemId == systemId);
        }

        public async Task<bool> ValidateAsync(int systemId, int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == systemId && _.SiteId == siteId)
                .AnyAsync();
        }

        public async Task UpdateCreatedByAsync(int userId, int systemId)
        {
            var system = DbSet.Where(_ => _.Id == systemId).SingleOrDefault();
            if (system != null && system.CreatedBy == Defaults.InitialInsertUserId)
            {
                system.CreatedBy = userId;
                DbSet.Update(system);
                await _context.SaveChangesAsync();
            }
        }
    }
}
