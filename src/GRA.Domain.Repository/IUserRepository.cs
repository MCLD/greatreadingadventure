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
        Task SetUserPasswordAsync(int currentUserId, string password);
    }
}
