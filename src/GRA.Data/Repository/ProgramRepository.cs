using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<Program>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Position)
                .ApplyPagination(filter)
                .ProjectTo<Program>()
                .ToListAsync();
        }

        private IQueryable<Model.Program> ApplyFilters(BaseFilter filter)
        {
            var programList = DbSet
                 .AsNoTracking()
                 .Where(_ => _.SiteId == filter.SiteId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                programList = programList.Where(_ => _.Name.Contains(filter.Search));
            }

            return programList;
        }

        public async Task RemoveSaveAsync(int userId, Program program)
        {
            await DbSet.Where(_ => _.SiteId == program.SiteId && _.Position > program.Position)
                .ForEachAsync(_ => _.Position--);

            await base.RemoveSaveAsync(userId, program.Id);
        }

        public async Task<bool> IsInUseAsync(int programId, int siteId)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(_ => _.ProgramId == programId && _.SiteId == siteId && _.IsDeleted == false)
                .AnyAsync();
        }

        public async Task<bool> ValidateAsync(int programId, int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == programId && _.SiteId == siteId)
                .AnyAsync();
        }

        public async Task DecreasePositionAsync(int programId, int siteId)
        {
            var program = await DbSet
 
                .Where(_ => _.Id == programId && _.SiteId == siteId)
                .SingleOrDefaultAsync();
            if (program == null)
            {
                throw new Exception($"Program {programId} could not be found.");
            }

            var previousProgram = await DbSet
                .Where(_ => _.SiteId == program.SiteId && _.Position == program.Position - 1)
                .SingleOrDefaultAsync();
            if (previousProgram == null)
            {
                throw new Exception($"Program {programId} is already in the first position.");
            }
            previousProgram.Position++;
            program.Position--;
            await _context.SaveChangesAsync();
        }

        public async Task IncreasePositionAsync(int programId, int siteId)
        {
            var program = await DbSet
                .Where(_ => _.Id == programId && _.SiteId == siteId)
                .SingleOrDefaultAsync();
            if (program == null)
            {
                throw new Exception($"Program {programId} could not be found.");
            }

            var nextProgram = await DbSet
                .Where(_ => _.SiteId == program.SiteId && _.Position == program.Position + 1)
                .SingleOrDefaultAsync();
            if (nextProgram == null)
            {
                throw new Exception($"Program {programId} is already in the last position.");
            }
            nextProgram.Position--;
            program.Position++;
            await _context.SaveChangesAsync();
        }
    }
}
