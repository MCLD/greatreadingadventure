using GRA.Domain.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GRA.Domain.Repository
{
    public interface IAvatarElementRepository : IRepository<Model.AvatarElement>
    {
        Task<AvatarElement> GetByItemAndColorAsync(int item, int? color);
        Task<ICollection<AvatarElement>> GetUserAvatarAsync(int userId);
        Task SetUserAvatarAsync(int userId, List<int> elementIds);
        void RemoveByItemId(int id);
        Task<AvatarElement> GetRandomColorByItemAsync(int item);
    }
}
