using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsKitImageRepository : IRepository<PsKitImage>
    {
        Task<List<PsKitImage>> GetByKitIdAsync(int kitId);
    }
}
