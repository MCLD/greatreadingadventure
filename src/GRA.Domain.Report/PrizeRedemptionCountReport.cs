using System;
using System.Collections.Generic;
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
    [ReportInformation(20,
        "Prize Redemption Count Report",
        "Number of times a prize was redeemed by system or branch",
        "Participants")]
    public class PrizeRedemptionCountReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IPrizeWinnerRepository _prizeWinnerRepository;
        private readonly ISystemRepository _systemRepository;

        public PrizeRedemptionCountReport(ILogger<PrizeRedemptionCountReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IPrizeWinnerRepository prizeWinnerRepository,
            ISystemRepository systemRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentException(nameof(branchRepository));
            _prizeWinnerRepository = prizeWinnerRepository
                ?? throw new ArgumentNullException(nameof(prizeWinnerRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentException(nameof(systemRepository));
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            #region Reporting initialization
            request = await StartRequestAsync(request);

            var criterion
                = await _serviceFacade.ReportCriterionRepository.GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            if (criterion.SiteId == null)
            {
                throw new ArgumentException(nameof(criterion.SiteId));
            }

            var report = new StoredReport
            {
                Title = ReportAttribute?.Name,
                AsOf = _serviceFacade.DateTimeProvider.Now
            };

            var reportData = new List<object[]>();
            #endregion Reporting initialization

            #region Adjust report criteria as needed
            IEnumerable<int> triggerIds = null;

            if (!string.IsNullOrEmpty(criterion.TriggerList))
            {
                try
                {
                    triggerIds = criterion.TriggerList.Split(',').Select(int.Parse);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unable to convert trigger id list to numbers: {ex.Message}");
                    _logger.LogError($"Badge id list: {criterion.TriggerList}");
                }
            }
            else
            {
                throw new GraException("No prizes selected.");
            }
            #endregion Adjust report criteria as needed

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[] {
                criterion.SystemId.HasValue ? "Branch Name" : "System Name",
                "# Redeemed"
            };

            int count = 0;
            int totalRedemptionCount = 0;
            if (criterion.SystemId.HasValue)
            {
                var branches = await _branchRepository.GetBySystemAsync(criterion.SystemId.Value);
                foreach (var branch in branches)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    UpdateProgress(progress,
                        ++count * 100 / branches.Count(),
                        $"Processing: {branch.SystemName} - {branch.Name}",
                        request.Name);

                    var redemptionCount = await _prizeWinnerRepository
                        .GetBranchPrizeRedemptionCountAsync(branch.Id, triggerIds);
                    totalRedemptionCount += redemptionCount;

                    IEnumerable<object> row = new object[]
                    {
                        branch.Name,
                        redemptionCount
                    };
                    reportData.Add(row.ToArray());
                }
            }
            else
            {
                var systems = await _systemRepository.GetAllAsync(criterion.SiteId.Value);
                foreach (var system in systems)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    UpdateProgress(progress,
                        ++count * 100 / systems.Count(),
                        $"Processing: {system.Name}",
                        request.Name);

                    var redemptionCount = await _prizeWinnerRepository
                        .GetSystemPrizeRedemptionCountAsync(system.Id, triggerIds);
                    totalRedemptionCount += redemptionCount;

                    IEnumerable<object> row = new object[]
                    {
                        system.Name,
                        redemptionCount
                    };
                    reportData.Add(row.ToArray());
                }
            }

            report.Data = reportData.ToArray();

            IEnumerable<object> footerRow = new object[]
            {
                "Total",
                totalRedemptionCount
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
