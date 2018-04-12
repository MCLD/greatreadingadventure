using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IAvatarColorRepository : IRepository<AvatarColor>
    {
        Task<ICollection<AvatarColor>> GetByLayerAsync(int layerId);
    }
}
