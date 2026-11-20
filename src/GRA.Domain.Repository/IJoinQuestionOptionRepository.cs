using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IJoinQuestionOptionRepository : IRepository<JoinQuestionOption>
    {
        Task AddSaveUserJoinQuestionOptions(
            IEnumerable<UserJoinQuestionOption> userJoinQuestionOptions);
        Task<IEnumerable<JoinQuestionOption>> GetByJoinQuestionIdAsync(int joinQuestionId);
        Task<IEnumerable<int>> GetOptionsAsync(IEnumerable<int> joinQuestionIds);
        Task<IEnumerable<int>> GetSelectionsByUserIdAsync(int userId);
    }
}
