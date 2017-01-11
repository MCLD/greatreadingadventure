using System.Linq;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using GRA.Domain.Model;

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
            PageAllAsync(int siteId, int skip, int take, string search = null)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await DbSet
                    .AsNoTracking()
                    .Where(_ => _.IsDeleted == false
                        && _.SiteId == siteId)
                    .OrderBy(_ => _.Name)
                    .ThenBy(_ => _.Id)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<Challenge>()
                    .ToListAsync();
            }
            else
            {
                return await DbSet
                    .AsNoTracking()
                    .Include(_ => _.Tasks)
                    .Where(_ => _.IsDeleted == false
                        && _.SiteId == siteId
                        && (_.Name.Contains(search)
                        || _.Description.Contains(search)
                        || _.Tasks.Any(_t => _t.Title.Contains(search))
                        || _.Tasks.Any(_t => _t.Author.Contains(search))))
                    .OrderBy(_ => _.Name)
                    .ThenBy(_ => _.Id)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<Challenge>()
                    .ToListAsync();
            }
        }

        public async Task<int> GetChallengeCountAsync(int siteId, string search = null)
        {
            if (string.IsNullOrEmpty(search))
            {
                var challenges = await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.SiteId == siteId)
                .ToListAsync();
                return challenges.Count();
            }
            else
            {
                var challenges = await DbSet
                .AsNoTracking()
                .Include(_ => _.Tasks)
                .Where(_ => _.IsDeleted == false
                    && _.SiteId == siteId
                        && (_.Name.Contains(search)
                        || _.Description.Contains(search)
                        || _.Tasks.Any(_t => _t.Title.Contains(search))
                        || _.Tasks.Any(_t => _t.Author.Contains(search))))
                .ToListAsync();
                return challenges.Count();
            }

        }

        public async Task<Challenge> GetByIdAsync(int id, int? userId = null)
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

                if (userId != null)
                {
                    // determine if challenge is completed
                    var challengeStatus = await _context.UserLogs
                        .AsNoTracking()
                        .Where(_ => _.UserId == userId && _.ChallengeId == id)
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
            int userLogId,
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
            if (userChallengeTask == null || userChallengeTask.UserLogId == null)
            {
                return null;
            }
            else
            {
                return new ActivityLogResult
                {
                    BookId = userChallengeTask.BookId,
                    UserLogId = (int)userChallengeTask.UserLogId
                };
            }
        }

        public async Task<IEnumerable<int>>
            PageIdsAsync(int siteId, int skip, int take, string search = null)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await DbSet
                    .AsNoTracking()
                    .Where(_ => _.IsDeleted == false
                        && _.SiteId == siteId)
                    .OrderBy(_ => _.Name)
                    .ThenBy(_ => _.Id)
                    .Skip(skip)
                    .Take(take)
                    .Select(_ => _.Id)
                    .ToListAsync();
            }
            else
            {
                return await DbSet
                    .AsNoTracking()
                    .Include(_ => _.Tasks)
                    .Where(_ => _.IsDeleted == false
                        && _.SiteId == siteId
                        && (_.Name.Contains(search)
                        || _.Description.Contains(search)
                        || _.Tasks.Any(_t => _t.Title.Contains(search))
                        || _.Tasks.Any(_t => _t.Author.Contains(search))))
                    .OrderBy(_ => _.Name)
                    .ThenBy(_ => _.Id)
                    .Skip(skip)
                    .Take(take)
                    .Select(_ => _.Id)
                    .ToListAsync();
            }
        }

    }
}