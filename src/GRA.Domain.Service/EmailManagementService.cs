using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class EmailManagementService : BaseUserService<EmailManagementService>
    {
        private readonly IEmailSubscriptionAuditLogRepository _emailSubscriptionAuditLogRepository;
        private readonly IUserRepository _userRepository;
        public EmailManagementService(ILogger<EmailManagementService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IEmailSubscriptionAuditLogRepository emailSubscriptionAuditLogRepository,
            IUserRepository userRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _emailSubscriptionAuditLogRepository = emailSubscriptionAuditLogRepository
                ?? throw new ArgumentNullException(nameof(emailSubscriptionAuditLogRepository));
            _userRepository = userRepository ?? throw new ArgumentException(nameof(userRepository));
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
                    throw new GraException("Permission denied.");
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
    }
}
