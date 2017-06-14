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
            _badgeRepository = badgeRepository 
                ?? throw new ArgumentNullException(nameof(badgeRepository));
            _challengeRepository = challengeRepository
                ?? throw new ArgumentNullException(nameof(challengeRepository));
            _userLogRepository = userLogRepository
                ?? throw new ArgumentNullException(nameof(userLogRepository));
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

            var report = new StoredReport();
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
                catch (Exception ex)
                {
                    _logger.LogError($"Unable to convert badge id list to numbers: {ex.Message}");
                    _logger.LogError($"Badge id list: {criterion.BadgeRequiredList}");
                }
            }

            if (!string.IsNullOrEmpty(criterion.ChallengeRequiredList))
            {
                try
                {
                    challengeIds = criterion.ChallengeRequiredList.Split(',').Select(int.Parse);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unable to convert challenge id list to numbers: {ex.Message}");
                    _logger.LogError($"Challenge id list: {criterion.BadgeRequiredList}");
                }
            }

            int totalCount = challengeIds == null ? 0 : challengeIds.Count();
            totalCount += badgeIds == null ? 0 : badgeIds.Count();
            #endregion Adjust report criteria as needed

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...");

            // header row
            report.HeaderRow = new object[] {
                "Earned Item",
                "Participants"
            };

            // running totals
            long totalEarnedItems = 0;

            if (badgeIds != null)
            {
                foreach (var badgeId in badgeIds)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var badgeName = await _badgeRepository.GetBadgeNameAsync(badgeId);
                    var earned = await _userLogRepository.EarnedBadgeCountAsync(criterion, badgeId);

                    UpdateProgress(progress,
                        ++count * 100 / totalCount,
                        $"Processing badge: {badgeName}...");


                    reportData.Add(new object[]
                    {
                    badgeName,
                    earned
                    });

                    totalEarnedItems += earned;
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
                    var earned = await _userLogRepository.CompletedChallengeCountAsync(criterion, challengeId);

                    UpdateProgress(progress,
                        ++count * 100 / totalCount,
                        $"Processing challenge: {challenge.Name}...");

                    reportData.Add(new object[]
                    {
                    challenge.Name,
                    earned
                    });

                    totalEarnedItems += earned;
                }
            }

            report.Data = reportData.OrderByDescending(_ => _.ElementAt(1));

            report.FooterRow = new object[]
            {
                "Total",
                totalEarnedItems
            };
            report.AsOf = _serviceFacade.DateTimeProvider.Now;
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
