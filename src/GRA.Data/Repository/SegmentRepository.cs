using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class SegmentRepository
        : AuditingRepository<Model.Segment, Domain.Model.Segment>, ISegmentRepository
    {
        internal SegmentRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SegmentRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
