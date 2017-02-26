using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class VendorCodeService : BaseUserService<VendorCodeService>
    {
        private readonly ICodeGenerator _codeGenerator;
        private readonly IVendorCodeRepository _vendorCodeRepository;
        private readonly IVendorCodeTypeRepository _vendorCodeTypeRepository;
        public VendorCodeService(ILogger<VendorCodeService> logger,
            IUserContextProvider userContextProvider,
            ICodeGenerator codeGenerator,
            IVendorCodeRepository vendorCodeRepository,
            IVendorCodeTypeRepository vendorCodeTypeRepository) : base(logger, userContextProvider)
        {
            SetManagementPermission(Permission.ManageVendorCodes);
            _codeGenerator = Require.IsNotNull(codeGenerator, nameof(codeGenerator));
            _vendorCodeRepository = Require.IsNotNull(vendorCodeRepository, nameof(vendorCodeRepository));
            _vendorCodeTypeRepository = Require.IsNotNull(vendorCodeTypeRepository, nameof(vendorCodeTypeRepository));
        }

        public async Task<ICollection<VendorCodeType>> GetTypeAllAsync()
        {
            VerifyManagementPermission();
            return await _vendorCodeTypeRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<DataWithCount<ICollection<VendorCodeType>>> GetTypePaginatedListAsync(Filter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<VendorCodeType>>
            {
                Data = await _vendorCodeTypeRepository.PageAsync(filter),
                Count = await _vendorCodeTypeRepository.CountAsync(filter)
            };
        }

        public async Task<VendorCodeType> AddTypeAsync(VendorCodeType vendorCodeType)
        {
            VerifyManagementPermission();
            vendorCodeType.SiteId = GetCurrentSiteId();
            return await _vendorCodeTypeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                vendorCodeType);
        }

        public async Task<VendorCodeType> UpdateTypeAsync(VendorCodeType vendorCodeType)
        {
            VerifyManagementPermission();
            vendorCodeType.SiteId = GetCurrentSiteId();
            return await _vendorCodeTypeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                vendorCodeType);
        }

        public async Task RemoveTypeAsync(int vendorCodeTypeId)
        {
            VerifyManagementPermission();
            await _vendorCodeTypeRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId),
                vendorCodeTypeId);
        }

        public async Task<int> GenerateVendorCodesAsync(int vendorCodeTypeId, int numberOfCodes)
        {
            VerifyManagementPermission();
            var codeType = await _vendorCodeTypeRepository.GetByIdAsync(vendorCodeTypeId);
            if (codeType == null)
            {
                throw new GraException("Unable to find vendor code type.");
            }
            if (codeType.SiteId != GetCurrentSiteId())
            {
                throw new GraException("Code type provided does not match current site.");
            }

            int count = 1;
            var vendorCode = new VendorCode
            {
                IsUsed = false,
                SiteId = codeType.SiteId,
                VendorCodeTypeId = codeType.Id,
            };
            for (; count <= System.Math.Min(numberOfCodes, 5000); count++)
            {
                vendorCode.Code = _codeGenerator.Generate(15);
                await _vendorCodeRepository.AddAsync(GetClaimId(ClaimType.UserId), vendorCode);
                if (count % 1000 == 0)
                {
                    await _vendorCodeRepository.SaveAsync();
                }
            }
            await _vendorCodeRepository.SaveAsync();

            return --count;
        }

        public async Task<string> GetUserVendorCodeAsync(int userId)
        {
            var authId = GetClaimId(ClaimType.UserId);
            if (userId == authId || HasPermission(Permission.ViewParticipantDetails))
            {
                return await _vendorCodeRepository.GetUserVendorCode(userId);
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to view details for {userId}.");
                throw new GraException("Permission denied.");
            }
        }
    }
}