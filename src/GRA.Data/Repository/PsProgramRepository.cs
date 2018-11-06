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
    public class PsProgramRepository
        : AuditingRepository<Model.PsProgram, Domain.Model.PsProgram>, IPsProgramRepository
    {
        public PsProgramRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsProgramRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<PsProgram>> GetByPerformerIdAsync(int performerId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.PerformerId == performerId)
                .ProjectTo<PsProgram>()
                .ToListAsync();
        }

        public async Task AddProgramAgeGroupsAsync(int programId, List<int> ageGroupIds)
        {
            var programAgeGroups = ageGroupIds
                .Select(_ => new Model.PsProgramAgeGroup
                {
                    AgeGroupId = _,
                    ProgramId = programId
                });

            await _context.PsProgramAgeGroups.AddRangeAsync(programAgeGroups);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveProgramAgeGroupsAsync(int programId, List<int> ageGroupIds)
        {
            var programAgeGroups = _context.PsProgramAgeGroups
                .AsNoTracking()
                .Where(_ => _.ProgramId == programId && ageGroupIds.Contains(_.AgeGroupId));

            _context.PsProgramAgeGroups.RemoveRange(programAgeGroups);
            await _context.SaveChangesAsync();
        }
    }
}
