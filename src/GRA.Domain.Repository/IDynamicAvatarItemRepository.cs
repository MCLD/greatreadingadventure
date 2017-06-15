using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarItemRepository : IRepository<DynamicAvatarItem>
    {
        Task<ICollection<DynamicAvatarItem>> GetItemsByLayerAsync(int layerId);
        Task<ICollection<DynamicAvatarItem>> GetUserItemsByLayerAsync(int userId, int layerId);
        Task<bool> HasUserUnlockedItemAsync(int userId, int itemId);
        Task<ICollection<int>> GetUserUnlockedItemsAsync(int userId);
        Task AddUserItemsAsync(int userId, List<int> itemId);
        Task<int> CountAsync(AvatarFilter filter);
        Task<ICollection<DynamicAvatarItem>> PageAsync(AvatarFilter filter);
        Task<ICollection<DynamicAvatarItem>> GetByIdsAsync(List<int> ids);
    }
}
