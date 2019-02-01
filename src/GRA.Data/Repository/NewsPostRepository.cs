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
    public class NewsPostRepository
        : AuditingRepository<Model.NewsPost, NewsPost>, INewsPostRepository
    {
        public NewsPostRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<NewsPostRepository> logger) : base(repositoryFacade, logger) { }

        public async Task<bool> AnyPublishedPostsAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Category.SiteId == siteId && _.PublishedAt.HasValue)
                .AnyAsync();
        }

        public async Task<DataWithCount<IEnumerable<NewsPost>>> PageAsync(NewsFilter filter)
        {
            var posts = DbSet
                .AsNoTracking()
                .Where(_ => _.Category.SiteId == filter.SiteId);

            if (filter.IsActive.HasValue)
            {
                posts = posts.Where(_ => _.PublishedAt.HasValue);
            }

            if (filter.CategoryIds?.Count > 0)
            {
                posts = posts.Where(_ => filter.CategoryIds.Contains(_.CategoryId));
            }

            if (filter.DefaultCategory)
            {
                posts = posts.Where(_ => _.Category.IsDefault);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                posts = posts.Where(_ => _.Title.Contains(filter.Search));
            }

            var count = await posts.CountAsync();

            var data = await posts
                .OrderByDescending(_ => !_.PublishedAt.HasValue)
                .ThenByDescending(_ => _.PublishedAt)
                .ApplyPagination(filter)
                .ProjectTo<NewsPost>()
                .ToListAsync();

            return new DataWithCount<IEnumerable<NewsPost>>
            {
                Data = data,
                Count = count
            };
        }

        public async Task<int> GetLatestActiveIdAsync(BaseFilter filter)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Category.SiteId == filter.SiteId
                    && _.PublishedAt.HasValue
                    && _.Category.IsDefault)
                .OrderByDescending(_ => _.PublishedAt)
                .Select(_ => _.Id)
                .FirstOrDefaultAsync();
        }
    }
}
