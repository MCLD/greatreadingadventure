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
    public class AvatarColorRepository : AuditingRepository<Model.AvatarColor, AvatarColor>,
        IAvatarColorRepository
    {
        public AvatarColorRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AvatarColorRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<AvatarColor>> GetByLayerAsync(int layerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayerId == layerId)
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<AvatarColor>()
                .ToListAsync();
        }
    }
}
