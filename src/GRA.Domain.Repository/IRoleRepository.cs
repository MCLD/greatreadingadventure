using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IRoleRepository : IRepository<Model.Role>
    {
        void AddPermission(int userId, string permissionName);
        void AddPermissionToRole(int userId, int roleId, string permissionName);
    }
}
