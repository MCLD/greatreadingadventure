using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface ISiteSettingRepository : IRepository<SiteSetting>
    {
        Task<ICollection<SiteSetting>> GetBySiteIdAsync(int siteId);
        Task AddListAsync(int userId, IEnumerable<SiteSetting> siteSettings);
        Task UpdateListAsync(int userId, IEnumerable<SiteSetting> siteSettings);
        Task RemoveListAsync(int userId, IEnumerable<int> siteSettingIds);
    }
}
