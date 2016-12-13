using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

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
                .ProjectTo<Domain.Model.System>()
                .ToListAsync();
        }
    }
}
