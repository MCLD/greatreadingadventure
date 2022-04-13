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
        private readonly GRA.Abstract.IGraCache _cache;
        private readonly IDirectEmailTemplateRepository _directEmailTemplateRepository;
        private readonly IEmailBaseRepository _emailBaseRepository;
        private readonly IEmailReminderRepository _emailReminderRepository;
        private readonly IEmailSubscriptionAuditLogRepository _emailSubscriptionAuditLogRepository;
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;
        private readonly IUserRepository _userRepository;

        public EmailManagementService(ILogger<EmailManagementService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            GRA.Abstract.IGraCache cache,
            IDirectEmailTemplateRepository directEmailTemplateRepository,
            IEmailBaseRepository emailBaseRepository,
            IEmailReminderRepository emailReminderRepository,
            IEmailSubscriptionAuditLogRepository emailSubscriptionAuditLogRepository,
            IStringLocalizer<Resources.Shared> sharedLocalizer,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _directEmailTemplateRepository = directEmailTemplateRepository
                ?? throw new ArgumentNullException(nameof(directEmailTemplateRepository));
            _emailBaseRepository = emailBaseRepository
                ?? throw new ArgumentNullException(nameof(emailBaseRepository));
            _emailReminderRepository = emailReminderRepository
                ?? throw new ArgumentNullException(nameof(emailReminderRepository));
            _emailSubscriptionAuditLogRepository = emailSubscriptionAuditLogRepository
                ?? throw new ArgumentNullException(nameof(emailSubscriptionAuditLogRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _sharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));
        }

        public async Task<int> AddBaseTemplateAsync(EmailBase emailBase)
        {
            if (emailBase == null)
            {
                throw new ArgumentNullException(nameof(emailBase));
            }

            return await _emailBaseRepository.AddSaveWithTextAsync(GetActiveUserId(), emailBase);
        }

        public async Task<int> AddTemplateAsync(DirectEmailTemplate directEmailTemplate)
        {
            if (directEmailTemplate == null)
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

        public async Task<EmailBase> GetBaseTemplateAsync(int emailBaseId, int languageId)
        {
            var baseTemplate = await _emailBaseRepository
                .GetWithTextAsync(emailBaseId, languageId);

            if (baseTemplate?.Id == null)
            {
                throw new GraException($"Unable to find template id {emailBaseId}");
            }

            return baseTemplate;
        }

        public async Task<IEnumerable<EmailBase>> GetEmailBasesAsync()
        {
            return await _emailBaseRepository.GetAllAsync();
        }

        public async Task<IDictionary<string, Dictionary<int, int>>>
            GetEmailListsAsync(int defaultLanguageId)
        {
            return await _emailReminderRepository.GetEmailListsAsync(defaultLanguageId);
        }

        public async Task<ICollectionWithCount<EmailBase>>
            GetPaginatedEmailBaseListAsync(BaseFilter filter)
        {
            if (HasPermission(Permission.ManageBulkEmails))
            {
                var emailBases = await _emailBaseRepository.PageAsync(filter);
                foreach (var emailBase in emailBases.Data)
                {
                    emailBase.ConfiguredLanguages
                        = await _emailBaseRepository.GetTextLanguagesAsync(emailBase.Id);
                }
                return emailBases;
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view the base email list.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
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

        public async Task<DirectEmailTemplate> GetTemplateStatusAsync(int templateId)
        {
            var template = await _directEmailTemplateRepository.GetByIdAsync(templateId);

            template.LanguageUnsub = await _directEmailTemplateRepository
                .GetLanguageUnsubAsync(templateId);

            return template;
        }

        public async Task<ICollection<EmailSubscriptionAuditLog>> GetUserAuditLogAsync(int userId)
        {
            VerifyPermission(Permission.ViewParticipantDetails);
            return await _emailSubscriptionAuditLogRepository.GetUserAuditLogAsync(userId);
        }

        public async Task<IDictionary<int, string>> GetUserTemplatesAsync()
        {
            return await _directEmailTemplateRepository.GetAllUserTemplatesAsync();
        }

        public async Task<bool> IsAnyoneSubscribedAsync()
        {
            return await _emailReminderRepository.IsAnyoneSubscribedAsync()
                || await _userRepository.IsAnyoneSubscribedAsync();
        }

        public async Task ReplaceBaseTextAsync(int emailBaseId,
            int languageId,
            EmailBaseText emailBaseText)
        {
            if (emailBaseText == null)
            {
                throw new ArgumentNullException(nameof(emailBaseText));
            }

            var currentBase = await _emailBaseRepository
                .GetWithTextByIdAsync(emailBaseId, languageId);

            if (currentBase == null)
            {
                throw new GraException("Unable to find that base template in the database.");
            }

            currentBase.EmailBaseText.TemplateHtml = emailBaseText.TemplateHtml;
            currentBase.EmailBaseText.TemplateMjml = emailBaseText.TemplateMjml;
            currentBase.EmailBaseText.TemplateText = emailBaseText.TemplateText;

            await _emailBaseRepository.UpdateSaveWithText(GetActiveUserId(), currentBase);
        }

        public async Task ReplaceTemplateTextAsync(int directEmailTemplateId,
            int languageId,
            DirectEmailTemplateText emailTemplateText)
        {
            if (emailTemplateText == null)
            {
                throw new ArgumentNullException(nameof(emailTemplateText));
            }

            var currentTemplate = await _directEmailTemplateRepository
                .GetWithTextByIdAsync(directEmailTemplateId, languageId);

            if (currentTemplate == null)
            {
                throw new GraException("Unable to find that template in the database.");
            }

            currentTemplate.DirectEmailTemplateText.BodyCommonMark
                = emailTemplateText.BodyCommonMark;
            currentTemplate.DirectEmailTemplateText.Footer = emailTemplateText.Footer;
            currentTemplate.DirectEmailTemplateText.Preview = emailTemplateText.Preview;
            currentTemplate.DirectEmailTemplateText.Subject = emailTemplateText.Subject;
            currentTemplate.DirectEmailTemplateText.Title = emailTemplateText.Title;

            await _directEmailTemplateRepository
                .UpdateSaveWithTextAsync(GetActiveUserId(), currentTemplate);
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

        public async Task UpdateBaseTemplateAsync(EmailBase emailBase)
        {
            await _emailBaseRepository
                .UpdateSaveWithText(GetActiveUserId(), emailBase);

            if (emailBase?.EmailBaseText?.LanguageId != null)
            {
                await _cache.RemoveAsync(GetCacheKey(CacheKey.EmailBase,
                    emailBase.Id,
                    emailBase.EmailBaseText.LanguageId));
            }
        }

        public async Task UpdateTemplateAsync(DirectEmailTemplate directEmailTemplate)
        {
            var updated = await _directEmailTemplateRepository
                .UpdateSaveWithTextAsync(GetActiveUserId(), directEmailTemplate);

            if (directEmailTemplate?.DirectEmailTemplateText?.LanguageId != null)
            {
                if (!string.IsNullOrEmpty(updated.SystemEmailId))
                {
                    await _cache.RemoveAsync(GetCacheKey(CacheKey.DirectEmailTemplateSystemId,
                        updated.SystemEmailId,
                        directEmailTemplate.DirectEmailTemplateText.LanguageId));
                }

                await _cache.RemoveAsync(GetCacheKey(CacheKey.DirectEmailTemplateId,
                    directEmailTemplate.Id,
                    directEmailTemplate.DirectEmailTemplateText.LanguageId));
            }
        }
    }
}
