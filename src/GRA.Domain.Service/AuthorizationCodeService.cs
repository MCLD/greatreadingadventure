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
            ArgumentNullException.ThrowIfNull(authorizationCodeRepository);

            _authorizationCodeRepository = authorizationCodeRepository;

            SetManagementPermission(Permission.ManageRoles);
        }

        public async Task<AuthorizationCode> AddAsync(AuthorizationCode authorizationCode)
        {
            VerifyManagementPermission();
            ArgumentNullException.ThrowIfNull(authorizationCode);
            var siteId = GetCurrentSiteId();

            var inUse = await _authorizationCodeRepository.GetByCodeAsync(siteId, authorizationCode.Code);
            if (inUse != null)
            {
                throw new GraException("Code is already in use.");
            }

            authorizationCode.SiteId = siteId;

            return await _authorizationCodeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                authorizationCode);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize all authorization codes to lowercase")]
        public async Task<int?> AssignedBranchFromCode(string authorizationCode)
        {
            if (authorizationCode == null) { return null; }

            string fixedCode = authorizationCode.Trim().ToLowerInvariant();
            int siteId = GetCurrentSiteId();
            var authCode = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);

            return authCode.BranchId;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize all authorization codes to lowercase")]
        public async Task<int?> AssignedProgramFromCode(string authorizationCode)
        {
            if (authorizationCode == null) { return null; }

            string fixedCode = authorizationCode.Trim().ToLowerInvariant();
            int siteId = GetCurrentSiteId();
            var authCode = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);

            return authCode.ProgramId;
        }

        public async Task<AuthorizationCode> GetByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _authorizationCodeRepository.GetByIdAsync(id);
        }

        public async Task<DataWithCount<IEnumerable<AuthorizationCode>>>
            GetPaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();
            ArgumentNullException.ThrowIfNull(filter);
            filter.SiteId = GetCurrentSiteId();
            return await _authorizationCodeRepository.PageAsync(filter);
        }

        public async Task RemoveAsync(int authorizationCodeId)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var authorizationCode = await _authorizationCodeRepository.GetByIdAsync(authorizationCodeId);
            if (authorizationCode.SiteId != siteId)
            {
                _logger.LogError("User {AuthId} cannot delete authorization code {AuthorizationCodeId} for site {SiteId}.",
                    authId,
                    authorizationCodeId,
                    authorizationCode.SiteId);
                throw new GraException($"Permission denied - authorization code belongs to site id {authorizationCode.SiteId}.");
            }
            await _authorizationCodeRepository.RemoveSaveAsync(authId, authorizationCodeId);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize all authorization codes to lowercase")]
        public async Task<bool> SinglePageSignUpCode(string authorizationCode)
        {
            if (authorizationCode == null) { return false; }

            string fixedCode = authorizationCode.Trim().ToLowerInvariant();
            int siteId = GetCurrentSiteId();
            var authCode = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);

            return authCode.SinglePageSignUp;
        }

        public async Task UpdateAsync(AuthorizationCode authorizationCode)
        {
            VerifyManagementPermission();
            ArgumentNullException.ThrowIfNull(authorizationCode);

            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentAuthorizationCode = await _authorizationCodeRepository.GetByIdAsync(
                authorizationCode.Id);
            if (currentAuthorizationCode.SiteId != siteId)
            {
                _logger.LogError("User {AuthId} cannot update authorization code {AuthCodeId} for site {SiteId}.",
                    authId,
                    currentAuthorizationCode.Id,
                    currentAuthorizationCode.SiteId);
                throw new GraException($"Permission denied - authorization code belongs to site id {currentAuthorizationCode.SiteId}.");
            }
            var existingCode = await _authorizationCodeRepository
                .GetByCodeAsync(siteId, authorizationCode.Code);
            if (existingCode != null && existingCode.Id != currentAuthorizationCode.Id)
            {
                throw new GraException("Code is already in use.");
            }

            currentAuthorizationCode.Code = authorizationCode.Code;
            currentAuthorizationCode.Description = authorizationCode.Description;
            currentAuthorizationCode.IsSingleUse = authorizationCode.IsSingleUse;
            currentAuthorizationCode.RoleId = authorizationCode.RoleId;
            currentAuthorizationCode.ProgramId = authorizationCode.ProgramId;
            currentAuthorizationCode.BranchId = authorizationCode.BranchId;
            currentAuthorizationCode.SinglePageSignUp = authorizationCode.SinglePageSignUp;

            await _authorizationCodeRepository.UpdateSaveAsync(authId, currentAuthorizationCode);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize all authorization codes to lowercase")]
        public async Task<bool> ValidateAuthorizationCode(string authorizationCode)
        {
            ArgumentNullException.ThrowIfNull(authorizationCode);
            string fixedCode = authorizationCode.Trim().ToLowerInvariant();
            int siteId = GetCurrentSiteId();
            var authCode = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);
            return authCode != null;
        }
    }
}
