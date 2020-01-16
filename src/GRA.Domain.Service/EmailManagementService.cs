using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class EmailManagementService : BaseUserService<EmailManagementService>
    {
        private readonly IEmailSubscriptionAuditLogRepository _emailSubscriptionAuditLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;

        public EmailManagementService(ILogger<EmailManagementService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IEmailSubscriptionAuditLogRepository emailSubscriptionAuditLogRepository,
            IUserRepository userRepository,
            IStringLocalizer<Resources.Shared> sharedLocalizer)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _emailSubscriptionAuditLogRepository = emailSubscriptionAuditLogRepository
                ?? throw new ArgumentNullException(nameof(emailSubscriptionAuditLogRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _sharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));
        }

        public async Task<ICollection<EmailSubscriptionAuditLog>> GetUserAuditLogAsync(int userId)
        {
            VerifyPermission(Permission.ViewParticipantDetails);
            return await _emailSubscriptionAuditLogRepository.GetUserAuditLogAsync(userId);
        }

        public async Task<bool> SetUserEmailSubscriptionStatusAsync(int userId, bool subscribe,
            bool newUser = false, int? headOfHouseholdId = null)
        {
            int auditId;

            if (newUser)
            {
                auditId = userId;
            }
            else
            {
                var authId = GetClaimId(ClaimType.UserId);
                var activeUserId = GetActiveUserId();
                if (userId != authId
                    && userId != activeUserId
                    && (headOfHouseholdId.HasValue && authId != headOfHouseholdId.Value)
                    && !HasPermission(Permission.EditParticipants))
                {
                    throw new GraException(_sharedLocalizer["Permission denied."]);
                }
                auditId = authId;
            }

            var user = await _userRepository.GetByIdAsync(userId);
            user.IsEmailSubscribed = subscribe;
            await _userRepository.UpdateAsync(auditId, user);
            await _emailSubscriptionAuditLogRepository.AddEntryAsync(auditId, userId, subscribe);
            await _userRepository.SaveAsync();

            return subscribe;
        }

        public async Task TokenUnsubscribe(string token)
        {
            var tokenUser = await _userRepository.GetByUnsubscribeToken(GetCurrentSiteId(),
                token?.Trim());

            if (tokenUser == null)
            {
                throw new GraException(_sharedLocalizer["The unsubscribe token could not be found, please log in to change your preferences."]);
            }
            else if (!tokenUser.IsEmailSubscribed)
            {
                throw new GraException(_sharedLocalizer["You are already unsubscribed from emails."]);
            }

            tokenUser.IsEmailSubscribed = false;
            await _userRepository.UpdateAsync(tokenUser.Id, tokenUser);
            await _emailSubscriptionAuditLogRepository.AddEntryAsync(tokenUser.Id, tokenUser.Id,
                false, true);

            var usersWithEmail = await _userRepository
                .GetUsersByEmailAddressAsync(tokenUser.Email?.Trim());

            foreach (var user in usersWithEmail)
            {
                if (user.Id != tokenUser.Id && user.IsEmailSubscribed)
                {
                    user.IsEmailSubscribed = false;
                    await _userRepository.UpdateAsync(tokenUser.Id, user);
                    await _emailSubscriptionAuditLogRepository.AddEntryAsync(tokenUser.Id,
                        user.Id, false, true);
                }
            }

            await _userRepository.SaveAsync();
        }
    }
}
