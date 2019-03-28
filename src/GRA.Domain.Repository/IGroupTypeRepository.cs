using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IGroupTypeRepository : IRepository<GroupType>
    {
        Task<GroupType> GetDefaultAsync(int siteid);
        Task<IEnumerable<GroupType>> GetAllForListAsync(int siteId);
        Task<(IEnumerable<GroupType>, int)> GetAllPagedAsync(int siteId, int skip, int take);
    }
}
