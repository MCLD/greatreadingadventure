﻿using GRA.Domain.Model;
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
    [ReportInformation(12,
        "Participant Prize Report",
        "Prizes earned by partcipants filterable by system or branch.",
        "Participants")]
    public class ParticipantPrizeReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IPrizeWinnerRepository _prizeWinnterRepository;
        private readonly ISystemRepository _systemRepository;

        public ParticipantPrizeReport(ILogger<TopScoresReport> logger,
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
                "# Awarded",
                "# Redeemed"
            };

            int count = 0;

            var prizes = await _prizeWinnterRepository.GetUserPrizesAsync(criterion);

            var prizeGroups = prizes.GroupBy(_ => new
            {
                _.PrizeName,
                _.DrawingId,
                _.TriggerId
            })
            .Select(_ => new
            {
                Prize = _.Key,
                AwardedCount = _.Count(),
                RedeemedCount = _.Count(p => p.RedeemedAt.HasValue)
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
                        group.AwardedCount,
                        group.RedeemedCount
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
