using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Report.Abstract;
using GRA.Domain.Report.Attribute;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Report
{
    [ReportInformation(23,
        "Vendor Pending Prize Report",
        "Count of vendor prizes which have not shipped or not arrived.",
        "Participants")]
    public class VendorCodePendingReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;

        public VendorCodePendingReport(ILogger<VendorCodePendingReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IUserRepository userRepository,
            IVendorCodeRepository vendorCodeRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _vendorCodeRepository = vendorCodeRepository
                ?? throw new ArgumentNullException(nameof(vendorCodeRepository));
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            #region Reporting intialization

            request = await StartRequestAsync(request);

            var criterion
                    = await _serviceFacade.ReportCriterionRepository
                        .GetByIdAsync(request.ReportCriteriaId)
                    ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (!criterion.SiteId.HasValue)
            {
                throw new ArgumentException(nameof(criterion.SiteId));
            }

            var report = new StoredReport
            {
                Title = "Vendor Code Items Pending",
                AsOf = _serviceFacade.DateTimeProvider.Now
            };
            var reportData = new List<object[]>();

            #endregion Reporting intialization

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[]
            {
                "Pick Up System",
                "Pick Up Branch",
                "Ordered, not shipped",
                "Shipped, not arrived"
            };

            int count = 0;

            var remainingPrizes = await _vendorCodeRepository.GetPendingPrizesPickupBranch();

            foreach (var prize in remainingPrizes)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                UpdateProgress(progress,
                    ++count * 100 / remainingPrizes.Count,
                    $"Processing: {prize.Name}",
                    request.Name);

                reportData.Add(new object[]
                {
                   prize.SystemName,
                   prize.Name,
                   prize.OrderedNotShipped,
                   prize.ShippedNotArrived
                });
            }

            report.Data = reportData.ToArray();

            #endregion Collect data

            #region Finish up reporting

            if (!token.IsCancellationRequested)
            {
                ReportSet.Reports.Add(report);
            }
            await FinishRequestAsync(request, !token.IsCancellationRequested);

            #endregion Finish up reporting
        }
    }
}
