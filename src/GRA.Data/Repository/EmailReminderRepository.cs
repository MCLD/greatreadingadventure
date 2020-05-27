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
        : AuditingRepository<Model.EmailReminder, EmailReminder>,
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

        public async Task<ICollection<DataWithCount<string>>> GetEmailListsAsync()
        {
            return await DbSet
                .AsNoTracking()
                .GroupBy(_ => _.SignUpSource)
                .Select(_ => new DataWithCount<string>
                {
                    Data = _.Key,
                    Count = _.Distinct().Count()
                })
                .ToListAsync();
        }

        public async Task<int> GetListSubscribersCountAsync(string signUpSource)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SignUpSource == signUpSource && _.SentAt == null)
                .CountAsync();
        }

        public async Task<ICollection<EmailReminder>> GetListSubscribersAsync(string signUpSource,
            int skip,
            int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SignUpSource == signUpSource && _.SentAt == null)
                .OrderBy(_ => _.CreatedAt)
                .Skip(skip)
                .Take(System.Math.Min(take, 100))
                .ProjectTo<EmailReminder>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task UpdateSentDateAsync(int emailReminderId)
        {
            var reminder = await DbSet.Where(_ => _.Id == emailReminderId).SingleOrDefaultAsync();
            if (reminder != null)
            {
                reminder.SentAt = _dateTimeProvider.Now;
                DbSet.Update(reminder);
            }
            else
            {
                throw new GraException($"Could not find email reminder ID {emailReminderId}");
            }
        }

        public async Task<ICollection<EmailReminder>>
            GetAllListSubscribersAsync(string signUpSource)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SignUpSource == signUpSource)
                .OrderBy(_ => _.CreatedAt)
                .ProjectTo<EmailReminder>()
                .ToListAsync();
        }
    }
}
