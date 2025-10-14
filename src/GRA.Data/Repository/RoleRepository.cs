using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class RoleRepository
        : AuditingRepository<Model.Role, Domain.Model.Role>, IRoleRepository
    {
        public RoleRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<RoleRepository> logger) : base(repositoryFacade, logger)
        { }

        public async Task AddPermissionListAsync(int userId, IEnumerable<string> names)
        {
            var now = _dateTimeProvider.Now;

            var permissions = names.Select(_ => new Model.Permission
            {
                Name = _,
                CreatedAt = now,
                CreatedBy = userId
            });

            var adminRoles = await DbSet.AsNoTracking()
                .Where(_ => _.IsAdmin)
                .ToListAsync();

            var rolePermissions = new List<Model.RolePermission>();
            foreach (var permission in permissions)
            {
                foreach (var adminRole in adminRoles)
                {
                    rolePermissions.Add(new Model.RolePermission
                    {
                        RoleId = adminRole.Id,
                        Permission = permission,
                        CreatedAt = now,
                        CreatedBy = userId
                    });
                }
            }

            await _context.RolePermissions.AddRangeAsync(rolePermissions);
        }

        public async Task<Role> AddSaveAsync(int userId, Role role, IEnumerable<string> permissions)
        {
            var now = _dateTimeProvider.Now;
            var roleEntity = _mapper.Map<Role, Model.Role>(role);

            var rolePermissions = _context.Permissions
                .AsNoTracking()
                .Where(_ => permissions.Contains(_.Name))
                .Select(_ => new Model.RolePermission
                {
                    Role = roleEntity,
                    PermissionId = _.Id,
                    CreatedAt = now,
                    CreatedBy = userId
                });
            await _context.RolePermissions.AddRangeAsync(rolePermissions);
            await _context.SaveChangesAsync();
            return _mapper.Map<Model.Role, Role>(roleEntity);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .ProjectToType<Role>()
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .AsNoTracking()
                .Select(_ => _.Name)
                .OrderBy(_ => _)
                .ToListAsync();
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

        public async Task<IEnumerable<string>> GetPermissionNamesForRoleAsync(int roleId)
        {
            return await _context.RolePermissions
                .AsNoTracking()
                .Where(_ => _.RoleId == roleId)
                .Select(_ => _.Permission.Name)
                .OrderBy(_ => _)
                .ToListAsync();
        }

        public async Task<IDictionary<int, int>>
            GetUserCountForRolesAsync(IEnumerable<int> roleIds)
        {
            return await _context.UserRoles
                    .AsNoTracking()
                    .Where(_ => roleIds.Contains(_.RoleId))
                    .GroupBy(_ => _.RoleId)
                    .Select(_ => new { Id = _.Key, Count = _.Count() })
                    .ToDictionaryAsync(k => k.Id, v => v.Count);
        }

        public async Task<int> GetUsersWithAdminRoleCountAsync()
        {
            return await _context.UserRoles
                .AsNoTracking()
                .Where(_ => _.Role.IsAdmin)
                .Select(_ => _.UserId)
                .Distinct()
                .CountAsync();
        }

        public async Task<bool> HasInvalidRolesAsync(IEnumerable<int> roleIds)
        {
            var validRoleIds = await _context.Roles
                .AsNoTracking()
                .Select(_ => _.Id)
                .ToListAsync();

            return roleIds.Except(validRoleIds).Any();
        }

        public async Task<bool> ListContainsAdminRoleAsync(IEnumerable<int> roleIds)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsAdmin && roleIds.Contains(_.Id))
                .AnyAsync();
        }

        public async Task<DataWithCount<IEnumerable<Role>>> PageAsync(BaseFilter filter)
        {
            var roles = DbSet.AsNoTracking();
            var count = await roles.CountAsync();
            var data = await roles
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectToType<Role>()
                .ToListAsync();

            return new DataWithCount<IEnumerable<Role>>
            {
                Data = data,
                Count = count
            };
        }

        public void RemovePermissionList(IEnumerable<string> names)
        {
            var permissions = _context.Permissions.Where(_ => names.Contains(_.Name));

            var rolePermissions = _context.RolePermissions
                .Where(_ => permissions.Select(p => p.Id).Contains(_.PermissionId));

            _context.RolePermissions.RemoveRange(rolePermissions);
            _context.Permissions.RemoveRange(permissions);
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var rolePermissions = _context.RolePermissions.Where(_ => _.RoleId == id);
            _context.RolePermissions.RemoveRange(rolePermissions);

            var userRoles = _context.UserRoles.Where(_ => _.RoleId == id);
            _context.UserRoles.RemoveRange(userRoles);

            await RemoveAsync(userId, id);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateSaveAsync(int userId, Role role, List<string> permissionsToAdd,
                            List<string> permissionsToRemove)
        {
            await UpdateAsync(userId, role);

            var now = _dateTimeProvider.Now;

            var addPermissions = _context.Permissions
                .AsNoTracking()
                .Where(_ => permissionsToAdd.Contains(_.Name))
                .Select(_ => new Model.RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = _.Id,
                    CreatedAt = now,
                    CreatedBy = userId
                });
            var removePermissions = _context.RolePermissions
                .Where(_ => _.RoleId == role.Id && !role.IsAdmin
                    && permissionsToRemove.Contains(_.Permission.Name));

            await _context.RolePermissions.AddRangeAsync(addPermissions);
            _context.RolePermissions.RemoveRange(removePermissions);
            await _context.SaveChangesAsync();
        }
    }
}
