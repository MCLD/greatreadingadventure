using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class EmailReminderService : Abstract.BaseService<EmailReminderService>
    {
        private readonly IEmailReminderRepository _emailReminderRepository;
        public EmailReminderService(ILogger<EmailReminderService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IEmailReminderRepository emailReminderRepository) : base(logger, dateTimeProvider)
        {
            _emailReminderRepository = emailReminderRepository 
                ?? throw new ArgumentNullException(nameof(emailReminderRepository));
        }

        public async Task AddEmailReminderAsync(string email, string signUpSource)
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
                    SignUpSource = signUpSource
                });
            }
        }

        public async Task<bool> ImportEmailToListAsync(int userId,
            Model.EmailReminder emailReminder)
        {
            if (string.IsNullOrEmpty(emailReminder.Email))
            {
                throw new ArgumentNullException(nameof(emailReminder.Email));
            }
            if (string.IsNullOrEmpty(emailReminder.SignUpSource))
            {
                throw new ArgumentNullException(nameof(emailReminder.SignUpSource));
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
                        SignUpSource = emailReminder.SignUpSource
                    });
                return true;
            }
            return false;
        }

        public async Task SaveImportAsync()
        {
            await _emailReminderRepository.SaveAsync();
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

        public async Task<ICollection<EmailReminder>>
            GetSubscribersAsync(EmailReminderFilter filter)
        {
            return await _emailReminderRepository
                    .GetListSubscribersAsync(filter.MailingList,
                filter.Skip ?? 0,
                filter.Take ?? 30);
        }

        public async Task UpdateSentDateAsync(int emailReminderId)
        {
            await _emailReminderRepository.UpdateSentDateAsync(emailReminderId);
        }

        public async Task SaveAsync()
        {
            await _emailReminderRepository.SaveAsync();
        }
        public async Task<IEnumerable> GetAllSubscribersAsync(string signUpSource)
        {
            var subscribers = await _emailReminderRepository
                .GetAllListSubscribersAsync(signUpSource);
            return subscribers.Select(_ => new
            {
                _.SignUpSource,
                _.Email,
                _.CreatedAt
            });
        }
    }
}
