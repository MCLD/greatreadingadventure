﻿using AutoMapper.QueryableExtensions;
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
    public class AvatarBundleRepository
        : AuditingRepository<Model.AvatarBundle, AvatarBundle>,
        IAvatarBundleRepository
    {
        public AvatarBundleRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AvatarBundleRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<AvatarBundle> GetByIdAsync(int id, bool includeDeleted)
        {
            var bundles = DbSet.AsNoTracking()
                .Where(_ => _.Id == id);

            if (!includeDeleted)
            {
                bundles = bundles.Where(_ => !_.IsDeleted);
            }

            return await bundles
                .ProjectTo<AvatarBundle>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<int> CountAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<AvatarBundle>> PageAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<AvatarBundle>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private IQueryable<Model.AvatarBundle> ApplyFilters(AvatarFilter filter)
        {
            var bundles = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId && !_.IsDeleted);

            if (filter.Unlockable)
            {
                bundles = bundles.Where(_ => _.CanBeUnlocked);
            }

            return bundles;
        }

        public async Task AddItemsAsync(int bundleId, List<int> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                await _context.AvatarBundleItems.AddAsync(new Model.AvatarBundleItem
                {
                    AvatarBundleId = bundleId,
                    AvatarItemId = itemId
                });
            }

            await SaveAsync();
        }

        public async Task RemoveItemsAsync(int bundleId, List<int> itemIds)
        {
            _context.AvatarBundleItems.RemoveRange(
                _context.AvatarBundleItems
                .Where(_ => _.AvatarBundleId == bundleId
                    && itemIds.Contains(_.AvatarItemId)));

            await SaveAsync();
        }

        public async Task<ICollection<AvatarItem>> GetRandomDefaultBundleAsync(int siteId)
        {
            var bundleItems = await DbSet.AsNoTracking()
               .Where(_ => _.SiteId == siteId && !_.CanBeUnlocked && !_.IsDeleted)
               .OrderBy(_ => Guid.NewGuid())
               .Take(1)
               .Select(_ => _.AvatarBundleItems.Select(i => i.AvatarItem))
               .FirstOrDefaultAsync();

            if (bundleItems != null)
            {
                return _mapper.Map<ICollection<AvatarItem>>(bundleItems.ToList());
            }
            else
            {
                return new List<AvatarItem>();
            }
        }

        public async Task<ICollection<AvatarBundle>> GetAllAsync(int siteId,
            bool? unlockable = null)
        {
            var bundles = DbSet.AsNoTracking().Where(_ => _.SiteId == siteId);

            if (unlockable.HasValue)
            {
                bundles = bundles.Where(_ => _.CanBeUnlocked == unlockable.Value && !_.IsDeleted);
            }

            return await bundles.ProjectTo<AvatarBundle>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<bool> IsItemInBundle(int itemId, bool? unlockable = null)
        {
            var inBundle = _context.AvatarBundleItems
                .AsNoTracking()
                .Where(_ => _.AvatarItemId == itemId && !_.AvatarBundle.IsDeleted);

            if (unlockable.HasValue)
            {
                inBundle = inBundle.Where(_ => _.AvatarBundle.CanBeUnlocked == unlockable);
            }

            return await inBundle.AnyAsync();
        }

        public int GetBundleId(int itemId)
        {
            return _context.AvatarBundleItems
                .AsNoTracking()
                .Where(_ => _.AvatarItemId == itemId && !_.AvatarBundle.IsDeleted)
                .Select(_ => _.AvatarBundleId).FirstOrDefault();
        }

        public AvatarBundle GetItemsBundles(int bundleId)
        {
            return GetByIdAsync(bundleId).Result;
        }

        public void RemoveItemFromBundles(int id)
        {
            var bundleItems = _context.AvatarBundleItems.Where(_ => _.AvatarItemId == id);
            _context.AvatarBundleItems.RemoveRange(bundleItems);
        }
    }
}
