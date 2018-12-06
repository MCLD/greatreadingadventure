using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsPerformerImageRepository : IRepository<PsPerformerImage>
    {
        Task<List<PsPerformerImage>> GetByPerformerIdAsync(int performerId);
    }
}
