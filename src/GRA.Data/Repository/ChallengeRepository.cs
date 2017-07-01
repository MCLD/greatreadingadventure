using System.Linq;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using GRA.Domain.Model;
using GRA.Domain.Repository.Extensions;
using GRA.Domain.Model.Filters;

namespace GRA.Data.Repository
{
    public class ChallengeRepository
        : AuditingRepository<Model.Challenge, Challenge>, IChallengeRepository
    {
        public ChallengeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<ChallengeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<Challenge>>
            PageAllAsync(BaseFilter filter)
        {
            var challengeList = await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ThenBy(_ => _.Id)
                .ApplyPagination(filter)
                .ProjectTo<Challenge>()
                .ToListAsync();

            foreach (var challenge in challengeList)
            {
                challenge.HasDependents = await HasDependentsAsync(challenge.Id);
            }

            return challengeList;
        }

        public async Task<int> GetChallengeCountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter).CountAsync();
        }

        private IQueryable<Data.Model.Challenge> ApplyFilters(BaseFilter filter)
        {
            var challenges = _context.Challenges.AsNoTracking()
                    .Where(_ => _.IsDeleted == false
                        && _.SiteId == filter.SiteId);

            if (filter.SystemIds?.Any() == true)
            {
                challenges = challenges.Where(_ => filter.SystemIds.Contains(_.RelatedSystemId));
            }
            if (filter.BranchIds?.Any() == true)
            {
                challenges = challenges.Where(_ => filter.BranchIds.Contains(_.RelatedBranchId));
            }
            if (filter.ProgramIds?.Any() == true)
            {
                challenges = challenges
                    .Where(_ => filter.ProgramIds.Any(p => p == _.LimitToProgramId));
            }
            if (filter.UserIds?.Any() == true)
            {
                challenges = challenges.Where(_ => filter.UserIds.Contains(_.CreatedBy));
            }
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                challenges = challenges.Where(_ => _.Name.Contains(filter.Search)
                        || _.Description.Contains(filter.Search)
                        || _.Tasks.Any(_t => _t.Title.Contains(filter.Search))
                        || _.Tasks.Any(_t => _t.Author.Contains(filter.Search)));
            }
            if (filter.IsActive.HasValue)
            {
                challenges = challenges.Where(_ => _.IsActive == filter.IsActive.Value);
            }

            return challenges;
        }

        public new async Task<Challenge> GetByIdAsync(int id)
        {
            var challenge = _mapper.Map<Model.Challenge, Challenge>(await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleOrDefaultAsync());

            if (challenge != null)
            {
                challenge.Tasks = await _context.ChallengeTasks
                .AsNoTracking()
                .Where(_ => _.ChallengeId == id)
                .OrderBy(_ => _.Position)
                .ProjectTo<ChallengeTask>()
                .ToListAsync();

                await GetChallengeTasksTypeAsync(challenge.Tasks);

                var challengeTaskTypes = await _context.ChallengeTaskTypes
                    .AsNoTracking()
                    .ToDictionaryAsync(_ => _.Id);

                foreach (var task in challenge.Tasks)
                {
                    task.ActivityCount = challengeTaskTypes[task.ChallengeTaskTypeId].ActivityCount;
                    task.PointTranslationId = challengeTaskTypes[task.ChallengeTaskTypeId].PointTranslationId;
                }
            }
            return challenge;
        }

        public async Task<Challenge> GetActiveByIdAsync(int id, int? userId = null)
        {
            var challenge = _mapper.Map<Model.Challenge, Challenge>(await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.Id == id && _.IsActive == true)
                .SingleOrDefaultAsync());

            if (challenge != null)
            {
                challenge.Tasks = await _context.ChallengeTasks
                .AsNoTracking()
                .Where(_ => _.ChallengeId == id)
                .OrderBy(_ => _.Position)
                .ProjectTo<ChallengeTask>()
                .ToListAsync();

                await GetChallengeTasksTypeAsync(challenge.Tasks);

                var challengeTaskTypes = await _context.ChallengeTaskTypes
                    .AsNoTracking()
                    .ToDictionaryAsync(_ => _.Id);

                foreach (var task in challenge.Tasks)
                {
                    task.ActivityCount = challengeTaskTypes[task.ChallengeTaskTypeId].ActivityCount;
                    task.PointTranslationId = challengeTaskTypes[task.ChallengeTaskTypeId].PointTranslationId;
                }

                if (userId != null)
                {
                    // determine if challenge is completed
                    var challengeStatus = await _context.UserLogs
                        .AsNoTracking()
                        .Where(_ => _.UserId == userId && _.ChallengeId == id && _.IsDeleted == false)
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
                            .Where(_ => _.Id == userChallengeTask.ChallengeTaskId)
                            .SingleOrDefault();
                        if (task != null && userChallengeTask.IsCompleted)
                        {
                            task.IsCompleted = true;
                            task.CompletedAt = userChallengeTask.CreatedAt;
                        }
                    }
                }
            }
            return challenge;
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await _context.Challenges
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }

        public async Task<IEnumerable<ChallengeTask>>
            GetChallengeTasksAsync(int challengeId, int? userId = null)
        {
            var tasks = await _context.ChallengeTasks
                .AsNoTracking()
                .Where(_ => _.ChallengeId == challengeId)
                .OrderBy(_ => _.Position)
                .ProjectTo<ChallengeTask>()
                .ToListAsync();

            return await GetChallengeTasksTypeAsync(tasks);
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
                        IsCompleted = updatedChallengeTask.IsCompleted ?? false
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

        public async Task<DataWithCount<IEnumerable<int>>>
            PageIdsAsync(int siteId, int skip, int take, int userId, string search = null)
        {
            var user = await _context.Users.FindAsync(userId);

            if (string.IsNullOrEmpty(search))
            {
                var challenges = DbSet
                    .AsNoTracking()
                    .Where(_ => _.IsDeleted == false
                        && _.SiteId == siteId
                        && _.IsActive == true
                        && (_.LimitToSystemId == null || _.LimitToSystemId == user.SystemId)
                        && (_.LimitToBranchId == null || _.LimitToBranchId == user.BranchId)
                        && (_.LimitToProgramId == null || _.LimitToProgramId == user.ProgramId))
                    .OrderBy(_ => _.Name)
                    .ThenBy(_ => _.Id);

                return new DataWithCount<IEnumerable<int>>()
                {
                    Data = await challenges
                    .Skip(skip)
                    .Take(take)
                    .Select(_ => _.Id)
                    .ToListAsync(),
                    Count = await challenges.CountAsync()
                };
            }
            else
            {
                var challenges = DbSet
                    .AsNoTracking()
                    .Where(_ => _.IsDeleted == false
                        && _.SiteId == siteId
                        && _.IsActive == true
                        && (_.LimitToSystemId == null || _.LimitToSystemId == user.SystemId)
                        && (_.LimitToBranchId == null || _.LimitToBranchId == user.BranchId)
                        && (_.LimitToProgramId == null || _.LimitToProgramId == user.ProgramId)
                        && (_.Name.Contains(search)
                        || _.Description.Contains(search)
                        || _.Tasks.Any(_t => _t.Title.Contains(search))
                        || _.Tasks.Any(_t => _t.Author.Contains(search))))
                    .OrderBy(_ => _.Name)
                    .ThenBy(_ => _.Id);
                return new DataWithCount<IEnumerable<int>>()
                {
                    Data = await challenges
                    .Skip(skip)
                    .Take(take)
                    .Select(_ => _.Id)
                    .ToListAsync(),
                    Count = await challenges.CountAsync()
                };
            }
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

        public async Task<bool> HasDependentsAsync(int challengeId)
        {
            return await _context.TriggerChallenges
                .AsNoTracking()
                .Where(_ => _.ChallengeId == challengeId)
                .AnyAsync();
        }
    }
}