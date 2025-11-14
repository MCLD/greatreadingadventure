using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class NewsService : Abstract.BaseUserService<NewsService>
    {
        private readonly IGraCache _cache;
        private readonly EmailService _emailService;
        private readonly IJobRepository _jobRepository;
        private readonly INewsCategoryRepository _newsCategoryRepository;
        private readonly INewsPostRepository _newsPostRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRepository _userRepository;

        public NewsService(ILogger<NewsService> logger,
            IDateTimeProvider dateTimeProvider,
            IGraCache cache,
            IJobRepository jobRepository,
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
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
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

        public async Task<bool> AnyPublishedPostsAsync()
        {
            return await _newsPostRepository.AnyPublishedPostsAsync(GetCurrentSiteId());
        }

        public async Task<NewsCategory> CreateCategoryAsync(NewsCategory category)
        {
            VerifyManagementPermission();

            if (category == null)
            {
                throw new GraException("You must provide a category to add.");
            }

            category.Name = category.Name.Trim();
            category.IsDefault = false;
            category.SiteId = GetCurrentSiteId();

            return await _newsCategoryRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), category);
        }

        public async Task<NewsPost> CreatePostAsync(NewsPost post, bool publish)
        {
            VerifyManagementPermission();

            if (post == null)
            {
                throw new GraException("Could not add post: post was empty.");
            }

            post.Title = post.Title.Trim();
            post.Content = post.Content.Trim();
            post.EmailSummary = post.EmailSummary?.Trim();

            if (publish)
            {
                post.PublishedAt = _dateTimeProvider.Now;
                await _newsCategoryRepository.SetLastPostDate(post.CategoryId, _dateTimeProvider.Now);
            }

            var addedPost = await _newsPostRepository
                .AddSaveAsync(GetClaimId(ClaimType.UserId), post);

            if (publish)
            {
                var category = await _newsCategoryRepository.GetByIdAsync(addedPost.CategoryId);
                addedPost.CategoryName = category.Name;

                await _cache.RemoveAsync($"s{GetClaimId(ClaimType.SiteId)}.{CacheKey.LatestNewsPostId}");
            }

            return addedPost;
        }

        public async Task<NewsCategory> EditCategoryAsync(NewsCategory category)
        {
            VerifyManagementPermission();

            if (category == null)
            {
                throw new GraException("You must provide a category to edit.");
            }

            var currentCategory = await _newsCategoryRepository.GetByIdAsync(category.Id);

            currentCategory.Name = category.Name.Trim();

            return await _newsCategoryRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentCategory);
        }

        public async Task<NewsPost> EditPostAsync(NewsPost post, bool publish, bool markUpdated)
        {
            VerifyManagementPermission();

            if (post == null)
            {
                throw new GraException("Could not add post: post was empty.");
            }

            var currentPost = await _newsPostRepository.GetByIdAsync(post.Id);

            currentPost.Title = post.Title.Trim();
            currentPost.Content = post.Content.Trim();
            currentPost.EmailSummary = post.EmailSummary?.Trim();
            currentPost.CategoryId = post.CategoryId;
            currentPost.IsPinned = post.IsPinned;

            bool sendSubscriptionEmails = false;
            if (publish && !currentPost.PublishedAt.HasValue)
            {
                currentPost.PublishedAt = _dateTimeProvider.Now;
                sendSubscriptionEmails = true;
            }

            if (markUpdated)
            {
                currentPost.UpdatedAt = _dateTimeProvider.Now;
                currentPost.UpdatedBy = GetActiveUserId();
            }

            currentPost = await _newsPostRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentPost);

            if (sendSubscriptionEmails)
            {
                currentPost.CategoryName =
                    (await _newsCategoryRepository.GetByIdAsync(currentPost.CategoryId)).Name;
            }

            if (publish)
            {
                await _cache.RemoveAsync($"s{GetClaimId(ClaimType.SiteId)}.{CacheKey.LatestNewsPostId}");
            }

            return currentPost;
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

        public async Task<IEnumerable<NewsCategory>> GetAllCategoriesAsync()
        {
            return await _newsCategoryRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<NewsCategory> GetCategoryByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _newsCategoryRepository.GetByIdAsync(id);
        }

        public async Task<int> GetDefaultCategoryIdAsync()
        {
            var category = await _newsCategoryRepository.GetDefaultCategoryAsync(GetCurrentSiteId());
            return category.Id;
        }

        public async Task<int> GetLatestNewsIdAsync()
        {
            var siteId = GetClaimId(ClaimType.SiteId);
            string cacheKey = $"s{siteId}.{CacheKey.LatestNewsPostId}";

            var lastId = await _cache.GetIntFromCacheAsync(cacheKey);
            if (!lastId.HasValue)
            {
                lastId = await _newsPostRepository.GetLatestActiveIdAsync(new BaseFilter
                {
                    SiteId = siteId
                });
                await _cache.SaveToCacheAsync(cacheKey, lastId, ExpireInTimeSpan(30));
            }

            return lastId.Value;
        }

        public async Task<DataWithCount<IEnumerable<NewsCategory>>> GetPaginatedCategoryListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();

            var configuredFilter = filter ?? new BaseFilter();
            configuredFilter.SiteId = GetClaimId(ClaimType.SiteId);

            return await _newsCategoryRepository.PageAsync(configuredFilter);
        }

        public async Task<DataWithCount<IEnumerable<NewsPost>>> GetPaginatedPostListAsync(
            NewsFilter filter)
        {
            if (filter?.IsActive != true)
            {
                VerifyManagementPermission();
            }

            filter ??= new NewsFilter();

            filter.SiteId = GetClaimId(ClaimType.SiteId);

            return await _newsPostRepository.PageAsync(filter);
        }

        public async Task<NewsPost> GetPostByIdAsync(int id)
        {
            return await GetPostByIdAsync(id, false);
        }

        public async Task<NewsPost> GetPostByIdAsync(int id, bool getBorderingIds)
        {
            return await _newsPostRepository.GetByIdAsync(id, getBorderingIds);
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

        public async Task RemovePostAsync(int postId)
        {
            VerifyManagementPermission();
            await _newsPostRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), postId);
        }

        public async Task<JobStatus> RunSendNewsEmailsJob(int jobId,
            System.Threading.CancellationToken token,
            IProgress<JobStatus> progress)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var job = await _jobRepository.GetByIdAsync(jobId);
            var jobDetails
                = JsonConvert.DeserializeObject<JobSendNewsEmails>(job.SerializedParameters);

            _logger.LogInformation("Job {JobId}: {JobType} to send emails for post {NewsPostId}",
                job.Id,
                job.JobType,
                jobDetails.NewsPostId);

            token.Register(() =>
            {
                _logger.LogWarning("Job {JobId}: {JobType} to send emails for post {NewsPostId} cancelled after {Elapsed} ms",
                    job.Id,
                    job.JobType,
                    jobDetails.NewsPostId,
                    sw?.Elapsed.TotalMilliseconds);
            });

            var post = await _newsPostRepository.GetByIdAsync(jobDetails.NewsPostId);
            if (string.IsNullOrEmpty(post.CategoryName))
            {
                var category = await _newsCategoryRepository.GetByIdAsync(post.CategoryId);
                post.CategoryName = category?.Name;
            }

            if (post == null)
            {
                await _jobRepository.UpdateStatusAsync(jobId,
                    $"Could not locate news post id {jobDetails.NewsPostId} to send emails.");

                return new JobStatus
                {
                    Complete = true,
                    Error = true,
                    Status = $"Could not locate news post id {jobDetails.NewsPostId} to send emails."
                };
            }

            var subscribedUserIds = (await _userRepository
                .GetNewsSubscribedUserIdsAsync(job.SiteId, true)).ToList();

            if (subscribedUserIds.Count == 0)
            {
                await _jobRepository.UpdateStatusAsync(jobId,
                    "No subscribed users to send emails to.");

                return new JobStatus
                {
                    Complete = true,
                    Error = false,
                    Status = "No subscribed users to send emails to."
                };
            }

            int sentEmails = 0;
            var lastUpdate = sw.Elapsed.TotalSeconds;

            await _jobRepository.UpdateStatusAsync(jobId,
                $"Preparing to email {subscribedUserIds.Count} users...");

            progress?.Report(new JobStatus
            {
                PercentComplete = 0,
                Status = $"Preparing to email {subscribedUserIds.Count} users..."
            });

            var directEmailDetails = new DirectEmailDetails(jobDetails.SiteName)
            {
                DirectEmailSystemId = "NewsPost",
                IsBulk = true,
                SendingUserId = await _userRepository.GetSystemUserId()
            };
            directEmailDetails.Tags.Add("Category", post.CategoryName);
            directEmailDetails.Tags.Add("PostLink", jobDetails.PostLink);
            directEmailDetails.Tags.Add("PostTitle", post.Title);
            directEmailDetails.Tags.Add("Summary", post.EmailSummary);
            directEmailDetails.Tags.Add("UnsubscribeLink", jobDetails.SiteMcLink);

            foreach (var userId in subscribedUserIds)
            {
                if (token.IsCancellationRequested)
                {
                    await _jobRepository.UpdateStatusAsync(jobId,
                        $"Cancelling after {sentEmails}/{subscribedUserIds.Count} emails in {sw?.Elapsed.TotalMilliseconds} ms.");
                    return new JobStatus
                    {
                        PercentComplete = sentEmails * 100 / subscribedUserIds.Count,
                        Complete = true,
                        Status = $"Cancelling after {sentEmails}/{subscribedUserIds.Count} emails in {sw?.Elapsed.TotalMilliseconds} ms."
                    };
                }
                directEmailDetails.ToUserId = userId;
                try
                {
                    var history = await _emailService.SendDirectAsync(directEmailDetails);
                    if (history.Successful)
                    {
                        sentEmails++;
                    }
                    else
                    {
                        _logger.LogWarning("Unable to send newsletter notification email to user {UserId}",
                            userId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Unable to send newsletter notification email to user {UserId}: {ErrorMessage}",
                        userId,
                        ex.Message);
                }

                if (sw.Elapsed.TotalSeconds > lastUpdate + 5)
                {
                    await _jobRepository.UpdateStatusAsync(jobId,
                        $"Sent {sentEmails}/{subscribedUserIds.Count} emails...");
                    progress?.Report(new JobStatus
                    {
                        PercentComplete = sentEmails * 100 / subscribedUserIds.Count,
                        Status = $"Sent {sentEmails}/{subscribedUserIds.Count} emails..."
                    });
                    lastUpdate = sw.Elapsed.TotalSeconds;
                }
            }

            await _jobRepository.UpdateStatusAsync(jobId,
                $"Sent emails to {sentEmails}/{subscribedUserIds.Count} users in {sw?.Elapsed.TotalMilliseconds} ms.");

            return new JobStatus
            {
                PercentComplete = sentEmails * 100 / subscribedUserIds.Count,
                Complete = true,
                Status = $"Sent emails to {sentEmails}/{subscribedUserIds.Count} users in {sw?.Elapsed.TotalMilliseconds} ms."
            };
        }

        public bool WithinTimeFrame(DateTime date, int daysAllotted)
        {
            return _dateTimeProvider.Now.Date.Subtract(date) <= TimeSpan.FromDays(daysAllotted);
        }
    }
}
