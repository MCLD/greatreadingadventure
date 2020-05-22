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

        public async Task<ICollection<EmailReminder>> GetListSubscribersAsync(string signUpSource)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SignUpSource == signUpSource)
                .ProjectTo<EmailReminder>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
