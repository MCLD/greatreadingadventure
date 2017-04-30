using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
    }
}
