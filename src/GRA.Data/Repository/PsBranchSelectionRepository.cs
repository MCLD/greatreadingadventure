using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsBranchSelectionRepository
        : AuditingRepository<Model.PsBranchSelection, Domain.Model.PsBranchSelection>, IPsBranchSelectionRepository
    {
        public PsBranchSelectionRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsBranchSelectionRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
