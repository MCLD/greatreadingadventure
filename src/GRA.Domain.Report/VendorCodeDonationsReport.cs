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
    [ReportInformation(15,
        "Vendor Code Donations Report",
        "Vendor prizes by program filterable by system.",
        "Participants")]
    public class VendorCodeDonationsReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;

        public VendorCodeDonationsReport(ILogger<VendorCodeDonationsReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            ISystemRepository systemRepository,
            IUserRepository userRepository,
            IVendorCodeRepository vendorCodeRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentException(nameof(branchRepository));
            _programRepository = programRepository
                ?? throw new ArgumentNullException(nameof(programRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentException(nameof(systemRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _vendorCodeRepository = vendorCodeRepository
                ?? throw new ArgumentException(nameof(vendorCodeRepository));
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

            string title = "";

            if (criterion.SystemId.HasValue)
            {
                title = (await _systemRepository.GetByIdAsync(criterion.SystemId.Value)).Name;
            }

            var report = new StoredReport
            {
                Title = title,
                AsOf = _serviceFacade.DateTimeProvider.Now
            };
            var reportData = new List<object[]>();
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            report.HeaderRow = new object[] {
                criterion.SystemId.HasValue ? "Branch Name" : "System Name"
            };

            var programs = await _programRepository.GetAllAsync(criterion.SiteId.Value);
            foreach (var program in programs)
            {
                report.HeaderRow = report.HeaderRow.Append(program.Name);
            }

            report.HeaderRow = report.HeaderRow.Append("Total");

            int count = 0;

            var users = await _userRepository.GetUsersByCriterionAsync(criterion);

            if (criterion.SystemId.HasValue)
            {
                users = users.Where(_ => _.SystemId == criterion.SystemId);
                var branches = await _branchRepository.GetBySystemAsync(criterion.SystemId.Value);
                foreach (var branch in branches)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    UpdateProgress(progress,
                        ++count * 100 / branches.Count(),
                        $"Processing: {branch.SystemName} - {branch.Name}",
                        request.Name);

                    var branchUsers = users.Where(_ => _.BranchId == branch.Id);
                    IEnumerable<object> row = new object[]
                    {
                        branch.Name
                    };
                    foreach (var program in programs)
                    {
                        row = row.Append(branchUsers.Count(_ => _.ProgramId == program.Id));
                    }
                    row = row.Append(branchUsers.Count());
                    reportData.Add(row.ToArray());
                }
            }
            else
            {
                var systems = await _systemRepository.GetAllAsync(criterion.SiteId.Value);
                foreach (var system in systems)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    UpdateProgress(progress,
                        ++count * 100 / systems.Count(),
                        $"Processing: {system.Name}",
                        request.Name);

                    var systemUsers = users.Where(_ => _.SystemId == system.Id);
                    IEnumerable<object> row = new object[]
                    {
                        system.Name
                    };
                    foreach (var program in programs)
                    {
                        row = row.Append(systemUsers.Count(_ => _.ProgramId == program.Id));
                    }
                    row = row.Append(systemUsers.Count());
                    reportData.Add(row.ToArray());
                }
            }

            report.Data = reportData.ToArray();

            IEnumerable<object> footerRow = new object[]
            {
                "Total"
            };
            foreach (var program in programs)
            {
                footerRow = footerRow.Append(users.Count(_ => _.ProgramId == program.Id));
            }
            footerRow = footerRow.Append(users.Count());

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
