using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IRecoveryTokenRepository : IRepository<RecoveryToken>
    {
        Task<IEnumerable<RecoveryToken>> GetByUserIdAsync(int userId);
    }
}
