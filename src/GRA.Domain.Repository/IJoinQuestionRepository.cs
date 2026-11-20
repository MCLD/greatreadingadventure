using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IJoinQuestionRepository : IRepository<JoinQuestion>
    {
        public Task<IEnumerable<JoinQuestion>> GetQuestionsAsync();
    }
}
