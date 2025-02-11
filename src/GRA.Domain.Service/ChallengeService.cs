using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    public class ChallengeService : BaseUserService<ChallengeService>
    {
        private const string FeaturedFilesPath = "featuredchallenges";
        private const string TaskFilesPath = "tasks";

        private readonly IBadgeRepository _badgeRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IGraCache _cache;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IChallengeGroupRepository _challengeGroupRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IFeaturedChallengeGroupRepository _featuredChallengeGroupRepository;
        private readonly LanguageService _languageService;
        private readonly IPathResolver _pathResolver;
        private readonly SiteLookupService _siteLookupService;
        private readonly ITriggerRepository _triggerRepository;

        public ChallengeService(ILogger<ChallengeService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IGraCache cache,
            IUserContextProvider userContextProvider,
            IBadgeRepository badgeRepository,
            IBranchRepository branchRepository,
            ICategoryRepository categoryRepository,
            IChallengeGroupRepository challengeGroupRepository,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IEventRepository eventRepository,
            IFeaturedChallengeGroupRepository featuredChallengeGroupRepository,
            IPathResolver pathResolver,
            ITriggerRepository triggerRepository,
            LanguageService languageService,
            SiteLookupService siteLookupService
            )
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(badgeRepository);
            ArgumentNullException.ThrowIfNull(branchRepository);
            ArgumentNullException.ThrowIfNull(cache);
            ArgumentNullException.ThrowIfNull(categoryRepository);
            ArgumentNullException.ThrowIfNull(challengeGroupRepository);
            ArgumentNullException.ThrowIfNull(challengeRepository);
            ArgumentNullException.ThrowIfNull(challengeTaskRepository);
            ArgumentNullException.ThrowIfNull(eventRepository);
            ArgumentNullException.ThrowIfNull(featuredChallengeGroupRepository);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(pathResolver);
            ArgumentNullException.ThrowIfNull(siteLookupService);
            ArgumentNullException.ThrowIfNull(triggerRepository);

            _badgeRepository = badgeRepository;
            _branchRepository = branchRepository;
            _cache = cache;
            _categoryRepository = categoryRepository;
            _challengeGroupRepository = challengeGroupRepository;
            _challengeRepository = challengeRepository;
            _challengeTaskRepository = challengeTaskRepository;
            _eventRepository = eventRepository;
            _featuredChallengeGroupRepository = featuredChallengeGroupRepository;
            _languageService = languageService;
            _pathResolver = pathResolver;
            _siteLookupService = siteLookupService;
            _triggerRepository = triggerRepository;
        }

        public async Task ActivateChallengeAsync(Challenge challenge)
        {
            ArgumentNullException.ThrowIfNull(challenge);

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
                    _logger.LogError("User {AuthenticatedUserId} cannot activate invalid challenge {ChallengeId}.",
                        authUserId,
                        challenge.Id);
                    throw new GraException("Challenge is not valid.");
                }
            }
            else
            {
                _logger.LogError("User {AuthenticatedUserId} doesn't have permission to activate challenge {ChallengeId}.",
                    authUserId,
                    challenge.Id);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<ServiceResult<Challenge>> AddChallengeAsync(Challenge challenge)
        {
            ArgumentNullException.ThrowIfNull(challenge);

            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.AddChallenges))
            {
                var maxPointLimit = await GetMaximumAllowedPointsAsync(GetCurrentSiteId());
                if (maxPointLimit.HasValue
                    && !HasPermission(Permission.IgnorePointLimits)
                    && challenge.PointsAwarded > maxPointLimit * challenge.TasksToComplete)
                {
                    throw new GraException($"A challenge with {challenge.TasksToComplete} tasks may award a maximum of {maxPointLimit * challenge.TasksToComplete} points.");
                }
                if (challenge.LimitToSystemId.HasValue && challenge.LimitToBranchId.HasValue)
                {
                    var branch = await _branchRepository
                        .GetByIdAsync(challenge.LimitToBranchId.Value);
                    if (branch.SystemId != challenge.LimitToSystemId.Value)
                    {
                        _logger.LogError("User {AuthenticatedUserId} cannot set challenge limitaion branch {BranchId} for system {SystemId}",
                            authUserId,
                            challenge.LimitToBranchId.Value,
                            challenge.LimitToSystemId.Value);
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

                    if (invalidSelectedIds.Any())
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
            _logger.LogError("User {AuthenticatedUserId} doesn't have permission to add a challenge.",
                authUserId);
            throw new GraException("Permission denied.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize Web slugs to lowercase")]
        public async Task<ServiceResult<ChallengeGroup>>
            AddGroupAsync(ChallengeGroup challengeGroup, ICollection<int> ChallengeIds)
        {
            ArgumentNullException.ThrowIfNull(challengeGroup);
            ArgumentNullException.ThrowIfNull(ChallengeIds);

            VerifyPermission(Permission.AddChallengeGroups);

            var siteId = GetCurrentSiteId();
            var stub = challengeGroup.Stub.Trim().ToLowerInvariant();
            var existingStub = await _challengeGroupRepository.StubInUseAsync(siteId, stub);
            if (existingStub)
            {
                throw new GraException($"A challenge group with the link {stub} already exists.");
            }

            var serviceResult = new ServiceResult<ChallengeGroup>();
            challengeGroup.SiteId = siteId;
            challengeGroup.Stub = stub;

            var validChallengeIds = await _challengeRepository.ValidateChallengeIdsAsync(siteId,
                ChallengeIds);

            if (ChallengeIds.Count != validChallengeIds.Count())
            {
                serviceResult.Status = ServiceResultStatus.Warning;
                serviceResult.Message = "One or more of the selected challenges could not be added to this group.";
            }

            serviceResult.Data = await _challengeGroupRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), challengeGroup, validChallengeIds);

            return serviceResult;
        }

        public async Task<ChallengeTask> AddTaskAsync(ChallengeTask task, byte[] fileBytes = null)
        {
            ArgumentNullException.ThrowIfNull(task);

            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.EditChallenges))
            {
                var newTask = await _challengeTaskRepository
                    .AddSaveAsync(GetClaimId(ClaimType.UserId), task);
                newTask.ChallengeTaskType = task.ChallengeTaskType;
                if (fileBytes != null)
                {
                    newTask.Filename = WriteTaskFile(newTask, fileBytes);
                    newTask = await _challengeTaskRepository
                        .UpdateSaveAsync(GetClaimId(ClaimType.UserId), newTask);
                }

                var challenge = await _challengeRepository.GetByIdAsync(task.ChallengeId);
                if (challenge.TasksToComplete <= challenge.Tasks.Count() && !challenge.IsValid)
                {
                    await _challengeRepository.SetValidationAsync(authUserId, challenge.Id, true);
                }

                return newTask;
            }
            _logger.LogError("User {AuthenticatedUserId} doesn't have permission to add a task to challenge {ChallengeId}.",
                authUserId,
                task.ChallengeId);
            throw new GraException("Permission denied.");
        }

        public async Task DecreaseTaskPositionAsync(int taskId)
        {
            if (HasPermission(Permission.EditChallenges))
            {
                await _challengeTaskRepository.DecreasePositionAsync(taskId);
            }
            else
            {
                _logger.LogError("User {AuthenticatedUserId} doesn't have permission to modify a challenge task",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
        }

        public async Task<ServiceResult<Challenge>> EditChallengeAsync(Challenge challenge)
        {
            ArgumentNullException.ThrowIfNull(challenge);

            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.EditChallenges))
            {
                var currentChallenge = await _challengeRepository.GetByIdAsync(challenge.Id);

                var maxPointLimit = await GetMaximumAllowedPointsAsync(GetCurrentSiteId());
                if (maxPointLimit.HasValue && !HasPermission(Permission.IgnorePointLimits))
                {
                    if (currentChallenge.PointsAwarded >
                        maxPointLimit * currentChallenge.TasksToComplete)
                    {
                        throw new GraException("Permission denied.");
                    }

                    if (challenge.PointsAwarded > maxPointLimit * challenge.TasksToComplete)
                    {
                        throw new GraException($"A challenge with {challenge.TasksToComplete} tasks may award a maximum of {maxPointLimit * challenge.TasksToComplete} points.");
                    }
                }
                if (challenge.LimitToSystemId.HasValue && challenge.LimitToBranchId.HasValue)
                {
                    var branch = await _branchRepository
                        .GetByIdAsync(challenge.LimitToBranchId.Value);
                    if (branch.SystemId != challenge.LimitToSystemId.Value)
                    {
                        _logger.LogError("User {AuthenticatedUserId} cannot set challenge limitaion branch {BranchId} for system {SystemId}",
                            authUserId,
                            challenge.LimitToBranchId.Value,
                            challenge.LimitToSystemId.Value);
                        throw new GraException("Invalid branch limitation.");
                    }
                }
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
                    var validCategoryIds
                        = (await _categoryRepository.GetAllAsync(GetCurrentSiteId()))
                            .Select(_ => _.Id);
                    var invalidSelectedIds = challenge.CategoryIds.Except(validCategoryIds);

                    if (invalidSelectedIds.Any())
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
            _logger.LogError("User {AuthenticatedUserId} doesn't have permission to edit challenge {ChallengeId}.",
                authUserId,
                challenge.Id);
            throw new GraException("Permission denied.");
        }

        public async Task<ServiceResult<ChallengeGroup>> EditGroupAsync(
            ChallengeGroup challengeGroup,
            ICollection<int> ChallengeIds)
        {
            ArgumentNullException.ThrowIfNull(challengeGroup);
            ArgumentNullException.ThrowIfNull(ChallengeIds);

            VerifyPermission(Permission.EditChallengeGroups);

            var siteId = GetCurrentSiteId();
            var serviceResult = new ServiceResult<ChallengeGroup>();
            var currentChallengeGroup = await _challengeGroupRepository.GetByIdAsync(
                challengeGroup.Id);
            challengeGroup.SiteId = currentChallengeGroup.SiteId;
            challengeGroup.Stub = currentChallengeGroup.Stub;

            var validChallengeIds = await _challengeRepository.ValidateChallengeIdsAsync(siteId,
                ChallengeIds);
            if (ChallengeIds.Count != validChallengeIds.Count())
            {
                serviceResult.Status = ServiceResultStatus.Warning;
                serviceResult.Message = "One or more of the selected challenges could not be added to this group.";
            }

            var challengesToAdd = ChallengeIds.Except(currentChallengeGroup.ChallengeIds);
            var challengesToRemove = currentChallengeGroup.ChallengeIds.Except(ChallengeIds);

            serviceResult.Data = await _challengeGroupRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), challengeGroup, challengesToAdd, challengesToRemove);

            return serviceResult;
        }

        public async Task<ChallengeTask> EditTaskAsync(ChallengeTask task, byte[] fileBytes = null)
        {
            ArgumentNullException.ThrowIfNull(task);

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
            _logger.LogError("User {AuthenticatedUserId} doesn't have permission to edit a task for challenge {ChallengeId}.",
                userId,
                task.ChallengeId);
            throw new GraException("Permission denied.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize Web slugs to lowercase")]
        public async Task<ChallengeGroup> GetActiveGroupByStubAsync(string stub)
        {
            ArgumentNullException.ThrowIfNull(stub);

            return await _challengeGroupRepository.GetActiveByStubAsync(GetCurrentSiteId(),
                stub.ToLowerInvariant());
        }

        public async Task<List<Challenge>> GetByIdsAsync(IEnumerable<int> challengeIds)
        {
            VerifyPermission(Permission.ViewAllChallenges);
            return await _challengeRepository.GetByIdsAsync(GetCurrentSiteId(), challengeIds);
        }

        public async Task<Challenge> GetChallengeDetailsAsync(int challengeId)
        {
            int? userId = null;
            if (GetAuthUser().Identity.IsAuthenticated)
            {
                userId = GetActiveUserId();
            }
            var challenge = await _challengeRepository.GetActiveByIdAsync(challengeId, userId)
                ?? throw new GraException("Challenge not found.");

            await AddBadgeFileData(challenge);

            return challenge;
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

        public async Task<ChallengeGroup> GetGroupByIdAsync(int id)
        {
            var challengeGroup = await _challengeGroupRepository.GetByIdAsync(id)
                ?? throw new GraException("The request challenge group could not be accessed or does not exist");

            challengeGroup.Challenges = await _challengeRepository.GetByIdsAsync(GetCurrentSiteId(),
                challengeGroup.ChallengeIds);
            await AddBadgeFilenames(challengeGroup.Challenges);

            return challengeGroup;
        }

        public async Task<ICollection<ChallengeGroup>> GetGroupListAsync()
        {
            VerifyPermission(Permission.ViewAllChallenges);
            return await _challengeGroupRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<List<ChallengeGroup>> GetGroupsByChallengeId(int id)
        {
            VerifyPermission(Permission.ViewAllChallenges);
            return await _challengeGroupRepository.GetByChallengeId(GetCurrentSiteId(), id);
        }

        public async Task<int?> GetMaximumAllowedPointsAsync(int siteId)
        {
            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingIntAsync(siteId,
                SiteSettingKey.Challenges.MaxPointsPerChallengeTask);
            return IsSet ? SetValue : null;
        }

        public async Task<DataWithCount<IEnumerable<Challenge>>>
            GetPaginatedChallengeListAsync(ChallengeFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            ICollection<Challenge> challenges = null;
            int challengeCount;

            filter.IsActive = true;
            filter.SiteId = GetCurrentSiteId();
            if (GetAuthUser().Identity.IsAuthenticated)
            {
                var userLookupChallenges = new List<Challenge>();
                int userId = GetActiveUserId();
                filter.FavoritesUserId = userId;
                var challengeIds = await _challengeRepository.PageIdsAsync(filter, userId);
                foreach (var challengeId in challengeIds.Data)
                {
                    var challengeStatus = await _challengeRepository
                        .GetActiveByIdAsync(challengeId, userId);
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

        public async Task<DataWithCount<IEnumerable<ChallengeGroup>>>
            GetPaginatedGroupListAsync(ChallengeGroupFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            VerifyPermission(Permission.ViewAllChallenges);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<IEnumerable<ChallengeGroup>>
            {
                Data = await _challengeGroupRepository.PageAsync(filter),
                Count = await _challengeGroupRepository.CountAsync(filter)
            };
        }

        public async Task<ChallengeTask> GetTaskAsync(int id)
        {
            return await _challengeTaskRepository.GetByIdAsync(id);
        }

        public async Task<int> GetTotalChallengeCount()
        {
            return await _challengeRepository.GetChallengeCountAsync(new ChallengeFilter
            {
                IsActive = true,
                SiteId = GetCurrentSiteId()
            });
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
                _logger.LogError("User {AuthenticatedUserId} doesn't have permission to modify a challenge task",
                    userId);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<Challenge> MCGetChallengeDetailsAsync(int challengeId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.ViewAllChallenges))
            {
                var challenge = await _challengeRepository.GetByIdAsync(challengeId)
                    ?? throw new GraException("Challenge not found.");
                await AddBadgeFileData(challenge);

                return challenge;
            }
            _logger.LogError("User {AuthenticatedUserId} doesn't have permission to view all challenge {ChallengeId}.",
                authUserId,
                challengeId);
            throw new GraException("Permission denied.");
        }

        public async Task<DataWithCount<IEnumerable<Challenge>>>
            MCGetPaginatedChallengeListAsync(ChallengeFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.ViewAllChallenges))
            {
                if (filter.IsActive == false)
                {
                    if (!HasPermission(Permission.ActivateSystemChallenges)
                        && !HasPermission(Permission.ActivateAllChallenges))
                    {
                        _logger.LogError("User {AuthenticatedUserId} doesn't have permission to view pending challenges.",
                            authUserId);
                        throw new GraException("Permission denied.");
                    }
                    else if (!HasPermission(Permission.ActivateAllChallenges)
                        && filter.SystemIds?.FirstOrDefault() != GetClaimId(ClaimType.SystemId))
                    {
                        _logger.LogError("User {AuthenticatedUserId} doesn't have permission to view pending challenges for system.",
                            authUserId);
                        throw new GraException("Permission denied.");
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
            _logger.LogError("User {AuthenticatedUserId} doesn't have permission to view all challenges.",
                authUserId);
            throw new GraException("Permission denied.");
        }

        public async Task RemoveChallengeAsync(int challengeId)
        {
            var userId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.RemoveChallenges))
            {
                if (await _challengeRepository.HasDependentsAsync(challengeId))
                {
                    throw new GraException("Challenge has dependents");
                }
                await _eventRepository.DetachRelatedChallenge(userId, challengeId);
                await _challengeRepository
                    .RemoveSaveAsync(userId, challengeId);
            }
            else
            {
                _logger.LogError("User {AuthenticatedUserId} doesn't have permission to remove challenge {ChallengeId}.",
                    userId,
                    challengeId);
                throw new GraException("Permission denied.");
            }
        }

        public async Task RemoveGroupAsync(int groupId)
        {
            VerifyPermission(Permission.EditChallengeGroups);
            var userId = GetClaimId(ClaimType.UserId);
            await _eventRepository.DetachRelatedChallengeGroup(userId, groupId);
            await _challengeGroupRepository.RemoveSaveAsync(userId, groupId);
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
                _logger.LogError("User {AuthenticatedUserId} doesn't have permission to remove a challenge task",
                    authUserId);
                throw new GraException("Permission denied.");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize Web slugs to lowercase")]
        public async Task<bool> StubInUseAsync(string stub)
        {
            VerifyPermission(Permission.AddChallengeGroups);
            return await _challengeGroupRepository
                .StubInUseAsync(GetCurrentSiteId(), stub?.ToLowerInvariant());
        }

        private async Task AddBadgeFileData(Challenge challenge)
        {
            if (challenge.BadgeId != null)
            {
                var badge = await _badgeRepository.GetByIdAsync((int)challenge.BadgeId);
                if (badge != null)
                {
                    challenge.BadgeFilename = badge.Filename;
                    challenge.BadgeAltText = badge.AltText;
                }
            }
        }

        private async Task AddBadgeFilenames(IEnumerable<Challenge> challenges)
        {
            foreach (var challenge in challenges)
            {
                await AddBadgeFileData(challenge);
            }
        }

        private string GetTaskFilePath(string filename)
        {
            string contentDir = _pathResolver.ResolveContentFilePath();
            contentDir = Path.Combine(contentDir, $"site{GetCurrentSiteId()}", TaskFilesPath);

            if (!Directory.Exists(contentDir))
            {
                Directory.CreateDirectory(contentDir);
            }
            return Path.Combine(contentDir, filename);
        }

        private string GetTaskUrlPath(string filename)
        {
            return $"site{GetCurrentSiteId()}/{TaskFilesPath}/{filename}";
        }

        private void RemoveTaskFile(ChallengeTask task)
        {
            var filePath = _pathResolver.ResolveContentFilePath(task.Filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize file extensions to lowercase")]
        private string WriteTaskFile(ChallengeTask task, byte[] taskFile)
        {
            string extension = Path.GetExtension(task.Filename).ToLowerInvariant();
            string filename = $"task{task.Id}{extension}";
            string fullFilePath = GetTaskFilePath(filename);
            _logger.LogDebug("Writing out task file {TaskFile}", fullFilePath);
            File.WriteAllBytes(fullFilePath, taskFile);
            return GetTaskUrlPath(filename);
        }

        #region Featured Challenge Groups methods

        public async Task<FeaturedChallengeGroup> AddFeaturedGroupAsync(
            FeaturedChallengeGroup featuredGroup,
            FeaturedChallengeGroupText featuredGroupText,
            string filename,
            byte[] imageBytes)
        {
            ArgumentNullException.ThrowIfNull(featuredGroup);
            ArgumentNullException.ThrowIfNull(featuredGroupText);
            VerifyPermission(Permission.ManageFeaturedChallengeGroups);

            var siteId = GetCurrentSiteId();

            featuredGroup.Name = featuredGroup.Name?.Trim();
            featuredGroup.SiteId = siteId;

            var maxSortOrder = await _featuredChallengeGroupRepository.GetMaxSortOrderAsync(siteId);
            if (maxSortOrder.HasValue)
            {
                featuredGroup.SortOrder = maxSortOrder.Value + 1;
            }

            var addedFeaturedGroup = await _featuredChallengeGroupRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId),
                featuredGroup);

            featuredGroupText.AltText = featuredGroupText.AltText?.Trim();
            featuredGroupText.Filename = (await HandleFeaturedImage(filename, imageBytes))?.Trim();

            await _featuredChallengeGroupRepository.AddTextAsync(featuredGroupText,
                addedFeaturedGroup.Id,
                await _languageService.GetDefaultLanguageIdAsync());

            await _featuredChallengeGroupRepository.SaveAsync();

            await ClearFeaturedChallengeGroupsCacheAsync();

            return addedFeaturedGroup;
        }

        public async Task<FeaturedChallengeGroup> EditFeaturedGroupAsync(
            FeaturedChallengeGroup group,
            FeaturedChallengeGroupText text)
        {
            ArgumentNullException.ThrowIfNull(group);
            ArgumentNullException.ThrowIfNull(text);
            VerifyPermission(Permission.ManageFeaturedChallengeGroups);

            var featuredGroup = await _featuredChallengeGroupRepository.GetByIdAsync(group.Id);

            featuredGroup.ChallengeGroupId = group.ChallengeGroupId;
            featuredGroup.EndDate = group.EndDate;
            featuredGroup.Name = group.Name?.Trim();
            featuredGroup.StartDate = group.StartDate;

            var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();
            var groupText = await _featuredChallengeGroupRepository
                .GetTextByFeaturedGroupAndLanguageAsync(featuredGroup.Id, defaultLanguageId);

            groupText.AltText = text.AltText?.Trim();

            await _featuredChallengeGroupRepository.UpdateAsync(GetClaimId(ClaimType.UserId),
                featuredGroup);
            await _featuredChallengeGroupRepository.UpdateTextAsync(groupText,
                featuredGroup.Id,
                defaultLanguageId);
            await _featuredChallengeGroupRepository.SaveAsync();

            await ClearFeaturedChallengeGroupsCacheAsync();

            return featuredGroup;
        }

        public async Task<IEnumerable<DisplayChallengeGroup>> GetActiveFeaturedChallengeGroupsAsync()
        {
            var currentLanguageId = await _languageService
                .GetLanguageIdAsync(_userContextProvider.GetCurrentCulture().Name);
            var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();

            string cacheKey = string.Format(CultureInfo.InvariantCulture,
                CacheKey.FeaturedChallengeGroups,
                currentLanguageId);

            var featured = await _cache
                .GetObjectFromCacheAsync<IEnumerable<DisplayChallengeGroup>>(cacheKey);

            if (featured == null)
            {
                featured = await _featuredChallengeGroupRepository
                    .GetActiveAsync(GetCurrentSiteId(), currentLanguageId, defaultLanguageId);

                foreach (var featuredItem in featured)
                {
                    featuredItem.ImagePath = '/' + GetFeaturedUrlPath(featuredItem.ImageFile);
                }

                var now = _dateTimeProvider.Now;

                var nextFeaturedChallengeChange = await _featuredChallengeGroupRepository
                    .GetNextActiveTimestampAsync(GetCurrentSiteId());

                if (nextFeaturedChallengeChange == null
                    || nextFeaturedChallengeChange < now)
                {
                    nextFeaturedChallengeChange = now.AddHours(8);
                }

                _logger.LogDebug("Caching {Count} featured challenge groups until {Expiration} ({TimeSpan})",
                    featured?.Count() ?? 0,
                    nextFeaturedChallengeChange,
                    nextFeaturedChallengeChange - now);

                await _cache.SaveToCacheAsync(cacheKey,
                     featured,
                     nextFeaturedChallengeChange.Value - now);
            }

            return featured;
        }

        public async Task<FeaturedChallengeGroup> GetFeaturedGroupByIdAsync(int id)
        {
            var featuredGroup = await _featuredChallengeGroupRepository.GetByIdAsync(id);

            var text = await _featuredChallengeGroupRepository
                .GetTextByFeaturedGroupAndLanguageAsync(featuredGroup.Id,
                    await _languageService.GetDefaultLanguageIdAsync());

            text.ImagePath = GetFeaturedUrlPath(text.Filename);

            featuredGroup.FeaturedGroupText = text;

            return featuredGroup;
        }

        public async Task<ICollectionWithCount<FeaturedChallengeGroup>>
            GetPaginatedFeaturedGroupListAsync(BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            VerifyPermission(Permission.ViewAllChallenges);

            filter.SiteId = GetCurrentSiteId();

            return await _featuredChallengeGroupRepository.PageAsync(filter);
        }

        public async Task RemoveFeaturedGroupAsync(int featuredGroupId)
        {
            VerifyPermission(Permission.ManageFeaturedChallengeGroups);

            var texts = await _featuredChallengeGroupRepository
                .GetTextsForFeaturedGroupAsync(featuredGroupId);

            _featuredChallengeGroupRepository.RemoveFeaturedGroupTexts(featuredGroupId, null);
            await _featuredChallengeGroupRepository.SaveAsync();

            foreach (var text in texts)
            {
                var imagePath = GetFeaturedFilePath(text.Filename);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            await _featuredChallengeGroupRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId),
                featuredGroupId);

            await ClearFeaturedChallengeGroupsCacheAsync();
        }

        public async Task ReplaceFeaturedImageAsync(int featuredGroupId,
            string filename,
            byte[] imageBytes)
        {
            VerifyPermission(Permission.ManageFeaturedChallengeGroups);

            var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();

            var featuredGroupText = await _featuredChallengeGroupRepository
                .GetTextByFeaturedGroupAndLanguageAsync(featuredGroupId, defaultLanguageId);

            if (featuredGroupText == null)
            {
                return;
            }

            var oldImagePath = GetFeaturedFilePath(featuredGroupText.Filename);

            featuredGroupText.Filename = (await HandleFeaturedImage(filename, imageBytes))?.Trim();

            if (File.Exists(oldImagePath))
            {
                File.Delete(oldImagePath);
            }

            await _featuredChallengeGroupRepository.UpdateTextAsync(featuredGroupText,
                featuredGroupId,
                defaultLanguageId);

            await _featuredChallengeGroupRepository.SaveAsync();
            await ClearFeaturedChallengeGroupsCacheAsync();
        }

        public async Task UpdateFeaturedGroupSortAsync(int id, bool increase)
        {
            VerifyPermission(Permission.ManageFeaturedChallengeGroups);

            var featuredGroup = await _featuredChallengeGroupRepository.GetByIdAsync(id);

            if (increase)
            {
                var moveGroup = await _featuredChallengeGroupRepository
                    .GetNextInOrderAsync(GetCurrentSiteId(), featuredGroup.SortOrder, increase);
                if (moveGroup != null)
                {
                    moveGroup.SortOrder--;
                    featuredGroup.SortOrder++;
                    await _featuredChallengeGroupRepository.UpdateSaveNoAuditAsync(moveGroup);
                    await _featuredChallengeGroupRepository.UpdateSaveNoAuditAsync(featuredGroup);
                }
            }
            else
            {
                var moveGroup = await _featuredChallengeGroupRepository
                    .GetNextInOrderAsync(GetCurrentSiteId(), featuredGroup.SortOrder, increase);
                if (moveGroup != null)
                {
                    moveGroup.SortOrder++;
                    featuredGroup.SortOrder--;
                    await _featuredChallengeGroupRepository.UpdateSaveNoAuditAsync(moveGroup);
                    await _featuredChallengeGroupRepository.UpdateSaveNoAuditAsync(featuredGroup);
                }
            }

            await ClearFeaturedChallengeGroupsCacheAsync();
        }

        private async Task ClearFeaturedChallengeGroupsCacheAsync()
        {
            var languages = await _languageService.GetActiveAsync();
            foreach (var language in languages)
            {
                await _cache.RemoveAsync(string
                    .Format(CultureInfo.InvariantCulture,
                    CacheKey.FeaturedChallengeGroups,
                    language.Id));
            }
        }

        private string GetFeaturedFilePath(string filename)
        {
            string contentDir = _pathResolver.ResolveContentFilePath(Path.Combine(
                $"site{GetCurrentSiteId()}",
                FeaturedFilesPath));

            Directory.CreateDirectory(contentDir);

            return Path.Combine(contentDir, filename);
        }

        private string GetFeaturedUrlPath(string filename)
        {
            return _pathResolver
                .ResolveContentPath($"site{GetCurrentSiteId()}/{FeaturedFilesPath}/{filename}");
        }

        private async Task<string> HandleFeaturedImage(string filename, byte[] imageBytes)
        {
            var fullPath = GetFeaturedFilePath(filename);
            int dupeCheck = 1;
            while (File.Exists(fullPath))
            {
                filename = Path.GetFileNameWithoutExtension(filename)
                        + $"-{dupeCheck++}"
                        + Path.GetExtension(filename);
                fullPath = GetFeaturedFilePath(filename);
            }

            await File.WriteAllBytesAsync(fullPath, imageBytes);

            return filename;
        }

        #endregion Featured Challenge Groups methods
    }
}
