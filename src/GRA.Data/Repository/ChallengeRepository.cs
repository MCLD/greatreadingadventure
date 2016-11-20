using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GRA.Data.Repository
{
    public class ChallengeRepository
        : Abstract.BaseRepository<ChallengeRepository>, IChallengeRepository
    {
        private readonly GenericRepository<Model.Challenge, Domain.Model.Challenge> genericRepo;

        public ChallengeRepository(Context context,
            ILogger<ChallengeRepository> logger,
            IMapper mapper) : base(context, logger, mapper)
        {
            genericRepo =
                new GenericRepository<Model.Challenge, Domain.Model.Challenge>(context, logger, mapper, true);
        }

        private void FixChallengeTaskTypeIds(ref Domain.Model.Challenge entity)
        {
            foreach (var task in entity.GetTasks())
            {
                if (task.ChallengeTaskTypeId == 0)
                {
                    task.ChallengeTaskTypeId = GetChallengeTypeId(task.ChallengeTaskType.ToString());
                }
            }
        }

        public void Add(int userId, Domain.Model.Challenge entity)
        {
            FixChallengeTaskTypeIds(ref entity);
            var dbEntity = genericRepo.Map(entity);
            foreach (var task in dbEntity.Tasks)
            {
                task.CreatedAt = DateTime.Now;
                task.CreatedBy = userId;
            }
            genericRepo.Add(userId, dbEntity);
        }

        public Domain.Model.Challenge AddSave(int userId, Domain.Model.Challenge entity)
        {
            FixChallengeTaskTypeIds(ref entity);
            var dbEntity = genericRepo.Map(entity);
            foreach (var task in dbEntity.Tasks)
            {
                task.CreatedAt = DateTime.Now;
                task.CreatedBy = userId;
            }
            return genericRepo.AddSave(userId, dbEntity);
        }

        public IQueryable<Domain.Model.Challenge> GetPagedChallengeList(int skip, int take)
        {
            // todo: add logic to filter for user
            return genericRepo.DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false)
                .OrderBy(_ => _.Name)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Domain.Model.Challenge>();
        }

        public int GetChallengeCount()
        {
            return genericRepo.DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false)
                .Count();
        }

        public Domain.Model.Challenge GetById(int id)
        {
            // todo: add logic to filter for user
            return genericRepo.Map(genericRepo.DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .Single());
        }

        public void Remove(int userId, int id)
        {
            // todo: fix user lookup
            var entity = GetById(id);
            entity.IsDeleted = true;
            genericRepo.UpdateSave(userId, entity);
        }

        public void Update(int userId, Domain.Model.Challenge entity)
        {
            genericRepo.Update(userId, entity);
        }

        public Domain.Model.Challenge UpdateSave(int userId, Domain.Model.Challenge entity)
        {
            return genericRepo.UpdateSave(userId, entity);
        }

        public void Save()
        {
            genericRepo.Save();
        }

        private int GetChallengeTypeId(string name)
        {
            return context.ChallengeTaskTypes
                .AsNoTracking()
                .Where(_ => _.Name == name)
                .Select(_ => _.Id)
                .SingleOrDefault();
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
