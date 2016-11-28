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
        private readonly IPointTranslationRepository pointTranslationRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserLogRepository userLogRepository;

        public ActivityService(ILogger<UserService> logger,
            IPointTranslationRepository pointTranslationRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger)
        {
            this.pointTranslationRepository = Require.IsNotNull(pointTranslationRepository,
                nameof(pointTranslationRepository));
            this.userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            this.userLogRepository = Require.IsNotNull(userLogRepository,
                nameof(userLogRepository));
        }

        public async Task<User> LogActivityAsync(ClaimsPrincipal currentUser,
            int userIdToLog,
            int activityAmountEarned)
        {
            bool goodToLog = false;
            bool loggingAsAdminUser = false;

            var claimLookup = new UserClaimLookup(currentUser);
            int currentUserId = int.Parse(claimLookup.UserClaim(ClaimType.UserId));
            var userToLog = await userRepository.GetByIdAsync(userIdToLog);

            if (currentUserId == userIdToLog)
            {
                goodToLog = true;
            }
            else if (claimLookup.UserHasPermission(Permission.LogActivityForAll.ToString()))
            {
                goodToLog = true;
                loggingAsAdminUser = true;
            }
            else if (userToLog.HeadOfHouseholdUserId == currentUserId)
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
                = await pointTranslationRepository.GetByProgramIdAsync(userToLog.ProgramId);

            int pointsEarned
                = (activityAmountEarned / translation.ActivityAmount) * translation.PointsEarned;

            // add the row to the user's point log
            var userLog = new UserLog
            {
                ActivityEarned = activityAmountEarned,
                IsDeleted = false,
                PointsEarned = pointsEarned,
                UserId = userToLog.Id
            };
            if (currentUserId != userToLog.Id)
            {
                userLog.AwardedBy = currentUserId;
            }
            await userLogRepository.AddSaveAsync(currentUserId, userLog);

            // update the score in the user record
            return await userRepository
                .AddPointsSaveAsync(currentUserId, userToLog.Id, pointsEarned, loggingAsAdminUser);
        }
    }
}
