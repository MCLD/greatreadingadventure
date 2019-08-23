﻿using System;
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
                var now = _dateTimeProvider.Now;
                if (isCancelled)
                {
                    job.StatusAsOf = now;
                    job.Cancelled = now;
                }
                else
                {
                    job.StatusAsOf = now;
                    job.Completed = now;
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
                var now = _dateTimeProvider.Now;
                job.Status = "Starting...";
                job.StatusAsOf = now;
                job.Started = now;
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
