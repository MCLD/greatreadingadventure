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
    [ReportInformation(5,
        "Badge Report",
        "See participants that have earned a badge or badges over a time period.",
        "Program")]
    public class BadgeReport : BaseReport
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUserLogRepository _userLogRepository;

        public BadgeReport(ILogger<CurrentStatusReport> logger,
            ServiceFacade.Report serviceFacade,
            IBadgeRepository badgeRepository,
            IChallengeRepository challengeRepository,
            IUserLogRepository userLogRepository) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(badgeRepository);
            ArgumentNullException.ThrowIfNull(challengeRepository);
            ArgumentNullException.ThrowIfNull(userLogRepository);

            _badgeRepository = badgeRepository;
            _challengeRepository = challengeRepository;
            _userLogRepository = userLogRepository;
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            #region Reporting initialization

            request = await StartRequestAsync(request);

            var criterion = await _serviceFacade.ReportCriterionRepository
                    .GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            var report = new StoredReport(_reportInformation.Name,
                _serviceFacade.DateTimeProvider.Now);
            var reportData = new List<object[]>();

            int count = 0;

            #endregion Reporting initialization

            #region Adjust report criteria as needed

            IEnumerable<int> badgeIds = null;
            IEnumerable<int> challengeIds = null;

            if (!string.IsNullOrEmpty(criterion.BadgeRequiredList))
            {
                try
                {
                    badgeIds = criterion.BadgeRequiredList.Split(',').Select(int.Parse);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex,
                        "Unable to convert badge id list ({BadgeIdList}) to numbers: {ErrorMessage}",
                        criterion.BadgeRequiredList,
                        ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(criterion.ChallengeRequiredList))
            {
                try
                {
                    challengeIds = criterion.ChallengeRequiredList.Split(',').Select(int.Parse);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex,
                        "Unable to convert challenge id list ({ChallengeIdList}) to numbers: {ErrorMessage}",
                        criterion.ChallengeRequiredList,
                        ex.Message);
                }
            }

            var totalCount = challengeIds?.Count() ?? 0;
            totalCount += badgeIds?.Count() ?? 0;

            #endregion Adjust report criteria as needed

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = [
                "Earned Item",
                "Participants",
                "Achievers"
            ];

            // running totals
            long totalEarnedItems = 0;
            long totalEarnedAchievers = 0;

            if (badgeIds != null)
            {
                foreach (var badgeId in badgeIds)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var badgeName = await _badgeRepository.GetBadgeNamesAsync([badgeId]);
                    var (earned, earnedAchiever) = await _userLogRepository
                        .EarnedBadgeCountAsync(criterion, badgeId);

                    UpdateProgress(progress,
                        ++count * 100 / totalCount,
                        $"Processing badge: {string.Join(", ", badgeName)}...",
                        request.Name);

                    reportData.Add(
                    [
                        string.Join(", ", badgeName),
                        earned,
                        earnedAchiever
                    ]);

                    totalEarnedItems += earned;
                    totalEarnedAchievers += earnedAchiever;
                }
            }

            if (challengeIds != null)
            {
                foreach (var challengeId in challengeIds)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var challenge = await _challengeRepository.GetByIdAsync(challengeId);
                    var (earned, earnedAchiever) = await _userLogRepository
                        .CompletedChallengeCountAsync(criterion, challengeId);

                    UpdateProgress(progress,
                        ++count * 100 / totalCount,
                        $"Processing challenge: {challenge.Name}...",
                        request.Name);

                    reportData.Add(
                    [
                        challenge.Name,
                        earned,
                        earnedAchiever
                    ]);

                    totalEarnedItems += earned;
                    totalEarnedAchievers += earnedAchiever;
                }
            }

            report.Data = reportData.OrderByDescending(_ => _[1]);

            report.FooterRow =
            [
                "Total",
                totalEarnedItems,
                totalEarnedAchievers
            ];

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
