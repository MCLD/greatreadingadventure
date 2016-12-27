using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using System.Threading.Tasks;
using GRA.Domain.Service.Abstract;

namespace GRA.Domain.Service
{
    public class ChallengeService : Abstract.BaseUserService<ChallengeService>
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;

        public ChallengeService(ILogger<ChallengeService> logger,
            IUserContextProvider userContextProvider,
            IBadgeRepository badgeRepository,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository) : base(logger, userContextProvider)
        {
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _challengeRepository = Require.IsNotNull(challengeRepository,
                nameof(challengeRepository));
            _challengeTaskRepository = Require.IsNotNull(challengeTaskRepository,
                nameof(challengeTaskRepository));
        }

        public async Task<DataWithCount<IEnumerable<Challenge>>>
            GetPaginatedChallengeListAsync(int skip,
            int take,
            string search = null)
        {
            int siteId = GetCurrentSiteId();
            var challenges = await _challengeRepository.PageAllAsync(siteId, skip, take, search);
            await AddBadgeFilenames(challenges);
            return new DataWithCount<IEnumerable<Challenge>>
            {
                Data = challenges,
                Count = await _challengeRepository.GetChallengeCountAsync(siteId, search)
            };
        }

        public async Task<Challenge> GetChallengeDetailsAsync(int challengeId)
        {
            int? userId = null;
            if (GetAuthUser().Identity.IsAuthenticated)
            {
                userId = GetActiveUserId();
            }
            var challenge = await _challengeRepository.GetByIdAsync(challengeId, userId);
            await AddBadgeFilename(challenge);
            return challenge;
        }

        public async Task<Challenge> AddChallengeAsync(Challenge challenge)
        {
            if (HasPermission(Permission.AddChallenges))
            {
                challenge.IsDeleted = false;
                challenge.SiteId = GetCurrentSiteId();
                challenge.RelatedBranchId = GetClaimId(ClaimType.BranchId);
                challenge.RelatedSystemId = GetClaimId(ClaimType.SystemId);
                return await _challengeRepository
                    .AddSaveAsync(GetClaimId(ClaimType.UserId), challenge);
            }
            int userId = GetClaimId(ClaimType.UserId);
            _logger.LogError($"User {userId} doesn't have permission to add a challenge.");
            throw new Exception("Permission denied.");
        }

        public async Task<Challenge> EditChallengeAsync(Challenge challenge)
        {
            if (HasPermission(Permission.EditChallenges))
            {
                var currentChallenge = await _challengeRepository.GetByIdAsync(challenge.Id);
                challenge.SiteId = currentChallenge.SiteId;
                challenge.RelatedBranchId = currentChallenge.RelatedBranchId;
                challenge.RelatedSystemId = currentChallenge.RelatedSystemId;
                return await _challengeRepository
                    .UpdateSaveAsync(GetClaimId(ClaimType.UserId), challenge);
            }
            int userId = GetClaimId(ClaimType.UserId);
            _logger.LogError($"User {userId} doesn't have permission to edit challenge {challenge.Id}.");
            throw new Exception("Permission denied.");
        }

        public async Task RemoveChallengeAsync(int challengeId)
        {
            if (HasPermission(Permission.RemoveChallenges))
            {
                await _challengeRepository
                    .RemoveSaveAsync(GetClaimId(ClaimType.UserId), challengeId);
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to remove challenge {challengeId}.");
                throw new Exception("Permission denied.");
            }
        }
        public async Task<ChallengeTask> AddTaskAsync(ChallengeTask task)
        {
            if (HasPermission(Permission.EditChallenges))
            {
                return await _challengeTaskRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), task);
            }
            int userId = GetClaimId(ClaimType.UserId);
            _logger.LogError($"User {userId} doesn't have permission to add a task to challenge {task.ChallengeId}.");
            throw new Exception("Permission denied.");
        }

        public async Task<ChallengeTask> EditTaskAsync(ChallengeTask task)
        {
            if (HasPermission(Permission.EditChallenges))
            {
                return await _challengeTaskRepository
                    .UpdateSaveAsync(GetClaimId(ClaimType.UserId), task);
            }
            int userId = GetClaimId(ClaimType.UserId);
            _logger.LogError($"User {userId} doesn't have permission to edit a task for challenge {task.ChallengeId}.");
            throw new Exception("Permission denied.");
        }

        public async Task<ChallengeTask> GetTaskAsync(int id)
        {
            return await _challengeTaskRepository.GetByIdAsync(id);
        }

        public async Task RemoveTaskAsync(int taskId)
        {
            if (HasPermission(Permission.EditChallenges))
            {
                await _challengeTaskRepository
                    .RemoveSaveAsync(GetClaimId(ClaimType.UserId), taskId);
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to remove a challenge task");
                throw new Exception("Permission denied.");
            }
        }

        public async Task DecreaseTaskPositionAsync(int taskId)
        {
            if (HasPermission(Permission.EditChallenges))
            {
                await _challengeTaskRepository.DecreasePositionAsync(taskId);
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to modify a challenge task");
                throw new Exception("Permission denied.");
            }
        }

        public async Task IncreaseTaskPositionAsync(int taskId)
        {
            if (HasPermission(Permission.EditChallenges))
            {
                await _challengeTaskRepository.IncreasePositionAsync(taskId);
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to modify a challenge task");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<IEnumerable<ChallengeTask>> GetChallengeTasksAsync(int challengeId)
        {
            int? userId = null;
            if (GetAuthUser().Identity.IsAuthenticated)
            {
                userId = GetActiveUserId();
            }
            return await _challengeRepository.GetChallengeTasksAsync(challengeId, userId);
        }

        private async Task AddBadgeFilename(Challenge challenge)
        {
            if (challenge.BadgeId != null)
            {
                var badge = await _badgeRepository.GetByIdAsync((int)challenge.BadgeId);
                if (badge != null)
                {
                    challenge.BadgeFilename = badge.Filename;
                }
            }
        }

        private async Task AddBadgeFilenames(IEnumerable<Challenge> challenges)
        {
            foreach (var challenge in challenges)
            {
                await AddBadgeFilename(challenge);
            }
        }
    }
}