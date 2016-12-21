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
    public class StaticAvatarRepository
        : AuditingRepository<Model.StaticAvatar, Domain.Model.StaticAvatar>,
        IStaticAvatarRepository
    {
        public StaticAvatarRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<StaticAvatarRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<StaticAvatar>> GetAvartarListAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.CreatedAt)
                .ProjectTo<StaticAvatar>()
                .ToListAsync();
        }

        public async Task<StaticAvatar> GetByIdAsync(int siteId, int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Id == id)
                .ProjectTo<StaticAvatar>()
                .SingleOrDefaultAsync();
        }
    }
}
