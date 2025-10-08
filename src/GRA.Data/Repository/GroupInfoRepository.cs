using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
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
                .OrderBy(_ => _.Name)
                .ProjectToType<GroupInfo>()
                .ToListAsync();
        }

        public async Task<GroupInfo> GetByUserIdAsync(int householdHeadUserId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == householdHeadUserId)
                .ProjectToType<GroupInfo>()
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetCountByTypeAsync(int groupTypeId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.GroupTypeId == groupTypeId)
                .CountAsync();
        }

        public async Task<DataWithCount<ICollection<GroupInfo>>> PageAsync(GroupFilter filter)
        {
            var groups = DbSet.AsNoTracking();

            if (filter.GroupTypeIds?.Any() == true)
            {
                groups = groups.Where(_ => filter.GroupTypeIds.Contains(_.GroupTypeId));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                groups = groups.Where(_ => _.Name.Contains(filter.Search)
                    || (_.User.FirstName + " " + _.User.LastName).Contains(filter.Search));
            }

            var count = await groups.CountAsync();

            var groupList = await groups
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectToType<GroupInfo>()
                .ToListAsync();

            return new DataWithCount<ICollection<GroupInfo>>
            {
                Data = groupList,
                Count = count
            };
        }
    }
}
