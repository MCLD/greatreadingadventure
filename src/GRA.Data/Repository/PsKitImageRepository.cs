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
    public class PsKitImageRepository
        : AuditingRepository<Model.PsKitImage, Domain.Model.PsKitImage>, IPsKitImageRepository
    {
        public PsKitImageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsKitImageRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<List<PsKitImage>> GetByKitIdAsync(int kitId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.KitId == kitId)
                .ProjectToType<PsKitImage>()
                .ToListAsync();
        }
    }
}
