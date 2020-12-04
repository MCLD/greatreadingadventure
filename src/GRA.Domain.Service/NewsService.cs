using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class NewsService : Abstract.BaseUserService<NewsService>
    {
        private readonly IDistributedCache _cache;
        private readonly INewsCategoryRepository _newsCategoryRepository;
        private readonly INewsPostRepository _newsPostRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;

        public NewsService(ILogger<NewsService> logger,
            IDateTimeProvider dateTimeProvider,
            IDistributedCache cache,
            INewsCategoryRepository newsCategoryRepository,
            INewsPostRepository newsPostRepository,
            ISiteRepository siteRepository,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository,
            EmailService emailService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageNews);
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _newsCategoryRepository = newsCategoryRepository
                ?? throw new ArgumentNullException(nameof(newsCategoryRepository));
            _newsPostRepository = newsPostRepository
                ?? throw new ArgumentNullException(nameof(newsPostRepository));
            _siteRepository = siteRepository
                ?? throw new ArgumentNullException(nameof(siteRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _emailService = emailService
                ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task EnsureDefaultCategoryAsync()
        {
            var sites = await _siteRepository.GetAllAsync();
            foreach (var site in sites)
            {
                var defaultCategory = await _newsCategoryRepository
                    .GetDefaultCategoryAsync(site.Id);
                if (defaultCategory == null)
                {
                    var category = new NewsCategory
                    {
                        IsDefault = true,
                        Name = "News",
                        SiteId = site.Id
                    };

                    await _newsCategoryRepository.AddSaveAsync(-1, category);
                }
            }
        }

        public async Task<bool> AnyPublishedPostsAsync()
        {
            return await _newsPostRepository.AnyPublishedPostsAsync(GetCurrentSiteId());
        }

        public async Task<DataWithCount<IEnumerable<NewsPost>>> GetPaginatedPostListAsync(
            NewsFilter filter)
        {
            if (filter.IsActive != true)
            {
                VerifyManagementPermission();
            }

            filter.SiteId = GetClaimId(ClaimType.SiteId);

            return await _newsPostRepository.PageAsync(filter);
        }

        public async Task<NewsPost> GetPostByIdAsync(int id)
        {
            return await _newsPostRepository.GetByIdAsync(id);
        }

        public async Task<NewsPost> CreatePostAsync(NewsPost post, string postUrl,
            bool publish = false)
        {
            VerifyManagementPermission();

            post.Title = post.Title.Trim();
            post.Content = post.Content.Trim();
            if (publish)
            {
                post.PublishedAt = _dateTimeProvider.Now;
                await _newsCategoryRepository.SetLastPostDate(post.CategoryId, _dateTimeProvider.Now);
            }

            var addedPost = await _newsPostRepository
                .AddSaveAsync(GetClaimId(ClaimType.UserId), post);

            if (publish)
            {
                addedPost.CategoryName =
                    (await _newsCategoryRepository.GetByIdAsync(addedPost.CategoryId)).Name;
                await SendSubscriptionEmailsAsync(addedPost, postUrl);
            }

            if (publish)
            {
                _cache.Remove($"s{GetClaimId(ClaimType.SiteId)}.{CacheKey.LatestNewsPostId}");
            }

            return addedPost;
        }

        public async Task<NewsPost> EditPostAsync(NewsPost post, string postUrl,
            bool publish = false)
        {
            VerifyManagementPermission();

            var currentPost = await _newsPostRepository.GetByIdAsync(post.Id);

            currentPost.Title = post.Title.Trim();
            currentPost.Content = post.Content.Trim();
            currentPost.CategoryId = post.CategoryId;

            bool sendSubscriptionEmails = false;
            if (publish && !currentPost.PublishedAt.HasValue)
            {
                currentPost.PublishedAt = _dateTimeProvider.Now;
                sendSubscriptionEmails = true;
            }

            currentPost = await _newsPostRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentPost);

            if (sendSubscriptionEmails)
            {
                currentPost.CategoryName =
                    (await _newsCategoryRepository.GetByIdAsync(currentPost.CategoryId)).Name;
                await SendSubscriptionEmailsAsync(currentPost, postUrl);
            }

            if (publish)
            {
                _cache.Remove($"s{GetClaimId(ClaimType.SiteId)}.{CacheKey.LatestNewsPostId}");
            }

            return currentPost;
        }

        public async Task RemovePostAsync(int postId)
        {
            VerifyManagementPermission();
            await _newsPostRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), postId);
        }

        public async Task<IEnumerable<NewsCategory>> GetAllCategoriesAsync()
        {
            return await _newsCategoryRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<DataWithCount<IEnumerable<NewsCategory>>> GetPaginatedCategoryListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();

            filter.SiteId = GetClaimId(ClaimType.SiteId);

            return await _newsCategoryRepository.PageAsync(filter);
        }

        public async Task<NewsCategory> GetCategoryByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _newsCategoryRepository.GetByIdAsync(id);
        }

        public async Task<NewsCategory> CreateCategoryAsync(NewsCategory category)
        {
            VerifyManagementPermission();

            category.Name = category.Name.Trim();
            category.IsDefault = false;
            category.SiteId = GetCurrentSiteId();

            return await _newsCategoryRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), category);
        }

        public async Task<NewsCategory> EditCategoryAsync(NewsCategory category)
        {
            VerifyManagementPermission();

            var currentCategory = await _newsCategoryRepository.GetByIdAsync(category.Id);

            currentCategory.Name = category.Name.Trim();

            return await _newsCategoryRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentCategory);
        }

        public async Task RemoveCategoryAsync(int categoryId)
        {
            VerifyManagementPermission();

            var category = await _newsCategoryRepository.GetByIdAsync(categoryId);

            if (category.IsDefault)
            {
                throw new GraException("Category is the default category.");
            }
            else if (category.PostCount > 0)
            {
                throw new GraException("Category has posts.");
            }

            await _newsCategoryRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), categoryId);
        }

        private async Task SendSubscriptionEmailsAsync(NewsPost post, string postUrl)
        {
            var siteId = GetCurrentSiteId();
            var site = await _siteRepository.GetByIdAsync(siteId);

            var subscribedUserIds = await _userRepository.GetNewsSubscribedUserIdsAsync(siteId);

            var subject = post.Title;

            string mailBody
                = $"A new post has been made to {site.Name} in the {post.CategoryName} category."
                + $"\r\n\r\nView it in Mission Control:\r\n\r\n  {postUrl}";

            string htmlBody = $"<p style=\"font-size: larger; font-family: sans-serif;\">"
                + $"A new post has been made to {site.Name} in the <a href=\"{postUrl}\">"
                + $"<strong>{post.CategoryName}</strong></a> category.</p>"
                + "<p style=\"font-size: larger; font-family: sans-serif;\">View it in "
                + $"<a href=\"{postUrl}\">Mission Control</a>.</p>";

            foreach (var userId in subscribedUserIds)
            {
                await _emailService.Send(userId, subject, mailBody, htmlBody);
            }
        }

        public async Task<int> GetLatestNewsIdAsync()
        {
            var siteId = GetClaimId(ClaimType.SiteId);
            string cacheKey = $"s{siteId}.{CacheKey.LatestNewsPostId}";

            int lastId;
            var lastIdString = _cache.GetString(cacheKey);
            if (string.IsNullOrEmpty(lastIdString))
            {
                lastId = await _newsPostRepository.GetLatestActiveIdAsync(new BaseFilter
                {
                    SiteId = siteId
                });
                lastIdString = lastId.ToString();
                _cache.SetString(cacheKey, lastId.ToString(), ExpireIn(30));
            }
            else
            {
                lastId = int.Parse(lastIdString);
            }

            return lastId;
        }
        public bool WithinTimeFrame(DateTime date, int daysAllotted)
        {
            return _dateTimeProvider.Now.Date.Subtract(date) <= TimeSpan.FromDays(daysAllotted);
        }
    }
}
