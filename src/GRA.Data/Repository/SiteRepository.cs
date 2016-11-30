using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class SiteRepository
        : AuditingRepository<Model.Site, Site>, ISiteRepository
    {
        public SiteRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SiteRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
