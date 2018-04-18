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

        public async Task<DataWithCount<IEnumerable<Role>>> GetPaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();
            return await _roleRepository.PageAsync(filter);
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
