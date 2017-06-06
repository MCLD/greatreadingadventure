using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        "Shows program status by branch (filterable by system) including registered users, challenges completed, badges earned, and points earned.",
        "Program")]
    public class CurrentStatusReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;
        public CurrentStatusReport(ILogger<CurrentStatusReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            ISystemRepository systemRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentNullException(nameof(systemRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _userLogRepository = userLogRepository
                ?? throw new ArgumentNullException(nameof(userLogRepository));
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<OperationStatus> progress = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            StartTimer();

            request.Started = _serviceFacade.DateTimeProvider.Now;
            request.Finished = null;
            request.Success = null;
            request.ResultJson = null;
            request.InstanceName = null;
            request.Name = request.Name;
            await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);

            var criterion
                = await _serviceFacade.ReportCriterionRepository.GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            ICollection<int> systemIds = null;
            if (criterion.SystemId == null)
            {
                var systems = await _systemRepository.GetAllAsync((int)criterion.SiteId);
                systemIds = systems.Select(_ => _.Id).ToList();
            }
            else
            {
                systemIds = new List<int>();
                systemIds.Add((int)criterion.SystemId);
            }

            var data = new List<object[]>();

            var row = new List<object>();
            row.Add("System Name");
            row.Add("Branch Name");
            row.Add("Registered Users");
            row.Add("Achievers");
            row.Add("Challenges Completed");
            row.Add("Badges Earned");
            row.Add("Points Earned");
            data.Add(row.ToArray());

            int count = 0;
            int percentComplete = 0;

            long totalRegistered = 0;
            long totalAchiever = 0;
            long totalChallenges = 0;
            long totalBadges = 0;
            long totalPoints = 0;

            foreach (var systemId in systemIds)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                percentComplete = (++count * 100) / systemIds.Count;
                if (progress != null)
                {
                    progress.Report(new OperationStatus
                    {
                        PercentComplete = percentComplete
                    });
                }

                var branches = await _branchRepository.GetBySystemAsync(systemId);
                foreach (var branch in branches)
                {
                    if (progress != null)
                    {
                        progress.Report(new OperationStatus
                        {
                            Status = $"Processing: {branch.SystemName} - {branch.Name}"
                        });
                    }

                    criterion.SystemId = systemId;
                    criterion.BranchId = branch.Id;

                    var users = await _userRepository.GetCountAsync(criterion);
                    long challenge = await _userLogRepository
                        .CompletedChallengeCountAsync(criterion);
                    long badge = await _userLogRepository.EarnedBadgeCountAsync(criterion);
                    long points = await _userLogRepository.PointsEarnedTotalAsync(criterion);

                    totalRegistered += users.users;
                    totalAchiever += users.achievers;
                    totalChallenges += challenge;
                    totalBadges += badge;
                    totalPoints += points;

                    row = new List<object>();
                    row.Add(branch.SystemName);
                    row.Add(branch.Name);
                    row.Add(users.users);
                    row.Add(users.achievers);
                    row.Add(challenge);
                    row.Add(badge);
                    row.Add(points);

                    data.Add(row.ToArray());

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }

            row = new List<object>();
            row.Add("Total");
            row.Add(string.Empty);
            row.Add(totalRegistered);
            row.Add(totalAchiever);
            row.Add(totalChallenges);
            row.Add(totalBadges);
            row.Add(totalPoints);
            data.Add(row.ToArray());

            _logger.LogInformation($"Report {GetType().Name} with criterion {criterion.Id} ran in {StopTimer()}");

            if (token.IsCancellationRequested)
            {
                request.Success = false;
            }
            else
            {
                request.Finished = _serviceFacade.DateTimeProvider.Now;
                request.Success = true;
                request.ResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(data.ToArray());
            }
            await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
        }
    }
}
