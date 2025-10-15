using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IAvatarElementRepository : IRepository<Model.AvatarElement>
    {
        Task<AvatarElement> GetByItemAndColorAsync(int item, int? color);
        Task<ICollection<AvatarElement>> GetUserAvatarAsync(int userId, int languageId);
        Task SetUserAvatarAsync(int userId, List<int> elementIds);
        void RemoveByItemId(int id);
    }
}
