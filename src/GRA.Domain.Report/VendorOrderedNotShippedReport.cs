using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
        "Vendor Ordered Not Shipped",
        "Vendor orders which have not yet shipped.",
        "Vendor",
        RequiredPermission)]
    public class VendorOrderedNotShippedReport : BaseReport
    {
        public const int ReportId = 25;
        private const string RequiredPermission = nameof(Permission.VendorShippingReporting);

        private readonly IBranchRepository _branchRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;

        public VendorOrderedNotShippedReport(ILogger<VendorOrderedNotShippedReport> logger,
            ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IVendorCodeRepository vendorCodeRepository
           ) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(branchRepository);
            ArgumentNullException.ThrowIfNull(vendorCodeRepository);

            _branchRepository = branchRepository;
            _vendorCodeRepository = vendorCodeRepository;
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            #region Reporting intialization

            ArgumentNullException.ThrowIfNull(request);

            var properties = request.Properties;

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

            var report = new StoredReport
            {
                AsOf = _serviceFacade.DateTimeProvider.Now,
                Links = new Dictionary<string, string>(),
                Title = _reportInformation.Name
            };
            var reportData = new List<object[]>();

            #endregion Reporting intialization

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[]
            {
                "Code",
                "Username",
                "Reason for Reassignment",
                "Delivery Branch",
                "Order Details",
                "Order Date",
                "Days Since Ordered",
                "Packing Slip",
                "Tracking Number"
            };

            int count = 0;

            var branches = await _branchRepository.GetAllAsync(criterion.SiteId.Value);
            var branchLookup = branches.ToDictionary(k => k.Id, v => v.Name);

            var vendorCodeItemStatuses = await _vendorCodeRepository
                .GetOrderedNotShipped(criterion.VendorCodeTypeId.Value,
                    criterion.SystemId,
                    criterion.BranchId);

            int row = 0;

            properties.TryGetValue(JobDetailsPropertyName.PackingSlipLink, out var packingSlipLink);
            properties.TryGetValue(JobDetailsPropertyName.ProfileLink, out var profileLink);
            properties.TryGetValue(JobDetailsPropertyName.VendorCodeLink, out var vendorCodeLink);

            foreach (var status in vendorCodeItemStatuses)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var statusMessage = status.OrderDate.HasValue
                    ? status.OrderDate.Value.ToString("d", CultureInfo.CurrentCulture)
                    : status.ShipDate.HasValue
                        ? status.ShipDate.Value.ToString("d", CultureInfo.CurrentCulture)
                        : status.ArrivalDate.HasValue
                            ? status.ArrivalDate.Value.ToString("d", CultureInfo.CurrentCulture)
                            : "...";

                UpdateProgress(progress,
                    ++count * 100 / vendorCodeItemStatuses.Count,
                    $"Processing: {statusMessage}",
                    request.Name);

                if (status != null)
                {
                    var userInfo = new StringBuilder($"{status.FirstName} {status.LastName}");
                    if (!string.IsNullOrEmpty(status.Username))
                    {
                        userInfo.Append(CultureInfo.InvariantCulture, $" ({status.Username})");
                    }

                    if (!string.IsNullOrEmpty(vendorCodeLink))
                    {
                        report.Links.Add($"{row},0", $"{vendorCodeLink}{status.UserId}");
                    }

                    if (!string.IsNullOrEmpty(profileLink))
                    {
                        report.Links.Add($"{row},1", $"{profileLink}{status.UserId}");
                    }

                    string reassigned = null;
                    if (status.ReassignedAt.HasValue)
                    {
                        reassigned = $"By {status.ReassignedBy} at {status.ReassignedAt}: {status.ReassignedFor}";
                        if (!string.IsNullOrEmpty(profileLink)
                            && status.ReassignedBy.HasValue)
                        {
                            report.Links.Add($"{row},2", $"{profileLink}{status.ReassignedBy}");
                        }
                    }

                    if (!string.IsNullOrEmpty(packingSlipLink)
                        && !string.IsNullOrEmpty(status.PackingSlip))
                    {
                        report.Links.Add($"{row},8", $"{packingSlipLink}{status.PackingSlip}");
                    }

                    reportData.Add(new object[]
                    {
                        status.Code,
                        userInfo.ToString(),
                        reassigned,
                        status.DeliveryBranchId.HasValue
                            ? branchLookup[status.DeliveryBranchId.Value]
                            : "Unspecified",
                        status.OrderDetails,
                        status.OrderDate?.ToString("d", CultureInfo.CurrentCulture),
                        status.OrderDate.HasValue
                            ? (_serviceFacade.DateTimeProvider.Now - status.OrderDate.Value).Days
                            : string.Empty,
                        status.PackingSlip,
                        status.TrackingNumber
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
