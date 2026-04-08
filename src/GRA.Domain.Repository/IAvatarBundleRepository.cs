using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;

namespace GRA.Domain.Repository
{
    public interface IAvatarBundleRepository : IRepository<AvatarBundle>
    {
        Task AddItemsAsync(int bundleId, List<int> itemIds);

        Task<int> CountAsync(AvatarFilter filter);

        Task<ICollection<AvatarBundle>> GetAllAsync(int siteId, bool? unlockable = null);

        Task<AvatarBundle> GetByIdAsync(int id, bool includeDeleted);

        Task<ICollection<AvatarBundleTransfer>> GetForExportAsync(int siteId);

        Task<ICollection<AvatarItem>> GetRandomDefaultBundleAsync(int siteId);

        Task<bool> IsItemInBundleAsync(int itemId, bool? unlockable = null);

        Task<ICollection<AvatarBundle>> PageAsync(AvatarFilter filter);

        void RemoveItemFromBundles(int id);

        Task RemoveItemsAsync(int bundleId, List<int> itemIds);

        Task UpdateHasBeenViewedAsync(int userId, int bundleId);

        Task<List<UserLog>> UserHistoryAsync(int userId);
    }
}
