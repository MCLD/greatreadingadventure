using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IRoleRepository : IRepository<Model.Role>
    {
        Task<DataWithCount<IEnumerable<Role>>> PageAsync(BaseFilter filter);
        Task<IEnumerable<string>> GetAllPermissionsAsync();
        Task AddPermissionListAsync(IEnumerable<string> names);
        void RemovePermissionList(IEnumerable<string> names);
        Task AddPermissionAsync(int userId, string permissionName);
        Task AddPermissionToRoleAsync(int userId, int roleId, string permissionName);
        Task<IEnumerable<string>> GetPermisisonNamesForUserAsync(int userId);
    }
}
