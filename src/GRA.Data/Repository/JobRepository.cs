using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class JobRepository : AuditingRepository<Model.Job, Domain.Model.Job>, IJobRepository
    {
        public JobRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<JobRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<Domain.Model.Job> GetJobInfoFromTokenAsync(Guid jobToken)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.JobToken == jobToken)
                .Select(_ => new Domain.Model.Job { Id = _.Id, JobType = _.JobType })
                .SingleOrDefaultAsync();
        }

        public async Task UpdateFinishAsync(int jobId, bool isCancelled)
        {
            var job = await DbSet.FindAsync(jobId);
            if (job != null)
            {
                if (isCancelled)
                {
                    job.StatusAsOf = _dateTimeProvider.Now;
                    job.Cancelled = _dateTimeProvider.Now;
                }
                else
                {
                    job.StatusAsOf = _dateTimeProvider.Now;
                    job.Completed = _dateTimeProvider.Now;
                }
                DbSet.Update(job);
                await SaveAsync();
            }
        }

        public async Task UpdateStartAsync(int jobId)
        {
            var job = await DbSet.FindAsync(jobId);
            if (job != null)
            {
                job.Status = "Starting...";
                job.StatusAsOf = job.Started = _dateTimeProvider.Now;
                DbSet.Update(job);
                await SaveAsync();
            }
        }

        public async Task UpdateStatusAsync(int jobId, string status)
        {
            var job = await DbSet.FindAsync(jobId);
            if (job != null)
            {
                job.Status = status;
                job.StatusAsOf = _dateTimeProvider.Now;
                DbSet.Update(job);
                await SaveAsync();
            }
        }
    }
}
