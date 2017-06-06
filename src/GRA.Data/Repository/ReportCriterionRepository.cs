using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class ReportCriterionRepository
        : AuditingRepository<Model.ReportCriterion, Domain.Model.ReportCriterion>,
        IReportCriterionRepository
    {
        public ReportCriterionRepository(ServiceFacade.Repository repositoryFacade, 
            ILogger<ReportCriterionRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
