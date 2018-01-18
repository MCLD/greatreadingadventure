using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IPointTranslationRepository : IRepository<Model.PointTranslation>
    {
        Task<IEnumerable<PointTranslation>> GetAllAsync(int siteId);
        Task<ICollection<PointTranslation>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
        Task<bool> IsInUseAsync(int pointTranslationId);
        Task<Model.PointTranslation> GetByProgramIdAsync(int programId);
    }
}