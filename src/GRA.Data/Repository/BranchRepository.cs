using GRA.Domain.Repository;
using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository.Extensions;

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
                .ThenBy(_ => _.System.Name)
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

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<Branch>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<Branch>()
                .ToListAsync();
        }

        private IQueryable<Model.Branch> ApplyFilters(BaseFilter filter)
        {
            var branchList = DbSet
                .AsNoTracking()
                .Where(_ => _.System.SiteId == filter.SiteId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                branchList = branchList.Where(_ => _.Name.Contains(filter.Search)
                || _.System.Name.Contains(filter.Search));
            }

            return branchList;
        }

        public async Task<bool> IsInUseAsync(int branchId)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(_ => _.IsDeleted == false && _.BranchId == branchId);
        }

        public async Task<bool> ValidateAsync(int branchId, int systemId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == branchId && _.SystemId == systemId)
                .AnyAsync();
        }

        public async Task<bool> ValidateBySiteAsync(int branchId, int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.System)
                .Where(_ => _.Id == branchId && _.System.SiteId == siteId)
                .AnyAsync();
        }
    }
}
