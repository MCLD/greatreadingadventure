using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface ISiteSettingRepository : IRepository<SiteSetting>
    {
        Task<ICollection<SiteSetting>> GetBySiteIdAsync(int siteId);
    }
}
