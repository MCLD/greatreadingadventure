using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class GroupTypeService : BaseUserService<GroupTypeService>
    {
        private readonly IGroupTypeRepository _groupTypeRepository;
        private readonly IGroupInfoRepository _groupInfoRepository;
        public GroupTypeService(ILogger<GroupTypeService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IGroupInfoRepository groupInfoRepository,
            IGroupTypeRepository groupTypeRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _groupInfoRepository = groupInfoRepository
                ?? throw new ArgumentNullException(nameof(groupInfoRepository));
            _groupTypeRepository = groupTypeRepository
                ?? throw new ArgumentNullException(nameof(groupTypeRepository));
            SetManagementPermission(Permission.ManageGroupTypes);
        }

        public async Task<DataWithCount<IEnumerable<GroupType>>> GetAllMCPagedAsync(int skip,
            int take)
        {
            VerifyManagementPermission();
            int siteId = GetCurrentSiteId();
            var (list, count) = await _groupTypeRepository.GetAllPagedAsync(siteId, skip, take);
            return new DataWithCount<IEnumerable<GroupType>>
            {
                Count = count,
                Data = list
            };
        }

        public async Task<(bool, string)> Add(int currentUserId, string groupTypeName)
        {
            VerifyManagementPermission();
            if (string.IsNullOrEmpty(groupTypeName)
                || string.IsNullOrEmpty(groupTypeName.Trim()))
            {
                throw new GraException("Please supply a group type name to add.");
            }
            try
            {
                await _groupTypeRepository.AddSaveAsync(currentUserId, new GroupType
                {
                    SiteId = GetCurrentSiteId(),
                    Name = groupTypeName.Trim()
                });
                return (true, groupTypeName.Trim());
            }
            catch (Exception ex)
            {
                return (false, $"Unable to add group type: {ex.Message}");
            }
        }

        public async Task<string> Remove(int currentUserId, int groupTypeId)
        {
            VerifyManagementPermission();

            int usingThisType = 0;
            try
            {
                usingThisType = await _groupInfoRepository.GetCountByTypeAsync(groupTypeId);
            }
            catch(Exception ex)
            {
                return $"Unable to remove group type - cannot tell if any group(s) are using it: {ex.Message}";
            }

            if(usingThisType > 0)
            {
                return $"Unable to remove group type - {usingThisType} group(s) currently have it selected.";
            }

            try
            {
                await _groupTypeRepository.RemoveSaveAsync(currentUserId, groupTypeId);
                return null;
            }
            catch (Exception ex)
            {
                return $"Unable to remove group type: {ex.Message}";
            }
        }

        public async Task<(bool, string)> Edit(int currentUserId, int groupTypeId, string groupTypeName)
        {
            VerifyManagementPermission();
            if (string.IsNullOrEmpty(groupTypeName)
                || string.IsNullOrEmpty(groupTypeName.Trim()))
            {
                throw new GraException("Group types must have a name.");
            }
            try
            {
                var groupType = await _groupTypeRepository.GetByIdAsync(groupTypeId);
                groupType.Name = groupTypeName.Trim();
                await _groupTypeRepository.UpdateSaveAsync(currentUserId, groupType);
                return (true, groupTypeName.Trim());
            }
            catch (Exception ex)
            {
                return (false, $"Unable to edit group type: {ex.Message}");
            }
        }

    }
}
