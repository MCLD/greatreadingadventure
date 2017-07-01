using System;
using System.Collections.Generic;
using System.Globalization;
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
    [ReportInformation(1,
        "Current Status Report",
        "Overall status by branch (filterable by system) including registered users, challenges completed, badges earned, and points earned.",
        "Program")]
    public class CurrentStatusReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;
        public CurrentStatusReport(ILogger<CurrentStatusReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IPointTranslationRepository pointTranslationRepository,
            IProgramRepository programRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _pointTranslationRepository = pointTranslationRepository
                ?? throw new ArgumentNullException(nameof(pointTranslationRepository));
            _programRepository = programRepository
                ?? throw new ArgumentNullException(nameof(programRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
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

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            var headerRow = new List<object>() {
                "System Name",
                "Branch Name",
                "Registered Users",
                "Achievers",
                "Challenges Completed",
                "Badges Earned"
            };

            var translations = new Dictionary<string, ICollection<int?>>();
            var translationTotals = new Dictionary<string, long>();

            foreach (var program in await _programRepository.GetAllAsync((int)criterion.SiteId))
            {
                var pointTranslation = await _pointTranslationRepository
                    .GetByProgramIdAsync(program.Id);

                string description = pointTranslation.ActivityDescriptionPlural;

                if (!translations.ContainsKey(description))
                {
                    translations.Add(description, new List<int?> { pointTranslation.Id });
                    translationTotals.Add(description, 0);
                    if (description.Length > 2)
                    {
                        headerRow.Add(description.First().ToString().ToUpper()
                            + description.Substring(1));
                    }
                    else
                    {
                        headerRow.Add(description);
                    }
                }
                else
                {
                    translations[description].Add(pointTranslation.Id);
                }
            }

            headerRow.Add("Points Earned");
            report.HeaderRow = headerRow.ToArray();

            int count = 0;

            // running totals
            long totalRegistered = 0;
            long totalAchiever = 0;
            long totalChallenges = 0;
            long totalBadges = 0;
            long totalPoints = 0;

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

                    int users = await _userRepository.GetCountAsync(criterion);
                    int achievers = await _userRepository.GetAchieverCountAsync(criterion);
                    long challenge = await _userLogRepository
                        .CompletedChallengeCountAsync(criterion);
                    long badge = await _userLogRepository.EarnedBadgeCountAsync(criterion);
                    long points = await _userLogRepository.PointsEarnedTotalAsync(criterion);

                    totalRegistered += users;
                    totalAchiever += achievers;
                    totalChallenges += challenge;
                    totalBadges += badge;
                    totalPoints += points;

                    var row = new List<object>() {
                        branch.SystemName,
                        branch.Name,
                        users,
                        achievers,
                        challenge,
                        badge,
                    };

                    foreach (var translationName in translations.Keys)
                    {
                        long total = await _userLogRepository.TranslationEarningsAsync(criterion,
                            translations[translationName]);
                        row.Add(total);
                        translationTotals[translationName] += total;
                    }

                    row.Add(points);
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
                totalRegistered,
                totalAchiever,
                totalChallenges,
                totalBadges
            };

            foreach (var total in translationTotals.Values)
            {
                footerRow.Add(total);
            }

            footerRow.Add(totalPoints);

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
