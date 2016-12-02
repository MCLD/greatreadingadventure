using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using GRA.Domain.Repository;
using GRA.Domain.Model;

namespace GRA.Domain.Service
{
    public class ActivityService : Abstract.BaseService<UserService>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;

        public ActivityService(ILogger<UserService> logger,
            IBookRepository bookRepository,
            IPointTranslationRepository pointTranslationRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger)
        {
            _bookRepository = Require.IsNotNull(bookRepository, nameof(bookRepository));
            _pointTranslationRepository = Require.IsNotNull(pointTranslationRepository,
                nameof(pointTranslationRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository,
                nameof(userLogRepository));
        }

        public async Task<User> LogActivityAsync(ClaimsPrincipal currentUser,
            int userIdToLog,
            int activityAmountEarned)
        {
            bool goodToLog = false;
            bool loggingAsAdminUser = false;

            int currentUserId = GetId(currentUser, ClaimType.UserId);
            var userToLog = await _userRepository.GetByIdAsync(userIdToLog);

            if (currentUserId == userIdToLog)
            {
                goodToLog = true;
            }
            else if (UserHasPermission(currentUser, Permission.LogActivityForAny))
            {
                goodToLog = true;
                loggingAsAdminUser = true;
            }
            else if (userToLog.HouseholdHeadUserId == currentUserId)
            {
                // current user is the earning user's head of household
                goodToLog = true;
            }

            if (!goodToLog)
            {
                string error = $"User id {currentUserId} cannot log activity for user id {userIdToLog}";
                logger.LogError(error);
                throw new Exception(error);
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
            if (currentUserId != userToLog.Id)
            {
                userLog.AwardedBy = currentUserId;
            }
            await _userLogRepository.AddSaveAsync(currentUserId, userLog);

            // update the score in the user record
            return await _userRepository
                .AddPointsSaveAsync(currentUserId, userToLog.Id, pointsEarned, loggingAsAdminUser);
        }

        public async Task<User> RemoveActivityAsync(ClaimsPrincipal user,
            int userIdToLog,
            int userLogIdToRemove)
        {
            int currentUserId = GetId(user, ClaimType.UserId);
            if(UserHasPermission(user, Permission.LogActivityForAny))
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
                logger.LogError(error);
                throw new Exception(error);
            }
        }

        public async Task AddBook(ClaimsPrincipal user, int userId, Book book)
        {
            int requestedByUserId = GetId(user, ClaimType.UserId);
            if (requestedByUserId == userId
                || UserHasPermission(user, Permission.LogActivityForAny))
            {
                await _bookRepository.AddSaveForUserAsync(requestedByUserId, userId, book);
            }
            else
            {
                logger.LogError($"User {requestedByUserId} doesn't have permission to add a book for {userId}.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task RemoveBook(ClaimsPrincipal user, int userId, int bookId)
        {
            int requestedByUserId = GetId(user, ClaimType.UserId);
            if (requestedByUserId == userId
                || UserHasPermission(user, Permission.LogActivityForAny))
            {
                await _bookRepository.RemoveForUserAsync(requestedByUserId, userId, bookId);
            }
            else
            {
                logger.LogError($"User {requestedByUserId} doesn't have permission to remove a book for {userId}.");
                throw new Exception("Permission denied.");
            }

        }

        public async Task UpdateBook(ClaimsPrincipal user, int userId, Book book)
        {
            int requestedByUserId = GetId(user, ClaimType.UserId);
            if (requestedByUserId == userId
                || UserHasPermission(user, Permission.LogActivityForAny))
            {
                await _bookRepository.UpdateSaveAsync(requestedByUserId, book);
            }
            else
            {
                logger.LogError($"User {requestedByUserId} doesn't have permission to edit a book for {userId}.");
                throw new Exception("Permission denied.");
            }
        }
    }
}
