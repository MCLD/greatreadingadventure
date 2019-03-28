using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IGroupInfoRepository : IRepository<Model.GroupInfo>
    {
        Task<IEnumerable<GroupInfo>> GetAllAsync(int siteId);
        Task<int> GetCountByTypeAsync(int groupTypeId);
        Task<GroupInfo> GetByUserIdAsync(int householdHeadUserId);
        Task<DataWithCount<ICollection<GroupInfo>>> PageAsync(GroupFilter filter);
    }
}
