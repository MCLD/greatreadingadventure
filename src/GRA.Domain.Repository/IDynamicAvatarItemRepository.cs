using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarItemRepository : IRepository<DynamicAvatarItem>
    {
        Task<ICollection<DynamicAvatarItem>> GetItemsByLayerAsync(int layerId);
        Task<ICollection<DynamicAvatarItem>> GetUserItemsByLayerAsync(int userId, int layerId);
        Task<bool> HasUserUnlockedItemAsync(int userId, int itemId);
        Task<ICollection<int>> GetUserUnlockedItemsAsync(int userId);
        Task AddUserItemsAsync(int userId, List<int> itemId);
    }
}
