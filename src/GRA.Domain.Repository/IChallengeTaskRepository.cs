namespace GRA.Domain.Repository
{
    public interface IChallengeTaskRepository : IRepository<Model.ChallengeTask>
    {
        void AddChallengeTaskType(int userId, string name);
        void DecreasePosition(int taskId);
        void IncreasePosition(int taskId);
    }
}
