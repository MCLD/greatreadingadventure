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
    [ReportInformation(13,
        "Vendor Code Report",
        "Select a system or for all see vendor prizes earned and ordered. Includes deleted participants!",
        "Participants")]
    public class VendorCodeReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;

        public VendorCodeReport(ILogger<TopScoresReport> logger,
            Domain.Report.ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            ISystemRepository systemRepository,
            IVendorCodeRepository vendorCodeRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentException(nameof(branchRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentException(nameof(systemRepository));
            _vendorCodeRepository = vendorCodeRepository
                ?? throw new ArgumentException(nameof(vendorCodeRepository));
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

            var askIfFirstTime
               = await GetSiteSettingBoolAsync(criterion, SiteSettingKey.Users.AskIfFirstTime);
            #endregion Reporting initialization

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...", request.Name);

            // header row
            var headerRow = new List<object>
            {
                "System Name",
                "Branch Name"
            };

            // first time?

            if (askIfFirstTime)
            {
                headerRow.Add("First Time # Earned");
                headerRow.Add("First Time # Ordered");
            }

            headerRow.Add("# Earned");
            headerRow.Add("# Ordered");

            report.HeaderRow = headerRow.ToArray();

            int count = 0;

            // running totals
            int totalEarned = 0;
            int totalOrdered = 0;

            int totalFirstEarned = 0;
            int totalFirstOrdered = 0;

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

                    var row = new List<object> {
                        branch.SystemName,
                        branch.Name,
                    };

                    if (askIfFirstTime)
                    {
                        criterion.IsFirstTimeParticipant = true;
                        var firstVendorCodes =
                            await _vendorCodeRepository.GetEarnedCodesAsync(criterion);
                        int firstEarnedCodes = firstVendorCodes.Count;
                        int firstUsedCodes = firstVendorCodes.Count(_ => _.IsUsed);

                        criterion.IsFirstTimeParticipant = false;

                        row.Add(firstEarnedCodes);
                        row.Add(firstUsedCodes);

                        totalFirstEarned += firstEarnedCodes;
                        totalFirstOrdered += firstUsedCodes;
                    }

                    var vendorCodes = await _vendorCodeRepository.GetEarnedCodesAsync(criterion);
                    int earnedCodes = vendorCodes.Count;
                    int usedCodes = vendorCodes.Count(_ => _.IsUsed);

                    totalEarned += earnedCodes;
                    totalOrdered += usedCodes;

                    row.Add(earnedCodes);
                    row.Add(usedCodes);

                    reportData.Add(row.ToArray());

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }

            report.Data = reportData.ToArray();

            // total row
            var footerRow = new List<object>
            {
                "Total",
                string.Empty
            };

            if (askIfFirstTime)
            {
                footerRow.Add(totalFirstEarned);
                footerRow.Add(totalFirstOrdered);
            }

            footerRow.Add(totalEarned);
            footerRow.Add(totalOrdered);

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
