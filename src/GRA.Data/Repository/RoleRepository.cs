using GRA.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class RoleRepository
        : AuditingRepository<Model.Role, Domain.Model.Role>, IRoleRepository
    {
        public RoleRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<RoleRepository> logger) : base(repositoryFacade, logger) { }

        public async Task AddPermissionAsync(int userId, string name)
        {
            await _context.Permissions.AddAsync(new Model.Permission
            {
                Name = name,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            });
        }

        public async Task AddPermissionToRoleAsync(int userId, int roleId, string permissionName)
        {
            var permission = await _context.Permissions
                .Where(_ => _.Name == permissionName)
                .SingleOrDefaultAsync();
            if (permission == null)
            {
                throw new Exception($"Permission '{permissionName}' not found.");
            }

            await _context.RolePermissions.AddAsync(new Model.RolePermission
            {
                RoleId = roleId,
                PermissionId = permission.Id,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            });
        }

        public async Task<IEnumerable<string>> GetPermisisonNamesForUserAsync(int userId)
        {
            var roleIds = await _context
                .UserRoles
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.RoleId)
                .ToListAsync();

            return await _context
                .RolePermissions
                .AsNoTracking()
                .Where(_ => roleIds.Contains(_.RoleId))
                .Select(_ => _.Permission.Name)
                .Distinct()
                .OrderBy(_ => _)
                .ToListAsync();
        }
    }
}

