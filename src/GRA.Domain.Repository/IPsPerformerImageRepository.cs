using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsPerformerImageRepository : IRepository<PsPerformerImage>
    {
        Task<ICollection<PsPerformerImage>> GetByPerformerIdAsync(int performerId);
    }
}
