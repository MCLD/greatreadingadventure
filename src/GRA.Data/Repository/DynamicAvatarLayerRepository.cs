using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class DynamicAvatarLayerRepository
        : AuditingRepository<Model.DynamicAvatarLayer, DynamicAvatarLayer>,
        IDynamicAvatarLayerRepository
    {
        public DynamicAvatarLayerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarLayerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<DynamicAvatarLayer>> GetAllAsync(int siteId)
        {
            return await DbSet.AsNoTracking()
               .Where(_ => _.SiteId == siteId)
               .OrderBy(_ => _.GroupId)
               .ThenBy(_ => _.SortOrder)
               .ProjectTo<DynamicAvatarLayer>()
               .ToListAsync();
        }

        public async Task<ICollection<DynamicAvatarLayer>> GetAllWithColorsAsync(int siteId, int userId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.GroupId)
                .ThenBy(_ => _.SortOrder)
                .ProjectTo<DynamicAvatarLayer>(_ => _.DynamicAvatarColors)
                .ToListAsync();
        }
    }
}
