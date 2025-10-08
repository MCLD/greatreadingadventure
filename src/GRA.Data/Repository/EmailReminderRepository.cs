using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
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

        public async Task<ICollection<EmailReminder>>
            GetAllListSubscribersAsync(string signUpSource)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SignUpSource == signUpSource)
                .OrderBy(_ => _.CreatedAt)
                .ProjectToType<EmailReminder>()
                .ToListAsync();
        }

        public async Task<IDictionary<string, Dictionary<int, int>>> GetEmailListsAsync(
            int defaultLanguageId)
        {
            var listStatuses = await DbSet.AsNoTracking()
                .GroupBy(_ => new { _.SignUpSource, _.LanguageId })
                .Select(_ => new
                {
                    _.Key.SignUpSource,
                    _.Key.LanguageId,
                    Count = _.Count()
                })
                .ToListAsync();

            var lists = new Dictionary<string, Dictionary<int, int>>();

            foreach (var signUpSource in listStatuses.Select(_ => _.SignUpSource).Distinct())
            {
                lists.Add(signUpSource, listStatuses
                    .Where(_ => _.SignUpSource == signUpSource && _.LanguageId.HasValue)
                    .ToDictionary(k => k.LanguageId.Value, v => v.Count));

                var noLanguageId = listStatuses
                    .Where(_ => _.SignUpSource == signUpSource && !_.LanguageId.HasValue)
                    .Sum(_ => _.Count);

                if (noLanguageId > 0)
                {
                    if (lists[signUpSource].ContainsKey(defaultLanguageId))
                    {
                        lists[signUpSource][defaultLanguageId] += noLanguageId;
                    }
                    else
                    {
                        lists[signUpSource].Add(defaultLanguageId, noLanguageId);
                    }
                }
            }

            return lists;
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
                .ProjectToType<EmailReminder>()
                .ToListAsync();
        }

        public async Task<int> GetListSubscribersCountAsync(string signUpSource)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SignUpSource == signUpSource && _.SentAt == null)
                .CountAsync();
        }

        public async Task<bool> IsAnyoneSubscribedAsync()
        {
            return await DbSet
                .AsNoTracking()
                .AnyAsync();
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
    }
}
