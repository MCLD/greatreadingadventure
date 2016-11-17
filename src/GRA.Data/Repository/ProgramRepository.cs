using AutoMapper.QueryableExtensions;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GRA.Data.Repository
{
    public class ProgramRepository
        : Abstract.BaseRepository<ProgramRepository>, IProgramRepository
    {
        private readonly GenericRepository<Model.Program, Domain.Model.Program> genericRepo;

        public ProgramRepository(
            Context context,
            ILogger<ProgramRepository> logger,
            AutoMapper.IMapper mapper)
            : base(context, logger, mapper)
        {
            genericRepo =
                new GenericRepository<Model.Program, Domain.Model.Program>(context, logger, mapper, true);
        }

        public void Add(int userId, Domain.Model.Program entity)
        {
            genericRepo.Add(userId, entity);
        }
        public Domain.Model.Program AddSave(int userId, Domain.Model.Program entity)
        {
            return genericRepo.AddSave(userId, entity);
        }


        public IQueryable<Domain.Model.Program> GetAll()
        {
            return genericRepo.DbSet.AsNoTracking().ProjectTo<Domain.Model.Program>();
        }

        public Domain.Model.Program GetById(int id)
        {
            return genericRepo.GetById(id);
        }

        public IQueryable<Domain.Model.Program> PageAll(int skip, int take)
        {
            return genericRepo.PageAll(skip, take);
        }

        public void Remove(int userId, int id)
        {
            genericRepo.Remove(userId, id);
        }

        public void Update(int userId, Domain.Model.Program entity)
        {
            genericRepo.Update(userId, entity);
        }
        public Domain.Model.Program UpdateSave(int userId, Domain.Model.Program entity)
        {
            return genericRepo.UpdateSave(userId, entity);
        }
        public void Save()
        {
            genericRepo.Save();
        }
    }

}
