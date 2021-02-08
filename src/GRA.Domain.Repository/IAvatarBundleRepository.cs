using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IAvatarBundleRepository : IRepository<AvatarBundle>
    {
        Task<int> CountAsync(AvatarFilter filter);
        Task<ICollection<AvatarBundle>> PageAsync(AvatarFilter filter);
        Task AddItemsAsync(int bundleId, List<int> itemIds);
        Task RemoveItemsAsync(int bundleId, List<int> itemIds);
        Task<ICollection<AvatarItem>> GetRandomDefaultBundleAsync(int siteId);
        Task<ICollection<AvatarBundle>> GetAllAsync(int siteId, bool? unlockable = null);
        Task<AvatarBundle> GetByIdAsync(int id, bool includeDeleted);
        Task<bool> IsItemInBundleAsync(int itemId, bool? unlockable = null);
        void RemoveItemFromBundles(int id);
        Task<List<UserLog>> UserHistoryAsync(int userId);
        Task UpdateHasBeenViewedAsync(int userId, int bundleId);
        Task<List<AvatarBundle>> GetBundlesByAssociatedId(int bundleId);
        Task<ICollection<AvatarBundle>> GetAllPreconfiguredParentsAsync(int siteId);
    }
}
