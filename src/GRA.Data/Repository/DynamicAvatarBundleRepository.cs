using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class DynamicAvatarBundleRepository
        : AuditingRepository<Model.DynamicAvatarBundle, DynamicAvatarBundle>,
        IDynamicAvatarBundleRepository
    {
        public DynamicAvatarBundleRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarBundleRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<DynamicAvatarBundle> GetByIdAsync(int id, bool includeDeleted)
        {
            var bundles = DbSet.AsNoTracking()
                .Include(_ => _.DynamicAvatarBundleItems)
                    .ThenInclude(_ => _.DynamicAvatarItem)
                .Where(_ => _.Id == id);

            if (!includeDeleted)
            {
                bundles = bundles.Where(_ => _.IsDeleted == false);
            }

            return await bundles
                .ProjectTo<DynamicAvatarBundle>()
                .SingleOrDefaultAsync();
        }

        public async Task<int> CountAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<DynamicAvatarBundle>> PageAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .ApplyPagination(filter)
                .ProjectTo<DynamicAvatarBundle>()
                .ToListAsync();
        }

        private IQueryable<Model.DynamicAvatarBundle> ApplyFilters(AvatarFilter filter)
        {
            var bundles = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId && _.IsDeleted == false);

            if (filter.Unlockable.HasValue)
            {
                bundles = bundles.Where(_ => _.CanBeUnlocked == filter.Unlockable.Value);
            }

            return bundles;
        }

        public async Task AddItemsAsync(int bundleId, List<int> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                await _context.DynamicAvatarBundleItems.AddAsync(new Model.DynamicAvatarBundleItem
                {
                    DynamicAvatarBundleId = bundleId,
                    DynamicAvatarItemId = itemId
                });
            }

            await SaveAsync();
        }

        public async Task RemoveItemsAsync(int bundleId, List<int> itemIds)
        {
            _context.DynamicAvatarBundleItems.RemoveRange(
                _context.DynamicAvatarBundleItems
                .Where(_ => _.DynamicAvatarBundleId == bundleId
                    && itemIds.Contains(_.DynamicAvatarItemId)));

            await SaveAsync();
        }

        public async Task<ICollection<DynamicAvatarItem>> GetRandomDefaultBundleAsync(int siteId)
        {
            var bundleItems = await DbSet.AsNoTracking()
               .Include(_ => _.DynamicAvatarBundleItems)
                   .ThenInclude(_ => _.DynamicAvatarItem)
               .Where(_ => _.SiteId == siteId && _.CanBeUnlocked == false)
               .OrderBy(_ => Guid.NewGuid())
               .Take(1)
               .Select(_ => _.DynamicAvatarBundleItems.Select(i => i.DynamicAvatarItem))
               .FirstOrDefaultAsync();

            if (bundleItems != null)
            {
                return _mapper.Map<ICollection<DynamicAvatarItem>>(bundleItems.ToList());
            }
            else
            {
                return new List<DynamicAvatarItem>();
            }
        }

        public async Task<ICollection<DynamicAvatarBundle>> GetAllAsync(int siteId,
            bool? unlockable = null)
        {
            var bundles = DbSet.AsNoTracking().Where(_ => _.SiteId == siteId);

            if (unlockable.HasValue)
            {
                bundles = bundles.Where(_ => _.CanBeUnlocked == unlockable.Value 
                    && _.IsDeleted == false);
            }

            return await bundles.ProjectTo<DynamicAvatarBundle>().ToListAsync();
        }
    }
}
