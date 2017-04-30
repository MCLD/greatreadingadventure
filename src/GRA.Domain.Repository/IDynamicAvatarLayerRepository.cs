using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarLayerRepository : IRepository<DynamicAvatarLayer>
    {
        Task<ICollection<DynamicAvatarLayer>> GetAllAsync(int siteId);
        Task<ICollection<DynamicAvatarLayer>> GetAllWithColorsAsync(int siteId, int userId);
    }
}
