using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class EmailReminderRepository
        : AuditingRepository<Model.EmailReminder, Domain.Model.EmailReminder>,
        IEmailReminderRepository
    {
        public EmailReminderRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<EmailReminderRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<bool> ExistsEmailSourceAsync(string emailAddress, string signUpSource)
        {
            var lookup = await DbSet
                .AsNoTracking()
                .Where(_ => _.Email == emailAddress && _.SignUpSource == signUpSource)
                .SingleOrDefaultAsync();
            return lookup != null;
        }

        public async Task<ICollection<EmailReminder>> GetAllEmailRemindersAsync()
        {
            return await DbSet
                .AsNoTracking()
                .ProjectTo<EmailReminder>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ICollection<EmailReminder>> GetEmailRemindersBySignUpSource(string signUpSource)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SignUpSource == signUpSource)
                .ProjectTo<EmailReminder>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
