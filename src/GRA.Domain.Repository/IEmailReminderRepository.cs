using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IEmailReminderRepository : IRepository<EmailReminder>
    {
        Task<bool> ExistsEmailSourceAsync(string emailAddress, string signUpSource);
        Task<ICollection<EmailReminder>> GetAllEmailRemindersAsync();
        Task<ICollection<EmailReminder>> GetEmailRemindersBySignUpSource(string signUpSource);
    }
}
