using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsPerformerRepository : IRepository<PsPerformer>
    {
        Task<PsPerformer> GetByUserIdAsync(int userId);
    }
}
