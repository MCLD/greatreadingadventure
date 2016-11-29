using System.Linq;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GRA.Data.Repository
{
    public class ChallengeRepository
        : AuditingRepository<Model.Challenge, Domain.Model.Challenge>, IChallengeRepository
    {
        public ChallengeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<ChallengeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Domain.Model.Challenge>>
            PageAllAsync(int siteId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                       && _.SiteId == siteId)
                .OrderBy(_ => _.Name)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Domain.Model.Challenge>()
                .ToListAsync();
        }

        public async Task<int> GetChallengeCountAsync()
        {
            var challenges = await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false)
                .ToListAsync();
            return challenges.Count();
        }

        public override async Task<Domain.Model.Challenge> GetByIdAsync(int id)
        {
            var challenge = mapper.Map<Model.Challenge, Domain.Model.Challenge>(await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleAsync());

            if (challenge != null)
            {
                challenge.Tasks = await context.ChallengeTasks
                .AsNoTracking()
                .Where(_ => _.ChallengeId == id)
                .OrderBy(_ => _.Position)
                .ProjectTo<Domain.Model.ChallengeTask>()
                .ToListAsync();

                await GetChallengeTasksTypeAsync(challenge.Tasks);
            }

            return challenge;
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await context.Challenges
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }

        public async Task<IEnumerable<Domain.Model.ChallengeTask>>
            GetChallengeTasksAsync(int challengeId)
        {
            var tasks = await context.ChallengeTasks
                .AsNoTracking()
                .Where(_ => _.ChallengeId == challengeId)
                .OrderBy(_ => _.Position)
                .ProjectTo<Domain.Model.ChallengeTask>()
                .ToListAsync();

            return await GetChallengeTasksTypeAsync(tasks);
        }

        private async Task<IEnumerable<Domain.Model.ChallengeTask>>
            GetChallengeTasksTypeAsync(IEnumerable<Domain.Model.ChallengeTask> tasks)
        {
            var challengeTaskTypes =
                await context.ChallengeTaskTypes
                .AsNoTracking()
                .ToDictionaryAsync(_ => _.Id);

            foreach (var task in tasks)
            {
                task.ChallengeTaskType = (Domain.Model.ChallengeTaskType)
                    Enum.Parse(typeof(Domain.Model.ChallengeTaskType),
                    challengeTaskTypes[task.ChallengeTaskTypeId].Name);
            }
            return tasks;
        }
    }
}
