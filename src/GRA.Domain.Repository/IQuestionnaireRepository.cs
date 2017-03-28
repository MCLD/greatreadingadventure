using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IQuestionnaireRepository : IRepository<Questionnaire>
    {
        Task<int> CountAsync(BaseFilter filter);
        Task<ICollection<Questionnaire>> PageAsync(BaseFilter filter);
        Task<Questionnaire> GetByIdAsync(int id, bool includeAnswers);
        new Task RemoveSaveAsync(int userId, int id);
    }
}
