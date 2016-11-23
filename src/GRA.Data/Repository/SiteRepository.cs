using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.Repository
{
    public class SiteRepository
        : AuditingRepository<Model.Site, Domain.Model.Site>, ISiteRepository
    {
        public SiteRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SiteRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
