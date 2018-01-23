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

        public async Task<GroupInfo> GetByUserIdAsync(int householdHeadUserId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == householdHeadUserId)
                .Include(_ => _.GroupType)
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
