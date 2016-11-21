using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace GRA.Data.Repository
{
    public class ChallengeTaskRepository
        : AuditingRepository<Model.ChallengeTask, Domain.Model.ChallengeTask>, IChallengeTaskRepository
    {
        public ChallengeTaskRepository(Context context,
            ILogger<BranchRepository> logger,
            AutoMapper.IMapper mapper,
            IConfigurationRoot config) : base(context, logger, mapper, config) { }

        public override void Add(int userId, ChallengeTask domainEntity)
        {
            FixChallengeTaskTypeId(ref domainEntity);
            base.Add(userId, domainEntity);
        }

        public override ChallengeTask AddSave(int userId, ChallengeTask domainEntity)
        {
            FixChallengeTaskTypeId(ref domainEntity);
            return base.AddSave(userId, domainEntity);
        }
        public override void Update(int userId, ChallengeTask domainEntity)
        {
            FixChallengeTaskTypeId(ref domainEntity);
            base.Update(userId, domainEntity);
        }

        public override ChallengeTask UpdateSave(int userId, ChallengeTask domainEntity)
        {
            FixChallengeTaskTypeId(ref domainEntity);
            return base.UpdateSave(userId, domainEntity);
        }

        public override void RemoveSave(int userId, int id)
        {
            var entity = DbSet.Find(id);
            if (entity == null)
            {
                throw new Exception($"ChallengeTask id {id} could not be found.");
            }
            var challengeId = entity.ChallengeId;
            DbSet.Remove(entity);
            var tasks = DbSet.Where(_ => _.ChallengeId == challengeId)
                .OrderBy(_ => _.Position);
            int position = 1;
            foreach (var task in tasks)
            {
                task.Position = position++;
            }
            Save();
        }

        public void DecreasePosition(int taskId)
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
            var previousTask = DbSet
                .Where(_ => _.ChallengeId == task.ChallengeId && _.Position == task.Position - 1)
                .SingleOrDefault();
            previousTask.Position++;
            task.Position--;
            Save();
        }

        public void IncreasePosition(int taskId)
        {
            var task = DbSet.Find(taskId);
            if (task == null)
            {
                throw new Exception($"Task {taskId} could not be found");
            }
            var nextTask = DbSet
                .Where(_ => _.ChallengeId == task.ChallengeId && _.Position == task.Position + 1)
                .SingleOrDefault();
            if (nextTask == null)
            {
                throw new Exception($"Task {taskId} is already in the last position for this challenge.");
            }
            nextTask.Position--;
            task.Position++;
            Save();
        }

        private int GetChallengeTypeId(string name)
        {
            return context.ChallengeTaskTypes
                .AsNoTracking()
                .Where(_ => _.Name == name)
                .Select(_ => _.Id)
                .SingleOrDefault();
        }

        private void FixChallengeTaskTypeId(ref ChallengeTask task)
        {
            if (task.ChallengeTaskTypeId == 0)
            {
                task.ChallengeTaskTypeId = GetChallengeTypeId(task.ChallengeTaskType.ToString());
            }
        }
    }
}

