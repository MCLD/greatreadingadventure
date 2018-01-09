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
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly IDrawingRepository _drawingRepository;
        private readonly IDynamicAvatarBundleRepository _dynamicAvatarBundleRepository;
        private readonly IDynamicAvatarItemRepository _dynamicAvatarItemRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IRequiredQuestionnaireRepository _requiredQuestionnaireRepository;
        private readonly ITriggerRepository _triggerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;
        private readonly IVendorCodeTypeRepository _vendorCodeTypeRepository;
        private readonly ICodeSanitizer _codeSanitizer;
        private readonly MailService _mailService;
        private readonly PrizeWinnerService _prizeWinnerService;

        public ActivityService(ILogger<UserService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContext,
            IBadgeRepository badgeRepository,
            IBookRepository bookRepository,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IDrawingRepository drawingRepository,
            IDynamicAvatarBundleRepository dynamicAvatarBundleRepository,
            IDynamicAvatarItemRepository dynamicAvatarItemRepository,
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
            PrizeWinnerService prizeWinnerService) : base(logger, dateTimeProvider, userContext)
        {
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _bookRepository = Require.IsNotNull(bookRepository, nameof(bookRepository));
            _challengeRepository = Require.IsNotNull(challengeRepository,
                nameof(challengeRepository));
            _challengeTaskRepository = Require.IsNotNull(challengeTaskRepository,
                nameof(challengeTaskRepository));
            _drawingRepository = Require.IsNotNull(drawingRepository, nameof(drawingRepository));
            _dynamicAvatarBundleRepository = Require.IsNotNull(dynamicAvatarBundleRepository,
                nameof(dynamicAvatarBundleRepository));
            _dynamicAvatarItemRepository = Require.IsNotNull(dynamicAvatarItemRepository,
                nameof(dynamicAvatarItemRepository));
            _notificationRepository = Require.IsNotNull(notificationRepository,
                nameof(notificationRepository));
            _pointTranslationRepository = Require.IsNotNull(pointTranslationRepository,
                nameof(pointTranslationRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _requiredQuestionnaireRepository = Require.IsNotNull(requiredQuestionnaireRepository,
                nameof(requiredQuestionnaireRepository));
            _triggerRepository = Require.IsNotNull(triggerRepository, nameof(triggerRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository,
                nameof(userLogRepository));
            _vendorCodeRepository = Require.IsNotNull(vendorCodeRepository,
                nameof(vendorCodeRepository));
            _vendorCodeTypeRepository = Require.IsNotNull(vendorCodeTypeRepository,
                nameof(vendorCodeTypeRepository));
            _codeSanitizer = Require.IsNotNull(codeSanitizer, nameof(codeSanitizer));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            _prizeWinnerService = Require.IsNotNull(prizeWinnerService,
                nameof(prizeWinnerService));
        }

        public async Task<ActivityLogResult> LogActivityAsync(int userIdToLog,
            int activityAmountEarned,
            Book book = null)
        {
            VerifyCanLog();

            if (activityAmountEarned < 0)
            {
                throw new GraException($"Cannot log negative activity amounts!");
            }

            if (book != null)
            {
                if (string.IsNullOrWhiteSpace(book.Title)
                    && !string.IsNullOrWhiteSpace(book.Author))
                {
                    throw new GraException("When providing an author you must also provide a title.");
                }
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
                string error = $"User id {activeUserId} cannot log activity for user id {userIdToLog}";
                _logger.LogError(error);
                throw new GraException("Permission denied.");
            }

            if ((await _requiredQuestionnaireRepository.GetForUser(GetCurrentSiteId(), userToLog.Id,
                userToLog.Age)).Any())
            {
                string error = $"User id {activeUserId} cannot log activity for user id {userIdToLog} who has a pending questionnaire.";
                _logger.LogError(error);
                throw new GraException("Activity cannot be logged while there is a pending questionnaire to be taken.");
            }

            var translation
                = await _pointTranslationRepository.GetByProgramIdAsync(userToLog.ProgramId);

            int pointsEarned
                = (activityAmountEarned / translation.ActivityAmount) * translation.PointsEarned;

            // cap points at int.MaxValue
            long totalPoints = Convert.ToInt64(userToLog.PointsEarned)
                + Convert.ToInt64(pointsEarned);
            if (totalPoints > int.MaxValue)
            {
                pointsEarned = int.MaxValue - userToLog.PointsEarned;
            }

            // add the row to the user's point log
            var userLog = new UserLog
            {
                ActivityEarned = activityAmountEarned,
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
                Text = $"<span class=\"fa fa-star\"></span> You earned <strong>{pointsEarned} points</strong> {activityDescription}!",
                UserId = userToLog.Id
            };

            int? bookId = null;
            // add the book if one was supplied
            if (book != null && !string.IsNullOrWhiteSpace(book.Title))
            {
                bookId = await AddBookAsync(GetActiveUserId(), book);
                notification.Text += $" The book <strong><em>{book.Title}</em></strong> by <strong>{book.Author}</strong> was added to your book list.";
            }

            await _notificationRepository.AddSaveAsync(authUserId, notification);

            return new ActivityLogResult
            {
                UserLogId = userLog.Id,
                BookId = bookId
            };
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
                        if (prize != null && prize.RedeemedAt.HasValue)
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
                string error = $"User id {authUserId} cannot remove activity for user id {userIdToLog}";
                _logger.LogError(error);
                throw new GraException(error);
            }
        }

        public async Task<int> AddBookAsync(int userId, Book book)
        {
            VerifyCanLog();
            int activeUserId = GetActiveUserId();
            var activeUser = await _userRepository.GetByIdAsync(activeUserId);
            int authUserId = GetClaimId(ClaimType.UserId);

            if (userId != activeUserId
                && activeUser.HouseholdHeadUserId != authUserId
                && !HasPermission(Permission.LogActivityForAny))
            {
                _logger.LogError($"User {activeUserId} doesn't have permission to add a book for {userId}.");
                throw new GraException("Permission denied.");
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if ((await _requiredQuestionnaireRepository.GetForUser(GetCurrentSiteId(), user.Id,
                user.Age)).Any())
            {
                string error = $"User id {activeUserId} cannot add a book for user {userId} who has a pending questionnaire.";
                _logger.LogError(error);
                throw new GraException("Books cannot be added while there is a pending questionnaire to be taken.");
            }


            return await _bookRepository.AddSaveForUserAsync(activeUserId, userId, book);
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
                    Book newBook = new Book()
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
                _logger.LogError($"User {authUserId} doesn't have permission to edit book {book.Id} for user {forUserId}.");
                throw new GraException("Permission denied.");
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
                _logger.LogError($"User {authUserId} doesn't have permission to remove book {bookId} for user {forUserId}.");
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
                activeUser.Age)).Any())
            {
                string error = $"User id {activeUserId} cannot complete challenges tasks while having a pending questionnaire.";
                _logger.LogError(error);
                throw new GraException("Challenge tasks cannot be completed while there is a pending questionnaire to be taken.");
            }

            var challenge = await _challengeRepository.GetActiveByIdAsync(challengeId, activeUserId);

            if (challenge.IsCompleted == true)
            {
                _logger.LogError($"User {authUserId} cannot make changes to a completed challenge {challengeId}.");
                throw new GraException("Challenge is already completed.");
            }

            var updateStatuses = await _challengeRepository.UpdateUserChallengeTasksAsync(activeUserId,
                challengeTasks);

            // re-fetch challenge with tasks completed
            challenge = await _challengeRepository.GetActiveByIdAsync(challengeId, activeUserId);

            // loop tasks to see if we need to perform any additional point translation/book tasks
            foreach (var updateStatus in updateStatuses)
            {
                var challengeTaskDetails = challenge.Tasks.Where(_ => _.Id == updateStatus.ChallengeTask.Id).SingleOrDefault();
                // is there work we need to do on this item
                if ((challengeTaskDetails.ActivityCount != null
                    && challengeTaskDetails.PointTranslationId != null)
                    || challengeTaskDetails.ChallengeTaskType == ChallengeTaskType.Book)
                {
                    // did something change?
                    _logger.LogDebug($"Challenge task {updateStatus.ChallengeTask.Id} counts as an activity");
                    if (updateStatus.WasComplete != updateStatus.IsComplete)
                    {
                        _logger.LogDebug($"Status of {updateStatus.ChallengeTask.Id}: was {updateStatus.WasComplete}, is {updateStatus.IsComplete}");
                        if (updateStatus.IsComplete)
                        {
                            // person completed the task
                            Book book = null;
                            if (challengeTaskDetails.ChallengeTaskType == ChallengeTaskType.Book)
                            {
                                _logger.LogDebug($"Challenge task {updateStatus.ChallengeTask.Id} is a book");
                                book = new Book
                                {
                                    Title = updateStatus.ChallengeTask.Title,
                                    Author = updateStatus.ChallengeTask.Author,
                                    ChallengeId = challenge.Id
                                };
                            }

                            if (challengeTaskDetails.ActivityCount != null
                                && challengeTaskDetails.PointTranslationId != null)
                            {
                                _logger.LogDebug($"Logging activity for {activeUserId} based on challenge task {updateStatus.ChallengeTask.Id}");
                                var userLogResult = await LogActivityAsync(activeUserId,
                                    (int)challengeTaskDetails.ActivityCount,
                                    book);

                                // update record with user log result
                                _logger.LogDebug($"Update success, recording UserLogId {userLogResult.UserLogId.Value} and BookId {userLogResult.BookId}");
                                await _challengeRepository.UpdateUserChallengeTaskAsync(activeUserId,
                                    updateStatus.ChallengeTask.Id,
                                    userLogResult.UserLogId.Value,
                                    userLogResult.BookId);
                            }
                            else if (book != null)
                            {
                                var bookId = await AddBookAsync(activeUserId, book);
                                await _challengeRepository.UpdateUserChallengeTaskAsync(activeUserId,
                                    updateStatus.ChallengeTask.Id,
                                    null,
                                    bookId);
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
                                _logger.LogError($"Unable to unwind points for {activeUserId} on {updateStatus.ChallengeTask.Id} - no UserLogId recorded");
                            }
                            else
                            {
                                _logger.LogDebug($"Unwinding points for {activeUserId} earned in UserLogId {challengeTaskInfo.UserLogId}");
                                if (challengeTaskInfo.UserLogId.HasValue)
                                {
                                    await RemoveActivityAsync(activeUserId, challengeTaskInfo.UserLogId.Value);
                                }

                                // remove the title
                                if (challengeTaskDetails.ChallengeTaskType == ChallengeTaskType.Book
                                    && challengeTaskInfo.BookId != null)
                                {
                                    _logger.LogDebug($"Removing for {activeUserId} book registration {challengeTaskInfo.BookId}");
                                    await RemoveBookAsync(challengeTaskInfo.BookId.Value, activeUserId);
                                }
                            }
                        }
                    }
                }
            }

            int pointsAwarded = (int)challenge.PointsAwarded;

            // cap points at int.MaxValue
            long totalPoints = Convert.ToInt64(activeUser.PointsEarned)
                + Convert.ToInt64(pointsAwarded);
            if (totalPoints > int.MaxValue)
            {
                pointsAwarded = int.MaxValue - activeUser.PointsEarned;
            }

            int completedTasks = challengeTasks.Where(_ => _.IsCompleted == true).Count();
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

                // update the score in the user record
                var postUpdateUser = await AddPointsSaveAsync(authUserId,
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
                    Text = $"<span class=\"fa fa-star\"></span> You earned <strong>{pointsAwarded} points{badgeNotification}</strong> for completing the challenge: <strong>{challenge.Name}</strong>!",
                    UserId = activeUserId,
                    ChallengeId = challengeId
                };
                if (badge != null)
                {
                    notification.BadgeId = challenge.BadgeId;
                    notification.BadgeFilename = badge.Filename;
                };

                await _notificationRepository.AddSaveAsync(authUserId, notification);

                return true;
            }
            else
            {
                return false;
            }
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
                if (householdMemebers.Count() > 0)
                {
                    foreach (var member in householdMemebers)
                    {
                        await AwardTriggersAsync(member.Id, logPoints, userContext.SiteId,
                            !userContext.User.Identity.IsAuthenticated);
                    }
                }
            }
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
                throw new Exception($"Could not find a user with id {whoEarnedUserId}");
            }

            // cap points at int.MaxValue
            long totalPoints = Convert.ToInt64(earnedUser.PointsEarned)
                + Convert.ToInt64(pointsEarned);
            if (totalPoints > int.MaxValue)
            {
                pointsEarned = int.MaxValue - earnedUser.PointsEarned;
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
                    Text = $"<span class=\"fa fa-certificate\"></span> Congratulations! You've achieved <strong>{program.AchieverPointAmount} points</strong> reaching the goal of the program!",
                    UserId = earnedUser.Id,
                    IsAchiever = true
                };

                var badge = await AwardBadgeAsync(activeUserId, program.AchieverBadgeId);

                if (badge != null)
                {
                    await _userLogRepository.AddAsync(activeUserId, new UserLog
                    {
                        UserId = whoEarnedUserId,
                        PointsEarned = 0,
                        IsDeleted = false,
                        BadgeId = badge.Id,
                        Description = $"You reached the goal of {program.AchieverPointAmount} points!"
                    });
                    notification.Text += " You've also earned a badge!";
                    notification.BadgeId = badge.Id;
                    notification.BadgeFilename = badge.Filename;
                }

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

        public async Task<PointTranslation> GetUserPointTranslationAsync()
        {
            var user = await _userRepository.GetByIdAsync(GetActiveUserId());
            return await _pointTranslationRepository.GetByIdAsync(user.ProgramId);
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
                throw new Exception($"Could not find single user with id {removePointsFromUserId}");
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

        private async Task AwardTriggersAsync(int userId, bool logPoints = true, int? siteId = null,
            bool userIdIsCurrentUser = false)
        {
            // load the initial list of triggers that might have been achieved
            var triggers = await _triggerRepository.GetTriggersAsync(userId);
            // if three are no triggers in the current query or in the queue then we are done
            if (triggers == null || triggers.Count() == 0)
            {
                return;
            }

            // if any triggers came back let's check them
            while (triggers.Count() > 0)
            {
                // pull the first trigger off the list and remove it from the list
                var trigger = triggers.First();
                triggers.Remove(trigger);

                // add that we've processed this trigger for this user
                await _triggerRepository.AddTriggerActivationAsync(userId, trigger.Id);
                var user = await _userRepository.GetByIdAsync(userId);

                var pointsAwarded = trigger.AwardPoints;

                // cap points at int.MaxValue
                long totalPoints = Convert.ToInt64(user.PointsEarned)
                    + Convert.ToInt64(pointsAwarded);
                if (totalPoints > int.MaxValue)
                {
                    pointsAwarded = int.MaxValue - user.PointsEarned;
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

                // add the award to the user's history
                await _userLogRepository.AddSaveAsync(userId, new UserLog
                {
                    UserId = userId,
                    PointsEarned = pointsAwarded,
                    IsDeleted = false,
                    BadgeId = trigger.AwardBadgeId,
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

        private async Task AwardVendorCodeAsync(int userId, int? vendorCodeTypeId, int? siteId = null)
        {
            if (vendorCodeTypeId != null)
            {
                var codeType = await _vendorCodeTypeRepository.GetByIdAsync((int)vendorCodeTypeId);
                try
                {
                    var assignedCode = await _vendorCodeRepository.AssignCodeAsync((int)vendorCodeTypeId, userId);
                    await _mailService.SendSystemMailAsync(new Mail
                    {
                        ToUserId = userId,
                        CanParticipantDelete = false,
                        Subject = codeType.MailSubject,
                        Body = codeType.Mail.Contains("{Code}")
                            ? codeType.Mail.Replace("{Code}", assignedCode.Code)
                            : codeType.Mail + " " + assignedCode.Code
                    }, siteId);
                }
                catch (Exception)
                {
                    await _mailService.SendSystemMailAsync(new Mail
                    {
                        ToUserId = userId,
                        CanParticipantDelete = true,
                        Subject = codeType.MailSubject,
                        Body = codeType.Mail.Contains("{Code}")
                            ? codeType.Mail.Replace("{Code}", $"{codeType.Description} not available - please contact us.")
                            : codeType.Mail + " " + $"{codeType.Description} not available - please contact us."
                    }, siteId);

                    // TODO let admin know that vendor code assignment didn't work?
                }
            }
        }

        public async Task<bool> LogSecretCodeAsync(int userIdToLog, string secretCode,
            bool householdLogging = false)
        {
            VerifyCanLog();

            if (string.IsNullOrWhiteSpace(secretCode))
            {
                throw new GraException("You must enter a code!");
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
                string error = $"User id {activeUserId} cannot log a code for user id {userIdToLog}";
                _logger.LogError(error);
                throw new GraException("You do not have permission to apply that code.");
            }

            if ((await _requiredQuestionnaireRepository.GetForUser(GetCurrentSiteId(), userToLog.Id,
                userToLog.Age)).Any())
            {
                string error = $"User id {activeUserId} cannot log secret code for user {userToLog.Id} who has a pending questionnaire.";
                _logger.LogError(error);
                throw new GraException("Secret codes cannot be entered while there is a pending questionnaire to be taken.");
            }

            var trigger = await _triggerRepository.GetByCodeAsync(GetCurrentSiteId(), secretCode);

            if (trigger == null)
            {
                throw new GraException($"<strong>{secretCode}</strong> is not a valid code.");
            }

            var pointsAwarded = trigger.AwardPoints;

            // cap points at int.MaxValue
            long totalPoints = Convert.ToInt64(userToLog.PointsEarned)
                + Convert.ToInt64(pointsAwarded);
            if (totalPoints > int.MaxValue)
            {
                pointsAwarded = int.MaxValue - userToLog.PointsEarned;
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

            // add the award to the user's history
            var userLog = new UserLog
            {
                UserId = userIdToLog,
                PointsEarned = pointsAwarded,
                IsDeleted = false,
                BadgeId = trigger.AwardBadgeId,
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
            int? mailId = await SendMailAsync(activeUserId, trigger);

            // award prize if applicable
            await AwardPrizeAsync(activeUserId, trigger, mailId);

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

        public async Task LogHouseholdActivityAsync(List<int> userIds, int activityAmount)
        {
            VerifyCanLog();

            if (activityAmount < 1)
            {
                throw new GraException($"Amount must be at least 1.");
            }
            int authUserId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.LogActivityForAny))
            {
                var authUser = await _userRepository.GetByIdAsync(authUserId);
                if (authUser.HouseholdHeadUserId.HasValue)
                {
                    string error = $"User id {authUserId} cannot log activity for a household";
                    _logger.LogError(error);
                    throw new GraException("Permission denied.");
                }

                var householdList = (await _userRepository.GetHouseholdAsync(authUserId))
                .Select(_ => _.Id).ToList();
                householdList.Add(authUserId);
                if (userIds.Except(householdList).Any())
                {
                    string error = $"User id {authUserId} cannot log activity for {userIds.Except(householdList).First()}";
                    _logger.LogError(error);
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
                throw new GraException("You must enter a code!");
            }

            secretCode = _codeSanitizer.Sanitize(secretCode);

            int authUserId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.LogActivityForAny))
            {
                var authUser = await _userRepository.GetByIdAsync(authUserId);
                if (authUser.HouseholdHeadUserId.HasValue)
                {
                    string error = $"User id {authUserId} cannot log codes for a household";
                    _logger.LogError(error);
                    throw new GraException("Permission denied.");
                }

                var householdList = (await _userRepository.GetHouseholdAsync(authUserId))
                    .Select(_ => _.Id).ToList();
                householdList.Add(authUserId);
                if (userIds.Except(householdList).Any())
                {
                    string error = $"User id {authUserId} cannot log codes for {userIds.Except(householdList).First()}";
                    _logger.LogError(error);
                    throw new GraException("Permission denied.");
                }
            }

            var trigger = await _triggerRepository.GetByCodeAsync(GetCurrentSiteId(), secretCode);

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

        public async Task MCAwardVendorCodeAsync(int userId, int vendorCodeTypeId)
        {
            var authUserId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.ManageVendorCodes))
            {
                _logger.LogError($"User {authUserId} cannot award vendor codes.");
                throw new GraException("Permission denied.");
            }

            await AwardVendorCodeAsync(userId, vendorCodeTypeId);
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
                serviceResult.Message = "One or more of the selected challenges could not favorited.";
            }

            var userFavorites = await _challengeRepository.GetUserFavoriteChallenges(activeUserId,
                challengeIds);

            var validChallenges = challenges.Where(_ => validChallengeIds.Contains(_.Id));

            var favoritesToAdd = validChallenges
                .Where(_ => _.IsFavorited == true)
                .Select(_ => _.Id)
                .Except(userFavorites);

            var favoritesToRemove = validChallenges
                .Where(_ => _.IsFavorited == false && userFavorites.Contains(_.Id))
                .Select(_ => _.Id);

            await _challengeRepository.UpdateUserFavoritesAsync(authUserId, activeUserId, 
                favoritesToAdd, favoritesToRemove);

            return serviceResult;
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

        private async Task AwardPrizeAsync(int userId, Trigger trigger, int? mailId,
            bool userIdIsCurrentUser = false)
        {
            if (!string.IsNullOrEmpty(trigger.AwardPrizeName))
            {
                var prize = new PrizeWinner
                {
                    CreatedBy = userId,
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

        private async Task AwardUserBundle(int userId, int bundleId,
            bool userIdIsCurrentUser = false)
        {
            var bundle = await _dynamicAvatarBundleRepository.GetByIdAsync(bundleId, false);
            if (bundle.DynamicAvatarItems.Count > 0)
            {
                var loggingUser = (userIdIsCurrentUser ? userId : GetActiveUserId());
                var userItems = await _dynamicAvatarItemRepository.GetUserUnlockedItemsAsync(userId);

                var newItems = bundle.DynamicAvatarItems.Select(_ => _.Id).Except(userItems).ToList();

                if (newItems.Count > 0)
                {
                    await _dynamicAvatarItemRepository.AddUserItemsAsync(userId, newItems);
                }

                var notification = new Notification
                {
                    PointsEarned = 0,
                    Text = $"<span class=\"fa fa-shopping-bag\"></span> You've unlocked the <strong>{bundle.Name}</strong> avatar bundle!",
                    UserId = userId,
                    BadgeFilename = bundle.DynamicAvatarItems.FirstOrDefault().Thumbnail
                };

                if (bundle.DynamicAvatarItems.Count > 1)
                {
                    notification.Text += " You can view the full list of pieces unlocked in your Profile History.";
                }

                await _notificationRepository.AddSaveAsync(loggingUser, notification);

                await _userLogRepository.AddSaveAsync(loggingUser, new UserLog
                {
                    UserId = userId,
                    PointsEarned = 0,
                    IsDeleted = false,
                    AvatarBundleId = bundleId,
                    Description = $"You unlocked the <strong>{bundle.Name}</strong> avatar bundle!"
                });

                if (!bundle.HasBeenAwarded)
                {
                    bundle.HasBeenAwarded = true;
                    await _dynamicAvatarBundleRepository.UpdateSaveAsync(loggingUser, bundle);
                }
            }
        }
    }
}