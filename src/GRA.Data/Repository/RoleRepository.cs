using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
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

        public async Task<DataWithCount<IEnumerable<Role>>> PageAsync(BaseFilter filter)
        {
            var roles = DbSet.AsNoTracking();
            var count = await roles.CountAsync();
            var data = await roles
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<Role>()
                .ToListAsync();

            return new DataWithCount<IEnumerable<Role>>()
            {
                Data = data,
                Count = count
            };
        }

        public async Task<IEnumerable<string>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .AsNoTracking()
                .Select(_ => _.Name)
                .ToListAsync();
        }

        public async Task AddPermissionListAsync(IEnumerable<string> names)
        {
            var now = _dateTimeProvider.Now;

            var permissions = names.Select(_ => new Model.Permission
            {
                Name = _,
                CreatedAt = now,
                CreatedBy = -1
            });

            var rolePermissions = DbSet.AsNoTracking()
                .Where(_ => _.IsAdmin)
                .SelectMany(_ => permissions, (r, p) => new Model.RolePermission
                {
                    RoleId = r.Id,
                    Permission = p,
                    CreatedAt = now,
                    CreatedBy = -1
                });

            await _context.RolePermissions.AddRangeAsync(rolePermissions);
        }

        public void RemovePermissionList(IEnumerable<string> names)
        {
            var permissions = _context.Permissions.Where(_ => names.Contains(_.Name));

            var rolePermissions = _context.RolePermissions
                .Where(_ => permissions.Select(p => p.Id).Contains(_.PermissionId));

            _context.RolePermissions.RemoveRange(rolePermissions);
            _context.Permissions.RemoveRange(permissions);
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

