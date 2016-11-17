using System.Linq;

namespace GRA.Domain.Repository
{
    public interface IRepository<DomainEntity>
    {
        void Add(int userId, DomainEntity entity);
        DomainEntity AddSave(int userId, DomainEntity entity);
        DomainEntity GetById(int id);
        void Remove(int userId, int id);
        void Save();
        void Update(int userId, DomainEntity entity);
        DomainEntity UpdateSave(int userId, DomainEntity entity);
    }
}
