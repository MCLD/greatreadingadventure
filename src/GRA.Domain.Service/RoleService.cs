using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class RoleService : BaseUserService<RoleService>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteRepository _siteRepository;

        public RoleService(ILogger<RoleService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IRoleRepository roleRepository,
            ISiteRepository siteRepository) : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageRoles);
            _roleRepository = roleRepository
                ?? throw new ArgumentNullException(nameof(roleRepository));
            _siteRepository = siteRepository
                ?? throw new ArgumentNullException(nameof(siteRepository));
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            VerifyManagementPermission();
            return await _roleRepository.GetAllAsync();
        }

        public async Task<DataWithCount<IEnumerable<Role>>> GetPaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();
            var roleList = await _roleRepository.PageAsync(filter);
            foreach (var role in roleList.Data)
            {
                var permissions = await _roleRepository.GetPermissionNamesForRoleAsync(role.Id);
                role.PermissionCount = permissions.Count();
            }
            return roleList;
        }

        public async Task<Role> AddAsync(Role role, IEnumerable<string> permissions)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            role.Name = role.Name.Trim();
            role.IsAdmin = false;

            if (permissions.Count() > 0)
            {
                role.CreatedAt = _dateTimeProvider.Now;
                role.CreatedBy = authId;
                return await _roleRepository.AddSaveAsync(authId, role, permissions);
            }
            else
            {
                return await _roleRepository.AddSaveAsync(authId, role);
            }
        }

        public async Task EditAsync(Role role, IEnumerable<string> permissions)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var currentRole = await _roleRepository.GetByIdAsync(role.Id);
            currentRole.Name = role.Name.Trim();

            var permissionsToAdd = new List<string>();
            var permissionsToRemove = new List<string>();
            if (currentRole.IsAdmin == false)
            {
                var currentPermissions = await _roleRepository
                    .GetPermissionNamesForRoleAsync(currentRole.Id);

                permissionsToAdd = permissions.Except(currentPermissions).ToList();
                permissionsToRemove = currentPermissions.Except(permissions).ToList();
            }
            await _roleRepository.UpdateSaveAsync(authId, currentRole, permissionsToAdd, permissionsToRemove);
        }

        public async Task RemoveAsync(int roleId)
        {
            VerifyManagementPermission();
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role.IsAdmin)
            {
                throw new GraException("Cannot delete an admin role.");
            }
            await _roleRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), roleId);
        }

        public async Task<IEnumerable<string>> GetAllPermissionsAsync()
        {
            VerifyManagementPermission();
            return await _roleRepository.GetAllPermissionsAsync();
        }

        public async Task<IEnumerable<string>> GetPermissionsForRoleAsync(int roleId)
        {
            VerifyManagementPermission();
            return await _roleRepository.GetPermissionNamesForRoleAsync(roleId);
        }

        public async Task SyncPermissionsAsync()
        {
            var sites = await _siteRepository.GetAllAsync();
            if (sites.Count() > 0)
            {
                var permissionList = Enum.GetValues(typeof(Permission))
                    .Cast<Permission>()
                    .Select(_ => _.ToString());
                var currentPermissions = await _roleRepository.GetAllPermissionsAsync();

                var permissionsToAdd = permissionList.Except(currentPermissions);
                var permissionsToRemove = currentPermissions.Except(permissionList);

                if (permissionsToAdd.Count() > 0 || permissionsToRemove.Count() > 0)
                {
                    if (permissionsToAdd.Count() > 0)
                    {
                        await _roleRepository.AddPermissionListAsync(permissionsToAdd);
                    }
                    if (permissionsToRemove.Count() > 0)
                    {
                        _roleRepository.RemovePermissionList(permissionsToRemove);
                    }
                    await _roleRepository.SaveAsync();
                }
            }
        }
    }
}
