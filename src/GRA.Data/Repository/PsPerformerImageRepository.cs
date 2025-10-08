using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsPerformerImageRepository
        : AuditingRepository<Model.PsPerformerImage, Domain.Model.PsPerformerImage>, IPsPerformerImageRepository
    {
        public PsPerformerImageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsPerformerImageRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<List<PsPerformerImage>> GetByPerformerIdAsync(int performerId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.PerformerId == performerId)
                .ProjectToType<PsPerformerImage>()
                .ToListAsync();
        }
    }
}
