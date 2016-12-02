using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;

namespace GRA.Domain.Service
{
    public class ActivityService : Abstract.BaseUserService<UserService>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;

        public ActivityService(ILogger<UserService> logger, 
            IUserContextProvider userContext,
            IBookRepository bookRepository,
            IPointTranslationRepository pointTranslationRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger, userContext)
        {
            _bookRepository = Require.IsNotNull(bookRepository, nameof(bookRepository));
            _pointTranslationRepository = Require.IsNotNull(pointTranslationRepository,
                nameof(pointTranslationRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository,
                nameof(userLogRepository));
        }

        public async Task<User> LogActivityAsync(int userIdToLog,
            int activityAmountEarned)
        {
            bool goodToLog = false;
            bool loggingAsAdminUser = false;

            int currentUserId = await GetClaimId(ClaimType.UserId);
            var userToLog = await _userRepository.GetByIdAsync(userIdToLog);

            if (currentUserId == userIdToLog)
            {
                goodToLog = true;
            }
            else if (await HasPermission(Permission.LogActivityForAny))
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
                _logger.LogError(error);
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

        public async Task<User> RemoveActivityAsync(int userIdToLog,
            int userLogIdToRemove)
        {
            int currentUserId = await GetClaimId(ClaimType.UserId);
            if(await HasPermission(Permission.LogActivityForAny))
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
                throw new Exception(error);
            }
        }

        public async Task AddBook(int userId, Book book)
        {
            int requestedByUserId = await GetClaimId(ClaimType.UserId);
            if (requestedByUserId == userId
                || await HasPermission(Permission.LogActivityForAny))
            {
                await _bookRepository.AddSaveForUserAsync(requestedByUserId, userId, book);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to add a book for {userId}.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task RemoveBook(int userId, int bookId)
        {
            int requestedByUserId = await GetClaimId(ClaimType.UserId);
            if (requestedByUserId == userId
                || await HasPermission(Permission.LogActivityForAny))
            {
                await _bookRepository.RemoveForUserAsync(requestedByUserId, userId, bookId);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to remove a book for {userId}.");
                throw new Exception("Permission denied.");
            }

        }

        public async Task UpdateBook(int userId, Book book)
        {
            int requestedByUserId = await GetClaimId(ClaimType.UserId);
            if (requestedByUserId == userId
                || await HasPermission(Permission.LogActivityForAny))
            {
                await _bookRepository.UpdateSaveAsync(requestedByUserId, book);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to edit a book for {userId}.");
                throw new Exception("Permission denied.");
            }
        }
    }
}
