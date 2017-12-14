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
    }
}
