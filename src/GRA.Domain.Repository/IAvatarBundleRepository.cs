using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<bool> IsItemInBundle(int itemId, bool? unlockable = null);
        void RemoveItemFromBundles(int id);
        AvatarBundle GetItemsBundles(int bundleIds);
        int GetBundleId(int itemId);
    }
}
