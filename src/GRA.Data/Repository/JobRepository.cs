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
    }
}
