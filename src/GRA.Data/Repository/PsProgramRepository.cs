using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsProgramRepository
        : AuditingRepository<Model.PsProgram, PsProgram>, IPsProgramRepository
    {
        public PsProgramRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsProgramRepository> logger) : base(repositoryFacade, logger)
        {
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

        public async Task<bool> AvailableAtBranchAsync(int programId, int branchId)
        {
            var performsAtBranch = DbSet
                    .GroupJoin(_context.PsPerformerBranches,
                        program => program.PerformerId,
                        performerbranches => performerbranches.PsPerformerId,
                        (_, performerbranches) => performerbranches)
                    .SelectMany(_ => _)
                    .Where(_ => _.BranchId == branchId);

            return await DbSet.AsNoTracking()
                .Where(_ => _.Id == programId
                    && (_.Performer.AllBranches || performsAtBranch.Any()))
                .AnyAsync();
        }

        public async Task<PsProgram> GetByIdAsync(int id, bool onlyApproved)
        {
            var program = DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id);

            if (onlyApproved)
            {
                program = program.Where(_ => _.IsApproved && _.Performer.IsApproved);
            }

            return await program
                .ProjectToType<PsProgram>()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<PsProgram>> GetByPerformerIdAsync(int performerId,
            bool onlyApproved)
        {
            var programs = DbSet
                .AsNoTracking()
                .Where(_ => _.PerformerId == performerId);

            if (onlyApproved)
            {
                programs = programs.Where(_ => _.IsApproved);
            }

            return await programs
                .ProjectToType<PsProgram>()
                .ToListAsync();
        }

        public async Task<int> GetCountByPerformerAsync(int performerId, bool onlyApproved)
        {
            var programs = DbSet
                .AsNoTracking()
                .Where(_ => _.PerformerId == performerId);

            if (onlyApproved)
            {
                programs = programs.Where(_ => _.IsApproved);
            }

            return await programs.CountAsync();
        }

        public async Task<List<int>> GetIndexListAsync(int? ageGroupId, bool onlyApproved)
        {
            var programs = DbSet.AsNoTracking();

            if (ageGroupId.HasValue)
            {
                programs = programs
                    .Join(_context.PsProgramAgeGroups.Where(_ => _.AgeGroupId == ageGroupId.Value),
                        program => program.Id,
                        ageGroup => ageGroup.ProgramId,
                        (program, _) => program);
            }
            if (onlyApproved)
            {
                programs = programs.Where(_ => _.IsApproved && _.Performer.IsApproved);
            }

            return await programs
                .OrderBy(_ => _.Performer.Name)
                .ThenBy(_ => _.Title)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<ICollection<PsAgeGroup>> GetProgramAgeGroupsAsync(int programId)
        {
            return await _context.PsProgramAgeGroups
                .AsNoTracking()
                .Where(_ => _.ProgramId == programId)
                .Select(_ => _.AgeGroup)
                .ProjectToType<PsAgeGroup>()
                .ToListAsync();
        }

        public async Task<int> GetProgramCountAsync()
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync();
        }

        public async Task<bool> IsValidAgeGroupAsync(int programId, int ageGroupId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == programId)
                .Join(_context.PsProgramAgeGroups.Where(_ => _.AgeGroupId == ageGroupId),
                    program => program.Id,
                    ageGroup => ageGroup.ProgramId,
                    (program, _) => program)
                .AnyAsync();
        }

        public async Task<DataWithCount<ICollection<PsProgram>>> PageAsync(
                                                    PerformerSchedulingFilter filter)
        {
            var programs = DbSet.AsNoTracking();

            if (filter.AgeGroupId.HasValue)
            {
                programs = programs
                    .Join(_context.PsProgramAgeGroups.Where(_ => _.AgeGroupId == filter.AgeGroupId.Value),
                        program => program.Id,
                        ageGroup => ageGroup.ProgramId,
                        (program, _) => program);
            }
            if (filter.IsApproved.HasValue)
            {
                programs = programs.Where(_ => _.IsApproved == filter.IsApproved
                    && _.Performer.IsApproved == filter.IsApproved);
            }

            var count = await programs.CountAsync();

            var programList = await programs
                .OrderBy(_ => _.Performer.Name)
                .ThenBy(_ => _.Title)
                .ApplyPagination(filter)
                .ProjectToType<PsProgram>()
                .ToListAsync();

            return new DataWithCount<ICollection<PsProgram>>
            {
                Data = programList,
                Count = count
            };
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
