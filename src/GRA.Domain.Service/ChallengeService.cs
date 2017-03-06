using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using System.Threading.Tasks;
using GRA.Domain.Service.Abstract;
using System.Linq;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Service
{
    public class ChallengeService : Abstract.BaseUserService<ChallengeService>
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly ITriggerRepository _triggerRepository;
        private readonly IUserRepository _userRepository;

        public ChallengeService(ILogger<ChallengeService> logger,
            IUserContextProvider userContextProvider,
            IBadgeRepository badgeRepository,
            IBranchRepository branchRepository,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            ITriggerRepository triggerRepository,
            IUserRepository userRepository) : base(logger, userContextProvider)
        {
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _challengeRepository = Require.IsNotNull(challengeRepository,
                nameof(challengeRepository));
            _challengeTaskRepository = Require.IsNotNull(challengeTaskRepository,
                nameof(challengeTaskRepository));
            _triggerRepository = Require.IsNotNull(triggerRepository, nameof(triggerRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
        }

        public async Task<DataWithCount<IEnumerable<Challenge>>>
            GetPaginatedChallengeListAsync(int skip,
            int take,
            string search = null)
        {
            ICollection<Challenge> challenges = null;
            int challengeCount;
            int siteId = GetCurrentSiteId();
            if (GetAuthUser().Identity.IsAuthenticated)
            {
                var userLookupChallenges = new List<Challenge>();
                int userId = GetActiveUserId();
                var challengeIds = await _challengeRepository.PageIdsAsync(siteId, skip, take, userId, search);
                foreach (var challengeId in challengeIds.Data)
                {
                    var challengeStatus = await _challengeRepository.GetActiveByIdAsync(challengeId, userId);
                    int completed = challengeStatus.Tasks.Count(_ => _.IsCompleted == true);
                    if (completed > 0)
                    {
                        challengeStatus.Status = $"Completed {completed} of {challengeStatus.TasksToComplete} tasks.";
                        challengeStatus.PercentComplete = Math.Min((int)(completed * 100 / challengeStatus.TasksToComplete), 100);
                        challengeStatus.CompletedTasks = completed;
                    }

                    userLookupChallenges.Add(challengeStatus);
                }
                challenges = userLookupChallenges;
                challengeCount = challengeIds.Count;
            }
            else
            {
                BaseFilter filter = new BaseFilter()
                {
                    SiteId = siteId,
                    Skip = skip,
                    Take = take,
                    Search = search,
                    IsActive = true
                };
                challenges = await _challengeRepository.PageAllAsync(filter);
                challengeCount = await _challengeRepository.GetChallengeCountAsync(filter);
            }
            await AddBadgeFilenames(challenges);
            return new DataWithCount<IEnumerable<Challenge>>
            {
                Data = challenges,
                Count = challengeCount
            };
        }

        public async Task<DataWithCount<IEnumerable<Challenge>>>
            MCGetPaginatedChallengeListAsync(BaseFilter filter)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.ViewAllChallenges))
            {
                if (filter.IsActive == false)
                {
                    if (!HasPermission(Permission.ActivateSystemChallenges) && !HasPermission(Permission.ActivateAllChallenges))
                    {
                        _logger.LogError($"User {authUserId} doesn't have permission to view pending challenges.");
                        throw new Exception("Permission denied.");
                    }
                    else if (!HasPermission(Permission.ActivateAllChallenges)
                        && filter.SystemIds?.FirstOrDefault() != GetClaimId(ClaimType.SystemId))
                    {
                        _logger.LogError($"User {authUserId} doesn't have permission to view pending challenges for system.");
                        throw new Exception("Permission denied.");
                    }
                }

                filter.SiteId = GetCurrentSiteId();
                var challenges = await _challengeRepository.PageAllAsync(filter);
                await AddBadgeFilenames(challenges);
                return new DataWithCount<IEnumerable<Challenge>>
                {
                    Data = challenges,
                    Count = await _challengeRepository.GetChallengeCountAsync(filter)
                };
            }
            _logger.LogError($"User {authUserId} doesn't have permission to view all challenges.");
            throw new Exception("Permission denied.");
        }

        public async Task<Challenge> GetChallengeDetailsAsync(int challengeId)
        {
            int? userId = null;
            if (GetAuthUser().Identity.IsAuthenticated)
            {
                userId = GetActiveUserId();
            }
            var challenge = await _challengeRepository.GetActiveByIdAsync(challengeId, userId);
            if (challenge == null)
            {
                throw new GraException("The requested challenge could not be accessed or does not exist.");
            }
            await AddBadgeFilename(challenge);

            return challenge;
        }

        public async Task<Challenge> MCGetChallengeDetailsAsync(int challengeId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.ViewAllChallenges))
            {
                var challenge = await _challengeRepository.GetByIdAsync(challengeId);
                if (challenge == null)
                {
                    throw new GraException("The requested challenge could not be accessed or does not exist.");
                }
                await AddBadgeFilename(challenge);

                return challenge;
            }
            _logger.LogError($"User {authUserId} doesn't have permission to view all challenge {challengeId}.");
            throw new Exception("Permission denied.");
        }

        public async Task<Challenge> AddChallengeAsync(Challenge challenge)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.AddChallenges))
            {
                if (challenge.LimitToSystemId.HasValue && challenge.LimitToBranchId.HasValue)
                {
                    var branch = await _branchRepository
                        .GetByIdAsync(challenge.LimitToBranchId.Value);
                    if (branch.SystemId != challenge.LimitToSystemId.Value)
                    {
                        _logger.LogError($"User {authUserId} cannot set challenge limitaion branch {challenge.LimitToBranchId.Value} for system {challenge.LimitToSystemId.Value}");
                        throw new GraException("Invalid branch limitation.");
                    }
                }
                challenge.IsActive = false;
                challenge.IsDeleted = false;
                challenge.IsValid = false;
                challenge.SiteId = GetCurrentSiteId();
                challenge.RelatedBranchId = GetClaimId(ClaimType.BranchId);
                challenge.RelatedSystemId = GetClaimId(ClaimType.SystemId);
                return await _challengeRepository
                    .AddSaveAsync(GetClaimId(ClaimType.UserId), challenge);
            }
            _logger.LogError($"User {authUserId} doesn't have permission to add a challenge.");
            throw new Exception("Permission denied.");
        }

        public async Task<Challenge> EditChallengeAsync(Challenge challenge)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.EditChallenges))
            {
                if (challenge.LimitToSystemId.HasValue && challenge.LimitToBranchId.HasValue)
                {
                    var branch = await _branchRepository
                        .GetByIdAsync(challenge.LimitToBranchId.Value);
                    if (branch.SystemId != challenge.LimitToSystemId.Value)
                    {
                        _logger.LogError($"User {authUserId} cannot set challenge limitaion branch {challenge.LimitToBranchId.Value} for system {challenge.LimitToSystemId.Value}");
                        throw new GraException("Invalid branch limitation.");
                    }
                }
                var currentChallenge = await _challengeRepository.GetByIdAsync(challenge.Id);
                challenge.SiteId = currentChallenge.SiteId;
                challenge.RelatedBranchId = currentChallenge.RelatedBranchId;
                challenge.RelatedSystemId = currentChallenge.RelatedSystemId;
                if (challenge.TasksToComplete <= currentChallenge.Tasks.Count())
                {
                    challenge.IsValid = true;
                    challenge.IsActive = currentChallenge.IsActive;
                }
                else
                {
                    challenge.IsActive = false;
                    challenge.IsValid = false;
                }
                return await _challengeRepository
                    .UpdateSaveAsync(authUserId, challenge);
            }
            _logger.LogError($"User {authUserId} doesn't have permission to edit challenge {challenge.Id}.");
            throw new GraException("Permission denied.");
        }

        public async Task ActivateChallengeAsync(Challenge challenge)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.ActivateAllChallenges)
                || ((HasPermission(Permission.ActivateSystemChallenges))
                    && challenge.RelatedSystemId == GetClaimId(ClaimType.SystemId)))
            {
                if (challenge.IsValid)
                {
                    challenge.IsActive = true;
                    await _challengeRepository.UpdateSaveAsync(authUserId, challenge);
                }
                else
                {
                    _logger.LogError($"User {authUserId} cannot activate invalid challenge {challenge.Id}.");
                    throw new GraException("Challenge is not valid.");
                }
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to activate challenge {challenge.Id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task RemoveChallengeAsync(int challengeId)
        {
            if (HasPermission(Permission.RemoveChallenges))
            {
                if (await _challengeRepository.HasDependentsAsync(challengeId))
                {
                    throw new GraException("Challenge has dependents");
                }
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
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.EditChallenges))
            {
                var newTask = await _challengeTaskRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), task);

                var challenge = await _challengeRepository.GetByIdAsync(task.ChallengeId);
                if (challenge.TasksToComplete <= challenge.Tasks.Count() && !challenge.IsValid)
                {
                    await _challengeRepository.SetValidationAsync(authUserId, challenge.Id, true);
                }
                return newTask;
            }
            _logger.LogError($"User {authUserId} doesn't have permission to add a task to challenge {task.ChallengeId}.");
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
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.EditChallenges))
            {
                var task = await _challengeTaskRepository.GetByIdAsync(taskId);
                await _challengeTaskRepository
                    .RemoveSaveAsync(GetClaimId(ClaimType.UserId), taskId);

                var challenge = await _challengeRepository.GetByIdAsync(task.ChallengeId);
                if (challenge.TasksToComplete > challenge.Tasks.Count()
                    && (challenge.IsValid || challenge.IsActive))
                {
                    await _challengeRepository.SetValidationAsync(authUserId, challenge.Id, false);
                }
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to remove a challenge task");
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

        public async Task<ICollection<Trigger>> GetDependentsAsync(int challengeId)
        {
            return await _triggerRepository.GetChallengeDependentsAsync(challengeId);
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