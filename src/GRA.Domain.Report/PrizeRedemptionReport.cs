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
    [ReportInformation(11,
        "Prize Redemption Report",
        "Prizes redeemed at a system or branch.",
        "Participants")]
    public class PrizeRedemptionReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IPrizeWinnerRepository _prizeWinnerRepository;
        private readonly ISystemRepository _systemRepository;

        public PrizeRedemptionReport(ILogger<PrizeRedemptionReport> logger,
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

            var prizes = await _prizeWinnerRepository.GetRedemptionsAsync(criterion);

            foreach (var prize in prizes
                .Where(_ => string.IsNullOrEmpty(_.PrizeName)
                && _.DrawingId == null
                && _.TriggerId == null))
            {
                prize.PrizeName = "Free Book";
            }

            var prizeGroups = prizes.GroupBy(_ => new
            {
                _.PrizeName,
                _.DrawingId,
                _.TriggerId
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
            if (!token.IsCancellationRequested)
            {
                ReportSet.Reports.Add(report);
            }
            await FinishRequestAsync(request, !token.IsCancellationRequested);
            #endregion Finish up reporting
        }
    }
}
