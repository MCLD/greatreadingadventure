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

            int count = 0;
            var asof = _serviceFacade.DateTimeProvider.Now;
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            var programTotals = new Dictionary<int, (int users, int achievers)>();

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

                report.HeaderRow = new string[]
                {
                    "Library System",
                    "Library",
                    "Signups",
                    "Achievers"
                };

                int programTotalUsers = 0;
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

                        reportData.Add(new object[]
                        {
                            branch.SystemName,
                            branch.Name,
                            users,
                            achievers
                        });

                        programTotalUsers += users;
                        programTotalAchievers += achievers;

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

                report.FooterRow = new object[]
                    {
                        "Total",
                        string.Empty,
                        programTotalUsers,
                        programTotalAchievers
                    };

                programTotals.Add(program.Id, (programTotalUsers, programTotalAchievers));

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
            summaryReport.HeaderRow = new string[]
            {
                "Program",
                "Signups",
                "Achievers",
                "Achiever Points"
            }.ToArray();

            int signupTotal = 0;
            int achieverTotal = 0;

            foreach (var program in programs)
            {
                var programData = programTotals[program.Id];
                summaryReportData.Add(new object[]
                {
                    program.Name,
                    programData.users,
                    programData.achievers,
                    program.AchieverPointAmount
                });

                signupTotal += programData.users;
                achieverTotal += programData.achievers;
            }

            summaryReport.Data = summaryReportData.ToArray();

            summaryReport.FooterRow = new object[]
            {
                "Total",
                signupTotal,
                achieverTotal,
                string.Empty
            };

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
            _logger.LogInformation($"Report {GetType().Name} with criterion {criterion.Id} ran in {StopTimer()}");

            request.Success = !token.IsCancellationRequested;

            if (request.Success == true)
            {
                ReportSet.Reports.Add(summaryReport);
                foreach (var report in programReports)
                {
                    ReportSet.Reports.Add(report);
                }
                request.Finished = _serviceFacade.DateTimeProvider.Now;
                request.ResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(ReportSet);
            }
            await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
            #endregion Finish up reporting
        }
    }
}
