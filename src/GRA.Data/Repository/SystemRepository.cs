using AutoMapper.QueryableExtensions;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GRA.Data.Repository
{
    public class SystemRepository
        : Abstract.BaseRepository<SystemRepository>, ISystemRepository
    {
        private readonly GenericRepository<Model.System, Domain.Model.System> genericRepo;

        public SystemRepository(Context context,
            ILogger<SystemRepository> logger,
            AutoMapper.IMapper mapper) : base(context, logger, mapper)
        {
            genericRepo =
                new GenericRepository<Model.System, Domain.Model.System>(context, logger, mapper, true);
        }
        public void Add(int userId, Domain.Model.System entity)
        {
            genericRepo.Add(userId, entity);
        }

        public Domain.Model.System AddSave(int userId, Domain.Model.System entity)
        {
            return genericRepo.AddSave(userId, entity);
        }

        public IQueryable<Domain.Model.System> GetAll()
        {
            return genericRepo.DbSet.AsNoTracking().ProjectTo<Domain.Model.System>();
        }

        public Domain.Model.System GetById(int id)
        {
            return genericRepo.GetById(id);
        }

        public IQueryable<Domain.Model.System> PageAll(int skip, int take)
        {
            return genericRepo.PageAll(skip, take);
        }

        public void Remove(int userId, int id)
        {
            genericRepo.Remove(userId, id);
        }

        public void Update(int userId, Domain.Model.System entity)
        {
            genericRepo.Update(userId, entity);
        }
        public Domain.Model.System UpdateSave(int userId, Domain.Model.System entity)
        {
            return genericRepo.UpdateSave(userId, entity);
        }

        public void Save()
        {
            genericRepo.Save();
        }

    }
}
