using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsPerformerRepository
        : AuditingRepository<Model.PsPerformer, Domain.Model.PsPerformer>, IPsPerformerRepository
    {
        public PsPerformerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsPerformerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<PsPerformer> GetByUserIdAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .ProjectTo<PsPerformer>()
                .SingleOrDefaultAsync();
        }
    }
}
