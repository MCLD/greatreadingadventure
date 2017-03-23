using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarRepository : IRepository<DynamicAvatar>
    {
        Task<ICollection<DynamicAvatar>> GetPaginatedAvatarListAsync(
            int siteId,
            int skip,
            int take,
            string search = default(string));

        new Task<DynamicAvatar> GetByIdAsync(int id);
    }
}
