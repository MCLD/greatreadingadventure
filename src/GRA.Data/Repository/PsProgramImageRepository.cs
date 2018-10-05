using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsProgramImageRepository
        : AuditingRepository<Model.PsProgramImage, Domain.Model.PsProgramImage>, IPsProgramImageRepository
    {
        public PsProgramImageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsProgramImageRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
