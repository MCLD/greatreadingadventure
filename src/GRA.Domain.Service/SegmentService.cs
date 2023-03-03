using GRA.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SegmentService : Abstract.BaseService<SegmentService>
    {
        public SegmentService(ILogger<SegmentService> logger,
            IDateTimeProvider dateTimeProvider) : base(logger, dateTimeProvider)
        {
        }
    }
}
