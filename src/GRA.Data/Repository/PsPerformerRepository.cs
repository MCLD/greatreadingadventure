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
    public class PsPerformerRepository
        : AuditingRepository<Model.PsPerformer, Domain.Model.PsPerformer>, IPsPerformerRepository
    {
        public PsPerformerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsPerformerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public override async Task<PsPerformer> GetByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.Branches)
                    .ThenInclude(_ => _.Branch)
                .Where(_ => _.Id == id)
                .ProjectTo<PsPerformer>()
                .SingleOrDefaultAsync();
        }

        public async Task<PsPerformer> GetByUserIdAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.Branches)
                    .ThenInclude(_ => _.Branch)
                .Where(_ => _.UserId == userId)
                .ProjectTo<PsPerformer>()
                .SingleOrDefaultAsync();     
        }

        public async Task AddPerformerBranchesAsync(int performerId, List<int> branchIds)
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

        public async Task RemovePerformerBranchesAync(int performerId, List<int> branchIds)
        {
            var performerBranches = _context.PsPerformerBranches
                .AsNoTracking()
                .Where(_ => _.PsPerformerId == performerId && branchIds.Contains(_.BranchId));

            _context.PsPerformerBranches.RemoveRange(performerBranches);
            await _context.SaveChangesAsync();
        }
    }
}
