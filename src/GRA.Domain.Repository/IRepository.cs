using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IRepository<DomainEntity>
    {
        Task AddAsync(int currentUserId, DomainEntity entity);
        Task<DomainEntity> AddSaveAsync(int currentUserId, DomainEntity entity);
        Task<DomainEntity> AddSaveNoAuditAsync(DomainEntity entity);
        Task<DomainEntity> GetByIdAsync(int id);
        Task RemoveAsync(int currentUserId, int id);
        Task RemoveSaveAsync(int currentUserId, int id);
        Task SaveAsync();
        Task UpdateAsync(int currentUserId, DomainEntity entity);
        Task<DomainEntity> UpdateSaveAsync(int currentUserId, DomainEntity entity);
        Task<DomainEntity> UpdateSaveNoAuditAsync(DomainEntity entity);
    }
}
