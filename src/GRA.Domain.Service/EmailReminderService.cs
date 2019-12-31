using System;
using System.Threading.Tasks;
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
            _emailReminderRepository = Require.IsNotNull(emailReminderRepository,
                nameof(emailReminderRepository));
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
    }
}
