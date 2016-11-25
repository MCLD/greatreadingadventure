using System.Collections.Generic;

namespace GRA.Domain.Repository
{
    public interface IRoleRepository : IRepository<Model.Role>
    {
        void AddPermission(int userId, string permissionName);
        void AddPermissionToRole(int userId, int roleId, string permissionName);
        IEnumerable<string> GetPermisisonNamesForUser(int userId);
    }
}
