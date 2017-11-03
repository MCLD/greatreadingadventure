using System;
using System.Collections.Generic;
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
    [ReportInformation(13,
        "Vendor Code Report",
        "Vendor prizes earned and ordered by partcipants filterable by system.",
        "Participants")]
    public class VendorCodeReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;

        public VendorCodeReport(ILogger<TopScoresReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            ISystemRepository systemRepository,
            IVendorCodeRepository vendorCodeRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentException(nameof(branchRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentException(nameof(systemRepository));
            _vendorCodeRepository = vendorCodeRepository
                ?? throw new ArgumentException(nameof(vendorCodeRepository));
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<OperationStatus> progress = null)
        {
            #region Reporting initialization
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request = await StartRequestAsync(request);

            var criterion
                = await _serviceFacade.ReportCriterionRepository.GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (criterion.SiteId == null)
            {
                throw new ArgumentNullException(nameof(criterion.SiteId));
            }

            string title = "";

            if (criterion.SystemId.HasValue)
            {
                title = (await _systemRepository.GetByIdAsync(criterion.SystemId.Value)).Name;
            }

            var report = new StoredReport
            {
                Title = title,
                AsOf = _serviceFacade.DateTimeProvider.Now
            };
            var reportData = new List<object[]>();
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[] {
                "System Name",
                "Branch Name",
                "# Earned",
                "# Ordered"
            };

            int count = 0;

            // running totals
            int totalEarned = 0;
            int totalOrdered = 0;

            var branches = criterion.SystemId != null
                ? await _branchRepository.GetBySystemAsync((int)criterion.SystemId)
                : await _branchRepository.GetAllAsync((int)criterion.SiteId);

            var systemIds = branches
                .OrderBy(_ => _.SystemName)
                .GroupBy(_ => _.SystemId)
                .Select(_ => _.First().SystemId);

            foreach (var systemId in systemIds)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                foreach (var branch in branches.Where(_ => _.SystemId == systemId))
                {
                    UpdateProgress(progress,
                        ++count * 100 / branches.Count(),
                        $"Processing: {branch.SystemName} - {branch.Name}",
                        request.Name);

                    criterion.SystemId = systemId;
                    criterion.BranchId = branch.Id;

                    var vendorCodes = await _vendorCodeRepository.GetEarnedCodesAsync(criterion);
                    int earnedCodes = vendorCodes.Count;
                    int usedCodes = vendorCodes.Where(_ => _.IsUsed).Count();

                    totalEarned += earnedCodes;
                    totalOrdered += usedCodes;

                    var row = new List<object>() {
                        branch.SystemName,
                        branch.Name,
                        earnedCodes,
                        usedCodes
                    };

                    reportData.Add(row.ToArray());

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }

            report.Data = reportData.ToArray();

            // total row
            var footerRow = new List<object>()
            {
                "Total",
                string.Empty,
                totalEarned,
                totalOrdered
            };

            report.FooterRow = footerRow.ToArray();

            #endregion Collect data

            #region Finish up reporting
            _logger.LogInformation($"Report {GetType().Name} with criterion {criterion.Id} ran in {StopTimer()}");

            request.Success = !token.IsCancellationRequested;

            if (request.Success == true)
            {
                ReportSet.Reports.Add(report);
                request.Finished = _serviceFacade.DateTimeProvider.Now;
                request.ResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(ReportSet);
            }
            await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
            #endregion Finish up reporting
        }
    }
}
