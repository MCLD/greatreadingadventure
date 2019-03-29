using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PageHeaderRepository
        : AuditingRepository<Model.PageHeader, Domain.Model.PageHeader>, IPageHeaderRepository
    {
        internal PageHeaderRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PageHeaderRepository> logger)
            : base(repositoryFacade, logger)
        {
        }
    }
}
