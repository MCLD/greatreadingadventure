using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IChallengeTaskRepository : IRepository<Model.ChallengeTask>
    {
        Task AddChallengeTaskTypeAsync(int userId,
            string name,
            int? activityCount = default(int?),
            int? pointTranslationId = default(int?));
        Task DecreasePositionAsync(int taskId);
        Task IncreasePositionAsync(int taskId);
    }
}
