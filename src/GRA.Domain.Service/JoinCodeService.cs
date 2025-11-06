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
    public class JoinCodeService : BaseUserService<JoinCodeService>
    {
        private const string JoinCodeAllowedCharacters = "bcdfghjklmnpqrstvwxz1234567890";
        private const int JoinCodeLength = 8;

        private readonly ICodeGenerator _codeGenerator;
        private readonly IJoinCodeRepository _joinCodeRepository;

        public JoinCodeService(ILogger<JoinCodeService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            ICodeGenerator codeGenerator,
            IJoinCodeRepository joinCodeRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(joinCodeRepository);

            _codeGenerator = codeGenerator;
            _joinCodeRepository = joinCodeRepository;
        }

        public async Task<JoinCode> GetByCodeAndIncrementAccessCountAsync(string code)
        {
            var joinCode = await _joinCodeRepository.GetByCodeAsync(code);
            if (joinCode != null)
            {
                await _joinCodeRepository.IncrementAccessCountAsync(joinCode.Id);
            }
            else
            {
                _logger.LogWarning("Unable to find join code {code}",
                    code);
            }

            return joinCode;
        }

        public async Task<JoinCode> GetByTypeAndBranch(bool isQRCode, int? branchId)
        {
            VerifyPermission(Permission.ViewJoinCodes);

            var joinCode = await _joinCodeRepository.GetByTypeAndBranchAsync(isQRCode, branchId);

            if (joinCode == null)
            {
                VerifyPermission(Permission.CreateJoinCodes);
                await GenerateJoinCodeAsync(isQRCode, branchId);
                joinCode = await _joinCodeRepository.GetByTypeAndBranchAsync(isQRCode, branchId);
                joinCode.NewCode = true;
            }

            return joinCode;
        }

        public async Task IncrementJoinCountForCodeAsync(string code)
        {
            var joinCode = await _joinCodeRepository.GetByCodeAsync(code);
            if (joinCode != null)
            {
                await _joinCodeRepository.IncrementJoinCountAsync(joinCode.Id);
            }
            else
            {
                _logger.LogError("Unable to find join code {code} user completed registration with.",
                    code);
            }
        }

        public async Task<DataWithCount<IEnumerable<JoinCode>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            VerifyPermission(Permission.ViewJoinCodes);
            ArgumentNullException.ThrowIfNull(filter);

            filter.SiteId = GetCurrentSiteId();
            return await _joinCodeRepository.PageAsync(filter);
        }

        private async Task GenerateJoinCodeAsync(bool isQRCode, int? branchId)
        {
            _codeGenerator.SetAllowedCharacters(JoinCodeAllowedCharacters);

            string code;
            do
            {
                code = _codeGenerator.Generate(JoinCodeLength, false);
            }
            while (await _joinCodeRepository.CodeExistsAsync(code));

            var joinCode = new JoinCode
            {
                BranchId = branchId,
                Code = code,
                IsQRCode = isQRCode,
                SiteId = GetCurrentSiteId()
            };

            await _joinCodeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), joinCode);
        }
    }
}
