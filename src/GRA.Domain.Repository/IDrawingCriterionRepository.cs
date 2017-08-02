using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDrawingCriterionRepository : IRepository<DrawingCriterion>
    {
        new Task<DrawingCriterion> GetByIdAsync(int Id);
        new Task<DrawingCriterion> AddSaveAsync(int userId, DrawingCriterion criterion);
        new Task<DrawingCriterion> UpdateSaveAsync(int userId, DrawingCriterion criterion);
        Task<IEnumerable<DrawingCriterion>> PageAllAsync(BaseFilter filter);
        Task<int> GetCountAsync(BaseFilter filter);
        Task<int> GetEligibleUserCountAsync(int criterionId);
        Task<ICollection<int>> GetEligibleUserIdsAsync(int criterionId);
    }
}
