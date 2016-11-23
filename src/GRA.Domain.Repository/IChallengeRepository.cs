using GRA.Domain.Model;
using System.Linq;

namespace GRA.Domain.Repository
{
    public interface IChallengeRepository : IRepository<Challenge>
    {
        int GetChallengeCount();
    }
}
