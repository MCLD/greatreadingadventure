using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class EmailReminderService : Abstract.BaseService<EmailReminderService>
    {
        private readonly IEmailReminderRepository _emailReminderRepository;
        private readonly LanguageService _languageService;

        public EmailReminderService(ILogger<EmailReminderService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IEmailReminderRepository emailReminderRepository,
            LanguageService languageService) : base(logger, dateTimeProvider)
        {
            _emailReminderRepository = emailReminderRepository
                ?? throw new ArgumentNullException(nameof(emailReminderRepository));
            _languageService = languageService 
                ?? throw new ArgumentNullException(nameof(languageService));
        }

        public async Task AddEmailReminderAsync(string email,
            string signUpSource,
            int languageId)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }
            if (string.IsNullOrEmpty(signUpSource))
            {
                throw new ArgumentNullException(nameof(signUpSource));
            }
            var alreadySubscribed
                = await _emailReminderRepository.ExistsEmailSourceAsync(email, signUpSource);
            if (!alreadySubscribed)
            {
                await _emailReminderRepository.AddSaveNoAuditAsync(new Model.EmailReminder
                {
                    CreatedAt = _dateTimeProvider.Now,
                    Email = email,
                    LanguageId = languageId,
                    SignUpSource = signUpSource
                });
            }
        }

        public async Task<IEnumerable<EmailReminderExport>> ExportSubscribersAsync(string signUpSource)
        {
            var subscribers = await _emailReminderRepository
                .GetAllListSubscribersAsync(signUpSource);

            var languages = (await _languageService.GetActiveAsync())
                .ToDictionary(k => k.Id, v => v.Name);

            return subscribers.Select(_ => new
            EmailReminderExport
            {
                CreatedAt = _.CreatedAt,
                Email = _.Email,
                LanguageName = _.LanguageId.HasValue ? languages[_.LanguageId.Value] : null,
                SignUpSource = _.SignUpSource
            }); ;
        }

        public async Task<ICollection<EmailReminder>>
            GetSubscribersAsync(EmailReminderFilter filter)
        {
            return await _emailReminderRepository
                    .GetListSubscribersAsync(filter.MailingList,
                filter.Skip ?? 0,
                filter.Take ?? 30);
        }

        public async Task<DataWithCount<ICollection<EmailReminder>>>
            GetSubscribersWithCountAsync(EmailReminderFilter filter)
        {
            return new DataWithCount<ICollection<EmailReminder>>
            {
                Count = await _emailReminderRepository
                    .GetListSubscribersCountAsync(filter.MailingList),
                Data = await GetSubscribersAsync(filter)
            };
        }

        public async Task<bool> ImportEmailToListAsync(int userId, EmailReminder emailReminder)
        {
            if(emailReminder == null)
            {
                throw new ArgumentNullException(nameof(emailReminder));
            }

            var alreadySubscribed = await _emailReminderRepository
                .ExistsEmailSourceAsync(emailReminder.Email, emailReminder.SignUpSource);
            if (!alreadySubscribed)
            {
                await _emailReminderRepository.AddAsync(userId,
                    new EmailReminder
                    {
                        CreatedAt = emailReminder.CreatedAt,
                        Email = emailReminder.Email,
                        LanguageId = emailReminder.LanguageId,
                        SignUpSource = emailReminder.SignUpSource
                    });
                return true;
            }
            return false;
        }

        public async Task SaveAsync()
        {
            await _emailReminderRepository.SaveAsync();
        }

        public async Task SaveImportAsync()
        {
            await _emailReminderRepository.SaveAsync();
        }

        public async Task UpdateSentDateAsync(int emailReminderId)
        {
            await _emailReminderRepository.UpdateSentDateAsync(emailReminderId);
        }
    }
}
