using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
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
            ArgumentNullException.ThrowIfNull(reportRequestRepository);

            _reportRequestRepository = reportRequestRepository;
        }

        public async Task<DataWithCount<ICollection<ReportRequestSummary>>> GetPaginatedListAsync(
            ReportRequestFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            filter.SiteId = GetCurrentSiteId();

            var data = await _reportRequestRepository.PageAsync(filter);
            var count = await _reportRequestRepository.CountAsync(filter);

            return new DataWithCount<ICollection<ReportRequestSummary>>
            {
                Data = data,
                Count = count
            };
        }
    }
}
