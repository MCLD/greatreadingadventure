using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IBadgeRepository : IRepository<Badge>
    {
        Task AddUserBadge(int userId, int badgeId);
        Task<IEnumerable<Badge>> PageForUserAsync(int userId, int skip, int take);
        Task<int> GetCountForUserAsync(int userId);
        Task<bool> UserHasBadge(int userId, int badgeId);
        Task RemoveUserBadgeAsync(int userId, int badgeId);
        Task<string> GetBadgeNameAsync(int badgeId);
    }
}