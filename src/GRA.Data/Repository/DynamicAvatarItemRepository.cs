using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DynamicAvatarItemRepository : AuditingRepository<Model.DynamicAvatarItem, DynamicAvatarItem>,
        IDynamicAvatarItemRepository
    {
        public DynamicAvatarItemRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarItemRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<DynamicAvatarItem>> GetItemsByLayerAsync(int layerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.DynamicAvatarLayerId == layerId)
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<DynamicAvatarItem>()
                .ToListAsync();
        }

        public async Task<ICollection<DynamicAvatarItem>> GetUserItemsByLayerAsync(int userId,
            int layerId)
        {
            var userUnlockedItems = _context.UserAvatarItems.AsNoTracking()
                .Where(_ => _.UserId == userId 
                    && _.DynamicAvatarItem.DynamicAvatarLayerId == layerId)
                .Select(_ => _.DynamicAvatarItem);

            return await DbSet.AsNoTracking()
                .Where(_ => _.DynamicAvatarLayerId == layerId 
                && (_.Unlockable == false || userUnlockedItems.Select(u => u.Id).Contains(_.Id)))
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<DynamicAvatarItem>()
                .ToListAsync();
        }

        public async Task<bool> HasUserUnlockedItemAsync(int userId, int itemId)
        {
            return await _context.UserAvatarItems.AsNoTracking()
                .Where(_ => _.UserId == userId && _.DynamicAvatarItemId == itemId)
                .AnyAsync();
        }

        public async Task<ICollection<int>> GetUserUnlockedItemsAsync(int userId)
        {
            return await _context.UserAvatarItems.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.DynamicAvatarItemId)
                .ToListAsync();
        }

        public async Task AddUserItemsAsync(int userId, List<int> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                await _context.UserAvatarItems.AddAsync(new Model.UserAvatarItem()
                {
                    UserId = userId,
                    DynamicAvatarItemId = itemId
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}
