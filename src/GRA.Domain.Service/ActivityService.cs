using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Service.Models;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class ActivityService : Abstract.BaseUserService<UserService>
    {
        private readonly IAvatarBundleRepository _avatarBundleRepository;
        private readonly IAvatarItemRepository _avatarItemRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly ICodeSanitizer _codeSanitizer;
        private readonly IEventRepository _eventRepository;
        private readonly MailService _mailService;
        private readonly INotificationRepository _notificationRepository;
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

        public ActivityService(ILogger<UserService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContext,
            IAvatarBundleRepository avatarBundleRepository,
            IAvatarItemRepository avatarItemRepository,
            IBadgeRepository badgeRepository,
            IBookRepository bookRepository,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IEventRepository eventRepository,
            INotificationRepository notificationRepository,
            IPointTranslationRepository pointTranslationRepository,
            IProgramRepository programRepository,
            IRequiredQuestionnaireRepository requiredQuestionnaireRepository,
            ITriggerRepository triggerRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository,
            IVendorCodeRepository vendorCodeRepository,
            IVendorCodeTypeRepository vendorCodeTypeRepository,
            ICodeSanitizer codeSanitizer,
            MailService mailService,
            PrizeWinnerService prizeWinnerService,
            SiteLookupService siteLookupService,
            VendorCodeService vendorCodeService) : base(logger, dateTimeProvider, userContext)
        {
            _avatarBundleRepository = avatarBundleRepository
                ?? throw new ArgumentNullException(nameof(avatarBundleRepository));
            _avatarItemRepository = avatarItemRepository
                ?? throw new ArgumentNullException(nameof(avatarItemRepository));
            _badgeRepository = badgeRepository
                ?? throw new ArgumentNullException(nameof(badgeRepository));
            _bookRepository = bookRepository
                ?? throw new ArgumentNullException(nameof(bookRepository));
            _challengeRepository = challengeRepository
                ?? throw new ArgumentNullException(nameof(challengeRepository));
            _challengeTaskRepository = challengeTaskRepository
                ?? throw new ArgumentNullException(nameof(challengeTaskRepository));
            _eventRepository = eventRepository
                ?? throw new ArgumentNullException(nameof(eventRepository));
            _notificationRepository = notificationRepository
                ?? throw new ArgumentNullException(nameof(notificationRepository));
            _pointTranslationRepository = pointTranslationRepository
                ?? throw new ArgumentNullException(nameof(pointTranslationRepository));
            _programRepository = programRepository
                ?? throw new ArgumentNullException(nameof(programRepository));
            _requiredQuestionnaireRepository = requiredQuestionnaireRepository
                ?? throw new ArgumentNullException(nameof(requiredQuestionnaireRepository));
            _triggerRepository = triggerRepository
                ?? throw new ArgumentNullException(nameof(triggerRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _userLogRepository = userLogRepository
                ?? throw new ArgumentNullException(nameof(userLogRepository));
            _vendorCodeRepository = vendorCodeRepository
                ?? throw new ArgumentNullException(nameof(vendorCodeRepository));
            _vendorCodeTypeRepository = vendorCodeTypeRepository
                ?? throw new ArgumentNullException(nameof(vendorCodeTypeRepository));
            _codeSanitizer = codeSanitizer
                ?? throw new ArgumentNullException(nameof(codeSanitizer));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _prizeWinnerService = prizeWinnerService
                ?? throw new ArgumentNullException(nameof(prizeWinnerService));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
        }

        public async Task<ServiceResult<int>> AddBookAsync(int userId, Book book, bool addNotification = false)
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

            book.Title = book.Title?.Trim();
            book.Author = book.Author?.Trim();

            var addedBook = await _bookRepository.GetBookAsync(book) ?? book;
            if (await _bookRepository.UserHasBookAsync(userId, addedBook.Id))
            {
                serviceResult.Status = ServiceResultStatus.Warning;
                serviceResult.Message = $"The book <strong><em>{addedBook.Title}</em></strong> by <strong>{addedBook.Author}</strong> is already on the booklist.";
            }
            else
            {
                await _bookRepository.AddSaveForUserAsync(activeUserId, userId, addedBook);

                if (addNotification)
                {
                    var notification = new Notification
                    {
                        UserId = userId,
                        Text = $"The book <strong><em>{book.Title}</em></strong> by <strong>{book.Author}</strong> was added to your book list."
                    };

                    await _notificationRepository.AddSaveAsync(authUserId, notification);
                    serviceResult.Status = ServiceResultStatus.Success;
                    serviceResult.Data = addedBook.Id;
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

        public async Task<int> GetActivityEarnedAsync()
        {
            return await _userLogRepository.GetActivityEarnedForUserAsync(GetActiveUserId());
        }

        public async Task<PointTranslation> GetUserPointTranslationAsync()
        {
            var user = await _userRepository.GetByIdAsync(GetActiveUserId());
            return await _pointTranslationRepository.GetByProgramIdAsync(user.ProgramId);
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
                userLog = await _userLogRepository.AddSaveAsync(activeUserId, userLog);

                activityLogResult.UserLogId = userLog.Id;

                // update the score in the user record
                userToLog = await AddPointsSaveAsync(authUserId,
                    activeUserId,
                    userToLog.Id,
                    pointsEarned);

                // prepare the notification text
                string activityDescription = "for <strong>";
                if (translation.TranslationDescriptionPresentTense.Contains("{0}"))
                {
                    activityDescription += string.Format(
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

                // create the notification record
                var notification = new Notification
                {
                    PointsEarned = pointsEarned,
                    Text = $"<span class=\"far fa-star\"></span> You earned <strong>{pointsEarned} points</strong> {activityDescription}!",
                    UserId = userToLog.Id
                };

                // add the book if one was supplied
                if (book != null && !string.IsNullOrWhiteSpace(book.Title))
                {
                    var bookData = await AddBookAsync(GetActiveUserId(), book);
                    activityLogResult.BookId = bookData.Data;
                    notification.Text += $" The book <strong><em>{book.Title}</em></strong> by <strong>{book.Author}</strong> was added to your book list.";
                }

                await _notificationRepository.AddSaveAsync(authUserId, notification);

                // add userlog and notification if the max activity amount has been reached
                if (activityAmountToAdd < activityAmountEarned || totalActivity == maximumActivity)
                {
                    string descriptionPastTense = translation.TranslationDescriptionPastTense
                        .Replace("{0}", "").Trim();

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
            else if (book != null && !string.IsNullOrWhiteSpace(book.Title))
            {
                activityLogResult.BookId = (await AddBookAsync(GetActiveUserId(), book, true)).Data;
            }

            return activityLogResult;
        }

        public async Task LogHouseholdActivityAsync(List<int> userIds, int activityAmount)
        {
            VerifyCanLog();

            if (activityAmount < 1)
            {
                throw new GraException("Amount must be at least 1.");
            }
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

        public async Task<bool> LogHouseholdSecretCodeAsync(List<int> userIds, string secretCode)
        {
            VerifyCanLog();

            if (string.IsNullOrWhiteSpace(secretCode))
            {
                throw new GraException(Annotations.Required.SecretCode);
            }

            secretCode = _codeSanitizer.Sanitize(secretCode);

            int authUserId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.LogActivityForAny))
            {
                var authUser = await _userRepository.GetByIdAsync(authUserId);
                if (authUser.HouseholdHeadUserId.HasValue)
                {
                    _logger.LogError("User id {UserId} cannot log codes for a family/group", authUserId);
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

            var trigger = await _triggerRepository.GetByCodeAsync(GetCurrentSiteId(), secretCode,
                true);

            if (trigger == null)
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

        public async Task<bool> LogSecretCodeAsync(int userIdToLog, string secretCode,
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
                true);

            if (trigger == null)
            {
                throw new GraException($"<strong>{secretCode}</strong> is not a valid code.");
            }

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

            // log the notification
            await _notificationRepository.AddSaveAsync(authUserId, new Notification
            {
                PointsEarned = pointsAwarded,
                UserId = userIdToLog,
                Text = trigger.AwardMessage,
                BadgeId = trigger.AwardBadgeId,
                BadgeFilename = badge.Filename
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
                Description = trigger.AwardMessage
            };

            if (activeUserId != userToLog.Id)
            {
                userLog.AwardedBy = activeUserId;
            }

            await _userLogRepository.AddSaveAsync(authUserId, userLog);

            // award any vendor code that is necessary
            await AwardVendorCodeAsync(userIdToLog, trigger.AwardVendorCodeTypeId);

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

            await AwardVendorCodeAsync(userId, vendorCodeTypeId);
        }

        public async Task<User> RemoveActivityAsync(int userIdToLog,
                                            int userLogIdToRemove)
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
                _logger.LogError("User id {UserId} cannot remove activity for user id {UserIdToLog}",
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

            if ((await _requiredQuestionnaireRepository.GetForUser(GetCurrentSiteId(), activeUser.Id,
                activeUser.Age)).Count > 0)
            {
                _logger.LogError("User id {UserId} cannot complete challenges tasks while having a pending questionnaire.",
                    activeUserId);
                throw new GraException("Challenge tasks cannot be completed while there is a pending questionnaire to be taken.");
            }

            var challenge = await _challengeRepository.GetActiveByIdAsync(challengeId, activeUserId);

            if (challenge.IsCompleted == true)
            {
                _logger.LogError("User {UserId} cannot make changes to completed challenge id {ChallengeId}.",
                    authUserId,
                    challengeId);
                throw new GraException("Challenge is already completed.");
            }

            var updateStatuses = await _challengeRepository.UpdateUserChallengeTasksAsync(activeUserId,
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
                                await _challengeRepository.UpdateUserChallengeTaskAsync(activeUserId,
                                    updateStatus.ChallengeTask.Id,
                                    userLogResult.UserLogId,
                                    userLogResult.BookId);
                            }
                            else if (book != null)
                            {
                                var bookId = await AddBookAsync(activeUserId, book, true);
                                await _challengeRepository.UpdateUserChallengeTaskAsync(activeUserId,
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
                                    await RemoveActivityAsync(activeUserId, challengeTaskInfo.UserLogId.Value);
                                }

                                // remove the title
                                if (challengeTaskDetails.ChallengeTaskType == ChallengeTaskType.Book
                                    && challengeTaskInfo.BookId != null)
                                {
                                    _logger.LogDebug("Removing from {UserId} book registration {BookId}",
                                        activeUserId,
                                        challengeTaskInfo.BookId);
                                    await RemoveBookAsync(challengeTaskInfo.BookId.Value, activeUserId);
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
                    badgeNotification = $" and a badge";
                }

                // create the notification record
                var notification = new Notification
                {
                    PointsEarned = pointsAwarded,
                    Text = $"<span class=\"far fa-star\"></span> You earned <strong>{pointsAwarded} points{badgeNotification}</strong> for completing the challenge: <strong>{challenge.Name}</strong>!",
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
                    Annotations.Validate.CouldNotFavorite, Annotations.Title.Challenges);
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
                    Annotations.Validate.CouldNotFavorite, Annotations.Interface.Events);
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
                throw new GraException($"Cannot log negative points!");
            }

            var earnedUser = await _userRepository.GetByIdAsync(whoEarnedUserId);
            if (earnedUser == null)
            {
                throw new GraException($"Could not find a user with id {whoEarnedUserId}");
            }

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

        private async Task AwardTriggersAsync(int userId, bool logPoints = true, int? siteId = null,
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

                // log the notification
                await _notificationRepository.AddSaveAsync(userId, new Notification
                {
                    PointsEarned = pointsAwarded,
                    UserId = userId,
                    Text = trigger.AwardMessage,
                    BadgeId = trigger.AwardBadgeId,
                    BadgeFilename = badge.Filename
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
                    Description = trigger.AwardMessage
                });

                // award any vendor code that is necessary
                await AwardVendorCodeAsync(userId, trigger.AwardVendorCodeTypeId, siteId);

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

        private async Task AwardUserBundle(int userId, int bundleId,
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
                    notification.Text += " See the full list of unlocked pieces in your Profile History.";
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

        private async Task AwardVendorCodeAsync(int userId, int? vendorCodeTypeId, int? siteId = null)
        {
            if (vendorCodeTypeId != null)
            {
                var codeType = await _vendorCodeTypeRepository.GetByIdAsync((int)vendorCodeTypeId);

                try
                {
                    await _vendorCodeRepository.AssignCodeAsync((int)vendorCodeTypeId, userId);

                    // if there are no award options then send the email with the code
                    if (string.IsNullOrEmpty(codeType.OptionSubject))
                    {
                        await _vendorCodeService.ResolveCodeStatusAsync(userId, false, false);
                    }
                    else
                    {
                        // award has options, let the user know
                        await _mailService.SendSystemMailAsync(new Mail
                        {
                            ToUserId = userId,
                            CanParticipantDelete = false,
                            Subject = codeType.OptionSubject,
                            Body = codeType.OptionMail
                        }, siteId);
                    }
                }
                catch (Exception ex)
                {
                    await _mailService.SendSystemMailAsync(new Mail
                    {
                        ToUserId = userId,
                        CanParticipantDelete = true,
                        Subject = codeType.MailSubject,
                        Body = codeType.Mail.Contains(TemplateToken.VendorCodeToken)
                            ? codeType.Mail.Replace(TemplateToken.VendorCodeToken, $"{codeType.Description} not available - please contact us.")
                            : codeType.Mail + " " + $"{codeType.Description} not available - please contact us."
                    }, siteId);

                    // TODO let admin know that vendor code assignment didn't work?
                    _logger.LogError(ex,
                        "Vendor code assignment failed, probably out of codes: {Message}",
                        ex.Message);
                }
            }
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

        private async Task<User>
                                                                    RemovePointsSaveAsync(int currentUserId,
            int removePointsFromUserId,
            int pointsToRemove)
        {
            if (pointsToRemove < 0)
            {
                throw new GraException($"Cannot remove negative points!");
            }

            var removeUser = await _userRepository.GetByIdAsync(removePointsFromUserId);

            if (removeUser == null)
            {
                throw new GraException($"Could not find single user with id {removePointsFromUserId}");
            }

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
                var mail = await _mailService.SendSystemMailAsync(new Mail
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
