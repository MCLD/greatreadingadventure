using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsProgramImageRepository : IRepository<PsProgramImage>
    {
        Task<List<PsProgramImage>> GetByProgramIdAsync(int programId);
    }
}
