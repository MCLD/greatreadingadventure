using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IEmailReminderRepository : IRepository<EmailReminder>
    {
        Task<bool> ExistsEmailSourceAsync(string emailAddress, string signUpSource);

        Task<ICollection<EmailReminder>> GetAllListSubscribersAsync(string signUpSource);

        Task<IDictionary<string, Dictionary<int, int>>> GetEmailListsAsync(int defaultLanguageId);

        Task<ICollection<EmailReminder>> GetListSubscribersAsync(string signUpSource,
            int skip,
            int take);

        Task<int> GetListSubscribersCountAsync(string signUpSource);

        Task<bool> IsAnyoneSubscribedAsync();

        Task UpdateSentDateAsync(int emailReminderId);
    }
}
