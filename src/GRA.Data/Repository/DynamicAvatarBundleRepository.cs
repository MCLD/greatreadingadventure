using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository.Extensions;

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

        public override async Task<DynamicAvatarBundle> GetByIdAsync(int id)
        {
            return await DbSet.AsNoTracking()
                .Include(_ => _.DynamicAvatarBundleItems)
                    .ThenInclude(_ => _.DynamicAvatarItem)
                .Where(_ => _.Id == id)
                .ProjectTo<DynamicAvatarBundle>()
                .SingleOrDefaultAsync();
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<DynamicAvatarBundle>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .ApplyPagination(filter)
                .ProjectTo<DynamicAvatarBundle>()
                .ToListAsync();
        }

        private IQueryable<Model.DynamicAvatarBundle> ApplyFilters(BaseFilter filter)
        {
            var bundles = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);
 
            return bundles;
        }

        public async Task AddItemAsync(int bundleId, int itemId)
        {
            await _context.DynamicAvatarBundleItems.AddAsync(new Model.DynamicAvatarBundleItem
            {
                DynamicAvatarBundleId = bundleId,
                DynamicAvatarItemId = itemId
            });

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
                bundles = bundles.Where(_ => _.CanBeUnlocked == unlockable.Value);
            }

            return await bundles.ProjectTo<DynamicAvatarBundle>().ToListAsync();
        }
    }
}
