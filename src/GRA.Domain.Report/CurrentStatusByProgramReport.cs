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
    [ReportInformation(4,
        "Current Status By Program Report",
        "Overall status for each branch and each program from the start to a specified date.",
        "Program")]
    public class CurrentStatusByProgramReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IUserRepository _userRepository;

        public CurrentStatusByProgramReport(ILogger<CurrentStatusReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _programRepository = programRepository
                ?? throw new ArgumentNullException(nameof(programRepository));
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

            int count = 0;
            var asof = _serviceFacade.DateTimeProvider.Now;

            var askIfFirstTime
                = await GetSiteSettingBoolAsync(criterion, SiteSettingKey.Users.AskIfFirstTime);
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            var programTotals = new Dictionary<int, (int users, int firstTime, int achievers)>();

            var programReports = new List<StoredReport>();

            var programs = await _programRepository.GetAllAsync((int)criterion.SiteId);
            var branches = await _branchRepository.GetAllAsync((int)criterion.SiteId);

            int totalItems = branches.Count() * programs.Count();

            foreach (var program in programs)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var report = new StoredReport
                {
                    Title = program.Name,
                    AsOf = asof
                };
                var reportData = new List<object[]>();

                // header row
                var headerRow = new List<object>() {
                    "Library System",
                    "Library",
                    "Signups"
                };

                if (askIfFirstTime)
                {
                    headerRow.Add("First time Participants");
                }

                headerRow.Add("Achievers");

                report.HeaderRow = headerRow.ToArray();

                int programTotalUsers = 0;
                int programTotalFirstTime = 0;
                int programTotalAchievers = 0;

                var systemIds = branches
                    .OrderBy(_ => _.SystemName)
                    .GroupBy(_ => _.SystemId)
                    .Select(_ => _.First().SystemId);
                foreach (var systemId in systemIds)
                {
                    foreach (var branch in branches.Where(_ => _.SystemId == systemId))
                    {
                        if (totalItems > 0)
                        {
                            UpdateProgress(progress,
                                ++count * 100 / totalItems,
                                $"{program.Name} - {branch.SystemName}",
                                request.Name);
                        }

                        criterion.BranchId = branch.Id;
                        criterion.ProgramId = program.Id;

                        int users = await _userRepository.GetCountAsync(criterion);
                        int achievers = await _userRepository.GetAchieverCountAsync(criterion);

                        var row = new List<object>() {
                            branch.SystemName,
                            branch.Name,
                            users
                        };

                        if (askIfFirstTime)
                        {
                            int firstTime = await _userRepository.GetFirstTimeCountAsync(criterion);
                            programTotalFirstTime += firstTime;

                            row.Add(firstTime);
                        }

                        row.Add(achievers);

                        programTotalUsers += users;
                        programTotalAchievers += achievers;

                        reportData.Add(row.ToArray());

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                    } // foreach branch

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                } // foreach system

                report.Data = reportData.ToArray();

                var footerRow = new List<object>()
                {
                    "Total",
                    string.Empty,
                    programTotalUsers,
                };

                if (askIfFirstTime)
                {
                    footerRow.Add(programTotalFirstTime);
                }

                footerRow.Add(programTotalAchievers);

                report.FooterRow = footerRow.ToArray();

                programTotals.Add(program.Id, (programTotalUsers, programTotalFirstTime, programTotalAchievers));

                double completion = 0;
                if (programTotalUsers > 0)
                {
                    completion = (double)programTotalAchievers * 100 / programTotalUsers;
                }

                var endDate = criterion.EndDate ?? asof;

                report.FooterText = new string[]
                    {
                        $"Completion rate: {completion.ToString("N2")}%",
                        $"This report was run on {asof.ToString("g")} and contains data up to {endDate.ToString("g")}."
                    };

                programReports.Add(report);
            } // foreach program

            var summaryReport = new StoredReport
            {
                Title = "Summary",
                AsOf = asof
            };
            var summaryReportData = new List<object[]>();

            var summaryHeader = new List<object>()
            {
                "Program",
                "Signups",
            };

            if (askIfFirstTime)
            {
                summaryHeader.Add("First time Participants");
            }

            summaryHeader.Add("Achievers");
            summaryHeader.Add("Achiever Points");

            summaryReport.HeaderRow = summaryHeader.ToArray();

            int signupTotal = 0;
            int firstTimeTotal = 0;
            int achieverTotal = 0;

            foreach (var program in programs)
            {
                var (users, firstTime, achievers) = programTotals[program.Id];

                var summaryRow = new List<object>()
                {
                    program.Name,
                    users
                };

                if (askIfFirstTime)
                {
                    summaryRow.Add(firstTime);
                }

                summaryRow.Add(achievers);
                summaryRow.Add(program.AchieverPointAmount);

                summaryReportData.Add(summaryRow.ToArray());

                signupTotal += users;
                firstTimeTotal += firstTime;
                achieverTotal += achievers;
            }

            summaryReport.Data = summaryReportData.ToArray();

            var summaryFooter = new List<object>()
            {
                "Total",
                signupTotal,
            };

            if (askIfFirstTime)
            {
                summaryFooter.Add(firstTimeTotal);
            }

            summaryFooter.Add(achieverTotal);
            summaryFooter.Add(string.Empty);

            summaryReport.FooterRow = summaryFooter.ToArray();

            double totalCompletion = 0;
            if (signupTotal > 0)
            {
                totalCompletion = (double)achieverTotal * 100 / signupTotal;
            }

            summaryReport.FooterText = new string[] {
                $"Completion rate: {totalCompletion.ToString("N2")}%"
            };
            #endregion Collect data

            #region Finish up reporting
            if (!token.IsCancellationRequested)
            {
                ReportSet.Reports.Add(summaryReport);
                foreach (var report in programReports)
                {
                    ReportSet.Reports.Add(report);
                }
            }
            await FinishRequestAsync(request, !token.IsCancellationRequested);
            #endregion Finish up reporting
        }
    }
}
