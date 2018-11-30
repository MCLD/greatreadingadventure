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
    public class PsProgramRepository
        : AuditingRepository<Model.PsProgram, Domain.Model.PsProgram>, IPsProgramRepository
    {
        public PsProgramRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsProgramRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<PsProgram> GetByIdAsync(int id, bool onlyApproved = false)
        {
            var program = DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id);

            if (onlyApproved)
            {
                program = program.Where(_ => _.Performer.IsApproved);
            }

            return await program
                .ProjectTo<PsProgram>()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<PsProgram>> GetByPerformerIdAsync(int performerId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.PerformerId == performerId)
                .ProjectTo<PsProgram>()
                .ToListAsync();
        }

        public async Task<DataWithCount<ICollection<PsProgram>>> PageAsync(
            PerformerSchedulingFilter filter)
        {
            var programs = DbSet.AsNoTracking();

            if (filter.AgeGroupId.HasValue)
            {
                programs = programs.Where(_ => _.AgeGroups
                    .Select(a => a.AgeGroupId).Contains(filter.AgeGroupId.Value));
            }
            if (filter.IsApproved.HasValue)
            {
                programs = programs.Where(_ => _.Performer.IsApproved == filter.IsApproved);
            }

            var count = await programs.CountAsync();

            var programList = await programs
                .OrderBy(_ => _.Performer.Name)
                .ThenBy(_ => _.Title)
                .ApplyPagination(filter)
                .ProjectTo<PsProgram>()
                .ToListAsync();

            return new DataWithCount<ICollection<PsProgram>>
            {
                Data = programList,
                Count = count
            };
        }

        public async Task<List<int>> GetIndexListAsync(int? ageGroupId = null, 
            bool onlyApproved = false)
        {
            var programs = DbSet.AsNoTracking();

            if (ageGroupId.HasValue)
            {
                programs = programs.Where(_ => _.AgeGroups
                    .Select(a => a.AgeGroupId).Contains(ageGroupId.Value));
            }
            if (onlyApproved)
            {
                programs = programs.Where(_ => _.Performer.IsApproved == true);
            }

            return await programs
                .OrderBy(_ => _.Performer.Name)
                .ThenBy(_ => _.Title)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<int> GetCountByPerformerAsync(int performerId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.PerformerId == performerId)
                .CountAsync();
        }

        public async Task<bool> IsValidAgeGroupAsync(int programId, int ageGroupId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == programId
                    && _.AgeGroups.Select(a => a.AgeGroupId).Contains(ageGroupId))
                .AnyAsync();
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

        public async Task<bool> AvailableAtBranchAsync(int programId, int branchId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.Id == programId)
                .GroupJoin(_context.PsPerformerBranches, 
                    program => program.PerformerId, 
                    performerbranches => performerbranches.PsPerformerId, 
                    (program, performerbranches) => new { program, performerbranches })
                .Where(_ => _.program.Performer.AllBranches 
                    || _.performerbranches.Select(b => b.BranchId).Contains(branchId))
                .AnyAsync();
        }
    }
}
