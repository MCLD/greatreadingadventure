using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IAnswerRepository : IRepository<Answer>
    {
        Task<ICollection<Answer>> GetByQuestionIdAsync(int questionId);
        new Task<Answer> AddSaveAsync(int userId, Answer answer);
    }
}
