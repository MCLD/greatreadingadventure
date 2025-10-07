using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IAvatarItemRepository : IRepository<AvatarItem>
    {
        Task<ICollection<AvatarItem>> GetByLayerAsync(int layerId);
        Task<ICollection<AvatarItem>> GetUserItemsByLayerAsync(int userId, int layerId,
            int languageId);
        Task<bool> HasUserUnlockedItemAsync(int userId, int itemId);
        Task<ICollection<int>> GetUserUnlockedItemsAsync(int userId);
        Task AddUserItemsAsync(int userId, List<int> itemIds);
        Task<DataWithCount<ICollection<AvatarItem>>> PageAsync(AvatarFilter filter);
        Task<int> GetLayerAvailableItemCountAsync(int layerId);
        Task<int> GetLayerUnavailableItemCountAsync(int layerId);
        Task<int> GetLayerUnlockableItemCountAsync(int layerId);
        Task<ICollection<AvatarItem>> GetByIdsAsync(List<int> ids);
        Task<AvatarItem> GetByLayerPositionSortOrderAsync(int layerPosition, int sortOrder);
        Task DecreaseSortPosition(int siteId, int itemId);
        Task IncreaseSortPosition(int siteId, int itemId);
        Task<bool> IsLastInRequiredLayer(int itemId);
        Task<bool> IsInUse(int itemId, bool ignoreUnlockedUsers = false);
        Task RemoveUserItemAsync(int id);
        void RemoveUserUnlockedItem(int id);
        Task<List<AvatarItem>> GetBundleItemsAsync(int bundleId);
        Task<IEnumerable<AvatarItemText>> GetTextsByItemIdsAsync(IEnumerable<int> itemIds);
        Task AddTextsAsync(IEnumerable<AvatarItemText> texts);
        void RemoveTexts(IEnumerable<AvatarItemText> texts);
        void UpdateTexts(IEnumerable<AvatarItemText> texts);
    }
}
