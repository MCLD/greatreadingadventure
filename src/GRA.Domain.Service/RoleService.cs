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
        private readonly SiteLookupService _siteLookupService;
        private readonly ISiteRepository _siteRepository;

        public RoleService(ILogger<RoleService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IRoleRepository roleRepository,
            ISiteRepository siteRepository,
            SiteLookupService siteLookupService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageRoles);
            _roleRepository = roleRepository
                ?? throw new ArgumentNullException(nameof(roleRepository));
            _siteRepository = siteRepository
                ?? throw new ArgumentNullException(nameof(siteRepository));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
        }

        public async Task<Role> AddAsync(Role role, IEnumerable<string> permissions)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            role.Name = role.Name.Trim();
            role.IsAdmin = false;

            if (permissions.Any())
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
            if (!currentRole.IsAdmin)
            {
                var currentPermissions = await _roleRepository
                    .GetPermissionNamesForRoleAsync(currentRole.Id);

                permissionsToAdd = permissions.Except(currentPermissions).ToList();
                permissionsToRemove = currentPermissions.Except(permissions).ToList();
            }
            await _roleRepository.UpdateSaveAsync(authId, currentRole, permissionsToAdd, permissionsToRemove);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            VerifyManagementPermission();
            return await _roleRepository.GetAllAsync();
        }

        public async Task<IEnumerable<string>> GetAllPermissionsAsync()
        {
            VerifyManagementPermission();
            return await _roleRepository.GetAllPermissionsAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _roleRepository.GetByIdAsync(id);
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

        public async Task<IEnumerable<string>> GetPermissionsForRoleAsync(int roleId)
        {
            VerifyManagementPermission();
            return await _roleRepository.GetPermissionNamesForRoleAsync(roleId);
        }

        public async Task<IDictionary<int, int>>
            GetUserCountForRolesAsync(IEnumerable<int> roleIds)
        {
            VerifyManagementPermission();
            return await _roleRepository.GetUserCountForRolesAsync(roleIds);
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

        public async Task SyncPermissionsAsync()
        {
            var sites = await _siteRepository.GetAllAsync();
            if (sites.Any())
            {
                var adminRole = await _roleRepository.GetByIdAsync(1);
                if (adminRole != null)
                {
                    if (!adminRole.IsAdmin)
                    {
                        _logger.LogError("Role ID 1 is not set as IsAdmin, fixing.");
                        adminRole.IsAdmin = true;
                        await _roleRepository.UpdateSaveAsync(-1, adminRole);
                    }
                }
                else
                {
                    _logger.LogError("Cannot find role ID 1 in order to ensure it is marked as IsAdmin");
                }

                var permissionList = Enum.GetValues(typeof(Permission))
                    .Cast<Permission>()
                    .Select(_ => _.ToString());
                var currentPermissions = await _roleRepository.GetAllPermissionsAsync();

                var permissionsToAdd = permissionList.Except(currentPermissions);
                var permissionsToRemove = currentPermissions.Except(permissionList);

                if (permissionsToAdd.Any() || permissionsToRemove.Any())
                {
                    if (permissionsToAdd.Any())
                    {
                        var userId = await _siteLookupService.GetSystemUserId();
                        await _roleRepository.AddPermissionListAsync(userId, permissionsToAdd);
                    }
                    if (permissionsToRemove.Any())
                    {
                        _roleRepository.RemovePermissionList(permissionsToRemove);
                    }
                    await _roleRepository.SaveAsync();
                }
            }
        }
    }
}