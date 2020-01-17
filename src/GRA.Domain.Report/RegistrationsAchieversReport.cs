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
    [ReportInformation(2,
    "Registrations And Achievers Report",
    "Registered participants and achievers by branch (filterable by system and date).",
    "Program")]
    public class RegistrationsAchieversReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IUserRepository _userRepository;

        public RegistrationsAchieversReport(ILogger<RegistrationsAchieversReport> logger,
            ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
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

            var report = new StoredReport
            {
                Title = ReportAttribute?.Name,
                AsOf = _serviceFacade.DateTimeProvider.Now
            };
            var reportData = new List<object[]>();

            var askIfFirstTime
                = await GetSiteSettingBoolAsync(criterion, SiteSettingKey.Users.AskIfFirstTime);
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            var headerRow = new List<object>() {
                "System Name",
                "Branch Name",
                "Registered Users"
            };

            if (askIfFirstTime)
            {
                headerRow.Add("First time Participants");
            }

            headerRow.Add("Achievers");

            report.HeaderRow = headerRow.ToArray();

            int count = 0;

            // running totals
            long totalRegistered = 0;
            long totalFirstTime = 0;
            long totalAchiever = 0;

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
                    totalRegistered += users;
                    totalAchiever += achievers;

                    var row = new List<object>()
                    {
                        branch.SystemName,
                        branch.Name,
                        users
                    };

                    if (askIfFirstTime)
                    {
                        int firstTime = await _userRepository.GetFirstTimeCountAsync(criterion);
                        totalFirstTime += firstTime;

                        row.Add(firstTime);
                    }

                    row.Add(achievers);

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
                totalRegistered
            };

            if (askIfFirstTime)
            {
                footerRow.Add(totalFirstTime);
            }

            footerRow.Add(totalAchiever);

            report.FooterRow = footerRow.ToArray();
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
