using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class NewsService : Abstract.BaseUserService<NewsService>
    {
        private readonly IDistributedCache _cache;
        private readonly IJobRepository _jobRepository;
        private readonly INewsCategoryRepository _newsCategoryRepository;
        private readonly INewsPostRepository _newsPostRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;

        public NewsService(ILogger<NewsService> logger,
            IDateTimeProvider dateTimeProvider,
            IDistributedCache cache,
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
            if (filter?.IsActive != true)
            {
                VerifyManagementPermission();
            }

            if (filter == null)
            {
                filter = new NewsFilter();
            }

            filter.SiteId = GetClaimId(ClaimType.SiteId);

            return await _newsPostRepository.PageAsync(filter);
        }

        public async Task<NewsPost> GetPostByIdAsync(int id)
        {
            return await _newsPostRepository.GetByIdAsync(id);
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
                addedPost.CategoryName =
                    (await _newsCategoryRepository.GetByIdAsync(addedPost.CategoryId)).Name;

                _cache.Remove($"s{GetClaimId(ClaimType.SiteId)}.{CacheKey.LatestNewsPostId}");
            }

            return addedPost;
        }

        public async Task<NewsPost> EditPostAsync(NewsPost post, bool publish)
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

            var configuredFilter = filter ?? new BaseFilter();
            configuredFilter.SiteId = GetClaimId(ClaimType.SiteId);

            return await _newsCategoryRepository.PageAsync(configuredFilter);
        }

        public async Task<NewsCategory> GetCategoryByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _newsCategoryRepository.GetByIdAsync(id);
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
                _cache.SetString(cacheKey,
                    lastId.ToString(CultureInfo.InvariantCulture),
                    ExpireIn(30));
            }
            else
            {
                lastId = int.Parse(lastIdString, CultureInfo.InvariantCulture);
            }

            return lastId;
        }
        public bool WithinTimeFrame(DateTime date, int daysAllotted)
        {
            return _dateTimeProvider.Now.Date.Subtract(date) <= TimeSpan.FromDays(daysAllotted);
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
                    sw?.Elapsed.TotalMilliseconds);
            });

            var post = await _newsPostRepository.GetByIdAsync(jobDetails.NewsPostId);

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
                .GetNewsSubscribedUserIdsAsync(job.SiteId)).ToList();

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

            var tags = new Dictionary<string, string>
            {
                { "PostCategory", post.CategoryName},
                { nameof(jobDetails.PostLink), jobDetails.PostLink},
                { "PostSummary", post.EmailSummary},
                { "Preview", $"New post in {jobDetails.SiteName} Mission Control!"},
                { nameof(jobDetails.SiteLink), jobDetails.SiteLink},
                { nameof(jobDetails.SiteMcLink), jobDetails.SiteMcLink},
                { nameof(jobDetails.SiteName), jobDetails.SiteName},
            };

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

                var sent = await _emailService.Send(userId,
                    "[{{SiteName}}] {{PostTitle}}",
                    EmailTemplateText,
                    EmailTemplateHtml,
                    tags);

                if (sent)
                {
                    sentEmails++;
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

        private const string EmailTemplateText = @"A new post has been made to {{SiteName}} in the {{PostCategory}} category:

{{PostSummary}}

To read the entire post, view it on Mission Control: {{PostLink}}

-- 
You are receiving this email because you are subscribed to news updates from Mission Control of
{{SiteName}}. You can unsubscribe at any time from Mission Control: {{SiteMcLink}}
";

        private const string EmailTemplateHtml = @"<!doctype html><html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office""><head><title></title><!--[if !mso]><!--><meta http-equiv=""X-UA-Compatible"" content=""IE=edge""><!--<![endif]--><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""><meta name=""viewport"" content=""width=device-width,initial-scale=1""><style type=""text/css"">#outlook a { padding:0; }
          body { margin:0;padding:0;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%; }
          table, td { border-collapse:collapse;mso-table-lspace:0pt;mso-table-rspace:0pt; }
          img { border:0;height:auto;line-height:100%; outline:none;text-decoration:none;-ms-interpolation-mode:bicubic; }
          p { display:block;margin:13px 0; }</style><!--[if mso]>
        <xml>
        <o:OfficeDocumentSettings>
          <o:AllowPNG/>
          <o:PixelsPerInch>96</o:PixelsPerInch>
        </o:OfficeDocumentSettings>
        </xml>
        <![endif]--><!--[if lte mso 11]>
        <style type=""text/css"">
          .mj-outlook-group-fix { width:100% !important; }
        </style>
        <![endif]--><style type=""text/css"">@media only screen and (min-width:480px) {
        .mj-column-per-100 { width:100% !important; max-width: 100%; }
      }</style><style type=""text/css""></style></head><body style=""word-spacing:normal;""><div style=""display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden;"">{{Preview}}</div><div><!--[if mso | IE]><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class="""" style=""width:600px;"" width=""600"" ><tr><td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;""><![endif]--><div style=""margin:0px auto;max-width:600px;""><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""width:100%;""><tbody><tr><td style=""border-bottom:4px solid #cccccc;direction:ltr;font-size:0px;padding:0px;text-align:center;""><!--[if mso | IE]><table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0""><tr><td class="""" style=""vertical-align:top;width:600px;"" ><![endif]--><div class=""mj-column-per-100 mj-outlook-group-fix"" style=""font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;""><table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%""><tbody><tr><td style=""vertical-align:top;padding:0px;""><table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%""><tbody><tr><td align=""center"" style=""font-size:0px;padding:10px;word-break:break-word;""><div style=""font-family:Helvetica, Arial, Verdana, Trebuchet MS, sans-serif;font-size:18px;line-height:22px;text-align:center;text-decoration:none;color:#000000;""><a href=""{{SiteLink}}"" style=""color: #000000; text-decoration: none;""><strong style=""color: #000000; font-weight: normal; text-decoration: none;"">{{SiteName}}</strong></a></div></td></tr></tbody></table></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class="""" style=""width:600px;"" width=""600"" ><tr><td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;""><![endif]--><div style=""margin:0px auto;max-width:600px;""><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""width:100%;""><tbody><tr><td style=""direction:ltr;font-size:0px;padding:15px;text-align:center;""><!--[if mso | IE]><table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0""><tr><td class="""" style=""vertical-align:top;width:570px;"" ><![endif]--><div class=""mj-column-per-100 mj-outlook-group-fix"" style=""font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;""><table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%""><tbody><tr><td style=""vertical-align:top;padding:0px;""><table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%""><tbody><tr><td align=""left"" style=""font-size:0px;padding:0px;word-break:break-word;""><div style=""font-family:Helvetica, Arial, Verdana, Trebuchet MS, sans-serif;font-size:15px;line-height:22px;text-align:left;color:#000000;"">A new post has been made to {{SiteName}} in the {{PostCategory}} category:</div></td></tr><tr><td align=""left"" style=""font-size:0px;padding:15px;word-break:break-word;""><div style=""font-family:Helvetica, Arial, Verdana, Trebuchet MS, sans-serif;font-size:15px;line-height:22px;text-align:left;color:#000000;"">{{PostSummary}}</div></td></tr><tr><td align=""left"" style=""font-size:0px;padding:0px;word-break:break-word;""><div style=""font-family:Helvetica, Arial, Verdana, Trebuchet MS, sans-serif;font-size:15px;line-height:22px;text-align:left;color:#000000;"">To read the entire post, <a href=""{{PostLink}}"">view it in Mission Control</a>.</div></td></tr></tbody></table></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class="""" style=""width:600px;"" width=""600"" ><tr><td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;""><![endif]--><div style=""margin:0px auto;max-width:600px;""><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""width:100%;""><tbody><tr><td style=""border-top:1px solid #cccccc;direction:ltr;font-size:0px;padding:8px 5px;text-align:center;""><!--[if mso | IE]><table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0""><tr><td class="""" style=""vertical-align:top;width:590px;"" ><![endif]--><div class=""mj-column-per-100 mj-outlook-group-fix"" style=""font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;""><table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%""><tbody><tr><td style=""vertical-align:top;padding:0px;""><table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%""><tbody><tr><td align=""left"" style=""font-size:0px;padding:0px;word-break:break-word;""><div style=""font-family:Helvetica, Arial, Verdana, Trebuchet MS, sans-serif;font-size:12px;line-height:22px;text-align:left;color:#000000;""><em>You are receiving this email because you are subscribed to news updates from Mission Control of {{SiteName}}. You can unsubscribe at any time from <a href=""{{SiteMcLink}}"" style=""color: #000000"">Mission Control</a>.</em></div></td></tr></tbody></table></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></div></body></html>";
    }
}
