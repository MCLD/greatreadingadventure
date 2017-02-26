using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarLayerRepository : IRepository<Model.DynamicAvatarLayer>
    {
        Task<ICollection<int>> GetLayerIdsAsync();
    }
}
