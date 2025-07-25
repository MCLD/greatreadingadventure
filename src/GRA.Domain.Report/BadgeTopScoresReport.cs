﻿using System;
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
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;

        public BadgeTopScoresReport(ILogger<CurrentStatusReport> logger,
            ServiceFacade.Report serviceFacade,
            IUserLogRepository userLogRepository,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(userLogRepository);
            ArgumentNullException.ThrowIfNull(userRepository);

            _userLogRepository = userLogRepository;
            _userRepository = userRepository;
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

            if (criterion.SiteId == null)
            {
                throw new ArgumentException(nameof(criterion.SiteId));
            }

            var report = new StoredReport(_reportInformation.Name,
                _serviceFacade.DateTimeProvider.Now);
            var reportData = new List<object[]>();

            #endregion Reporting initialization

            #region Adjust report criteria as needed

            int? badgeId = null;
            int? challengeId = null;

            if (!string.IsNullOrEmpty(criterion.BadgeRequiredList))
            {
                try
                {
                    badgeId = Convert.ToInt32(criterion.BadgeRequiredList);
                }
                catch (Exception ex) when (ex is FormatException or OverflowException)
                {
                    _logger.LogError(ex,
                        "Unable to convert badge id ({BadgeId}) to a number: {ErrorMessage}",
                        criterion.BadgeRequiredList,
                        ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(criterion.ChallengeRequiredList))
            {
                try
                {
                    challengeId = Convert.ToInt32(criterion.ChallengeRequiredList);
                }
                catch (Exception ex) when (ex is FormatException or OverflowException)
                {
                    _logger.LogError(ex,
                        "Unable to convert challenge id ({ChallengeId}) to a number: {ErrorMessage}",
                        criterion.ChallengeRequiredList,
                        ex.Message);
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

            if (badgeId != null && !token.IsCancellationRequested)
            {
                UpdateProgress(progress, 1, "Looking up users who earned the badge...", request.Name);

                var earned = await _userLogRepository.UserIdsEarnedBadgeAsync(badgeId.Value, criterion);

                usersWhoEarned = earned;
            }

            if (challengeId != null && !token.IsCancellationRequested)
            {
                UpdateProgress(progress, 1, "Looking up users who completed the challenge...", request.Name);

                var earned
                    = await _userLogRepository.UserIdsCompletedChallengesAsync(challengeId.Value, criterion);

                if (usersWhoEarned == null || usersWhoEarned.Count == 0)
                {
                    usersWhoEarned = earned;
                }
                else
                {
                    usersWhoEarned = usersWhoEarned.Union(earned).ToList();
                }
            }

            int count = 0;
            long totalPoints = 0;

            foreach (int userId in usersWhoEarned)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                UpdateProgress(progress,
                    ++count * 100 / usersWhoEarned.Count,
                    $"Processing user: {count}/{usersWhoEarned.Count}",
                    request.Name);

                long pointsEarned = await _userLogRepository.GetEarningsOverPeriodAsync(userId, criterion);

                var user = await _userRepository.GetByIdAsync(userId);

                var name = new StringBuilder(user.FirstName);
                if (!string.IsNullOrEmpty(user.LastName))
                {
                    name.Append(' ').Append(user.LastName);
                }
                if (!string.IsNullOrEmpty(user.Username))
                {
                    name.Append(" (").Append(user.Username).Append(')');
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
                .OrderByDescending(_ => _[5])
                .Select(_ => new object[] {
                   rank++,
                   _[1],
                   _[2],
                   _[3],
                   _[4],
                   _[5],
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
