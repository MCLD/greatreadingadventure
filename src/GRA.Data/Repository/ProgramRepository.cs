using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class ProgramRepository
        : AuditingRepository<Model.Program, Program>, IProgramRepository
    {
        public ProgramRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<ProgramRepository> logger) : base(repositoryFacade, logger)
        {
        }
        public async Task<IEnumerable<Program>> GetAllAsync(int siteId)
        {
            return await DbSet
               .AsNoTracking()
               .Where(_ => _.SiteId == siteId)
               .OrderBy(_ => _.Position)
               .ProjectTo<Program>()
               .ToListAsync();
        }

        public async Task<bool> ValidateAsync(int programId, int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == programId && _.SiteId == siteId)
                .AnyAsync();
        }
    }
}
