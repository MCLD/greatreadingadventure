using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Data.Extensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class ChallengeRepository
        : AuditingRepository<Model.Challenge, Challenge>, IChallengeRepository
    {
        public ChallengeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<ChallengeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public override async Task<Challenge> AddSaveAsync(int userId, Challenge challenge)
        {
            var newChallenge = await base.AddSaveAsync(userId, challenge);

            if (challenge.CategoryIds?.Count > 0)
            {
                var time = _dateTimeProvider.Now;
                var challengeCategoryList = new List<Model.ChallengeCategory>();
                foreach (var categoryId in challenge.CategoryIds)
                {
                    challengeCategoryList.Add(new Model.ChallengeCategory()
                    {
                        CategoryId = categoryId,
                        ChallengeId = newChallenge.Id,
                        CreatedAt = time,
                        CreatedBy = userId
                    });
                }
                await _context.ChallengeCategories.AddRangeAsync(challengeCategoryList);
                await _context.SaveChangesAsync();
            }

            return newChallenge;
        }

        public async Task<Challenge> GetActiveByIdAsync(int id, int? userId = null)
        {
            var challenge = _mapper.Map<Model.Challenge, Challenge>(await DbSet
                .AsNoTracking()
                .Include(_ => _.ChallengeCategories)
                    .ThenInclude(_ => _.Category)
                .Where(_ => !_.IsDeleted && _.Id == id && _.IsActive)
                .SingleOrDefaultAsync());

            if (challenge != null)
            {
                challenge.Tasks = await _context.ChallengeTasks
                .AsNoTracking()
                .Where(_ => _.ChallengeId == id)
                .OrderBy(_ => _.Position)
                .ProjectToType<ChallengeTask>()
                .ToListAsync();

                await GetChallengeTasksTypeAsync(challenge.Tasks);

                var challengeTaskTypes = await _context.ChallengeTaskTypes
                    .AsNoTracking()
                    .ToDictionaryAsync(_ => _.Id);

                foreach (var task in challenge.Tasks)
                {
                    task.ActivityCount = challengeTaskTypes[task.ChallengeTaskTypeId].ActivityCount;
                    task.PointTranslationId
                        = challengeTaskTypes[task.ChallengeTaskTypeId].PointTranslationId;
                }

                if (userId != null)
                {
                    // determine if challenge is completed
                    var challengeStatus = await _context.UserLogs
                        .AsNoTracking()
                        .Where(_ => _.UserId == userId && _.ChallengeId == id && !_.IsDeleted)
                        .SingleOrDefaultAsync();
                    if (challengeStatus != null)
                    {
                        challenge.IsCompleted = true;
                        challenge.CompletedAt = challengeStatus.CreatedAt;
                    }

                    var userChallengeTasks = await _context.UserChallengeTasks
                        .AsNoTracking()
                        .Where(_ => _.UserId == (int)userId)
                        .ToListAsync();

                    foreach (var userChallengeTask in userChallengeTasks)
                    {
                        var task = challenge.Tasks
                            .SingleOrDefault(_ => _.Id == userChallengeTask.ChallengeTaskId);
                        if (task != null && userChallengeTask.IsCompleted)
                        {
                            task.IsCompleted = true;
                            task.CompletedAt = userChallengeTask.CreatedAt;
                        }
                    }

                    challenge.IsFavorited = await _context.UserFavoriteChallenges
                        .AsNoTracking()
                        .Where(_ => _.ChallengeId == id && _.UserId == userId)
                        .AnyAsync();
                }
            }
            return challenge;
        }

        public new async Task<Challenge> GetByIdAsync(int id)
        {
            var challenge = await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.Id == id)
                .ProjectToType<Challenge>()
                .SingleOrDefaultAsync();

            if (challenge != null)
            {
                challenge.Tasks = await _context.ChallengeTasks
                .AsNoTracking()
                .Where(_ => _.ChallengeId == id)
                .OrderBy(_ => _.Position)
                .ProjectToType<ChallengeTask>()
                .ToListAsync();

                await GetChallengeTasksTypeAsync(challenge.Tasks);

                var challengeTaskTypes = await _context.ChallengeTaskTypes
                    .AsNoTracking()
                    .ToDictionaryAsync(_ => _.Id);

                foreach (var task in challenge.Tasks)
                {
                    task.ActivityCount = challengeTaskTypes[task.ChallengeTaskTypeId].ActivityCount;
                    task.PointTranslationId
                        = challengeTaskTypes[task.ChallengeTaskTypeId].PointTranslationId;
                }

                challenge.Categories = await _context.ChallengeCategories
                    .AsNoTracking()
                    .Where(_ => _.ChallengeId == id)
                    .Select(_ => _.Category)
                    .ProjectToType<Category>()
                    .ToListAsync();
            }
            return challenge;
        }

        public async Task<List<Challenge>> GetByIdsAsync(int siteId, IEnumerable<int> ids,
            bool ActiveOnly = false)
        {
            var challenges = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && !_.IsDeleted && ids.Contains(_.Id));

            if (ActiveOnly)
            {
                challenges = challenges.Where(_ => _.IsActive);
            }

            return await challenges
                .OrderBy(_ => _.Name)
                .Distinct()
                .ProjectToType<Challenge>()
                .ToListAsync();
        }

        public async Task<int> GetChallengeCountAsync(ChallengeFilter filter)
        {
            return await ApplyFilters(filter).CountAsync();
        }

        public async Task<IEnumerable<ChallengeTask>>
            GetChallengeTasksAsync(int challengeId, int? userId = null)
        {
            var tasks = await _context.ChallengeTasks
                .AsNoTracking()
                .Where(_ => _.ChallengeId == challengeId)
                .OrderBy(_ => _.Position)
                .ProjectToType<ChallengeTask>()
                .ToListAsync();

            return await GetChallengeTasksTypeAsync(tasks);
        }

        public async Task<IEnumerable<string>> GetNamesAsync(IEnumerable<int> challengeIds)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => challengeIds.Contains(_.Id))
                .Select(_ => _.Name)
                .ToListAsync();
        }

        public async Task<ActivityLogResult> GetUserChallengeTaskResultAsync(int userId,
            int challengeTaskId)
        {
            var userChallengeTask = await _context.UserChallengeTasks
                .AsNoTracking()
                .Where(_ => _.UserId == userId && _.ChallengeTaskId == challengeTaskId)
                .SingleOrDefaultAsync();
            if (userChallengeTask == null
                || (userChallengeTask.UserLogId == null && userChallengeTask.BookId == null))
            {
                return null;
            }
            else
            {
                return new ActivityLogResult
                {
                    BookId = userChallengeTask.BookId,
                    UserLogId = userChallengeTask.UserLogId
                };
            }
        }

        public async Task<IEnumerable<int>> GetUserFavoriteChallenges(int userId,
            IEnumerable<int> challengeIds = null)
        {
            var favoriteChallenges = _context.UserFavoriteChallenges
                .AsNoTracking()
                .Where(_ => _.UserId == userId);

            if (challengeIds?.Count() > 0)
            {
                favoriteChallenges = favoriteChallenges
                    .Where(_ => challengeIds.Contains(_.ChallengeId));
            }

            return await favoriteChallenges
                .Select(_ => _.ChallengeId)
                .ToListAsync();
        }

        public async Task<bool> HasDependentsAsync(int challengeId)
        {
            return await _context.TriggerChallenges
                .AsNoTracking()
                .Where(_ => _.ChallengeId == challengeId)
                .AnyAsync();
        }

        public async Task IncrementPopularity(int challengeId)
        {
            try
            {
                await _context
                    .Database
                    .ExecuteSqlInterpolatedAsync($"UPDATE [Challenges] SET [EstimatedPopularity] = [EstimatedPopularity] + 1 WHERE [Id] = {challengeId}");
            }
            catch (Exception ex)
            {
                throw new GraException("Unable to increment popularity for challenge id {challengeId}", ex);
            }
        }

        public async Task<ICollection<Challenge>> PageAllAsync(ChallengeFilter filter)
        {
            var challengeList = await ApplyFilters(filter)
                .ApplyOrdering(filter)
                .ApplyPagination(filter)
                .ProjectToType<Challenge>()
                .ToListAsync();

            foreach (var challenge in challengeList)
            {
                challenge.HasDependents = await HasDependentsAsync(challenge.Id);
            }

            return challengeList;
        }

        public async Task<DataWithCount<IEnumerable<int>>> PageIdsAsync(ChallengeFilter filter,
            int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            var challengeList = ApplyFilters(filter)
                .Where(_ => (_.LimitToSystemId == null || _.LimitToSystemId == user.SystemId)
                        && (_.LimitToBranchId == null || _.LimitToBranchId == user.BranchId));

            if (filter.IsCompleted.HasValue)
            {
                var userCompletedChallenges = _context.UserLogs
                    .AsNoTracking()
                    .Where(_ => _.UserId == userId && _.ChallengeId.HasValue && !_.IsDeleted)
                    .Select(_ => _.ChallengeId);

                challengeList = challengeList
                    .Where(_ => userCompletedChallenges.Contains(_.Id) == filter.IsCompleted.Value);
            }

            var data = await challengeList
                .ApplyOrdering(filter)
                .ApplyPagination(filter)
                .Select(_ => _.Id)
                .ToListAsync();

            return new DataWithCount<IEnumerable<int>>()
            {
                Data = data,
                Count = await challengeList.CountAsync()
            };
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await _context.Challenges
                .Include(_ => _.ChallengeGroupChallenges)
                .Where(_ => !_.IsDeleted && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            _context.ChallengeGroupChallenges.RemoveRange(entity.ChallengeGroupChallenges);
            await base.SaveAsync();
        }

        public async Task SetValidationAsync(int userId, int challengeId, bool valid)
        {
            var challenge = await DbSet.Where(_ => _.Id == challengeId).SingleOrDefaultAsync();
            if (challenge != null)
            {
                challenge.IsValid = valid;
                if (!valid)
                {
                    challenge.IsActive = false;
                }
                DbSet.Update(challenge);
                await base.SaveAsync();
            }
        }

        public async Task<Challenge> UpdateSaveAsync(int userId, Challenge challenge,
            List<int> categoriesToAdd, List<int> categoriesToRemove)
        {
            await base.UpdateAsync(userId, challenge);

            if (categoriesToAdd.Count > 0)
            {
                var time = _dateTimeProvider.Now;
                var challengeCategoryList = new List<Model.ChallengeCategory>();
                foreach (var categoryId in categoriesToAdd)
                {
                    challengeCategoryList.Add(new Model.ChallengeCategory()
                    {
                        CategoryId = categoryId,
                        ChallengeId = challenge.Id,
                        CreatedAt = time,
                        CreatedBy = userId
                    });
                }
                await _context.ChallengeCategories.AddRangeAsync(challengeCategoryList);
            }
            if (categoriesToRemove.Count > 0)
            {
                var removeList = _context.ChallengeCategories
                    .Where(_ => _.ChallengeId == challenge.Id && categoriesToRemove
                    .Contains(_.CategoryId));
                _context.ChallengeCategories.RemoveRange(removeList);
            }

            await _context.SaveChangesAsync();

            return challenge;
        }

        public async Task UpdateUserChallengeTaskAsync(int userId,
            int challengeTaskId,
            int? userLogId,
            int? bookId)
        {
            var userChallengeTask = await _context.UserChallengeTasks
                .Where(_ => _.UserId == userId && _.ChallengeTaskId == challengeTaskId)
                .SingleOrDefaultAsync();
            if (userChallengeTask == null)
            {
                _logger.LogError("Unable to update UserChallengeTask with UserLogId and BookId");
            }
            else
            {
                userChallengeTask.UserLogId = userLogId;
                userChallengeTask.BookId = bookId;
                _context.UserChallengeTasks.Update(userChallengeTask);
                await SaveAsync();
            }
        }

        public async Task<IEnumerable<ChallengeTaskUpdateStatus>>
            UpdateUserChallengeTasksAsync(int userId, IEnumerable<ChallengeTask> challengeTasks)
        {
            var result = new List<ChallengeTaskUpdateStatus>();
            foreach (var updatedChallengeTask in challengeTasks)
            {
                var dataSourceChallengeTask = await _context.ChallengeTasks
                    .AsNoTracking()
                    .Where(_ => _.Id == updatedChallengeTask.Id)
                    .SingleAsync();

                var status = new ChallengeTaskUpdateStatus
                {
                    ChallengeTask = _mapper.Map<ChallengeTask>(dataSourceChallengeTask),
                    IsComplete = updatedChallengeTask.IsCompleted ?? false
                };
                var savedChallengeTask = await _context
                    .UserChallengeTasks.Where(_ => _.UserId == userId
                     && _.ChallengeTaskId == updatedChallengeTask.Id)
                     .SingleOrDefaultAsync();

                if (savedChallengeTask == null)
                {
                    status.WasComplete = false;
                    _context.UserChallengeTasks.Add(new Model.UserChallengeTask
                    {
                        ChallengeTaskId = updatedChallengeTask.Id,
                        UserId = userId,
                        IsCompleted = updatedChallengeTask.IsCompleted ?? false,
                        CreatedAt = _dateTimeProvider.Now
                    });
                }
                else
                {
                    status.WasComplete = savedChallengeTask.IsCompleted;
                    savedChallengeTask.IsCompleted = updatedChallengeTask.IsCompleted ?? false;
                    _context.UserChallengeTasks.Update(savedChallengeTask);
                }
                result.Add(status);
            }
            await SaveAsync();
            return result;
        }

        public async Task UpdateUserFavoritesAsync(int authUserId, int userId,
            IEnumerable<int> favoritesToAdd, IEnumerable<int> favoritesToRemove)
        {
            if (favoritesToAdd.Any())
            {
                var time = _dateTimeProvider.Now;
                var userFavoriteList = new List<Model.UserFavoriteChallenge>();
                foreach (var challengeId in favoritesToAdd)
                {
                    userFavoriteList.Add(new Model.UserFavoriteChallenge()
                    {
                        UserId = userId,
                        ChallengeId = challengeId,
                        CreatedAt = time,
                        CreatedBy = authUserId
                    });
                }
                await _context.UserFavoriteChallenges.AddRangeAsync(userFavoriteList);
            }
            if (favoritesToRemove.Any())
            {
                var removeList = _context.UserFavoriteChallenges
                    .Where(_ => _.UserId == userId && favoritesToRemove
                    .Contains(_.ChallengeId));
                _context.UserFavoriteChallenges.RemoveRange(removeList);
            }
            await SaveAsync();
        }

        public async Task<IEnumerable<int>> ValidateChallengeIdsAsync(int siteId,
            IEnumerable<int> challengeIds)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && challengeIds.Contains(_.Id))
                .Select(_ => _.Id)
                .ToListAsync();
        }

        private IQueryable<Data.Model.Challenge> ApplyFilters(ChallengeFilter filter)
        {
            var challenges = _context.Challenges.AsNoTracking()
                    .Where(_ => !_.IsDeleted
                        && _.SiteId == filter.SiteId);

            if (filter.SystemIds?.Count > 0)
            {
                challenges = challenges.Where(_ => filter.SystemIds.Contains(_.RelatedSystemId));
            }
            if (filter.BranchIds?.Count > 0)
            {
                challenges = challenges.Where(_ => filter.BranchIds.Contains(_.RelatedBranchId));
            }
            if (filter.ProgramIds?.Count > 0)
            {
                challenges = challenges
                    .Where(_ => filter.ProgramIds.Any(p => p == _.AssociatedProgramId));
            }
            if (filter.UserIds?.Count > 0)
            {
                challenges = challenges.Where(_ => filter.UserIds.Contains(_.CreatedBy));
            }

            if (filter.ChallengeIds?.Count > 0)
            {
                challenges = challenges.Where(_ => !filter.ChallengeIds.Contains(_.Id));
            }

            if (filter.CategoryIds?.Count > 0)
            {
                challenges = challenges
                    .Where(_ => _.ChallengeCategories
                        .Select(c => c.CategoryId)
                        .Any(c => filter.CategoryIds.Contains(c)));
            }

            if (filter.Favorites == true && filter.FavoritesUserId.HasValue)
            {
                var userFavoriteChallenges = _context.UserFavoriteChallenges
                    .AsNoTracking()
                    .Where(_ => _.UserId == filter.FavoritesUserId);

                challenges = from challengeList in challenges
                             join userFavorites in userFavoriteChallenges
                             on challengeList.Id equals userFavorites.ChallengeId
                             select challengeList;
            }

            if (filter.GroupId.HasValue)
            {
                var challengeGroup = _context.ChallengeGroups
                    .AsNoTracking()
                    .Where(_ => _.Id == filter.GroupId)
                    .SelectMany(_ => _.ChallengeGroupChallenges)
                    .Select(_ => _.ChallengeId);

                challenges = challenges.Where(_ => challengeGroup.Contains(_.Id));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                challenges = challenges
                    .Where(_ => _.Name.Contains(filter.Search)
                        || _.Description.Contains(filter.Search)
                        || _.Tasks.Any(_t => _t.Title.Contains(filter.Search))
                        || _.Tasks.Any(_t => _t.Author.Contains(filter.Search))
                        || _.ChallengeCategories.Any(c => c.Category.Name.Contains(filter.Search)));
            }

            if (filter.IsActive.HasValue)
            {
                challenges = challenges.Where(_ => _.IsActive == filter.IsActive.Value);
            }

            return challenges;
        }

        private async Task<IEnumerable<ChallengeTask>>
            GetChallengeTasksTypeAsync(IEnumerable<ChallengeTask> tasks)
        {
            var challengeTaskTypes =
                await _context.ChallengeTaskTypes
                .AsNoTracking()
                .ToDictionaryAsync(_ => _.Id);

            foreach (var task in tasks)
            {
                task.ChallengeTaskType = (ChallengeTaskType)
                    Enum.Parse(typeof(ChallengeTaskType),
                    challengeTaskTypes[task.ChallengeTaskTypeId].Name);
            }
            return tasks;
        }
    }
}
