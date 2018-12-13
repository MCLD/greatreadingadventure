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
    public class GroupInfoRepository : AuditingRepository<Model.GroupInfo, Domain.Model.GroupInfo>,
        IGroupInfoRepository
    {
        public GroupInfoRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<GroupInfoRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<GroupInfo>> GetAllAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Join(_context.Users,
                    group => group.UserId,
                    user => user.Id,
                    (group, user) => new { group, user })
                .Where(_ => _.user.SiteId == siteId)
                .Select(_ => _.group)
                .ProjectTo<GroupInfo>()
                .ToListAsync();
        }

        public async Task<GroupInfo> GetByUserIdAsync(int householdHeadUserId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == householdHeadUserId)
                .ProjectTo<GroupInfo>()
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetCountByTypeAsync(int groupTypeId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.GroupTypeId == groupTypeId)
                .CountAsync();
        }
    }
}
