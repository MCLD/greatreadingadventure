using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class ChallengeTaskRepository
        : AuditingRepository<Model.ChallengeTask, Domain.Model.ChallengeTask>, IChallengeTaskRepository
    {
        public ChallengeTaskRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<ChallengeTaskRepository> logger) : base(repositoryFacade, logger)
        { }

        public override async Task<Domain.Model.ChallengeTask> GetByIdAsync(int id)
        {
            var task = _mapper.Map<Model.ChallengeTask, ChallengeTask>(await DbSet
               .AsNoTracking()
               .Where(_ => _.Id == id)
               .SingleAsync());

            var challengeTaskTypes =
                await _context.ChallengeTaskTypes
                .AsNoTracking()
                .ToDictionaryAsync(_ => _.Id);

            task.ChallengeTaskType = (ChallengeTaskType)
                   Enum.Parse(typeof(ChallengeTaskType),
                   challengeTaskTypes[task.ChallengeTaskTypeId].Name);

            return task;
        }

        public override async Task AddAsync(int userId, ChallengeTask domainEntity)
        {
            await LookUpChallengeTaskTypeAsync(domainEntity);
            domainEntity.Position = DbSet
                .Where(_ => _.ChallengeId == domainEntity.ChallengeId)
                .Max(_ => _.Position) + 1;
            await base.AddAsync(userId, domainEntity);
        }

        public override async Task<ChallengeTask> AddSaveAsync(int userId, ChallengeTask domainEntity)
        {
            await LookUpChallengeTaskTypeAsync(domainEntity);
            domainEntity.Position = DbSet
                .Where(_ => _.ChallengeId == domainEntity.ChallengeId)
                .Max(_ => _.Position) + 1;
            return await base.AddSaveAsync(userId, domainEntity);
        }
        public override async Task UpdateAsync(int userId, ChallengeTask domainEntity)
        {
            await LookUpChallengeTaskTypeAsync(domainEntity);
            await base.UpdateAsync(userId, domainEntity);
        }

        public override async Task<ChallengeTask> UpdateSaveAsync(int userId, ChallengeTask domainEntity)
        {
            await LookUpChallengeTaskTypeAsync(domainEntity);
            return await base.UpdateSaveAsync(userId, domainEntity);
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = DbSet.Find(id);
            if (entity == null)
            {
                throw new Exception($"ChallengeTask id {id} could not be found.");
            }
            var challengeId = entity.ChallengeId;
            DbSet.Remove(entity);
            await DbSet.Where(_ => _.ChallengeId == challengeId && _.Position > entity.Position)
                .ForEachAsync(_ => _.Position--);

            await SaveAsync();
        }

        public async Task DecreasePositionAsync(int taskId)
        {
            var task = DbSet.Find(taskId);
            if (task == null)
            {
                throw new Exception($"Task {taskId} could not be found");
            }
            if (task.Position == 1)
            {
                throw new Exception($"Task {taskId} is already in the first position for this challenge.");
            }
            var previousTask = await DbSet
                .Where(_ => _.ChallengeId == task.ChallengeId && _.Position == task.Position - 1)
                .SingleOrDefaultAsync();
            previousTask.Position++;
            task.Position--;
            await SaveAsync();
        }

        public async Task IncreasePositionAsync(int taskId)
        {
            var task = DbSet.Find(taskId);
            if (task == null)
            {
                throw new Exception($"Task {taskId} could not be found");
            }
            var nextTask = await DbSet
                .Where(_ => _.ChallengeId == task.ChallengeId && _.Position == task.Position + 1)
                .SingleOrDefaultAsync();
            if (nextTask == null)
            {
                throw new Exception($"Task {taskId} is already in the last position for this challenge.");
            }
            nextTask.Position--;
            task.Position++;
            await SaveAsync();
        }

        public async Task AddChallengeTaskTypeAsync(int userId,
            string name,
            int? activityCount = null,
            int? pointTranslationId = null)
        {
            await _context.ChallengeTaskTypes.AddAsync(new Model.ChallengeTaskType
            {
                Name = name,
                CreatedBy = userId,
                CreatedAt = _dateTimeProvider.Now,
                ActivityCount = activityCount,
                PointTranslationId = pointTranslationId
            });
        }

        public async Task<bool> UserHasTaskAsync(int id)
        {
            return await _context.UserChallengeTasks.AsNoTracking()
                .Where(_ => _.ChallengeTaskId == id)
                .AnyAsync();
        }

        public async Task UnsetUserChallengeTasksAsync(int userId, int challengeId)
        {
            await _context.UserChallengeTasks
                .Where(_ => _.UserId == userId && _.ChallengeTask.ChallengeId == challengeId)
                .ForEachAsync(_ => _.IsCompleted = false);

            await _context.SaveChangesAsync();
        }

        private async Task LookUpChallengeTaskTypeAsync(ChallengeTask task)
        {
            string taskTypeName = task.ChallengeTaskType.ToString();
            task.ChallengeTaskTypeId = await _context.ChallengeTaskTypes
                .AsNoTracking()
                .Where(_ => _.Name == taskTypeName)
                .Select(_ => _.Id)
                .SingleOrDefaultAsync();
        }
    }
}

