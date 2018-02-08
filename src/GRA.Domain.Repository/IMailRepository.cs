using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IMailRepository : IRepository<Mail>
    {
        Task<int> GetAllCountAsync(int siteId);
        Task<int> GetAdminUnrepliedCountAsync(int siteId);
        Task<IEnumerable<Mail>> PageAdminUnrepliedAsync(int siteId, int skip, int take);
        Task MarkAdminReplied(int mailId);
        Task<int> GetUserCountAsync(int userId);
        Task<int> GetUserInboxCountAsync(int userId);
        Task<IEnumerable<Mail>> PageAllAsync(int siteId, int skip, int take);
        Task<IEnumerable<Mail>> PageUserAsync(int userId, int skip, int take);
        Task<IEnumerable<Mail>> PageUserInboxAsync(int userId, int skip, int take);
        Task<int> GetUserUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int mailId);
        Task<bool> UserHasUnreadAsync(int userId);
        Task<List<Mail>> GetThreadAsync(int threadId);
    }
}
