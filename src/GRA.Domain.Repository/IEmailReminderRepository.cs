using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IEmailReminderRepository : IRepository<EmailReminder>
    {
        Task<bool> ExistsEmailSourceAsync(string emailAddress, string signUpSource);
        Task<ICollection<DataWithCount<string>>> GetEmailListsAsync();
        Task<ICollection<EmailReminder>> GetListSubscribersAsync(string signUpSource);
    }
}
