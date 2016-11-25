using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IUserRepository : IRepository<Model.User>
    {
        Task SetUserPasswordAsync(int currentUserId, string password);
        Task AddRoleAsync(int currentUserId, int userId, int roleId);
        Task<Model.User> GetByUsernameAsync(string username);
        Task<Model.AuthenticationResult> AuthenticateUserAsync(string username, string password);
    }
}
