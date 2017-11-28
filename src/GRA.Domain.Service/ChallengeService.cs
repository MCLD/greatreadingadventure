using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Service.Models;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class ChallengeService : Abstract.BaseUserService<ChallengeService>
    {
        private const string TaskFilesPath = "tasks";
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly IPathResolver _pathResolver;
        private readonly ITriggerRepository _triggerRepository;
        private readonly IUserRepository _userRepository;

        public ChallengeService(ILogger<ChallengeService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBadgeRepository badgeRepository,
            IBranchRepository branchRepository,
            ICategoryRepository categoryRepository,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IPathResolver pathResolver,
            ITriggerRepository triggerRepository,
            IUserRepository userRepository) : base(logger, dateTimeProvider, userContextProvider)
        {
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _categoryRepository = Require.IsNotNull(categoryRepository, nameof(categoryRepository));
            _challengeRepository = Require.IsNotNull(challengeRepository,
                nameof(challengeRepository));
            _challengeTaskRepository = Require.IsNotNull(challengeTaskRepository,
                nameof(challengeTaskRepository));
            _pathResolver = Require.IsNotNull(pathResolver, nameof(pathResolver));
            _triggerRepository = Require.IsNotNull(triggerRepository, nameof(triggerRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
        }

        public async Task<DataWithCount<IEnumerable<Challenge>>>
            GetPaginatedChallengeListAsync(ChallengeFilter filter)
        {
            ICollection<Challenge> challenges = null;
            int challengeCount;

            filter.IsActive = true;
            filter.SiteId = GetCurrentSiteId();
            if (GetAuthUser().Identity.IsAuthenticated)
            {
                var userLookupChallenges = new List<Challenge>();
                int userId = GetActiveUserId();
                filter.UserIds = new List<int>() { userId };
                var challengeIds = await _challengeRepository.PageIdsAsync(filter, userId);
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
            MCGetPaginatedChallengeListAsync(ChallengeFilter filter)
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

        public async Task<ServiceResult<Challenge>> AddChallengeAsync(Challenge challenge)
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
                var serviceResult = new ServiceResult<Challenge>();
                var siteId = GetCurrentSiteId();

                challenge.IsActive = false;
                challenge.IsDeleted = false;
                challenge.IsValid = false;
                challenge.SiteId = siteId;
                challenge.RelatedBranchId = GetClaimId(ClaimType.BranchId);
                challenge.RelatedSystemId = GetClaimId(ClaimType.SystemId);

                if (challenge.CategoryIds?.Count > 0)
                {
                    var validCategoryIds = (await _categoryRepository.GetAllAsync(siteId))
                        .Select(_ => _.Id);
                    var invalidSelectedIds = challenge.CategoryIds.Except(validCategoryIds);

                    if (invalidSelectedIds.Count() > 0)
                    {
                        challenge.CategoryIds = challenge.CategoryIds.Except(invalidSelectedIds)
                            .ToList();
                        serviceResult.Status = ServiceResultStatus.Warning;
                        serviceResult.Message = "One or more of the selected categories could not be added to this challenge.";
                    }
                }

                serviceResult.Data = await _challengeRepository
                    .AddSaveAsync(GetClaimId(ClaimType.UserId), challenge);

                return serviceResult;
            }
            _logger.LogError($"User {authUserId} doesn't have permission to add a challenge.");
            throw new Exception("Permission denied.");
        }

        public async Task<ServiceResult<Challenge>> EditChallengeAsync(Challenge challenge)
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
                var serviceResult = new ServiceResult<Challenge>();

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

                if (challenge.CategoryIds?.Count > 0)
                {
                    var validCategoryIds = (await _categoryRepository.GetAllAsync(GetCurrentSiteId()))
                        .Select(_ => _.Id);
                    var invalidSelectedIds = challenge.CategoryIds.Except(validCategoryIds);

                    if (invalidSelectedIds.Count() > 0)
                    {
                        challenge.CategoryIds = challenge.CategoryIds.Except(invalidSelectedIds)
                            .ToList();
                        serviceResult.Status = ServiceResultStatus.Warning;
                        serviceResult.Message = "One or more of the selected categories could not be added to this challenge.";
                    }
                }
                else
                {
                    challenge.CategoryIds = new List<int>();
                }

                var currentCategoryIds = currentChallenge.Categories.Select(_ => _.Id);
                var categoriesToAdd = challenge.CategoryIds.Except(currentCategoryIds).ToList();
                var categoriesToRemove = currentCategoryIds.Except(challenge.CategoryIds).ToList();

                serviceResult.Data = await _challengeRepository
                    .UpdateSaveAsync(authUserId, challenge, categoriesToAdd, categoriesToRemove);

                return serviceResult;
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
        public async Task<ChallengeTask> AddTaskAsync(ChallengeTask task, byte[] fileBytes = null)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.EditChallenges))
            {
                var newTask = await _challengeTaskRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), task);
                if (fileBytes != null)
                {
                    newTask.Filename = WriteTaskFile(newTask, fileBytes);
                    newTask = await _challengeTaskRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), newTask);
                }

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

        public async Task<ChallengeTask> EditTaskAsync(ChallengeTask task, byte[] fileBytes = null)
        {
            if (HasPermission(Permission.EditChallenges))
            {
                var originalTask = await _challengeTaskRepository.GetByIdAsync(task.Id);
                task.ChallengeId = originalTask.ChallengeId;

                if (fileBytes != null)
                {
                    if (!string.IsNullOrWhiteSpace(originalTask.Filename))
                    {
                        RemoveTaskFile(originalTask);
                    }
                    task.Filename = WriteTaskFile(task, fileBytes);
                }
                else if (!string.IsNullOrWhiteSpace(originalTask.Filename)
                    && string.IsNullOrWhiteSpace(task.Filename))
                {
                    RemoveTaskFile(originalTask);
                    task.Filename = null;

                }
                else
                {
                    task.Filename = originalTask.Filename;
                }
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
                if (await _challengeTaskRepository.UserHasTaskAsync(taskId))
                {
                    throw new GraException("Challenge has been started by a participant.");
                }
                var task = await _challengeTaskRepository.GetByIdAsync(taskId);
                if (!string.IsNullOrWhiteSpace(task.Filename))
                {
                    RemoveTaskFile(task);
                }
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

        private string GetTaskFilePath(string filename)
        {
            string contentDir = _pathResolver.ResolveContentFilePath();
            contentDir = System.IO.Path.Combine(contentDir,
                    $"site{GetCurrentSiteId()}",
                    TaskFilesPath);

            if (!System.IO.Directory.Exists(contentDir))
            {
                System.IO.Directory.CreateDirectory(contentDir);
            }
            return System.IO.Path.Combine(contentDir, filename);
        }

        private string GetTaskUrlPath(string filename)
        {
            return $"site{GetCurrentSiteId()}/{TaskFilesPath}/{filename}";
        }

        private string WriteTaskFile(ChallengeTask task, byte[] taskFile)
        {
            string extension = System.IO.Path.GetExtension(task.Filename).ToLower();
            string filename = $"task{task.Id}{extension}";
            string fullFilePath = GetTaskFilePath(filename);
            _logger.LogInformation($"Writing out task file {fullFilePath}...");
            System.IO.File.WriteAllBytes(fullFilePath, taskFile);
            return GetTaskUrlPath(filename);
        }

        private void RemoveTaskFile(ChallengeTask task)
        {
            var filePath = _pathResolver.ResolveContentFilePath(task.Filename);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}