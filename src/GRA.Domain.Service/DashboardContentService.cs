using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class DashboardContentService : Abstract.BaseUserService<DashboardContentService>
    {
        private readonly IDashboardContentRepository _dashboardContentRepository;
        public DashboardContentService(ILogger<DashboardContentService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IDashboardContentRepository dashboardContentRepository) 
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageDashboardContent);
            _dashboardContentRepository = dashboardContentRepository
                ?? throw new ArgumentNullException(nameof(dashboardContentRepository));
        }

        public async Task<DataWithCount<IEnumerable<DashboardContent>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId =  GetClaimId(ClaimType.SiteId);
            return new DataWithCount<IEnumerable<DashboardContent>>
            {
                Data = await _dashboardContentRepository.PageAsync(filter),
                Count = await _dashboardContentRepository.CountAsync(filter)
            };
        }
        
        public async Task<DashboardContent> GetByIdAsync(int id)
        {
            return await _dashboardContentRepository.GetByIdAsync(id);
        }

        public async Task<DashboardContent> GetCurrentContentAsync()
        {
            return await _dashboardContentRepository.GetCurrentAsync(GetClaimId(ClaimType.SiteId));
        }

        public async Task<DashboardContent> AddAsync(DashboardContent dashboardContent)
        {
            VerifyManagementPermission();
            dashboardContent.SiteId = GetCurrentSiteId();
            return await _dashboardContentRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), 
                dashboardContent);
        }

        public async Task<DashboardContent> EditAsync(DashboardContent dashboardContent)
        {
            VerifyManagementPermission();
            var current = await _dashboardContentRepository.GetByIdAsync(dashboardContent.Id);
            dashboardContent.SiteId = current.SiteId;
            return await _dashboardContentRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                dashboardContent);
        }

        public async Task RemoveAsync(int dashboardContentId)
        {
            VerifyManagementPermission();
            await _dashboardContentRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), 
                dashboardContentId);
        }
    }
}
