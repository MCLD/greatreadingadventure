using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class GroupTypeRepository : AuditingRepository<Model.GroupType, Domain.Model.GroupType>,
        IGroupTypeRepository
    {
        public GroupTypeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<GroupTypeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<GroupType> GetDefaultAsync(int siteid)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteid)
                .OrderBy(_ => _.Id)
                .ProjectToType<GroupType>()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<GroupType>> GetAllForListAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.Name)
                .Select(_ => new GroupType
                {
                    Id = _.Id,
                    Name = _.Name
                })
                .ToListAsync();
        }

        public async Task<(IEnumerable<GroupType>, int)> GetAllPagedAsync(int siteId,
            int skip,
            int take)
        {
            var count = await DbSet.CountAsync();
            var list = await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.Name)
                .Skip(skip)
                .Take(take)
                .ProjectToType<GroupType>()
                .ToListAsync();
            return (list, count);
        }
    }
}
