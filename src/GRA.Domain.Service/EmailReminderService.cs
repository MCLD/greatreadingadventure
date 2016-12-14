using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;

namespace GRA.Domain.Service
{
    public class EmailReminderService : Abstract.BaseService<EmailReminderService>
    {
        private readonly IEmailReminderRepository _emailReminderRepository;
        public EmailReminderService(ILogger<EmailReminderService> logger,
            IEmailReminderRepository emailReminderRepository) : base(logger)
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
                    CreatedAt = DateTime.Now,
                    Email = email,
                    SignUpSource = signUpSource
                });
            }
        }
    }
}
