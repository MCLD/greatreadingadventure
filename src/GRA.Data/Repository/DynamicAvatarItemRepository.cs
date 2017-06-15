using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
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

        public async Task<int> CountAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<DynamicAvatarItem>> PageAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .ApplyPagination(filter)
                .ProjectTo<DynamicAvatarItem>()
                .ToListAsync();
        }

        private IQueryable<Model.DynamicAvatarItem> ApplyFilters(AvatarFilter filter)
        {
            var items = DbSet.AsNoTracking()
                .Where(_ => _.DynamicAvatarLayer.SiteId == filter.SiteId);

            if (filter.Unlockable.HasValue)
            {
                items = items.Where(_ => _.Unlockable == filter.Unlockable.Value);
            }

            if (filter.LayerId.HasValue)
            {
                items = items.Where(_ => _.DynamicAvatarLayerId == filter.LayerId.Value);
            }

            if (filter.ItemIds?.Count > 0)
            {
                items = items.Where(_ => !filter.ItemIds.Contains(_.Id));
            }

            if (!string.IsNullOrEmpty(filter.Search))
            {
                items = items.Where(_ => _.Name.Contains(filter.Search));
            }

            return items;
        }

        public async Task<ICollection<DynamicAvatarItem>> GetByIdsAsync(List<int> ids)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => ids.Contains(_.Id))
                .ProjectTo<DynamicAvatarItem>()
                .ToListAsync();
        }
    }
}
