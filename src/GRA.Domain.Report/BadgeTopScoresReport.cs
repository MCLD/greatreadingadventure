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
    [ReportInformation(7,
        "Badge Top Scores Report",
        "Top 30 scoring participants who have earned a badge or badges.",
        "Participants")]
    public class BadgeTopScoresReport : BaseReport
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;
        public BadgeTopScoresReport(ILogger<CurrentStatusReport> logger,
            ServiceFacade.Report serviceFacade,
            IBadgeRepository badgeRepository,
            IChallengeRepository challengeRepository,
            IUserLogRepository userLogRepository,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
            _badgeRepository = badgeRepository
                ?? throw new ArgumentNullException(nameof(badgeRepository));
            _challengeRepository = challengeRepository
                ?? throw new ArgumentNullException(nameof(challengeRepository));
            _userLogRepository = userLogRepository
                ?? throw new ArgumentNullException(nameof(userLogRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
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

            var report = new StoredReport
            {
                Title = ReportAttribute?.Name,
                AsOf = _serviceFacade.DateTimeProvider.Now
            };
            var reportData = new List<object[]>();
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

            if (criterion.StartDate == null)
            {
                criterion.StartDate = DateTime.MinValue;
            }

            if (criterion.EndDate == null)
            {
                criterion.EndDate = DateTime.MaxValue;
            }

            #endregion Adjust report criteria as needed

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[] {
                "Rank",
                "Participant",
                "System Name",
                "Branch Name",
                "Program",
                "Points Earned"
            };

            ICollection<int> usersWhoEarned = null;

            if (badgeIds != null && !token.IsCancellationRequested)
            {
                UpdateProgress(progress, 1, "Looking up users who earned badges...", request.Name);

                foreach (var badgeId in badgeIds)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var earned = await _userLogRepository.UserIdsEarnedBadgeAsync(badgeId, criterion);

                    usersWhoEarned = earned;
                }
            }

            if (challengeIds != null && !token.IsCancellationRequested)
            {
                UpdateProgress(progress, 1, "Looking up users who completed challenges...", request.Name);

                foreach (var challengeId in challengeIds)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var earned
                        = await _userLogRepository.UserIdsCompletedChallengesAsync(challengeId, criterion);

                    if (usersWhoEarned == null || usersWhoEarned.Count == 0)
                    {
                        usersWhoEarned = earned;
                    }
                    else
                    {
                        usersWhoEarned = usersWhoEarned.Union(earned).ToList();
                    }
                }
            }

            int totalCount = usersWhoEarned.Count;
            int count = 0;
            long totalPoints = 0;

            foreach (int userId in usersWhoEarned)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                UpdateProgress(progress,
                    ++count * 100 / usersWhoEarned.Count(),
                    $"Processing user: {count}/{usersWhoEarned.Count()}",
                    request.Name);

                long pointsEarned = await _userLogRepository.GetEarningsOverPeriodAsync(userId, criterion);

                var user = await _userRepository.GetByIdAsync(userId);

                var name = new StringBuilder(user.FirstName);
                if (!string.IsNullOrEmpty(user.LastName))
                {
                    name.Append($" {user.LastName}");
                }
                if (!string.IsNullOrEmpty(user.Username))
                {
                    name.Append($" ({user.Username})");
                }

                reportData.Add(new object[]
                {
                    0,
                    name.ToString(),
                    user.SystemName,
                    user.BranchName,
                    user.ProgramName,
                    pointsEarned
                });

                totalPoints += pointsEarned;
            }

            int rank = 1;
            report.Data = reportData
                .OrderByDescending(_ => _.ElementAt(5))
                .Select(_ => new object[] {
                   rank++,
                   _.ElementAt(1),
                   _.ElementAt(2),
                   _.ElementAt(3),
                   _.ElementAt(4),
                   _.ElementAt(5),
                });

            report.FooterRow = new object[]
            {
                "Total",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                totalPoints
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
