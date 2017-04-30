using GRA.Domain.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarElementRepository : IRepository<Model.DynamicAvatarElement>
    {
        Task<DynamicAvatarElement> GetByItemAndColorAsync(int item, int? color);
        Task<ICollection<DynamicAvatarElement>> GetUserAvatarAsync(int userId);
        Task SetUserAvatarAsync(int userId, List<int> elementIds);
    }
}
