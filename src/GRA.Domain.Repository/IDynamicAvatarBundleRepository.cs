using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarBundleRepository : IRepository<DynamicAvatarBundle>
    {
        Task AddItemAsync(int bundleId, int itemId);
        Task<ICollection<DynamicAvatarItem>> GetRandomDefaultBundleAsync(int siteId);
    }
}
