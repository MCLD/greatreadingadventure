using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IBadgeRepository : IRepository<Badge>
    {
        Task AddUserBadge(int userId, int badgeId);

        Task<string> GetBadgeFileNameAsync(int badgeId);

        Task<string> GetBadgeNameAsync(int badgeId);

        Task<int> GetCountBySystemAsync(int systemId);

        Task<int> GetCountForUserAsync(int userId);

        Task<IEnumerable<string>> GetFilesBySystemAsync(int systemId);

        Task<IEnumerable<Badge>> PageForUserAsync(int userId, int skip, int take);

        Task RemoveUserBadgeAsync(int userId, int badgeId);

        Task<bool> UserHasBadge(int userId, int badgeId);

        Task<bool> UserHasJoinBadgeAsync(int userId);
    }
}
