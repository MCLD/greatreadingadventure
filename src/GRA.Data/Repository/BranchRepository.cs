using GRA.Domain.Repository;
using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class BranchRepository
        : AuditingRepository<Model.Branch, Branch>, IBranchRepository
    {
        public BranchRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<BranchRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Branch>> GetAllAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.System)
                .Where(_ => _.System.SiteId == siteId)
                .OrderBy(_ => _.Name)
                .ProjectTo<Branch>()
                .ToListAsync();
        }

        public async Task<IEnumerable<Branch>> GetBySystemAsync(int systemId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SystemId == systemId)
                .OrderBy(_ => _.Name)
                .ProjectTo<Branch>()
                .ToListAsync();
        }

        public async Task<bool> ValidateAsync(int branchId, int systemId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == branchId && _.SystemId == systemId)
                .AnyAsync();
        }
    }
}
