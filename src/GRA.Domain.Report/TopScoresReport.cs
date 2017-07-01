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
    [ReportInformation(6,
        "Top Scores Report",
        "Top 30 scoring participants filterable by program, system, and branch",
        "Participants")]
    public class TopScoresReport : BaseReport
    {
        private const int TopToShow = 30;
        private readonly IUserRepository _userRepository;

        public TopScoresReport(ILogger<TopScoresReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
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

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[] {
                "Rank",
                "System Name",
                "Branch Name",
                "Program",
                "Participant",
                "Points Earned"
            };

            int count = 0;
            int total = TopToShow;

            IEnumerable<User> users = await _userRepository.GetTopScoresAsync(criterion, total);

            foreach (var user in users)
            {
                UpdateProgress(progress,
                    ++count * 100 / users.Count(),
                    $"Processing: {count}/{users.Count()}",
                    request.Name);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                var name = new StringBuilder(user.FirstName);
                if(!string.IsNullOrEmpty(user.LastName))
                {
                    name.Append($" {user.LastName}");
                }
                if(!string.IsNullOrEmpty(user.Username))
                {
                    name.Append($" ({user.Username})");
                }

                reportData.Add(new object[] {
                        count,
                        user.SystemName,
                        user.BranchName,
                        user.ProgramName,
                        name.ToString(),
                        user.PointsEarned
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
