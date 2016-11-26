using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IRoleRepository : IRepository<Model.Role>
    {
        Task AddPermissionAsync(int userId, string permissionName);
        Task AddPermissionToRoleAsync(int userId, int roleId, string permissionName);
        Task<IEnumerable<string>> GetPermisisonNamesForUserAsync(int userId);
    }
}
