using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IBadgeRepository : IRepository<Badge>
    {
        Task<IEnumerable<Badge>> PageForUserAsync(int userId, int skip, int take);
        Task<int> GetCountForUserAsync(int userId);
    }
}