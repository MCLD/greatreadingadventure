using System;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IJobRepository : IRepository<Model.Job>
    {
        Task<Job> GetJobInfoFromTokenAsync(Guid jobToken);
        Task UpdateStartAsync(int jobId);
        Task UpdateFinishAsync(int jobId, bool isCancelled);
        Task UpdateStatusAsync(int jobId, string status);
    }
}
