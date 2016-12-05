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

        public async Task<ActivityResult> LogActivityAsync(int userIdToLog,
            int activityAmountEarned)
        {
            var result = new ActivityResult();
            bool goodToLog = false;
            bool loggingAsAdminUser = false;

            int activeUserId = await GetActiveUserId();
            int authUserId = await GetClaimId(ClaimType.UserId);
            var userToLog = await _userRepository.GetByIdAsync(userIdToLog);

            if (activeUserId == userIdToLog)
            {
                goodToLog = true;
            }
            else if (await HasPermission(Permission.LogActivityForAny))
            {
                goodToLog = true;
                loggingAsAdminUser = true;
            }
            else if (userToLog.HouseholdHeadUserId == authUserId)
            {
                // current user is the earning user's head of household
                goodToLog = true;
            }

            if (!goodToLog)
            {
                string error = $"User id {activeUserId} cannot log activity for user id {userIdToLog}";
                _logger.LogError(error);
                throw new Exception(error);
            }

            var translation
                = await _pointTranslationRepository.GetByProgramIdAsync(userToLog.ProgramId);

            result.PointsEarned
                = (activityAmountEarned / translation.ActivityAmount) * translation.PointsEarned;

            // add the row to the user's point log
            var userLog = new UserLog
            {
                ActivityEarned = activityAmountEarned,
                IsDeleted = false,
                PointsEarned = result.PointsEarned,
                UserId = userToLog.Id,
                PointTranslationId = translation.Id
            };
            if (activeUserId != userToLog.Id)
            {
                userLog.AwardedBy = activeUserId;
            }
            await _userLogRepository.AddSaveAsync(activeUserId, userLog);

            // update the score in the user record
            result.User = await _userRepository.AddPointsSaveAsync(activeUserId, 
                userToLog.Id, 
                result.PointsEarned, 
                loggingAsAdminUser);
            return result;
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
            int activeUserId = await GetActiveUserId();
            var activeUser = await _userRepository.GetByIdAsync(activeUserId);
            int authUserId = await GetClaimId(ClaimType.UserId);


            if (userId == activeUserId
                || activeUser.HouseholdHeadUserId == authUserId
                || await HasPermission(Permission.LogActivityForAny))
            {
                await _bookRepository.AddSaveForUserAsync(activeUserId, userId, book);
            }
            else
            {
                _logger.LogError($"User {activeUserId} doesn't have permission to add a book for {userId}.");
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
