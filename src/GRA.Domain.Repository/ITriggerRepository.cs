using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface ITriggerRepository : IRepository<Trigger>
    {
        new Task<Trigger> AddSaveAsync(int userId, Trigger trigger);

        Task AddTriggerActivationAsync(int userId, int triggerId);

        Task<bool> BundleIsInUseAsync(int bundleId);

        Task<DateTime?> CheckTriggerActivationAsync(int userId, int triggerId);

        Task<bool> CodeExistsAsync(int siteId, string secretCode, int? triggerId = null);

        Task<int> CountAsync(TriggerFilter filter);

        Task<int> CountRequirementsAsync(BaseFilter filter);

        Task<IDictionary<int, string>> DependentTriggers(int triggerId);

        Task<Trigger> GetByBadgeIdAsync(int badgeId);

        Task<Trigger> GetByCodeAsync(int siteId, string secretCode, bool mustBeActive);

        new Task<Trigger> GetByIdAsync(int id);

        Task<ICollection<Trigger>> GetChallengeDependentsAsync(int challengeId);

        Task<IEnumerable<string>> GetNamesAsync(IEnumerable<int> triggerIds);

        Task<ICollection<Trigger>> GetTriggerDependentsAsync(int triggerBadgeId);

        Task<ICollection<TriggerRequirement>> GetTriggerRequirmentsAsync(Trigger trigger);

        Task<ICollection<Trigger>> GetTriggersAsync(int userId);

        Task<ICollection<Trigger>> GetTriggersAwardingBundleAsync(int bundleId);

        Task<ICollection<Trigger>> GetTriggersAwardingPrizesAsync(int siteId);

        Task<ICollection<Trigger>> PageAsync(TriggerFilter filter);

        Task<ICollection<TriggerRequirement>> PageRequirementsAsync(BaseFilter filter);

        Task RemoveUserTriggerAsync(int userId, int triggerId);

        Task<bool> SecretCodeInUseAsync(int siteId, string secretCode);

        new Task<Trigger> UpdateSaveAsync(int userId, Trigger trigger);
    }
}
