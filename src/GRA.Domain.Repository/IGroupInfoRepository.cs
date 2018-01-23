using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IGroupInfoRepository : IRepository<Model.GroupInfo>
    {
        Task<int> GetCountByTypeAsync(int groupTypeId);
        Task<Model.GroupInfo> GetByUserIdAsync(int householdHeadUserId);
    }
}
