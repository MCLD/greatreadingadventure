using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsSettingsRepository
        : AuditingRepository<Model.PsSettings, Domain.Model.PsSettings>, IPsSettingsRepository
    {
        public PsSettingsRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsSettingsRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<PsSettings> GetBySiteIdAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .ProjectTo<PsSettings>()
                .SingleOrDefaultAsync();
        }
    }
}
