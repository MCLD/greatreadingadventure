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
    public class ReportRunService : BaseUserService<ReportRunService>
    {
        private readonly IReportRequestRepository _reportRequestRepository;

        public ReportRunService(ILogger<ReportRunService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IReportRequestRepository reportRequestRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _reportRequestRepository = reportRequestRepository;
            SetManagementPermission(Permission.ViewAllReporting);
        }

        public async Task<DataWithCount<ICollection<ReportRunSummary>>> GetPaginatedReportRunsAsync(
            ReportRequestFilter filter)
        {
            VerifyManagementPermission();

            filter.SiteId = GetCurrentSiteId();

            var data = await _reportRequestRepository.PageAsync(filter);
            var count = await _reportRequestRepository.CountAsync(filter);

            _logger.LogInformation(
                "Listing report runs {SiteId} skip {Skip} take {Take} filterReportId {ReportId} user {UserId}",
                filter.SiteId, filter.Skip, filter.Take, filter.ReportId, GetClaimId(ClaimType.UserId));

            return new DataWithCount<ICollection<ReportRunSummary>> { Data = data, Count = count };
        }
    }
}
