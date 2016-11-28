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
        private readonly IChallengeRepository challengeRepository;
        private readonly IChallengeTaskRepository challengeTaskRepository;

        public ChallengeService(ILogger<ChallengeService> logger,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository) : base(logger)
        {
            if (challengeRepository == null)
            {
                throw new ArgumentNullException(nameof(challengeRepository));
            }
            this.challengeRepository = challengeRepository;
            if (challengeTaskRepository == null)
            {
                throw new ArgumentNullException(nameof(challengeTaskRepository));
            }
            this.challengeTaskRepository = challengeTaskRepository;
        }

        /// <summary>
        /// A paginated list of challenges which are visible to the provided user
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="skip">The number of elements to skip before returning the remaining
        /// elements</param>
        /// <param name="take">The number of elements to return</param>
        /// <returns><see cref="DataWithCount{DataType}"/> containing the challenges and the total
        /// challenge count</returns>
        public async Task<DataWithCount<IEnumerable<Challenge>>>
            GetPaginatedChallengeListAsync(ClaimsPrincipal user,
            int skip,
            int take)
        {
            var dataTask = challengeRepository.PageAllAsync(skip, take);
            var countTask = challengeRepository.GetChallengeCountAsync();
            await Task.WhenAll(dataTask, countTask);
            // todo: fix user id
            // todo: add access control - only view authorized challenges
            return new DataWithCount<IEnumerable<Challenge>>
            {
                Data = dataTask.Result,
                Count = countTask.Result
            };
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
            // todo: fix user id
            // todo: add access control - only view authorized challenges
            return await challengeRepository.GetByIdAsync(challengeId);
        }

        /// <summary>
        /// Create a new challenge if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challenge">A populated challenge object</param>
        /// <returns>The challenge which was added with the Id property populated</returns>
        public async Task<Challenge> AddChallengeAsync(ClaimsPrincipal user, Challenge challenge)
        {
            // todo: fix user id
            // todo: add access control - only some users can add
            return await challengeRepository
                .AddSaveAsync(GetId(user, ClaimType.UserId), challenge);
        }

        /// <summary>
        /// Edit an existing challenge if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challenge">The modified challenge object</param>
        /// <returns>The updated challenge</returns>
        public async Task<Challenge> EditChallengeAsync(ClaimsPrincipal user, Challenge challenge)
        {
            // todo: fix user id
            // todo: add access control - only some users can edit
            return await challengeRepository
                .UpdateSaveAsync(GetId(user, ClaimType.UserId), challenge);
        }

        /// <summary>
        /// Remove an existing challenge if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challenge">The id of the challenge to remove</param>
        public async Task RemoveChallengeAsync(ClaimsPrincipal user, int challengeId)
        {
            // todo: fix user id
            // todo: add access control - only some users can remove
            await challengeRepository.RemoveSaveAsync(GetId(user, ClaimType.UserId), challengeId);
        }

        /// <summary>
        /// Create a new task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="task">The task to add to the challenge</param>
        /// <param name="challengeId">The id of the challenge to add the task to</param>
        public async Task<ChallengeTask> AddTaskAsync(ClaimsPrincipal user, ChallengeTask task)
        {
            // todo: fix user id
            return await challengeTaskRepository.AddSaveAsync(GetId(user, ClaimType.UserId), task);
        }

        /// <summary>
        /// Edit an existing task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="task">The modified task object</param>
        public async Task<ChallengeTask> EditTaskAsync(ClaimsPrincipal user, ChallengeTask task)
        {
            // todo: fix user id
            return await challengeTaskRepository
                .UpdateSaveAsync(GetId(user, ClaimType.UserId), task);
        }

        /// <summary>
        /// Get an existing task by id if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="task">TThe id of the task to return</param>
        public async Task<ChallengeTask> GetTaskAsync(ClaimsPrincipal user, int id)
        {
            // todo: fix user id
            return await challengeTaskRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Remove an existing task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="taskId">The id of the task to remove</param>
        public async Task RemoveTaskAsync(ClaimsPrincipal user, int taskId)
        {
            // todo: fix user id
            await challengeTaskRepository
                .RemoveSaveAsync(GetId(user, ClaimType.UserId), taskId);
        }

        /// <summary>
        /// Decrease the sorting position of the task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="taskId">The id of the task whose position to decrease</param>
        public async Task DecreaseTaskPositionAsync(ClaimsPrincipal user, int taskId)
        {
            // todo: fix user id
            await challengeTaskRepository.DecreasePositionAsync(taskId);
        }

        /// <summary>
        /// Increase the sorting position of the task if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="taskId">The id of the task whose position to increase</param>
        public async Task IncreaseTaskPositionAsync(ClaimsPrincipal user, int taskId)
        {
            // todo: fix user id
            await challengeTaskRepository.IncreasePositionAsync(taskId);
        }

        public async Task<IEnumerable<ChallengeTask>> GetChallengeTasksAsync(int challengeId)
        {
            return await challengeRepository.GetChallengeTasksAsync(challengeId);
        }

    }
}