using System;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.Repository
{
    public class ChallengeRepository
        : AuditingRepository<Model.Challenge, Domain.Model.Challenge>, IChallengeRepository
    {
        public ChallengeRepository(Context context,
            ILogger<ChallengeRepository> logger,
            IMapper mapper,
            IConfigurationRoot config) : base(context, logger, mapper, config)
        {
        }

        public override IQueryable<Domain.Model.Challenge> PageAll(int skip, int take)
        {
            // todo: add logic to filter for user
            return DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false)
                .OrderBy(_ => _.Name)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Domain.Model.Challenge>();
        }

        public int GetChallengeCount()
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false)
                .Count();
        }

        public override Domain.Model.Challenge GetById(int id)
        {
            // todo: add logic to filter for user
            var challenge = mapper.Map<Model.Challenge, Domain.Model.Challenge>(DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .Single());

            if (challenge != null)
            {
                challenge.Tasks = context.ChallengeTasks
                .AsNoTracking()
                .Where(_ => _.ChallengeId == id)
                .OrderBy(_ => _.Position)
                .ProjectTo<Domain.Model.ChallengeTask>()
                .ToList();
            }

            return challenge;
        }

        public override void RemoveSave(int userId, int id)
        {
            // todo: fix user lookup
            var entity = context.Challenges
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .Single();
            entity.IsDeleted = true;
            base.UpdateSave(userId, entity, string.Empty);
        }

        public void AddChallengeTaskType(int userId, string name)
        {
            context.ChallengeTaskTypes.Add(new Model.ChallengeTaskType
            {
                Name = name,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();
        }
    }
}
