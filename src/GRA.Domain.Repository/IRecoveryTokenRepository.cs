using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IRecoveryTokenRepository : IRepository<RecoveryToken>
    {
        Task<IEnumerable<RecoveryToken>> GetByUserIdAsync(int userId);
    }
}
