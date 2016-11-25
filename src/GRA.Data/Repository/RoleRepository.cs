using GRA.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace GRA.Data.Repository
{
    public class RoleRepository
        : AuditingRepository<Model.Role, Domain.Model.Role>, IRoleRepository
    {
        public RoleRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<RoleRepository> logger) : base(repositoryFacade, logger) { }

        public void AddPermission(int userId, string name)
        {
            context.Permissions.Add(new Model.Permission
            {
                Name = name,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            });
        }

        public void AddPermissionToRole(int userId, int roleId, string permissionName)
        {
            var permission = context.Permissions
                .Where(_ => _.Name == permissionName)
                .SingleOrDefault();
            if (permission == null)
            {
                throw new Exception($"Permission '{permissionName}' not found.");
            }

            context.RolePermissions.Add(new Model.RolePermission
            {
                RoleId = roleId,
                PermissionId = permission.Id,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            });
        }

        public IEnumerable<string> GetPermisisonNamesForUser(int userId)
        {
            var roleIds = context
                .UserRoles
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.RoleId);

            return context
                .RolePermissions
                .AsNoTracking()
                .Where(_ => roleIds.Contains(_.RoleId))
                .Select(_ => _.Permission.Name)
                .Distinct()
                .OrderBy(_ => _);
        }
    }
}

