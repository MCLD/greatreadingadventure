using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class AuthorizationCodeService : BaseUserService<AuthorizationCodeService>
    {
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        public AuthorizationCodeService(ILogger<AuthorizationCodeService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IAuthorizationCodeRepository authorizationCodeRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Model.Permission.ManageRoles);
            _authorizationCodeRepository = authorizationCodeRepository
                ?? throw new ArgumentNullException(nameof(authorizationCodeRepository));
        }

        public async Task<DataWithCount<IEnumerable<AuthorizationCode>>>
            GetPaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return await _authorizationCodeRepository.PageAsync(filter);
        }

        public async Task<AuthorizationCode> GetByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _authorizationCodeRepository.GetByIdAsync(id);
        }

        public async Task<AuthorizationCode> AddAsync(AuthorizationCode authorizationCode)
        {
            VerifyManagementPermission();
            var siteId = GetCurrentSiteId();

            var inUse = await _authorizationCodeRepository.GetByCodeAsync(siteId, authorizationCode.Code);
            if (inUse != null)
            {
                throw new GraException($"Code is already in use.");
            }

            authorizationCode.SiteId = siteId;

            return await _authorizationCodeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                authorizationCode);
        }

        public async Task UpdateAsync(AuthorizationCode authorizationCode)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentAuthorizationCode = await _authorizationCodeRepository.GetByIdAsync(
                authorizationCode.Id);
            if (currentAuthorizationCode.SiteId != siteId)
            {
                _logger.LogError($"User {authId} cannot update authorization code {currentAuthorizationCode.Id} for site {currentAuthorizationCode.SiteId}.");
                throw new GraException($"Permission denied - authorization code belongs to site id {currentAuthorizationCode.SiteId}.");
            }
            var existingCode = await _authorizationCodeRepository
                .GetByCodeAsync(siteId, authorizationCode.Code);
            if (existingCode != null && existingCode.Id != currentAuthorizationCode.Id)
            {
                throw new GraException($"Code is already in use.");
            }

            currentAuthorizationCode.Code = authorizationCode.Code;
            currentAuthorizationCode.Description = authorizationCode.Description;
            currentAuthorizationCode.IsSingleUse = authorizationCode.IsSingleUse;
            currentAuthorizationCode.RoleId = authorizationCode.RoleId;
            currentAuthorizationCode.ProgramId = authorizationCode.ProgramId;
            currentAuthorizationCode.SinglePageSignUp = authorizationCode.SinglePageSignUp;

            await _authorizationCodeRepository.UpdateSaveAsync(authId, currentAuthorizationCode);
        }

        public async Task RemoveAsync(int authorizationCodeId)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var authorizationCode = await _authorizationCodeRepository.GetByIdAsync(authorizationCodeId);
            if (authorizationCode.SiteId != siteId)
            {
                _logger.LogError($"User {authId} cannot delete authorization code {authorizationCodeId} for site {authorizationCode.SiteId}.");
                throw new GraException($"Permission denied - authorization code belongs to site id {authorizationCode.SiteId}.");
            }
            await _authorizationCodeRepository.RemoveSaveAsync(authId, authorizationCodeId);
        }

        public async Task<bool> ValidateAuthorizationCode(string authorizationCode)
        {
            string fixedCode = authorizationCode.Trim().ToLowerInvariant();
            int siteId = GetCurrentSiteId();
            var authCode = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);
            if (authCode == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> SinglePageSignUpCode(string authorizationCode)
        {
            if (authorizationCode == null) { return false; }

            string fixedCode = authorizationCode.Trim().ToLowerInvariant();
            int siteId = GetCurrentSiteId();
            var authCode = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);

            return authCode.SinglePageSignUp;
        }

        public async Task<int?> AssignedProgramFromCode(string authorizationCode)
        {
            if (authorizationCode == null) { return null; }

            string fixedCode = authorizationCode.Trim().ToLowerInvariant();
            int siteId = GetCurrentSiteId();
            var authCode = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);

            return authCode.ProgramId;
        }
    }
}
