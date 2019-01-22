using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class NewsService : Abstract.BaseUserService<NewsService>
    {
        private readonly INewsCategoryRepository _newsCategoryRepository;
        private readonly INewsPostRepository _newsPostRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;

        public NewsService(ILogger<NewsService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            INewsCategoryRepository newsCategoryRepository,
            INewsPostRepository newsPostRepository,
            ISiteRepository siteRepository,
            IUserRepository userRepository,
            EmailService emailService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageNews);
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
                        DisplayInSidebar = true,
                        IsDefault = true,
                        Name = "News",
                        SiteId = site.Id
                    };

                    await _newsCategoryRepository.AddSaveAsync(-1, category);
                }
            }
        }

        public async Task<DataWithCount<IEnumerable<NewsPost>>> GetPaginatedPostListAsync(
            BaseFilter filter)
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

        public async Task<NewsPost> CreatePostAsync(NewsPost post, bool publish = false)
        {
            VerifyManagementPermission();

            post.Title = post.Title.Trim();
            post.Content = post.Content.Trim();
            if (publish)
            {
                post.PublishedAt = _dateTimeProvider.Now;
            }

            var addedPost = await _newsPostRepository
                .AddSaveAsync(GetClaimId(ClaimType.UserId), post);

            if (publish)
            {
                await SendSubscriptionEmailsAsync(post);
            }

            return addedPost;
        }

        public async Task<NewsPost> EditPostAsync(NewsPost post, bool publish = false)
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
                await SendSubscriptionEmailsAsync(currentPost);
            }

            return currentPost;
        }

        public async Task RemovePostAsync(int postId)
        {
            VerifyManagementPermission();
            await _newsPostRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), postId);
        }

        public async Task<IEnumerable<NewsCategory>> GetCategoriesAsync()
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
            currentCategory.DisplayInSidebar = category.DisplayInSidebar;

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

        private async Task SendSubscriptionEmailsAsync(NewsPost post)
        {
            var siteId = GetCurrentSiteId();
            var site = await _siteRepository.GetByIdAsync(siteId);

            var subscribedUserIds = await _userRepository.GetNewsSubscribedUserIdsAsync(siteId);

            var postUrl = "";
            var subject = $"A new {site.Name} update has been posted in the {post.CategoryName} category!";
            string mailBody = $"{post.Title} has been posted to the {post.CategoryName} category"
                + $"\r\nThe post can be viewed at this page:"
                + $"\n\r  {postUrl}";

            string htmlBody = $"<p>A new {site.Name} update has been posted in the {post.CategoryName} category!</p>"
                + "<p>Access the "
                + $"<a href=\"{postUrl}\">"
                + "mission control home page</a> to view it.</p>";

            foreach (var userId in subscribedUserIds)
            {
                await _emailService.Send(userId, subject, mailBody, htmlBody);
            }
        }
    }
}
