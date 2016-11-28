using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IMailRepository : IRepository<Mail>
    {
        Task<int> GetAllCountAsync();
        Task<int> GetAdminUnreadCountAsync();
        Task<ICollection<Mail>> PageAdminUnreadAsync(int skip, int take);
        Task<int> GetUserCountAsync(int userId);
        Task<ICollection<Mail>> PageUserAsync(int userId, int skip, int take);
        Task<int> GetUserUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int mailId);
    }
}
