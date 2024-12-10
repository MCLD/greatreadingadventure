using System;
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
    public class NewsCategoryRepository
        : AuditingRepository<Model.NewsCategory, NewsCategory>, INewsCategoryRepository
    {
        public NewsCategoryRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<NewsCategoryRepository> logger) : base(repositoryFacade, logger) { }

        public async Task<ICollection<NewsCategory>> GetAllAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderByDescending(_ => _.IsDefault)
                .ThenBy(_ => _.Name)
                .ProjectTo<NewsCategory>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public override async Task<NewsCategory> GetByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .ProjectTo<NewsCategory>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<NewsCategory> GetDefaultCategoryAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.IsDefault)
                .ProjectTo<NewsCategory>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<DataWithCount<IEnumerable<NewsCategory>>> PageAsync(BaseFilter filter)
        {
            var categories = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            return new DataWithCount<IEnumerable<NewsCategory>>
            {
                Count = await categories.CountAsync(),
                Data = await categories
                    .OrderBy(_ => _.Name)
                    .ApplyPagination(filter)
                    .ProjectTo<NewsCategory>(_mapper.ConfigurationProvider)
                    .ToListAsync()
            };
        }

        public async Task SetLastPostDate(int categoryId, DateTime? date)
        {
            var category = await DbSet
                .Where(_ => _.Id == categoryId)
                .SingleOrDefaultAsync();
            category.LastPostDate = date;
            DbSet.Update(category);
        }
    }
}
