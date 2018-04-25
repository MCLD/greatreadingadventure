using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IRoleRepository : IRepository<Model.Role>
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<DataWithCount<IEnumerable<Role>>> PageAsync(BaseFilter filter);
        Task<Role> AddSaveAsync(int userId, Role role, IEnumerable<string> permissions);
        Task UpdateSaveAsync(int userId, Role role, List<string> permissionsToAdd,
            List<string> PermissionsToRemove);
        Task<IEnumerable<string>> GetAllPermissionsAsync();
        Task AddPermissionListAsync(IEnumerable<string> names);
        void RemovePermissionList(IEnumerable<string> names);
        Task<IEnumerable<string>> GetPermisisonNamesForUserAsync(int userId);
        Task<IEnumerable<string>> GetPermissionNamesForRoleAsync(int roleId);
        Task<bool> ListContainsAdminRoleAsync(IEnumerable<int> roleIds);
        Task<int> GetUsersWithAdminRoleCountAsync();
    }
}
