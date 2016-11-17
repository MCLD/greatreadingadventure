using GRA.Domain.Repository;
using System.Linq;
using Microsoft.Extensions.Logging;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace GRA.Data.Repository
{
    public class SiteRepository
        : Abstract.BaseRepository<SiteRepository>, ISiteRepository
    {
        private readonly GenericRepository<Model.Site, Domain.Model.Site> genericRepo;

        public SiteRepository(
            Context context,
            ILogger<SiteRepository> logger,
            AutoMapper.IMapper mapper)
            : base(context, logger, mapper)
        {
            genericRepo =
                new GenericRepository<Model.Site, Domain.Model.Site>(context, logger, mapper, true);
        }

        public void Add(int userId, Domain.Model.Site entity)
        {
            genericRepo.Add(userId, entity);
        }

        public Domain.Model.Site AddSave(int userId, Domain.Model.Site entity)
        {
            return genericRepo.AddSave(userId, entity);
        }

        public IQueryable<Domain.Model.Site> GetAll()
        {
            return genericRepo.DbSet.AsNoTracking().ProjectTo<Domain.Model.Site>();
        }

        public Domain.Model.Site GetById(int id)
        {
            return genericRepo.GetById(id);
        }

        public IQueryable<Domain.Model.Site> PageAll(int skip, int take)
        {
            return genericRepo.PageAll(skip, take);
        }

        public void Remove(int userId, int id)
        {
            genericRepo.Remove(userId, id);
        }

        public void Update(int userId, Domain.Model.Site entity)
        {
            genericRepo.Update(userId, entity);
        }
        public Domain.Model.Site UpdateSave(int userId, Domain.Model.Site entity)
        {
            return genericRepo.UpdateSave(userId, entity);
        }

        public void Save()
        {
            genericRepo.Save();
        }
    }
}
