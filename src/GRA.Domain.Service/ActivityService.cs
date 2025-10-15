using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Service.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stubble.Core.Builders;

namespace GRA.Domain.Service
{
    public class ActivityService : Abstract.BaseUserService<UserService>
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IAvatarBundleRepository _avatarBundleRepository;
        private readonly IAvatarItemRepository _avatarItemRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IGraCache _cache;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly ICodeSanitizer _codeSanitizer;
        private readonly IEventRepository _eventRepository;
        private readonly IJobRepository _jobRepository;
        private readonly LanguageService _languageService;
        private readonly MailService _mailService;
        private readonly MessageTemplateService _messageTemplateService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPathResolver _pathResolver;
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly PrizeWinnerService _prizeWinnerService;
        private readonly IProgramRepository _programRepository;
        private readonly IRequiredQuestionnaireRepository _requiredQuestionnaireRepository;
        private readonly SiteLookupService _siteLookupService;
        private readonly ITriggerRepository _triggerRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;
        private readonly VendorCodeService _vendorCodeService;
        private readonly IVendorCodeTypeRepository _vendorCodeTypeRepository;

        public ActivityService(IAttachmentRepository attachmentRepository,
            IAvatarBundleRepository avatarBundleRepository,
            IAvatarItemRepository avatarItemRepository,
            IBadgeRepository badgeRepository,
            IBookRepository bookRepository,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            ICodeSanitizer codeSanitizer,
            IDateTimeProvider dateTimeProvider,
            IEventRepository eventRepository,
            IGraCache cache,
            IJobRepository jobRepository,
            ILogger<UserService> logger,
            INotificationRepository notificationRepository,
            IPathResolver pathResolver,
            IPointTranslationRepository pointTranslationRepository,
            IProgramRepository programRepository,
            IRequiredQuestionnaireRepository requiredQuestionnaireRepository,
            ITriggerRepository triggerRepository,
            IUserContextProvider userContext,
            IUserLogRepository userLogRepository,
            IUserRepository userRepository,
            IVendorCodeRepository vendorCodeRepository,
            IVendorCodeTypeRepository vendorCodeTypeRepository,
            LanguageService languageService,
            MailService mailService,
            MessageTemplateService messageTemplateService,
            PrizeWinnerService prizeWinnerService,
            SiteLookupService siteLookupService,
            VendorCodeService vendorCodeService) : base(logger, dateTimeProvider, userContext)
        {
            ArgumentNullException.ThrowIfNull(attachmentRepository);
            ArgumentNullException.ThrowIfNull(avatarBundleRepository);
            ArgumentNullException.ThrowIfNull(avatarItemRepository);
            ArgumentNullException.ThrowIfNull(badgeRepository);
            ArgumentNullException.ThrowIfNull(bookRepository);
            ArgumentNullException.ThrowIfNull(cache);
            ArgumentNullException.ThrowIfNull(challengeRepository);
            ArgumentNullException.ThrowIfNull(challengeTaskRepository);
            ArgumentNullException.ThrowIfNull(codeSanitizer);
            ArgumentNullException.ThrowIfNull(eventRepository);
            ArgumentNullException.ThrowIfNull(jobRepository);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(mailService);
            ArgumentNullException.ThrowIfNull(messageTemplateService);
            ArgumentNullException.ThrowIfNull(notificationRepository);
            ArgumentNullException.ThrowIfNull(pathResolver);
            ArgumentNullException.ThrowIfNull(pointTranslationRepository);
            ArgumentNullException.ThrowIfNull(prizeWinnerService);
            ArgumentNullException.ThrowIfNull(programRepository);
            ArgumentNullException.ThrowIfNull(requiredQuestionnaireRepository);
            ArgumentNullException.ThrowIfNull(siteLookupService);
            ArgumentNullException.ThrowIfNull(triggerRepository);
            ArgumentNullException.ThrowIfNull(userLogRepository);
            ArgumentNullException.ThrowIfNull(userRepository);
            ArgumentNullException.ThrowIfNull(vendorCodeRepository);
            ArgumentNullException.ThrowIfNull(vendorCodeService);
            ArgumentNullException.ThrowIfNull(vendorCodeTypeRepository);

            _attachmentRepository = attachmentRepository;
            _avatarBundleRepository = avatarBundleRepository;
            _avatarItemRepository = avatarItemRepository;
            _badgeRepository = badgeRepository;
            _bookRepository = bookRepository;
            _cache = cache;
            _challengeRepository = challengeRepository;
            _challengeTaskRepository = challengeTaskRepository;
            _codeSanitizer = codeSanitizer;
            _eventRepository = eventRepository;
            _jobRepository = jobRepository;
            _languageService = languageService;
            _mailService = mailService;
            _messageTemplateService = messageTemplateService;
            _notificationRepository = notificationRepository;
            _pathResolver = pathResolver;
            _pointTranslationRepository = pointTranslationRepository;
            _prizeWinnerService = prizeWinnerService;
            _programRepository = programRepository;
            _requiredQuestionnaireRepository = requiredQuestionnaireRepository;
            _siteLookupService = siteLookupService;
            _triggerRepository = triggerRepository;
            _userLogRepository = userLogRepository;
            _userRepository = userRepository;
            _vendorCodeRepository = vendorCodeRepository;
            _vendorCodeService = vendorCodeService;
            _vendorCodeTypeRepository = vendorCodeTypeRepository;
        }

        public async Task<ServiceResult<int>> AddBookAsync(int userId,
            Book book,
            bool addNotification = false)
        {
            VerifyCanLog();
            int activeUserId = GetActiveUserId();
            var activeUser = await _userRepository.GetByIdAsync(activeUserId);
            int authUserId = GetClaimId(ClaimType.UserId);
            var serviceResult = new ServiceResult<int>();

            if (userId != activeUserId
                && activeUser.HouseholdHeadUserId != authUserId
                && !HasPermission(Permission.LogActivityForAny))
            {
                _logger.LogError("User {UserId} doesn't have permission to add a book for {UserIdToLog}.",
                    activeUserId,
                    userId);
                throw new GraException("Permission denied.");
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if ((await _requiredQuestionnaireRepository.GetForUser(GetCurrentSiteId(), user.Id,
                user.Age)).Count > 0)
            {
                _logger.LogError("User id {UserId} cannot add a book for user {UserIdToLog} who has a pending questionnaire.",
                    activeUserId,
                    userId);
                throw new GraException("Books cannot be added while there is a pending questionnaire to be taken.");
            }

            if (book != null)
            {
                book.Title = book.Title?.Trim();
                book.Author = book.Author?.Trim();
            }

            var addedBook = await _bookRepository.GetBookAsync(book) ?? book;
            if (await _bookRepository.UserHasBookAsync(userId, addedBook.Id))
            {
                serviceResult.Status = ServiceResultStatus.Warning;
                serviceResult.Message = $"The book <strong><em>{addedBook.Title}</em></strong> by <strong>{addedBook.Author}</strong> is already on the booklist.";
                serviceResult.Data = addedBook.Id;
            }
            else
            {
                serviceResult.Data
                    = await _bookRepository.AddSaveForUserAsync(activeUserId, userId, addedBook);

                if (addNotification && book != null)
                {
                    var notification = new Notification
                    {
                        UserId = userId,
                        Text = $"The book <strong><em>{book.Title}</em></strong> by <strong>{book.Author}</strong> was added to your book list."
                    };

                    await _notificationRepository.AddSaveAsync(authUserId, notification);
                    serviceResult.Status = ServiceResultStatus.Success;
                }
            }
            return serviceResult;
        }

        public async Task AwardUserTriggersAsync(int userId, bool awardHousehold)
        {
            var userContext = GetUserContext();
            var logPoints = userContext.SiteStage == SiteStage.ProgramOpen;

            await AwardTriggersAsync(userId, logPoints, userContext.SiteId,
                !userContext.User.Identity.IsAuthenticated);

            if (awardHousehold)
            {
                var householdMemebers = await _userRepository.GetHouseholdAsync(userId);
                if (householdMemebers.Any())
                {
                    foreach (var member in householdMemebers)
                    {
                        await AwardTriggersAsync(member.Id, logPoints, userContext.SiteId,
                            !userContext.User.Identity.IsAuthenticated);
                    }
                }
            }
        }

        public async Task<JobStatus> BulkReassignCodes(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress)
        {
            var requestingUser = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.ManageVendorCodes))
            {
                var sw = Stopwatch.StartNew();

                var job = await _jobRepository.GetByIdAsync(jobId);
                var jobDetails
                    = JsonConvert
                        .DeserializeObject<JobDetailsVendorCodeBulkReassignment>(job.SerializedParameters);

                var filename = jobDetails.Filename;
                var reason = jobDetails.Reason;

                token.Register(() =>
                {
                    _logger.LogWarning("Import of {FilePath} for user {UserId} was cancelled after {Elapsed} ms",
                        filename,
                        requestingUser,
                        sw?.Elapsed.TotalMilliseconds);
                });

                string fullPath = _pathResolver.ResolvePrivateTempFilePath(filename);

                if (!System.IO.File.Exists(fullPath))
                {
                    _logger.LogError("Could not find {FilePath}", fullPath);
                    return new JobStatus
                    {
                        PercentComplete = 0,
                        Status = "Could not find the import file.",
                        Error = true,
                        Complete = true
                    };
                }

                try
                {
                    // perform the import
                    int row = 0;
                    int success = 0;
                    var lines = System.IO.File.ReadLines(fullPath);
                    var totalRows = lines.Count();
                    var issues = new List<string>();

                    foreach (var line in lines)
                    {
                        row++;
                        if (row % 10 == 0 || row == 1)
                        {
                            await _jobRepository.UpdateStatusAsync(jobId,
                                $"Processing line {row}/{totalRows}...");

                            progress?.Report(new JobStatus
                            {
                                PercentComplete = row * 100 / totalRows,
                                Status = $"Processing line {row}/{totalRows}...",
                                Error = false
                            });
                        }

                        try
                        {
                            var vendorCode = await _vendorCodeService.GetVendorCodeByCode(line)
                                ?? throw new GraException($"Could not find vendor code {line}.");

                            if (!vendorCode.UserId.HasValue)
                            {
                                throw new GraException($"Vendor code {line} is not associated with a participant.");
                            }

                            int userId = vendorCode.UserId.Value;

                            var user = await _userRepository.GetByIdAsync(userId);
                            if (user?.IsDeleted != false)
                            {
                                throw new GraException($"User id {userId} associated with code {line} has been deleted.");
                            }

                            var prizeWinner = await _prizeWinnerService
                                .GetPrizeForVendorCodeAsync(vendorCode.Id);

                            if (prizeWinner != null)
                            {
                                try
                                {
                                    await _prizeWinnerService.RemovePrizeAsync(prizeWinner.Id);
                                }
                                catch (GraException gex)
                                {
                                    if (!issues.Contains(gex.Message))
                                    {
                                        issues.Add(gex.Message);
                                    }
                                }
                            }

                            await _vendorCodeService.AssociateAsync(vendorCode.Id, reason);
                            await MCAwardVendorCodeAsync(userId, vendorCode.VendorCodeTypeId);
                            success++;
                        }
                        catch (GraException gex)
                        {
                            issues.Add(gex.Message);
                        }
                    }

                    if (token.IsCancellationRequested)
                    {
                        await _jobRepository.UpdateStatusAsync(jobId,
                            $"Import cancelled at line {row}/{totalRows}.");

                        return new JobStatus
                        {
                            Status = $"Operation cancelled at line {row}."
                        };
                    }

                    await _jobRepository.UpdateStatusAsync(jobId,
                        $"Updated {success} records, {issues?.Count ?? 0} issues of {totalRows} total lines.");

                    _logger.LogInformation("Import of {FileName} completed: {UpdatedRecords} updates, {IssueCount} issues, of {TotalRows} total lines in {Elapsed} ms",
                        filename,
                        success,
                        issues?.Count ?? 0,
                        totalRows,
                        sw?.ElapsedMilliseconds ?? 0);

                    var sb = new StringBuilder("<strong>Import complete</strong>");
                    if (success > 0)
                    {
                        sb.Append(": ").Append(success).Append(" records were updated");
                    }
                    if (issues?.Count > 0)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(", ");
                        }
                        sb.Append(issues.Count).Append(" issues encountered");
                    }
                    sb.Append('.');

                    if (issues.Count > 0)
                    {
                        sb.Append(" Issues detected:<ul>");
                        foreach (string issue in issues)
                        {
                            sb.Append("<li>").Append(issue).Append("</li>");
                        }
                        sb.Append("</ul>");
                        return new JobStatus
                        {
                            PercentComplete = 100,
                            Complete = true,
                            Status = sb.ToString(),
                            Error = true
                        };
                    }
                    else
                    {
                        return new JobStatus
                        {
                            PercentComplete = 100,
                            Complete = true,
                            Status = sb.ToString(),
                        };
                    }
                }
                finally
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to bulk reassign codes.",
                    requestingUser);
                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true,
                    Complete = true
                };
            }
        }

        public async Task<int> GetActivityEarnedAsync()
        {
            return await _userLogRepository.GetActivityEarnedForUserAsync(GetActiveUserId());
        }

        public async Task<int?> GetSiteActivityEarnedAsync()
        {
            string cacheKey = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                CacheKey.SiteActivityEarned,
                GetCurrentSiteId());

            var siteActivityEarned = await _cache.GetIntFromCacheAsync(cacheKey);

            if (siteActivityEarned == null || siteActivityEarned.Value == 0)
            {
                siteActivityEarned
                    = await _userLogRepository.GetSiteActivityEarnedAsync(GetCurrentSiteId());

                await _cache.SaveToCacheAsync(cacheKey,
                    siteActivityEarned,
                    ExpireInTimeSpan(5));
            }

            return siteActivityEarned;
        }

        public async Task<PointTranslation> GetUserPointTranslationAsync()
        {
            var user = await _userRepository.GetByIdAsync(GetActiveUserId());
            return await _pointTranslationRepository.GetByProgramIdAsync(user.ProgramId);
        }

        public bool IsOpenToLog()
        {
            return OpenToLog();
        }

        public async Task<ActivityLogResult> LogActivityAsync(int userIdToLog,
            int activityAmountEarned,
            Book book = null)
        {
            VerifyCanLog();

            if (activityAmountEarned < 1)
            {
                throw new GraException("Cannot log activity amounts less than 1!");
            }

            if (book != null && string.IsNullOrWhiteSpace(book.Title)
                    && !string.IsNullOrWhiteSpace(book.Author))
            {
                throw new GraException("When providing an author you must also provide a title.");
            }

            int activeUserId = GetActiveUserId();
            int authUserId = GetClaimId(ClaimType.UserId);
            var userToLog = await _userRepository.GetByIdAsync(userIdToLog);

            bool loggingAsAdminUser = HasPermission(Permission.LogActivityForAny);

            if (activeUserId != userIdToLog
                && authUserId != userIdToLog
                && authUserId != userToLog.HouseholdHeadUserId
                && !loggingAsAdminUser)
            {
                _logger.LogError("User id {UserId} cannot log activity for user id {UserIdToLog}",
                    activeUserId,
                    userIdToLog);
                throw new GraException("Permission denied.");
            }

            if ((await _requiredQuestionnaireRepository.GetForUser(GetCurrentSiteId(), userToLog.Id,
                userToLog.Age)).Count > 0)
            {
                _logger.LogError("User id {UserId} cannot log activity for user id {UserIdToLog} who has a pending questionnaire.",
                    activeUserId,
                    userIdToLog);
                throw new GraException("Activity cannot be logged while there is a pending questionnaire to be taken.");
            }

            var activityLogResult = new ActivityLogResult();

            if (book != null && !string.IsNullOrWhiteSpace(book.Title))
            {
                activityLogResult.BookId = (await AddBookAsync(GetActiveUserId(), book, true)).Data;
            }

            var maximumActivity = await GetMaximumAllowedActivityAsync(userToLog.SiteId);
            var userActivityAmount = await _userLogRepository.GetActivityEarnedForUserAsync(
                userToLog.Id);
            var totalActivity = Convert.ToInt64(userActivityAmount)
                + Convert.ToInt64(activityAmountEarned);
            var activityAmountToAdd = activityAmountEarned;
            if (totalActivity > maximumActivity)
            {
                activityAmountToAdd = maximumActivity - userActivityAmount;
            }

            if (activityAmountToAdd > 0)
            {
                var translation
                    = await _pointTranslationRepository.GetByProgramIdAsync(userToLog.ProgramId);

                int pointsEarned
                    = (activityAmountToAdd / translation.ActivityAmount) * translation.PointsEarned;

                // cap points at setting or int.MaxValue
                long totalPoints = Convert.ToInt64(userToLog.PointsEarned)
                    + Convert.ToInt64(pointsEarned);
                var maximumPoints = await GetMaximumAllowedPointsAsync(userToLog.SiteId);
                if (totalPoints > maximumPoints)
                {
                    pointsEarned = maximumPoints - userToLog.PointsEarned;
                }

                // add the row to the user's point log
                var userLog = new UserLog
                {
                    ActivityEarned = activityAmountToAdd,
                    IsDeleted = false,
                    PointsEarned = pointsEarned,
                    UserId = userToLog.Id,
                    PointTranslationId = translation.Id
                };
                if (activeUserId != userToLog.Id)
                {
                    userLog.AwardedBy = activeUserId;
                }

                // prepare the notification text
                string activityDescription = "for <strong>";
                if (translation.TranslationDescriptionPresentTense.Contains("{0}",
                    StringComparison.OrdinalIgnoreCase))
                {
                    activityDescription += string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        translation.TranslationDescriptionPresentTense,
                        userLog.ActivityEarned);
                    if (userLog.ActivityEarned == 1)
                    {
                        activityDescription += $" {translation.ActivityDescription}";
                    }
                    else
                    {
                        activityDescription += $" {translation.ActivityDescriptionPlural}";
                    }
                }
                else
                {
                    activityDescription = $"{translation.TranslationDescriptionPresentTense} {translation.ActivityDescription}";
                }
                activityDescription += "</strong>";

                var pointsText = pointsEarned == 1
                    ? "point"
                    : "points";

                // create the notification record
                var notification = new Notification
                {
                    PointsEarned = pointsEarned,
                    Text = $"<span class=\"far fa-star\"></span> You earned <strong>{pointsEarned} {pointsText}</strong> {activityDescription}!",
                    UserId = userToLog.Id
                };

                // add the book if one was supplied
                if (book != null && !string.IsNullOrWhiteSpace(book.Title))
                {
                    notification.Text += $" The book <strong><em>{book.Title}</em></strong> by <strong>{book.Author}</strong> was added to your book list.";
                    userLog.BookId = activityLogResult.BookId;
                }

                userLog = await _userLogRepository.AddSaveAsync(activeUserId, userLog);

                activityLogResult.UserLogId = userLog.Id;

                // update the score in the user record
                userToLog = await AddPointsSaveAsync(authUserId,
                    activeUserId,
                    userToLog.Id,
                    pointsEarned);

                await _notificationRepository.AddSaveAsync(authUserId, notification);

                // add userlog and notification if the max activity amount has been reached
                if (activityAmountToAdd < activityAmountEarned || totalActivity == maximumActivity)
                {
                    string descriptionPastTense = translation.TranslationDescriptionPastTense
                        .Replace("{0}", "", StringComparison.InvariantCulture)
                        .Trim();

                    var maxDescription = $"Congratulations, you have {descriptionPastTense} the maximum amount of {translation.ActivityDescriptionPlural} for this program. Good job!";

                    var maxActivityLog = new UserLog
                    {
                        Description = maxDescription,
                        IsDeleted = false,
                        UserId = userToLog.Id,
                    };

                    await _userLogRepository.AddSaveAsync(activeUserId, maxActivityLog);

                    var maxNotification = new Notification
                    {
                        Text = maxDescription,
                        UserId = userToLog.Id
                    };

                    await _notificationRepository.AddSaveAsync(authUserId, maxNotification);
                }
            }

            return activityLogResult;
        }

        public async Task LogHouseholdActivityAsync(ICollection<int> userIds, int activityAmount)
        {
            VerifyCanLog();

            if (activityAmount < 1)
            {
                throw new GraException("Amount must be at least 1.");
            }

            ArgumentNullException.ThrowIfNull(userIds);

            int authUserId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.LogActivityForAny))
            {
                var authUser = await _userRepository.GetByIdAsync(authUserId);
                if (authUser.HouseholdHeadUserId.HasValue)
                {
                    _logger.LogError("User id {UserId} cannot log activity for a household",
                        authUserId);
                    throw new GraException("Permission denied.");
                }

                var householdList = (await _userRepository.GetHouseholdAsync(authUserId))
                .Select(_ => _.Id).ToList();
                householdList.Add(authUserId);
                if (userIds.Except(householdList).Any())
                {
                    _logger.LogError("User id {UserId} cannot log activity for {UserIdList}",
                        authUserId,
                        userIds.Except(householdList).First());
                    throw new GraException("Permission denied.");
                }
            }

            foreach (var userId in userIds)
            {
                await LogActivityAsync(userId, activityAmount);
            }
        }

        public async Task<bool> LogHouseholdSecretCodeAsync(ICollection<int> userIds,
            string secretCode)
        {
            VerifyCanLog();

            if (string.IsNullOrWhiteSpace(secretCode))
            {
                throw new GraException(Annotations.Required.SecretCode);
            }

            ArgumentNullException.ThrowIfNull(userIds);

            secretCode = _codeSanitizer.Sanitize(secretCode);

            int authUserId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.LogActivityForAny))
            {
                var authUser = await _userRepository.GetByIdAsync(authUserId);
                if (authUser.HouseholdHeadUserId.HasValue)
                {
                    _logger.LogError("User id {UserId} cannot log codes for a family/group",
                        authUserId);
                    throw new GraException("Permission denied.");
                }

                var householdList = (await _userRepository.GetHouseholdAsync(authUserId))
                    .Select(_ => _.Id).ToList();
                householdList.Add(authUserId);
                if (userIds.Except(householdList).Any())
                {
                    _logger.LogError("User id {UserId} cannot log codes for {UserIdList}",
                        authUserId,
                        userIds.Except(householdList).First());
                    throw new GraException("Permission denied.");
                }
            }

            if (await _triggerRepository
                .GetByCodeAsync(GetCurrentSiteId(), secretCode, true) == null)
            {
                throw new GraException($"<strong>{secretCode}</strong> is not a valid code.");
            }

            var codeApplied = false;

            foreach (var userId in userIds)
            {
                if (await LogSecretCodeAsync(userId, secretCode, true))
                {
                    codeApplied = true;
                }
            }
            return codeApplied;
        }

        public async Task<bool> LogSecretCodeAsync(int userIdToLog,
            string secretCode,
            bool householdLogging = false)
        {
            VerifyCanLog();

            if (string.IsNullOrWhiteSpace(secretCode))
            {
                throw new GraException(Annotations.Required.SecretCode);
            }

            secretCode = _codeSanitizer.Sanitize(secretCode);

            int activeUserId = GetActiveUserId();
            int authUserId = GetClaimId(ClaimType.UserId);
            var userToLog = await _userRepository.GetByIdAsync(userIdToLog);

            bool loggingAsAdminUser = HasPermission(Permission.LogActivityForAny);

            if (activeUserId != userIdToLog
                && authUserId != userIdToLog
                && authUserId != userToLog.HouseholdHeadUserId
                && !loggingAsAdminUser)
            {
                _logger.LogError("User id {UserId} cannot log a code for user id {UserIdToLog}",
                    activeUserId,
                    userIdToLog);
                throw new GraException("You do not have permission to apply that code.");
            }

            if ((await _requiredQuestionnaireRepository.GetForUser(GetCurrentSiteId(), userToLog.Id,
                userToLog.Age)).Count > 0)
            {
                _logger.LogError("User id {UserId} cannot log a code for user id {UserIdToLog} who has a pending questionnaire",
                    activeUserId,
                    userToLog.Id);
                throw new GraException("Secret codes cannot be entered while there is a pending questionnaire to be taken.");
            }

            var trigger = await _triggerRepository.GetByCodeAsync(GetCurrentSiteId(), secretCode,
                true)
                ?? throw new GraException($"<strong>{secretCode}</strong> is not a valid code.");

            var pointsAwarded = trigger.AwardPoints;

            // cap points at setting or int.MaxValue
            long totalPoints = Convert.ToInt64(userToLog.PointsEarned)
                + Convert.ToInt64(pointsAwarded);
            var maximumPoints = await GetMaximumAllowedPointsAsync(userToLog.SiteId);

            if (totalPoints > maximumPoints)
            {
                pointsAwarded = maximumPoints - userToLog.PointsEarned;
            }

            // check if this user's gotten this code
            var alreadyDone
                = await _triggerRepository.CheckTriggerActivationAsync(userIdToLog, trigger.Id);
            if (alreadyDone != null)
            {
                if (householdLogging)
                {
                    return false;
                }
                else
                {
                    throw new GraException($"You already entered the code <strong>{secretCode}</strong> on <strong>{alreadyDone:d}</strong>!");
                }
            }

            // add that we've processed this trigger for this user
            await _triggerRepository.AddTriggerActivationAsync(userIdToLog, trigger.Id);

            // every trigger awards a badge
            var badge = await AwardBadgeAsync(userIdToLog, trigger.AwardBadgeId);
            var attachment = trigger.AwardAttachmentId.HasValue
                ? await _attachmentRepository.GetByIdAsync(trigger.AwardAttachmentId.Value)
                : null;

            // log the notification
            await _notificationRepository.AddSaveAsync(authUserId, new Notification
            {
                PointsEarned = pointsAwarded,
                UserId = userIdToLog,
                Text = trigger.AwardMessage,
                BadgeId = trigger.AwardBadgeId,
                BadgeFilename = badge.Filename,
                AttachmentFilename = attachment?.FileName
            });

            // find if the trigger is related to an event
            var relatedEvents = await _eventRepository.GetRelatedEventsForTriggerAsync(
                    trigger.Id);
            var relatedEventId = relatedEvents.FirstOrDefault()?.Id;

            // add the award to the user's history
            var userLog = new UserLog
            {
                UserId = userIdToLog,
                PointsEarned = pointsAwarded,
                IsDeleted = false,
                BadgeId = trigger.AwardBadgeId,
                EventId = relatedEventId,
                TriggerId = trigger.Id,
                Description = trigger.AwardMessage,
                AttachmentId = trigger.AwardAttachmentId
            };

            if (activeUserId != userToLog.Id)
            {
                userLog.AwardedBy = activeUserId;
            }

            await _userLogRepository.AddSaveAsync(authUserId, userLog);

            // award any vendor code that is necessary
            var assignedCode = await AwardVendorCodeAsync(userIdToLog,
                trigger.AwardVendorCodeTypeId);

            if (assignedCode != null)
            {
                _logger.LogInformation("Trigger based on code {SecretCode} (trigger id {TriggerId}) assigned vendor code {Code} to user {UserId}",
                    secretCode,
                    trigger?.Id,
                    assignedCode.Code,
                    userIdToLog);
            }

            // award any avatar bundle that is necessary
            if (trigger.AwardAvatarBundleId.HasValue)
            {
                await AwardUserBundle(userIdToLog, trigger.AwardAvatarBundleId.Value);
            }

            // send mail if applicable
            int? mailId = await SendMailAsync(userIdToLog, trigger);

            // award prize if applicable
            await AwardPrizeAsync(userIdToLog, trigger, mailId);

            // if there are points to be awarded, do that now, also check for other triggers
            if (pointsAwarded > 0)
            {
                await AddPointsSaveAsync(authUserId,
                    activeUserId,
                    userIdToLog,
                    pointsAwarded);
            }
            else
            {
                await AwardTriggersAsync(userIdToLog);
            }
            return true;
        }

        public async Task MCAwardVendorCodeAsync(int userId, int vendorCodeTypeId)
        {
            var authUserId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.ManageVendorCodes))
            {
                _logger.LogError("User {UserId} cannot award vendor codes.", authUserId);
                throw new GraException("Permission denied.");
            }

            var assignedCode = await AwardVendorCodeAsync(userId, vendorCodeTypeId);

            if (assignedCode != null)
            {
                _logger.LogInformation("Mission control user id {AuthUserId} assigned vendor code {Code} to user {UserId}",
                    authUserId,
                    assignedCode.Code,
                    userId);
            }
        }

        public async Task<User> RemoveActivityAsync(int userIdToLog, int userLogIdToRemove)
        {
            int activeUserId = GetActiveUserId();
            var activeUser = await _userRepository.GetByIdAsync(activeUserId);
            int authUserId = GetClaimId(ClaimType.UserId);

            if (userIdToLog == activeUserId
                || activeUser.HouseholdHeadUserId == authUserId
                || HasPermission(Permission.LogActivityForAny))
            {
                var userLog = await _userLogRepository.GetByIdAsync(userLogIdToRemove);
                Trigger trigger = null;
                PrizeWinner prize = null;

                if (userLog.AvatarBundleId.HasValue)
                {
                    throw new GraException("Avatar bundles cannot be removed.");
                }
                else if (userLog.BadgeId.HasValue && !userLog.ChallengeId.HasValue)
                {
                    trigger = await _triggerRepository.GetByBadgeIdAsync(userLog.BadgeId.Value);
                    if (trigger == null)
                    {
                        throw new GraException("This badge cannot be removed.");
                    }
                    else if (trigger.AwardAvatarBundleId.HasValue
                        || trigger.AwardVendorCodeTypeId.HasValue
                        || !string.IsNullOrWhiteSpace(trigger.AwardMail))
                    {
                        throw new GraException("Trigger has non-prize awards.");
                    }
                    else
                    {
                        prize = await _prizeWinnerService.GetUserTriggerPrizeAsync(userIdToLog,
                            trigger.Id);
                        if (prize?.RedeemedAt.HasValue == true)
                        {
                            throw new GraException("The prize for this trigger has already been reedeemed.");
                        }
                    }
                }

                int pointsToRemove = userLog.PointsEarned;
                await _userLogRepository.RemoveSaveAsync(authUserId, userLogIdToRemove);
                if (userLog.ChallengeId.HasValue)
                {
                    await _challengeTaskRepository.UnsetUserChallengeTasksAsync(userIdToLog,
                        userLog.ChallengeId.Value);
                }
                if (userLog.BadgeId.HasValue)
                {
                    await _badgeRepository.RemoveUserBadgeAsync(userIdToLog, userLog.BadgeId.Value);
                }
                if (trigger != null)
                {
                    await _triggerRepository.RemoveUserTriggerAsync(userIdToLog, trigger.Id);
                }
                if (prize != null)
                {
                    await _prizeWinnerService.RemovePrizeAsync(prize.Id);
                }
                return await RemovePointsSaveAsync(authUserId, userIdToLog, pointsToRemove);
            }
            else
            {
                _logger.LogError(
                    "User id {UserId} cannot remove activity for user id {UserIdToLog}",
                    authUserId,
                    userIdToLog);
                throw new GraException($"User id {authUserId} cannot remove activity for user id {userIdToLog}");
            }
        }

        public async Task RemoveBookAsync(int bookId, int? userId = null)
        {
            var authUserId = GetClaimId(ClaimType.UserId);
            var activeUserId = GetActiveUserId();
            var forUserId = userId ?? activeUserId;
            if (HasPermission(Permission.LogActivityForAny)
                || await _bookRepository.UserHasBookAsync(activeUserId, bookId))
            {
                await _bookRepository.RemoveForUserAsync(authUserId, forUserId, bookId);
                var bookUserCount = await _bookRepository.GetUserCountForBookAsync(bookId);
                if (bookUserCount == 0)
                {
                    await _bookRepository.RemoveSaveAsync(authUserId, bookId);
                }
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to remove book {BookId} for user {UserIdToLog}.",
                    authUserId,
                    bookId,
                    forUserId);
                throw new GraException("Permission denied.");
            }
        }

        public async Task UpdateBookAsync(Book book, int? userId = null)
        {
            ArgumentNullException.ThrowIfNull(book);

            var authUserId = GetClaimId(ClaimType.UserId);
            var activeUserId = GetActiveUserId();
            var forUserId = userId ?? activeUserId;
            if (HasPermission(Permission.LogActivityForAny)
                || await _bookRepository.UserHasBookAsync(activeUserId, book.Id))
            {
                var bookUserCount = await _bookRepository.GetUserCountForBookAsync(book.Id);
                if (bookUserCount > 1)
                {
                    var newBook = new Book
                    {
                        Title = book.Title,
                        Author = book.Author,
                        Isbn = book.Isbn,
                        Url = book.Url
                    };
                    await _bookRepository.AddSaveForUserAsync(authUserId, forUserId, newBook);
                    await _bookRepository.RemoveForUserAsync(authUserId, forUserId, book.Id);
                }
                else
                {
                    await _bookRepository.UpdateSaveAsync(authUserId, book);
                }
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to edit book {BookId} for user {UserIdToLog}.",
                    authUserId,
                    book.Id,
                    forUserId);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<bool> UpdateChallengeTasksAsync(int challengeId,
            IEnumerable<ChallengeTask> challengeTasks)
        {
            VerifyCanLog();
            int activeUserId = GetActiveUserId();
            int authUserId = GetClaimId(ClaimType.UserId);

            var activeUser = await _userRepository.GetByIdAsync(activeUserId);

            if ((await _requiredQuestionnaireRepository.GetForUser(GetCurrentSiteId(),
                activeUser.Id,
                activeUser.Age)).Count > 0)
            {
                _logger.LogError("User id {UserId} cannot complete challenges tasks while having a pending questionnaire.",
                    activeUserId);
                throw new GraException("Challenge tasks cannot be completed while there is a pending questionnaire to be taken.");
            }

            var challenge
                = await _challengeRepository.GetActiveByIdAsync(challengeId, activeUserId);

            if (challenge.IsCompleted == true)
            {
                _logger.LogError("User {UserId} cannot make changes to completed challenge id {ChallengeId}.",
                    authUserId,
                    challengeId);
                throw new GraException("Challenge is already completed.");
            }

            var updateStatuses
                = await _challengeRepository.UpdateUserChallengeTasksAsync(activeUserId,
                    challengeTasks);

            // re-fetch challenge with tasks completed
            challenge = await _challengeRepository.GetActiveByIdAsync(challengeId, activeUserId);

            // loop tasks to see if we need to perform any additional point translation/book tasks
            foreach (var updateStatus in updateStatuses)
            {
                var challengeTaskDetails = challenge.Tasks
                    .SingleOrDefault(_ => _.Id == updateStatus.ChallengeTask.Id);
                // is there work we need to do on this item
                if ((challengeTaskDetails.ActivityCount > 0
                    && challengeTaskDetails.PointTranslationId != null)
                    || challengeTaskDetails.ChallengeTaskType == ChallengeTaskType.Book)
                {
                    // did something change?
                    _logger.LogTrace("Challenge task {ChallengeTaskId} counts as an activity",
                        updateStatus.ChallengeTask.Id);
                    if (updateStatus.WasComplete != updateStatus.IsComplete)
                    {
                        _logger.LogTrace("Status of {ChallengeTaskId}: was {WasComplete}, is {IsComplete}",
                            updateStatus.ChallengeTask.Id,
                            updateStatus.WasComplete,
                            updateStatus.IsComplete);
                        if (updateStatus.IsComplete)
                        {
                            // person completed the task
                            Book book = null;
                            if (challengeTaskDetails.ChallengeTaskType == ChallengeTaskType.Book)
                            {
                                book = new Book
                                {
                                    Title = updateStatus.ChallengeTask.Title,
                                    Author = updateStatus.ChallengeTask.Author,
                                    ChallengeId = challenge.Id
                                };
                            }

                            if (challengeTaskDetails.ActivityCount > 0
                                && challengeTaskDetails.PointTranslationId != null)
                            {
                                _logger.LogDebug("Logging activity for {UserId} based on challenge task {ChallengeTaskId}",
                                    activeUserId,
                                    updateStatus.ChallengeTask.Id);
                                var userLogResult = await LogActivityAsync(activeUserId,
                                    (int)challengeTaskDetails.ActivityCount,
                                    book);

                                // update record with user log result
                                _logger.LogDebug("Update success, recording UserLogId {UserLogId} and BookId {BookId}",
                                    userLogResult.UserLogId,
                                    userLogResult.BookId);
                                await _challengeRepository.UpdateUserChallengeTaskAsync(
                                    activeUserId,
                                    updateStatus.ChallengeTask.Id,
                                    userLogResult.UserLogId,
                                    userLogResult.BookId);
                            }
                            else if (book != null)
                            {
                                var bookId = await AddBookAsync(activeUserId, book, true);
                                await _challengeRepository.UpdateUserChallengeTaskAsync(
                                    activeUserId,
                                    updateStatus.ChallengeTask.Id,
                                    null,
                                    bookId.Data);
                            }
                        }
                        if (updateStatus.WasComplete)
                        {
                            // person un-completed the task
                            // unwind the points they earned
                            var challengeTaskInfo = await _challengeRepository
                                .GetUserChallengeTaskResultAsync(activeUserId,
                                    updateStatus.ChallengeTask.Id);
                            if (challengeTaskInfo == null)
                            {
                                _logger.LogError("Unable to unwind points for {UserId} on {ChallengeTaskId} - no UserLogId recorded",
                                    activeUserId,
                                    updateStatus.ChallengeTask.Id);
                            }
                            else
                            {
                                _logger.LogDebug("Unwinding points for {UserId} earned in UserLogId {UserLogId}",
                                    activeUserId,
                                    challengeTaskInfo.UserLogId);
                                if (challengeTaskInfo.UserLogId.HasValue)
                                {
                                    await RemoveActivityAsync(activeUserId,
                                        challengeTaskInfo.UserLogId.Value);
                                }

                                // remove the title
                                if (challengeTaskDetails.ChallengeTaskType == ChallengeTaskType.Book
                                    && challengeTaskInfo.BookId != null)
                                {
                                    _logger.LogDebug("Removing from {UserId} book registration {BookId}",
                                        activeUserId,
                                        challengeTaskInfo.BookId);
                                    await RemoveBookAsync(challengeTaskInfo.BookId.Value,
                                        activeUserId);
                                }
                            }
                        }
                    }
                }
            }

            int pointsAwarded = (int)challenge.PointsAwarded;

            // cap points at setting or int.MaxValue
            long totalPoints = Convert.ToInt64(activeUser.PointsEarned)
                + Convert.ToInt64(pointsAwarded);
            var maximumPoints = await GetMaximumAllowedPointsAsync(activeUser.SiteId);

            if (totalPoints > maximumPoints)
            {
                pointsAwarded = maximumPoints - activeUser.PointsEarned;
            }

            int completedTasks = challengeTasks.Count(_ => _.IsCompleted == true);
            if (completedTasks >= challenge.TasksToComplete)
            {
                var userLog = new UserLog
                {
                    IsDeleted = false,
                    PointsEarned = pointsAwarded,
                    UserId = activeUserId,
                    ChallengeId = challengeId
                };
                if (challenge.BadgeId != null)
                {
                    userLog.BadgeId = challenge.BadgeId;
                }
                await _userLogRepository.AddSaveAsync(activeUserId, userLog);

                try
                {
                    await _challengeRepository.IncrementPopularity(challenge.Id);
                }
                catch (GraException gex)
                {
                    var exception = gex.InnerException ?? gex;
                    _logger.LogError(exception,
                        "Unable to increment popularity for challenge id {challengeId}: {ErrorMessage}",
                        challenge.Id,
                        exception.Message);
                }

                // update the score in the user record
                await AddPointsSaveAsync(authUserId,
                    activeUserId,
                    activeUserId,
                    pointsAwarded);

                string badgeNotification = null;
                Badge badge = await AwardBadgeAsync(activeUserId, challenge.BadgeId);
                if (badge != null)
                {
                    badgeNotification = " and a badge";
                }

                var pointsText = pointsAwarded == 1
                    ? "point"
                    : "points";

                // create the notification record
                var notification = new Notification
                {
                    PointsEarned = pointsAwarded,
                    Text = $"<span class=\"far fa-star\"></span> You earned <strong>{pointsAwarded} {pointsText}{badgeNotification}</strong> for completing the challenge: <strong>{challenge.Name}</strong>!",
                    UserId = activeUserId,
                    ChallengeId = challengeId
                };
                if (badge != null)
                {
                    notification.BadgeId = challenge.BadgeId;
                    notification.BadgeFilename = badge.Filename;
                }

                await _notificationRepository.AddSaveAsync(authUserId, notification);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ServiceResult> UpdateFavoriteChallenges(IList<Challenge> challenges)
        {
            var authUserId = GetClaimId(ClaimType.UserId);
            var activeUserId = GetActiveUserId();
            var serviceResult = new ServiceResult();

            var challengeIds = challenges.Select(_ => _.Id);
            var validChallengeIds = await _challengeRepository.ValidateChallengeIdsAsync(
                GetCurrentSiteId(), challengeIds);
            if (challengeIds.Count() != validChallengeIds.Count())
            {
                serviceResult.Status = ServiceResultStatus.Warning;
                serviceResult.Message = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    Annotations.Validate.CouldNotFavorite,
                    Annotations.Title.Challenges);
            }

            var userFavorites = await _challengeRepository.GetUserFavoriteChallenges(activeUserId,
                challengeIds);

            var validChallenges = challenges.Where(_ => validChallengeIds.Contains(_.Id));

            var favoritesToAdd = validChallenges
                .Where(_ => _.IsFavorited)
                .Select(_ => _.Id)
                .Except(userFavorites);

            var favoritesToRemove = validChallenges
                .Where(_ => !_.IsFavorited && userFavorites.Contains(_.Id))
                .Select(_ => _.Id);

            await _challengeRepository.UpdateUserFavoritesAsync(authUserId, activeUserId,
                favoritesToAdd, favoritesToRemove);

            return serviceResult;
        }

        public async Task<ServiceResult> UpdateFavoriteEvents(IList<Event> events)
        {
            var authUserId = GetClaimId(ClaimType.UserId);
            var activeUserId = GetActiveUserId();
            var serviceResult = new ServiceResult();

            var eventIds = events.Select(_ => _.Id);
            var validEventIds = await _eventRepository.ValidateEventIdsAsync(
                GetCurrentSiteId(), eventIds);
            if (eventIds.Count() != validEventIds.Count())
            {
                serviceResult.Status = ServiceResultStatus.Warning;
                serviceResult.Message = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    Annotations.Validate.CouldNotFavorite,
                    Annotations.Interface.Events);
            }

            var userFavorites = await _eventRepository.GetUserFavoriteEvents(activeUserId,
                eventIds);

            var validEvents = events.Where(_ => validEventIds.Contains(_.Id));

            var favoritesToAdd = validEvents
                .Where(_ => _.IsFavorited)
                .Select(_ => _.Id)
                .Except(userFavorites);

            var favoritesToRemove = validEvents
                .Where(_ => !_.IsFavorited && userFavorites.Contains(_.Id))
                .Select(_ => _.Id);

            await _eventRepository.UpdateUserFavoritesAsync(authUserId, activeUserId,
                favoritesToAdd, favoritesToRemove);

            return serviceResult;
        }

        private async Task<User> AddPointsSaveAsync(int authUserId,
            int activeUserId,
            int whoEarnedUserId,
            int pointsEarned,
            bool checkTriggers = true)
        {
            if (pointsEarned < 0)
            {
                throw new GraException("Cannot log negative points!");
            }

            var earnedUser = await _userRepository.GetByIdAsync(whoEarnedUserId)
                ?? throw new GraException($"Could not find a user with id {whoEarnedUserId}");

            // cap points at setting or int.MaxValue
            long totalPoints = Convert.ToInt64(earnedUser.PointsEarned)
                + Convert.ToInt64(pointsEarned);
            var maximumPoints = await GetMaximumAllowedPointsAsync(earnedUser.SiteId);

            if (totalPoints > maximumPoints)
            {
                pointsEarned = maximumPoints - earnedUser.PointsEarned;
            }

            earnedUser.PointsEarned += pointsEarned;
            earnedUser.IsActive = true;
            earnedUser.LastActivityDate = _dateTimeProvider.Now;

            // update the user's achiever status if they've crossed the threshhold
            var program = await _programRepository.GetByIdAsync(earnedUser.ProgramId);

            if (!earnedUser.AchievedAt.HasValue
                && earnedUser.PointsEarned >= program.AchieverPointAmount)
            {
                earnedUser.AchievedAt = _dateTimeProvider.Now;

                var notification = new Notification
                {
                    PointsEarned = 0,
                    Text = $"<span class=\"fas fa-certificate\"></span> Congratulations! You've achieved <strong>{program.AchieverPointAmount} points</strong> reaching the goal of the program!",
                    UserId = earnedUser.Id,
                    IsAchiever = true
                };

                await _notificationRepository.AddSaveAsync(authUserId, notification);
            }

            // save user's changes
            if (activeUserId == earnedUser.Id
                || authUserId == earnedUser.HouseholdHeadUserId)
            {
                earnedUser = await _userRepository.UpdateSaveNoAuditAsync(earnedUser);
            }
            else
            {
                earnedUser = await _userRepository.UpdateSaveAsync(activeUserId, earnedUser);
            }

            if (checkTriggers)
            {
                await AwardTriggersAsync(earnedUser.Id);
            }

            return earnedUser;
        }

        private async Task<Badge> AwardBadgeAsync(int userId, int? badgeId)
        {
            Badge badge = null;
            if (badgeId != null)
            {
                badge = await _badgeRepository.GetByIdAsync((int)badgeId);
                await _badgeRepository.AddUserBadge(userId, (int)badgeId);
            }
            return badge;
        }

        private async Task AwardPrizeAsync(int userId, Trigger trigger, int? mailId,
            bool userIdIsCurrentUser = false)
        {
            if (!string.IsNullOrEmpty(trigger.AwardPrizeName))
            {
                var prize = new PrizeWinner
                {
                    CreatedBy = userIdIsCurrentUser ? userId : GetClaimId(ClaimType.UserId),
                    CreatedAt = _dateTimeProvider.Now,
                    TriggerId = trigger.Id,
                    PrizeName = trigger.AwardPrizeName,
                    PrizeRedemptionInstructions = trigger.AwardPrizeRedemptionInstructions,
                    UserId = userId
                };

                if (mailId != null)
                {
                    prize.MailId = mailId;
                }

                await _prizeWinnerService.AddPrizeWinnerAsync(prize, userIdIsCurrentUser);
            }
        }

        private async Task AwardTriggersAsync(int userId,
            bool logPoints = true,
            int? siteId = null,
            bool userIdIsCurrentUser = false)
        {
            // load the initial list of triggers that might have been achieved
            var triggers = await _triggerRepository.GetTriggersAsync(userId);
            // if three are no triggers in the current query or in the queue then we are done
            if (triggers == null || triggers.Count == 0)
            {
                return;
            }

            // if any triggers came back let's check them
            while (triggers.Count > 0)
            {
                // pull the first trigger off the list and remove it from the list
                var trigger = triggers.First();
                triggers.Remove(trigger);

                // add that we've processed this trigger for this user
                await _triggerRepository.AddTriggerActivationAsync(userId, trigger.Id);
                var user = await _userRepository.GetByIdAsync(userId);

                var pointsAwarded = trigger.AwardPoints;

                // cap points at setting or int.MaxValue
                long totalPoints = Convert.ToInt64(user.PointsEarned)
                    + Convert.ToInt64(pointsAwarded);
                var maximumPoints = await GetMaximumAllowedPointsAsync(user.SiteId);

                if (totalPoints > maximumPoints)
                {
                    pointsAwarded = maximumPoints - user.PointsEarned;
                }

                // if there are points to be awarded, do that now
                if (pointsAwarded > 0 && logPoints)
                {
                    if (userIdIsCurrentUser)
                    {
                        await AddPointsSaveAsync(userId,
                        userId,
                        userId,
                        pointsAwarded,
                        checkTriggers: false);
                    }
                    else
                    {
                        await AddPointsSaveAsync(GetClaimId(ClaimType.UserId),
                            GetActiveUserId(),
                            userId,
                            pointsAwarded,
                            checkTriggers: false);
                    }
                }

                // every trigger awards a badge
                var badge = await AwardBadgeAsync(userId, trigger.AwardBadgeId);
                var attachment = trigger.AwardAttachmentId.HasValue
                    ? await _attachmentRepository.GetByIdAsync(trigger.AwardAttachmentId.Value)
                    : null;

                // log the notification
                await _notificationRepository.AddSaveAsync(userId, new Notification
                {
                    PointsEarned = pointsAwarded,
                    UserId = userId,
                    Text = trigger.AwardMessage,
                    BadgeId = trigger.AwardBadgeId,
                    BadgeFilename = badge.Filename,
                    AttachmentFilename = attachment?.FileName
                });

                // find if the trigger is related to an event
                var relatedEvents = await _eventRepository.GetRelatedEventsForTriggerAsync(
                    trigger.Id);
                var relatedEventId = relatedEvents.FirstOrDefault()?.Id;

                // add the award to the user's history
                await _userLogRepository.AddSaveAsync(userId, new UserLog
                {
                    UserId = userId,
                    PointsEarned = pointsAwarded,
                    IsDeleted = false,
                    BadgeId = trigger.AwardBadgeId,
                    EventId = relatedEventId,
                    TriggerId = trigger.Id,
                    Description = trigger.AwardMessage,
                    AttachmentId = trigger.AwardAttachmentId
                });

                // award any vendor code that is necessary
                var assignedCode = await AwardVendorCodeAsync(userId,
                    trigger.AwardVendorCodeTypeId,
                    siteId);

                if (assignedCode != null)
                {
                    _logger.LogInformation("Trigger {TriggerName} (id {TriggerId}) assigned vendor code {Code} to user {UserId}",
                        trigger.Name,
                        trigger?.Id,
                        assignedCode.Code,
                        userId);
                }

                if (trigger.AwardAvatarBundleId.HasValue)
                {
                    await AwardUserBundle(userId, trigger.AwardAvatarBundleId.Value,
                        userIdIsCurrentUser);
                }

                // send mail if applicable
                int? mailId = await SendMailAsync(userId, trigger, siteId);

                // award prize if applicable
                await AwardPrizeAsync(userId, trigger, mailId, userIdIsCurrentUser);
            }
            // this call will recursively call this method in case any additional
            // triggers are fired by this action
            await AwardTriggersAsync(userId, logPoints, siteId, userIdIsCurrentUser);
        }

        private async Task AwardUserBundle(int userId,
            int bundleId,
            bool userIdIsCurrentUser = false)
        {
            var bundle = await _avatarBundleRepository.GetByIdAsync(bundleId, false);
            if (bundle.AvatarItems.Count > 0)
            {
                var loggingUser = (userIdIsCurrentUser ? userId : GetActiveUserId());
                var userItems = await _avatarItemRepository.GetUserUnlockedItemsAsync(userId);

                var newItems = bundle.AvatarItems.Select(_ => _.Id).Except(userItems).ToList();

                if (newItems.Count > 0)
                {
                    await _avatarItemRepository.AddUserItemsAsync(userId, newItems);
                }
                var notification = new Notification
                {
                    PointsEarned = 0,

                    Text = $"<span class=\"fas fa-shopping-bag\"></span> You've unlocked the <strong>{bundle.Name}</strong> avatar bundle!",
                    UserId = userId,
                    BadgeFilename = bundle.AvatarItems.FirstOrDefault()?.Thumbnail,
                    AvatarBundleId = bundleId
                };

                if (bundle.AvatarItems.Count > 1)
                {
                    notification.Text
                        += " See the full list of unlocked pieces in your Profile History.";
                }

                await _notificationRepository.AddSaveAsync(loggingUser, notification);

                await _userLogRepository.AddSaveAsync(loggingUser, new UserLog
                {
                    UserId = userId,
                    PointsEarned = 0,
                    IsDeleted = false,
                    AvatarBundleId = bundleId,
                    Description = $"You unlocked the <strong>{bundle.Name}</strong> avatar bundle!",
                    HasBeenViewed = false,
                });

                if (!bundle.HasBeenAwarded)
                {
                    bundle.HasBeenAwarded = true;
                    await _avatarBundleRepository.UpdateSaveAsync(loggingUser, bundle);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Any exception here is a critical error and should send an email.")]
        private async Task<VendorCode> AwardVendorCodeAsync(int userId,
            int? vendorCodeTypeId,
            int? siteId = null)
        {
            VendorCode assignedCode = null;

            if (vendorCodeTypeId != null)
            {
                var codeType = await _vendorCodeTypeRepository.GetByIdAsync((int)vendorCodeTypeId);

                var alreadyAssigned = await _vendorCodeRepository.GetUserVendorCode(userId);

                if (alreadyAssigned != null)
                {
                    _logger.LogError("Asked to assign vendor code {CodeTypeDescription} to user {UserId} but user already has a primary code, aborting.",
                        codeType.Description,
                        userId);
                    return null;
                }

                try
                {
                    assignedCode = await _vendorCodeRepository
                        .AssignCodeAsync((int)vendorCodeTypeId, userId);

                    // if there are no award options then send the email with the code
                    if (!codeType.OptionMessageTemplateId.HasValue)
                    {
                        await _vendorCodeService.ResolveCodeStatusAsync(userId, false, false);
                    }
                    else
                    {
                        // award has options, let the user know
                        var user = await _userRepository.GetByIdAsync(userId);

                        int languageId = string.IsNullOrEmpty(user.Culture)
                            ? await _languageService.GetDefaultLanguageIdAsync()
                            : await _languageService.GetLanguageIdAsync(user.Culture);

                        var message = await _messageTemplateService
                            .GetMessageTextAsync(codeType.OptionMessageTemplateId.Value,
                                languageId);

                        var markedUpUrl = await new StubbleBuilder().Build()
                            .RenderAsync(codeType.Url, null);

                        await _mailService.SendSystemMailAsync(new Model.Mail
                        {
                            Body = message.Body,
                            CanParticipantDelete = false,
                            Subject = message.Subject,
                            TemplateDictionary = new Dictionary<string, string>
                            {
                                { TemplateToken.VendorLinkToken, markedUpUrl }
                            },
                            ToUserId = userId
                        }, siteId);
                    }
                }
                catch (Exception ex)
                {
                    await _mailService.SendSystemMailAsync(new Model.Mail
                    {
                        Body = $"There was a difficulty assigning a {codeType.Description}, please contact us.",
                        CanParticipantDelete = true,
                        Subject = "Your award",
                        ToUserId = userId
                    }, siteId);

                    // TODO let admin know that vendor code assignment didn't work?
                    _logger.LogError(ex,
                        "Vendor code assignment failed, probably out of codes: {Message}",
                        ex.Message);
                }
            }

            return assignedCode;
        }

        private async Task<int> GetMaximumAllowedActivityAsync(int siteId)
        {
            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingIntAsync(siteId,
                SiteSettingKey.Users.MaximumActivityPermitted);

            return IsSet ? SetValue : int.MaxValue;
        }

        private async Task<int> GetMaximumAllowedPointsAsync(int siteId)
        {
            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingIntAsync(siteId,
                SiteSettingKey.Points.MaximumPermitted);

            return IsSet ? SetValue : int.MaxValue;
        }

        private async Task<User> RemovePointsSaveAsync(int currentUserId,
            int removePointsFromUserId,
            int pointsToRemove)
        {
            if (pointsToRemove < 0)
            {
                throw new GraException("Cannot remove negative points!");
            }

            var removeUser = await _userRepository.GetByIdAsync(removePointsFromUserId)
                ?? throw new GraException($"Could not find single user with id {removePointsFromUserId}");

            removeUser.PointsEarned -= pointsToRemove;

            // update the user's achiever status if they've crossed the threshhold
            var program = await _programRepository.GetByIdAsync(removeUser.ProgramId);

            if (removeUser.PointsEarned < program.AchieverPointAmount)
            {
                removeUser.AchievedAt = null;
            }

            return await _userRepository.UpdateSaveAsync(currentUserId, removeUser);
        }

        private async Task<int?> SendMailAsync(int userId, Trigger trigger, int? siteId = null)
        {
            if (!string.IsNullOrEmpty(trigger.AwardMailSubject)
                && !string.IsNullOrEmpty(trigger.AwardMail))
            {
                var mail = await _mailService.SendSystemMailAsync(new Model.Mail
                {
                    Body = trigger.AwardMail,
                    Subject = trigger.AwardMailSubject,
                    ToUserId = userId,
                    TriggerId = trigger.Id,
                }, siteId);

                return mail.Id;
            }
            return null;
        }
    }
}
