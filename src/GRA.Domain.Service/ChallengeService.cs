using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class ChallengeService : Abstract.BaseService<ChallengeService>
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;

        public ChallengeService(ILogger<ChallengeService> logger,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository) : base(logger)
        {
            _challengeRepository = Require.IsNotNull(challengeRepository, nameof(challengeRepository));
            _challengeTaskRepository = Require.IsNotNull(challengeTaskRepository, nameof(challengeTaskRepository));
        }

        public async Task<DataWithCount<IEnumerable<Challenge>>>
            GetPaginatedChallengeListAsync(int siteId,
            int skip,
            int take)
        {
            var dataTask = _challengeRepository.PageAllAsync(siteId, skip, take);
            var countTask = _challengeRepository.GetChallengeCountAsync();
            await Task.WhenAll(dataTask, countTask);
            return new DataWithCount<IEnumerable<Challenge>>
            {
                Data = dataTask.Result,
                Count = countTask.Result
            };
        }

        public async Task<DataWithCount<IEnumerable<Challenge>>>
            GetPaginatedChallengeListAsync(ClaimsPrincipal user,
            int skip,
            int take)
        {
            int siteId = GetId(user, ClaimType.SiteId);
            return await GetPaginatedChallengeListAsync(siteId, skip, take);
        }

        /// <summary>
        /// Details on a specific challenge if it's visible to the provided user
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challengeId">A challenge id</param>
        /// <returns>Details for the requested challenge</returns>
        public async Task<Challenge> GetChallengeDetailsAsync(
            ClaimsPrincipal user,
            int challengeId)
        {
            return await _challengeRepository.GetByIdAsync(challengeId);
        }

        /// <summary>
        /// Create a new challenge if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challenge">A populated challenge object</param>
        /// <returns>The challenge which was added with the Id property populated</returns>
        public async Task<Challenge> AddChallengeAsync(ClaimsPrincipal user, Challenge challenge)
        {
            if (UserHasPermission(user, Permission.AddChallenges))
            {
                challenge.IsDeleted = false;
                challenge.SiteId = GetId(user, ClaimType.SiteId);
                challenge.RelatedBranchId = GetId(user, ClaimType.BranchId);
                challenge.RelatedSystemId = GetId(user, ClaimType.SystemId);
                return await _challengeRepository
                    .AddSaveAsync(GetId(user, ClaimType.UserId), challenge);
            }
            int userId = GetId(user, ClaimType.UserId);
            logger.LogError($"User {userId} doesn't have permission to add a challenge.");
            throw new Exception("Permission denied.");
        }

        /// <summary>
        /// Edit an existing challenge if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challenge">The modified challenge object</param>
        /// <returns>The updated challenge</returns>
        public async Task<Challenge> EditChallengeAsync(ClaimsPrincipal user, Challenge challenge)
        {
            if (UserHasPermission(user, Permission.EditChallenges))
            {
                var currentChallenge = await _challengeRepository.GetByIdAsync(challenge.Id);
                challenge.SiteId = currentChallenge.SiteId;
                challenge.RelatedBranchId = currentChallenge.RelatedBranchId;
                challenge.RelatedSystemId = currentChallenge.RelatedSystemId;
                return await _challengeRepository
                    .UpdateSaveAsync(GetId(user, ClaimType.UserId), challenge);
            }
            int userId = GetId(user, ClaimType.UserId);
            logger.LogError($"User {userId} doesn't have permission to edit challenge {challenge.Id}.");
            throw new Exception("Permission denied.");
        }

        /// <summary>
        /// Remove an existing challenge if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challenge">The id of the challenge to remove</param>
        public async Task RemoveChallengeAsync(ClaimsPrincipal user, int challengeId)
        {
            if (UserHasPermission(user, Permission.RemoveChallenges))
            {
                await _challengeRepository
                    .RemoveSaveAsync(GetId(user, ClaimType.UserId), challengeId);
            }
            else
            {
                int userId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {userId} doesn't have permission to remove challenge {challengeId}.");
                throw new Exception("Permission denied.");
            }
        }

        /// <summary>
        /// Create a new task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="task">The task to add to the challenge</param>
        /// <param name="challengeId">The id of the challenge to add the task to</param>
        public async Task<ChallengeTask> AddTaskAsync(ClaimsPrincipal user, ChallengeTask task)
        {
            if (UserHasPermission(user, Permission.EditChallenges))
            {
                return await _challengeTaskRepository.AddSaveAsync(GetId(user, ClaimType.UserId), task);
            }
            int userId = GetId(user, ClaimType.UserId);
            logger.LogError($"User {userId} doesn't have permission to add a task to challenge {task.ChallengeId}.");
            throw new Exception("Permission denied.");
        }

        /// <summary>
        /// Edit an existing task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="task">The modified task object</param>
        public async Task<ChallengeTask> EditTaskAsync(ClaimsPrincipal user, ChallengeTask task)
        {
            if (UserHasPermission(user, Permission.EditChallenges))
            {
                return await _challengeTaskRepository
                    .UpdateSaveAsync(GetId(user, ClaimType.UserId), task);
            }
            int userId = GetId(user, ClaimType.UserId);
            logger.LogError($"User {userId} doesn't have permission to edit a task for challenge {task.ChallengeId}.");
            throw new Exception("Permission denied.");
        }

        /// <summary>
        /// Get an existing task by id if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="task">The id of the task to return</param>
        public async Task<ChallengeTask> GetTaskAsync(ClaimsPrincipal user, int id)
        {
            return await _challengeTaskRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Remove an existing task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="taskId">The id of the task to remove</param>
        public async Task RemoveTaskAsync(ClaimsPrincipal user, int taskId)
        {
            if (UserHasPermission(user, Permission.EditChallenges))
            {
                await _challengeTaskRepository
                    .RemoveSaveAsync(GetId(user, ClaimType.UserId), taskId);
            }
            else
            {
                int userId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {userId} doesn't have permission to remove a challenge task");
                throw new Exception("Permission denied.");
            }
        }

        /// <summary>
        /// Decrease the sorting position of the task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="taskId">The id of the task whose position to decrease</param>
        public async Task DecreaseTaskPositionAsync(ClaimsPrincipal user, int taskId)
        {
            if (UserHasPermission(user, Permission.EditChallenges))
            {
                await _challengeTaskRepository.DecreasePositionAsync(taskId);
            }
            else
            {
                int userId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {userId} doesn't have permission to modify a challenge task");
                throw new Exception("Permission denied.");
            }
        }

        /// <summary>
        /// Increase the sorting position of the task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="taskId">The id of the task whose position to increase</param>
        public async Task IncreaseTaskPositionAsync(ClaimsPrincipal user, int taskId)
        {
            if (UserHasPermission(user, Permission.EditChallenges))
            {
                await _challengeTaskRepository.IncreasePositionAsync(taskId);
            }
            else
            {
                int userId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {userId} doesn't have permission to modify a challenge task");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<IEnumerable<ChallengeTask>> GetChallengeTasksAsync(int challengeId)
        {
            return await _challengeRepository.GetChallengeTasksAsync(challengeId);
        }
    }
}