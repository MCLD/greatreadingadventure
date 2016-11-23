using GRA.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IChallengeTaskRepository : IRepository<Model.ChallengeTask>
    {
        void AddChallengeTaskType(int userId, string name);
        void DecreasePosition(int taskId);
        void IncreasePosition(int taskId);
    }
}
