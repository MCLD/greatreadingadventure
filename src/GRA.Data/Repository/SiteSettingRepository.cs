using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class SiteSettingRepository
        : AuditingRepository<Model.SiteSetting, SiteSetting>, ISiteSettingRepository
    {
        public SiteSettingRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SiteSettingRepository> logger) : base(repositoryFacade, logger) { }

        public async Task<ICollection<SiteSetting>> GetBySiteIdAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .ProjectTo<SiteSetting>()
                .ToListAsync();
        }

        public async Task AddListAsync(int userId, IEnumerable<SiteSetting> siteSettings)
        {
            foreach (var siteSetting in siteSettings)
            {
                await base.AddAsync(userId, siteSetting);
            }
        }

        public async Task UpdateListAsync(int userId, IEnumerable<SiteSetting> siteSettings)
        {
            foreach (var siteSetting in siteSettings)
            {
                var setting = await DbSet
                    .Where(_ => _.SiteId == siteSetting.SiteId && _.Key == siteSetting.Key)
                    .SingleAsync();
                string original = null;
                if (AuditSet != null)
                {
                    original = _entitySerializer.Serialize(setting);
                }
                setting.Value = siteSetting.Value;
                await base.UpdateAsync(userId, setting, original);
            }
        }

        public async Task RemoveListAsync(int userId, IEnumerable<int> siteSettingIds)
        {
            foreach (var id in siteSettingIds)
            {
                await base.RemoveAsync(userId, id);
            }
        }
    }
}
