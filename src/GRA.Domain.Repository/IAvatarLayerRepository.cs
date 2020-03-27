using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IAvatarLayerRepository : IRepository<AvatarLayer>
    {
        Task<ICollection<AvatarLayer>> GetAllAsync(int siteId, int languageId);
        Task<ICollection<AvatarLayer>> GetAllWithColorsAsync(int siteId, int languageId);
    }
}
