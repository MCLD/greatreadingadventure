using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Report.Abstract;
using GRA.Domain.Report.Attribute;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Report
{
    [ReportInformation(ReportId,
        "Vendor Titles On Order",
        "Unshipped titles on order with a count and earliest/most recent order dates.",
        "Vendor",
        RequiredPermission)]
    public class VendorTitlesOnOrderReport : BaseReport
    {
        public const int ReportId = 27;
        private const string RequiredPermission = nameof(Permission.VendorShippingReporting);

        private readonly IVendorCodeRepository _vendorCodeRepository;

        public VendorTitlesOnOrderReport(ILogger<VendorTitlesOnOrderReport> logger,
            ServiceFacade.Report serviceFacade,
            IVendorCodeRepository vendorCodeRepository
           ) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(vendorCodeRepository);

            _vendorCodeRepository = vendorCodeRepository;
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            #region Reporting intialization

            ArgumentNullException.ThrowIfNull(request);

            request = await StartRequestAsync(request);

            var criterion = await _serviceFacade.ReportCriterionRepository
                    .GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (!criterion.SiteId.HasValue)
            {
                throw new ArgumentException(nameof(criterion.SiteId));
            }

            if (!criterion.VendorCodeTypeId.HasValue)
            {
                throw new ArgumentException(nameof(criterion.VendorCodeTypeId));
            }

            var report = new StoredReport(_reportInformation.Name,
                _serviceFacade.DateTimeProvider.Now);
            var reportData = new List<object[]>();

            #endregion Reporting intialization

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[]
            {
                "Details",
                "Count",
                "Earliest Order Date",
                "Latest Order Date"
            };

            int count = 0;

            var vendorCodeTitles = await _vendorCodeRepository
                .GetTitlesOnOrderAsync(criterion.VendorCodeTypeId.Value);

            int row = 0;

            foreach (var title in vendorCodeTitles.OrderByDescending(_ => _.Count)
                .ThenBy(_ => _.EarliestDate))
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                UpdateProgress(progress,
                    ++count * 100 / vendorCodeTitles.Count,
                    $"Processing: {title.Details}",
                    request.Name);

                if (title != null)
                {
                    reportData.Add(new object[]
                    {
                        title.Details,
                        title.Count,
                        title.EarliestDate.ToString("d", CultureInfo.CurrentCulture),
                        title.LatestDate.ToString("d", CultureInfo.CurrentCulture)
                    });

                    row++;
                }
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
