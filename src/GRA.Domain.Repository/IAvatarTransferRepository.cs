using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IAvatarTransferRepository : IRepository<AvatarTransfer>
    {
        public Task<ICollection<AvatarTransfer>> GetAllAsync();
    }
}
