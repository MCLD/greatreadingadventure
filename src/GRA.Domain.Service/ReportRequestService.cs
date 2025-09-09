using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class ReportRequestService : BaseUserService<ReportRequestService>
    {
        private readonly IReportRequestRepository _reportRequestRepository;

        public ReportRequestService(ILogger<ReportRequestService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IReportRequestRepository reportRequestRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _reportRequestRepository = reportRequestRepository;
            SetManagementPermission(Permission.ViewAllReporting);
        }

        public async Task<DataWithCount<ICollection<ReportRequestSummary>>> GetPaginatedListAsync(
            ReportRequestFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();

            var data = await _reportRequestRepository.PageAsync(filter);
            var count = await _reportRequestRepository.CountAsync(filter);

            return new DataWithCount<ICollection<ReportRequestSummary>> { Data = data, Count = count };
        }
    }
}
