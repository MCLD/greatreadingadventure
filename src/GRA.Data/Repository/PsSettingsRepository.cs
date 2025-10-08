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
    public class PsSettingsRepository
        : AuditingRepository<Model.PsSettings, Domain.Model.PsSettings>, IPsSettingsRepository
    {
        public PsSettingsRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsSettingsRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<PsSettings> GetBySiteIdAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .ProjectToType<PsSettings>()
                .SingleOrDefaultAsync();
        }

        public async Task<DataWithCount<ICollection<Branch>>> PageExcludedBranchesAsync(
            BaseFilter filter)
        {
            var excludedBranches = _context.PsExcludeBranches.AsNoTracking();

            var count = await excludedBranches.CountAsync();

            var branchList = await excludedBranches
                .Select(_ => _.Branch)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectToType<Branch>()
                .ToListAsync();

            return new DataWithCount<ICollection<Branch>>
            {
                Data = branchList,
                Count = count
            };
        }

        public async Task<ICollection<int>> GetExcludedBranchIdsAsync()
        {
            return await _context.PsExcludeBranches
                .AsNoTracking()
                .Select(_ => _.BranchId)
                .ToListAsync();
        }

        public async Task<ICollection<Branch>> GetNonExcludedSystemBranchesAsync(int systemId,
            int? prioritizeBranchId = null)
        {
            var excludedBranches = _context.PsExcludeBranches
                .AsNoTracking()
                .Select(_ => _.BranchId);

            var branches = _context.Branches
                .Where(_ => _.SystemId == systemId && !excludedBranches.Contains(_.Id));

            if (prioritizeBranchId.HasValue)
            {
                branches = branches.OrderByDescending(_ => _.Id == prioritizeBranchId.Value)
                    .ThenBy(_ => _.Name);
            }
            else
            {
                branches = branches.OrderBy(_ => _.Name);
            }

            return await branches
                .ProjectToType<Branch>()
                .ToListAsync();
        }

        public async Task<Branch> GetNonExcludedBranchAsync(int branchId)
        {
            var branchExcluded = _context.PsExcludeBranches
                .AsNoTracking()
                .Where(_ => _.BranchId == branchId);

            return await _context.Branches
                .AsNoTracking()
                .Where(_ => _.Id == branchId && !branchExcluded.Any())
                .ProjectToType<Branch>()
                .FirstOrDefaultAsync();
        }

        public async Task AddBranchExclusionAsync(int branchId)
        {
            var excludedBranch = new Model.PsExcludeBranch
            {
                BranchId = branchId
            };

            await _context.PsExcludeBranches.AddAsync(excludedBranch);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveBranchExclusionAsync(int branchId)
        {
            var excludedBranch = await _context.PsExcludeBranches
                .AsNoTracking()
                .Where(_ => _.BranchId == branchId)
                .FirstOrDefaultAsync();

            _context.PsExcludeBranches.Remove(excludedBranch);
            await _context.SaveChangesAsync();
        }
    }
}
