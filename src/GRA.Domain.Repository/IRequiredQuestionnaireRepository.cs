using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IRequiredQuestionnaireRepository : IRepository<RequiredQuestionnaire>
    {
        Task<ICollection<int>> GetForUser(int siteId, int userId, int? userAge);
        Task<bool> UserHasRequiredQuestionnaire(int siteId, int userId, int? userAge,
            int questionnaireId);
        Task SubmitQuestionnaire(int questionnaireId, int userId, IList<Question> questions);
    }
}
