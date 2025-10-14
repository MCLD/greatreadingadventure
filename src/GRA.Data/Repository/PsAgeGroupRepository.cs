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
    public class PsAgeGroupRepository
        : AuditingRepository<Model.PsAgeGroup, Domain.Model.PsAgeGroup>, IPsAgeGroupRepository
    {
        public PsAgeGroupRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsAgeGroupRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<PsAgeGroup>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .ProjectToType<PsAgeGroup>()
                .ToListAsync();
        }

        public async Task<DataWithCount<ICollection<PsAgeGroup>>> PageAsync(
            BaseFilter filter)
        {
            var ageGroups = DbSet.AsNoTracking();

            var count = await ageGroups.CountAsync();

            var ageGroupList = await ageGroups
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectToType<PsAgeGroup>()
                .ToListAsync();

            return new DataWithCount<ICollection<PsAgeGroup>>
            {
                Data = ageGroupList,
                Count = count
            };
        }

        public async Task<ICollection<int>> GetAgeGroupBackToBackBranchIdsAsync(int ageGroupId)
        {
            return await _context.PsBackToBack
                .AsNoTracking()
                .Where(_ => _.PsAgeGroupId == ageGroupId)
                .Select(_ => _.BranchId)
                .ToListAsync();
        }

        public async Task AddAgeGroupBackToBackBranchesAsync(int ageGroupId, List<int> branchIds)
        {
            var backToBackBranches = branchIds.Select(_ => new Model.PsBackToBack
            {
                BranchId = _,
                PsAgeGroupId = ageGroupId
            });

            await _context.PsBackToBack.AddRangeAsync(backToBackBranches);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAgeGroupBackToBackBranchesAsync(int ageGroupId, List<int> branchIds)
        {
            var backToBackBraches = _context.PsBackToBack
                .AsNoTracking()
                .Where(_ => _.PsAgeGroupId == ageGroupId && branchIds.Contains(_.BranchId));

            _context.PsBackToBack.RemoveRange(backToBackBraches);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> BranchHasBackToBackAsync(int ageGroupId, int branchId)
        {
            return await _context.PsBackToBack
                .AsNoTracking()
                .Where(_ => _.PsAgeGroupId == ageGroupId && _.BranchId == branchId)
                .AnyAsync();
        }
    }
}
