using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IChallengeTaskRepository : IRepository<Model.ChallengeTask>
    {
        Task AddChallengeTaskTypeAsync(int userId, string name);
        Task DecreasePositionAsync(int taskId);
        Task IncreasePositionAsync(int taskId);
    }
}
