using GRA.Data.ServiceFacade;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DrawingCriterionRepository
        : AuditingRepository<Model.DrawingCriterion, Domain.Model.DrawingCriterion>,
        IDrawingCriterionRepository
    {
        public DrawingCriterionRepository(ServiceFacade.Repository repositoryFacade, 
            ILogger<DrawingCriterionRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
