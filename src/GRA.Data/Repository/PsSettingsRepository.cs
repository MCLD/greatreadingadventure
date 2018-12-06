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
