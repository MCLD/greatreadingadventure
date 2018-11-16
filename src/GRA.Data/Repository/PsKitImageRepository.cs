using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Data.Model;
using GRA.Domain.Repository;
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

        public async Task<ICollection<PsKitImage>> GetByKitIdAsync(int kitId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.KitId == kitId)
                .ProjectTo<PsKitImage>()
                .ToListAsync();
        }
    }
}
