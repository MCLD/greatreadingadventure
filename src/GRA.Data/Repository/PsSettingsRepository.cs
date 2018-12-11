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
                .ProjectTo<PsSettings>()
                .SingleOrDefaultAsync();
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
                .Where(_ => _.SystemId == systemId && excludedBranches.Contains(_.Id) == false);

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
                .ProjectTo<Branch>()
                .ToListAsync();
        }

        public async Task<Branch> GetNonExcludedBranchAsync(int branchId)
        {
            var branchExcluded = _context.PsExcludeBranches
                .AsNoTracking()
                .Where(_ => _.BranchId == branchId);

            return await _context.Branches
                .AsNoTracking()
                .Where(_ => _.Id == branchId && branchExcluded.Any() == false)
                .ProjectTo<Branch>()
                .FirstOrDefaultAsync();
        }

        public async Task AddBranchExclusionsAsync(List<int> branchIds)
        {
            var excludedBranches = branchIds.Select(_ => new Model.PsExcludeBranch
            {
                BranchId = _
            });

            await _context.PsExcludeBranches.AddRangeAsync(excludedBranches);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveBranchExclusionsAsync(List<int> branchIds)
        {
            var excludedBranches = _context.PsExcludeBranches
                .AsNoTracking()
                .Where(_ => branchIds.Contains(_.BranchId));

            _context.PsExcludeBranches.RemoveRange(excludedBranches);
            await _context.SaveChangesAsync();
        }
    }
}
