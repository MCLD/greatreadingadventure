using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IStaticAvatarRepository : IRepository<StaticAvatar>
    {
        Task<IEnumerable<StaticAvatar>> GetAvartarListAsync(int siteId);
        Task<StaticAvatar> GetByIdAsync(int siteId, int id);
    }
}
