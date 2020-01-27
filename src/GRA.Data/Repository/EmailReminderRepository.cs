using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public IEnumerable GetDistinctSignUpSourceWithCount()
        {
            return DbSet
                .GroupBy(_ => _.SignUpSource)
                .Select(_ => new
                {
                    DisplayText = _.Key + " (" + _.Distinct().Count().ToString() + ")",
                    Value = _.Key
                });
        }

        public ICollection<EmailReminder> GetEmailsBySignUpSource(string signUpSource)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.SignUpSource == signUpSource)
                .Select(_ => new EmailReminder
                {
                    SignUpSource = _.SignUpSource,
                    Email = _.Email,
                    CreatedAt = _.CreatedAt
                })
                .ToList();
        }
    }
}
