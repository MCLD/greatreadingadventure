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
    public class CategoryRepository :
        AuditingRepository<Model.Category, Domain.Model.Category>, ICategoryRepository
    {
        public CategoryRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<CategoryRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Category>> GetAllAsync(int siteId, bool hideEmpty = false)
        {
            var categoryList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId);

            if (hideEmpty)
            {
                var activeChallengeCategories = await _context.Challenges.
                    AsNoTracking()
                    .Include(_ => _.ChallengeCategories)
                    .Where(_ => _.IsActive && _.IsDeleted == false)
                    .SelectMany(_ => _.ChallengeCategories
                        .Select(c => c.CategoryId))
                    .Distinct()
                    .ToListAsync();

                categoryList = categoryList.Where(_ => activeChallengeCategories.Contains(_.Id));
            }

            return await categoryList
                .OrderBy(_ => _.Name)
                .ProjectTo<Category>()
                .ToListAsync();
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<IEnumerable<Category>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<Category>()
                .ToListAsync();
        }

        public IQueryable<Model.Category> ApplyFilters(BaseFilter filter)
        {
            var categoryList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if(!string.IsNullOrWhiteSpace(filter.Search))
            {
                categoryList = categoryList.Where(_ => _.Name.Contains(filter.Search));
            }

            return categoryList;
        }

        public override async Task RemoveSaveAsync(int userId, int categoryId)
        {
            var challengeCategories = await _context.ChallengeCategories
                .Where(_ => _.CategoryId == categoryId)
                .ToListAsync();
            _context.ChallengeCategories.RemoveRange(challengeCategories);

            await base.RemoveSaveAsync(userId, categoryId);
        }
    }
}
