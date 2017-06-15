using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarBundleRepository : IRepository<DynamicAvatarBundle>
    {
        Task<int> CountAsync(AvatarFilter filter);
        Task<ICollection<DynamicAvatarBundle>> PageAsync(AvatarFilter filter);
        Task AddItemsAsync(int bundleId, List<int> itemIds);
        Task RemoveItemsAsync(int bundleId, List<int> itemIds);
        Task<ICollection<DynamicAvatarItem>> GetRandomDefaultBundleAsync(int siteId);
        Task<ICollection<DynamicAvatarBundle>> GetAllAsync(int siteId, bool? unlockable = null);
        Task<DynamicAvatarBundle> GetByIdAsync(int id, bool includeDeleted);
    }
}
