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
    public class AvatarItemRepository : AuditingRepository<Model.AvatarItem, AvatarItem>,
        IAvatarItemRepository
    {
        public AvatarItemRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AvatarItemRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<AvatarItem>> GetByLayerAsync(int layerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayerId == layerId)
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<AvatarItem>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ICollection<AvatarItem>> GetUserItemsByLayerAsync(int userId,
            int layerId)
        {
            var userUnlockedItems = _context.UserAvatarItems.AsNoTracking()
                .Where(_ => _.UserId == userId
                    && _.AvatarItem.AvatarLayerId == layerId)
                .Select(_ => _.AvatarItem);

            return await DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayerId == layerId
                    && (!_.Unlockable || userUnlockedItems.Select(u => u.Id).Contains(_.Id)))
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<AvatarItem>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<bool> HasUserUnlockedItemAsync(int userId, int itemId)
        {
            return await _context.UserAvatarItems.AsNoTracking()
                .Where(_ => _.UserId == userId && _.AvatarItemId == itemId)
                .AnyAsync();
        }

        public async Task<ICollection<int>> GetUserUnlockedItemsAsync(int userId)
        {
            return await _context.UserAvatarItems.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.AvatarItemId)
                .ToListAsync();
        }
        public async Task<AvatarItem> GetDefaultLayerItemAsync(int defaultLayerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayerId == defaultLayerId)
                .ProjectTo<AvatarItem>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task AddUserItemsAsync(int userId, List<int> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                await _context.UserAvatarItems.AddAsync(new Model.UserAvatarItem
                {
                    UserId = userId,
                    AvatarItemId = itemId
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<AvatarItem>> PageAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.SortOrder)
                .ApplyPagination(filter)
                .ProjectTo<AvatarItem>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private IQueryable<Model.AvatarItem> ApplyFilters(AvatarFilter filter)
        {
            var items = DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayer.SiteId == filter.SiteId);

            if (filter.Available)
            {
                items = items.Where(_ => !_.Unlockable);
            }
            else if (filter.CanBeUnlocked || filter.Unavailable || filter.Unlockable)
            {
                items = items.Where(_ => _.Unlockable);

                if (filter.Unavailable || filter.Unlockable)
                {
                    var unlockableItems = _context.AvatarBundleItems
                       .Where(_ => !_.AvatarBundle.IsDeleted && _.AvatarBundle.CanBeUnlocked)
                       .Select(_ => _.AvatarItemId)
                       .Distinct();

                    if (filter.Unavailable)
                    {
                        items = items.Where(_ => !unlockableItems.Contains(_.Id));
                    }
                    else
                    {
                        items = items.Where(_ => unlockableItems.Contains(_.Id));
                    }
                }
            }

            if (filter.LayerId.HasValue)
            {
                items = items.Where(_ => _.AvatarLayerId == filter.LayerId.Value);
            }

            if (filter.ItemIds?.Count > 0)
            {
                items = items.Where(_ => !filter.ItemIds.Contains(_.Id));
            }

            if (!string.IsNullOrEmpty(filter.Search))
            {
                items = items.Where(_ =>
                    _.Name.Contains(filter.Search, System.StringComparison.OrdinalIgnoreCase));
            }

            return items;
        }

        public async Task<int> GetLayerAvailableItemCountAsync(int layerId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.AvatarLayerId == layerId && !_.Unlockable)
                .CountAsync();
        }

        public async Task<int> GetLayerUnavailableItemCountAsync(int layerId)
        {
            var unlockableItems = _context.AvatarBundleItems
                       .Where(_ => !_.AvatarBundle.IsDeleted && _.AvatarBundle.CanBeUnlocked)
                       .Select(_ => _.AvatarItemId)
                       .Distinct();

            return await DbSet
                .AsNoTracking()
                .Where(_ => _.AvatarLayerId == layerId && _.Unlockable
                    && !unlockableItems.Contains(_.Id))
                .CountAsync();
        }

        public async Task<int> GetLayerUnlockableItemCountAsync(int layerId)
        {
            var unlockableItems = _context.AvatarBundleItems
                       .Where(_ => !_.AvatarBundle.IsDeleted && _.AvatarBundle.CanBeUnlocked)
                       .Select(_ => _.AvatarItemId)
                       .Distinct();

            return await DbSet
                .AsNoTracking()
                .Where(_ => _.AvatarLayerId == layerId && _.Unlockable
                    && unlockableItems.Contains(_.Id))
                .CountAsync();
        }

        public async Task<List<AvatarItem>> GetBundleItemsAsync(int bundleId)
        {
            return await _context.AvatarBundleItems
                .AsNoTracking()
                .Where(_ => _.AvatarBundleId == bundleId)
                .Select(_ => _.AvatarItem)
                .ProjectTo<AvatarItem>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ICollection<AvatarItem>> GetByIdsAsync(List<int> ids)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => ids.Contains(_.Id))
                .ProjectTo<AvatarItem>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<AvatarItem> GetByLayerPositionSortOrderAsync(int layerPosition,
            int sortOrder)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayer.Position == layerPosition && _.SortOrder == sortOrder)
                .ProjectTo<AvatarItem>(_mapper.ConfigurationProvider)
                .SingleAsync();
        }

        public async Task DecreaseSortPosition(int siteId, int itemId)
        {
            var item = await DbSet
                .Where(_ => _.AvatarLayer.SiteId == siteId && _.Id == itemId)
                .SingleAsync();
            var prevItem = await DbSet
                .Where(_ => _.AvatarLayerId == item.AvatarLayerId
                    && _.SortOrder == item.SortOrder - 1)
                .SingleOrDefaultAsync();

            if (prevItem != null)
            {
                item.SortOrder--;
                prevItem.SortOrder++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncreaseSortPosition(int siteId, int itemId)
        {
            var item = await DbSet
                .Where(_ => _.AvatarLayer.SiteId == siteId && _.Id == itemId)
                .SingleAsync();
            var nextItem = await DbSet
                .Where(_ => _.AvatarLayerId == item.AvatarLayerId
                    && _.SortOrder == item.SortOrder + 1)
                .SingleOrDefaultAsync();

            if (nextItem != null)
            {
                item.SortOrder++;
                nextItem.SortOrder--;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsLastInRequiredLayer(int itemId)
        {
            var layer = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == itemId)
                .Select(_ => _.AvatarLayer)
                .SingleAsync();
            if (!layer.CanBeEmpty)
            {
                var availableItems = await DbSet.AsNoTracking()
                    .Where(_ => _.AvatarLayerId == layer.Id && !_.Unlockable)
                    .CountAsync();
                if (availableItems <= 1)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> IsInUse(int itemId, bool ignoreUnlockedUsers = false)
        {
            var elements = _context.AvatarElements
                .AsNoTracking()
                .Where(_ => _.AvatarItemId == itemId)
                .Select(_ => _.Id);

            var inUseBy = _context.UserAvatars
                .AsNoTracking()
                .Where(_ => !_.User.IsDeleted
                    && elements.Contains(_.AvatarElementId));

            if (ignoreUnlockedUsers)
            {
                var unlockedUsers = _context.UserAvatarItems
                .AsNoTracking()
                .Where(_ => _.AvatarItemId == itemId)
                .Select(_ => _.UserId);

                inUseBy = inUseBy.Where(_ => !unlockedUsers.Contains(_.UserId));
            }

            return await inUseBy.AnyAsync();
        }

        public async Task RemoveUserItemAsync(int id)
        {
            var elements = _context.AvatarElements
                .AsNoTracking()
                .Where(_ => _.AvatarItemId == id)
                .Select(_ => _.Id);

            var userElements = _context.UserAvatars
                .Where(_ => elements.Contains(_.AvatarElementId));
            if (await userElements.AnyAsync())
            {
                _context.UserAvatars.RemoveRange(userElements);

                var item = await DbSet
                    .AsNoTracking()
                    .Include(_ => _.AvatarLayer)
                    .Where(_ => _.Id == id)
                    .SingleAsync();

                if (!item.AvatarLayer.CanBeEmpty)
                {
                    var firstElement = _context.AvatarElements
                        .AsNoTracking()
                        .Where(_ => _.AvatarItem.AvatarLayerId == item.AvatarLayerId
                            && _.AvatarItemId != id)
                        .OrderBy(_ => _.AvatarItem.SortOrder)
                        .Select(_ => _.Id)
                        .First();

                    var newUserElements = userElements
                        .Select(_ => new Model.UserAvatar
                        {
                            AvatarElementId = firstElement,
                            UserId = _.UserId
                        });

                    await _context.UserAvatars.AddRangeAsync(newUserElements);
                }
            }
        }

        public void RemoveUserUnlockedItem(int id)
        {
            var unlockedItems = _context.UserAvatarItems.Where(_ => _.AvatarItemId == id);
            _context.UserAvatarItems.RemoveRange(unlockedItems);
        }

        public override async Task RemoveAsync(int userId, int id)
        {
            var item = DbSet.AsNoTracking().Where(_ => _.Id == id).Single();
            var itemsToUpdate = DbSet.Where(_ => _.AvatarLayerId == item.AvatarLayerId
                && _.SortOrder > item.SortOrder);
            await itemsToUpdate.ForEachAsync(_ => _.SortOrder--);
            DbSet.UpdateRange(itemsToUpdate);
            await base.RemoveAsync(userId, id);
        }
    }
}
