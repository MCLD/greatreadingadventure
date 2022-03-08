using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class EmailManagementService : BaseUserService<EmailManagementService>
    {
        private readonly IDirectEmailTemplateRepository _directEmailTemplateRepository;
        private readonly IEmailBaseRepository _emailBaseRepository;
        private readonly IEmailReminderRepository _emailReminderRepository;
        private readonly IEmailSubscriptionAuditLogRepository _emailSubscriptionAuditLogRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;
        private readonly IUserRepository _userRepository;

        public EmailManagementService(ILogger<EmailManagementService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IDirectEmailTemplateRepository directEmailTemplateRepository,
            IEmailBaseRepository emailBaseRepository,
            IEmailReminderRepository emailReminderRepository,
            IEmailSubscriptionAuditLogRepository emailSubscriptionAuditLogRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IStringLocalizer<Resources.Shared> sharedLocalizer,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository,
            LanguageService languageService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _directEmailTemplateRepository = directEmailTemplateRepository
                ?? throw new ArgumentNullException(nameof(directEmailTemplateRepository));
            _emailBaseRepository = emailBaseRepository
                ?? throw new ArgumentNullException(nameof(emailBaseRepository));
            _emailReminderRepository = emailReminderRepository
                ?? throw new ArgumentNullException(nameof(emailReminderRepository));
            _emailSubscriptionAuditLogRepository = emailSubscriptionAuditLogRepository
                ?? throw new ArgumentNullException(nameof(emailSubscriptionAuditLogRepository));
            _emailTemplateRepository = emailTemplateRepository
                ?? throw new ArgumentNullException(nameof(emailTemplateRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _sharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));
        }

        public async Task<int> AddTemplateAsync(DirectEmailTemplate directEmailTemplate)
        {
            if(directEmailTemplate == null)
            {
                throw new ArgumentNullException(nameof(directEmailTemplate));
            }

            var baseExists = await _emailBaseRepository
                .GetByIdAsync(directEmailTemplate.EmailBaseId);

            if (baseExists == null)
            {
                throw new GraException($"Could not find base email template id {directEmailTemplate.EmailBaseId}");
            }

            return await _directEmailTemplateRepository
                .AddSaveWithTextAsync(GetActiveUserId(), directEmailTemplate);
        }

        public async Task UpdateTemplateAsync(DirectEmailTemplate directEmailTemplate)
        {
            await _directEmailTemplateRepository
                .UpdateSaveWithText(GetActiveUserId(), directEmailTemplate);
        }

        public async Task<DirectEmailTemplate> GetTemplateAsync(int templateId, int languageId)
        {
            var template = await _directEmailTemplateRepository
                .GetWithTextByIdAsync(templateId, languageId);

            if (template?.Id == null)
            {
                throw new GraException($"Unable to find template id {templateId}");
            }

            return template;
        }

        public async Task<IEnumerable<EmailBase>> GetEmailBasesAsync()
        {
            return await _emailBaseRepository.GetAllAsync();
        }

        public async Task<ICollection<DataWithCount<string>>> GetEmailListsAsync()
        {
            return await _emailReminderRepository.GetEmailListsAsync();
        }

        public async Task<ICollectionWithCount<EmailTemplateListItem>>
            GetPaginatedEmailTemplateListAsync(BaseFilter filter)
        {
            if (HasPermission(Permission.ManageBulkEmails))
            {
                return await _directEmailTemplateRepository.PageAsync(filter);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view the email template list.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
        }

        public async Task<int> GetSubscriberCount()
        {
            return await _userRepository.GetCountAsync(new UserFilter
            {
                SiteId = GetCurrentSiteId(),
                IsSubscribed = true
            });
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
                throw new GraException(_sharedLocalizer["An error occurred, please log in to change your email subscription preferences."]);
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
