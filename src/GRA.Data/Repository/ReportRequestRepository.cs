using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class ReportRequestRepository
        : AuditingRepository<Model.ReportRequest, Domain.Model.ReportRequest>,
        IReportRequestRepository
    {
        public ReportRequestRepository(ServiceFacade.Repository repositoryFacade, 
            ILogger<ReportRequestRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
