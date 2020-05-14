using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface ITriggerRepository : IRepository<Trigger>
    {
        new Task<Trigger> GetByIdAsync(int id);
        new Task<Trigger> AddSaveAsync(int userId, Trigger trigger);
        new Task<Trigger> UpdateSaveAsync(int userId, Trigger trigger);
        Task<ICollection<Trigger>> PageAsync(TriggerFilter filter);
        Task<int> CountAsync(TriggerFilter filter);
        Task<ICollection<Trigger>> GetTriggersAsync(int userId);
        Task AddTriggerActivationAsync(int userId, int triggerId);
        Task<Trigger> GetByCodeAsync(int siteId, string secretCode, bool mustBeActive);
        Task<DateTime?> CheckTriggerActivationAsync(int userId, int triggerId);
        Task<ICollection<TriggerRequirement>> GetTriggerRequirmentsAsync(Trigger trigger);
        Task<int> CountRequirementsAsync(BaseFilter filter);
        Task<ICollection<TriggerRequirement>> PageRequirementsAsync(BaseFilter filter);
        Task<bool> CodeExistsAsync(int siteId, string secretCode, int? triggerId = null);
        Task<IDictionary<int, string>> DependentTriggers(int triggerId);
        Task<ICollection<Trigger>> GetTriggerDependentsAsync(int triggerBadgeId);
        Task<ICollection<Trigger>> GetChallengeDependentsAsync(int challengeId);
        Task<bool> SecretCodeInUseAsync(int siteId, string secretCode);
        Task<Trigger> GetByBadgeIdAsync(int badgeId);
        Task RemoveUserTriggerAsync(int userId, int triggerId);
        Task<ICollection<Trigger>> GetTriggersAwardingBundleAsync(int bundleId);
        Task<bool> BundleIsInUseAsync(int bundleId);
        Task<ICollection<Trigger>> GetTriggersAwardingPrizesAsync(int siteId);
    }
}
