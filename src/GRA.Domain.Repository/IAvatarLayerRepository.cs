using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IAvatarLayerRepository : IRepository<AvatarLayer>
    {
        Task<ICollection<AvatarLayer>> GetAllAsync(int siteId);
        Task<ICollection<AvatarLayer>> GetAllWithColorsAsync(int siteId, int userId);
    }
}
