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

        public async Task<PsPerformer> GetByUserIdAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .ProjectTo<PsPerformer>()
                .SingleOrDefaultAsync();     
        }

        public async Task<DataWithCount<ICollection<PsPerformer>>> PageAsync(BaseFilter filter)
        {
            var performers = DbSet.AsNoTracking();

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

        public async Task<List<int>> GetIndexListAsync()
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.Name)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<ICollection<Branch>> GetPerformerBranchesAsync(int performerId)
        {
            return await _context.PsPerformerBranches
                .AsNoTracking()
                .Where(_ => _.PsPerformerId == performerId)
                .Select(_ => _.Branch).ProjectTo<Branch>()
                .ToListAsync();
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
