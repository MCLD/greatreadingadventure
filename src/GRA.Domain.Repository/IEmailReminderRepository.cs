using GRA.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IEmailReminderRepository : IRepository<EmailReminder>
    {
        Task<bool> ExistsEmailSourceAsync(string emailAddress, string signUpSource);
    }
}
