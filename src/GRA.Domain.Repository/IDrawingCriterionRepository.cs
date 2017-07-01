using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDrawingCriterionRepository : IRepository<DrawingCriterion>
    {
        Task<IEnumerable<DrawingCriterion>> PageAllAsync(BaseFilter filter);
        Task<int> GetCountAsync(BaseFilter filter);
        Task<int> GetEligibleUserCountAsync(int criterionId);
        Task<ICollection<int>> GetEligibleUserIdsAsync(int criterionId);
    }
}
