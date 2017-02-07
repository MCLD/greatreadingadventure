using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class TriggerService : BaseUserService<TriggerService>
    {
        private readonly ITriggerRepository _triggerRepository;
        public TriggerService(ILogger<TriggerService> logger,
            IUserContextProvider userContextProvider,
            ITriggerRepository triggerRepository) : base(logger, userContextProvider)
        {
            SetManagementPermission(Permission.ManageTriggers);
            _triggerRepository = Require.IsNotNull(triggerRepository, nameof(triggerRepository));
        }

        public async Task<DataWithCount<ICollection<Trigger>>> GetPaginatedListAsync(Filter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Trigger>>
            {
                Data = await _triggerRepository.PageAsync(filter),
                Count = await _triggerRepository.CountAsync(filter)
            };
        }

        public async Task<Trigger> AddAsync(Trigger trigger)
        {
            VerifyManagementPermission();
            trigger.SiteId = GetCurrentSiteId();
            trigger.RelatedBranchId = GetClaimId(ClaimType.BranchId);
            trigger.RelatedSystemId = GetClaimId(ClaimType.SystemId);
            return await _triggerRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                trigger);
        }

        public async Task<Trigger> UpdateAsync(Trigger trigger)
        {
            VerifyManagementPermission();
            trigger.SiteId = GetCurrentSiteId();
            return await _triggerRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                trigger);
        }

        public async Task RemoveAsync(int triggerId)
        {
            VerifyManagementPermission();
            await _triggerRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId),
                triggerId);
        }
    }
}
