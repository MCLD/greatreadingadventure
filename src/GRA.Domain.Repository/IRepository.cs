﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IRepository<DomainEntity>
    {
        Task AddAsync(int currentUserId, DomainEntity entity);

        Task<DomainEntity> AddSaveAsync(int currentUserId, DomainEntity entity);

        Task<DomainEntity> AddSaveNoAuditAsync(DomainEntity entity);

        Task<DomainEntity> GetByIdAsync(int id);

        Task<ICollection<ChangedItem<DomainEntity>>> GetChangesAsync(int entityId);

        Task RemoveAsync(int currentUserId, int id);

        Task RemoveSaveAsync(int currentUserId, int id);

        Task SaveAsync();

        Task UpdateAsync(int currentUserId, DomainEntity entity);

        Task<DomainEntity> UpdateSaveAsync(int currentUserId, DomainEntity entity);

        Task<DomainEntity> UpdateSaveNoAuditAsync(DomainEntity entity);
    }
}
