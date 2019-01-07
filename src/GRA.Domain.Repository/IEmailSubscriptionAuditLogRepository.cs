using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IEmailSubscriptionAuditLogRepository
    {
        Task<ICollection<EmailSubscriptionAuditLog>> GetUserAuditLogAsync(int userId);
        Task AddEntryAsync(int auditId, int userId, bool subscribe);
    }
}
