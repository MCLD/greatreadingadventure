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
            ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IUserRepository userRepository,
            IVendorCodeRepository vendorCodeRepository) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(branchRepository);
            ArgumentNullException.ThrowIfNull(userRepository);
            ArgumentNullException.ThrowIfNull(vendorCodeRepository);

            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _vendorCodeRepository = vendorCodeRepository;
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            #region Reporting intialization

            request = await StartRequestAsync(request);

            var criterion = await _serviceFacade.ReportCriterionRepository
                    .GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (!criterion.SiteId.HasValue)
            {
                throw new ArgumentException(nameof(criterion.SiteId));
            }

            var report = new StoredReport("Vendor Code Items Pending",
                _serviceFacade.DateTimeProvider.Now);
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

            var count = 0;

            var remainingPrizes = await _vendorCodeRepository.GetPendingPrizesPickupBranch();

            var totalOrderedNotShipped = 0;
            var totalShippedNotArrived = 0;

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

                totalOrderedNotShipped += prize.OrderedNotShipped;
                totalShippedNotArrived += prize.ShippedNotArrived;
            }

            report.Data = reportData.ToArray();

            report.FooterRow = new object[]
            {
                    "Total",
                    "",
                    totalOrderedNotShipped,
                    totalShippedNotArrived
            };

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
