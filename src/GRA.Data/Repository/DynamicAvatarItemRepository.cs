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

        public async Task<ICollection<DynamicAvatarItem>> GetUserItemsByLayerAsync(int userId,
            int layerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.DynamicAvatarLayerId == layerId && _.Unlockable == false)
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<DynamicAvatarItem>()
                .ToListAsync();
        }
    }
}
