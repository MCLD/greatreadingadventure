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
    public class PsPerformerRepository
        : AuditingRepository<Model.PsPerformer, Domain.Model.PsPerformer>, IPsPerformerRepository
    {
        public PsPerformerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsPerformerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<PsPerformer> GetByIdAsync(int id, bool onlyApproved = false)
        {
            var performer = DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id);

            if (onlyApproved)
            {
                performer = performer.Where(_ => _.IsApproved);
            }

            return await performer
                .ProjectTo<PsPerformer>()
                .FirstOrDefaultAsync();
        }

        public async Task<PsPerformer> GetByUserIdAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .ProjectTo<PsPerformer>()
                .SingleOrDefaultAsync();     
        }

        public async Task<DataWithCount<ICollection<PsPerformer>>> PageAsync(
            PerformerSchedulingFilter filter)
        {
            var performers = DbSet.AsNoTracking();

            if (filter.IsApproved.HasValue)
            {
                performers = performers.Where(_ => _.IsApproved == filter.IsApproved);
            }

            var count = await performers.CountAsync();

            var performerList = await performers
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<PsPerformer>()
                .ToListAsync();

            return new DataWithCount<ICollection<PsPerformer>>
            {
                Data = performerList,
                Count = count
            };
        }

        public async Task<List<int>> GetIndexListAsync(bool onlyApproved = false)
        {
            var performers = DbSet
                .AsNoTracking();

            if (onlyApproved)
            {
                performers = performers.Where(_ => _.IsApproved == true);
            }

            return await performers
                .OrderBy(_ => _.Name)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<ICollection<PsAgeGroup>> GetPerformerAgeGroupsAsync(int performerId)
        {
            return await _context.PsPrograms
                .AsNoTracking()
                .Where(_ => _.PerformerId == performerId)
                .Select(_ => _.AgeGroups)
                .Distinct()
                .ProjectTo<PsAgeGroup>()
                .ToListAsync();
        }

        public async Task<ICollection<Branch>> GetPerformerBranchesAsync(int performerId)
        {
            return await _context.PsPerformerBranches
                .AsNoTracking()
                .Where(_ => _.PsPerformerId == performerId)
                .Select(_ => _.Branch)
                .ProjectTo<Branch>()
                .ToListAsync();
        }

        public async Task<ICollection<int>> GetPerformerBranchIdsAsync(int performerId,
            int? systemId = null)
        {
            var performerBranches = _context.PsPerformerBranches
                .AsNoTracking()
                .Where(_ => _.PsPerformerId == performerId);

            if (systemId.HasValue)
            {
                performerBranches = performerBranches
                    .Where(_ => _.Branch.SystemId == systemId.Value);
            }

            return await performerBranches
                .Select(_ => _.BranchId)
                .ToListAsync();
        }

        public async Task<bool> GetPerformerSystemAvailability(int performerId, int systemId)
        {
            return await _context.PsPerformerBranches
                .AsNoTracking()
                .Where(_ => _.PsPerformerId == performerId 
                    && (_.PsPerformer.AllBranches || _.Branch.SystemId == systemId))
                .AnyAsync();
        }

        public async Task AddPerformerBranchListAsync(int performerId, List<int> branchIds)
        {
            var performerBranches = new List<Model.PsPerformerBranch>();
            foreach (var branchId in branchIds)
            {
                performerBranches.Add(new Model.PsPerformerBranch
                {
                    BranchId = branchId,
                    PsPerformerId = performerId
                });
            }

            await _context.PsPerformerBranches.AddRangeAsync(performerBranches);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePerformerBranchListAsync(int performerId, List<int> branchIds)
        {
            var performerBranches = _context.PsPerformerBranches
                .AsNoTracking()
                .Where(_ => _.PsPerformerId == performerId && branchIds.Contains(_.BranchId));

            _context.PsPerformerBranches.RemoveRange(performerBranches);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePerformerBranchesAsync(int performerId)
        {
            var performerBranches = _context.PsPerformerBranches
                .Where(_ => _.PsPerformerId == performerId);
            _context.PsPerformerBranches.RemoveRange(performerBranches);
            await _context.SaveChangesAsync();
        }
    }
}
