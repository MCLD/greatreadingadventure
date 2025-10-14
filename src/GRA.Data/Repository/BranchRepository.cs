using System;
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
    public class BranchRepository
        : AuditingRepository<Model.Branch, Branch>, IBranchRepository
    {
        public BranchRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<BranchRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return await ApplyFilters(filter).CountAsync();
        }

        public async Task<IEnumerable<Branch>> GetAllAsync(int siteId,
            bool requireGeolocation = false)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.System.SiteId == siteId
                    && (!requireGeolocation || !string.IsNullOrWhiteSpace(_.Geolocation)))
                .OrderBy(_ => _.Name)
                .ThenBy(_ => _.System.Name)
                .ProjectToType<Branch>()
                .ToListAsync();
        }

        public async Task<IEnumerable<Branch>> GetBySystemAsync(int systemId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SystemId == systemId)
                .OrderBy(_ => _.Name)
                .ProjectToType<Branch>()
                .ToListAsync();
        }

        public async Task<int> IsInUseAsync(int branchId)
        {
            return await _context.Users
                .AsNoTracking()
                .CountAsync(_ => _.BranchId == branchId);
        }

        public async Task<ICollection<Branch>> PageAsync(BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectToType<Branch>()
                .ToListAsync();
        }

        public async Task UpdateCreatedByAsync(int userId, int branchId)
        {
            var branch = DbSet.Where(_ => _.Id == branchId).SingleOrDefault();
            if (branch != null && branch.CreatedBy == Defaults.InitialInsertUserId)
            {
                branch.CreatedBy = userId;
                DbSet.Update(branch);
                await _context.SaveChangesAsync();
            }
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
                .Where(_ => _.Id == branchId && _.System.SiteId == siteId)
                .AnyAsync();
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
    }
}
