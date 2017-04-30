using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarItemRepository : IRepository<DynamicAvatarItem>
    {
        Task<ICollection<DynamicAvatarItem>> GetUserItemsByLayerAsync(int userId, int layerId);
    }
}
