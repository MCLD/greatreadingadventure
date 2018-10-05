using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsDatesRepository
        : AuditingRepository<Model.PsDates, Domain.Model.PsDates>, IPsDatesRepository
    {
        public PsDatesRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsDatesRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<PsDates> GetBySiteIdAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .ProjectTo<PsDates>()
                .SingleOrDefaultAsync();
        }
    }
}
