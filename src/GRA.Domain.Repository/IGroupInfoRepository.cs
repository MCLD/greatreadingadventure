using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IGroupInfoRepository : IRepository<Model.GroupInfo>
    {
        Task<IEnumerable<Model.GroupInfo>> GetAllAsync(int siteId);
        Task<int> GetCountByTypeAsync(int groupTypeId);
        Task<Model.GroupInfo> GetByUserIdAsync(int householdHeadUserId);
    }
}
