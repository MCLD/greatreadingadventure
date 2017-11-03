using GRA.Domain.Model;
using GRA.Domain.Report.Abstract;
using GRA.Domain.Report.Attribute;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GRA.Domain.Report
{
    [ReportInformation(11,
        "Prize Redemption Report",
        "Prizes redeemed at a system or branch.",
        "Participants")]
    public class PrizeRedemptionReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IPrizeWinnerRepository _prizeWinnterRepository;
        private readonly ISystemRepository _systemRepository;

        public PrizeRedemptionReport(ILogger<TopScoresReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IPrizeWinnerRepository prizeWinnterRepository,
            ISystemRepository systemRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository 
                ?? throw new ArgumentException(nameof(branchRepository));
            _prizeWinnterRepository = prizeWinnterRepository
                ?? throw new ArgumentNullException(nameof(prizeWinnterRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentException(nameof(systemRepository));
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

            if (criterion.BranchId.HasValue)
            {
                criterion.SystemId = null;
                title = (await _branchRepository.GetByIdAsync(criterion.BranchId.Value)).Name;
            }
            else if (criterion.SystemId.HasValue)
            {
                title = (await _systemRepository.GetByIdAsync(criterion.SystemId.Value)).Name;
            }
            else 
            {
                throw new GraException("No system or branch selected.");
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
                "Prize Name",
                "# Redeemed"
            };

            int count = 0;

            var prizes = await _prizeWinnterRepository.GetRedemptionsAsync(criterion);

            var prizeGroups = prizes.GroupBy(_ => new
            {
                PrizeName = _.PrizeName,
                DrawingId = _.DrawingId,
                TriggerId = _.TriggerId
            })
            .Select(_ => new
            {
                Prize = _.Key,
                Count = _.Count()
            })
            .OrderBy(_ => _.Prize.PrizeName);

            int prizeGroupsCount = prizeGroups.Count();

            foreach (var group in prizeGroups)
            {
                UpdateProgress(progress,
                    ++count * 100 / prizeGroupsCount,
                    $"Processing: {count}/{prizeGroupsCount}",
                    request.Name);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                reportData.Add(new object[] {
                        group.Prize.PrizeName,
                        group.Count
                    });
            }

            report.Data = reportData.ToArray();
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
