using GRA.Domain.Model;
using System.Linq;

namespace GRA.Domain.Repository
{
    public interface IChallengeRepository : IRepository<Challenge>
    {
        void AddChallengeTaskType(int userId, string name);
        int GetChallengeCount();
    }
}
