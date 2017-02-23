using GRA.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ITriggerRepository : IRepository<Trigger>
    {
        new Task<Trigger> GetByIdAsync(int id);
        new Task<Trigger> AddSaveAsync(int userId, Trigger trigger);
        new Task<Trigger> UpdateSaveAsync(int userId, Trigger trigger);
        Task<ICollection<Trigger>> PageAsync(Filter filter);
        Task<int> CountAsync(Filter filter);
        Task<ICollection<Trigger>> GetTriggersAsync(int userId);
        Task AddTriggerActivationAsync(int userId, int triggerId);
        Task<Trigger> GetByCodeAsync(int siteId, string secretCode);
        Task<DateTime?> CheckTriggerActivationAsync(int userId, int triggerId);
        Task<ICollection<TriggerRequirement>> GetTriggerRequirmentsAsync(Trigger trigger);
        Task<int> CountRequirementsAsync(Filter filter);
        Task<ICollection<TriggerRequirement>> PageRequirementsAsync(Filter filter);
        Task<bool> CodeExistsAsync(int siteId, string secretCode, int? triggerId = null);
        Task<bool> HasDependentsAsync(int triggerId);
        Task<ICollection<Trigger>> GetTriggerDependentsAsync(int triggerBadgeId);
        Task<ICollection<Trigger>> GetChallengeDependentsAsync(int challengeId);
    }
}
