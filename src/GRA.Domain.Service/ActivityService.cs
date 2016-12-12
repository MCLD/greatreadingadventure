using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;
using System.Collections.Generic;

namespace GRA.Domain.Service
{
    public class ActivityService : Abstract.BaseUserService<UserService>
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;

        public ActivityService(ILogger<UserService> logger,
            IUserContextProvider userContext,
            IBadgeRepository badgeRepository,
            IBookRepository bookRepository,
            IChallengeRepository challengeRepository,
            INotificationRepository notificationRepository,
            IPointTranslationRepository pointTranslationRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger, userContext)
        {
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _bookRepository = Require.IsNotNull(bookRepository, nameof(bookRepository));
            _challengeRepository = Require.IsNotNull(challengeRepository,
                nameof(challengeRepository));
            _notificationRepository = Require.IsNotNull(notificationRepository,
                nameof(notificationRepository));
            _pointTranslationRepository = Require.IsNotNull(pointTranslationRepository,
                nameof(pointTranslationRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository,
                nameof(userLogRepository));
        }

        public async Task LogActivityAsync(int userIdToLog,
            int activityAmountEarned,
            Book book = null)
        {
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
                && authUserId != userToLog.HouseholdHeadUserId
                && !loggingAsAdminUser)
            {
                string error = $"User id {activeUserId} cannot log activity for user id {userIdToLog}";
                _logger.LogError(error);
                throw new GraException(error);
            }

            var translation
                = await _pointTranslationRepository.GetByProgramIdAsync(userToLog.ProgramId);

            int pointsEarned
                = (activityAmountEarned / translation.ActivityAmount) * translation.PointsEarned;

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
            await _userLogRepository.AddSaveAsync(activeUserId, userLog);

            // update the score in the user record
            var postUpdateUser = await _userRepository.AddPointsSaveAsync(activeUserId,
                userToLog.Id,
                pointsEarned,
                loggingAsAdminUser);

            // create the notification record
            var notification = new Notification
            {
                PointsEarned = pointsEarned,
                Text = $"<span class=\"fa fa-star\"></span> You earned <strong>{pointsEarned} points</strong> and currently have <strong>{postUpdateUser.PointsEarned} points</strong>!",
                UserId = userToLog.Id
            };

            // add the book if one was supplied
            if (book != null && !string.IsNullOrWhiteSpace(book.Title))
            {
                await AddBook(GetActiveUserId(), book);
                notification.Text += $" The book <strong><em>{book.Title}</em> by {book.Author}</strong> was added to your book list.";
            }

            await _notificationRepository.AddSaveAsync(authUserId, notification);
        }

        public async Task<User> RemoveActivityAsync(int userIdToLog,
            int userLogIdToRemove)
        {
            int currentUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.LogActivityForAny))
            {
                var userLog = await _userLogRepository.GetByIdAsync(userLogIdToRemove);

                int pointsToRemove = userLog.PointsEarned;
                await _userLogRepository.RemoveSaveAsync(currentUserId, userLogIdToRemove);
                return await _userRepository
                    .RemovePointsSaveASync(currentUserId, userIdToLog, pointsToRemove);
            }
            else
            {
                string error = $"User id {currentUserId} cannot remove activity for user id {userIdToLog}";
                _logger.LogError(error);
                throw new GraException(error);
            }
        }

        public async Task AddBook(int userId, Book book)
        {
            int activeUserId = GetActiveUserId();
            var activeUser = await _userRepository.GetByIdAsync(activeUserId);
            int authUserId = GetClaimId(ClaimType.UserId);


            if (userId == activeUserId
                || activeUser.HouseholdHeadUserId == authUserId
                || HasPermission(Permission.LogActivityForAny))
            {
                await _bookRepository.AddSaveForUserAsync(activeUserId, userId, book);
            }
            else
            {
                _logger.LogError($"User {activeUserId} doesn't have permission to add a book for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task RemoveBook(int userId, int bookId)
        {
            int requestedByUserId = GetClaimId(ClaimType.UserId);
            if (requestedByUserId == userId
                || HasPermission(Permission.LogActivityForAny))
            {
                await _bookRepository.RemoveForUserAsync(requestedByUserId, userId, bookId);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to remove a book for {userId}.");
                throw new GraException("Permission denied.");
            }

        }

        public async Task UpdateBook(int userId, Book book)
        {
            int requestedByUserId = GetClaimId(ClaimType.UserId);
            if (requestedByUserId == userId
                || HasPermission(Permission.LogActivityForAny))
            {
                await _bookRepository.UpdateSaveAsync(requestedByUserId, book);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to edit a book for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<bool> UpdateChallengeTasks(int challengeId,
            IEnumerable<ChallengeTask> challengeTasks)
        {
            int activeUserId = GetActiveUserId();
            int authUserId = GetClaimId(ClaimType.UserId);

            var challengeAlreadyCompleted = 
                await _challengeRepository.GetByIdAsync(challengeId, activeUserId);

            if (challengeAlreadyCompleted.IsCompleted == true)
            {
                _logger.LogError($"User {authUserId} cannot make changes to a completed challenge {challengeId}.");
                throw new GraException("Challenge is already completed.");
            }

            await _challengeRepository.UpdateUserChallengeTask(activeUserId, challengeTasks);
            // check if the challenge was completed
            var challenge = await _challengeRepository.GetByIdAsync(challengeId);
            int pointsAwarded = (int)challenge.PointsAwarded;
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
                await _userLogRepository.AddSaveAsync(activeUserId, userLog);

                // update the score in the user record
                var postUpdateUser = await _userRepository.AddPointsSaveAsync(authUserId,
                    activeUserId,
                    pointsAwarded,
                    false);

                string badgeNotification = null;
                Badge badge = null;
                if (challenge.BadgeId != null)
                {
                    badge = await _badgeRepository.GetByIdAsync((int)challenge.BadgeId);
                    badgeNotification = $" and the badge: {badge.Name}";
                }

                // create the notification record
                var notification = new Notification
                {
                    PointsEarned = pointsAwarded,
                    Text = $"<span class=\"fa fa-star\"></span> You earned <strong>{pointsAwarded} points{badgeNotification}</strong> for completing the challenge: <strong>{challenge.Name}</strong> and currently have <strong>{postUpdateUser.PointsEarned} points</strong>!",
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
    }
}