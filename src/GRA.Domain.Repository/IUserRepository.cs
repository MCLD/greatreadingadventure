using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IUserRepository : IRepository<Model.User>
    {
        Task<Model.User> AddPointsSaveAsync(int currentUserId,
            int whoEarnedUserId,
            int pointsEarned,
            bool loggingAsAdminUser);
        Task AddRoleAsync(int currentUserId, int userId, int roleId);
        Task<Model.AuthenticationResult> AuthenticateUserAsync(string username, string password);
        Task<Model.User> GetByUsernameAsync(string username);
        Task<int> GetCountAsync();
        Task<int> GetFamilyCountAsync(int householdHeadUserId);
        Task<IEnumerable<Model.User>> PageAllAsync(int siteId, int skip, int take);

        Task<IEnumerable<Model.User>>
            PageFamilyAsync(int householdHeadUserId, int skip, int take);
        Task SetUserPasswordAsync(int currentUserId, string password);
    }
}
