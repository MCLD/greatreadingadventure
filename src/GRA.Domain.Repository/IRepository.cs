using System.Linq;

namespace GRA.Domain.Repository
{
    public interface IRepository<DomainEntity>
    {
        void Add(int currentUserId, DomainEntity entity);
        DomainEntity AddSave(int currentUserId, DomainEntity entity);
        DomainEntity GetById(int id);
        IQueryable<DomainEntity> PageAll(int skip, int take);
        void RemoveSave(int currentUserId, int id);
        void Save();
        void Update(int currentUserId, DomainEntity entity);
        DomainEntity UpdateSave(int currentUserId, DomainEntity entity);
    }
}
