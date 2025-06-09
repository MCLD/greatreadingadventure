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
    [ReportInformation(21,
        "Vendor Code By Program Report",
        "Vendor prizes by program filterable by system.",
        "Participants")]
    public class VendorCodeByProgramReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;

        public VendorCodeByProgramReport(ILogger<VendorCodeByProgramReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            ISystemRepository systemRepository,
            IVendorCodeRepository vendorCodeRepository) : base(logger, serviceFacade)
        {
            ArgumentNullException.ThrowIfNull(branchRepository);
            ArgumentNullException.ThrowIfNull(programRepository);
            ArgumentNullException.ThrowIfNull(systemRepository);
            ArgumentNullException.ThrowIfNull(vendorCodeRepository);

            _branchRepository = branchRepository;
            _programRepository = programRepository;
            _systemRepository = systemRepository;
            _vendorCodeRepository = vendorCodeRepository;
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

            if (!criterion.SiteId.HasValue)
            {
                throw new ArgumentException(nameof(criterion.SiteId));
            }

            int count = 0;

            #endregion Reporting initialization

            #region Collect data

            UpdateProgress(progress, 1, "Starting report...", request.Name);

            var programTotals = new Dictionary<int, (int earned, int donated, int redeemed)>();

            var programReports = new List<StoredReport>();

            var programs = await _programRepository.GetAllAsync((int)criterion.SiteId);

            int totalItems;

            IEnumerable<Model.System> systems = null;
            IEnumerable<Branch> branches = null;

            if (criterion.SystemId.HasValue)
            {
                branches = await _branchRepository.GetBySystemAsync(criterion.SystemId.Value);

                totalItems = programs.Count() * branches.Count();
            }
            else
            {
                systems = await _systemRepository.GetAllAsync(criterion.SiteId.Value);

                totalItems = programs.Count() * systems.Count();
            }

            foreach (var program in programs)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var report = new StoredReport(program.Name ?? _reportInformation.Name,
                    _serviceFacade.DateTimeProvider.Now);
                var reportData = new List<object[]>();

                criterion.ProgramId = program.Id;

                // header row
                var headerRow = new object[] {
                    "Earned",
                    "Donated",
                    "Redeemed"
                };

                int programTotalEarned = 0;
                int programTotalDonated = 0;
                int programTotalRedeened = 0;

                if (branches != null)
                {
                    headerRow = headerRow.Prepend("Branch Name").ToArray();

                    foreach (var branch in branches)
                    {
                        if (totalItems > 0)
                        {
                            UpdateProgress(progress,
                                ++count * 100 / totalItems,
                                $"{program.Name} - {branch.Name}",
                                request.Name);
                        }

                        criterion.BranchId = branch.Id;

                        var vendorCodes = await _vendorCodeRepository
                            .GetEarnedCodesAsync(criterion);

                        var earned = vendorCodes.Count;
                        var donated = vendorCodes.Count(_ => _.IsDonated == true);
                        var redeemed = vendorCodes.Count(_ => _.IsUsed);

                        programTotalEarned += earned;
                        programTotalDonated += donated;
                        programTotalRedeened += redeemed;

                        var row = new List<object>
                        {
                            branch.Name,
                            earned,
                            donated,
                            redeemed
                        };

                        reportData.Add(row.ToArray());

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                } // foreach branch
                else
                {
                    headerRow = headerRow.Prepend("System Name").ToArray();

                    foreach (var system in systems)
                    {
                        if (totalItems > 0)
                        {
                            UpdateProgress(progress,
                                ++count * 100 / totalItems,
                                $"{program.Name} - {system.Name}",
                                request.Name);
                        }

                        criterion.SystemId = system.Id;

                        var vendorCodes = await _vendorCodeRepository
                            .GetEarnedCodesAsync(criterion);

                        var earned = vendorCodes.Count;
                        var donated = vendorCodes.Count(_ => _.IsDonated == true);
                        var redeemed = vendorCodes.Count(_ => _.IsUsed);

                        programTotalEarned += earned;
                        programTotalDonated += donated;
                        programTotalRedeened += redeemed;

                        var row = new List<object>
                        {
                            system.Name,
                            earned,
                            donated,
                            redeemed
                        };

                        reportData.Add(row.ToArray());

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                    } // foreach system
                } // foreach program

                report.HeaderRow = headerRow;
                report.Data = reportData.ToArray();

                report.FooterRow = new object[]
                {
                    "Total",
                    programTotalEarned,
                    programTotalDonated,
                    programTotalRedeened
                };

                programTotals.Add(program.Id,
                    (programTotalEarned, programTotalDonated, programTotalRedeened));

                programReports.Add(report);
            }

            var summaryReport = new StoredReport("Summary", _serviceFacade.DateTimeProvider.Now);

            var summaryReportData = new List<object[]>();

            summaryReport.HeaderRow = new object[]
            {
                "Program",
                "# Earned",
                "# Donated",
                "# Redeemed"
            };

            int earnedTotal = 0;
            int donatedTotal = 0;
            int redeemedTotal = 0;

            foreach (var program in programs)
            {
                var (earned, donated, redeemed) = programTotals[program.Id];

                earnedTotal += earned;
                donatedTotal += donated;
                redeemedTotal += redeemed;

                summaryReportData.Add(new object[]
                {
                    program.Name,
                    earned,
                    donated,
                    redeemed
                });
            }

            summaryReport.Data = summaryReportData;

            summaryReport.FooterRow = new object[]
            {
                "Total",
                earnedTotal,
                donatedTotal,
                redeemedTotal
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
